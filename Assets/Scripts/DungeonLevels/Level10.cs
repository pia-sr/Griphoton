using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level10 : MonoBehaviour
{
    public GridField grid;
    public GameObject wall;
    public GameObject wallManager;
    public GameObject floorTile;
    public GameObject tileManager;
    public GameObject door;
    public GameObject spikes;
    public GameObject spikesManager;
    private float size;


    // Start is called before the first frame update
    void Start()
    {
        int middleX = Mathf.RoundToInt(grid.getGridSizeX() / 2);
        int middleY = Mathf.RoundToInt(grid.getGridSizeY() / 2);
        grid.door(grid.grid[grid.getGridSizeX()-1, middleY], "vertical", false);
        grid.door(grid.grid[middleX, grid.getGridSizeY() - 1], "horizonal", true);
        grid.spikesCustom(grid.grid[1,1], 3, 15);
        grid.spikesCustom(grid.grid[9,1], 2, 15);
        grid.spikesCustom(grid.grid[16,1], 2, 15);
        grid.spikesCustom(grid.grid[23,1], 2, 15);
        grid.spikesCustom(grid.grid[30,1], 2, 15);

        size = 2 * grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Spikes")
            {
                Instantiate(spikes, node.worldPosition, Quaternion.identity, spikesManager.transform);

                spikes.transform.localScale = new Vector3(size, size, 0);
            }
            else if (node == grid.grid[grid.getGridSizeX() - 1, middleY])
            {
                Instantiate(door, node.worldPosition, Quaternion.Euler(0, 0, -90), this.transform);

                door.transform.localScale = new Vector3(size * 3, size, 0);
            }
        }
        foreach (Node node in grid.grid)
        {

            foreach (Node neighbour in grid.GetNodeNeighboursDiagonal(node))
            {

                if ((neighbour.onTop == "Nothing" || node.gridX == 0  || node.gridX == grid.getGridSizeX()-1 || node.gridY == 0 || node.gridY == grid.getGridSizeY()-1) && node.onTop == null)
                {
                    node.setItemOnTop("Wall");
                    Instantiate(wall, node.worldPosition, Quaternion.identity, wallManager.transform);

                    wall.transform.localScale = new Vector3(size, size, 0);
                }
            }
        }
        foreach (Node node in grid.grid)
        {
            if (node.onTop == null || node.onTop == "Entrance")
            {
                if (node.onTop == null)
                {

                    node.setItemOnTop("Floor");
                }
                Instantiate(floorTile, node.worldPosition, Quaternion.identity, tileManager.transform);
                floorTile.transform.localScale = new Vector3(size, size, 0);

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
