using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Class to read or write a file to save the game data
//source: https://www.youtube.com/watch?v=XOjd_qU2Ido
public static class SaveSystem 
{
    public static void saveGame(Game game)
    {
        FileStream stream = null;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameData.game";
        if (!System.IO.File.Exists("gameDat.game"))
        {

            stream = new FileStream(path, FileMode.Create);
        }

        GameData data = new GameData(game);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    public static GameData loadGame()
    {
        string path = Application.persistentDataPath + "/gameData.game";
        FileStream stream = new FileStream(path, FileMode.Open);
        if (File.Exists(path) && stream.Length > 0)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            GameData data = (GameData)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}
