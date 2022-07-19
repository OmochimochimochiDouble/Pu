using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public Transform cardParent;
    bool canDrag = true; // カードを動かせるかどうかのフラグ
 
    public void OnBeginDrag(PointerEventData eventData) // ドラッグを始めるときに行う処理
    {
        CardController card = GetComponent<CardController>();

        cardParent = transform.parent;

        canDrag = GameManager.instance.checkCardAvailability(card);
 
        if (canDrag == false)
        {
            return;
        }

        transform.SetParent(cardParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false; // blocksRaycastsをオフにする
    }
 
    public void OnDrag(PointerEventData eventData) // ドラッグした時に起こす処理
    {
        if (canDrag == false)
        {
            return;
        }

        transform.position = eventData.position;
    }
 
    public void OnEndDrag(PointerEventData eventData) // カードを離したときに行う処理
    {
        if (canDrag == false)
        {
            return;
        }
        
        transform.SetParent(cardParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true; // blocksRaycastsをオンにする
    }

    public IEnumerator AttackMotion(Transform target)
    {
        Vector3 currentPosition = transform.position;
        cardParent = transform.parent;
 
        transform.SetParent(cardParent.parent); // cardの親を一時的にCanvasにする
 
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);
 
        transform.SetParent(cardParent); // cardの親を元に戻す
    }
}