using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardView view; // カードの見た目の処理
    public CardModel model; // カードのデータを処理
    public CardMovement movement;  // 移動(movement)に関することを操作

    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardID, bool isPlayerCard) // カードを生成した時に呼ばれる関数
    {
        model = new CardModel(cardID, isPlayerCard); // カードデータを生成
        view.Show(model); // 表示
    }

    public void DestroyCard(CardController card)
    {
        Destroy(card.gameObject);
    }
 
    public void DropField()
    {
        GameManager.instance.ReduceManaPoint(model.cost);
        model.isField = true; // フィールドのカードのフラグを立てる
        model.canUse = false; // 使えるカードのフラグを消す

        if(model.addNode.Count > 0)
        {
            if(model.addNode.IndexOf(1) >= 0)
            {
                Debug.Log("速攻");
                model.canAttack = true;
            }
        }
        
        //canUsePanelとcanAttackPanelを現在の状態に適応する
        view.SetCanUsePanel(model.canUse);
        view.SetCanAttackPanel(model.canAttack);
    }
}