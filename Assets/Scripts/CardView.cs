using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text nameText, loreText, powerText, costText;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject canAttackPanel, canUsePanel;

    // これがevent
    public event Action<int> OnPointerCard;
    public event Action OnPointerExtisCard;
    
    public int cardID;

    public void Show(CardModel cardModel) // cardModelのデータ取得と反映
    {
        cardID = cardModel.modelID;
        nameText.text = cardModel.name;
        loreText.text = cardModel.lore;
        powerText.text = cardModel.power.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
    }

    public void SetCanAttackPanel(bool flag) // フラグに合わせてCanAttackPanelを付けるor消す
    {
        canAttackPanel.SetActive(flag);
    }
 
    public void SetCanUsePanel(bool flag) // フラグに合わせてCanUsePanelを付けるor消す
    {
        canUsePanel.SetActive(flag);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("あああ"+cardID);
        //OnPointerCard?.Invoke(cardID,nameText.text,loreText.text,powerText.text,costText.text,iconImage.sprite); // マウスオーバーしたのでカードIDを発火
    	OnPointerCard?.Invoke(cardID); // マウスオーバーしたのでカードIDを発火
	}

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExtisCard?.Invoke();
    }
}