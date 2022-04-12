using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchstickPuzzle2 : MonoBehaviour
{
    public GameObject matchstickManager;
    public GameObject rotateButton;
    public GameObject moveButton;
    private GameObject selectedStick;
    private List<GameObject> matchsticks;
    private Bounds selectedBounds;
    private bool selected;
    private bool activeMove;
    private List<GameObject> movedSticks;
    private bool lost;
    private float distStick;
    private float distRotate;
    private float distMove;
    private bool moveStick;



    // Start is called before the first frame update
    void Start()
    {
        movedSticks = new List<GameObject>();
        selectedStick = null;
        matchsticks = new List<GameObject>();
        for(int i = 0; i < matchstickManager.transform.childCount; i++)
        {
            matchsticks.Add(matchstickManager.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !won() && !lost)
        {

            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (moveStick)
            {
                if (!movedSticks.Contains(selectedStick))
                {
                    movedSticks.Add(selectedStick);
                }
                if (!activeMove)
                {
                    distStick = touchPosition.x - selectedStick.transform.localPosition.x;
                    distRotate = touchPosition.x - rotateButton.transform.localPosition.x;
                    distMove = touchPosition.x - moveButton.transform.localPosition.x;
                }
                activeMove = true;
                moveButton.transform.localPosition = touchPosition - new Vector2(distMove,0);
                selectedStick.transform.localPosition = touchPosition - new Vector2(distStick,0);
                rotateButton.transform.localPosition = touchPosition - new Vector2(distRotate, 0);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (!selected)
                {

                    foreach (GameObject matchstick in matchsticks)
                    {
                        var rend = matchstick.GetComponent<SpriteRenderer>();

                        if (rend.bounds.Contains(touchPosition))
                        {
                            if(movedSticks.Count == 3 && !movedSticks.Contains(matchstick))
                            {
                                lost = true;
                            }
                            selected = true;
                            selectedBounds = new Bounds(matchstick.transform.localPosition, matchstick.transform.localScale);
                            rotateButton.SetActive(true);
                            rotateButton.transform.localPosition = matchstick.transform.position + new Vector3(-1, 0, 0);
                            moveButton.SetActive(true);
                            moveButton.transform.localPosition = matchstick.transform.position + new Vector3(1, 0, 0);
                            selectedBounds.Encapsulate(rotateButton.GetComponent<SpriteRenderer>().bounds);
                            selectedBounds.Encapsulate(moveButton.GetComponent<SpriteRenderer>().bounds);
                            selectedStick = matchstick;

                        }

                    }


                }
                else
                {
                    
                    if (rotateButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        if (!movedSticks.Contains(selectedStick))
                        {
                            movedSticks.Add(selectedStick);
                        }
                        var rotation = selectedStick.transform.localRotation.eulerAngles;
                            float rotationZ = rotation.z;
                        //rotationZ -= 30;
                        if (rotation.x <= 180f)
                        {
                            rotationZ += 30;
                        }
                        else
                        {
                            rotationZ -= 330;
                        }
                        rotation.z = rotationZ;
                            selectedStick.transform.localRotation = Quaternion.Euler(rotation);
                        
                    }
                    else if (moveButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        moveStick = true;
                    }
                    else
                    {
                        moveButton.SetActive(false);
                        rotateButton.SetActive(false);
                        selected = false;
                        selectedStick = null;
                    }
                }
            }
            


        }
        if (Input.GetMouseButtonUp(0))
        {
            activeMove = false;
            moveStick = false;
        }
        if (won())
        {
          
            Debug.Log("Won!");
        }
        else if (lost)
        {
            Debug.Log("Lost!");
        }
    }
    
    private bool won()
    {
        
        Bounds rightArea1 = new Bounds(new Vector2(0,-0.2f), new Vector2(1, 1f));
        Bounds rightArea2 = new Bounds(new Vector2(7,-0.25f), new Vector2(1f, 1.3f));
        List<GameObject> position2 = new List<GameObject>();

        bool pos1 = false;
        bool pos2 = false;
        
        if (!activeMove && movedSticks.Count == 3)
        {
            foreach(GameObject matchstick in movedSticks)
            {
                if((Mathf.Round(matchstick.transform.rotation.z * 100) * 0.01f).ToString() == "0.71")
                {
                    if (rightArea1.Contains(matchstick.transform.localPosition))
                    {
                        pos1 = true;
                    }
                    else if (rightArea2.Contains(matchstick.transform.localPosition))
                    {
                        if (!position2.Contains(matchstick))
                        {
                            position2.Add(matchstick);
                        }
                    }
                }
            }
            if(position2.Count == 2)
            {
                float dist = position2[0].transform.position.y - position2[1].transform.position.y;
                if(dist < 0)
                {
                    dist *= -1;
                }
                if(dist > 0.25f)
                {
                    pos2 = true;
                }
            }
            if(pos1 && pos2)
            {
                return true;
            }

        }
        return false;
    }
}
