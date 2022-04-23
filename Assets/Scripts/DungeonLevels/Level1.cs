using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    public GridField grid;
    public GameObject wall;
    public GameObject prefabManager;
    public GameObject floorTile;
    public GameObject door;
    private Game data;
    private bool build;

    private void Awake()
    {
        data = GameObject.Find("GameData").GetComponent<Game>();
        SaveSystem.loadGame();
    }
    // Start is called before the first frame update

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
        }

        int middleX = Mathf.RoundToInt(grid.getGridSizeX() / 2);
        grid.door(grid.grid[middleX, 0], "horizontal", "Entrance");
        grid.door(grid.grid[middleX, grid.getGridSizeY() - 1], "horizontal", exit);
        float size = 2 * grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            if (node == grid.grid[middleX, grid.getGridSizeY() - 1] && node.onTop == "Exit")
            {
                Instantiate(door, node.worldPosition, Quaternion.identity, prefabManager.transform);

                door.transform.localScale = new Vector3(size * 3, size, 0);
            }
            else if (node.gridX == 0 || node.gridX == grid.getGridSizeX() - 1 || node.gridY == 0 || node.gridY == grid.getGridSizeY() - 1)
            {
                if (node.onTop == null)
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
    void Start()
    {
        //if (!data.tutorial)
        //{
            begin();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(data.activeLevel == int.Parse(this.gameObject.tag) && noEnemiesLeft() && !data.tutorial)
        {
            foreach(Node node in grid.grid)
            {
                if(node.onTop == "Exit")
                {
                    node.setItemOnTop("ExitOpen");
                    Instantiate(floorTile, node.worldPosition, Quaternion.identity, prefabManager.transform);
                    floorTile.transform.localScale = new Vector3(2 * grid.nodeRadius, 2 * grid.nodeRadius, 0);

                }
                
            }
            data.setLevel(2);
           
        }
        if (GameObject.Find("Player").GetComponent<Player>().leaveLevel)
        {
            begin();
        }
    }
    private void resetGrid()
    {
        foreach(Node node in grid.grid)
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
