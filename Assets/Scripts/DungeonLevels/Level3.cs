using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3 : MonoBehaviour
{
    public GridField grid;
    public GameObject wallUp;
    public GameObject wallSides;
    public GameObject prefabManager;
    public GameObject floorTile;
    public GameObject spikes;
    public GameObject door;
    private Game data;
    private float size;

    private void Awake()
    {
        data = GameObject.Find("GameData").GetComponent<Game>();
    }

    private void begin()
    {
        resetGrid();
        string exit;
        if (data.activeLevel == int.Parse(this.gameObject.tag))
        {
            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
            exit = "Exit";
        }
        else
        {
            exit = "ExitOpen";
            transform.GetChild(0).gameObject.SetActive(false);
        }

        int middleX = Mathf.RoundToInt(grid.getGridSizeX() / 2);
        grid.door(grid.grid[middleX, 0], "horizontal", "Entrance");
        grid.door(grid.grid[middleX, grid.getGridSizeY() - 1], "horizontal", exit);
        grid.spikes(grid.grid[6, 8]);
        grid.spikes(grid.grid[13, 8]);
        grid.spikes(grid.grid[20, 8]);
        grid.spikes(grid.grid[27, 8]);
        size = 2 * grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Spikes")
            {
                Instantiate(spikes, node.worldPosition + new Vector3(0, 0, -0.1f), Quaternion.identity, prefabManager.transform);
            }
            else if (node == grid.grid[middleX, grid.getGridSizeY() - 1] && node.onTop == "Exit")
            {
                door.transform.localScale = new Vector3(2.75f, 1.85f, 0);
                Instantiate(door, node.worldPosition + new Vector3(0, 0, -0.1f), Quaternion.identity, prefabManager.transform);
            }
            else if ((node.gridX == 0 || node.gridX == grid.getGridSizeX() - 1 || node.gridY == 0 || node.gridY == grid.getGridSizeY() - 1) && node.onTop == null)
            {

                GameObject wall = null;
                wallUp.transform.localScale = new Vector3(size * 6.5f, size * 7, 0);
                wallSides.transform.localScale = new Vector3(size * 6.5f, size * 6.5f, 0);
                foreach (Node neighbour in grid.GetNodeNeighbours(node))
                {
                    if (node.onTop == null && neighbour.gridY < node.gridY && neighbour.onTop == "Wall")
                    {
                        wall = wallUp;
                        break;
                    }
                    else if (node.onTop == null)
                    {
                        wall = wallSides;
                    }
                }
                if (wall != null)
                {
                    node.setItemOnTop("Wall");
                    Instantiate(wall, node.worldPosition + new Vector3(0, 0, 1), Quaternion.identity, prefabManager.transform);
                }
            }
        }
        foreach (Node node in grid.grid)
        {
            if (node.onTop == null || node.onTop == "Entrance" || node.onTop == "ExitOpen" || node.onTop == "Spikes")
            {
                if (node.onTop == null)
                {

                    node.setItemOnTop("Floor");
                }
                floorTile.transform.localScale = new Vector3(1.1f, 1.1f, 0);
                Instantiate(floorTile, node.worldPosition, Quaternion.identity, prefabManager.transform);

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        begin();
    }

    // Update is called once per frame
    void Update()
    {
        if (data.activeLevel == int.Parse(this.gameObject.tag) && noEnemiesLeft())
        {
            foreach (Node node in grid.grid)
            {
                if (node.onTop == "Exit")
                {
                    node.setItemOnTop("ExitOpen");
                    floorTile.transform.localScale = new Vector3(1.1f, 1.1f, 0);
                    Instantiate(floorTile, node.worldPosition, Quaternion.identity, prefabManager.transform);

                }
                data.setLevel(4);
            }
        }
        if (GameObject.Find("Player").GetComponent<Player>().leaveLevel)
        {
            begin();
        }
    }
    private void resetGrid()
    {
        foreach (Node node in grid.grid)
        {
            node.setItemOnTop(null);
        }
        for (int i = 0; i < prefabManager.transform.childCount; i++)
        {
            Destroy(prefabManager.transform.GetChild(0).gameObject);
        }
    }
    private bool noEnemiesLeft()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }
}
