using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class PiecesOf8_2 : MonoBehaviour
{
    public GridField grid;

    public GameObject tile;
    public GameObject tilemanager;
    public GameObject colourTile;
    private List<GameObject> neighbours;
    private List<Node> sideNumbersPos;
    public GameObject sideNumbers;
    public GameObject tileNumbers;
    public Text numbers;
    public Text colourText;

    private List<Color> colours;


    private float size;

    //compare bounds instead of list size

    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        colourTile.GetComponent<SpriteRenderer>().color = Color.red;
        size = grid.nodeRadius;
        Color orange = new Color(1, 0.5539422f, 0.1006289f, 1);
        Color blue = new Color(0.2019105f, 0.5038049f, 0.7735849f, 1);
        Color purple = new Color(0.7108399f, 0.5169099f, 0.9339623f, 1);
        sideNumbersPos = new List<Node>()
        {
            grid.grid[2,0],
            grid.grid[3,0],
            grid.grid[4,0],
            grid.grid[5,4]
        };
        colours = new List<Color>()
        {
            Color.red,
            blue,
            Color.green,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            orange,
            purple
        };
        foreach (Node node in grid.grid)
        {
            tile.transform.localScale = new Vector3(size * 2, size * 2, 0);
            tile.GetComponent<SpriteRenderer>().color = Color.white;
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform); 
            numbers.GetComponent<RectTransform>().sizeDelta = new Vector3(130, 130, 0);
            numbers.fontSize = 90;
            Instantiate(numbers, node.worldPosition, Quaternion.identity, tileNumbers.transform);

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
        if (Input.GetMouseButtonDown(0) && !checkWin())
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
                    if (rend.color == Color.white)
                    {
                        if (colourAlreadyExists())
                        {
                            foreach (GameObject neighbour in neighbourTiles(tilemanager.transform.GetChild(i).gameObject))
                            {
                                if (neighbour.GetComponent<SpriteRenderer>().color == colour && !exceedingColourCount())
                                {
                                    rend.color = colour;
                                    tileNumbers.transform.GetChild(i).GetComponent<Text>().text = colourText.text;

                                }
                            }
                        }
                        else
                        {
                            rend.color = colour;
                            tileNumbers.transform.GetChild(i).GetComponent<Text>().text = colourText.text;
                        }
                    }
                    else
                    {
                        rend.color = Color.white;
                        tileNumbers.transform.GetChild(i).GetComponent<Text>().text = " ";
                    }
                }

            }
            if (rectColour.Contains(touchPosition))
            {

                for(int i = 0; i < colours.Count; i++)
                {
                    if(colour == colours[i])
                    {
                        if(i != colours.Count - 1)
                        {
                            colourTile.GetComponent<SpriteRenderer>().color = colours[i + 1];
                            int textString = i + 2;
                            colourText.text = (textString).ToString();
                        }
                        else
                        {

                            colourTile.GetComponent<SpriteRenderer>().color = colours[0];
                            colourText.text = "1";

                        }
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

    private bool exceedingColourCount()
    {
        int colourCounter = 0;
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            if (colourTile.GetComponent<SpriteRenderer>().color == tilemanager.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color)
            {
                colourCounter++;
            }
        }
        if (colourCounter == int.Parse(colourText.text)) 
        {
            return true;
        }
        return false;
    }
    private int numberAtNode(Node node)
    {
        int textNumber = 0;
        for( int i = 0; i < tileNumbers.transform.childCount; i++)
        {
            if(tileNumbers.transform.GetChild(i).transform.position == node.worldPosition && tileNumbers.transform.GetChild(i).GetComponent<Text>().text != "")
            {
                textNumber = int.Parse(tileNumbers.transform.GetChild(i).GetComponent<Text>().text);
            }
        }
        return textNumber;
    }
    private bool checkWin()
    {
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (rend.color == Color.white)
            {
                return false;
            }
        }
        for (int i = 0; i < sideNumbersPos.Count; i++)
        {
            int result = int.Parse(sideNumbers.transform.GetChild(i).GetComponent<Text>().text);
            for(int j = 0; j < 6; j++)
            {
                if(sideNumbersPos[i].gridY == 0)
                {
                    result -= numberAtNode(grid.grid[sideNumbersPos[i].gridX, j]);
                }
                else
                {
                    result -= numberAtNode(grid.grid[j,sideNumbersPos[i].gridY]);
                }
            }
            if(result != 0)
            {
                return false;
            }
        }

        return true;
    }
}
