using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public GridField grid;
    private Node targetNode;
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
    private bool ready = true;
    private bool foundPos = false;
    private bool coroutineStart;
    private float fullHealth;
    public bool levelLoaded;
    public Game data;
    public bool leaveLevel;
    private bool chooseExit;

    public HealthBar healthBar;
    private void Awake()
    {

        data.setLevel(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyHit = false;
        //hitValue = 25 + (10 * strengthMultiplier());
        hitValue = 500;
        targetNode = null;
        existingTarget = null;
        strength = 100;
        //strength = 100 + (25 + strengthMultiplier());
        fullHealth = strength;
        coroutineStart = false;
        if (!upperWorld)
        {
            healthBar.SetHealthBarValue(1);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
                levelLoaded = false;
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
            }
            else if (grid.GetNodeFromWorldPos(transform.position).onTop == "ExitOpen")
            {
                nextLevel();
            }
        }
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        if (entraceNodes.Count != 0)
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
            targetNode = null;
            StopAllCoroutines();
            coroutineStart = false;
            levels.transform.GetChild(levelNr).gameObject.SetActive(false);
            levels.transform.GetChild(levelNr - 1).gameObject.SetActive(true);
            chooseExit = true;
            foundPos = false;
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
            targetNode = null;
            StopAllCoroutines();
            coroutineStart = false;
            levels.transform.GetChild(levelNr).gameObject.SetActive(false);
            levels.transform.GetChild(levelNr + 1).gameObject.SetActive(true);
            foundPos = false;
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
            coroutineStart = false;
            StopAllCoroutines();
            leaveLevel = true;
            if(data.activeLevel != 1) 
            { 
                levels.transform.GetChild(data.activeLevel-1).gameObject.SetActive(false);
                levels.transform.GetChild(data.activeLevel-2).gameObject.SetActive(true);
            }
            healthBar.SetHealthBarValue(1);
            foundPos = false;
            strength = fullHealth;
        }
    }

    public void attack()
    {

        if (enemyNearby() && ready)
        {
            targetNode = null;
            coroutineStart = false;
            StartCoroutine(wait2Hit());
        }
    }
    public void block()
    {
        if (ready && enemyNearby())
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
        ready = false;
        enemyHit = true;
        yield return new WaitForSeconds(1);
        ready = true;
    }

    IEnumerator wait2Block()
    {
        ready = false;
        blockEnemy = true;
        yield return new WaitForSeconds(0.5f);
        blockEnemy = false;
        yield return new WaitForSeconds(2);
        ready = true;
    }
    private int strengthMultiplier()
    {
        int counter = 0;
        foreach(string name in data.nodeTags)
        {
            if (grid.ghostNames.Contains(name))
            {
                counter++;
            }
        }
        int strength = grid.ghostNames.Count - counter;
        return strength;
    }
}
