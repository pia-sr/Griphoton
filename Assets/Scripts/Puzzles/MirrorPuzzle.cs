using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class MirrorPuzzle : MonoBehaviour
{

    public GridField grid;
    public GameObject tile;
    public GameObject tileManager;
    public GameObject symbolManager;
    public GameObject person;
    public GameObject mirror;
    public Canvas canvas;
    public GameObject griphoton;
    public GameObject player;
    public MirrorTutorial tutorial;
    public GameObject message;


    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    private void setUp()
    {
        for(int i = 0; i < symbolManager.transform.childCount; i++)
        {
            Destroy(symbolManager.transform.GetChild(i).gameObject);
        }
        person.GetComponent<SpriteRenderer>().color = Color.black;
        person.transform.localScale = new Vector3(grid.nodeRadius, grid.nodeRadius, 0);

        mirror.transform.localScale = new Vector3(grid.nodeRadius * 2, 0.05f, 0);
        mirror.GetComponent<SpriteRenderer>().color = Color.black;
        tile.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 0);
        foreach (Node node in grid.grid)
        {
            node.onTop = "Empty";
            tile.GetComponent<SpriteRenderer>().color = Color.white;
            Instantiate(tile, node.worldPosition, Quaternion.identity, tileManager.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        setUp();
        
    }
    private Rect tile2Rect(Vector3 center)
    {

        Rect rect = new Rect(center.x - grid.nodeRadius, center.y - grid.nodeRadius, grid.nodeRadius * 2f, grid.nodeRadius * 2f);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            foreach(Node node in grid.grid)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Rect rect = tile2Rect(node.worldPosition);
                if (rect.Contains(touchPosition))
                {
                    if(node.onTop == "Empty")
                    {
                        Instantiate(person, node.worldPosition + new Vector3(0,0,-1), Quaternion.identity, symbolManager.transform);
                        node.onTop = "Person";

                    }
                    else if(node.onTop == "Person")
                    {
                        GameObject oldSymbol = null;
                        for (int i = 0; i < symbolManager.transform.childCount; i++)
                        {
                            if (grid.GetNodeFromWorldPos(symbolManager.transform.GetChild(i).transform.localPosition) == node)
                            {
                                oldSymbol = symbolManager.transform.GetChild(i).gameObject;
                            }
                        }
                        if (oldSymbol != null)
                        {
                            Destroy(oldSymbol);
                            GameObject currentMirror = Instantiate(mirror, node.worldPosition + new Vector3(0, 0, -1), Quaternion.identity, symbolManager.transform);
                            var rotation = currentMirror.transform.localRotation.eulerAngles;
                            rotation.z = 315;
                            currentMirror.transform.localRotation = Quaternion.Euler(rotation);
                            node.onTop = "LeftRight";
                        }
                        
                    }
                    else if(node.onTop == "LeftRight")
                    {
                        GameObject oldSymbol = null;
                        for(int i = 0; i < symbolManager.transform.childCount; i++)
                        {
                            if(grid.GetNodeFromWorldPos(symbolManager.transform.GetChild(i).transform.localPosition) == node)
                            {
                                oldSymbol = symbolManager.transform.GetChild(i).gameObject;
                            }
                        }
                        if(oldSymbol != null)
                        {
                            Destroy(oldSymbol);
                            GameObject currentMirror = Instantiate(mirror, node.worldPosition + new Vector3(0, 0, -1), Quaternion.identity, symbolManager.transform);
                            var rotation = currentMirror.transform.localRotation.eulerAngles;
                            rotation.z = 45;
                            currentMirror.transform.localRotation = Quaternion.Euler(rotation);
                            node.onTop = "RightLeft";
                        }
                    }
                    else
                    {
                        GameObject oldSymbol = null;
                        for (int i = 0; i < symbolManager.transform.childCount; i++)
                        {
                            if (grid.GetNodeFromWorldPos(symbolManager.transform.GetChild(i).transform.localPosition) == node)
                            {
                                oldSymbol = symbolManager.transform.GetChild(i).gameObject;
                            }
                        }
                        if (oldSymbol != null)
                        {
                            Destroy(oldSymbol);
                            node.onTop = "Empty";
                        }
                    }



                }

            }
            if (checkWin())
            {
                griphoton.SetActive(true);
                player.SetActive(true);
                griphoton.GetComponent<Upperworld>().setHouseSolved(this.transform.parent.tag);
                this.transform.parent.gameObject.SetActive(false);
            }

        }
    }
    private bool checkWin()
    {
        List<string> counters = new List<string>();
        string direction;
        int x = 0;
        int y = 0;
        foreach(Node node in grid.grid)
        {
            if(node.onTop == "Empty")
            {
                return false;
            }
        }
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            int counter = 0;
            GameObject text = canvas.transform.GetChild(i).gameObject;
            Node nextNode;
            bool search = true;
            int counterCheck = 0;
            if (grid.bounds().Contains(new Vector2(text.transform.position.x, text.transform.position.y + (grid.nodeRadius * 2))))
            {
                direction = "Up";
                x = 0;
                y = 1;
                nextNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.position.x, text.transform.position.y + (grid.nodeRadius * 2)));
            }
            else if (grid.bounds().Contains(new Vector2(text.transform.position.x, text.transform.position.y - (grid.nodeRadius * 2))))
            {
                direction = "Down";
                x = 0;
                y = -1;
                nextNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.position.x, text.transform.position.y - (grid.nodeRadius * 2)));
            }
            else if (grid.bounds().Contains(new Vector2(text.transform.position.x - (grid.nodeRadius * 2), text.transform.position.y)))
            {
                direction = "Left";
                x = -1;
                y = 0;

                nextNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.position.x - (grid.nodeRadius * 2), text.transform.position.y));
            }
            else
            {
                direction = "Right";
                x = 1;
                y = 0;
                nextNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.position.x + (grid.nodeRadius * 2), text.transform.position.y));
            }
            while (search)
            {
                counterCheck++;
                if (nextNode.onTop == "Person")
                {
                    counter++;
                }
                else if (nextNode.onTop == "LeftRight")
                {
                    switch (direction)
                    {
                        case "Down":
                            direction = "Right";
                            x = 1;
                            y = 0;
                            break;
                        case "Up":
                            direction = "Left";
                            x = -1;
                            y = 0;
                            break;
                        case "Right":
                            direction = "Down";
                            x = 0;
                            y = -1;
                            break;
                        case "Left":
                            direction = "Up";
                            x = 0;
                            y = 1;
                            break;
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case "Up":
                            direction = "Right";
                            x = 1;
                            y = 0;
                            break;
                        case "Down":
                            direction = "Left";
                            x = -1;
                            y = 0;
                            break;
                        case "Left":
                            direction = "Down";
                            x = 0;
                            y = -1;
                            break;
                        case "Right":
                            direction = "Up";
                            x = 0;
                            y = 1;
                            break;
                    }
                }
                if(nextNode.gridX + x >= 0 && nextNode.gridX + x < grid.getGridSizeX() && nextNode.gridY + y >= 0 && nextNode.gridY + y < grid.getGridSizeY())
                {

                    nextNode = grid.grid[nextNode.gridX + x, nextNode.gridY + y];
                }
                else
                {
                    search = false;
                }
            }
            counters.Add(counter.ToString());
        }
        for (int i = 0; i < counters.Count; i++)
        {
            if (counters[i] != canvas.transform.GetChild(i).GetComponent<Text>().text)
            {
                return false;
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
        griphoton.SetActive(true);
        player.SetActive(true);
        setUp();

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
