using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIManager uIManager;

    [SerializeField] CardController cardPrefab;
    [SerializeField] Transform playerLeader, playerHand, playerField, enemyField, enemyHand;
    [SerializeField] Text playerLeaderHPText;
    [SerializeField] Text enemyLeaderHPText;
    [SerializeField] Text playerManaPointText;
    [SerializeField] Text playerDefaultManaPointText;

    [SerializeField] List<int> playerDeck, enemyDeck;

    public bool isPlayerTurn = true; // プレイヤーのターンから始まる, 全体から参照可能
    public bool isGameTime = true; // ゲームを進行しても構わないかどうかをここで設定, 勝利時にオフ
    public bool isWin = false; // 勝利フェイズに入ったかどうかをここで判断
    public bool isPlayerWin = true; // どっちが勝ったのか？

    public int playerManaPoint; // 使用すると減るマナポイント
    public int playerDefaultManaPoint; // 毎ターン増えていくベースのマナポイント
    public int playerLeaderHP; // プレイヤーのHP
    public int enemyLeaderHP; // 敵のHP

    public Transform firstCard;

    public static GameManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start() // 読み込み時のみ
    {
        StartGame();
    }
    void StartGame() // 初期値の設定 
    {
        playerDeck = new List<int>()
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
        playerDeck = playerDeck.OrderBy ( a => Guid.NewGuid () ).ToList ();
        enemyDeck = new List<int>()
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
        enemyDeck = enemyDeck.OrderBy ( a => Guid.NewGuid () ).ToList ();

        enemyLeaderHP = 20000;
        playerLeaderHP = 20000;
        ShowLeaderHP();
 
        // マナの初期値設定
        playerManaPoint = 1;
        playerDefaultManaPoint = 1;
        ShowManaPoint();
 
        // 初期手札を配る
        SetStartHand();
 
        // ターンの決定
        StartCoroutine(TurnCalc()); // ターンを相手に回す
    }
    void CreateCard(int cardID, Transform place) // カードを任意の領域に作成
    {
        CardController card = Instantiate(cardPrefab, place);
 
        // Playerの手札かプレイヤーのフィールドに生成されたカードはPlayerのカードとする
        if (place == playerHand || place == playerField)
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
    }
    void DrawCard(Transform hand, List<int> deck) // カードを引く
    {
        // デッキがないなら引かない
        if (deck.Count == 0)
        {
            return;
        }

        CardController[] handCardList = hand.GetComponentsInChildren<CardController>();

        if (handCardList.Length < 9)
        {
            // デッキの一番上のカードを抜き取り、手札に加える
            int cardID = deck[0];
            deck.RemoveAt(0);
            CreateCard(cardID, hand);
        }
 
        SetCanUsePanelHand();
    }
    void SetStartHand() // 手札を3枚配る
    {
        for (int i = 0; i < 3; i++)
        {
            // おたがいに初期手札を三枚ずつ配る
            DrawCard(playerHand, playerDeck);
            DrawCard(enemyHand, enemyDeck);
        }
    }
    IEnumerator TurnCalc() // ターンを管理する
    {
        yield return StartCoroutine(uIManager.ShowChangeTurnPanel());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            //EnemyTurn(); // コメントアウトする
            StartCoroutine(EnemyTurn()); // StartCoroutineで呼び出す
        }
    }
    public void ChangeTurn() // ターンエンド処理
    {
        isPlayerTurn = !isPlayerTurn; // ターンを逆にする
        StartCoroutine(TurnCalc()); // ターンを相手に回す
    }
    public void ChangeTurnButton() // ターンエンドボタンにつける処理
    {
        if (isPlayerTurn)
        {
            ChangeTurn(); // ターンを相手に回す
        }
    }
    void PlayerTurn() // プレイヤーのターン
    {
        Debug.Log("Playerのターン");
 
        CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(playerFieldCardList, true);

        // マナを増やす
        playerDefaultManaPoint++;
        playerManaPoint = playerDefaultManaPoint;
        ShowManaPoint();

        DrawCard(playerHand, playerDeck); // 自分のデッキから手札にカードを一枚加える
    }
    IEnumerator EnemyTurn() // 敵のターン
    {
        Debug.Log("Enemyのターン");

        CardController[] enemyHandCardList = enemyHand.GetComponentsInChildren<CardController>();
        CardController[] enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();

        yield return new WaitForSeconds(0.4f);

        // 敵のフィールドのカードを攻撃可能にして、緑の枠を付ける
        SetAttackableFieldCard(enemyFieldCardList,true);

        yield return new WaitForSeconds(0.4f);

        DrawCard(enemyHand, enemyDeck); // 自分のデッキから手札にカードを一枚加える

        enemyHandCardList = enemyHand.GetComponentsInChildren<CardController>();
        enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();
        
        if (enemyHandCardList.Length > 0)
        {
            this.firstCard = enemyHand.gameObject.transform.GetChild(0);
        }

        while (enemyFieldCardList.Length < 7)
        {
            // デッキがないなら引かない
            if (enemyHandCardList.Length > 0)
            {
                this.firstCard.gameObject.transform.SetParent(enemyField, false); // カードの親をフィールドに変える
				//this.firstCard.DropField()
                //firstCard.gameObject.movement.cardParent = enemyField.transform; // カードの親要素を自分（アタッチされてるオブジェクト）にする 今回の書き換え部分
                
                enemyHandCardList = enemyHand.GetComponentsInChildren<CardController>();
                enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();
        
                if (enemyHandCardList.Length > 0)
                {
                    this.firstCard = enemyHand.gameObject.transform.GetChild(0);
                }
            }
            else
            {
                break;
            }
        }
 
        CardController[] enemyFieldCardListSecond = enemyField.GetComponentsInChildren<CardController>();

        yield return new WaitForSeconds(0.4f);
 
        while (Array.Exists(enemyFieldCardListSecond, card => card.model.canAttack))
        {
            // 攻撃可能カードを取得
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyFieldCardListSecond, card => card.model.canAttack);
            CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();
 
            CardController attackCard = enemyCanAttackCardList[UnityEngine.Random.Range(0, enemyCanAttackCardList.Length)];

            // 攻撃力において下回るか等しいカードのみを対象に取るようにフィルタ処理を行う
            CardController[] playerFieldCardListAttackable = Array.FindAll(playerFieldCardList, card => card.model.power < attackCard.model.power);
            CardController[] playerFieldCardListAttackableEqual = Array.FindAll(playerFieldCardList, card => card.model.power == attackCard.model.power);
 
            //AttackToLeader(attackCard, false); // コメントアウトする
 
            if (playerFieldCardListAttackable.Length > 0) // プレイヤーの場にノーリスクで破壊できるカードがある場合
            {
                CardController defenceCard = playerFieldCardListAttackable[UnityEngine.Random.Range(0, playerFieldCardListAttackable.Length)];
                yield return StartCoroutine (attackCard.movement.AttackMotion(defenceCard.transform));
                CardBattle(attackCard, defenceCard);
            }
            else if (playerFieldCardListAttackableEqual.Length > 0) // プレイヤーの場に両破壊になるカードがある場合
            {
                CardController defenceCard = playerFieldCardListAttackableEqual[UnityEngine.Random.Range(0, playerFieldCardListAttackableEqual.Length)];
                yield return StartCoroutine (attackCard.movement.AttackMotion(defenceCard.transform));
                CardBattle(attackCard, defenceCard);
            }
            else // プレイヤーの場にカードがない場合
            {
                yield return StartCoroutine (attackCard.movement.AttackMotion(playerLeader.transform));
                AttackToLeader(attackCard, false);
            }

            yield return new WaitForSeconds(0.4f);
 
            // カード配列を更新　これなくていいよね？
            //enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();
        }

        ChangeTurn(); // ターンエンドする
    }
    public void CardBattle(CardController attackCard, CardController defenceCard) // カード同士の戦闘
    {
        // 攻撃カードと攻撃されるカードが同じプレイヤーのカードならバトルしない
        if (attackCard.model.isPlayer == defenceCard.model.isPlayer)
        {
            return;
        }

        // 攻撃側のパワーが高かった場合、攻撃されたカードを破壊する
        if (attackCard.model.power > defenceCard.model.power)
        {
            defenceCard.DestroyCard(defenceCard);
        }
 
        // 攻撃された側のパワーが高かった場合、攻撃側のカードを破壊する
        if (attackCard.model.power < defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
        }
 
        // パワーが同じだった場合、両方のカードを破壊する
        if (attackCard.model.power == defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
            defenceCard.DestroyCard(defenceCard);
        }
 
        attackCard.model.canAttack = false;
        attackCard.view.SetCanAttackPanel(false);
    }
    void SetAttackableFieldCard(CardController[] cardList, bool canAttack) // リストの攻撃許可を一括でする
    {
        foreach (CardController card in cardList)
        {
            card.model.canAttack = canAttack;
            card.view.SetCanAttackPanel(canAttack);
        }
    }
    public void AttackToLeader(CardController attackCard, bool isPlayerCard) // リーダーに攻撃し、勝利判定
    {
        if (attackCard.model.isPlayer == true) // attackCardがプレイヤーのカードなら
        {
            enemyLeaderHP -= attackCard.model.power; // 敵のリーダーのHPを減らす
        }
        else // attackCardが敵のカードなら
        {
            playerLeaderHP -= attackCard.model.power; // プレイヤーのリーダーのHPを減らす
        }

        if (playerLeaderHP <= 0)
        {
            playerLeaderHP = 0;
            isGameTime = false;
            isPlayerWin = false;
            isWin = true;
        }
        if (enemyLeaderHP <= 0)
        {
            enemyLeaderHP = 0;
            isGameTime = false;
            isPlayerWin = true;
            isWin = true;
        }
 
        attackCard.model.canAttack = false;
        attackCard.view.SetCanAttackPanel(false);
        Debug.Log("敵のHPは、"+enemyLeaderHP+" あなたのHPは、"+playerLeaderHP);
        ShowLeaderHP();

        if(isWin==true)
        {
            StartCoroutine(BattleResultCalc());
        };
    }
    public void ShowLeaderHP() // お互いのリーダーのHPを表示する
    {
        playerLeaderHPText.text = playerLeaderHP.ToString();
        enemyLeaderHPText.text = enemyLeaderHP.ToString();
    }
    void ShowManaPoint() // マナポイントを表示するメソッド
    {
        playerManaPointText.text = playerManaPoint.ToString();
        playerDefaultManaPointText.text = playerDefaultManaPoint.ToString();
    }
    public void ReduceManaPoint(int cost) // コストの分、マナポイントを減らす
    {
        playerManaPoint -= cost;
        ShowManaPoint();
 
        SetCanUsePanelHand();
    }
    void SetCanUsePanelHand() // 手札のカードを取得して、使用可能なカードにCanUseパネルを付ける
    {        
        CardController[] playerHandCardList = playerHand.GetComponentsInChildren<CardController>();
        foreach (CardController card in playerHandCardList)
        {
            if (card.model.cost <= playerManaPoint)
            {
                card.model.canUse = true;
                card.view.SetCanUsePanel(card.model.canUse);
            }
            else
            {
                card.model.canUse = false;
                card.view.SetCanUsePanel(card.model.canUse);
            }
        }
    }
    public bool checkCardAvailability(CardController card) // 動かしていいカードかどうか bool 値で返す
    {
        bool able = true; // カードを動かせるかな？
 
        if (card.model.isField == false) // 手札のカードなら
        {
            if (card.model.canUse == false) // マナコストより少ないカードは動かせない
            {
                able = false;
            }
        }
        else
        {
            if (card.model.canAttack == false) // 攻撃不可能なカードは動かせない
            {
                able = false;
            }
        }

        if (card.model.isPlayer == false) // 敵のカードは動かせない
        {
            able = false;
        }

        if (isPlayerTurn == false) // 敵のターン中は動かせない
        {
            able = false;
        }

        return able;
    }
    public bool checkCardAttackability(CardController card) // 動かしていいカードかどうか bool 値で返す
    {
        bool able = true; // カードを動かせるかな？

        if (card.model.canAttack == false) // 攻撃不可能なカードは動かせない
        {
            able = false;
        }

        if (card.model.isPlayer == false) // 敵のカードは動かせない
        {
            able = false;
        }

        if (isPlayerTurn == false) // 敵のターン中は動かせない
        {
            able = false;
        }

        return able;
    }
    IEnumerator BattleResultCalc() // 勝利、敗北を管理する
    {
        Debug.Log("かち");
        yield return StartCoroutine(uIManager.ShowChangeTurnPanel());
    }
    public bool FlipBool() // 0か1を返す
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

}
