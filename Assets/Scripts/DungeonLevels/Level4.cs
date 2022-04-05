using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4 : MonoBehaviour
{
    public GridField grid;
    public GameObject wall;
    public GameObject wallManager;
    public GameObject floorTile;
    public GameObject tileManager;
    public GameObject door;
    private float size;


    // Start is called before the first frame update
    void Start()
    {
        int middleX = Mathf.RoundToInt(grid.getGridSizeX() / 2);
        grid.door(grid.grid[middleX, 0], "horizontal", true);
        grid.door(grid.grid[grid.getGridSizeX()-1, grid.getGridSizeY()-4], "vertical", false);
        size = 2 * grid.nodeRadius;
        foreach (Node node in grid.grid)
        {
            if(node == grid.grid[middleX, 0])
            {
                Instantiate(door, node.worldPosition, Quaternion.identity, this.transform);

                door.transform.localScale = new Vector3(size*3, size, 0);
            }
            else if (node == grid.grid[grid.getGridSizeX() - 1, grid.getGridSizeY() - 4])
            {
                Instantiate(door, node.worldPosition, Quaternion.Euler(0,0,-90), this.transform);

                door.transform.localScale = new Vector3(size * 3, size, 0);
            }
            else if(node.gridX > 6 && node.gridX < grid.getGridSizeX() && node.gridY > 7 && node.gridY < 11)
            {
                node.setItemOnTop("Nothing");
            }
            else if(node.gridX > middleX + 3 && node.gridX < grid.getGridSizeX() && node.gridY >= 0 && node.gridY < 8)
            {
                node.setItemOnTop("Nothing");
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
            if (node.onTop == null)
            {
                node.setItemOnTop("Floor");
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