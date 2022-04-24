using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float distStick;
    private float distRotate;
    private float distMove;
    private bool moveStick;
    public Text message;
    public GameObject griphoton;
    public GameObject player;
    public MatchstickTutorial2 tutorial;
    public GameObject messageExit;
    private List<Vector3> positions;
    private List<float> rotations;

    private void setUp()
    {
        moveStick = false;
        selected = false;
        movedSticks = new List<GameObject>();
        selectedStick = null;
        matchsticks = new List<GameObject>();
        for(int i = 0; i < matchstickManager.transform.childCount; i++)
        {

            matchstickManager.transform.GetChild(i).transform.localPosition = positions[i];
            matchsticks.Add(matchstickManager.transform.GetChild(i).gameObject); 
            var rotation = matchstickManager.transform.GetChild(i).transform.localRotation.eulerAngles;
            rotation.z = rotations[i];
            matchstickManager.transform.GetChild(i).transform.localRotation = Quaternion.Euler(rotation);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        positions = new List<Vector3>();
        rotations = new List<float>();
        for (int i = 0; i < matchstickManager.transform.childCount; i++)
        {
            positions.Add(matchstickManager.transform.GetChild(i).transform.localPosition);
            rotations.Add(matchstickManager.transform.GetChild(i).transform.localRotation.eulerAngles.z);
        }
        setUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {

            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

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
            else if (touch.phase == TouchPhase.Began)
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
                                tutorial.inactive = true;
                                message.transform.parent.gameObject.SetActive(true);
                                message.text = "You already moved 3 matchsticks! \nYou can reset the matchsticks with the reset button, if you want to move a different one.";
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

            else if (touch.phase == TouchPhase.Ended)
            {
                activeMove = false;
                moveStick = false;
            }


        }
        if (won())
        {

            griphoton.SetActive(true);
            player.SetActive(true);
            griphoton.GetComponent<Upperworld>().setHouseSolved(this.transform.parent.transform.parent.tag);
            this.transform.parent.transform.parent.gameObject.SetActive(false);
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

    public void restart()
    {
        if (!tutorial.inactive)
        {
            setUp();
        }
    }
    public void leave()
    {
        if (!tutorial.inactive)
        {
            tutorial.inactive = true;
            messageExit.SetActive(true);
        }

    }

    public void yes()
    {
        setUp();
        griphoton.SetActive(true);
        player.SetActive(true);
        messageExit.SetActive(false);
        tutorial.gameObject.SetActive(true);
        tutorial.setUp();
        this.transform.parent.transform.parent.gameObject.SetActive(false);
    }
    public void no()
    {
        tutorial.inactive = false;
        messageExit.SetActive(false);
    }

    public void close()
    {
        message.transform.parent.gameObject.SetActive(false);
        tutorial.inactive = false;
    }
}
