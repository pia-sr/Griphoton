using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public bool tutorial;
    public int activeLevel;
    public string namePlayer;

    public string[] nodeTags;

    public void setLevel(int level)
    {
        this.activeLevel = level;
    }

    public void setName(string name)
    {
        this.namePlayer = name;
    }

    public void SaveGame()
    {
        SaveSystem.saveGame(this);
    }
    public string[] getGrid()
    {
        GridField grid = FindObjectOfType<Upperworld>().gameObject.GetComponent<GridField>();
        string[] tags = new string[grid.GetMaxGridSize];
        int counter = 0;
        foreach (Node node in grid.grid)
        {
            tags[counter] = node.onTop;
            counter++;
        }
        return tags;
    }

    public void loadGame()
    {
        GameData data = SaveSystem.loadGame();

        tutorial = data.tutorial;
        activeLevel = data.activeLevel;
        namePlayer = data.namePlayer;
        nodeTags = data.nodeTags;

    }
}
