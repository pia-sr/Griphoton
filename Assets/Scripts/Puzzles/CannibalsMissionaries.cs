using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannibalsMissionaries : MonoBehaviour
{
    //Animals and boat
    public GameObject snakes;
    public GameObject mice;
    public GameObject boat;

    //UI
    public GameObject moveButton;
    public Text counterText;
    public Text messageLost;
    public GameObject messageExit;

    //Public Gameobjects to switch between Griphoton and the puzzle
    public GameObject griphoton;
    public GameObject player;
    public CMTutorial tutorial;

    //private variables for the puzzle
    private List<GameObject> _onBoat;
    private List<GameObject> _leftSide;
    private List<GameObject> _rightSide;
    private List<Vector3> _snakesPos;
    private List<Vector3> _micePos;
    private GameObject[] _boatSeats;
    private bool _onTheMove;
    private bool _boatIsLeft;
    private int _moveCounter;

    //Once the puzzle starts, the puzzle is set up to its start state
    void Start()
    {
        SetUp();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if the player touches the screen and the boat is currently not moving, the tutorial is also done
        //and rules of the puzzle have not been violated, an action can be carried out
        if (Input.touchCount > 0 && !_onTheMove && !tutorial.inactive && !CheckLost())
        {
            GameObject animal;
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            //Interates through the animals to check if one of them has been touched
            for (int i = 0; i < snakes.transform.childCount *2; i++)
            {
                
                if(i < 3)
                {
                    animal = mice.transform.GetChild(i).gameObject;
                }
                else
                {
                    animal = snakes.transform.GetChild(i-3).gameObject;
                }
                Rect animalRect = Object2Rect(animal);
                
                //If an animals has been touched it either will be put on the boat or removed from it
                //But only if the boat is on the same side as that animal
                if(animalRect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                {

                    if (_leftSide.Contains(animal) && _onBoat.Count < 2 && _boatIsLeft)
                    {
                        _leftSide.Remove(animal);
                        _onBoat.Add(animal);
                        if (_boatSeats[0] == null)
                        {
                            _boatSeats[0] = animal;
                        }
                        else
                        {
                            _boatSeats[1] = animal;
                        }
                        PositionsBoat();
                    }
                    else if(_rightSide.Contains(animal) && _onBoat.Count < 2 && !_boatIsLeft)
                    {
                        _rightSide.Remove(animal);
                        _onBoat.Add(animal);
                        if (_boatSeats[0] == null)
                        {
                            _boatSeats[0] = animal;
                        }
                        else
                        {
                            _boatSeats[1] = animal;
                        }
                        PositionsBoat();
                    }
                    else if(_onBoat.Contains(animal))
                    {
                        if(_boatSeats[0] == animal)
                        {
                            _boatSeats[0] = null;
                        }
                        else
                        {
                            _boatSeats[1] = null;
                        }
                        _onBoat.Remove(animal);
                        if (_boatIsLeft)
                        {
                            _leftSide.Add(animal);
                        }
                        else
                        {
                            _rightSide.Add(animal);
                        }
                    }
                }
            }
            //If instead the button to move the boat is pressed and there is at least one animal on it
            //the boat will move to the other side
            if (moveButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
            {
                if (_onBoat.Count != 0)
                {
                    _onTheMove = true;
                    _moveCounter++;
                    counterText.text = _moveCounter.ToString();
                }
            }               

        }
        //The boat will move to the other side with all the animal on it and will rotate once all the 
        //animals are of the boat
        else if (_onTheMove)
        {
            if (_boatIsLeft)
            {
                boat.transform.Translate(new Vector3(2.2f, 0, 0) * 0.6f * Time.deltaTime);
                for (int i = 0; i < _onBoat.Count; i++)
                {
                    _onBoat[i].transform.Translate(new Vector3(2.2f,0,0) * 0.6f * Time.deltaTime);
                }
                if(boat.transform.localPosition.x >= 2.2)
                {

                    _onTheMove = false;
                    _boatIsLeft = false;
                    for (int i = 0; i < _onBoat.Count; i++)
                    {
                        _rightSide.Add(_onBoat[i]);
                    }
                    _boatSeats[0] = null;
                    _boatSeats[1] = null;
                    _onBoat.Clear(); 
                    var rotation = boat.transform.localRotation.eulerAngles;
                    rotation.z = 180;
                    boat.transform.localRotation = Quaternion.Euler(rotation);
                }
            }
            else
            {
                boat.transform.Translate(new Vector3(2.2f, 0, 0) * 0.6f * Time.deltaTime);
                for (int i = 0; i < _onBoat.Count; i++)
                {
                    _onBoat[i].transform.Translate(new Vector3(-2.2f, 0, 0) * 0.6f * Time.deltaTime);
                }
                if (boat.transform.localPosition.x <= -2.2)
                {

                    _onTheMove = false;
                    _boatIsLeft = true;
                    for (int i = 0; i < _onBoat.Count; i++)
                    {
                        _leftSide.Add(_onBoat[i]);
                    }
                    _boatSeats[0] = null;
                    _boatSeats[1] = null;
                    _onBoat.Clear(); 
                    var rotation = boat.transform.localRotation.eulerAngles;
                    rotation.z = 0;
                    boat.transform.localRotation = Quaternion.Euler(rotation);
                }
                
            }
        }
        PositionsRiversides();

        //If the player has violated at least one of the rules of the puzzle, they will get a message to either restart or leave the game
        if (CheckLost())
        {
            messageLost.transform.parent.transform.parent.gameObject.SetActive(true);
            messageLost.transform.parent.transform.GetChild(0).transform.GetComponentInChildren<Text>().text = "Yes";
            messageLost.transform.parent.transform.GetChild(1).transform.GetComponentInChildren<Text>().text = "No";
            messageLost.text = "Game over! \nDo you want to try again?";
        }
        //if the player has won the game, they will leave the house and the puzzle will be set as solved
        else if (CheckWin() && !tutorial.inactive)
        {
            tutorial.inactive = true;
            tutorial.WonPuzzle();
        }
    }

    //Function to set up the puzzle
    private void SetUp()
    {
        messageLost.transform.parent.gameObject.SetActive(false);
        _leftSide = new List<GameObject>();
        _rightSide = new List<GameObject>();
        _onBoat = new List<GameObject>();
        _onTheMove = false;
        _moveCounter = 0;
        _boatSeats = new GameObject[] { null, null };
        counterText.text = _moveCounter.ToString();

        //All animals and the boat are at the beginning on the left riverside
        _boatIsLeft = true;
        for (int i = 0; i < snakes.transform.childCount * 2; i++)
        {

            if (i < 3)
            {
                _leftSide.Add(mice.transform.GetChild(i).gameObject);
            }
            else
            {
                _leftSide.Add(snakes.transform.GetChild(i - 3).gameObject);
            }
        }
        _snakesPos = new List<Vector3>()
        {
            new Vector3(-6,2,0),
            new Vector3(-5,0.6f,0),
            new Vector3(-7,0.6f,0),
        };
        _micePos = new List<Vector3>()
        {
            new Vector3(-6,-1,0),
            new Vector3(-5,-2.4f,0),
            new Vector3(-7,-2.4f,0)
        };
        boat.transform.localPosition = new Vector3(-2.2f,0, 0);
        var rotation = boat.transform.localRotation.eulerAngles;
        rotation.z = 0;
        boat.transform.localRotation = Quaternion.Euler(rotation);
    }

    //Function to create a rectagle on top of a given object to set rectangular boundaries for that object
    private Rect Object2Rect(GameObject tile)
    {

        Rect rect = new Rect(tile.transform.localPosition.x - 0.4f, tile.transform.localPosition.y -0.4f, 0.8f, 0.8f);
        return rect;
    }

    //Function to set the position of the animals on the riversides
    private void PositionsRiversides()
    {
        for(int i = 0; i < _leftSide.Count; i++)
        {
            if (_leftSide[i].transform.IsChildOf(snakes.transform))
            {
                int index = _leftSide[i].transform.GetSiblingIndex();
                _leftSide[i].transform.localPosition = _snakesPos[index];
            }
            else
            {
                int index = _leftSide[i].transform.GetSiblingIndex();
                _leftSide[i].transform.localPosition = _micePos[index];
            }
        }
        for(int i = 0; i < _rightSide.Count; i++)
        {
            if (_rightSide[i].transform.IsChildOf(snakes.transform))
            {
                int index = _rightSide[i].transform.GetSiblingIndex();
                _rightSide[i].transform.localPosition = new Vector3(_snakesPos[index].x *-1 , _snakesPos[index].y, 0);
            }
            else
            {
                int index = _rightSide[i].transform.GetSiblingIndex();
                _rightSide[i].transform.localPosition = new Vector3(_micePos[index].x * -1, _micePos[index].y, 0);
            }
        }
    }
    
    //Function to set the position of the animals on the boat
    private void PositionsBoat()
    {
        float seat1;
        float seat2;
        if (_boatIsLeft)
        {
            seat1 = -2.8f;
            seat2 = -1.6f;
        }
        else
        {
            seat1 = 2.9f;
            seat2 = 1.8f;
        }
        if(_boatSeats[0] != null)
        {
            _boatSeats[0].transform.localPosition = new Vector3(seat1, 0.43f, -0.1f);
        }
        if(_boatSeats[1] != null)
        {
            _boatSeats[1].transform.localPosition = new Vector3(seat2, 0.43f, -0.1f);
        }
    }

    //Function to determine if the player has lost the puzzle
    //If the player has moved the boat more than 11 times lost will be set as true
    //If there are more snakes than mice on either riverside, lost will be set as true
    private bool CheckLost()
    {
        if(_moveCounter > 11)
        {
            return true;
        }
        if (_onBoat.Count == 0)
        {
            for (int j = 0; j < 2; j++)
            {
                List<GameObject> side = new List<GameObject>();
                int snakeCounter = 0;
                int mouseCounter = 0;

                if (j == 0)
                {
                    side = _leftSide;
                }
                else if (j == 1)
                {

                    side = _rightSide;
                }
                for (int i = 0; i < side.Count; i++)
                {
                    if (side[i].transform.IsChildOf(snakes.transform))
                    {
                        snakeCounter++;
                    }
                    else if (side[i].transform.IsChildOf(mice.transform))
                    {
                        mouseCounter++;
                    }
                }
                if (mouseCounter != 0 && snakeCounter > mouseCounter)
                {
                    return true;
                }
            }

        }
        return false;
    }

    //If the player has not lost and all the animals are on the right side, the player has solved the puzzle
    private bool CheckWin()
    {
        if(_leftSide.Count == 0 && _onBoat.Count == 0)
        {
            return true;
        }
        return false;
    }

    //Function of the restart button which sets the puzzle to its start state
    public void RestartPuzzle()
    {
        if (!tutorial.inactive)
        {
            SetUp();

        }
        if (messageLost.transform.parent.parent.gameObject.activeSelf)
        {
            messageLost.transform.parent.parent.gameObject.SetActive(false);
        }
    }

    //Function of the leave button to open up a panel if the player really wants to leave
    public void leave()
    {
        if (!tutorial.inactive)
        {
            tutorial.inactive = true;
            messageExit.SetActive(true);
        }
        if (messageLost.transform.parent.parent.gameObject.activeSelf)
        {
            messageLost.transform.parent.parent.gameObject.SetActive(false);
        }

    }

    //Function of the yes button of the panel
    //If the player presses yes the whole is being reseted and the player leaves the house
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

    //Function of the no button of the panel to close the panel again and to go back to the puzzle
    public void no()
    {
        tutorial.inactive = false;
        messageExit.SetActive(false);
    }
}
