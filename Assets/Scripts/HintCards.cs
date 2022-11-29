/*
 * HintCards.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintCards : MonoBehaviour
{
    private int _clickIndex;
    private Game _data;
    private int puzzleIndex;
    public GameObject adMessage;

    public GameObject message;
    public Text keyCount;


    // Start is called before the first frame update
    void Awake()
    {
        _clickIndex = 0;
        _data = GameObject.Find("GameData").GetComponent<Game>();
        puzzleIndex = transform.parent.GetSiblingIndex();
        if(_data.usedKeys[puzzleIndex] > 0)
        {
            RemoveLock();
        }
        keyCount.text = _data.hintKeys.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(!this.gameObject.activeSelf && _clickIndex != 0)
        {
            _clickIndex = 0;
        }
        if(keyCount.text != _data.hintKeys.ToString())
        {
            keyCount.text = _data.hintKeys.ToString();
        }
    }

    //Function of the next button in the hint cards to go to the next hint card
    public void Next()
    {
        //if the user has enough keys or has already unlocked the hint card, they can move on to the next hint card
        if(_clickIndex < _data.usedKeys[puzzleIndex])
        {
            this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(false);
            _clickIndex++;
            this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(true);
            if(_clickIndex < _data.usedKeys[puzzleIndex])
            {
                RemoveLock();
            }
        }
        else if(_data.hintKeys > 0)
        {
            message.SetActive(true);
        }
        else
        {
            adMessage.SetActive(true);
        }
    }

    //Function of the previous button in the monster overview to go to the previous monster
    public void Previous()
    {
        this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(false);
        _clickIndex--;
        this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(true);
    }
    
    //function for the yes button, to unlock the next hint card
    public void Yes()
    {
        message.SetActive(false);

        _data.KeyDecrease();
        _data.ChangeUsedKeys(puzzleIndex);
        keyCount.text = _data.hintKeys.ToString();
        RemoveLock();
    }

    //function to close the message 
    public void No()
    {
        message.SetActive(false);
    }

    //function to remove the lock on the next button
    private void RemoveLock()
    {
        int index = this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).childCount - 1;
        this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).GetChild(index).GetChild(0).gameObject.SetActive(false);

    }

    //Function to close the ad message
    public void CloseAdMessage()
    {
        adMessage.SetActive(false);
    }
}
