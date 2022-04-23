using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Upperworld : MonoBehaviour
{
    public GridField grid;
    public GameObject houses;
    public GameObject pathTile;
    public GameObject pathManager;
    public GameObject tree;
    public GameObject treeManager;
    public Game data;
    public string playerName;
    public Text messageSimple;

    private List<string> wonSentences;

    


    // Start is called before the first frame update
    void Start()
    {
        houses.SetActive(true);
        Node center = grid.grid[(int)grid.getGridSizeX() / 2, ((int)grid.getGridSizeY() / 2) +2];
        grid.setHouse(center, "Dungeon");
        houses.transform.GetChild(houses.transform.childCount - 1).transform.localPosition = center.worldPosition + new Vector3(0, 0, -1);
        houses.transform.GetChild(houses.transform.childCount - 1).GetChild(0).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 6, grid.nodeRadius * 6, 1);
        
        Pathfinder pathFinder = this.GetComponent<Pathfinder>();

        if (data.namePlayer == null)
        {
            for (int i = 0; i < houses.transform.childCount - 1; i++)
            {
                int x = Random.Range(5, grid.getGridSizeX()-5);
                int y = Random.Range(5, grid.getGridSizeY()-5);


                while (grid.grid[x, y].onTop != null)
                {
                    x = Random.Range(5, grid.getGridSizeX()-5);
                    y = Random.Range(5, grid.getGridSizeY()-5);
                }

                grid.setHouse(grid.grid[x, y], houses.transform.GetChild(i).name);
                buildHouse(grid.grid[x, y]);

            }
            for (int i = 0; i < houses.transform.childCount; i++)
            {
                int index = Random.Range(0, houses.transform.childCount - 1);
                while (index == i)
                {
                    index = Random.Range(0, houses.transform.childCount - 1);
                }
                List<Node> path = pathFinder.FindPath(houses.transform.GetChild(i).transform.localPosition, houses.transform.GetChild(index).transform.localPosition);
                for (int k = 0; k < path.Count; k++)
                {
                    GameObject pathT = Instantiate(pathTile, path[k].worldPosition, Quaternion.identity, pathManager.transform);
                    pathT.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 1);
                    path[k].setItemOnTop("Path");
                }
            }
            for (int i = 0; i < 4000; i++)
            {
                int x = Random.Range(0, grid.getGridSizeX());
                int y = Random.Range(1, grid.getGridSizeY());

                while (grid.grid[x, y].onTop != null)
                {
                    x = Random.Range(0, grid.getGridSizeX());
                    y = Random.Range(1, grid.getGridSizeY());

                }
                GameObject treeT = Instantiate(tree, grid.grid[x, y].worldPosition, Quaternion.identity, treeManager.transform);
                treeT.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 1);
                grid.grid[x, y].setItemOnTop("Tree");

            }
            data.namePlayer = playerName;
            data.tutorial = false;
            SaveSystem.saveGame(data);
            
        }
        else
        {
            int counter = 0;
            foreach (Node node in grid.grid)
            {
                if (grid.ghostNames().Contains(node.onTop))
                {
                    buildHouse(node);
                    
                }
                else if (node.onTop == "Path")
                {
                    GameObject pathT = Instantiate(pathTile, node.worldPosition, Quaternion.identity, pathManager.transform);
                    pathT.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 1);
                }
                else if (node.onTop == "Tree")
                {
                    GameObject treeT = Instantiate(tree, node.worldPosition, Quaternion.identity, treeManager.transform);
                    treeT.transform.localScale = new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 1);
                }
                counter++;
            }


        }
        wonSentences = new List<string>()
        {
            "Congratulations! \nYou solved the puzzle!",
            "Thank you so much for your help, " + playerName + "!",
            "Goodbye, " + playerName + ".\nGood luck with the dungeon!",
            "Brilliant! \nI knew you could help me.",
            "Now I can finally pass on. \nThank you, " + playerName + "!"

        };


    }

    private void buildHouse(Node node)
    {
        GameObject house = GameObject.Find(node.onTop);
        house.transform.GetChild(0).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 6, grid.nodeRadius * 6, 1);
        house.transform.localPosition = node.worldPosition + new Vector3(0, 0, -1);
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

                if (neighbours.onTop == "House" || neighbours.onTop == "Dungeon" || neighbour.onTop == "House" || neighbour.onTop == "Dungeon")
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void activateHouses(bool active)
    {
        for(int i = 0; i < houses.transform.childCount; i++)
        {
            houses.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(active);
        }
    }

    public void setHouseSolved(string houseName)
    {
        foreach(Node node in grid.grid)
        {
            if(node.onTop == houseName)
            {
                int randIndex = Random.Range(0, wonSentences.Count);
                grid.solvedHouse(node);
                GameObject house = GameObject.Find(houseName);
                Destroy(house);
                messageSimple.transform.parent.gameObject.SetActive(true);
                messageSimple.text = wonSentences[randIndex];
            }
        }
    }
    public void close()
    {
        messageSimple.transform.parent.gameObject.SetActive(false);
    }
}
