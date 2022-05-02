using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class MirrorPuzzle : MonoBehaviour
{

    //public field objects
    public GridField grid;
    public GameObject tile;
    public GameObject tileManager;
    public GameObject symbolManager;
    public GameObject ghosts;
    public GameObject mirror;
    public Canvas canvas;
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public MirrorTutorial tutorial;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
        
    }

    //Function to set the puzzle to its original state
    private void SetUp()
    {
        for(int i = 0; i < symbolManager.transform.childCount; i++)
        {
            Destroy(symbolManager.transform.GetChild(i).gameObject);
        }
        

        mirror.transform.localScale = new Vector3(grid.nodeRadius * 2, 0.05f, 0);
        mirror.GetComponent<SpriteRenderer>().color = Color.blue;
        tile.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 0);
        foreach (Node node in grid.grid)
        {
            node.onTop = "Empty";
            tile.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.65f);
            Instantiate(tile, node.worldPosition, Quaternion.identity, tileManager.transform);
        }

    }

    //Function to create a rectagle on top of a given object to set rectangular boundaries for that object
    private Rect Object2Rect(Vector3 center)
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
                Rect rect = Object2Rect(node.worldPosition);
                if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    //Sets Ghost
                    if(node.onTop == "Empty")
                    {
                        GameObject ghost = Instantiate(ghosts, node.worldPosition + new Vector3(0, 0, -4), Quaternion.identity, symbolManager.transform);
                        ghost.transform.localScale = new Vector3(grid.nodeRadius * 5, grid.nodeRadius * 5, 0);
                        node.onTop = "Ghost";

                    }
                    //Sets first mirror
                    else if(node.onTop == "Ghost")
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
                    //Sets second mirror
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
                    //Remove all symbols
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
                griphoton.GetComponent<Upperworld>().SetHouseSolved(this.transform.parent.transform.parent.tag);
                this.transform.parent.transform.parent.gameObject.SetActive(false);
            }

        }
    }


    //Function to check if the player has solved the puzzle
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
            if (grid.Bounds().Contains(new Vector2(text.transform.position.x, text.transform.position.y + (grid.nodeRadius * 2))))
            {
                direction = "Up";
                x = 0;
                y = 1;
                nextNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.position.x, text.transform.position.y + (grid.nodeRadius * 2)));
            }
            else if (grid.Bounds().Contains(new Vector2(text.transform.position.x, text.transform.position.y - (grid.nodeRadius * 2))))
            {
                direction = "Down";
                x = 0;
                y = -1;
                nextNode = grid.GetNodeFromWorldPos(new Vector2(text.transform.position.x, text.transform.position.y - (grid.nodeRadius * 2)));
            }
            else if (grid.Bounds().Contains(new Vector2(text.transform.position.x - (grid.nodeRadius * 2), text.transform.position.y)))
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
                if (nextNode.onTop == "Ghost")
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
                if(nextNode.gridX + x >= 0 && nextNode.gridX + x < grid.GetGridSizeX() && nextNode.gridY + y >= 0 && nextNode.gridY + y < grid.GetGridSizeY())
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
        message.SetActive(false);
    }

    //Function for the no button, if the user does not want to leave
    public void no()
    {
        tutorial.inactive = false;
        message.SetActive(false);
    }

}
