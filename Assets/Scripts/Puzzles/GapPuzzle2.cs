/*
 * GapPuzzle2.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridField))]
public class GapPuzzle2 : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public GapPuzzleTutorial tutorial;

    //private variables
    private Color _colourEmpty;
    private Color _colourFull;
    private List<int> _tilesBlack;


    //Start is called before the first frame update
    //Sets up the colour variables
    void Start()
    {

        _colourEmpty = new Color(1, 0.8f, 0.65f);
        _colourFull = new Color(0.6f,0.34f,0.3f);
        SetUp();

    }

    //Sets up the tiles to the start state
    private void SetUp()
    {

        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            Destroy(tilemanager.transform.GetChild(i).gameObject);
        }
        _tilesBlack = new List<int>();
        float size = (grid.nodeRadius * 2) - 0.005f;

        foreach (Node node in grid.grid)
        {
            node.selected = false;
            tile.transform.localScale = new Vector3(size, size, 0);
            tile.GetComponent<SpriteRenderer>().color = _colourEmpty;
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);

        }
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for (int i = 0; i < tilemanager.transform.childCount; i++)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Rect rect = Object2Rect(tilemanager.transform.GetChild(i));


                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {

                    Node node = grid.GetNodeFromWorldPos(rect.center);
                    var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
                    if (rend.color == _colourEmpty)
                    {
                        rend.color = _colourFull;

                        node.selected = true;
                        _tilesBlack.Add(i);
                    }
                    else
                    {
                        rend.color = _colourEmpty;
                        _tilesBlack.Remove(i);
                        node.selected = false;
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
    private Rect Object2Rect(Transform tile)
    {

        Rect rect = new Rect(tile.transform.position.x - grid.nodeRadius, tile.transform.position.y - grid.nodeRadius, grid.nodeRadius * 2, grid.nodeRadius * 2);
        return rect;
    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        for (int i = 0; i < 9; i++)
        {
            int counter = 0;
            List<Node> selected = new List<Node>();
            for (int j = 0; j < 9; j++)
            {
                if (grid.grid[i, j].selected)
                {
                    foreach (Node neighbour in grid.GetNodeNeighboursDiagonal(grid.grid[i, j]))
                    {
                        if (neighbour.selected)
                        {
                            return false;
                        }
                    }
                    counter++;
                    selected.Add(grid.grid[i, j]);
                }
            }
            if (i == 3 && selected.Count == 2)
            {
                if (selected[0].gridY - selected[1].gridY != -7)
                {
                    return false;
                }
            }
            if (counter != 2)
            {
                return false;
            }
        }
        for (int i = 0; i < grid.GetGridSizeX(); i++)
        {
            int counter = 0;
            List<Node> selected = new List<Node>();
            for (int j = 0; j < grid.GetGridSizeX(); j++)
            {
                if (grid.grid[j, i].selected)
                {
                    foreach (Node neighbour in grid.GetNodeNeighboursDiagonal(grid.grid[j, i]))
                    {
                        if (neighbour.selected)
                        {
                            return false;
                        }
                    }
                    counter++;
                    selected.Add(grid.grid[j, i]);
                }
            }
            if (i == 2 && selected.Count == 2)
            {
                if (selected[0].gridX - selected[1].gridX != -3)
                {
                    return false;
                }
            }
            if (counter != 2)
            {
                return false;
            }
        }
        return true;
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
