/*
 * PolyAdd1.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PolyAdd1 : MonoBehaviour
{
    //public field objects
    public List<GameObject> shapes;
    public GameObject shapeEq1;
    public GameObject shapeEq2;
    public GameObject messageExit;
    public GridField grid;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public PolyAddTutorial tutorial;

    //private variables
    private GameObject _selectedShape;
    private bool _selected;
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
            _positions.Add(shape.transform.position);
        }
        SetUp();
    }

    //Function to set matchsticks to original position and rotation
    private void SetUp()
    {
        _selectedShape = null;
        _selected = false;
        int counter = 0;
        foreach(GameObject shape in shapes)
        {
            shape.transform.localPosition = _positions[counter];
            shape.transform.localRotation = Quaternion.Euler(Vector3.zero);
            shape.transform.GetChild(0).gameObject.SetActive(false);
            counter++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            if (touch.phase == TouchPhase.Began)
            {
                if (!_selected)
                {
                    foreach (GameObject shape in shapes)
                    {
                        var bounds = shape.GetComponent<SpriteRenderer>().bounds;

                        //selects the chosen shape
                        if (bounds.Contains(touchPosition))
                        {
                            _selected = true;
                            _selectedShape = shape;
                            shape.transform.GetChild(0).gameObject.SetActive(true);

                        }

                    }
                }
                else
                {
                    //unselects the chosen shape
                    if(EventSystem.current.currentSelectedGameObject == null)
                    {
                        _selected = false;
                        _selectedShape.transform.GetChild(0).gameObject.SetActive(false);
                        _selectedShape = null;
                    }
                

                }

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
        GetNodes();
        if (_selectedNodesLeft.Count != _squareCount || _selectedNodesRight.Count != _squareCount)
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
    
    //Function to get the nodes of the shapes 
    private void GetNodes()
    {
        _selectedNodesLeft = new List<Node>();
        _selectedNodesRight = new List<Node>();
        foreach (GameObject shape in shapes)
        {
            for (int i = 0; i < shape.GetComponent<EdgeCollider2D>().pointCount; i++)
            {
                
                Vector2 point = shape.GetComponent<EdgeCollider2D>().points[i];
                Vector2 pos = shape.transform.TransformPoint(point);
                Node node = grid.GetNodeFromWorldPos(pos);
                grid.GetNodeFromWorldPos(pos).onTop = shape.name;

                //the shapes are divided into to lists, left and right
                if (node.onTop.Contains("L"))
                {
                    if (!_selectedNodesLeft.Contains(node))
                    {
                        _selectedNodesLeft.Add(node);

                    }
                }
                else
                {
                    if (!_selectedNodesRight.Contains(node))
                    {
                        _selectedNodesRight.Add(node);
                    }
                    
                }
            }

        }
        
    }

    //Boolean to check if the two final shapes are identical
    private bool SameShape()
    {
        SortList(_selectedNodesLeft);
        SortList(_selectedNodesRight);
        int difx = _selectedNodesLeft[0].gridX - _selectedNodesRight[0].gridX;
        int dify = _selectedNodesLeft[0].gridY - _selectedNodesRight[0].gridY;

        for(int i = 0; i < _selectedNodesRight.Count; i++)
        {
            if(_selectedNodesRight[i].gridX + difx < 0 || _selectedNodesRight[i].gridX + difx > grid.GetGridSizeX()-1 || _selectedNodesRight[i].gridY + dify < 0 ||
                _selectedNodesRight[i].gridY + dify > grid.GetGridSizeY() - 1)
            {
                return false;
            }
            Node node = grid.grid[_selectedNodesRight[i].gridX + difx, _selectedNodesRight[i].gridY + dify];
            if(node != _selectedNodesLeft[i])
            {
                return false;
            }
        }
        return true;
    }

    //Sets the initial shape positions
    private void ShapePos()
    {
        int counter = 0;
        foreach(GameObject shape in shapes)
        {
            
            
            if (shape.name.Contains("L"))
            {

                shape.transform.position = grid.grid[(1+counter * 3), 10].worldPosition;

            }
            else
            {

                shape.transform.position = grid.grid[(1+counter * 3), 10].worldPosition;
            }
            
            counter++;
        }
    }

    //Function to move the selected shape down
    public void ButtonDown()
    {
        if (_selected)
        {
            Node node = grid.GetNodeFromWorldPos(_selectedShape.transform.position);
            if (node.gridY - 1 > 0)
            {
                Vector3 target = grid.grid[node.gridX, node.gridY - 1].worldPosition;
                _selectedShape.transform.position = target;

            }
        }
    }

    //Function to move the selected shape up
    public void ButtonUp()
    {
        if (_selected)
        {
            Node node = grid.GetNodeFromWorldPos(_selectedShape.transform.position);
            if (node.gridY + 1 < grid.GetGridSizeY() - 1)
            {
                Vector3 target = grid.grid[node.gridX, node.gridY + 1].worldPosition;
                _selectedShape.transform.position = Vector2.MoveTowards(_selectedShape.transform.position, target, 1f);

            }
        }
    }

    //Function to move the selected shape to the left
    //Shapes on the left must stay on the left side and shapes on the right must stay on the right side
    public void ButtonLeft()
    {
        if (_selected)
        {
            Node node = grid.GetNodeFromWorldPos(_selectedShape.transform.position);
            if (_selectedShape.name.Contains("L"))
            {
                if (node.gridX - 1 > 0)
                {
                    Vector3 target = grid.grid[node.gridX - 1, node.gridY].worldPosition;
                    _selectedShape.transform.position = Vector2.MoveTowards(_selectedShape.transform.position, target, 1f);

                }

            }
            else
            {
                if (node.gridX - 1 > 9)
                {
                    Vector3 target = grid.grid[node.gridX - 1, node.gridY].worldPosition;
                    _selectedShape.transform.position = Vector2.MoveTowards(_selectedShape.transform.position, target, 1f);

                }
            }
        }
    }

    //Function to move the selected shape to the right
    //Shapes on the left must stay on the left side and shapes on the right must stay on the right side
    public void ButtonRight()
    {
        if (_selected)
        {
            Node node = grid.GetNodeFromWorldPos(_selectedShape.transform.position);
            if (_selectedShape.name.Contains("L"))
            {
                if (node.gridX + 1 < 8)
                {
                    Vector3 target = grid.grid[node.gridX + 1, node.gridY].worldPosition;
                    _selectedShape.transform.position = Vector2.MoveTowards(_selectedShape.transform.position, target, 1f);

                }

            }
            else
            {
                if (node.gridX + 1 < grid.GetGridSizeX() - 1)
                {
                    Vector3 target = grid.grid[node.gridX + 1, node.gridY].worldPosition;
                    _selectedShape.transform.position = Vector2.MoveTowards(_selectedShape.transform.position, target, 1f);

                }
            }
        }
    }

    //Function to rotate the selected shape
    public void ButtonRotate()
    {
        if (_selected)
        {
            var rotation = _selectedShape.transform.localRotation.eulerAngles;
            float rotationZ = rotation.z;
            rotationZ += 90;

            rotation.z = rotationZ;
            _selectedShape.transform.localRotation = Quaternion.Euler(rotation);

        }
        
    }

    //Function to mirror the selected shape
    public void ButtonMirror()
    {
        if (_selected)
        {
            var rotation = _selectedShape.transform.localRotation.eulerAngles;
            float rotationY = rotation.y;
            rotationY += 180;
            rotation.y = rotationY;
            _selectedShape.transform.localRotation = Quaternion.Euler(rotation);
        }
        
    }

    //Function to sort out the list of nodes
    private void SortList(List<Node> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            for(int j = 0; j < list.Count-i-1; j++)
            {
                if(list[j].gridX > list[j+1].gridX)
                {
                    Node node = list[j+1];
                    list[j+1] = list[j];
                    list[j] = node;
                }
                else if (list[j + 1].gridX == list[j].gridX)
                {
                    if (list[j].gridY > list[j + 1].gridY)
                    {
                        Node node = list[j + 1];
                        list[j + 1] = list[j];
                        list[j] = node;

                    }
                }
            }
        }
    }
}
