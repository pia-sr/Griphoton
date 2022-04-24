using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    //The position of the node
    public Vector3 worldPosition;

    public string owner;

    //Bool to determine if a wall is on the node
    public bool isWalkable = true;

    public string onTop;

    public int tileValue;

    private int indexHeap;

    public bool button;

    private List<string> unwalkable = new List<string>()
    {
        "House",
        "Dungeon",
        "Tree",
        "Wall",
        "Exit"
    };

    //The x and y value of the node in the grid
    public int gridX;
    public int gridY;

    public Node parent;

    public int gCost;
    public int hCost;

    public bool selected;

    //Constructor
    public Node(Vector3 worldPos, int x, int y)
    {
        worldPosition = worldPos;
        gridX = x;
        gridY = y;
        tileValue = 0;
        selected = false;
        onTop = null;

    }

    public void setItemOnTop(string itemOnTop)
    {
        onTop = itemOnTop;
        if(unwalkable.Contains(itemOnTop))
        {
            isWalkable = false;
        }
        else
        {
            isWalkable = true;
        }
    }


    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public int IndexHeap
    {
        get
        {
            return indexHeap;
        }
        set
        {
            indexHeap = value;
        }
    }

    //compare the fcost of the current node with another
    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }


    public void increaseTileValue(int value)
    {
        tileValue += value;
    }
}
