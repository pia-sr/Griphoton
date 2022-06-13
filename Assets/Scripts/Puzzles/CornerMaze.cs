using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class CornerMaze : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject path;
    public GameObject line;

    //UI
    public Text numbers;
    public Canvas canvas;
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public CornerTutorial tutorial;

    //private variables
    private List<Node> _selectedTiles;
    private Node _startTile;
    private List<Vector3> _linePath;
    private List<Node> _corners;
    private LineRenderer _pathRend;
    
    //Start is called before the first frame update
    //Sets up the tiles and numbers, since they do not change during the game
    void Start()
    {
        float size = (grid.nodeRadius * 2) - 0.015f;
        _startTile = grid.grid[0, 0];
        _selectedTiles = new List<Node>()
        {
            _startTile
        };
        foreach (Node node in grid.grid)
        {

            tile.transform.localScale = new Vector3(size, size, 0);
            tile.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.65f);
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);

        }

        _corners = new List<Node>()
        {
            grid.grid[0,1],
            grid.grid[0,2],
            grid.grid[0,3],
            grid.grid[2,4],
            grid.grid[3,0],
            grid.grid[3,2],
            grid.grid[4,1],
            grid.grid[4,3]
        };
        for (int i = 0; i < _corners.Count; i++)
        {
            numbers.fontSize = 150;
            numbers.GetComponent<RectTransform>().sizeDelta = new Vector3(160, 160, 0);
            Instantiate(numbers, _corners[i].worldPosition, Quaternion.identity, canvas.transform);
        }
        canvas.transform.GetChild(0).GetComponent<Text>().text = "3";
        canvas.transform.GetChild(1).GetComponent<Text>().text = "3";
        canvas.transform.GetChild(2).GetComponent<Text>().text = "3";
        canvas.transform.GetChild(3).GetComponent<Text>().text = "2";
        canvas.transform.GetChild(4).GetComponent<Text>().text = "1";
        canvas.transform.GetChild(5).GetComponent<Text>().text = "2";
        canvas.transform.GetChild(6).GetComponent<Text>().text = "1";
        canvas.transform.GetChild(7).GetComponent<Text>().text = "2";
        SetUp();
    }

    //Sets up the path to the start state
    private void SetUp()
    {
        for (int i = 0; i < path.transform.childCount; i++)
        {
            Destroy(path.transform.GetChild(i).gameObject);
        }
        _selectedTiles.Clear();
        _selectedTiles.Add(_startTile);
    }

    // Update is called once per frame
    void Update()
    {
        //waits for user's touch to place a path
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for (int i = 0; i < tilemanager.transform.childCount; i++)
            {

                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Rect rect = Object2Rect(tilemanager.transform.GetChild(i));
                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    Node currentNode = grid.GetNodeFromWorldPos(rect.center);
                    Node neighbour = null;
                    foreach (Node node in grid.GetNodeNeighbours(currentNode))
                    {
                        //if the user touches the last bit of path it will be removed
                        if(_selectedTiles[_selectedTiles.Count-1] == currentNode && currentNode != _startTile)
                        {
                            GameObject line = path.transform.GetChild(path.transform.childCount - 1).gameObject;
                            Destroy(line);
                            _selectedTiles.Remove(currentNode);
                            break;
                        }
                        //If the tile is empty and next to the last bit of path, a new bit of path is added
                        else if (_selectedTiles[_selectedTiles.Count-1] == node && !_selectedTiles.Contains(currentNode))
                        {
                            neighbour = node;
                        }
                    }
                    if(neighbour != null)
                    {
                        DrawLine(currentNode.worldPosition, neighbour.worldPosition);
                        _selectedTiles.Add(currentNode);
                    }


                }

            }

            //if the player has won the game, they will leave the house and the puzzle will be set as solved
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
        Rect rect = new Rect(tile.transform.position.x - grid.nodeRadius, tile.transform.position.y - grid.nodeRadius, grid.nodeRadius * 2, grid.nodeRadius * 2);
        return rect;
    }

    //Function to create a line between the two given vectors
    private void DrawLine(Vector3 start, Vector3 end)
    {
        start += new Vector3(0, 0, -0.1f);
        end += new Vector3(0, 0, -0.1f);
        GameObject currentLine = Instantiate(line, start, Quaternion.identity, path.transform);
        _linePath = new List<Vector3>()
        {
            start,
            end
        };
        _pathRend = currentLine.GetComponent<LineRenderer>();
        _pathRend.material.color = new Color(0.5f, 0.3f, 0.25f);
        _pathRend.positionCount = _linePath.Count;
        _pathRend.SetPositions(_linePath.ToArray());
    }

    //Function to check if the node has a corner on top of it
    private bool IsCorner(Node node)
    {
        if(path.transform.childCount > 1)
        {
            List<float> differences = new List<float>();
            for (int i = 0; i < path.transform.childCount; i++)
            {
                _pathRend = path.transform.GetChild(i).gameObject.GetComponent<LineRenderer>();
                if (_pathRend.GetPosition(0) + new Vector3(0, 0, 0.1f) == node.worldPosition || _pathRend.GetPosition(_pathRend.positionCount - 1) + new Vector3(0, 0, 0.1f) == node.worldPosition)
                {
                    float dif = _pathRend.GetPosition(0).x - _pathRend.GetPosition(_pathRend.positionCount - 1).x;
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

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        foreach(Node node in grid.grid)
        {
            if (_selectedTiles[_selectedTiles.Count - 1] != grid.grid[4, 4])
            {
                return false;
            }
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                GameObject text = canvas.transform.GetChild(i).gameObject;
                if(text.transform.position == node.worldPosition)
                {
                    int counter = int.Parse(text.GetComponent<Text>().text);
                    if (IsCorner(node))
                    {
                        counter--;
                    }
                    foreach(Node neighbour in grid.GetNodeNeighbours(node))
                    {
                        if (IsCorner(neighbour)){
                            counter--;
                        }
                    }
                    if(counter != 0)
                    {
                        return false;
                    }
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
