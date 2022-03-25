using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridField))]
public class Upperworld : MonoBehaviour
{
    public GridField grid;
    public GameObject houses;
    public GameObject pathTile;
    public GameObject pathManager;
    public GameObject tree;
    public GameObject treeManager;

    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Node center = grid.GetNodeFromWorldPos(new Vector3(0, 0, 0));
        center.setItemOnTop("Dungeon"); 
        houses.transform.GetChild(houses.transform.childCount-1).transform.localPosition = center.worldPosition + new Vector3(0, 0, -1);
        houses.transform.GetChild(houses.transform.childCount - 1).GetChild(0).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 5, grid.nodeRadius * 5, 1);
        Pathfinder pathFinder = this.GetComponent<Pathfinder>();


        for (int i = 0; i < houses.transform.childCount-1; i++)
        {
            int x = Random.Range(1, grid.getGridSizeX());
            int y = Random.Range(1, grid.getGridSizeY());

            while(grid.grid[x,y].onTop != null)
            {
                x = Random.Range(1, grid.getGridSizeX());
                y = Random.Range(1, grid.getGridSizeY());
            }

            grid.grid[x, y].setItemOnTop("House");
            houses.transform.GetChild(i).transform.localPosition = grid.grid[x, y].worldPosition + new Vector3(0,0,-1);
            houses.transform.GetChild(i).GetChild(1).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 5, grid.nodeRadius * 5, 1);

        }
        for(int i = 0; i < houses.transform.childCount; i++)
        {
            int index = Random.Range(0, houses.transform.childCount - 1);
            List<Node> path = pathFinder.FindPath(houses.transform.GetChild(i).transform.localPosition, houses.transform.GetChild(index).transform.localPosition);
            for (int k = 0; k < path.Count; k++)
            {
                GameObject pathT = Instantiate(pathTile, path[k].worldPosition, Quaternion.identity, pathManager.transform);
                pathT.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 1);
                path[k].setItemOnTop("Path");
            }
        }
        for(int i = 0; i < 3000; i++)
        {
            int x = Random.Range(0, grid.getGridSizeX());
            int y = Random.Range(1, grid.getGridSizeY());

            while (grid.grid[x, y].onTop != null || !neighboursTree(grid.grid[x,y]))
            {
                x = Random.Range(0, grid.getGridSizeX());
                y = Random.Range(1, grid.getGridSizeY());

            }
            GameObject treeT = Instantiate(tree, grid.grid[x,y].worldPosition, Quaternion.identity, treeManager.transform);
            treeT.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 1);
            grid.grid[x, y].setItemOnTop("Tree");

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private bool neighboursTree(Node node)
    {
        foreach(Node neighbour in grid.GetNodeNeighbours(node))
        {
            foreach(Node neighbours in grid.GetNodeNeighbours(neighbour))
            {

                if (neighbours.onTop == "Tree")
                {
                    return false;
                }
            }
        }
        return true;
    }
}
