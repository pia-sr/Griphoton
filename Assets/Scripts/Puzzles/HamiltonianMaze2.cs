/*
 * HamiltonianMaze2.cs
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
public class HamiltonianMaze2 : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject linesManager;
    public GameObject line;
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public HamiltonianTutorial tutorial;

    //private variables
    private List<Vector3> _lineVec;
    private LineRenderer _lineRend;
    private bool _selected;
    private bool _horizontal;
    private List<Node> _noThere;



    //Start is called before the first frame update
    //Sets up the pipe construct, since it is not changing during the game
    void Start()
    {
        List<Node> noConnectionEnd = new List<Node>()
        {
            grid.grid[1,1],
            grid.grid[1,2],
            grid.grid[3,1],
            grid.grid[5,3],

        };
        List<Node> noConnectionStart = new List<Node>()
        {
            grid.grid[1,0],
            grid.grid[1,1],
            grid.grid[2,1],
            grid.grid[5,2]
        };
        _noThere = new List<Node>()
        {
            grid.grid[2, 4],
            grid.grid[2, 2],
            grid.grid[3,0],
            grid.grid[4,3]
        };

        float size = grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            foreach (Node neighbour in grid.GetNodeNeighbours(node))
            {
                if (!_noThere.Contains(node))
                {
                    tile.transform.localScale = new Vector3(size*5.5f, size*5.5f, 0);
                    tile.GetComponent<SpriteRenderer>().color = Color.grey;
                    Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);

                    if (neighbour.gridX - node.gridX >= 0 && neighbour.gridY - node.gridY >= 0)
                    {
                        if (!(noConnectionEnd.Contains(neighbour) && noConnectionStart.Contains(node)))
                        {
                            if (_noThere.Contains(neighbour))
                            {
                                int x = neighbour.gridX + (neighbour.gridX - node.gridX);
                                int y = neighbour.gridY + (neighbour.gridY - node.gridY);

                                if (x < grid.GetGridSizeX() && x >= 0 && y < grid.GetGridSizeY() && y >= 0)
                                {

                                    DrawLine(node.worldPosition, grid.grid[x, y].worldPosition);
                                }
                            }
                            else
                            {
                                DrawLine(node.worldPosition, neighbour.worldPosition);
                            }
                        }

                    }
                }
            }

        }
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            GameObject currentTile = tilemanager.transform.GetChild(i).gameObject;


        }
        SetUp();
    }

    //Sets up the pipes to the start state
    private void SetUp()
    {
        foreach (Node node in grid.grid)
        {
            node.tileValue = 0;
        }
        for (int i = 0; i < linesManager.transform.childCount; i++)
        {
            linesManager.transform.GetChild(i).GetComponent<LineRenderer>().material.color = Color.grey;
        }


    }


    // Update is called once per frame
    void Update()
    {
        _selected = false;
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for (int i = 0; i < linesManager.transform.childCount; i++)
            {
                _lineRend = linesManager.transform.GetChild(i).gameObject.GetComponent<LineRenderer>();
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                float dify = _lineRend.GetPosition(_lineRend.positionCount - 1).y - _lineRend.GetPosition(0).y;
                float difx = _lineRend.GetPosition(_lineRend.positionCount - 1).x - _lineRend.GetPosition(0).x;
                float y;
                float x;
                if (_lineRend.GetPosition(0).x == _lineRend.GetPosition(_lineRend.positionCount - 1).x)
                {
                    _horizontal = false;
                    x = 0.3f;
                    y = dify;
                }
                else
                {
                    _horizontal = true;
                    x = difx;
                    y = 0.3f;
                }

                Bounds realBounds = new Bounds(_lineRend.GetPosition(0) + new Vector3(difx / 2, dify / 2, 0), new Vector3(x, y, 2));
                if (realBounds.Contains(touchPosition) && _selected == false && touch.phase == TouchPhase.Began)
                {
                    int value;
                    _selected = true;
                    if (_lineRend.material.color != Color.blue)
                    {
                        _lineRend.material.color = Color.blue;
                        value = 1;

                    }
                    else
                    {
                        _lineRend.material.color = Color.grey;
                        value = -1;

                    }

                    foreach (Node node in grid.grid)
                    {
                        if (_horizontal)
                        {
                            if (_lineRend.GetPosition(0) - new Vector3(0.25f, 0, 0) == node.worldPosition)
                            {
                                node.IncreaseTileValue(value);
                            }
                            else if (_lineRend.GetPosition(_lineRend.positionCount - 1) + new Vector3(0.25f, 0, 0) == node.worldPosition)
                            {
                                node.IncreaseTileValue(value);
                            }
                        }
                        else
                        {
                            if (_lineRend.GetPosition(0) - new Vector3(0, 0.25f, 0) == node.worldPosition)
                            {
                                node.IncreaseTileValue(value);
                            }
                            else if (_lineRend.GetPosition(_lineRend.positionCount - 1) + new Vector3(0, 0.25f, 0) == node.worldPosition)
                            {
                                node.IncreaseTileValue(value);
                            }
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

    //Function to create a line between the two given vectors
    private void DrawLine(Vector3 start, Vector3 end)
    {
        if (start.x == end.x)
        {
            start += new Vector3(0, 0.25f, 0);
            end -= new Vector3(0, 0.25f, 0);

        }
        else
        {
            start += new Vector3(0.25f, 0, 0);
            end -= new Vector3(0.25f, 0, 0);

        }
        GameObject currentLine = Instantiate(line, start, Quaternion.identity, linesManager.transform);
        _lineVec = new List<Vector3>()
        {
            start,
            end
        };
        _lineRend = currentLine.GetComponent<LineRenderer>();
        _lineRend.positionCount = _lineVec.Count;
        _lineRend.SetPositions(_lineVec.ToArray());


    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        foreach (Node node in grid.grid)
        {
            if (!_noThere.Contains(node))
            {
                if (node.tileValue != 2)
                {
                    return false;
                }
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
