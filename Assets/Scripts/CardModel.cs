using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 
public class CardModel
{
    public int modelID;
    public string name;
    public string lore;
    public int cost;
    public int power;
    public Sprite icon;
    public List<int> addNode;

    public bool canUse = true;
    public bool canAttack = false;
    public bool isPlayer = false;
    public bool isField = false;
 
    public CardModel(int cardID, bool isPlayerCard)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card" + cardID);
 
        modelID = cardEntity.cardID;
        name = cardEntity.name;
        lore = cardEntity.lore;
        cost = cardEntity.cost;
        power = cardEntity.power;
        icon = cardEntity.icon;
        addNode = cardEntity.addNode;
        isPlayer = isPlayerCard;

		/*
        if(addNode.Count > 0)
        {
            if(addNode.IndexOf(1) >= 0)
            {
                power = 5000;
            }
        }
		*/
    }
}