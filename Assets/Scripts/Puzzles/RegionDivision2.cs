using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridField))]
public class RegionDivision2 : MonoBehaviour
{
    public GridField grid;

    public GameObject tile;
    public GameObject tilemanager;
    public GameObject colourTile;
    private List<Node> notThere;
    private List<GameObject> neighbours;
    public RegionDivisionTutorial tutorial;
    public GameObject message;

    private List<GameObject> red;
    private List<GameObject> blue;
    private List<GameObject> yellow;
    private List<GameObject> green;
    private bool select;
    private bool unselect;
    public GameObject griphoton;
    public GameObject player;
    private Color colourEmpty;


    private float size;

    //compare bounds instead of list size

    private void setUp()
    {

        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            Destroy(tilemanager.transform.GetChild(i).gameObject);
        }
        select = false;
        unselect = false;
        colourTile.GetComponent<SpriteRenderer>().color = Color.red;
        size = grid.nodeRadius;
        red = new List<GameObject>();
        blue = new List<GameObject>();
        yellow = new List<GameObject>();
        green = new List<GameObject>();
        notThere = new List<Node>()
        {
            grid.grid[0,4],
            grid.grid[0,5],
            grid.grid[2,2],
            grid.grid[2,5],
            grid.grid[3,5],
            grid.grid[4,5]
        };
        foreach (Node node in grid.grid)
        {
            if (!notThere.Contains(node))
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (j == 0 || i == 0)
                        {
                            continue;
                        }
                        float x = (size / 2) * i;
                        float y = (size / 2) * j;
                        tile.transform.localScale = new Vector3(size, size, 0);
                        tile.GetComponent<SpriteRenderer>().color = colourEmpty;
                        Instantiate(tile, node.worldPosition + new Vector3(x, y, 0), Quaternion.identity, tilemanager.transform);
                    }
                }
            }

        }
    }

    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        colourEmpty = new Color(1, 0.8f, 0.65f);
        setUp();
    }
    private Rect tile2Rect(Transform tile)
    {

        Rect rect = new Rect(tile.transform.position.x - size / 2, tile.transform.position.y - size / 2, size, size);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {

        
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            Touch touch = Input.GetTouch(0);
            Color colour = colourTile.GetComponent<SpriteRenderer>().color;
            Rect rectColour = tile2Rect(colourTile.transform);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            for (int i = 0; i < tilemanager.transform.childCount; i++)
            {
                Rect rect = tile2Rect(tilemanager.transform.GetChild(i));



                if (rect.Contains(touchPosition))
                {
                    var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
                    GameObject child = tilemanager.transform.GetChild(i).gameObject;
                    if (rend.color == colourEmpty && !unselect)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            select = true;
                        }
                        if (colourAlreadyExists())
                        {
                            foreach (GameObject neighbour in neighbourTiles(child))
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
                                    else if (colour == Color.green && !green.Contains(child))
                                    {
                                        green.Add(child);
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
                            else if (colour == Color.green && !green.Contains(child))
                            {
                                green.Add(child);
                            }
                        }
                    }

                    else if (!select)
                    {
                        if (touch.phase == TouchPhase.Began)
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
                        else if (rend.color == Color.green)
                        {
                            green.Remove(child);
                        }
                        rend.color = colourEmpty;
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
            if (touch.phase == TouchPhase.Ended)
            {
                select = false;
                unselect = false;
            }
            if (checkWin())
            {
                griphoton.SetActive(true);
                player.SetActive(true);
                griphoton.GetComponent<Upperworld>().setHouseSolved(this.transform.parent.transform.parent.tag);
                this.transform.parent.transform.parent.gameObject.SetActive(false);
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
                        if (bounds.Contains(tile.transform.position + new Vector3(size * i, size * j, 0)) && tile.transform.position + new Vector3(size * i, size * j, 0) == tilemanager.transform.GetChild(k).transform.position)
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
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (rend.color == colourEmpty)
            {
                return false;
            }
        }
        if (!(red.Count == blue.Count && green.Count == yellow.Count && yellow.Count == red.Count))
        {
            return false;
        }
        else
        {

            if (red.Count > 1)
            {
                Bounds redBounds = new Bounds(red[0].transform.position, new Vector3(size, size, 0));
                Bounds blueBounds = new Bounds(blue[0].transform.position, new Vector3(size, size, 0));
                Bounds yellowBounds = new Bounds(yellow[0].transform.position, new Vector3(size, size, 0));
                Bounds greenBounds = new Bounds(green[0].transform.position, new Vector3(size, size, 0));
                for (int i = 1; i < red.Count; i++)
                {
                    Bounds newBoundsR = new Bounds(red[i].transform.position, new Vector3(size, size, 0));
                    redBounds.Encapsulate(newBoundsR);
                    Bounds newBoundsB = new Bounds(blue[i].transform.position, new Vector3(size, size, 0));
                    blueBounds.Encapsulate(newBoundsB);
                    Bounds newBoundsY = new Bounds(yellow[i].transform.position, new Vector3(size, size, 0));
                    yellowBounds.Encapsulate(newBoundsY);
                    Bounds newBoundsG = new Bounds(green[i].transform.position, new Vector3(size, size, 0));
                    greenBounds.Encapsulate(newBoundsG);
                }
                float shapeR = (float)Math.Round(redBounds.size.x * redBounds.size.y, 2);
                float shapeB = (float)Math.Round(blueBounds.size.x * blueBounds.size.y, 2);
                float shapeY = (float)Math.Round(yellowBounds.size.x * yellowBounds.size.y, 2);
                float shapeG = (float)Math.Round(greenBounds.size.x * greenBounds.size.y, 2);
                if (!(shapeR == shapeB && shapeY == shapeG && shapeG == shapeR))
                {
                    return false;
                }

            }
        }
        return true;
    }
    public void restart()
    {
        if (!tutorial.inactive)
        {
            setUp();
        }
    }
    public void leave()
    {
        if (!tutorial.inactive)
        {
            tutorial.inactive = true;
            message.SetActive(true);
        }
        
    }

    public void yes()
    {
        setUp();
        griphoton.SetActive(true);
        player.SetActive(true);
        message.SetActive(false);
        tutorial.gameObject.SetActive(true);
        tutorial.setUp();
        this.transform.parent.transform.parent.gameObject.SetActive(false);
    }
    public void no()
    {
        tutorial.inactive = false;
        message.SetActive(false);
    }
}
