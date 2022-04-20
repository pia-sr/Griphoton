using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannibalsMissionaries2 : MonoBehaviour
{
    public GameObject wolves;
    public GameObject chicken;
    public GameObject cabbages;
    public GameObject boat;
    public GameObject button;
    public Text counterText;
    public Text message;
    public GameObject griphoton;
    public GameObject player;

    private List<GameObject> onBoat;
    private List<GameObject> left;
    private List<GameObject> right;
    private List<Vector3> wolvesPos;
    private List<Vector3> chickenPos;
    private List<Vector3> cabbagesPos;
    private bool onTheMove;
    private bool boatLeft;
    private GameObject[] boatSeats;
    private int moveCounter;
    private bool inactive;

    // Start is called before the first frame update
    private void setUp()
    {
        left = new List<GameObject>();
        right = new List<GameObject>();
        onBoat = new List<GameObject>();
        onTheMove = false;
        moveCounter = 0;
        for (int i = 0; i < wolves.transform.childCount * 3; i++)
        {

            if (i < 3)
            {
                left.Add(chicken.transform.GetChild(i).gameObject);
            }
            else if (i < 6)
            {
                left.Add(wolves.transform.GetChild(i - 3).gameObject);
            }
            else
            {
                left.Add(cabbages.transform.GetChild(i - 6).gameObject);
            }
        }
        boatSeats = new GameObject[] { null, null, null };
        wolvesPos = new List<Vector3>()
        {
            new Vector3(-6,4,0),
            new Vector3(-5,2.6f,0),
            new Vector3(-7,2.6f,0),
        };
        chickenPos = new List<Vector3>()
        {
            new Vector3(-6,1,0),
            new Vector3(-5,-0.4f,0),
            new Vector3(-7,-0.4f,0)
        };
        cabbagesPos = new List<Vector3>()
        {
            new Vector3(-6,-2,0),
            new Vector3(-5,-3.4f,0),
            new Vector3(-7,-3.4f,0)
        };

        boatLeft = true;
        boat.transform.localPosition = new Vector2(-2.1f, 0);
        counterText.text = moveCounter.ToString();
        inactive = false;
    }
    
    void Start()
    {
        setUp();
    }
    private Rect tile2Rect(GameObject tile)
    {

        Rect rect = new Rect(tile.transform.localPosition.x - 0.4f, tile.transform.localPosition.y -0.4f, 0.8f, 0.8f);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !onTheMove && !inactive)
        {
            GameObject animal;
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for (int i = 0; i < wolves.transform.childCount *3; i++)
            {
                
                if(i < 3)
                {
                    animal = chicken.transform.GetChild(i).gameObject;
                }
                else if(i < 6)
                {
                    animal = wolves.transform.GetChild(i-3).gameObject;
                }
                else
                {
                    animal = cabbages.transform.GetChild(i - 6).gameObject;
                }
                Rect animalRect = tile2Rect(animal);
                if(animalRect.Contains(touchPosition))
                {

                    if (left.Contains(animal) && onBoat.Count < 3 && boatLeft)
                    {
                        left.Remove(animal);
                        onBoat.Add(animal);
                        if (boatSeats[0] == null)
                        {
                            boatSeats[0] = animal;
                        }
                        else if (boatSeats[1] == null)
                        {
                            boatSeats[1] = animal;
                        }
                        else
                        {
                            boatSeats[2] = animal;
                        }
                        boatPos();
                    }
                    else if(right.Contains(animal) && onBoat.Count < 3 && !boatLeft)
                    {
                        right.Remove(animal);
                        onBoat.Add(animal);
                        if (boatSeats[0] == null)
                        {
                            boatSeats[0] = animal;
                        }
                        else if(boatSeats[1] == null)
                        {
                            boatSeats[1] = animal;
                        }
                        else
                        {
                            boatSeats[2] = animal;
                        }
                        boatPos();
                    }
                    else if(onBoat.Contains(animal))
                    {
                        if(boatSeats[0] == animal)
                        {
                            boatSeats[0] = null;
                        }
                        else if(boatSeats[1] == animal)
                        {
                            boatSeats[1] = null;
                        }
                        else
                        {
                            boatSeats[2] = null;
                        }
                        onBoat.Remove(animal);
                        if (boatLeft)
                        {
                            left.Add(animal);
                        }
                        else
                        {
                            right.Add(animal);
                        }
                    }
                }
            }
            if (button.GetComponent<SpriteRenderer>().bounds.Contains(new Vector3(touchPosition.x,touchPosition.y,-0.1f)))
            {
                if (onBoat.Count > 1)
                {
                    onTheMove = true;
                    moveCounter++;
                    counterText.text = moveCounter.ToString();
                }
            }               

        }
        else if (onTheMove)
        {
            if (boatLeft)
            {
                boat.transform.Translate(new Vector3(0,-2.2f,0) * 0.6f * Time.deltaTime);
                for (int i = 0; i < onBoat.Count; i++)
                {
                    onBoat[i].transform.Translate(new Vector3(2.2f,0,0) * 0.6f * Time.deltaTime);
                }
                if(boat.transform.localPosition.x >= 2.1f)
                {

                    onTheMove = false;
                    boatLeft = false;
                    for (int i = 0; i < onBoat.Count; i++)
                    {
                        right.Add(onBoat[i]);
                    }
                    boatSeats[0] = null;
                    boatSeats[1] = null;
                    boatSeats[2] = null;
                    onBoat.Clear();
                }
            }
            else
            {
                boat.transform.Translate(new Vector3(0, 2.2f, 0) * 0.6f * Time.deltaTime);
                for (int i = 0; i < onBoat.Count; i++)
                {
                    onBoat[i].transform.Translate(new Vector3(-2.2f, 0, 0) * 0.6f * Time.deltaTime);
                }
                if (boat.transform.localPosition.x <= -2.1f)
                {

                    onTheMove = false;
                    boatLeft = true;
                    for (int i = 0; i < onBoat.Count; i++)
                    {
                        left.Add(onBoat[i]);
                    }
                    boatSeats[0] = null;
                    boatSeats[1] = null;
                    boatSeats[2] = null;
                    onBoat.Clear();
                }
                
            }
        }positions();
        if (lost())
        {
            message.transform.parent.gameObject.SetActive(true);
            message.transform.parent.transform.GetChild(0).transform.GetComponentInChildren<Text>().text = "Yes";
            message.transform.parent.transform.GetChild(1).transform.GetComponentInChildren<Text>().text = "No";
            message.text = "Game over! \nDo you want to try again?";
        }
        else if (won())
        {
            griphoton.SetActive(true);
            player.SetActive(true);
            griphoton.GetComponent<Upperworld>().setHouseSolved(this.transform.parent.tag);
            this.transform.parent.gameObject.SetActive(false);
        }
    }
    private void positions()
    {
        for(int i = 0; i < left.Count; i++)
        {
            if (left[i].transform.IsChildOf(wolves.transform))
            {
                int index = left[i].transform.GetSiblingIndex();
                left[i].transform.localPosition = wolvesPos[index];
            }
            else if(left[i].transform.IsChildOf(chicken.transform))
            {
                int index = left[i].transform.GetSiblingIndex();
                left[i].transform.localPosition = chickenPos[index];
            }
            else
            {
                int index = left[i].transform.GetSiblingIndex();
                left[i].transform.localPosition = cabbagesPos[index];
            }
        }
        for(int i = 0; i < right.Count; i++)
        {
            if (right[i].transform.IsChildOf(wolves.transform))
            {
                int index = right[i].transform.GetSiblingIndex();
                right[i].transform.localPosition = new Vector3(wolvesPos[index].x *-1 , wolvesPos[index].y, 0);
            }
            else if (right[i].transform.IsChildOf(chicken.transform))
            {
                int index = right[i].transform.GetSiblingIndex();
                right[i].transform.localPosition = new Vector3(chickenPos[index].x * -1, chickenPos[index].y, 0);
            }
            else 
            {
                int index = right[i].transform.GetSiblingIndex();
                right[i].transform.localPosition = new Vector3(cabbagesPos[index].x * -1, cabbagesPos[index].y, 0);
            }
        }
    }
    private void boatPos()
    {
        float seat1;
        float seat2;
        float seat3;
        if (boatLeft)
        {
            seat1 = -3.1f;
            seat2 = -2.2f;
            seat3 = -1.3f;
        }
        else
        {
            seat1 = 2.9f;
            seat2 = 2f;
            seat3 = 1.1f;
        }
        if(boatSeats[0] != null)
        {
            boatSeats[0].transform.localPosition = new Vector3(seat1, 0.43f, -0.1f);
        }
        if(boatSeats[1] != null)
        {
            boatSeats[1].transform.localPosition = new Vector3(seat2, 0.43f, -0.1f);
        }
        if(boatSeats[2] != null)
        {
            boatSeats[2].transform.localPosition = new Vector3(seat3, 0.43f, -0.1f);
        }
    }
    private bool lost()
    {
        if (moveCounter > 13)
        {
            return true;
        }
        if (onBoat.Count == 0)
        {
            for(int j = 0; j < 2; j++)
            {
                List<GameObject> side = new List<GameObject>();
                int wolfCounter = 0;
                int chickenCounter = 0;
                int cabbageCounter = 0;

                if (j == 0)
                {
                    side = left;
                }
                else if(j == 1)
                {

                    side = right;
                }
                for (int i = 0; i < side.Count; i++)
                {
                    if (side[i].transform.IsChildOf(wolves.transform))
                    {
                        wolfCounter++;
                    }
                    else if (side[i].transform.IsChildOf(chicken.transform))
                    {
                        chickenCounter++;
                    }
                    else if (side[i].transform.IsChildOf(cabbages.transform))
                    {
                        cabbageCounter++;
                    }
                }
                if (chickenCounter != 0 && wolfCounter > chickenCounter)
                {
                    return true;
                }
                else if (cabbageCounter != 0 && chickenCounter > cabbageCounter)
                {
                    return true;
                }
            }

        }
        return false;
    }
    private bool won()
    {
        if(left.Count == 0 && onBoat.Count == 0)
        {
            return true;
        }
        return false;
    }
    public void restart()
    {
        setUp();
    }
    public void leave()
    {
        griphoton.SetActive(true);
        player.SetActive(true);
        setUp();
        this.transform.parent.gameObject.SetActive(false);
    }
}
