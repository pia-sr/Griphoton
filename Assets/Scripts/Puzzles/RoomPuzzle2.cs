using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class RoomPuzzle2 : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject colourTile;
    public Canvas canvas;
    public Text numbers;
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public RoomTutorial tutorial;

    //private variabels
    private List<Node> _red;
    private List<Node> _blue;
    private List<Node> _yellow;
    private bool _select;
    private bool _unselect;
    private Color _colourEmpty;
    private List<GameObject> _neighbours;
    private List<Node> _textTiles;
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

        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            Destroy(tilemanager.transform.GetChild(i).gameObject);
        }
        _select = false;
        _unselect = false;
        colourTile.GetComponent<SpriteRenderer>().color = Color.red;
        _size = grid.nodeRadius;
        _red = new List<Node>();
        _blue = new List<Node>();
        _yellow = new List<Node>();

        _textTiles = new List<Node>()
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
            tile.transform.localScale = new Vector3(_size * 2, _size * 2, 0);
            tile.GetComponent<SpriteRenderer>().color = _colourEmpty;
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);
            if (_textTiles.Contains(node) && canvas.transform.childCount < _textTiles.Count)
            {
                numbers.fontSize = 120;
                numbers.GetComponent<RectTransform>().sizeDelta = new Vector3(160, 160, 0);
                numbers.text = "2";
                Instantiate(numbers, node.worldPosition, Quaternion.identity, canvas.transform);
            }

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

                //Depending if the touch just a tap or hold and depending if the tile is already coloured-in
                //it will either colour in one or more tiles or set them as empty
                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {

                    var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
                    Node child = grid.GetNodeFromWorldPos(rect.center);
                    if (rend.color == _colourEmpty && !_unselect)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            _select = true;
                        }
                        if (ColourAlreadyExists())
                        {
                            foreach (GameObject neighbour in NeighbourTiles(tilemanager.transform.GetChild(i).gameObject))
                            {
                                if (neighbour.GetComponent<SpriteRenderer>().color == colour)
                                {
                                    rend.color = colour;
                                    if (colour == Color.red && !_red.Contains(child))
                                    {
                                        _red.Add(child);
                                    }
                                    else if (colour == Color.blue && !_blue.Contains(child))
                                    {
                                        _blue.Add(child);
                                    }
                                    else if (colour == Color.yellow && !_yellow.Contains(child))
                                    {
                                        _yellow.Add(child);
                                    }

                                }
                            }
                        }
                        else
                        {
                            rend.color = colour;
                            if (colour == Color.red && !_red.Contains(child))
                            {
                                _red.Add(child);
                            }
                            else if (colour == Color.blue && !_blue.Contains(child))
                            {
                                _blue.Add(child);
                            }
                            else if (colour == Color.yellow && !_yellow.Contains(child))
                            {
                                _yellow.Add(child);
                            }
                        }
                    }
                    else if (!_select)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            _unselect = true;
                        }
                        if (rend.color == Color.red)
                        {
                            _red.Remove(child);
                        }
                        else if (rend.color == Color.blue)
                        {
                            _blue.Remove(child);
                        }
                        else if (rend.color == Color.yellow)
                        {
                            _yellow.Remove(child);
                        }
                        rend.color = _colourEmpty;
                    }
                }

            }
            if (touch.phase == TouchPhase.Began)
            {
                if (rectColour.Contains(touchPosition) && touch.phase == TouchPhase.Began)
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
            else if (touch.phase == TouchPhase.Ended)
            {
                _select = false;
                _unselect = false;
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

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        if (!(_red.Count == 12 && _blue.Count == 12 && _yellow.Count == 12))
        {
            return false;
        }
        else
        {

            for (int i = 0; i < _textTiles.Count; i++)
            {
                int next = 0;
                int counter = 0;
                int varX;
                int varY;
                Node nextNode = _textTiles[i];
                List<Node> varList = new List<Node>();
                if (_red.Contains(nextNode))
                {
                    varList = _red;
                }
                else if (_blue.Contains(nextNode))
                {
                    varList = _blue;
                }
                else if (_yellow.Contains(nextNode))
                {
                    varList = _yellow;
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
                    if (nextNode.gridX + varX >= 0 && nextNode.gridX + varX < grid.GetGridSizeX() && nextNode.gridY + varY >= 0
                        && nextNode.gridY + varY < grid.GetGridSizeY() && varList.Contains(grid.grid[nextNode.gridX + varX, nextNode.gridY + varY]))
                    {
                        counter++;
                        nextNode = grid.grid[nextNode.gridX + varX, nextNode.gridY + varY];
                    }
                    else
                    {
                        next++;
                        nextNode = _textTiles[i];
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
