﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour {

    public Text nameLabel;
    public Image iconImage;
    public string itemId;
    private Item item;
    private ObjectScrollList scrollList;
	// Use this for initialization
	void Start () {
		
	}

    public void Setup(Item currentItem, ObjectScrollList currentScrollList)
    {
        item = currentItem;
        nameLabel.text = item.itemName;
        iconImage.sprite = item.icon;
        itemId = item.itemId;
        scrollList = currentScrollList;
    }
	

}
