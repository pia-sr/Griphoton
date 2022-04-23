using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public bool tutorial;
    public int activeLevel;
    public string namePlayer;
    public bool sound;

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
        string[] tags;
        Player player = GameObject.Find("Player").GetComponent<Player>();
        if (player.upperWorld)
        {

            GridField grid = GameObject.Find("Grid").GetComponent<GridField>();
            tags = new string[grid.GetMaxGridSize];
            int counter = 0;
            foreach (Node node in grid.grid)
            {

                if (node.onTop == "Erin")
                {
                    Debug.Log("Not the problem here");
                }
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

    public void loadGame()
    {
        GameData data = SaveSystem.loadGame();

        tutorial = data.tutorial;
        activeLevel = data.activeLevel;
        namePlayer = data.namePlayer;
        nodeTags = data.nodeTags;
        sound = data.sound;

    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
