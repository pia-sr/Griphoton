/*
 * Upperworld.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Upperworld : MonoBehaviour
{
    //public objects used in the field
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
    public GameObject map;

    


    // Start is called before the first frame update
    void Start()
    {
        //grass tiles
        foreach (Node node in grid.grid)
        {
            int rand = Random.Range(0, 100);
            GameObject grassObject;
            if (rand < 2)
            {
                grassObject = grass[0];
            }
            else if (rand < 4)
            {
                grassObject = grass[1];
            }
            else if (rand < 6)
            {
                grassObject = grass[2];
            }
            else if (rand < 8)
            {
                grassObject = grass[3];
            }
            else if (rand < 10)
            {
                grassObject = grass[4];
            }
            else if (rand < 12)
            {
                grassObject = grass[5];
            }
            else if (rand < 30)
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

        //Dungeon
        houses.SetActive(true);
        Node center = grid.grid[(int)grid.GetGridSizeX() / 2, ((int)grid.GetGridSizeY() / 2) +2];
        grid.SetHouse(center, "Dungeon");

        houses.transform.GetChild(houses.transform.childCount - 1).transform.localPosition = center.worldPosition + new Vector3(0, 0, (center.gridY * 0.000001f * center.gridX));
        houses.transform.GetChild(houses.transform.childCount - 1).GetChild(0).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 9, grid.nodeRadius * 9, 5);
        
        Pathfinder pathFinder = this.GetComponent<Pathfinder>();
        
        //if the user opens the app for the first time
        if (data.namePlayer.Length == 0 || data.namePlayer == null)
        {
            foreach(Node node in grid.grid)
            {
                node.mapTag = null;
            }

            //houses
            for (int i = 0; i < houses.transform.childCount - 1; i++)
            {
                int x = Random.Range(5, grid.GetGridSizeX()-5);
                int y = Random.Range(5, grid.GetGridSizeY()-5);


                while (grid.grid[x, y].onTop != null  || HouseNearby(grid.grid[x,y]))
                {
                    x = Random.Range(5, grid.GetGridSizeX()-5);
                    y = Random.Range(5, grid.GetGridSizeY()-5);
                }

                grid.SetHouse(grid.grid[x, y], houses.transform.GetChild(i).name);
                BuildHouse(grid.grid[x, y]);

            }
            //creating the paths
            for (int i = 0; i < houses.transform.childCount; i++)
            {
                int index = Random.Range(0, houses.transform.childCount - 1);
                while (index == i)
                {
                    index = Random.Range(0, houses.transform.childCount - 1);
                }
                List<Node> path = pathFinder.FindPath(houses.transform.GetChild(i).transform.localPosition, houses.transform.GetChild(index).transform.localPosition);
                
            }
            //trees
            for (int i = 0; i < 3000; i++)
            {
                int x = Random.Range(0, grid.GetGridSizeX());
                int y = Random.Range(1, grid.GetGridSizeY());

                while (grid.grid[x, y].onTop != null  || TreeNext2House(grid.grid[x,y]))
                {
                    x = Random.Range(0, grid.GetGridSizeX());
                    y = Random.Range(1, grid.GetGridSizeY());

                }
                GameObject treeT = Instantiate(tree, grid.grid[x, y].worldPosition, Quaternion.identity, treeManager.transform);
                treeT.transform.localScale = new Vector3(grid.nodeRadius * 7, grid.nodeRadius * 7, 1);
                treeT.transform.localPosition += new Vector3(0, 0, -0.1f + ((y*2+x/10) *  0.00001f ));
                grid.grid[x, y].SetItemOnTop("Tree");

            }
            //setting the path
            FindEndPath();
            CreatingPath();

            //saving everything
            grid.grid[center.gridX, center.gridY-2].mapTag = "Dungeon";
            grid.grid[center.gridX, center.gridY - 2].status = "Dungeon";
            data.xPos = center.gridX;
            data.yPos = center.gridY - 2;
            data.namePlayer = playerName;
            SaveSystem.saveGame(data);
            data.loadGame();
            
        }
        //user has played before and the previous layout is used
        else
        {
            int counter = 0;
            foreach (Node node in grid.grid)
            {
                //houses
                if (grid.ghostNames().Contains(node.onTop))
                {
                    grid.SetHouse(node, node.onTop);
                    BuildHouse(node);
                    
                }
                //trees
                else if (node.onTop == "Tree")
                {
                    GameObject treeT = Instantiate(tree, node.worldPosition, Quaternion.identity, treeManager.transform);
                    treeT.transform.localScale = new Vector3(grid.nodeRadius * 7, grid.nodeRadius * 7, 4);
                    treeT.transform.localPosition += new Vector3(0, 0, -0.1f + (((node.gridX/10) + (node.gridY*2)) * 0.000001f));
                }
                //solved houses
                else if(node.onTop == "SolvedCenter")
                {
                    GameObject solvedHouse = Instantiate(moudField, node.worldPosition + new Vector3(0, 0, 1), Quaternion.identity, pathManager.transform);
                    solvedHouse.transform.localScale = new Vector3(grid.nodeRadius * 7, grid.nodeRadius * 7, 1);
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
            //path
            FindEndPath();
            CreatingPath();

        }
    }

    //Function to build a house at a given node
    private void BuildHouse(Node node)
    {
        GameObject house = GameObject.Find(node.onTop);
        house.transform.GetChild(0).gameObject.transform.localScale = new Vector3(grid.nodeRadius * 9, grid.nodeRadius * 9, 5);
        house.transform.localPosition = node.worldPosition + new Vector3(0, 0, 1 + (node.gridY * node.gridX * 0.000001f));
    }

    //Function to set a given house as solved
    public void SetHouseSolved(string houseName)
    {
        data.increaseMultiplier();
        foreach(Node node in grid.grid)
        {
            if(node.onTop == houseName)
            {
                grid.SetHouseSolved(node);
                GameObject house = GameObject.Find(houseName);
                house.SetActive(false);
                map.GetComponent<Map>().SetSolvedTag(houseName);
                grid.grid[node.gridX, node.gridY - 2].status = "solved";
                GameObject solvedHouse = Instantiate(moudField, node.worldPosition + new Vector3(0,0,1), Quaternion.identity, houses.transform);
                solvedHouse.transform.localScale = new Vector3(grid.nodeRadius * 7, grid.nodeRadius * 7, 1);
            }
        }
    }

    //Function to close the won message
    public void close()
    {
        messageSimple.transform.parent.parent.gameObject.SetActive(false);
    }

    //Function to set the path for every single type of path
    private void CreatingPath()
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
                GameObject pathTile = Instantiate(pathEnd, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform); ;
                pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                var rotation = pathTile.transform.localRotation.eulerAngles;
                if (node.onTop.Contains("Right"))
                {
                    rotation.z = 180;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
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
                }
            }

            else if ((node.onTop.Contains("Up") && node.onTop.Contains("Down")) && !node.onTop.Contains("Left") && !node.onTop.Contains("Right"))
            {
                GameObject pathTile = Instantiate(path1Direction, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
            }

            else if (node.onTop.Contains("Up"))
            {
                if (node.onTop.Contains("Down"))
                {
                    if (!node.onTop.Contains("Left"))
                    {
                        GameObject pathTile = Instantiate(path3Direction, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                        var rotation = pathTile.transform.localRotation.eulerAngles;
                        rotation.z = 90;
                        pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    }
                    else if (!node.onTop.Contains("Right"))
                    {
                        GameObject pathTile = Instantiate(path3Direction, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                        var rotation = pathTile.transform.localRotation.eulerAngles;
                        rotation.z = 270;
                        pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    }
                    else
                    {
                        GameObject pathTile = Instantiate(path4Direction, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    }
                }
                else
                {
                    if (!node.onTop.Contains("Right"))
                    {
                        GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    }
                    else if (!node.onTop.Contains("Left"))
                    {
                        GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                        pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                        var rotation = pathTile.transform.localRotation.eulerAngles;
                        rotation.z = 270;
                        pathTile.transform.localRotation = Quaternion.Euler(rotation);
                    }
                    else
                    {
                        GameObject pathTile = Instantiate(path3Direction, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
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
                    GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                    pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    var rotation = pathTile.transform.localRotation.eulerAngles;
                    rotation.z = 180;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
                }
                else if (!node.onTop.Contains("Right"))
                {
                    GameObject pathTile = Instantiate(pathChangingDirection, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                    pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                    var rotation = pathTile.transform.localRotation.eulerAngles;
                    rotation.z = 90;
                    pathTile.transform.localRotation = Quaternion.Euler(rotation);
                }
                else
                {
                    GameObject pathTile = Instantiate(path3Direction, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                    pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                }
            }
            else if (node.onTop.Contains("Left") || node.onTop.Contains("Right"))
            {
                GameObject pathTile = Instantiate(path1Direction, node.worldPosition + new Vector3(0, 0, 2), Quaternion.identity, pathManager.transform);
                pathTile.transform.localScale = new Vector3(grid.nodeRadius * size, grid.nodeRadius * size, 1);
                var rotation = pathTile.transform.localRotation.eulerAngles;
                rotation.z = 90;
                pathTile.transform.localRotation = Quaternion.Euler(rotation);
            }
        }
    }

    //Function to find the end of a path
    private void FindEndPath()
    {
        foreach(Node node in grid.grid)
        {
            if(node.onTop == "Right" || node.onTop == "Left" || node.onTop == "Up" || node.onTop == "Down")
            {
                node.onTop += "End";
            }
        }
    }

    //Function to check if another house is nearby so that the houses are not build so close to each other
    private bool HouseNearby(Node node)
    {
        foreach(Node neighbour in grid.GetNodeNeighbourhood(node, 30))
        {
            if (neighbour.onTop == "House" || grid.ghostNames().Contains(neighbour.onTop) || neighbour.onTop == "Dungeon")
            {
                return true;
            }
        }
        return false;
    }

    //Boolean to check if the given node is close to a house
    private bool TreeNext2House(Node node)
    {
        if(node.gridX > 0 && node.gridX < grid.GetGridSizeX() -1 && (grid.grid[node.gridX+1,node.gridY].onTop == "House" || grid.grid[node.gridX - 1, node.gridY].onTop == "House"))
        {
            return true;
        }
        return false;
    }
}
