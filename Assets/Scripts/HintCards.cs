using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintCards : MonoBehaviour
{
    private int _clickIndex;
    private Game _data;
    private int puzzleIndex;

    public GameObject message;
    // Start is called before the first frame update
    void Awake()
    {
        _clickIndex = 0;
        _data = GameObject.Find("GameData").GetComponent<Game>();
        puzzleIndex = transform.parent.GetSiblingIndex();
        if(_data.usedKeys[puzzleIndex] > 0)
        {
            SetLock(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!this.gameObject.activeSelf && _clickIndex != 0)
        {
            _clickIndex = 0;
        }
    }
    //Function of the next button in the monster overview to go to the next monster
    public void Next()
    {
        this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(false);
        if(_clickIndex < _data.usedKeys[puzzleIndex])
        {
            _clickIndex++;
            this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(true);
            if(_clickIndex < _data.usedKeys[puzzleIndex])
            {
                SetLock(false);
            }
        }
        else if(_data.hintKeys > 0)
        {
            message.SetActive(true);
            _clickIndex++;
            this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(true);
        }
    }

    //Function of the previous button in the monster overview to go to the previous monster
    public void Previous()
    {
        this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(false);
        _clickIndex--;
        this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).gameObject.SetActive(true);
    }

    public void Yes()
    {
        message.SetActive(false);

        _data.hintKeys--;
        _data.usedKeys[puzzleIndex]++;
        SetLock(false);
    }

    public void No()
    {
        message.SetActive(false);
    }

    private void SetLock(bool active)
    {
        int index = this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).childCount - 1;
        this.transform.GetChild(0).GetChild(0).GetChild(_clickIndex).GetChild(index).GetChild(0).gameObject.SetActive(active);

    }
}
