using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridField : MonoBehaviour
{
    //Array of nodes in the grid
    public Node[,] grid;

    //Grid size (280,310)
    public Vector2 gridWorldSize;

    //Areas where pac man should not walk
    public LayerMask unwalkableMask;
    public LayerMask buttonMask;
    //Array of nodes in the grid
    public float nodeRadius;

    //Number of nodes on the x and y axes
    private int _gridSizeX;
    private int _gridSizeY;

    //Diameter of the node
    private float _nodeDiameter;

    public List<Node> listNodes;



    /*
     * use gismo to draw cubes with spaces inbetween for lines (cubes white, background black)
     * if user taps on space between cubes, draw line from corner of cube to the other
     * maybe calculate if touch is inside the bounds of node and then how far it is away from center and based on that choose side
     * or draw every line in blaxk with line renderer and make a set of every line. check if line is touched and just change the colour
     */


    // Start is called before the first frame update
    void Awake()
    {

        //_nodeDiameter, _gridSizeX and _gridSizeY are set passed on the radius and gridWorldSize
        _nodeDiameter = 2 * nodeRadius;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        CreateGrid();
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));

        /*
        foreach(Node node in grid)
        {
            
            if (node == grid[getGridSizeX()/2, getGridSizeY()/2])
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.cyan;
            }
            Gizmos.DrawCube(node.worldPosition, new Vector3(1,1,1) * (_nodeDiameter - 0.05f));
        }
        
    }*/

    public Node dungeonNode()
    {
        Node dungeon = null;
        foreach(Node node in grid)
        {
            if(node.onTop == "Dungeon")
            {
                dungeon = node;
            }
        }
        return dungeon;
    }
    
    //Getter for the total number of nodes in the grid
    public int GetMaxGridSize
    {
        get
        {
            return _gridSizeX * _gridSizeY;
        }
    }

    public int getGridSizeX()
    {
        return _gridSizeX;
    }

    public int getGridSizeY()
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
    //Since the play can walk through a passage to end up on the other side of
    //the field, two extra nodes are added in a special case
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
    public List<Node> getEnemiesPos()
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

    //Returns a list of nodes which are up, left, down and right of a given node
    //Since the play can walk through a passage to end up on the other side of
    //the field, two extra nodes are added in a special case
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
   

    //Returns a neighbour node from a given Vector3 on the grid, based on a given rotation
    public Node GetNextNeighbour(Vector3 position, Vector3 rotation)
    {
        Node current = GetNodeFromWorldPos(position);
        List<Node> neighbours = GetNodeNeighbours(current);
        Node next = current;
        switch (rotation.y)
        {
            case 0:
            case 360:
                next = neighbours[2];
                break;
            case 90:
            case -270:
                next = neighbours[3];
                break;
            case 180:
            case -180:
                next = neighbours[1];
                break;
            case 270:
            case -90:
                next = neighbours[0];
                break;
        }

        return next;
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

                //nodes which are on either ghostHouse or unwalkable layers are marked
                //bool walkable = !Physics.CheckSphere(nodeWorldPos, nodeRadius, unwalkableMask);

                //all the nodes of the grid are added to the grid array
                grid[x, y] = new Node(nodeWorldPos, x, y);
            }
        }
    }

    public Rect bounds()
    {
        Vector3 gridBottomLeftWordPos = transform.position - Vector3.right * (gridWorldSize.x / 2) - Vector3.up * (gridWorldSize.y / 2);
        Rect rectGrid = new Rect(gridBottomLeftWordPos, gridWorldSize);
        return rectGrid;
    }

    public void spikes(Node node)
    {
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                grid[node.gridX + i, node.gridY + j].setItemOnTop("Spikes");
            }
        }
    }
    public void spikesLarger(Node node, int size)
    {
        for(int i = -size; i <= size; i++)
        {
            for(int j = -size; j <= size; j++)
            {
                grid[node.gridX + i, node.gridY + j].setItemOnTop("Spikes");
            }
        }
    }
    public void spikesCustom(Node node, int x, int y)
    {
        for(int i = 0; i <= x; i++)
        {
            for(int j = 0; j <= y; j++)
            {
                if(grid[node.gridX + i, node.gridY + j].onTop == null)
                {

                    grid[node.gridX + i, node.gridY + j].setItemOnTop("Spikes");
                }
            }
        }
    }

    public void door(Node node, string direction, string entrance)
    {
        for(int i = -1; i < 2; i++)
        {
            if(direction == "vertical")
            {
                grid[node.gridX, node.gridY + i].setItemOnTop(entrance);
            }
            else
            {
                grid[node.gridX+i, node.gridY].setItemOnTop(entrance);
            }
        }
    }
    public void setHouse(Node node, string name)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if(i == 0 && j == 0)
                {
                    node.onTop = name;
                    node.isWalkable = false;
                }
                else
                {
                    grid[node.gridX + i, node.gridY + j].setItemOnTop("House");
                    grid[node.gridX + i, node.gridY + j].isWalkable = false;
                    grid[node.gridX + i, node.gridY + j].owner = name;
                }
            }
        }
    }
    public void solvedHouse(Node node)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    node.onTop = "SolvedCenter";
                    node.isWalkable = true;
                }
                else
                {
                    grid[node.gridX + i, node.gridY + j].setItemOnTop("Solved");
                    grid[node.gridX + i, node.gridY + j].owner = null;
                    grid[node.gridX + i, node.gridY + j].isWalkable = true;
                }
            }
        }
    }
    public Node tagToNode(string tag)
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
            "Payton"
        };
        return names;
    }

}
