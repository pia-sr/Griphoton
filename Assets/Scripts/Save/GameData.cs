using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to serialize the variables from the game
//source:https://www.youtube.com/watch?v=XOjd_qU2Ido
[System.Serializable]
public class GameData
{
    public bool tutorial;
    public int activeLevel;
    public string namePlayer;
    public string[] nodeTags;
    public bool sound;
    public int strenghtMultiplier;
    public int xPos;
    public int yPos;

    public GameData(Game game)
    {
        tutorial = game.tutorial;
        activeLevel = game.activeLevel;
        namePlayer = game.namePlayer;
        nodeTags = game.getGrid();
        sound = game.sound;
        strenghtMultiplier = game.strenghtMultiplier;
        xPos = game.xPos;
        yPos = game.yPos;
    }
}
