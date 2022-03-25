using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridField))]
public class ReplacementPuzzle2 : MonoBehaviour
{
    public GridField grid;
    public GameObject symbol;
    public Text rowText;
    public GameObject rowCanvas;
    public GameObject symbolManager;
    public GameObject exchangeButton;
    public GameObject selectedTiles;

    private int rowNumber;
    private List<GameObject> selected;
    private List<int> row;
    private List<int> finalRow;
    private float size;
    private List<List<int>> inputSymbols;
    private List<List<int>> outputSymbols;
    private Color buttonColour;


    void Awake()
    {
        grid = GetComponent<GridField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        size = grid.nodeRadius *1.25f;
        selected = new List<GameObject>();
        for(int i = 0; i < grid.getGridSizeY(); i++)
        {
            rowText.GetComponent<RectTransform>().sizeDelta = new Vector3(130, 130, 0);
            rowText.fontSize = 100;
            Text text = Instantiate(rowText, grid.grid[0,i].worldPosition, Quaternion.identity, rowCanvas.transform);
            text.text = (7 - i).ToString();
        }
        rowNumber = 6;
        row = new List<int>() { 1,0 };
        symbol.transform.localScale = new Vector3(size, size, 0);
        GameObject symbol1 = Instantiate(symbol, grid.grid[1, 6].worldPosition, Quaternion.identity, symbolManager.transform);
        symbol1.GetComponent<SpriteRenderer>().color = Color.red;
        GameObject symbol2 = Instantiate(symbol, grid.grid[2, 6].worldPosition, Quaternion.identity, symbolManager.transform);
        symbol2.GetComponent<SpriteRenderer>().color = Color.black;
        inputSymbols = new List<List<int>>()
        {
            new List<int>(){1},
            new List<int>(){0},
            new List<int>() {0,1,1}
        };
        outputSymbols = new List<List<int>>()
        {
            new List<int>(){0,1},
            new List<int>(){0,1},
            new List<int>(){0,0}
        };
        finalRow = new List<int>() { 1, 0, 1, 1, 1, 0 };

        buttonColour = exchangeButton.GetComponent<SpriteRenderer>().color;
    }

    private Rect tile2Rect(GameObject tile)
    {

        Rect rect = new Rect(tile.transform.position.x - (grid.nodeRadius/2), tile.transform.position.y - (grid.nodeRadius/2), grid.nodeRadius, grid.nodeRadius);
        return rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !checkWin())
        {

            exchangeButton.GetComponent<SpriteRenderer>().color = buttonColour;
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rect button = tile2Rect(exchangeButton);
            if (button.Contains(touchPosition))
            {
                sortList();
                if (selected.Count != 0 && checkNeighbours())
                {
                    List<int> newRow = new List<int>();
                    List<int> rowSymbols = new List<int>();
                    for (int i = 0; i < selected.Count; i++)
                    {
                
                        if (selected[i].GetComponent<SpriteRenderer>().color == Color.black)
                        {
                            rowSymbols.Add(0);

                        }
                        else
                        {
                            rowSymbols.Add(1);
                        }
                        Destroy(selectedTiles.transform.GetChild(i).gameObject);
                    }
                    if (replace(rowSymbols).Count != 0)
                    {
                        rowNumber--;
                        for (int i = 0; i < row.Count; i++)
                        {
                            if (row[i] == -1)
                            {
                                for (int j = 0; j < replace(rowSymbols).Count; j++)
                                {
                                    newRow.Add(replace(rowSymbols)[j]);
                                }
                                rowSymbols.Clear();
                            }
                            else
                            {
                                newRow.Add(row[i]);
                            }
                        }
                        row = newRow;
                        for (int i = 0; i < row.Count; i++)
                        {

                            symbol.transform.localScale = new Vector3(size, size, 0);
                            GameObject item = Instantiate(symbol, grid.grid[1 + i, rowNumber].worldPosition, Quaternion.identity, symbolManager.transform);
                            if (row[i] == 0)
                            {
                                item.GetComponent<SpriteRenderer>().color = Color.black;
                            }
                            else
                            {
                                item.GetComponent<SpriteRenderer>().color = Color.red;
                            }
                        }
                        selected.Clear();
                    }
                    else
                    {
                        exchangeButton.GetComponent<SpriteRenderer>().color = Color.magenta;
                    }

                }
                else
                {
                    exchangeButton.GetComponent<SpriteRenderer>().color = Color.magenta;
                }
                
            }

            for (int i = 1; i < grid.getGridSizeX(); i++)
            {
                GameObject chosenSymbol = null;
                int index = -1;
                for(int j = 0; j< symbolManager.transform.childCount; j++)
                {
                    if (symbolManager.transform.GetChild(j).transform.position == grid.grid[i, rowNumber].worldPosition)
                    {
                        chosenSymbol = symbolManager.transform.GetChild(j).gameObject;
                        index = i-1;
                    }
                }
                if(chosenSymbol != null)
                {
                    Rect rect = tile2Rect(chosenSymbol);
                    if (rect.Contains(touchPosition))
                    {
                        if (!selected.Contains(chosenSymbol))
                        {
                            selected.Add(chosenSymbol);
                            row[index] = -1;
                            symbol.transform.localScale = new Vector3(grid.nodeRadius*1.5f, grid.nodeRadius * 1.5f, 0);
                            Color tinted = new Color(0.5566038f, 0.4865907f, 0.4965926f, 1);
                            symbol.GetComponent<SpriteRenderer>().color = tinted;
                            Instantiate(symbol, chosenSymbol.transform.position + new Vector3(0,0,0.5f), Quaternion.identity, selectedTiles.transform);

                        }
                        else
                        {
                            selected.Remove(chosenSymbol);
                            for(int j = 0; j < selectedTiles.transform.childCount; j++)
                            {

                                if(selectedTiles.transform.GetChild(j).transform.position - new Vector3(0,0,0.5f) == chosenSymbol.transform.position)
                                {

                                    Destroy(selectedTiles.transform.GetChild(j).gameObject);
                                }
                            }
                            if(chosenSymbol.GetComponent<SpriteRenderer>().color == Color.black)
                            {
                                row[index] = 0;
                            }
                            else
                            {
                                row[index] = 1;
                            }

                        }
                    }
                }

            }
            if (checkWin())
            {
                Debug.Log("Won!");
            }

        }
    }
    private void sortList()
    {
        if(selected.Count > 0)
        {
            for (int i = 1; i < selected.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (selected[j].transform.position.x > selected[j + 1].transform.position.x)
                    {
                        GameObject item = selected[j];
                        selected[j] = selected[j + 1];
                        selected[j + 1] = item;
                    }
                }
            }
        }
    }
    private bool checkNeighbours()
    {
        if(selected.Count > 1)
        {
            float distance = grid.grid[1, 0].worldPosition.x - grid.grid[0, 0].worldPosition.x;
            for(int i = 0; i < selected.Count -1; i++)
            {
                foreach(Node node in grid.grid)
                {
                    if(node.worldPosition == selected[i].transform.position)
                    {
                        if(grid.grid[node.gridX+1,node.gridY].worldPosition != selected[i + 1].transform.position)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private bool checkWin()
    {
        if(rowNumber > 0)
        {
            return false;
        }
        else
        {
            if(row.Count != finalRow.Count)
            {
                return false;
            }
            else
            {
                for(int i = 0; i < row.Count; i++)
                {
                    if(row[i] != finalRow[i])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private List<int> replace(List<int> input)
    {
        List<int> output = new List<int>();
        int counter = 0;
        for(int i = 0; i< inputSymbols.Count; i++)
        {
            if(input.Count == inputSymbols[i].Count)
            {
                for(int j = 0; j < inputSymbols[i].Count; j++)
                {
                    if(input[j] == inputSymbols[i][j])
                    {
                        counter++;
                    }
                }
                if(counter == input.Count)
                {
                    output = outputSymbols[i];
                }
            }
        }
        return output;
    }

}
