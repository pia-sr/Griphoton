using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level12 : MonoBehaviour
{
    public GridField grid;
    public GameObject wall;
    public GameObject prefabManager;
    public GameObject floorTile;
    public GameObject door;
    public GameObject spikes;
    public Game data;
    private float size;

    private void begin()
    {
        resetGrid();
        string exit;
        if (data.activeLevel == int.Parse(this.gameObject.tag))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            exit = "Exit";
        }
        else
        {
            exit = "ExitOpen";
        }

        int middleX = Mathf.RoundToInt(grid.getGridSizeX() / 2);
        int middleY = Mathf.RoundToInt(grid.getGridSizeY() / 2);
        grid.door(grid.grid[middleX, grid.getGridSizeY() - 1], "horizontal", exit);
        grid.door(grid.grid[0, middleY], "vertical", "Entrance");
        grid.spikesCustom(grid.grid[middleX - 7, middleY - 3], 14, 6);

        size = 2 * grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Spikes")
            {
                Instantiate(spikes, node.worldPosition, Quaternion.identity, prefabManager.transform);

                spikes.transform.localScale = new Vector3(size, size, 0);
            }
            else if (node == grid.grid[middleX, grid.getGridSizeY() - 1])
            {
                Instantiate(door, node.worldPosition, Quaternion.identity, prefabManager.transform);

                door.transform.localScale = new Vector3(size * 3, size, 0);
            }
        }
        foreach (Node node in grid.grid)
        {

            foreach (Node neighbour in grid.GetNodeNeighboursDiagonal(node))
            {

                if ((neighbour.onTop == "Nothing" || node.gridX == 0 || node.gridX == grid.getGridSizeX() - 1 || node.gridY == 0 || node.gridY == grid.getGridSizeY() - 1) && node.onTop == null)
                {
                    node.setItemOnTop("Wall");
                    Instantiate(wall, node.worldPosition, Quaternion.identity, prefabManager.transform);

                    wall.transform.localScale = new Vector3(size, size, 0);
                }
            }
        }
        foreach (Node node in grid.grid)
        {
            if (node.onTop == null || node.onTop == "Entrance" || node.onTop == "ExitOpen")
            {
                if (node.onTop == null)
                {

                    node.setItemOnTop("Floor");
                }
                Instantiate(floorTile, node.worldPosition, Quaternion.identity, prefabManager.transform);
                floorTile.transform.localScale = new Vector3(size, size, 0);

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
                    Instantiate(floorTile, node.worldPosition, Quaternion.identity, prefabManager.transform);
                    floorTile.transform.localScale = new Vector3(2 * grid.nodeRadius, 2 * grid.nodeRadius, 0);

                }
                data.setLevel(13);
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
