using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class RoomPuzzle2 : MonoBehaviour
{
    public GridField grid;

    public GameObject tile;
    public GameObject tilemanager;
    public GameObject colourTile;
    private List<GameObject> neighbours;
    private List<Node> textTiles;
    public Canvas canvas;
    public Text numbers;

    private List<Node> red;
    private List<Node> blue;
    private List<Node> yellow;
    private bool select;
    private bool unselect;


    private float size;

    //compare bounds instead of list size

    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        select = false;
        unselect = false;
        colourTile.GetComponent<SpriteRenderer>().color = Color.red;
        size = grid.nodeRadius;
        red = new List<Node>();
        blue = new List<Node>();
        yellow = new List<Node>();

        textTiles = new List<Node>()
        {
            grid.grid[0,0],
            grid.grid[0,3],
            grid.grid[1,1],
            grid.grid[3,2],
            grid.grid[3,4],
            grid.grid[5,4]
        };
        foreach (Node node in grid.grid)
        {
            tile.transform.localScale = new Vector3(size * 2, size * 2, 0);
            tile.GetComponent<SpriteRenderer>().color = Color.white;
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);
            if (textTiles.Contains(node))
            {
                numbers.fontSize = 100;
                numbers.GetComponent<RectTransform>().sizeDelta = new Vector3(160, 160, 0);
                numbers.text = "2";
                Instantiate(numbers, node.worldPosition, Quaternion.identity, canvas.transform);
            }

        }
    }
    private Rect tile2Rect(Transform tile)
    {

        Rect rect = new Rect(tile.transform.position.x - size, tile.transform.position.y - size, size * 2, size * 2);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            select = false;
            unselect = false;
        }
        else if (Input.GetMouseButton(0) && !checkWin())
        {

            Color colour = colourTile.GetComponent<SpriteRenderer>().color;
            Rect rectColour = tile2Rect(colourTile.transform);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for (int i = 0; i < tilemanager.transform.childCount; i++)
            {
                Rect rect = tile2Rect(tilemanager.transform.GetChild(i));



                if (rect.Contains(touchPosition))
                {

                    var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
                    Node child = grid.GetNodeFromWorldPos(rect.center);
                    if (rend.color == Color.white && !unselect)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            select = true;
                        }
                        if (colourAlreadyExists())
                        {
                            foreach (GameObject neighbour in neighbourTiles(tilemanager.transform.GetChild(i).gameObject))
                            {
                                if (neighbour.GetComponent<SpriteRenderer>().color == colour)
                                {
                                    rend.color = colour;
                                    if (colour == Color.red && !red.Contains(child))
                                    {
                                        red.Add(child);
                                    }
                                    else if (colour == Color.blue && !blue.Contains(child))
                                    {
                                        blue.Add(child);
                                    }
                                    else if (colour == Color.yellow && !yellow.Contains(child))
                                    {
                                        yellow.Add(child);
                                    }

                                }
                            }
                        }
                        else
                        {
                            rend.color = colour;
                            if (colour == Color.red && !red.Contains(child))
                            {
                                red.Add(child);
                            }
                            else if (colour == Color.blue && !blue.Contains(child))
                            {
                                blue.Add(child);
                            }
                            else if (colour == Color.yellow && !yellow.Contains(child))
                            {
                                yellow.Add(child);
                            }
                        }
                    }
                    else if (!select)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            unselect = true;
                        }
                        if (rend.color == Color.red)
                        {
                            red.Remove(child);
                        }
                        else if (rend.color == Color.blue)
                        {
                            blue.Remove(child);
                        }
                        else if (rend.color == Color.yellow)
                        {
                            yellow.Remove(child);
                        }
                        rend.color = Color.white;
                    }
                }

            }
            if (Input.GetMouseButtonDown(0))
            {
                if (rectColour.Contains(touchPosition))
                {

                    if (colour == Color.red)
                    {
                        colourTile.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                    else if (colour == Color.blue)
                    {
                        colourTile.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                    else if (colour == Color.yellow)
                    {
                        colourTile.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
            }

            if (checkWin())
            {
                Debug.Log("Won!");
            }

        }
    }
    private List<GameObject> neighbourTiles(GameObject tile)
    {
        neighbours = new List<GameObject>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (j == 0 && i == 0)
                {
                    continue;
                }
                else if (j == 0 || i == 0)
                {
                    for (int k = 0; k < tilemanager.transform.childCount; k++)
                    {
                        Bounds bounds = new Bounds(grid.transform.position, grid.gridWorldSize);
                        if (bounds.Contains(tile.transform.position + new Vector3(size * 2 * i, size * 2 * j, 0)) && tile.transform.position + new Vector3(size * 2 * i, size * 2 * j, 0) == tilemanager.transform.GetChild(k).transform.position)
                        {
                            neighbours.Add(tilemanager.transform.GetChild(k).gameObject);
                        }
                    }
                }

            }
        }
        return neighbours;

    }
    private bool colourAlreadyExists()
    {
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            if (colourTile.GetComponent<SpriteRenderer>().color == tilemanager.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color)
            {
                return true;
            }
        }
        return false;
    }
    private bool checkWin()
    {
        if (!(red.Count == 12 && blue.Count == 12 && yellow.Count == 12))
        {
            return false;
        }
        else
        {

            for (int i = 0; i < textTiles.Count; i++)
            {
                int next = 0;
                int counter = 0;
                int varX;
                int varY;
                Node nextNode = textTiles[i];
                List<Node> varList = new List<Node>();
                if (red.Contains(nextNode))
                {
                    varList = red;
                }
                else if (blue.Contains(nextNode))
                {
                    varList = blue;
                }
                else if (yellow.Contains(nextNode))
                {
                    varList = yellow;
                }
                while (next < 4)
                {
                    if (next == 0)
                    {
                        varX = 1;
                        varY = 0;
                    }
                    else if (next == 1)
                    {
                        varX = 0;
                        varY = 1;
                    }
                    else if (next == 2)
                    {
                        varX = -1;
                        varY = 0;
                    }
                    else
                    {
                        varX = 0;
                        varY = -1;
                    }
                    if (nextNode.gridX + varX >= 0 && nextNode.gridX + varX < grid.getGridSizeX() && nextNode.gridY + varY >= 0
                        && nextNode.gridY + varY < grid.getGridSizeY() && varList.Contains(grid.grid[nextNode.gridX + varX, nextNode.gridY + varY]))
                    {
                        counter++;
                        nextNode = grid.grid[nextNode.gridX + varX, nextNode.gridY + varY];
                    }
                    else
                    {
                        next++;
                        nextNode = textTiles[i];
                    }
                }
                if (counter != 2)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
