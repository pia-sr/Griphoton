using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridField))]
public class GapPuzzle : MonoBehaviour
{
    public GridField grid;
    public GameObject tile;
    public GameObject tilemanager;
    public GameObject griphoton;
    public GameObject player;
    private List<int> tilesBlack;
    public GapPuzzleTutorial tutorial;
    public GameObject message;

    void Awake()
    {

        grid = GetComponent<GridField>();

        
    }
    
    private void setUp()
    {

        for (int i = 0; i < tilemanager.transform.childCount; i++)
        {
            Destroy(tilemanager.transform.GetChild(i).gameObject);
        }
        tilesBlack = new List<int>();
        float size = (grid.nodeRadius * 2) - 0.005f;

        foreach (Node node in grid.grid)
        {
            tile.transform.localScale = new Vector3(size, size, 0);
            tile.GetComponent<SpriteRenderer>().color = Color.white;
            Instantiate(tile, node.worldPosition, Quaternion.identity, tilemanager.transform);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
                
                
                if (rect.Contains(touchPosition))
                {

                    Node node = grid.GetNodeFromWorldPos(rect.center);
                    var rend = tilemanager.transform.GetChild(i).GetComponent<SpriteRenderer>();
                    if (rend.color == Color.white)
                    {
                        rend.color = Color.black;

                        node.selected = true;
                        tilesBlack.Add(i);
                    }
                    else
                    {
                        rend.color = Color.white;
                        tilesBlack.Remove(i);
                        node.selected = false;
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
        for(int i = 0; i < 9; i++)
        {
            int counter = 0;
            List<Node> selected = new List<Node>();
            for (int j = 0; j < 9; j++)
            {
                if (grid.grid[i, j].selected)
                {
                    counter++;
                    selected.Add(grid.grid[i, j]);
                }
            }
            if(i == 1 && selected.Count == 2)
            {
                if(selected[0].gridY - selected[1].gridY != -5)
                {
                    return false;
                }
            }
            if (counter != 2)
            {
                return false;
            }
        }
        for(int i = 0; i < grid.getGridSizeX(); i++)
        {
            int counter = 0;
            List<Node> selected = new List<Node>();
            for (int j = 0; j< grid.getGridSizeX(); j++)
            {
                if (grid.grid[j, i].selected)
                {
                    counter++;
                    selected.Add(grid.grid[j,i]);
                }
            }
            if(i == 3 && selected.Count == 2)
            {
                if (selected[0].gridX - selected[1].gridX != -4)
                {
                    return false;
                }
            }
            if (counter != 2)
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
