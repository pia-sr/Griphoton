/*
 * MatchstickPuzzle1.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchstickPuzzle1 : MonoBehaviour
{
    //public field objects
    public GameObject matchstickManager;
    public GameObject rotateButton;
    public GameObject moveButton;
    public Text message;
    public GameObject messageExit;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public MatchstickTutorial1 tutorial;

    //private variables
    private GameObject _selectedStick;
    private List<GameObject> _matchsticks;
    private Bounds _selectedBounds;
    private bool _selected;
    private bool _activeMove;
    private GameObject _movedStick;
    private Vector2 _distStick;
    private Vector2 _distRotate;
    private Vector2 _distMove;
    private bool _moveStick;
    private List<Vector3> _positions;
    private List<float> _rotations;


    // Start is called before the first frame update
    void Start()
    {
        _positions = new List<Vector3>();
        _rotations = new List<float>();
        for (int i = 0; i< matchstickManager.transform.childCount; i++)
        {
            _positions.Add(matchstickManager.transform.GetChild(i).transform.localPosition);
            _rotations.Add(matchstickManager.transform.GetChild(i).transform.localRotation.eulerAngles.z);
        }
        SetUp();
    }

    //Function to set matchsticks to original position and rotation
    private void SetUp()
    {
        moveButton.SetActive(false);
        rotateButton.SetActive(false);
        _movedStick = null;
        _selectedStick = null;
        _matchsticks = new List<GameObject>();
        for(int i = 0; i < matchstickManager.transform.childCount; i++)
        {
            matchstickManager.transform.GetChild(i).transform.localPosition = _positions[i];
            _matchsticks.Add(matchstickManager.transform.GetChild(i).gameObject);
            var rotation = matchstickManager.transform.GetChild(i).transform.localRotation.eulerAngles;
            rotation.z = _rotations[i];
            matchstickManager.transform.GetChild(i).transform.localRotation = Quaternion.Euler(rotation);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {

            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            //if the user touches the move button the matchstick moves with the user's touch
            if (_moveStick)
            {
                _movedStick = _selectedStick;
                if (!_activeMove)
                {
                    _distStick.x = touchPosition.x - _selectedStick.transform.localPosition.x;
                    _distStick.y = touchPosition.y - _selectedStick.transform.localPosition.y;
                    _distRotate.x = touchPosition.x - rotateButton.transform.localPosition.x;
                    _distRotate.y = touchPosition.y - rotateButton.transform.localPosition.y;
                    _distMove.x = touchPosition.x - moveButton.transform.localPosition.x;
                    _distMove.y = touchPosition.y - moveButton.transform.localPosition.y;
                }
                _activeMove = true;
                moveButton.transform.localPosition = touchPosition - _distMove;
                _selectedStick.transform.localPosition = touchPosition - _distStick;
                rotateButton.transform.localPosition = touchPosition - _distRotate;
            }
            //waits for user to either select or rotate a matchstick
            else if (touch.phase == TouchPhase.Began)
            {
                if (!_selected)
                {

                    foreach (GameObject matchstick in _matchsticks)
                    {
                        var rend = matchstick.GetComponent<SpriteRenderer>();

                        if (rend.bounds.Contains(touchPosition))
                        {
                            if(_movedStick != null && _movedStick != matchstick)
                            {
                                tutorial.inactive = true;
                                message.transform.parent.transform.parent.gameObject.SetActive(true);
                                message.text = "You already moved a matchstick! \nYou can reset the matchsticks with the reset button, if you want to move a different one.";
                            }
                            _selected = true;
                            _selectedBounds = new Bounds(matchstick.transform.localPosition, matchstick.transform.localScale);
                            rotateButton.SetActive(true);
                            rotateButton.transform.localPosition = matchstick.transform.position + new Vector3(-1, 0, 0);
                            moveButton.SetActive(true);
                            moveButton.transform.localPosition = matchstick.transform.position + new Vector3(1, 0, 0);
                            _selectedBounds.Encapsulate(rotateButton.GetComponent<SpriteRenderer>().bounds);
                            _selectedBounds.Encapsulate(moveButton.GetComponent<SpriteRenderer>().bounds);
                            _selectedStick = matchstick;

                        }

                    }


                }
                else
                {
                    
                    if (rotateButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        _movedStick = _selectedStick;
                        var rotation = _selectedStick.transform.localRotation.eulerAngles;
                        float rotationZ = rotation.z;
                        if (rotation.x <= 180f)
                        {
                            rotationZ += 30;
                        }
                        else
                        {
                            rotationZ -= 330;
                        }
                        rotation.z = rotationZ;
                        _selectedStick.transform.localRotation = Quaternion.Euler(rotation);

                    }
                    else if (moveButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        _moveStick = true;
                    }
                    else
                    {
                        moveButton.SetActive(false);
                        rotateButton.SetActive(false);
                        _selected = false;
                        _selectedStick = null;
                    }
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                _activeMove = false;
                _moveStick = false;
            }



        }
        if (CheckWin() && !tutorial.inactive)
        {
            tutorial.inactive = true;
            tutorial.WonPuzzle();
        }
    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        Bounds rightArea = new Bounds(_matchsticks[2].transform.localPosition, new Vector2(1.5f, 0.75f));

        if (!_activeMove)
        {
            if ((Mathf.Round(_matchsticks[0].transform.rotation.z * 100) * 0.01f) == 0 || (Mathf.Round(_matchsticks[0].transform.rotation.z * 100) * 0.01f).ToString() == "1")
            {
                if(rightArea.Contains(_matchsticks[0].transform.localPosition))
                {
                    return true;
                }
            }
        }
        return false;
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
            messageExit.SetActive(true);
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
        messageExit.SetActive(false);
    }

    //Function for the no button, if the user does not want to leave
    public void no()
    {
        tutorial.inactive = false;
        messageExit.SetActive(false);
    }

    //Function to close the message after user violated one of the rules
    public void close()
    {
        message.transform.parent.transform.parent.gameObject.SetActive(false);
        tutorial.inactive = false;
    }
}
