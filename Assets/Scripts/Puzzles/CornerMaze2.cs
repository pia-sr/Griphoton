using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class CornerMaze2 : MonoBehaviour
{
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject path;
    public GameObject line;
    public GameObject griphoton;
    public GameObject player;
    public Text numbers;
    private List<Node> selectedTiles;
    private Node startTile;
    private List<Vector3> linePath;
    private List<Node> corners;
    public Canvas canvas;
    public CornerTutorial tutorial;
    public GameObject message;


    private LineRenderer pathRend;
    /*
    void Awake()
    {
        grid = GetComponent<GridField>();
    }*/

    private void setUp()
    {
        for (int i = 0; i < path.transform.childCount; i++)
        {
            Destroy(path.transform.GetChild(i).gameObject);
        }
        selectedTiles.Clear();
        selectedTiles.Add(startTile);
    }

    // Start is called before the first frame update
    void Start()
    {
        float size = (grid.nodeRadius * 2) - 0.015f;
        startTile = grid.grid[0, 0];
        selectedTiles = new List<Node>()
        {
            startTile
        };
        foreach (Node node in grid.grid)
        {
            tile.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.65f);
            tile.transform.localScale = new Vector3(size, size, 0);
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);

        }

        corners = new List<Node>()
        {
            grid.grid[0,3],
            grid.grid[2,1],
            grid.grid[2,3],
            grid.grid[4,1]
        };
        for (int i = 0; i < corners.Count; i++)
        {
            numbers.fontSize = 150;
            numbers.GetComponent<RectTransform>().sizeDelta = new Vector3(160, 160, 0);
            Instantiate(numbers, corners[i].worldPosition, Quaternion.identity, canvas.transform);
        }
        canvas.transform.GetChild(0).GetComponent<Text>().text = "3";
        canvas.transform.GetChild(1).GetComponent<Text>().text = "4";
        canvas.transform.GetChild(2).GetComponent<Text>().text = "0";
        canvas.transform.GetChild(3).GetComponent<Text>().text = "4";
        setUp();
    }


    private Rect tile2Rect(Transform tile)
    {

        Rect rect = new Rect(tile.transform.position.x - grid.nodeRadius, tile.transform.position.y - grid.nodeRadius, grid.nodeRadius * 2, grid.nodeRadius * 2);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for (int i = 0; i < tilemanager.transform.childCount; i++)
            {

                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Rect rect = tile2Rect(tilemanager.transform.GetChild(i));
                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    Node currentNode = grid.GetNodeFromWorldPos(rect.center);
                    Node neighbour = null;
                    foreach (Node node in grid.GetNodeNeighbours(currentNode))
                    {
                        if (selectedTiles[selectedTiles.Count - 1] == currentNode && currentNode != startTile)
                        {
                            GameObject line = path.transform.GetChild(path.transform.childCount - 1).gameObject;
                            Destroy(line);
                            selectedTiles.Remove(currentNode);
                            break;
                        }
                        else if (selectedTiles[selectedTiles.Count - 1] == node && !selectedTiles.Contains(currentNode))
                        {
                            neighbour = node;
                        }
                    }
                    if (neighbour != null)
                    {
                        DrawLine(currentNode.worldPosition, neighbour.worldPosition);
                        selectedTiles.Add(currentNode);
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

    private void DrawLine(Vector3 start, Vector3 end)
    {

        start += new Vector3(0, 0, -0.1f);
        end += new Vector3(0, 0, -0.1f);
        GameObject currentLine = Instantiate(line, start, Quaternion.identity, path.transform);
        linePath = new List<Vector3>()
        {
            start,
            end
        };
        pathRend = currentLine.GetComponent<LineRenderer>();
        pathRend.material.color = Color.black;
        pathRend.positionCount = linePath.Count;
        pathRend.SetPositions(linePath.ToArray());



    }
    private bool corner(Node node)
    {
        if (path.transform.childCount > 1)
        {
            List<float> differences = new List<float>();
            for (int i = 0; i < path.transform.childCount; i++)
            {
                pathRend = path.transform.GetChild(i).gameObject.GetComponent<LineRenderer>();
                if (pathRend.GetPosition(0) + new Vector3(0, 0, 0.1f) == node.worldPosition || pathRend.GetPosition(pathRend.positionCount - 1) + new Vector3(0, 0, 0.1f) == node.worldPosition)
                {
                    float dif = pathRend.GetPosition(0).x - pathRend.GetPosition(pathRend.positionCount - 1).x;
                    differences.Add(dif);
                }
            }
            if (differences.Count != 2 || (differences[0].ToString() == differences[1].ToString()) || (differences[0].ToString() == (-differences[1]).ToString()))
            {
                return false;
            }
            return true;
        }
        return false;
    }

    private bool checkWin()
    {
        foreach (Node node in grid.grid)
        {
            if (selectedTiles[selectedTiles.Count - 1] != grid.grid[4, 4]) 
            {
                return false;
            }
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                GameObject text = canvas.transform.GetChild(i).gameObject;
                if (text.transform.position == node.worldPosition)
                {
                    int counter = int.Parse(text.GetComponent<Text>().text);
                    if (corner(node))
                    {
                        counter--;
                    }
                    foreach (Node neighbour in grid.GetNodeNeighbours(node))
                    {
                        if (corner(neighbour))
                        {
                            counter--;
                        }
                    }
                    if (counter != 0)
                    {
                        return false;
                    }
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
