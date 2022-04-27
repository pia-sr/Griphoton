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
                path = TraceRoutePath(startNode, targetNode);
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
                int movementCostToNeighbour = currentNode.gCost + GetDistancePath(currentNode, neighbour);
                if (movementCostToNeighbour < currentNode.gCost || !openList.contains(neighbour))
                {
                    neighbour.gCost = movementCostToNeighbour;
                    neighbour.hCost = GetDistancePath(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openList.contains(neighbour))
                    {
                        openList.insert(neighbour);
                    }
                }
            }
        }
        return path;

    }//With the use of the heap, FindPath finds a path between startPos and targetPos
    public List<Node> FindPathEnemies(Vector3 startPos, Vector3 targetPos)
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
        if(targetNode == PlayerNode())
        {
            targetNode = closesNode(startNode);
        }
        else if (!targetNode.isWalkable)
        {
            targetNode = neighboursTarget[counterTarget];
            counterTarget++;
        }
        if (!startNode.isWalkable || targetNode == PlayerNode())
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
                if (!neighbour.isWalkable || closedList.Contains(neighbour) || neighbour == PlayerNode())
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
        if(targetNode.onTop == "House")
        {
            Node houseCenter = _grid.tagToNode(targetNode.owner);
            targetNode = _grid.grid[houseCenter.gridX, houseCenter.gridY - 2];
        }
        else if (_grid.ghostNames().Contains(targetNode.onTop) || targetNode.onTop == "Dungeon")
        {
            targetNode = _grid.grid[targetNode.gridX, targetNode.gridY - 2];
        }
        else
        {
            while (!targetNode.isWalkable || _grid.getEnemiesPos().Contains(targetNode))
            {
                targetNode = neighboursTarget[counterTarget];
                counterTarget++;
            }
            while (!startNode.isWalkable || _grid.getEnemiesPos().Contains(startNode))
            {
                startNode = neighboursStart[counterStart];
                counterStart++;

            }
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
                if (!neighbour.isWalkable || closedList.Contains(neighbour) || _grid.getEnemiesPos().Contains(neighbour))
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

        return distanceX > distanceY ? 14 * distanceY + 12 * distanceX : 14 * distanceX + 12 * distanceY;
    }

    private Node PlayerNode()
    {
        GameObject player = GameObject.Find("Player");
        return _grid.GetNodeFromWorldPos(player.transform.position);
    }
    private Node closesNode(Node start)
    {
        float initialDistance = 0;
        Node closesTarget = null;
        foreach(Node neighbour in _grid.GetNodeNeighbours(PlayerNode()))
        {
            float distance = Vector2.Distance(start.worldPosition, neighbour.worldPosition);
            if(distance < 0)
            {
                distance *= -1;
            }
            if((initialDistance == 0 || initialDistance > distance) && neighbour.isWalkable)
            {
                initialDistance = distance;
                closesTarget = neighbour;
            }
        }
        return closesTarget;
    }

    public int GetDistancePath(Node from, Node to)
    {
        int distanceX = Mathf.Abs(to.gridX - from.gridX);
        int distanceY = Mathf.Abs(to.gridY - from.gridY);
        int value = 50;
        if(to.onTop == null)
        {
            value = 50;
        }
        else if(distanceY == 0 && (to.onTop.Contains("Left") || to.onTop.Contains("Right")))
        {
            value = 1;
        }
        else if(distanceX == 0 && (to.onTop.Contains("Up") || to.onTop.Contains("Down")))
        {
            value = 1;
        }

        return distanceX * value + distanceY * value;
    }

    //Returns a path to the script of the ghost who is named
    public List<Node> TraceRoutePath(Node from, Node to)
    {
        bool done = false;
        List<Node> path = new List<Node>();
        Node currentNode = to;
        string directionChild = "";
        string directionParent = "";
        while (done == false)
        {
            path.Add(currentNode);
            if (currentNode == from)
            {
                done = true;
            }
            else
            {
                if(currentNode.gridX == currentNode.parent.gridX && currentNode.gridY < currentNode.parent.gridY)
                {
                    directionChild = "Up";
                    directionParent = "Down";
                }
                else if(currentNode.gridX == currentNode.parent.gridX && currentNode.gridY > currentNode.parent.gridY)
                {
                    directionChild = "Down";
                    directionParent = "Up";
                }
                else if(currentNode.gridY == currentNode.parent.gridY && currentNode.gridX < currentNode.parent.gridX)
                {
                    directionChild = "Right";
                    directionParent = "Left";
                }
                else if(currentNode.gridY == currentNode.parent.gridY && currentNode.gridX > currentNode.parent.gridX)
                {
                    directionChild = "Left";
                    directionParent = "Right";
                }

                if(currentNode.onTop == null)
                {
                    currentNode.onTop = directionChild;
                }
                else if (!currentNode.onTop.Contains(directionChild))
                {
                    currentNode.onTop += directionChild;
                }
                currentNode = currentNode.parent;
                if (currentNode.onTop == null)
                {
                    currentNode.onTop = directionParent;
                }
                else if (!currentNode.onTop.Contains(directionParent))
                {
                    currentNode.onTop += directionParent;
                }

            }
        }
        path.Reverse();
        return path;

    }


}
