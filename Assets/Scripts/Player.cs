using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GridField grid;
    private Node targetNode;
    public GameObject options;
    public GameObject puzzles;
    public GameObject griphoton;
    public Pathfinder pathFinder;
    private Node dungeon;
    private List<Node> path;
    private Node existingTarget;
    public float speed;
    private float strength;
    public bool lost = false;
    public bool upperWorld;
    public int hitValue;
    public bool enemyHit;
    public bool blockEnemy = false;
    private bool blockBool;
    private bool foundPos = false;
    private bool coroutineStart;
    private float fullHealth;
    private Game data;
    public bool leaveLevel;
    private bool chooseExit;
    private bool attackBool;

    private void setAllBoolsFalse()
    {
        blockEnemy = false;
        blockBool = false;
        coroutineStart = false;
        attackBool = false;
    }

    public HealthBar healthBar;
    private void Awake()
    {
        data = GameObject.Find("GameData").GetComponent<Game>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyHit = false;
        hitValue = 25 + (5 * data.strenghtMultiplier);
        targetNode = null;
        existingTarget = null;
        strength = 100 + (25 + data.strenghtMultiplier);
        fullHealth = strength;
        foundPos = false;
        if (!upperWorld)
        {
            healthBar.SetHealthBarValue(1);
            chooseExit = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            data.SaveGame();
            Application.Quit();
        }
        if(this.gameObject.activeSelf && upperWorld)
        {
            if (!options.activeSelf)
            {
                options.SetActive(true);

            }
        }
        restart();
        if (!foundPos)
        {
            if (dungeon == null && grid.dungeonNode() != null && upperWorld)
            {
                dungeon = grid.dungeonNode();
                Node startPos = grid.grid[dungeon.gridX, dungeon.gridY - 2];
                transform.position = startPos.worldPosition;
                foundPos = true;
            }
            else if (!upperWorld)
            {
                if (!chooseExit)
                {

                    entranceLevel();
                }
                else
                {
                    chooseExit = false;
                    exitLevel();
                }
            }
        }
        else if (targetNode != null)
        {
            path = pathFinder.FindPathPlayer(transform.position, targetNode.worldPosition);
            if(path.Count > 1)
            {
                if(targetNode == existingTarget)
                {
                    if (!coroutineStart)
                    {
                        coroutineStart = true;
                        StartCoroutine(move());
                    }


                }
                else if (!coroutineStart)
                {
                    existingTarget = targetNode;
                }

            }
            else if (grid.GetNodeFromWorldPos(transform.position).onTop == "Entrance" && targetNode.onTop == "Entrance")
            {
                previousLevel();
                data.SaveGame();
            }
            else if (grid.GetNodeFromWorldPos(transform.position).onTop == "ExitOpen")
            {
                nextLevel();
                data.SaveGame();
            }
            else if (grid.ghostNames().Contains(targetNode.onTop) || grid.ghostNames().Contains(targetNode.owner))
            {
                options.SetActive(false);
                data.SaveGame();
                string ghostName = grid.grid[path[0].gridX, path[0].gridY + 2].onTop;
                GameObject ghostHouse = findWithTag(ghostName);
                griphoton.SetActive(false);
                ghostHouse.SetActive(true);
                targetNode = null;
                setAllBoolsFalse();
                this.gameObject.SetActive(false);
            }else if(targetNode.onTop == "Dungeon" || targetNode.owner == "Dungeon")
            {
                data.SaveGame();
                SceneManager.LoadScene("Dungeon");
            }
        }
        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (grid.bounds().Contains(touchPosition))
            {
                targetNode = grid.GetNodeFromWorldPos(touchPosition);
            }

            
        }

    }
    private void entranceLevel()
    {
        List<Node> entraceNodes = new List<Node>();
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Entrance")
            {
                entraceNodes.Add(node);
            }
        }
        if (entraceNodes.Count == 3)
        {
            transform.position = entraceNodes[1].worldPosition - new Vector3(0, 0, 1);
            foundPos = true;
        }
    }
    private void exitLevel()
    {
        List<Node> entraceNodes = new List<Node>();
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "ExitOpen")
            {
                entraceNodes.Add(node);
            }
        }
        if (entraceNodes.Count != 0)
        {
            transform.position = entraceNodes[1].worldPosition - new Vector3(0, 0, 1);
            foundPos = true;
        }
    }

    private void previousLevel()
    {
        int levelNr = -1;
        GameObject levels = GameObject.FindWithTag("Levels");
        for (int i = 1; i < levels.transform.childCount; i++)
        {
            if (levels.transform.GetChild(i).gameObject.activeSelf)
            {
                levelNr = i;
            }
        }
        if (levelNr != -1)
        {
            leaveLevel = true;
            setAllBoolsFalse();
            pause();
            levels.transform.GetChild(levelNr).gameObject.SetActive(false);
            levels.transform.GetChild(levelNr - 1).gameObject.SetActive(true);
            chooseExit = true;
            foundPos = false;
            StartCoroutine(wait());
            unpause();
        }
    }

    private void nextLevel()
    {
        int levelNr = -1;
        GameObject levels = GameObject.FindWithTag("Levels");
        for(int i = 0; i < levels.transform.childCount-1 ; i++)
        {
            if (levels.transform.GetChild(i).gameObject.activeSelf)
            {
                levelNr = i;
            }
        }
        if(levelNr != -1)
        {
            leaveLevel = true;
            setAllBoolsFalse();
            pause();
            levels.transform.GetChild(levelNr).gameObject.SetActive(false);
            levels.transform.GetChild(levelNr + 1).gameObject.SetActive(true);
            foundPos = false;
            StartCoroutine(wait());
            unpause();
        }
    }

    private IEnumerator move()
    {
        //source: https://forum.unity.com/threads/transform-position-speed.744293/
        if(existingTarget == null)
        {
            existingTarget = targetNode;
        }
        Vector3 pos = transform.position;
        float goal;
        if (pos.x == path[1].worldPosition.x)
        {
            goal = path[1].worldPosition.y;
            while (pos.y != goal)
            {
                pos.y = Mathf.MoveTowards(pos.y, goal, 1.5f * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }
        }
        else if(pos.y == path[1].worldPosition.y)
        {
            goal = path[1].worldPosition.x;

            while (pos.x != goal)
            {
                pos.x = Mathf.MoveTowards(pos.x, goal, 1.5f * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }


        }
        coroutineStart = false;

    }

    public void reduceStrength(float hit)
    {
        float healthReduc = hit / fullHealth;
        
        strength -= hit;
        healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - healthReduc);
        if(strength <= 0)
        {
            lost = true;
        }

    }
    private void restart()
    {
        if (lost)
        {
            lost = false;
            GameObject levels = GameObject.FindWithTag("Levels");
            targetNode = null;
            StopAllCoroutines();
            leaveLevel = true;
            setAllBoolsFalse();

            if(data.activeLevel != 1) 
            { 
                levels.transform.GetChild(data.activeLevel-1).gameObject.SetActive(false);
                levels.transform.GetChild(data.activeLevel-2).gameObject.SetActive(true);
            }
            healthBar.SetHealthBarValue(1);
            foundPos = false;
            strength = fullHealth;
            StartCoroutine(wait());
        }
    }

    public void attack()
    {

        if (enemyNearby() && !attackBool)
        {
            targetNode = null;
            coroutineStart = false;
            StartCoroutine(wait2Hit());
        }
    }
    public void block()
    {
        if (!blockBool && enemyNearby())
        {
            targetNode = null;
            coroutineStart = false;
            StartCoroutine(wait2Block());
        }
    }

    private bool enemyNearby()
    {
        foreach (Node neighbour in grid.GetNodeNeighbours(grid.GetNodeFromWorldPos(transform.position)))
        {
            if (grid.getEnemiesPos().Contains(neighbour))
            {
                return true;
            }
        }

        return false;
    }
    IEnumerator wait2Hit()
    {
        attackBool = true;
        enemyHit = true;
        yield return new WaitForSeconds(1);
        attackBool = false;
    }

    IEnumerator wait2Block()
    {
        blockBool = true;
        blockEnemy = true;
        yield return new WaitForSeconds(0.5f);
        blockEnemy = false;
        yield return new WaitForSeconds(2);
        blockBool = false;
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.1f);
        leaveLevel = false;
    }

    private GameObject findWithTag(string tag)
    {
        GameObject foundPuzzles = null;
        for(int i = 0; i < puzzles.transform.childCount; i++)
        {
            if(puzzles.transform.GetChild(i).CompareTag(tag))
            {
                foundPuzzles = puzzles.transform.GetChild(i).gameObject;
            }
        }
        return foundPuzzles;
    }
    public void pause()
    {
        StopAllCoroutines();
        targetNode = null;
        coroutineStart = true;
    }
    public void unpause()
    {
        StartCoroutine(wait2Move());
    }

    IEnumerator wait2Move()
    {
        yield return new WaitForSeconds(0.5f);
        coroutineStart = false;

    }
}
