using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class PiecesOf8_2 : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject colourTile;
    public GameObject sideNumbers;
    public GameObject tileNumbers;
    public Text numbers;
    public GameObject message;
    public Text colourText;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public PieceOf8Tutorial tutorial;

    //private variables
    private List<GameObject> _neighbours;
    private List<Node> _sideNumbersPos;
    private Color _colourEmpty;
    private List<Color> _colours;
    private float _size;

    // Start is called before the first frame update
    void Start()
    {
        _colourEmpty = new Color(1, 0.8f, 0.65f);
        SetUp();
    }

    //Function to set the field to its original state
    private void SetUp()
    {
        colourTile.GetComponent<SpriteRenderer>().color = Color.red;
        colourText.text = "1";
        _size = grid.nodeRadius;
        for(int i = 0; i < tileNumbers.transform.childCount; i++)
        {
            Destroy(tileNumbers.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            Destroy(tilemanager.transform.GetChild(i).gameObject);
        }
        Color orange = new Color(1, 0.5539422f, 0.1006289f, 1);
        Color blue = new Color(0.2019105f, 0.5038049f, 0.7735849f, 1);
        Color purple = new Color(0.7108399f, 0.5169099f, 0.9339623f, 1);
        _sideNumbersPos = new List<Node>()
        {
            grid.grid[2,0],
            grid.grid[3,0],
            grid.grid[4,0],
            grid.grid[0,4]
        };
        _colours = new List<Color>()
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
            tile.transform.localScale = new Vector3(_size * 2, _size * 2, 0);
            tile.GetComponent<SpriteRenderer>().color = _colourEmpty;
            numbers.GetComponent<RectTransform>().sizeDelta = new Vector3(130, 130, 0);
            numbers.fontSize = 120;
            numbers.text = "";
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform); 
            Instantiate(numbers, node.worldPosition, Quaternion.identity, tileNumbers.transform);

        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {

            Color colour = colourTile.GetComponent<SpriteRenderer>().color;
            Rect rectColour = Object2Rect(colourTile.transform);
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            for (int i = 0; i < tilemanager.transform.childCount; i++)
            {
                Rect rect = Object2Rect(tilemanager.transform.GetChild(i));

                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
                    Node child = grid.GetNodeFromWorldPos(rect.center);
                    if (rend.color == _colourEmpty)
                    {
                        if (ColourAlreadyExists())
                        {
                            foreach (GameObject neighbour in NeighbourTiles(tilemanager.transform.GetChild(i).gameObject))
                            {
                                if (neighbour.GetComponent<SpriteRenderer>().color == colour && !ExceedingColourCount())
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
                        rend.color = _colourEmpty;
                        tileNumbers.transform.GetChild(i).GetComponent<Text>().text = " ";
                    }
                }

            }
            if (rectColour.Contains(touchPosition) && touch.phase == TouchPhase.Began)
            {

                for(int i = 0; i < _colours.Count; i++)
                {
                    if(colour == _colours[i])
                    {
                        if(i != _colours.Count - 1)
                        {
                            colourTile.GetComponent<SpriteRenderer>().color = _colours[i + 1];
                            int textString = i + 2;
                            colourText.text = (textString).ToString();
                        }
                        else
                        {

                            colourTile.GetComponent<SpriteRenderer>().color = _colours[0];
                            colourText.text = "1";

                        }
                    }
                }
            }
            if (CheckWin())
            {
                griphoton.SetActive(true);
                player.SetActive(true);
                player.GetComponent<Player>().SwitchCams();
                player.GetComponent<Player>().Unpause();
                griphoton.GetComponent<Upperworld>().SetHouseSolved(this.transform.parent.transform.parent.tag);
                this.transform.parent.transform.parent.gameObject.SetActive(false);
            }

        }
    }

    //Function to create a rectagle on top of a given object to set rectangular boundaries for that object
    private Rect Object2Rect(Transform tile)
    {

        Rect rect = new Rect(tile.transform.position.x - _size, tile.transform.position.y - _size, _size * 2, _size * 2);
        return rect;
    }


    //Function to create a list of all the neighbouring tiles of the given tile
    private List<GameObject> NeighbourTiles(GameObject tile)
    {
        _neighbours = new List<GameObject>();
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
                        if (bounds.Contains(tile.transform.position + new Vector3(_size * 2 * i, _size * 2 * j, 0)) && tile.transform.position + new Vector3(_size * 2 * i, _size * 2 * j, 0) == tilemanager.transform.GetChild(k).transform.position)
                        {
                            _neighbours.Add(tilemanager.transform.GetChild(k).gameObject);
                        }
                    }
                }

            }
        }
        return _neighbours;

    }

    //Function to check if a colour already exists on the field
    private bool ColourAlreadyExists()
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

    //Function to check if the colour exceeded the limit of tiles
    private bool ExceedingColourCount()
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

    //Function to return the number at a given node
    private int NumberAtNode(Node node)
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

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (rend.color == _colourEmpty)
            {
                return false;
            }
        }
        for (int i = 0; i < _sideNumbersPos.Count; i++)
        {
            int result = int.Parse(sideNumbers.transform.GetChild(i).GetComponent<Text>().text);
            for(int j = 0; j < 6; j++)
            {
                if(_sideNumbersPos[i].gridY == 0)
                {
                    result -= NumberAtNode(grid.grid[_sideNumbersPos[i].gridX, j]);
                }
                else
                {
                    result -= NumberAtNode(grid.grid[j,_sideNumbersPos[i].gridY]);
                }
            }
            if(result != 0)
            {
                return false;
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
