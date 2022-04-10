using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool tutorial;
    public int activeLevel;
    public string namePlayer;
    public string[] nodeTags;

    public GameData(Game game)
    {
        tutorial = game.tutorial;
        activeLevel = game.activeLevel;
        namePlayer = game.namePlayer;
        nodeTags = game.getGrid();
    }

    
}
