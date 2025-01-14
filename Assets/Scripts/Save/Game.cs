using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for all the variables that need to be saved
//source:https://www.youtube.com/watch?v=XOjd_qU2Ido
public class Game : MonoBehaviour
{
    public bool tutorial;
    public int activeLevel;
    public string namePlayer;
    public bool sound;
    public int xPos;
    public int yPos;
    public int hintKeys;
    public int[] usedKeys;

    public string[] mapTags;
    public string[] mapStatus;

    public string[] nodeTags;
    public int strenghtMultiplier;

    public void setLevel(int level)
    {
        this.activeLevel = level;
    }

    public void increaseMultiplier()
    {
        this.strenghtMultiplier++;
    }

    public void setName(string name)
    {
        this.namePlayer = name;
    }

    public void SaveGame()
    {
        SaveSystem.saveGame(this);
    }

    //Function to create an array of all the tags in the grid in Griphoton
    public string[] getGrid()
    {
        string[] tags;
        Player player = GameObject.Find("Player").GetComponent<Player>();
        if (player.upperWorld)
        {

            GridField grid = GameObject.Find("Grid").GetComponent<GridField>();
            tags = new string[grid.GetMaxGridSize];
            int counter = 0;
            foreach (Node node in grid.grid)
            {
                tags[counter] = node.onTop;
                counter++;
            }

        }
        else
        {
            tags = nodeTags;
        }
        return tags;
    }

    public int[] getKeys()
    {
        int[] keys;
        if (usedKeys.Length != 30)
        {
            keys = new int[30];
            for(int i = 0; i < 30; i++)
            {
                keys[i] = 0;
            }
            
        }
        else
        {
            keys = usedKeys;
        }
        return keys;

    }

    //Function to create an array of all the tags in the grid in Griphoton
    public string[] getMap()
    {
        string[] tags;
        Player player = GameObject.Find("Player").GetComponent<Player>();
        if (player.upperWorld)
        {

            GridField grid = GameObject.Find("Grid").GetComponent<GridField>();
            tags = new string[grid.GetMaxGridSize];
            int counter = 0;
            foreach (Node node in grid.grid)
            {
                tags[counter] = node.mapTag;
                counter++;
            }

        }
        else
        {
            tags = mapTags;
        }
        return tags;
    }

    //Function to create an array of all the tags in the grid in Griphoton
    public string[] getStatus()
    {
        string[] tags;
        Player player = GameObject.Find("Player").GetComponent<Player>();
        if (player.upperWorld)
        {

            GridField grid = GameObject.Find("Grid").GetComponent<GridField>();
            tags = new string[grid.GetMaxGridSize];
            int counter = 0;
            foreach (Node node in grid.grid)
            {
                tags[counter] = node.status;
                counter++;
            }

        }
        else
        {
            tags = mapStatus;
        }
        return tags;
    }

    //Function to get the read variables
    public void loadGame()
    {
        GameData data = SaveSystem.loadGame();

        tutorial = data.tutorial;
        activeLevel = data.activeLevel;
        namePlayer = data.namePlayer;
        nodeTags = data.nodeTags;
        mapTags = data.mapTags;
        mapStatus = data.mapStatus;
        sound = data.sound;
        strenghtMultiplier = data.strenghtMultiplier;
        xPos = data.xPos;
        yPos = data.yPos;
        hintKeys = data.hintKeys;
        usedKeys = data.usedKeys;
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void KeyIncrease()
    {
        this.hintKeys++;
    }

    public void KeyDecrease()
    {
        this.hintKeys--;
    }

    public void ChangeUsedKeys(int index)
    {
        this.usedKeys[index] = this.usedKeys[index] + 1;
    }
}
