using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyAdd1 : MonoBehaviour
{
    //public field objects
    public List<GameObject> shapes;
    public GameObject shapeEq1;
    public GameObject shapeEq2;
    public GameObject rotateButton;
    public GameObject mirrorButton;
    public GameObject messageExit;
    public GridField grid;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public RegionDivisionTutorial tutorial;

    //private variables
    private GameObject _selectedShape;
    private bool _selected;
    private bool _activeMove;
    private Vector2 _distStick;
    private Vector2 _distRotate;
    private Vector2 _distMove;
    private bool _moveStick;
    private List<Vector3> _positions;
    private List<Node> _selectedNodesLeft;
    private List<Node> _selectedNodesRight;
    private int _squareCount;


    // Start is called before the first frame update
    void Start()
    {
        _positions = new List<Vector3>();
        ShapePos();
        GetNodes();
        _squareCount = _selectedNodesLeft.Count;
        foreach(GameObject shape in shapes)
        {
            _positions.Add(shape.transform.localPosition);
        }
        SetUp();
    }

    //Function to set matchsticks to original position and rotation
    private void SetUp()
    {
        mirrorButton.SetActive(false);
        rotateButton.SetActive(false);
        _selectedShape = null;
        _selected = false;
        _moveStick = false;
        int counter = 0;
        foreach(GameObject shape in shapes)
        {
            //shape.transform.localPosition = _positions[counter];
            shape.transform.localRotation = Quaternion.Euler(Vector3.zero);
            counter++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        //if (Input.touchCount > 0 && !tutorial.inactive)
        {

            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Touch touch = Input.GetTouch(0);
            //Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            //if the user touches the move button the matchstick moves with the user's touch
            if (_moveStick)
            {
                if (!_activeMove)
                {
                    _distStick.x = touchPosition.x - _selectedShape.transform.localPosition.x;
                    _distStick.y = touchPosition.y - _selectedShape.transform.localPosition.y;
                    _distRotate.x = touchPosition.x - rotateButton.transform.localPosition.x;
                    _distRotate.y = touchPosition.y - rotateButton.transform.localPosition.y;
                    _distMove.x = touchPosition.x - mirrorButton.transform.localPosition.x;
                    _distMove.y = touchPosition.y - mirrorButton.transform.localPosition.y;
                }
                _activeMove = true;
                mirrorButton.transform.localPosition = touchPosition - _distMove;
                _selectedShape.transform.localPosition = touchPosition - _distStick;
                rotateButton.transform.localPosition = touchPosition - _distRotate;
            }
            //waits for user to either select or rotate a matchstick
            else if (Input.GetMouseButtonDown(0))
            //else if (touch.phase == TouchPhase.Began)
            {
                if (!_selected)
                {
                    foreach (GameObject shape in shapes)
                    {
                        var bounds = shape.GetComponent<SpriteRenderer>().bounds;
                        if (bounds.Contains(touchPosition))
                        {
                            _selected = true;
                            rotateButton.SetActive(true);
                            rotateButton.transform.localPosition = shape.transform.position + new Vector3(-1.5f, 0, 0);
                            mirrorButton.SetActive(true);
                            mirrorButton.transform.localPosition = shape.transform.position + new Vector3(1.5f, 0, 0);
                            _selectedShape = shape;

                        }

                    }
                }

                else
                {

                    if (rotateButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        var rotation = _selectedShape.transform.localRotation.eulerAngles;
                        float rotationZ = rotation.z;
                        rotationZ += 90;

                        rotation.z = rotationZ;
                        _selectedShape.transform.localRotation = Quaternion.Euler(rotation);

                    }
                    else if (mirrorButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {

                        var rotation = _selectedShape.transform.localRotation.eulerAngles;
                        float rotationY = rotation.y;
                        rotationY += 180;
                        rotation.y = rotationY;
                        _selectedShape.transform.localRotation = Quaternion.Euler(rotation);
                    }
                    else if(_selectedShape.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        _moveStick = true;
                    }
                    else
                    {
                        mirrorButton.SetActive(false);
                        rotateButton.SetActive(false);
                        _selected = false;
                        _selectedShape = null;

                    }
                

                }

            }/*
            if (Input.GetMouseButtonUp(0))
            //if (touch.phase == TouchPhase.Ended)
            {
                _activeMove = false;
                _moveStick = false;
            }*/



        }
        if (CheckWin() && !tutorial.inactive)
        {
            Debug.Log("Won!");
            //tutorial.inactive = true;
           //tutorial.WonPuzzle();
        }
        if (Input.GetMouseButtonUp(0))
        //if (touch.phase == TouchPhase.Ended)
        {
            _activeMove = false;
            _moveStick = false;
        }
    }

    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        GetNodes();
        if(_selectedNodesLeft.Count != _squareCount || _selectedNodesRight.Count != _squareCount)
        {
            return false;
        }
        else if (shapeEq1.GetComponent<CompositeCollider2D>().shapeCount != 1 || shapeEq2.GetComponent<CompositeCollider2D>().shapeCount != 1)
        {
            return false;
        }
        else if (!SameShape())
        {
            return false;
        }
        return true;
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
    
    private void GetNodes()
    {
        _selectedNodesLeft = new List<Node>();
        _selectedNodesRight = new List<Node>();
        foreach (GameObject shape in shapes)
        {
            for (int i = 0; i < shape.GetComponent<PolygonCollider2D>().GetTotalPointCount(); i++)
            {
                Vector2 pos = new Vector2(shape.transform.position.x, shape.transform.position.y) - shape.GetComponent<PolygonCollider2D>().points[i];
                Node node = grid.GetNodeFromWorldPos(pos);
                grid.GetNodeFromWorldPos(pos).onTop = shape.name;
                if (node.onTop.Contains("L"))
                {
                    _selectedNodesLeft.Add(node);
                }
                else
                {
                    _selectedNodesRight.Add(node);
                }
            }

        }
        
    }

    private bool SameShape()
    {
        int difx = _selectedNodesLeft[0].gridX - _selectedNodesRight[0].gridX;
        int dify = _selectedNodesLeft[0].gridY - _selectedNodesRight[0].gridY;

        for(int i = 0; i < _selectedNodesRight.Count; i++)
        {
            Node node = grid.grid[_selectedNodesRight[i].gridX + difx, _selectedNodesRight[i].gridY + dify];
            if(node != _selectedNodesLeft[i])
            {
                return false;
            }
        }
        return true;
    }

    private void ShapePos()
    {
        int counter = 1;
        foreach(GameObject shape in shapes)
        {
            if (shape.name.Contains("1"))
            {

                shape.transform.position = grid.grid[0 + (counter-1* counter-1), 10].worldPosition;

            }
            else
            {
                shape.transform.position = grid.grid[0, 10].worldPosition;

            }
            counter++;
        }
    }
}
