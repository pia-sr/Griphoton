using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(GridField))]
public class Pathfinder : MonoBehaviour
{
    public GridField _grid;
    public List<Node> path;
    /*
    void Awake()
    {
        _grid = GetComponent<GridField>();
    }*/

    //With the use of the heap, FindPath finds a path between startPos and targetPos
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = _grid.GetNodeFromWorldPos(startPos);
        Node targetNode = _grid.GetNodeFromWorldPos(targetPos);

        Heap<Node> openList = new Heap<Node>(_grid.GetMaxGridSize);
        HashSet<Node> closedList = new HashSet<Node>();
        List<Node> path = new List<Node>();
        while (!targetNode.isWalkable)
        {
            targetNode = _grid.grid[targetNode.gridX, targetNode.gridY-1];
        }
        while(!startNode.isWalkable)
        {
            startNode = _grid.grid[startNode.gridX, startNode.gridY-1];
        }

        openList.insert(startNode);
        while (openList.count > 0)
        {
            Node currentNode = openList.deleteFirst();
            closedList.Add(currentNode);
            if (currentNode == targetNode)
            {
                path = TraceRoute(startNode, targetNode);
                return path;
            }

            //Since only the ghosts use pathfinding it only accesses the neighbours the ghosts can walk on
            foreach (Node neighbour in _grid.GetNodeNeighbours(currentNode))
            {
                // For Pac Man neighbour.isGhostHouse must be added!
                if (!neighbour.isWalkable || closedList.Contains(neighbour))
                {
                    continue;
                }
                int movementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (movementCostToNeighbour < currentNode.gCost || !openList.contains(neighbour))
                {
                    neighbour.gCost = movementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openList.contains(neighbour))
                    {
                        openList.insert(neighbour);
                    }
                }
            }
        }
        return path;

    }
    
    
    public List<Node> FindPathPlayer(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = _grid.GetNodeFromWorldPos(startPos);
        Node targetNode = _grid.GetNodeFromWorldPos(targetPos);

        Heap<Node> openList = new Heap<Node>(_grid.GetMaxGridSize);
        HashSet<Node> closedList = new HashSet<Node>();
        List<Node> path = new List<Node>();
        List<Node> neighboursTarget = _grid.GetNodeNeighboursDiagonal(targetNode);
        List<Node> neighboursStart = _grid.GetNodeNeighboursDiagonal(startNode);
        int counterTarget = 0;
        int counterStart = 0;
        while (!targetNode.isWalkable)
        {
            targetNode = neighboursTarget[counterTarget];
            counterTarget++;
        }
        while(!startNode.isWalkable)
        {
            startNode = neighboursStart[counterStart];
            counterStart++;
        }

        openList.insert(startNode);
        while (openList.count > 0)
        {
            Node currentNode = openList.deleteFirst();
            closedList.Add(currentNode);
            if (currentNode == targetNode)
            {
                path = TraceRoute(startNode, targetNode);
                return path;
            }

            //Since only the ghosts use pathfinding it only accesses the neighbours the ghosts can walk on
            foreach (Node neighbour in _grid.GetNodeNeighbours(currentNode))
            {
                // For Pac Man neighbour.isGhostHouse must be added!
                if (!neighbour.isWalkable || closedList.Contains(neighbour))
                {
                    continue;
                }
                int movementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (movementCostToNeighbour < currentNode.gCost || !openList.contains(neighbour))
                {
                    neighbour.gCost = movementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openList.contains(neighbour))
                    {
                        openList.insert(neighbour);
                    }
                }
            }
        }
        return path;

    }

    //Returns a path to the script of the ghost who is named
    public List<Node> TraceRoute(Node from, Node to)
    {
        bool done = false;
        List<Node> path = new List<Node>();
        Node currentNode = to;
        while (done == false)
        {
            path.Add(currentNode);
            if(currentNode == from)
            {
                done = true;
            }
            else
            {

                currentNode = currentNode.parent;
            }
        }
        path.Reverse();
        return path;

    }

    //Returns the added distance between the x and y value of the current and next node
    public int GetDistance(Node from, Node to)
    {
        int distanceX = Mathf.Abs(to.gridX - from.gridX);
        int distanceY = Mathf.Abs(to.gridY - from.gridY);

        return distanceX > distanceY ? 16 * distanceY + 12 * distanceX : 16 * distanceX + 12 * distanceY;
    }

}
