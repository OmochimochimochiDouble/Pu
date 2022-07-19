using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
// フィールドにアタッチするクラス
public class DropPlace : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) // ドロップされた時に行う処理
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>(); // 今回の書き換え部分

        bool canDrag = GameManager.instance.checkCardAvailability(card);
 
        if (canDrag == true) // もし動かしてもいいカード、かつプレイヤーのカードがあれば、
        {
            card.movement.cardParent = this.transform; // カードの親要素を自分（アタッチされてるオブジェクト）にする 今回の書き換え部分
            card.DropField(); // カードをフィールドに置いた時の処理を行う
        }
    }
}