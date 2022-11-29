/*
 * ZebraPuzzle.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZebraPuzzle : MonoBehaviour
{
    //public field objects
    public List<GameObject> houses;
    public GameObject houseManager;
    public GameObject rules;

    //UI
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public ZebraTutorial tutorial; 

    //private list and arrays for the rooftop colours
    private List<int> coloursCurrent;
    private int[] coloursSolution;
    private string[] colours;

    // Start is called before the first frame update
    void Start()
    {
        colours = new string[6] { "Nothing", "Green", "Yellow", "Blue", "Purple", "Pink" };
        coloursSolution = new int[] { 4, 1, 5, 3, 2};
        SetUp();
    }

    //Sets up the houses to their start state
    private void SetUp()
    {
        for(int i = 0; i < houseManager.transform.childCount; i++)
        {
            Destroy(houseManager.transform.GetChild(i).gameObject);
        }
        coloursCurrent = new List<int>() { 0,0,0,0,0 };
        for(int i = 0; i < 5; i++)
        {
            Instantiate(houses[0], new Vector3(-6.25f + (i * 3.125f), -1.3f,0), Quaternion.identity, houseManager.transform);
            houseManager.transform.GetChild(i).name = colours[0];
        }
    }


    
    // Update is called once per frame
    void Update()
    {
        //waits for user's touch to select a house
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for(int i = 0; i < 5; i++)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                Bounds bounds = Bounds(houseManager.transform.GetChild(i).position); 

                if (bounds.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    int indexPos = IndexPos(houseManager.transform.GetChild(i));
                    Vector3 pos;

                    //if there is no house yet, the moud field will be replaced by a house
                    if(coloursCurrent[indexPos] == 0)
                    {
                        pos = houseManager.transform.GetChild(i).position + new Vector3(0,1,0);
                        coloursCurrent[indexPos]++;

                    }
                    //if there is already a house, but the roof has the last colour in the colour list
                    //the house will be replaced by the moud field
                    else if(coloursCurrent[indexPos] == 5)
                    {
                        pos = houseManager.transform.GetChild(i).position + new Vector3(0,-1,0);
                        coloursCurrent[indexPos] = 0;

                    }
                    //the colour of the roof of the selected house will change
                    else
                    {
                        pos = houseManager.transform.GetChild(i).position;
                        coloursCurrent[indexPos]++;
                    }

                    Destroy(houseManager.transform.GetChild(i).gameObject);
                    Instantiate(houses[coloursCurrent[indexPos]], pos, Quaternion.identity, houseManager.transform);
                }

            }

            if (CheckWin() && !tutorial.inactive)
            {
                tutorial.inactive = true;
                tutorial.WonPuzzle();
            }

        }
    }
    
    //Function to create specific boundaries for a given vector
    private Bounds Bounds(Vector3 pos)
    {
        Bounds bounds = new Bounds(pos, new Vector3(3f, 3f, 0));
        return bounds;
    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        int counter = 0;
        for(int i = 0; i < coloursCurrent.Count; i++)
        {
            if(coloursCurrent[i] == 0)
            {
                return false;
            }
            else if(coloursCurrent[i] == coloursSolution[i])
            {
                counter++;
            }
        }
        if(counter == 5)
        {
            return true;
        }
        return false;
    }

    //function to return the position of the given object
    private int IndexPos(Transform obj)
    {
        int index = 0;
        for (int i = 0; i < 5; i++)
        {
            float pos = -6.25f + (i * 3.125f);
            if (obj.position.x == pos)
            {
                index = i;
            }
        }
        return index;
    }

    //function for the button to open the rules of the puzzle
    public void OpenRules()
    {
        if (!tutorial.inactive)
        {
            rules.SetActive(true);
            tutorial.inactive = true;

        }
    }

    //function for the button to close the rules of the puzzles
    public void CloseRules()
    {
        rules.SetActive(false);
        tutorial.inactive = false;
    }
    
    //Function for the restart button
    public void restart()
    {
        if (!tutorial.inactive)
        {
            SetUp();
        }
    }

    //Function for the leave button
    //Open up a panel to check if the user really wants to leave
    public void leave()
    {
        if (!tutorial.inactive)
        {
            tutorial.inactive = true;
            message.SetActive(true);
        }

    }

    //Function for the yes button, if the user really wants to leave
    public void yes()
    {
        SetUp();
        this.transform.parent.transform.parent.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(true);
        griphoton.SetActive(true);
        player.SetActive(true);
        player.GetComponent<Player>().SwitchCams();
        player.GetComponent<Player>().Unpause();
        message.SetActive(false);
    }

    //Function for the no button, if the user does not want to leave
    public void no()
    {
        tutorial.inactive = false;
        message.SetActive(false);
    }
}
