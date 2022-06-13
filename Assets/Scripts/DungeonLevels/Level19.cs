using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level19 : MonoBehaviour
{
    //public variables
    public GridField grid;
    public GameObject wallUp;
    public GameObject wallSides;
    public GameObject prefabManager;
    public GameObject floorTile;
    public GameObject door;
    public GameObject spikes;

    //private variables
    private Game data;
    private float size;
    private GameObject exitDoor;

    private void Awake()
    {
        data = GameObject.Find("GameData").GetComponent<Game>();
    }


    //Function to set level back to its original state
    private void SetUp()
    {
        ResetGrid();
        string exit;
        if (data.activeLevel == int.Parse(this.gameObject.tag))
        {
            transform.GetChild(0).gameObject.SetActive(true);
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

        int middleX = Mathf.RoundToInt(grid.GetGridSizeX() / 2);
        int middleY = Mathf.RoundToInt(grid.GetGridSizeY() / 2);
        grid.SetDoors(grid.grid[grid.GetGridSizeX() - 1, middleY], "vertical", exit);
        grid.SetDoors(grid.grid[0, middleY], "vertical", "Entrance");
        grid.SetSpikesLager(grid.grid[6, 2], 1);
        grid.SetSpikesLager(grid.grid[13, grid.GetGridSizeY() - 3], 1);
        grid.SetSpikesLager(grid.grid[20, 2], 1);
        grid.SetSpikesLager(grid.grid[27, grid.GetGridSizeY() - 3], 1);


        size = 2 * grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Spikes")
            {
                Instantiate(spikes, node.worldPosition + new Vector3(0, 0, -0.1f), Quaternion.identity, prefabManager.transform);
            }
            else if (node == grid.grid[grid.GetGridSizeX() - 1, middleY] && node.onTop == "Exit")
            {
                door.transform.localScale = new Vector3(2.75f, 1.85f, 0);
                exitDoor = Instantiate(door, node.worldPosition + new Vector3(0, 0, -0.1f), Quaternion.identity, prefabManager.transform);
                var rotation = exitDoor.transform.localRotation.eulerAngles;
                rotation.z = 270;
                exitDoor.transform.localRotation = Quaternion.Euler(rotation);
            }
            else if (node.gridX == 6 && node.gridY > 4 && node.gridY < grid.GetGridSizeY())
            {
                node.SetItemOnTop("Nothing");
            }
            else if (node.gridX == 13 && node.gridY >= 0 && node.gridY < grid.GetGridSizeY() - 5)
            {
                node.SetItemOnTop("Nothing");
            }
            else if (node.gridX == 20 && node.gridY > 4 && node.gridY < grid.GetGridSizeY())
            {
                node.SetItemOnTop("Nothing");
            }
            else if (node.gridX == 27 && node.gridY >= 0 && node.gridY < grid.GetGridSizeY() - 5)
            {
                node.SetItemOnTop("Nothing");
            }
        }
        foreach (Node node in grid.grid)
        {
            foreach (Node neighbour in grid.GetNodeNeighboursDiagonal(node))
            {

                if ((neighbour.onTop == "Nothing" || node.gridX == 0 || node.gridX == grid.GetGridSizeX() - 1 || node.gridY == 0 || node.gridY == grid.GetGridSizeY() - 1) && node.onTop == null)
                {
                    GameObject wall = null;
                    wallUp.transform.localScale = new Vector3(size * 6.5f, size * 7, 0);
                    wallSides.transform.localScale = new Vector3(size * 6.5f, size * 6.5f, 0);
                    foreach (Node neighbour2 in grid.GetNodeNeighbours(node))
                    {
                        if (neighbour2.gridY < node.gridY && neighbour2.onTop == "Wall")
                        {
                            wall = wallUp;
                            break;
                        }
                        else
                        {
                            wall = wallSides;
                        }
                    }
                    if (wall != null)
                    {
                        node.SetItemOnTop("Wall");
                        Instantiate(wall, node.worldPosition + new Vector3(0, 0, 1), Quaternion.identity, prefabManager.transform);
                    }
                }
            }
        }
        foreach (Node node in grid.grid)
        {
            if (node.onTop == null || node.onTop == "Entrance" || node.onTop == "ExitOpen" || node.onTop == "Spikes")
            {
                if (node.onTop == null)
                {

                    node.SetItemOnTop("Floor");
                }
                floorTile.transform.localScale = new Vector3(1.1f, 1.1f, 0);
                Instantiate(floorTile, node.worldPosition, Quaternion.identity, prefabManager.transform);

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUp();

    }

    // Update is called once per frame
    void Update()
    {
        //if the player has won the door will open
        if (data.activeLevel == int.Parse(this.gameObject.tag) && NoEnemiesLeft())
        {
            Destroy(exitDoor);
            data.setLevel(20);
            foreach (Node node in grid.grid)
            {
                if (node.onTop == "Exit")
                {
                    node.SetItemOnTop("ExitOpen");
                    floorTile.transform.localScale = new Vector3(1.1f, 1.1f, 0);
                    Instantiate(floorTile, node.worldPosition, Quaternion.identity, prefabManager.transform);
                }
            }
        }
        if (GameObject.Find("Player").GetComponent<Player>().leaveLevel)
        {
            SetUp();
        }
    }


    //Function to reset the grid and all of its nodes
    private void ResetGrid()
    {
        foreach (Node node in grid.grid)
        {
            node.SetItemOnTop(null);
        }
        for (int i = 0; i < prefabManager.transform.childCount; i++)
        {
            Destroy(prefabManager.transform.GetChild(i).gameObject);
        }
    }

    //Function to check if no monster is left in the room
    private bool NoEnemiesLeft()
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
