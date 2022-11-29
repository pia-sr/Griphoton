/*
 * LightsOut.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOut : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject book;
    public GameObject bookManager;

    //Arrays for the books which determine the start state of each book
    public int[] lightsRow1 = new int[5];
    public int[] lightsRow2 = new int[5];
    public int[] lightsRow3 = new int[5];
    public int[] lightsRow4 = new int[5];
    public int[] lightsRow5 = new int[5];

    //UI
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public LightsOutTutorial tutorial;

    //private 2D arrays for the current and start states of the books
    private int[,] originalLights;
    private int[,] currentLights;

    // Start is called before the first frame update
    void Start()
    {
        book.transform.localScale = new Vector3(grid.nodeRadius * 0.25f, grid.nodeRadius * 0.25f, 0);
        originalLights = new int[5, 5];
        int[] helperArray = new int[5];
        for(int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    helperArray = lightsRow5;
                    break;
                case 1:
                    helperArray = lightsRow4;
                    break;
                case 2:
                    helperArray = lightsRow3;
                    break;
                case 3:
                    helperArray = lightsRow2;
                    break;
                case 4:
                    helperArray = lightsRow1;
                    break;
            }
            for(int j = 0; j < 5; j++)
            {
                originalLights[j,i] = helperArray[j];
                Instantiate(book, grid.grid[i,j].worldPosition, Quaternion.identity, bookManager.transform);
            }
        }
        SetUp();
        
    }

    //Sets up the books to their start state
    private void SetUp()
    {
        currentLights = new int[5, 5];
        int counter = 0;
        foreach (Node node in grid.grid)
        {
            if (originalLights[node.gridX, node.gridY] == 1)
            {
                currentLights[node.gridX, node.gridY] = 1;
                node.selected = true;

            }
            else
            {
                currentLights[node.gridX, node.gridY] = 0;
                node.selected = false;
                bookManager.transform.GetChild(counter).GetComponent<BookAnimation>().CloseBook();
            }
            counter++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //waits for user's touch to select a book
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            foreach (Node node in grid.grid)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Rect rect = Node2Rect(node.worldPosition);
                
                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    //the selected book and its neighbours will either be open or close depending on their previous state
                    ChangeLight(node);
                    foreach(Node neighbour in grid.GetNodeNeighbours(node))
                    {
                        ChangeLight(neighbour);
                    }

                }
            }

            if (CheckWin() && !tutorial.inactive)
            {
                tutorial.inactive = true;
                tutorial.WonPuzzle();
            }

        }
    }
    
    
    //Function to create a rectagle on top of a given object to set rectangular boundaries for that object
    private Rect Node2Rect(Vector2 nodePos)
    {
        Rect rect = new Rect(nodePos.x - grid.nodeRadius, nodePos.y - grid.nodeRadius, grid.nodeRadius * 2f, grid.nodeRadius * 2);
        return rect;
    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        foreach(int light in currentLights)
        {
            if(light != 0)
            {
                return false;
            }
        }
        return true;
    }

    //Function to change the state of the selected book to either open or closed
    private void ChangeLight(Node node)
    {
        int bookIndex = 5 * node.gridX + node.gridY;
        if (node.selected)
        {
            node.selected = false;
            currentLights[node.gridX, node.gridY] = 0;
            bookManager.transform.GetChild(bookIndex).GetComponent<BookAnimation>().CloseBook();
        }
        else
        {
            node.selected = true;
            currentLights[node.gridX, node.gridY] = 1;
            bookManager.transform.GetChild(bookIndex).GetComponent<BookAnimation>().OpenBook();
        }
    }

    //Function of the restart button which sets the puzzle to its start state
    public void Restart()
    {
        if (!tutorial.inactive)
        {
            int counter = 0;
            foreach (Node node in grid.grid)
            {
                if (currentLights[node.gridX, node.gridY] == 0)
                {
                    node.selected = true;
                    bookManager.transform.GetChild(counter).GetComponent<BookAnimation>().OpenBook();

                }
                counter++;
            }
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
