using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridField))]
public class RedPointPuzzle1 : MonoBehaviour
{
    public GridField grid;
    public GameObject circle;
    public GameObject circleManager;
    private List<Node> selectedDots;
    private List<Node> dots;
    public GameObject griphoton;
    public GameObject player;
    private bool inactive;


    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    private void setUp()
    {
        inactive = false;
        selectedDots = new List<Node>();
        dots = new List<Node>()
        {
            grid.grid[0,0],
            grid.grid[1,0],
            grid.grid[3,0],
            grid.grid[4,0],
            grid.grid[1,1],
            grid.grid[3,1],
            grid.grid[4,1],
            grid.grid[2,2],
            grid.grid[4,2],
            grid.grid[0,3],
            grid.grid[2,3],
            grid.grid[4,4]
        };
        foreach(Node node in dots)
        {

            circle.transform.localScale = new Vector3(grid.nodeRadius * 1.5f, grid.nodeRadius * 1.5f, 0);
            circle.GetComponent<SpriteRenderer>().color = Color.black;
            Instantiate(circle, node.worldPosition, Quaternion.identity, circleManager.transform);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        setUp();
    }

    private Rect tile2Rect(Transform tile)
    {

        Rect rect = new Rect(tile.transform.position.x - grid.nodeRadius, tile.transform.position.y - grid.nodeRadius, grid.nodeRadius *1.5f, grid.nodeRadius*1.5f);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !inactive)
        {
            for(int i = 0; i < circleManager.transform.childCount; i++)
            {

                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Rect rect = tile2Rect(circleManager.transform.GetChild(i));
                if (rect.Contains(touchPosition))
                {
                    GameObject currentDot = circleManager.transform.GetChild(i).gameObject;
                    if(currentDot.GetComponent<SpriteRenderer>().color == Color.black)
                    {
                        currentDot.GetComponent<SpriteRenderer>().color = Color.red;
                        foreach(Node node in dots)
                        {
                            if(grid.GetNodeFromWorldPos(currentDot.transform.position) == node)
                            {
                                selectedDots.Add(node);
                            }
                        }
                    }
                    else
                    {
                        Node node2Remove = null;
                        currentDot.GetComponent<SpriteRenderer>().color = Color.black;
                        foreach (Node node in selectedDots)
                        {
                            if (grid.GetNodeFromWorldPos(currentDot.transform.position) == node)
                            {
                                node2Remove = node;
                            }
                        }
                        if(node2Remove != null)
                        {
                            selectedDots.Remove(node2Remove);
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
        List<float> distances = new List<float>();
        if(selectedDots.Count == 5)
        {
            foreach(Node node in selectedDots)
            {
                foreach(Node nextDot in selectedDots)
                {
                    if(nextDot != node)
                    {
                        float dist = Vector2.Distance(nextDot.worldPosition, node.worldPosition);
                        if (!distances.Contains(dist))
                        {
                            distances.Add(dist);
                        }
                    }
                }
            }
            Debug.Log(distances.Count);
        }
        if(distances.Count == 10)
        {
            return true;
        }
        return false;
    }

    public void restart()
    {
        setUp();
    }
    public void leave()
    {
        griphoton.SetActive(true);
        player.SetActive(true);
        setUp();
        this.transform.parent.gameObject.SetActive(false);
    }
}
