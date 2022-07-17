using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZebraPuzzle : MonoBehaviour
{
    public List<GameObject> houses;
    public GameObject houseManager;
    public GameObject rules;
    public ZebraTutorial tutorial; 
    public GameObject message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;

    private List<int> coloursCurrent;
    private int[] coloursSolution;
    private string[] colours;

    private void SetUp()
    {
        for(int i = 0; i < houseManager.transform.childCount; i++)
        {
            Destroy(houseManager.transform.GetChild(i).gameObject);
        }
        coloursCurrent = new List<int>() { 0,0,0,0,0 };
        for(int i = 0; i < 5; i++)
        {
            Instantiate(houses[0], new Vector3(-6.25f + (i * 3.125f), -1.3f,0), Quaternion.identity, houseManager.transform);
            houseManager.transform.GetChild(i).name = colours[0];
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        colours = new string[6] { "Nothing", "Green", "Yellow", "Blue", "Purple", "Pink" };
        coloursSolution = new int[] { 4, 1, 5, 3, 2};
        SetUp();
    }


    private Bounds Bounds(Vector3 pos)
    {
        Bounds bounds = new Bounds(pos, new Vector3(3f, 3f, 0));
        return bounds;
    }
    // Update is called once per frame
    void Update()
    {

        //if (Input.GetMouseButtonDown(0))
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            for(int i = 0; i < 5; i++)
            {
                //Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                Bounds bounds = Bounds(houseManager.transform.GetChild(i).position); 
                //if (bounds.Contains(touchPosition))
                if (bounds.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {
                    int indexPos = IndexPos(houseManager.transform.GetChild(i));
                    Vector3 pos;
                    if(coloursCurrent[indexPos] == 0)
                    {
                        pos = houseManager.transform.GetChild(i).position + new Vector3(0,1,0);
                        coloursCurrent[indexPos]++;

                    }
                    else if(coloursCurrent[indexPos] == 5)
                    {
                        pos = houseManager.transform.GetChild(i).position + new Vector3(0,-1,0);
                        coloursCurrent[indexPos] = 0;

                    }
                    else
                    {
                        pos = houseManager.transform.GetChild(i).position;
                        coloursCurrent[indexPos]++;
                    }
                    Destroy(houseManager.transform.GetChild(i).gameObject);
                    Instantiate(houses[coloursCurrent[indexPos]], pos, Quaternion.identity, houseManager.transform);



                }

            }
            if (CheckWin() && !tutorial.inactive)
            {
                tutorial.inactive = true;
                tutorial.WonPuzzle();
            }

        }
    }

    private bool CheckWin()
    {
        int counter = 0;
        for(int i = 0; i < coloursCurrent.Count; i++)
        {
            if(coloursCurrent[i] == 0)
            {
                return false;
            }
            else if(coloursCurrent[i] == coloursSolution[i])
            {
                counter++;
            }
        }
        if(counter == 5)
        {
            return true;
        }
        return false;
    }

    private int IndexPos(Transform obj)
    {
        int index = 0;
        for (int i = 0; i < 5; i++)
        {
            float pos = -6.25f + (i * 3.125f);
            if (obj.position.x == pos)
            {
                index = i;
            }
        }
        return index;
    }

    public void OpenRules()
    {
        if (!tutorial.inactive)
        {
            rules.SetActive(true);
            tutorial.inactive = true;

        }
    }

    public void CloseRules()
    {
        rules.SetActive(false);
        tutorial.inactive = false;
    }
    
    //Function for the restart button
    public void restart()
    {
        if (!tutorial.inactive)
        {
            SetUp();
        }
    }

    //Function for the leave button
    //Open up a panel to check if the user really wants to leave
    public void leave()
    {
        if (!tutorial.inactive)
        {
            tutorial.inactive = true;
            message.SetActive(true);
        }

    }

    //Function for the yes button, if the user really wants to leave
    public void yes()
    {
        SetUp();
        this.transform.parent.transform.parent.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(true);
        griphoton.SetActive(true);
        player.SetActive(true);
        player.GetComponent<Player>().SwitchCams();
        player.GetComponent<Player>().Unpause();
        message.SetActive(false);
    }

    //Function for the no button, if the user does not want to leave
    public void no()
    {
        tutorial.inactive = false;
        message.SetActive(false);
    }
}
