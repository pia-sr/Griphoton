using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class RegionDivision : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject colourTile;
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public RegionDivisionTutorial tutorial;

    //private variables
    private List<Node> _notThere;
    private List<GameObject> _neighbours;
    private List<GameObject> _red;
    private List<GameObject> _blue;
    private List<GameObject> _yellow;
    private List<GameObject> _green;
    private bool _select;
    private bool _unselect;
    private Color _colourEmpty;
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
        _red = new List<GameObject>();
        _blue = new List<GameObject>();
        _yellow = new List<GameObject>();
        _green = new List<GameObject>();
        _notThere = new List<Node>()
        {
            grid.grid[0,0],
            grid.grid[0,2],
            grid.grid[0,5],
            grid.grid[2,5],
            grid.grid[4,0],
            grid.grid[4,5]
        };
        foreach (Node node in grid.grid)
        {
            if (!_notThere.Contains(node))
            {
                for(int i =-1; i < 2; i++)
                {
                    for(int j = -1; j < 2; j++)
                    {
                        if(j == 0 || i == 0)
                        {
                            continue;
                        }
                        float x = (_size / 2) * i;
                        float y = (_size / 2) * j;
                        tile.transform.localScale = new Vector3(_size, _size, 0); 
                        tile.GetComponent<SpriteRenderer>().color = _colourEmpty;
                        Instantiate(tile, node.worldPosition + new Vector3(x, y, 0), Quaternion.identity, tilemanager.transform);
                    }
                }
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
                if (rect.Contains(touchPosition))
                {
                    var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
                    GameObject child = tilemanager.transform.GetChild(i).gameObject;
                    if (rend.color == _colourEmpty && !_unselect)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            _select = true;
                        }
                        if (ColourAlreadyExists())
                        {
                            foreach (GameObject neighbour in NeighbourTiles(child))
                            {
                                if(neighbour.GetComponent<SpriteRenderer>().color == colour)
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
                                    else if (colour == Color.green && !_green.Contains(child))
                                    {
                                        _green.Add(child);
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
                            else if (colour == Color.green && !_green.Contains(child))
                            {
                                _green.Add(child);
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
                        else if (rend.color == Color.green)
                        {
                            _green.Remove(child);
                        }
                        rend.color = _colourEmpty;
                    }
                }

            }
            if (touch.phase == TouchPhase.Began)
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
                        colourTile.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else if (colour == Color.green)
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
            if (CheckWin() && !tutorial.inactive)
            {
                tutorial.inactive = true;
                tutorial.WonPuzzle();
            }

        }
    }

    //Function to create a rectagle on top of a given object to set rectangular boundaries for that object
    private Rect Object2Rect(Transform tile)
    {

        Rect rect = new Rect(tile.transform.position.x - _size/2, tile.transform.position.y - _size/2, _size, _size);
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
                else if(j == 0 || i == 0)
                {
                    for(int k = 0; k < tilemanager.transform.childCount; k++)
                    {
                        Bounds bounds = new Bounds(grid.transform.position, grid.gridWorldSize);
                        if (bounds.Contains(tile.transform.position + new Vector3(_size * i, _size * j, 0)) && tile.transform.position + new Vector3(_size * i, _size * j, 0) == tilemanager.transform.GetChild(k).transform.position)
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
            if(colourTile.GetComponent<SpriteRenderer>().color == tilemanager.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color)
            {
                return true;
            }
        }
        return false;
    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        for(int i = 0; i < tilemanager.transform.childCount; i++)
        {
            var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (rend.color == _colourEmpty)
            {
                return false;
            }
        }
        if(!(_red.Count == _blue.Count && _green.Count == _yellow.Count && _yellow.Count == _red.Count))
        {
            return false;
        }
        else
        {

            if (_red.Count > 1)
            {
                Bounds redBounds = new Bounds(_red[0].transform.position, new Vector3(_size, _size, 0));
                Bounds blueBounds = new Bounds(_blue[0].transform.position, new Vector3(_size, _size, 0));
                Bounds yellowBounds = new Bounds(_yellow[0].transform.position, new Vector3(_size, _size, 0));
                Bounds greenBounds = new Bounds(_green[0].transform.position, new Vector3(_size, _size, 0));
                for (int i = 1; i < _red.Count; i++)
                {
                    Bounds newBoundsR = new Bounds(_red[i].transform.position, new Vector3(_size, _size, 0));
                    redBounds.Encapsulate(newBoundsR);
                    Bounds newBoundsB = new Bounds(_blue[i].transform.position, new Vector3(_size, _size, 0));
                    blueBounds.Encapsulate(newBoundsB);
                    Bounds newBoundsY = new Bounds(_yellow[i].transform.position, new Vector3(_size, _size, 0));
                    yellowBounds.Encapsulate(newBoundsY);
                    Bounds newBoundsG = new Bounds(_green[i].transform.position, new Vector3(_size, _size, 0));
                    greenBounds.Encapsulate(newBoundsG);
                }
                float shapeR = (float)Math.Round(redBounds.size.x * redBounds.size.y,2);
                float shapeB = (float)Math.Round(blueBounds.size.x * blueBounds.size.y,2);
                float shapeY = (float)Math.Round(yellowBounds.size.x * yellowBounds.size.y,2);
                float shapeG = (float)Math.Round(greenBounds.size.x * greenBounds.size.y,2);
                if(!(shapeR == shapeB && shapeY == shapeG && shapeG == shapeR))
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
