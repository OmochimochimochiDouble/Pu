using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardZoom : MonoBehaviour {

    [SerializeField] Text nameText, loreText, powerText, costText;
    [SerializeField] Image icon;

    // マスターデータ（とりあえず名前だけ）
/*
    Dictionary<int, string> cardDatas = new Dictionary<int, string>() {
        { 0, "なし"},
        { 1, "がんなー" },
        { 2, "ふろすと" },
        { 3, "いぬ" }
    };
*/


    // Start is called before the first frame update
    void Update() {

        // Prefab生成をするならこのへん

        // 画面上にあるCardスクリプトを持つオブジェクトを取得
        var allCardInScreen = FindObjectsOfType<CardView>();
        
        // マウスオーバーイベントを購読
        foreach(CardView c in allCardInScreen) {
            c.OnPointerCard += cardID => {
				this.gameObject.SetActive(true);

        		CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card" + cardID);
 
        		nameText.text = cardEntity.name;
        		loreText.text = cardEntity.lore;
        		costText.text = cardEntity.cost.ToString();
        		powerText.text = cardEntity.power.ToString();
        		icon.sprite = cardEntity.icon;
            };

            c.OnPointerExtisCard += () => this.gameObject.SetActive(false);

        }

    }

}