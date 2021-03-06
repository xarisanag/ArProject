﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class VuMarkEvent : MonoBehaviour
{

	public List<GameObject> modelList;
    public List<string> modelIdList;
   

    private int modelN;
    private Vuforia.VuMarkManager vuMarkManager;
    private string targetFound = null;
    #region PRIVATE_MEMBER_VARIABLES



    #endregion // PRIVATE_MEMBER_VARIABLES

    public delegate void OnUpdatingSideItem();
    public static OnUpdatingSideItem sideItemDelegate;


    public GameObject canvas;
    public GameObject finishedGameDialog;
    public GameObject sidePanelUI;
    public GameObject gameOverDialog;
    public Text triesText;
    public GameObject wrongChoiceDialog;

    int countTries = 3;
    

    private bool stateCanvas = false;
    public event Action ScoChange;
    private FoundObjectScrollList p;
    private CatalogPbjectScrollList catalogPbjectScroll;

    public void Awake()
    {
        //Set onclicklistener for buttons in item prefab
        ButtonManager.onClickItem -= ButtonManager_onClickItem;
        ButtonManager.onClickItem += ButtonManager_onClickItem;
        DontDestroyOnLoad(canvas);
    }

    private void ButtonManager_onClickItem(ButtonManager obj)
    {
        //onClick event get text from item and render object if exists
        SetActiveObject(obj.itemNum.text);
      
    }

    void Start () {

        triesText.text = "Προσπάθειες : "+countTries;
        p = (FoundObjectScrollList)FindObjectOfType(typeof(FoundObjectScrollList));
        catalogPbjectScroll = (CatalogPbjectScrollList)FindObjectOfType(typeof(CatalogPbjectScrollList));
        // Set VuMarkManager
      
            vuMarkManager = TrackerManager.Instance.GetStateManager().GetVuMarkManager();
            // Set VuMark detected and lost behavior methods

            vuMarkManager.RegisterVuMarkDetectedCallback(onVuMarkDetected);
            vuMarkManager.RegisterVuMarkLostCallback(onVuMarkLost);
       
        
       
        // Deactivate all models 
        foreach (GameObject item in modelList){
			item.SetActive (false);
		}
	}
  

    void Update () {
        	foreach (var vmb in vuMarkManager.GetActiveBehaviours()) {
                    Debug.Log ("ID: "+ getVuMarkID(vmb.VuMarkTarget));
                }
        
        


    }

	private string getVuMarkID(VuMarkTarget vuMark){
		switch (vuMark.InstanceId.DataType){
		case InstanceIdType.BYTES:
			return vuMark.InstanceId.HexStringValue;
		case InstanceIdType.STRING:
			return vuMark.InstanceId.StringValue;
		case InstanceIdType.NUMERIC:
			return vuMark.InstanceId.NumericValue.ToString();
		}

		return null;
	}

    public void onVuMarkDetected(VuMarkTarget target){

        targetFound = getVuMarkID(target);
         // Enable canvas objects
          for (int i = 0; i < modelIdList.Count; i++)
          {
              if (modelIdList[i].Equals(targetFound)  )
              {
                if(p.GetItemState(targetFound) == false)
                  {
                    canvas.SetActive(true);
                }
                  else
                  {
                    canvas.SetActive(false);
                    modelList[i].SetActive(true);
                  }
              }
          }
    }

    private void onVuMarkLost(VuMarkTarget target){

        canvas.SetActive(false);
       
        String tartgLost = getVuMarkID(target);

        // Deactivate model by model number
        for (int i = 0; i < modelIdList.Count; i++)
        {
            if (modelIdList[i].Equals(tartgLost)) {
                modelList[i].SetActive(false);
            }
        }

    }
  

    public void SetActiveObject(string selectedItem)
    {
        bool found = false;
        if (targetFound != null)
        {

            for (int i = 0; i < modelIdList.Count; i++)
            {
                string s2 = modelIdList[i];

                if (selectedItem.Equals(s2) && selectedItem.Equals(targetFound) )
                {
                    canvas.SetActive(false);
                    modelList[i].SetActive(true);

                    // Set model number
                    modelN = i;
                   
                    //change state of checklist for sidepanel of objects that have been found
                    for (int j = 0; j < ObjectScrollList.cheeckedList.Count; j++){

                        if (ObjectScrollList.cheeckedList[j].itemId.Equals(selectedItem)){
                            ObjectScrollList.cheeckedList[j].setState(true);
                            found = true;
                            p.updateItem(selectedItem);
                            catalogPbjectScroll.UpdateList();
                            if (p.gameover()==true){
                                finishedGameDialog.SetActive(true);
                                sidePanelUI.SetActive(false);
                                ObjectScrollList.cheeckedList = new List<Item>();
                                vuMarkManager.UnregisterVuMarkDetectedCallback(onVuMarkDetected);
                                vuMarkManager.UnregisterVuMarkLostCallback(onVuMarkLost);
                                //TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
                            }
                        } 
                    }
                }
            }
        }
        if(found == false)
        {
              if(countTries > 0)
              {
                canvas.SetActive(false);
                wrongChoiceDialog.SetActive(true);
                  countTries--;
                  triesText.text = "Προσπάθειες : " + countTries ;
              }
              else
              {
                triesText.text = "Προσπάθειες : " + 0 ;
                canvas.SetActive(false);
                sidePanelUI.SetActive(false);
                gameOverDialog.SetActive(true);
                vuMarkManager.UnregisterVuMarkDetectedCallback(onVuMarkDetected);
                vuMarkManager.UnregisterVuMarkLostCallback(onVuMarkLost);
              }
        }
    }
    public void showGUI(bool showgui){
        if(showgui==false){
            canvas.SetActive(false);
        }
        else{
            canvas.SetActive(true);
        }
    }  
}

