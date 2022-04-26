using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Upperworld : MonoBehaviour
{
    public GridField grid;
    public GameObject houses;
    public GameObject pathManager;
    public GameObject tree;
    public GameObject treeManager;
    public Game data;
    public string playerName;
    public Text messageSimple;
    public List<GameObject> grass;
    public GameObject moudField;
    public GameObject path1Direction;
    public GameObject pathChangingDirection;
    public GameObject path3Direction;
    public GameObject path4Direction;
    public GameObject pathEnd;

    private List<string> wonSentences;

    


    // Start is called before the first frame update
    void Start()
    {
        foreach (Node node in grid.grid)
        {
            if (node.onTop == null)
            {
                int rand = Random.Range(0, 100);
                GameObject grassObject;
                if(rand < 2)
                {
                    grassObject = grass[0];
                }
                else if(rand < 4)
                {
                    grassObject = grass[1];
                }
                else if(rand < 6)
                {
                    grassObject = grass[2];
                }
                else if(rand < 8)
                {
                    grassObject = grass[3];
                }
                else if(rand < 10)
                {
                    grassObject = grass[4];
                }
                else if(rand < 12)
                {
                    grassObject = grass[5];
                }
                else if(rand < 30)
                {
                    grassObject = grass[6];
                }
                else
                {
                    grassObject = grass[7];
                }
                GameObject grassTile = Instantiate(grassObject, node.worldPosition, Quaternion.identity, treeManager.transform);
                grassTile.transform.localScale = new Vector3(grid.nodeRadius * 7, grid.nodeRadius * 7, 1);
                grassTile.transform.localPosition += new Vector3(0, 0, 5);

            }
        }
        houses.SetActive(true);
        Node center = grid.grid[(int)grid.getGridSizeX() / 2, ((int)grid.getGridSizeY() / 2) +2];
        grid.setHouse(center, "Dungeon");
        houses.transform.GetChild(houses.transform.childCount - 1).transform.localPosition = center.worldPosition + new Vector3(0, 0, -1);
        houses.transform.GetChild(houses.transform.childCount - 1).GetChild(0).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 9, grid.nodeRadius * 9, 2);
        
        Pathfinder pathFinder = this.GetComponent<Pathfinder>();
        
        if (data.namePlayer.Length == 0)
        {
            for (int i = 0; i < houses.transform.childCount - 1; i++)
            {
                int x = Random.Range(5, grid.getGridSizeX()-5);
                int y = Random.Range(5, grid.getGridSizeY()-5);


                while (grid.grid[x, y].onTop != null  || HouseNearby(grid.grid[x,y]))
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
                
            }
            for (int i = 0; i < 3000; i++)
            {
                int x = Random.Range(0, grid.getGridSizeX());
                int y = Random.Range(1, grid.getGridSizeY());

                while (grid.grid[x, y].onTop != null)
                {
                    x = Random.Range(0, grid.getGridSizeX());
                    y = Random.Range(1, grid.getGridSizeY());

                }
                GameObject treeT = Instantiate(tree, grid.grid[x, y].worldPosition, Quaternion.identity, treeManager.transform);
                treeT.transform.localScale = new Vector3(grid.nodeRadius * 6, grid.nodeRadius * 6, 4);
                treeT.transform.localPosition += new Vector3(0, grid.nodeRadius, -2 + ( y * x * 0.00001f));
                grid.grid[x, y].setItemOnTop("Tree");

            }
            findEndPath();
            creatingPath();
            data.namePlayer = playerName;
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
                else if (node.onTop == "Tree")
                {
                    GameObject treeT = Instantiate(tree, node.worldPosition, Quaternion.identity, treeManager.transform);
                    treeT.transform.localScale = new Vector3(grid.nodeRadius * 4, grid.nodeRadius * 4, 1);
                }
                else if(node.onTop == "SovedCenter")
                {
                    GameObject solvedHouse = Instantiate(moudField, node.worldPosition, Quaternion.identity, houses.transform);
                    solvedHouse.transform.localScale = new Vector3(grid.nodeRadius * 9, grid.nodeRadius * 9, 1);
                }
                counter++;
            }
            for(int i = 0; i < houses.transform.childCount-1; i++)
            {
                if(houses.transform.GetChild(i).GetChild(0).gameObject.transform.localScale == Vector3.one)
                {
                    houses.transform.GetChild(i).gameObject.SetActive(false);
                }
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
        house.transform.GetChild(0).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 9, grid.nodeRadius * 9, 1);
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
        data.increaseMultiplier();
        foreach(Node node in grid.grid)
        {
            if(node.onTop == houseName)
            {
                int randIndex = Random.Range(0, wonSentences.Count);
                grid.solvedHouse(node);
                GameObject house = GameObject.Find(houseName);
                house.SetActive(false);
                GameObject solvedHouse = Instantiate(moudField, node.worldPosition, Quaternion.identity, houses.transform);
                solvedHouse.transform.localScale = new Vector3(grid.nodeRadius * 9, grid.nodeRadius * 9, 1);
                messageSimple.transform.parent.gameObject.SetActive(true);
                messageSimple.text = wonSentences[randIndex];
            }
        }
    }
    public void close()
    {
        messageSimple.transform.parent.gameObject.SetActive(false);
    }

    private void creatingPath()
    {
        float size = 7;
        foreach(Node node in grid.grid)
        {
            if(node.onTop == null)
            {
                continue;
            }
            else if (node.onTop.Contains("End"))
            {
                GameObject pathTile = Instantiate(pathEnd, node.worldPosition, Quaternion.identity, pathManager.transform);
                pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                var rotation = pathTile.transform.localRotation.eulerAngles;
                if (node.onTop.Contains("Right"))
                {
                    rotation.z = 180;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    //pathTile.transform.localPosition -= new Vector3(0, 0.2f, 0);
                }
                else if (node.onTop.Contains("Up"))
                {
                    rotation.z = 270;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
                }
                else if (node.onTop.Contains("Down"))
                {
                    rotation.z = 90;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    //pathTile.transform.localPosition += new Vector3( 0.1f, 0, 0);
                }
            }

            else if ((node.onTop.Contains("Up") && node.onTop.Contains("Down")) && !node.onTop.Contains("Left") && !node.onTop.Contains("Right"))
            {
                GameObject pathTile = Instantiate(path1Direction, node.worldPosition, Quaternion.identity, pathManager.transform);
                pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
            }

            else if (node.onTop.Contains("Up"))
            {
                if (node.onTop.Contains("Down"))
                {
                    if (!node.onTop.Contains("Left"))
                    {
                        GameObject pathTile = Instantiate(path3Direction, node.worldPosition, Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                        var rotation = pathTile.transform.localRotation.eulerAngles;
                        rotation.z = 90;
                        pathTile.transform.localRotation = Quaternion.Euler(rotation);
                        //pathTile.transform.localPosition += new Vector3(-0.1f, 0, 0);
                    }
                    else if (!node.onTop.Contains("Right"))
                    {
                        GameObject pathTile = Instantiate(path3Direction, node.worldPosition, Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                        var rotation = pathTile.transform.localRotation.eulerAngles;
                        rotation.z = 270;
                        pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    }
                    else
                    {
                        GameObject pathTile = Instantiate(path4Direction, node.worldPosition, Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    }
                }
                else
                {
                    if (!node.onTop.Contains("Right"))
                    {
                        GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition, Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    }
                    else if (!node.onTop.Contains("Left"))
                    {
                        GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition, Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                        var rotation = pathTile.transform.localRotation.eulerAngles;
                        rotation.z = 270;
                        pathTile.transform.localRotation = Quaternion.Euler(rotation);
                        //pathTile.transform.localPosition += new Vector3(0, (0.085f * grid.nodeRadius * size), 0);
                        //pathTile.transform.localPosition += new Vector3(-0.2f, -0.2f, 0);
                    }
                    else
                    {
                        GameObject pathTile = Instantiate(path3Direction, node.worldPosition, Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                        var rotation = pathTile.transform.localRotation.eulerAngles;
                        rotation.z = 180;
                        pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    }
                }
            }
            else if (node.onTop.Contains("Down"))
            {
                if (!node.onTop.Contains("Left"))
                {
                    GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition, Quaternion.identity, pathManager.transform);
                    pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    var rotation = pathTile.transform.localRotation.eulerAngles;
                    rotation.z = 180;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    //pathTile.transform.localPosition += new Vector3((0.085f * grid.nodeRadius * size), (0.085f * grid.nodeRadius * size), 0);
                }
                else if (!node.onTop.Contains("Right"))
                {
                    GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition, Quaternion.identity, pathManager.transform);
                    pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    var rotation = pathTile.transform.localRotation.eulerAngles;
                    rotation.z = 90;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    //pathTile.transform.localPosition += new Vector3((0.085f * grid.nodeRadius * size), 0, 0);
                }
                else
                {
                    GameObject pathTile = Instantiate(path3Direction, node.worldPosition, Quaternion.identity, pathManager.transform);
                    pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                }
            }
            else if (node.onTop.Contains("Left") || node.onTop.Contains("Right"))
            {
                GameObject pathTile = Instantiate(path1Direction, node.worldPosition, Quaternion.identity, pathManager.transform);
                pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                var rotation = pathTile.transform.localRotation.eulerAngles;
                rotation.z = 90;
                pathTile.transform.localRotation = Quaternion.Euler(rotation);
            }
        }
    }

    private void findEndPath()
    {
        foreach(Node node in grid.grid)
        {
            if(node.onTop == "Right" || node.onTop == "Left" || node.onTop == "Up" || node.onTop == "Down")
            {
                node.onTop += "End";
            }
        }
    }

    private bool HouseNearby(Node node)
    {
        foreach(Node neighbour1 in grid.GetNodeNeighboursDiagonal(node))
        {
            if (neighbour1.onTop == "House" || grid.ghostNames().Contains(neighbour1.onTop) || neighbour1.onTop == "Dungeon")
            {
                return true;
            }
            foreach (Node neighbour2 in grid.GetNodeNeighboursDiagonal(neighbour1))
            {
                if(neighbour2.onTop == "House" || grid.ghostNames().Contains(neighbour2.onTop) || neighbour2.onTop == "Dungeon")
                {
                    return true;
                    
                }
                foreach (Node neighbour3 in grid.GetNodeNeighboursDiagonal(neighbour2))
                {
                    if (neighbour3.onTop == "House" || grid.ghostNames().Contains(neighbour3.onTop) || neighbour3.onTop == "Dungeon")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
