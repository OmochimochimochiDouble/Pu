using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "CardEntity", menuName = "Create CardEntity")]

public class CardEntity : ScriptableObject
{
    public int cardID;
    public new string name;
    public string lore;
    public int cost;
    public int power;
    public Sprite icon;
    public List<int> addNode;
}