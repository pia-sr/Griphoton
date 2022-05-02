using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Code is based on the code given by our lecturer in a module last year
public class Node : IHeapItem<Node>
{
    //The position of the node
    public Vector3 worldPosition;

    public string owner;

    //Bool to determine if a node is walkable
    public bool isWalkable = true;

    //tag of the node
    public string onTop;

    //value of a tile on that node
    public int tileValue;

    private int indexHeap;

    //List of all the unwalkable tags
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

    //Function to set a tag and if the node is walkable
    public void SetItemOnTop(string itemOnTop)
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

    //Function to increase the tile value
    public void IncreaseTileValue(int value)
    {
        tileValue += value;
    }
}
