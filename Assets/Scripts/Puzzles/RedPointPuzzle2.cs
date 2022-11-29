/*
 * RedPointPuzzle2.cs
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
public class RedPointPuzzle2 : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject circle;
    public GameObject circleManager;
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public RedPointTutorial tutorial;

    //private variables
    private List<Node> _selectedDots;
    private List<Node> _dots;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
        foreach (Node node in _dots)
        {

            circle.transform.localScale = new Vector3(grid.nodeRadius * 1.5f, grid.nodeRadius * 1.5f, 0);
            circle.GetComponent<SpriteRenderer>().color = Color.black;
            Instantiate(circle, node.worldPosition, Quaternion.identity, circleManager.transform);
        }
    }

    //Function to set up the points to their original state
    private void SetUp()
    {
        _selectedDots = new List<Node>();
        _dots = new List<Node>()
        {
            grid.grid[0,0],
            grid.grid[1,0],
            grid.grid[3,0],
            grid.grid[2,0],
            grid.grid[0,1],
            grid.grid[4,1],
            grid.grid[3,2],
            grid.grid[0,3],
            grid.grid[3,3],
            grid.grid[4,3],
            grid.grid[0,4],
            grid.grid[3,4]
        };
        for (int i = 0; i < circleManager.transform.childCount; i++)
        {
            circleManager.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.black;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for(int i = 0; i < circleManager.transform.childCount; i++)
            {

                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Rect rect = Object2Rect(circleManager.transform.GetChild(i));
                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    GameObject currentDot = circleManager.transform.GetChild(i).gameObject;
                    if(currentDot.GetComponent<SpriteRenderer>().color == Color.black)
                    {
                        currentDot.GetComponent<SpriteRenderer>().color = Color.red;
                        foreach(Node node in _dots)
                        {
                            if(grid.GetNodeFromWorldPos(currentDot.transform.position) == node)
                            {
                                _selectedDots.Add(node);
                            }
                        }
                    }
                    else
                    {
                        Node node2Remove = null;
                        currentDot.GetComponent<SpriteRenderer>().color = Color.black;
                        foreach (Node node in _selectedDots)
                        {
                            if (grid.GetNodeFromWorldPos(currentDot.transform.position) == node)
                            {
                                node2Remove = node;
                            }
                        }
                        if(node2Remove != null)
                        {
                            _selectedDots.Remove(node2Remove);
                        }
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
        Rect rect = new Rect(tile.transform.position.x - (grid.nodeRadius * 0.75f), tile.transform.position.y - (grid.nodeRadius * 0.75f), grid.nodeRadius * 1.5f, grid.nodeRadius * 1.5f);
        return rect;
    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        List<float> distances = new List<float>();
        if(_selectedDots.Count == 5)
        {
            foreach(Node node in _selectedDots)
            {
                foreach(Node nextDot in _selectedDots)
                {
                    if(nextDot != node)
                    {
                        float dist = CalcDist(node, nextDot);
                        if (!distances.Contains(dist))
                        {
                            distances.Add(dist);
                        }
                    }
                }
            }
        }
        if(distances.Count == 10)
        {
            return true;
        }
        return false;
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

    private float CalcDist(Node node1, Node node2)
    {
        float dis;
        if (node1.gridX == node2.gridX)
        {
            dis = Mathf.Abs(node1.gridY - node2.gridY);
        }
        else if (node1.gridY == node2.gridY)
        {
            dis = Mathf.Abs(node1.gridX - node2.gridX);
        }
        else
        {
            dis = Mathf.Pow(node1.gridX - node2.gridX, 2) + Mathf.Pow(node1.gridY - node2.gridY, 2);
            dis = Mathf.Sqrt(dis);
        }

        return dis;
    }
}
