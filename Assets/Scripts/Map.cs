/*
 * Map.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject hat;
    public GameObject house;
    public GameObject tileManager;
    public GameObject houseManager;
    public GameObject canvas;

    //public variables to communicate with other scripts
    private Game _data;
    public GameObject player;

    //variables for the mapp
    private GameObject playerHat;
    public Camera camera;
    public bool update;

    //On awke the player looks for the game data
    private void Awake()
    {
        _data = GameObject.Find("GameData").GetComponent<Game>();
    }


    // Update is called once per frame
    void Update()
    {
        //if there is no icon for the player, one is created
        if(playerHat == null && player.gameObject.activeSelf)
        {
            drawMap();
            playerHat = Instantiate(hat, grid.grid[_data.xPos, _data.yPos].worldPosition, Quaternion.identity, tileManager.transform);
            playerHat.transform.localScale = new Vector3(grid.nodeRadius * 12, grid.nodeRadius * 12, 0);
        }
        else if(update)
        {
            update = false;
            playerHat.transform.position = grid.grid[_data.xPos, _data.yPos].worldPosition;
        }
    }

    //Function to draw the map with every house the player has visited to far
    public void drawMap()
    {
        house.transform.localScale = new Vector3(grid.nodeRadius * 8, grid.nodeRadius * 8, 0);
        update = false;
        int counter = 0;
        foreach (Node node in grid.grid)
        {
            if(_data.mapTags[counter] != null)
            {
                GameObject newHouse = Instantiate(house, node.worldPosition, Quaternion.identity, tileManager.transform);
                newHouse.name = _data.mapTags[counter] + "Map";
                newHouse.transform.GetChild(0).GetComponent<Canvas>().worldCamera = camera;
                newHouse.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = _data.mapTags[counter];
                if(_data.mapStatus[counter] == "solved")
                {

                    newHouse.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontStyle = FontStyles.Strikethrough;
                }
            }
            counter++;

        }
    }


    //Function to add a new house position to the map
    public void AddTag(int x, int y, string tag)
    {
        Node node = grid.grid[x, y];
        GameObject newHouse = Instantiate(house, node.worldPosition, Quaternion.identity, tileManager.transform);
        newHouse.name = tag+ "Map";
        newHouse.transform.GetChild(0).GetComponent<Canvas>().worldCamera = camera;
        newHouse.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = tag;
    }

    //If a house is solved, it is crossed out on the map
    public void SetSolvedTag(string tag)
    {
        GameObject solvedHouse = GameObject.Find(tag + "Map");
        solvedHouse.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontStyle = FontStyles.Strikethrough;

    }

}
