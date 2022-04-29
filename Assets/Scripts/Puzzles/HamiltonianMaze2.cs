using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridField))]
public class HamiltonianMaze2 : MonoBehaviour
{
    public GridField grid;

    public GameObject tile;
    public GameObject tilemanager;
    public GameObject linesManager;
    public GameObject line;
    public GameObject griphoton;
    public GameObject player;
    private List<Vector3> lineVec;
    private LineRenderer lineRend;
    private bool selected;
    private bool horizontal;
    private List<Node> noThere;
    public HamiltonianTutorial tutorial;
    public GameObject message;


    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    private void setUp()
    {
        foreach (Node node in grid.grid)
        {
            node.tileValue = 0;
        }
        for (int i = 0; i < linesManager.transform.childCount; i++)
        {
            linesManager.transform.GetChild(i).GetComponent<LineRenderer>().material.color = Color.grey;
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        List<Node> noConnectionEnd = new List<Node>()
        {
            grid.grid[1,1],
            grid.grid[1,2],
            grid.grid[3,1],
            grid.grid[5,3],

        };
        List<Node> noConnectionStart = new List<Node>()
        {
            grid.grid[1,0],
            grid.grid[1,1],
            grid.grid[2,1],
            grid.grid[5,2]
        };
        noThere = new List<Node>()
        {
            grid.grid[2, 4],
            grid.grid[2, 2],
            grid.grid[3,0],
            grid.grid[4,3]
        };

        float size = grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            foreach (Node neighbour in grid.GetNodeNeighbours(node))
            {
                if (!noThere.Contains(node))
                {
                    tile.transform.localScale = new Vector3(size*5, size*5, 0);
                    tile.GetComponent<SpriteRenderer>().color = Color.grey;
                    Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);

                    if (neighbour.gridX - node.gridX >= 0 && neighbour.gridY - node.gridY >= 0)
                    {
                        if (!(noConnectionEnd.Contains(neighbour) && noConnectionStart.Contains(node)))
                        {
                            if (noThere.Contains(neighbour))
                            {
                                int x = neighbour.gridX + (neighbour.gridX - node.gridX);
                                int y = neighbour.gridY + (neighbour.gridY - node.gridY);

                                if (x < grid.getGridSizeX() && x >= 0 && y < grid.getGridSizeY() && y >= 0)
                                {

                                    DrawLine(node.worldPosition, grid.grid[x, y].worldPosition);
                                }
                            }
                            else
                            {
                                DrawLine(node.worldPosition, neighbour.worldPosition);
                            }
                        }

                    }
                }
            }

        }
        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            GameObject currentTile = tilemanager.transform.GetChild(i).gameObject;


        }
        setUp();
    }
    private void DrawLine(Vector3 start, Vector3 end)
    {
        if (start.x == end.x)
        {
            start += new Vector3(0, 0.25f, 0);
            end -= new Vector3(0, 0.25f, 0);

        }
        else
        {
            start += new Vector3(0.25f, 0, 0);
            end -= new Vector3(0.25f, 0, 0);

        }
        GameObject currentLine = Instantiate(line, start, Quaternion.identity, linesManager.transform);
        lineVec = new List<Vector3>()
        {
            start,
            end
        };
        lineRend = currentLine.GetComponent<LineRenderer>();
        lineRend.positionCount = lineVec.Count;
        lineRend.SetPositions(lineVec.ToArray());


    }

    // Update is called once per frame
    void Update()
    {
        selected = false;
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for (int i = 0; i < linesManager.transform.childCount; i++)
            {
                lineRend = linesManager.transform.GetChild(i).gameObject.GetComponent<LineRenderer>();
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                float dify = lineRend.GetPosition(lineRend.positionCount - 1).y - lineRend.GetPosition(0).y;
                float difx = lineRend.GetPosition(lineRend.positionCount - 1).x - lineRend.GetPosition(0).x;
                float y;
                float x;
                if (lineRend.GetPosition(0).x == lineRend.GetPosition(lineRend.positionCount - 1).x)
                {
                    horizontal = false;
                    x = 0.2f;
                    y = dify;
                }
                else
                {
                    horizontal = true;
                    x = difx;
                    y = 0.2f;
                }

                Bounds realBounds = new Bounds(lineRend.GetPosition(0) + new Vector3(difx / 2, dify / 2, 0), new Vector3(x, y, 2));
                if (realBounds.Contains(touchPosition) && selected == false && touch.phase == TouchPhase.Began)
                {
                    int value;
                    selected = true;
                    if (lineRend.material.color != Color.blue)
                    {
                        lineRend.material.color = Color.blue;
                        value = 1;

                    }
                    else
                    {
                        lineRend.material.color = Color.grey;
                        value = -1;

                    }

                    foreach (Node node in grid.grid)
                    {
                        if (horizontal)
                        {
                            if (lineRend.GetPosition(0) - new Vector3(0.25f, 0, 0) == node.worldPosition)
                            {
                                node.increaseTileValue(value);
                            }
                            else if (lineRend.GetPosition(lineRend.positionCount - 1) + new Vector3(0.25f, 0, 0) == node.worldPosition)
                            {
                                node.increaseTileValue(value);
                            }
                        }
                        else
                        {
                            if (lineRend.GetPosition(0) - new Vector3(0, 0.25f, 0) == node.worldPosition)
                            {
                                node.increaseTileValue(value);
                            }
                            else if (lineRend.GetPosition(lineRend.positionCount - 1) + new Vector3(0, 0.25f, 0) == node.worldPosition)
                            {
                                node.increaseTileValue(value);
                            }
                        }
                    }


                }

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
    private bool checkWin()
    {
        foreach (Node node in grid.grid)
        {
            if (!noThere.Contains(node))
            {
                if (node.tileValue != 2)
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
        this.transform.parent.transform.parent.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(true);
        griphoton.SetActive(true);
        player.SetActive(true);
        message.SetActive(false);
    }
    public void no()
    {
        tutorial.inactive = false;
        message.SetActive(false);
    }
}
