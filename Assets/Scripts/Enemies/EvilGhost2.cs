using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilGhost2 : MonoBehaviour
{
    public int[] startPos = new int[2];
    public GridField grid;
    public GameObject player;
    public Pathfinder pathFinder;
    private List<Node> path2Player;
    public float hitValue;
    private float healthValue;
    private float hitSpeed;
    private Node targetNode;
    private Vector2 diff;
    private bool moveAway = false;
    private bool stay = false;
    private bool attackPlayer = false;
    private List<Node> visitedNodes = new List<Node>();
    private Node existingTarget;
    private bool coroutineStart;
    private Node playerPos;

    // Start is called before the first frame update
    void Start()
    {
        
        healthValue = 500;
        Node startNode = grid.grid[startPos[0], startPos[1]];
        transform.position = startNode.worldPosition - new Vector3(0, 0, 1);
        targetNode = startNode;
    }

    // Update is called once per frame
    void Update()
    {
        if (grid.GetNodeFromWorldPos(this.gameObject.transform.position) == playerPos)
        {
            moveAway = false;
        }

        path2Player = pathFinder.FindPath(transform.position, player.transform.position);
        if (next2Player() && !moveAway)
        {
            attackPlayer = false;
            if (!stay)
            {
                stay = true;
                hitValue = 100 + (10 * hitSpeed);

                Debug.Log("Hitvalue: "+ hitValue + ", hitSpeed: " + hitSpeed);
                if (player.GetComponent<Player>().blockEnemy)
                {
                    player.GetComponent<Player>().reduceStrength(hitValue / 2);
                }
                else
                {
                    player.GetComponent<Player>().reduceStrength(hitValue);
                }
                StartCoroutine(fight());
            }
            if (player.GetComponent<Player>().enemyHit)
            {
                player.GetComponent<Player>().enemyHit = false;
                healthValue -= player.GetComponent<Player>().hitValue;
                if (healthValue <= 0)
                {
                    Destroy(this.gameObject);
                }
            }

        }
        else if (path2Player.Count > 1 && !moveAway)
        {
            attackPlayer = true;
            targetNode = grid.GetNodeFromWorldPos(player.transform.position);

        }
        List<Node> back2Pos = pathFinder.FindPath(transform.position, targetNode.worldPosition);
        if (back2Pos.Count > 1 && !stay)
        {

            hitSpeed = visitedNodes.Count;
            if(visitedNodes.Count == 0 || visitedNodes[visitedNodes.Count-1] != grid.GetNodeFromWorldPos(transform.position))
            {
                visitedNodes.Add(grid.GetNodeFromWorldPos(transform.position));
            }
            if (targetNode == existingTarget)
            {
                if (!coroutineStart)
                {
                    coroutineStart = true;
                    StartCoroutine(move(back2Pos));
                }

            }
            else if (!coroutineStart)
            {
                existingTarget = targetNode;
            }
        }
    }

    private IEnumerator move(List<Node> path)
    {
        //source: https://forum.unity.com/threads/transform-position-speed.744293/
        if (existingTarget == null)
        {
            existingTarget = targetNode;
        }
        Vector3 pos = transform.position;
        float goal;
        float speed;
        if (attackPlayer)
        {

            speed = 1.4f + (0.1f * hitSpeed);
        }
        else
        {

            speed = 1.6f;
        }
        if (pos.x == path[1].worldPosition.x)
        {
            goal = path[1].worldPosition.y;
            while (pos.y != goal)
            {
                pos.y = Mathf.MoveTowards(pos.y, goal, speed * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }
        }
        else if (pos.y == path[1].worldPosition.y)
        {
            goal = path[1].worldPosition.x;

            while (pos.x != goal)
            {
                pos.x = Mathf.MoveTowards(pos.x, goal, speed * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }


        }
        coroutineStart = false;

    }

    private bool next2Player()
    {
        foreach (Node neightbour in grid.GetNodeNeighbours(grid.GetNodeFromWorldPos(transform.position)))
        {
            if (neightbour == grid.GetNodeFromWorldPos(player.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    private void setPositionGhost(Node currentNode)
    {
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Enemy")
            {
                node.setItemOnTop(null);
            }
        }
        currentNode.setItemOnTop("Enemy");
    }

    IEnumerator fight()
    {
        yield return new WaitForSeconds(2);
        moveAway = true;
        hitSpeed = 0;
        visitedNodes.Clear();
        targetNode = grid.grid[Random.Range(0, grid.getGridSizeX() - 1), Random.Range(0, grid.getGridSizeY() - 1)];
        while (!(targetNode.onTop == "Floor" || targetNode.onTop == "Spikes"))
        {
            targetNode = grid.grid[Random.Range(0, grid.getGridSizeX() - 1), Random.Range(0, grid.getGridSizeY() - 1)];
            
        }
        playerPos = targetNode;
        Debug.Log(playerPos == null);
        stay = false;
    }
}
