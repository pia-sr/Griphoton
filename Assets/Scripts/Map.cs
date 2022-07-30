using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{

    public GridField grid;
    public GameObject tileManager;
    private Game _data;
    public GameObject player;
    public GameObject house;
    public GameObject houseManager;
    public GameObject hat;
    public GameObject canvas;
    private GameObject playerHat;
    public Camera camera;
    public Text text;
    public Font font;
    public bool update;


    //On awke the player looks for the game data
    private void Awake()
    {
        _data = GameObject.Find("GameData").GetComponent<Game>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
                newHouse.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _data.mapTags[counter];
            }
            counter++;

        }
    }


    private int getHouseNumber(string tag)
    {
        int counter = 0;
        for(int i = 0; i < houseManager.transform.childCount; i++)
        {
            if(houseManager.transform.GetChild(i).name == tag)
            {
                break;
            }
            else
            {
                counter++;
            }
        }
        return counter;
    }


    public void AddTag(int x, int y, string tag)
    {
        Node node = grid.grid[x, y];
        GameObject newHouse = Instantiate(house, node.worldPosition, Quaternion.identity, tileManager.transform);
        newHouse.name = tag+ "Map";
        newHouse.transform.GetChild(0).GetComponent<Canvas>().worldCamera = camera;
        newHouse.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = tag;
    }

    public void SetSolvedTag(string prevTag)
    {
        GameObject solvedHouse = GameObject.Find(prevTag + "Map");
        solvedHouse.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Solved";

    }

    private bool TouchNeigbourText(GameObject text)
    {
        if(canvas.transform.childCount > 1)
        {
            for(int i = 0; i < canvas.transform.childCount-1; i++)
            {
                Rect rectN = new Rect(canvas.transform.GetChild(i).transform.position.x - 40, canvas.transform.GetChild(i).transform.position.y - 10, 80, 20);
                Rect rectT = new Rect(text.transform.position.x - 40, text.transform.position.y - 10, 80, 20);
                if (rectN.Contains(rectT.min) || rectN.Contains(rectT.max))
                {
                    return true;
                }
            
            }

        }
        return false;
    }

}
