using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class ReplacementPuzzle3 : MonoBehaviour
{
    //public field objects
    public GridField grid;
    public GameObject symbol;
    public Text rowText;
    public GameObject rowCanvas;
    public GameObject symbolManager;
    public GameObject exchangeButton;
    public GameObject selectedTiles;
    public GameObject messageExit;
    public Text message;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public ReplacementTutorial tutorial;

    //private variables
    private int _rowNumber;
    private List<GameObject> _selected;
    private List<int> _row;
    private List<int> _finalRow;
    private float _size;
    private List<List<int>> _inputSymbols;
    private List<List<int>> _outputSymbols;
    private Color _buttonColour;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < grid.GetGridSizeY(); i++)
        {
            rowText.GetComponent<RectTransform>().sizeDelta = new Vector3(130, 130, 0);
            rowText.fontSize = 160;
            Text text = Instantiate(rowText, grid.grid[0, i].worldPosition, Quaternion.identity, rowCanvas.transform);
            text.text = (7 - i).ToString();
        }
        SetUp();
    }

    //Sets up the first row with symbols
    private void SetUp()
    {
        _size = grid.nodeRadius * 1.25f;
        _selected = new List<GameObject>();
        for (int i = 0; i < symbolManager.transform.childCount; i++)
        {
            Destroy(symbolManager.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < selectedTiles.transform.childCount; i++)
        {
            Destroy(selectedTiles.transform.GetChild(i).gameObject);
        }
        _rowNumber = 6;
        _row = new List<int>() { 1 };
        symbol.transform.localScale = new Vector3(_size, _size, 0);
        GameObject symbol1 = Instantiate(symbol, grid.grid[1, 6].worldPosition, Quaternion.identity, symbolManager.transform);
        symbol1.GetComponent<SpriteRenderer>().color = Color.red;
        _inputSymbols = new List<List<int>>()
        {
            new List<int>(){1},
            new List<int>(){0,1,0},
            new List<int>(){0,1,1}
        };
        _outputSymbols = new List<List<int>>()
        {
            new List<int>(){0,1,1},
            new List<int>(){0,0},
            new List<int>(){1}
        };
        _finalRow = new List<int>() { 0, 0, 1 };

        _buttonColour = exchangeButton.GetComponent<SpriteRenderer>().color;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !tutorial.inactive)
        {

            exchangeButton.GetComponent<SpriteRenderer>().color = _buttonColour;
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Rect button = Object2Rect(exchangeButton);

            //User can select multiple symbols and if they fit the rules they can be replaced
            //If not an error message will pop up
            if (button.Contains(touchPosition) && touch.phase == TouchPhase.Began)
            {
                SortList();
                if (_selected.Count != 0 && CheckNeighbours())
                {
                    List<int> newRow = new List<int>();
                    List<int> rowSymbols = new List<int>();
                    for (int i = 0; i < _selected.Count; i++)
                    {
                
                        if (_selected[i].GetComponent<SpriteRenderer>().color == Color.black)
                        {
                            rowSymbols.Add(0);

                        }
                        else
                        {
                            rowSymbols.Add(1);
                        }
                        Destroy(selectedTiles.transform.GetChild(i).gameObject);
                    }
                    _selected.Clear();
                    if (ReplacedList(rowSymbols).Count != 0)
                    {
                        _rowNumber--;
                        for (int i = 0; i < _row.Count; i++)
                        {
                            if (_row[i] == -1)
                            {
                                for (int j = 0; j < ReplacedList(rowSymbols).Count; j++)
                                {
                                    newRow.Add(ReplacedList(rowSymbols)[j]);
                                }
                                rowSymbols.Clear();
                            }
                            else
                            {
                                newRow.Add(_row[i]);
                            }
                        }
                        _row = newRow;
                        if(_row.Count < 8)
                        {

                            for (int i = 0; i < _row.Count; i++)
                            {

                                symbol.transform.localScale = new Vector3(_size, _size, 0);
                                GameObject item = Instantiate(symbol, grid.grid[1 + i, _rowNumber].worldPosition, Quaternion.identity, symbolManager.transform);
                                if (_row[i].ToString() == "0")
                                {
                                    item.GetComponent<SpriteRenderer>().color = Color.black;
                                }
                                else
                                {
                                    item.GetComponent<SpriteRenderer>().color = Color.red;
                                }
                            }
                            _selected.Clear();
                        }
                        else
                        {
                            tutorial.inactive = true;
                            message.transform.parent.transform.parent.gameObject.SetActive(true);
                            message.text = "You reached the limit of symbols for this row. \nPlease select a different pattern or reset the puzzle.";
                        }
                    }
                    else
                    {
                        tutorial.inactive = true;
                        message.transform.parent.transform.parent.gameObject.SetActive(true);
                        message.text = "Your selected pattern is not valid. \nPlease select a different pattern.";
                    }

                }
                else
                {
                    tutorial.inactive = true;
                    message.transform.parent.transform.parent.gameObject.SetActive(true);
                    message.text = "Your selected pattern is not valid. \nPlease select a different pattern.";
                }

            }

            for (int i = 1; i < grid.GetGridSizeX(); i++)
            {
                GameObject chosenSymbol = null;
                int index = -1;
                for(int j = 0; j< symbolManager.transform.childCount; j++)
                {
                    if (symbolManager.transform.GetChild(j).transform.position == grid.grid[i, _rowNumber].worldPosition)
                    {
                        chosenSymbol = symbolManager.transform.GetChild(j).gameObject;
                        index = i-1;
                    }
                }
                if(chosenSymbol != null)
                {
                    Rect rect = Object2Rect(chosenSymbol);
                    if (rect.Contains(touchPosition) && touch.phase == TouchPhase.Began)
                    {
                        if (!_selected.Contains(chosenSymbol))
                        {
                            _selected.Add(chosenSymbol);
                            _row[index] = -1;
                            symbol.transform.localScale = new Vector3(grid.nodeRadius*1.5f, grid.nodeRadius * 1.5f, 0);
                            Color tinted = new Color(0.5566038f, 0.4865907f, 0.4965926f, 1);
                            symbol.GetComponent<SpriteRenderer>().color = tinted;
                            Instantiate(symbol, chosenSymbol.transform.position + new Vector3(0,0,0.5f), Quaternion.identity, selectedTiles.transform);

                        }
                        else
                        {
                            _selected.Remove(chosenSymbol);
                            for(int j = 0; j < selectedTiles.transform.childCount; j++)
                            {

                                if(selectedTiles.transform.GetChild(j).transform.position - new Vector3(0,0,0.5f) == chosenSymbol.transform.position)
                                {

                                    Destroy(selectedTiles.transform.GetChild(j).gameObject);
                                }
                            }
                            if(chosenSymbol.GetComponent<SpriteRenderer>().color == Color.black)
                            {
                                _row[index] = 0;
                            }
                            else
                            {
                                _row[index] = 1;
                            }

                        }
                    }
                }

            }
            if (CheckWin() && !tutorial.inactive)
            {
                tutorial.inactive = true;
                tutorial.WonPuzzle();
            }
            else if (_rowNumber == 0)
            {
                tutorial.inactive = true;
                message.transform.parent.transform.parent.gameObject.SetActive(true);
                message.text = "You did not reach the goal! \nYou can try again by clicking on the restart button.";

            }

        }
    }

    //Function to create a rectagle on top of a given object to set rectangular boundaries for that object
    private Rect Object2Rect(GameObject tile)
    {
        Rect rect = new Rect(tile.transform.position.x - (grid.nodeRadius * 0.75f), tile.transform.position.y - (grid.nodeRadius * 0.75f), grid.nodeRadius * 1.5f, grid.nodeRadius * 1.5f);
        return rect;
    }

    //Function to sort the list with selected symbols based on their position
    private void SortList()
    {
        if(_selected.Count > 0)
        {
            for (int i = 1; i < _selected.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_selected[j].transform.position.x > _selected[j + 1].transform.position.x)
                    {
                        GameObject item = _selected[j];
                        _selected[j] = _selected[j + 1];
                        _selected[j + 1] = item;
                    }
                }
            }
        }
    }

    //Function to check if all the selected symbols are all next to each other
    private bool CheckNeighbours()
    {
        if(_selected.Count > 1)
        {
            float distance = grid.grid[1, 0].worldPosition.x - grid.grid[0, 0].worldPosition.x;
            for(int i = 0; i < _selected.Count -1; i++)
            {
                foreach(Node node in grid.grid)
                {
                    if(node.worldPosition == _selected[i].transform.position)
                    {
                        if(grid.grid[node.gridX+1,node.gridY].worldPosition != _selected[i + 1].transform.position)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }


    //Function to check if the player has solved the puzzle
    private bool CheckWin()
    {
        if(_rowNumber > 0)
        {
            return false;
        }
        else
        {
            if(_row.Count != _finalRow.Count)
            {
                return false;
            }
            else
            {
                for(int i = 0; i < _row.Count; i++)
                {
                    if(_row[i] != _finalRow[i])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    //Function to create of list with the new symbol sequence based on the given list
    private List<int> ReplacedList(List<int> input)
    {
        List<int> output = new List<int>();
        int index;
        for(int i = 0; i< _inputSymbols.Count; i++)
        {
            if(input.Count == _inputSymbols[i].Count)
            {
                int counter = 0;
                for (int j = 0; j < _inputSymbols[i].Count; j++)
                {
                    if(input[j] == _inputSymbols[i][j])
                    {
                        counter++;
                    }
                }
                if(counter == input.Count)
                {
                    output = _outputSymbols[i];
                }
            }
        }
        return output;
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
        tutorial.inactive = false;
        message.transform.parent.transform.parent.gameObject.SetActive(false);
    }

}
