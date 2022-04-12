using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class MirrorPuzzle1 : MonoBehaviour
{

    public GridField grid;
    public GameObject tile;
    public GameObject tileManager;
    public GameObject symbolManager;
    public GameObject person;
    public GameObject mirror;
    public Canvas canvas;


    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    // Start is called before the first frame update
    void Start()
    {

        person.GetComponent<SpriteRenderer>().color = Color.black;
        person.transform.localScale = new Vector3(grid.nodeRadius, grid.nodeRadius, 0);

        mirror.transform.localScale = new Vector3(grid.nodeRadius * 2, 0.05f, 0);
        mirror.GetComponent<SpriteRenderer>().color = Color.black;
        tile.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 0);
        foreach (Node node in grid.grid)
        {
            node.onTop = "Empty";
            Instantiate(tile, node.worldPosition, Quaternion.identity, tileManager.transform);
            tile.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    private Rect tile2Rect(Vector3 center)
    {

        Rect rect = new Rect(center.x - grid.nodeRadius, center.y - grid.nodeRadius, grid.nodeRadius * 2f, grid.nodeRadius * 2f);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !checkWin())
        {
            foreach(Node node in grid.grid)
            {

                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Rect rect = tile2Rect(node.worldPosition);
                if (rect.Contains(touchPosition))
                {
                    if(node.onTop == "Empty")
                    {
                        Instantiate(person, node.worldPosition, Quaternion.identity, symbolManager.transform);
                        node.onTop = "Person";

                    }
                    else if(node.onTop == "Person")
                    {
                        GameObject currentMirror = Instantiate(mirror, node.worldPosition, Quaternion.identity, symbolManager.transform);
                        var rotation = currentMirror.transform.localRotation.eulerAngles;
                        rotation.z = 315;
                        currentMirror.transform.localRotation = Quaternion.Euler(rotation);
                        node.onTop = "LeftRight";
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
                            GameObject currentMirror = Instantiate(mirror, node.worldPosition, Quaternion.identity, symbolManager.transform);
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
                Debug.Log("Won!");
            }

        }
    }
    private bool checkWin()
    {
        List<string> counters = new List<string>();
        string direction;
        int x = 0;
        int y = 0;
        bool notEmpty = false;
        foreach(Node node in grid.grid)
        {
            if(node.onTop == "Empty")
            {
                notEmpty = true;
            }
        }
        if (!notEmpty)
        {
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                int counter = 0;
                GameObject text = canvas.transform.gameObject;
                Node nextNode;
                Node currentNode;
                if (grid.bounds().Contains(new Vector2(text.transform.localPosition.x, text.transform.localPosition.y + (grid.nodeRadius * 2))))
                {
                    direction = "Up";
                    x = 0;
                    y = 1;
                    currentNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.localPosition.x, text.transform.localPosition.y + (grid.nodeRadius * 2)));
                }
                else if (grid.bounds().Contains(new Vector2(text.transform.localPosition.x, text.transform.localPosition.y - (grid.nodeRadius * 2))))
                {
                    direction = "Down";
                    x = 0;
                    y = -1;
                    currentNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.localPosition.x, text.transform.localPosition.y - (grid.nodeRadius * 2)));
                }
                else if (grid.bounds().Contains(new Vector2(text.transform.localPosition.x - (grid.nodeRadius * 2), text.transform.localPosition.y)))
                {
                    direction = "Left";
                    x = -1;
                    y = 0;

                    currentNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.localPosition.x - (grid.nodeRadius * 2), text.transform.localPosition.y));
                }
                else
                {
                    direction = "Right";
                    x = 1;
                    y = 0;
                    currentNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.localPosition.x + (grid.nodeRadius * 2), text.transform.localPosition.y));
                }
                while (currentNode.gridX + x >= 0 && currentNode.gridX + x < grid.getGridSizeX() && currentNode.gridY + y >= 0 && currentNode.gridY + y < grid.getGridSizeY())
                {
                    nextNode = grid.grid[currentNode.gridX + x, currentNode.gridY + y];
                    if (nextNode.onTop == "Person")
                    {
                        counter++;
                    }
                    else if (nextNode.onTop == "LeftRight")
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
                    else
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
        }
        
        return true;
       
    }
}
