/*
 * GridField.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code is based on the code given by our lecturer in a module last year
public class GridField : MonoBehaviour
{
    //Array of nodes in the grid
    public Node[,] grid;

    //Grid size 
    public Vector2 gridWorldSize;

    //Array of nodes in the grid
    public float nodeRadius;

    //Number of nodes on the x and y axes
    private int _gridSizeX;
    private int _gridSizeY;

    //Diameter of the node
    private float _nodeDiameter;

    public List<Node> listNodes;


    //the grid is drawn on awake
    void Awake()
    {
        _nodeDiameter = 2 * nodeRadius;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        CreateGrid();
    }

    public void ResizeGrid(float size, float radius)
    {
        gridWorldSize = new Vector2(size, size);
        nodeRadius = radius;
        _nodeDiameter = 2 * nodeRadius;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        CreateGrid();
    }
    
    //Getter for the total number of nodes in the grid
    public int GetMaxGridSize
    {
        get
        {
            return _gridSizeX * _gridSizeY;
        }
    }

    public int GetGridSizeX()
    {
        return _gridSizeX;
    }

    public int GetGridSizeY()
    {
        return _gridSizeY;
    }

    //Returns a node based on a given Vector3 in the grid
    public Node GetNodeFromWorldPos(Vector3 worldPos)
    {

        float xShift = worldPos.x - transform.localPosition.x + (gridWorldSize.x / 2);
        float yShift = worldPos.y - transform.localPosition.y +(gridWorldSize.y / 2);
       
        int x = Mathf.FloorToInt(xShift/ _nodeDiameter);
        int y = Mathf.FloorToInt(yShift/ _nodeDiameter);
        return grid[x, y];
    }

    //Returns a list of nodes which are up, left, down and right of a given node
    public List<Node> GetNodeNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                if (x == 0 || y == 0)
                {
                    int neighbourX = node.gridX + x;
                    int neighbourY = node.gridY + y;

                    if (neighbourX >= 0 && neighbourX < _gridSizeX && neighbourY >= 0 && neighbourY < _gridSizeY)
                    {
                        neighbours.Add(grid[neighbourX, neighbourY]);
                    }


                }


            }
        }
        return neighbours;
    }

    //Function to get the position of the monsters
    public List<Node> GetEnemiesPos()
    {
        List<Node> enemiesPos = new List<Node>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeSelf)
            {
                enemiesPos.Add(GetNodeFromWorldPos(enemy.transform.localPosition));
            }
        }
        return enemiesPos;
    }

    //Returns a list of nodes which are all next to the given node
    public List<Node> GetNodeNeighboursDiagonal(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                else
                {
                    int neighbourX = node.gridX + x;
                    int neighbourY = node.gridY + y;

                    if (neighbourX >= 0 && neighbourX < _gridSizeX && neighbourY >= 0 && neighbourY < _gridSizeY)
                    {
                        neighbours.Add(grid[neighbourX, neighbourY]);
                    }


                }


            }
        }
        return neighbours;
    }
    
    //Returns a list of nodes which are all next to the given node
    public List<Node> GetNodeNeighbourhood(Node node, int size)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                else
                {
                    int neighbourX = node.gridX + x;
                    int neighbourY = node.gridY + y;

                    if (neighbourX >= 0 && neighbourX < _gridSizeX && neighbourY >= 0 && neighbourY < _gridSizeY)
                    {
                        neighbours.Add(grid[neighbourX, neighbourY]);
                    }


                }


            }
        }
        return neighbours;
    }


    //Creates the grid based on the gridworldsize
    void CreateGrid()
    {
        grid = new Node[_gridSizeX, _gridSizeY];
        Vector3 gridBottomLeftWordPos = transform.position - Vector3.right * (gridWorldSize.x / 2) - Vector3.up * (gridWorldSize.y / 2);
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 nodeWorldPos = gridBottomLeftWordPos + Vector3.right * (x * _nodeDiameter + nodeRadius) + Vector3.up * (y * _nodeDiameter + nodeRadius);
                grid[x, y] = new Node(nodeWorldPos, x, y);
            }
        }
    }

    //Function to create boundaries for the whole grid
    public Rect Bounds()
    {
        Vector3 gridBottomLeftWordPos = transform.position - Vector3.right * (gridWorldSize.x / 2) - Vector3.up * (gridWorldSize.y / 2);
        Rect rectGrid = new Rect(gridBottomLeftWordPos, gridWorldSize);
        return rectGrid;
    }

    //Function to set the spike tags
    public void SetSpikes(Node node)
    {
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                grid[node.gridX + i, node.gridY + j].SetItemOnTop("Spikes");
            }
        }
    }

    //Function to set a bigger square of spikes
    public void SetSpikesLager(Node node, int size)
    {
        for(int i = -size; i <= size; i++)
        {
            for(int j = -size; j <= size; j++)
            {
                grid[node.gridX + i, node.gridY + j].SetItemOnTop("Spikes");
            }
        }
    }

    //Function to set a custom square of spikes
    public void SetSpikesCustom(Node node, int x, int y)
    {
        for(int i = 0; i <= x; i++)
        {
            for(int j = 0; j <= y; j++)
            {
                if(grid[node.gridX + i, node.gridY + j].onTop == null)
                {

                    grid[node.gridX + i, node.gridY + j].SetItemOnTop("Spikes");
                }
            }
        }
    }

    //Function to set the doors of the dungeon
    public void SetDoors(Node node, string direction, string entrance)
    {
        for(int i = -1; i < 2; i++)
        {
            if(direction == "vertical")
            {
                grid[node.gridX, node.gridY + i].SetItemOnTop(entrance);
            }
            else
            {
                grid[node.gridX+i, node.gridY].SetItemOnTop(entrance);
            }
        }
    }

    //Function to set a house on the given node with the given name
    public void SetHouse(Node node, string name)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 4; j++)
            {
                if(i == 0 && j == 0)
                {
                    node.onTop = name;
                    node.isWalkable = false;
                }
                else
                {
                    grid[node.gridX + i, node.gridY + j].SetItemOnTop("House");
                    grid[node.gridX + i, node.gridY + j].isWalkable = false;
                    grid[node.gridX + i, node.gridY + j].owner = name;
                }
            }
        }
    }

    //Function to set a tree at the given node
    public void SetTree(Node node)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 3; j++)
            {
                grid[node.gridX + i, node.gridY + j].isWalkable = false;
            }
        }
    }

    //Function to set a house as solved
    public void SetHouseSolved(Node node)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 4; j++)
            {
                if (i == 0 && j == 0)
                {
                    node.onTop = "SolvedCenter";
                    node.isWalkable = true;
                }
                else
                {
                    grid[node.gridX + i, node.gridY + j].SetItemOnTop("Solved");
                    grid[node.gridX + i, node.gridY + j].owner = null;
                    grid[node.gridX + i, node.gridY + j].isWalkable = true;
                }
            }
        }
    }

    //Function to return a node based on the given tag
    public Node TagToNode(string tag)
    {
        Node chosenNode = null;
        foreach(Node node in grid)
        {
            if(node.onTop == tag)
            {
                chosenNode = node;
            }
        }
        return chosenNode;
    }
    
    //Function to create a list of all the ghost names
    //all the names are gender-neutral
    public List<string> ghostNames()
    {
        List<string> names = new List<string>{
            "Taylor",
            "Sutton",
            "Robin",
            "Morgan",
            "Logan",
            "Hayden",
            "Dylan",
            "Carter",
            "Alexis",
            "Sam",
            "Quinn",
            "Harley",
            "Remy",
            "Charlie",
            "Avery",
            "Riley",
            "Rowan",
            "Jessie",
            "Terrie",
            "Brennan",
            "Erin",
            "Kaden",
            "Payton",
            "Harper",
            "Jace",
            "Kane",
            "Kennedy",
            "Brook",
            "River",
            "Clarke"
        };
        return names;
    }

    
}
