using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton1 : MonoBehaviour
{
    public int[] start = new int[2];
    public int[] end = new int[2];
    public GridField grid;
    public GameObject player;
    private Node startNode;
    private Node endNode;
    public Pathfinder pathFinder;
    private List<Node> path2Player;
    public float hitValue = 25;
    private float healthValue;
    private bool hitPlayer = false;
    private Node existingTarget;
    private Node targetNode;
    private bool coroutineStart;

    // Start is called before the first frame update
    void Start()
    {
        healthValue = 100;
        startNode = grid.grid[start[0], start[1]];
        endNode = grid.grid[end[0], end[1]];
        transform.position = startNode.worldPosition - new Vector3(0,0,1);
        existingTarget = null;
        coroutineStart = false;

    }

    // Update is called once per frame
    void Update()
    {
        path2Player = pathFinder.FindPath(transform.position, player.transform.position);
        if (next2Player())
        {
            if (!hitPlayer && !player.GetComponent<Player>().blockEnemy)
            {
                hitPlayer = true;
                StartCoroutine(hit());
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
        else if(path2Player.Count < 6 && path2Player.Count > 1)
        {
            targetNode = grid.GetNodeFromWorldPos(player.transform.position);
            if (targetNode == existingTarget)
            {
                if (!coroutineStart)
                {
                    coroutineStart = true;
                    StartCoroutine(move(path2Player));
                }

            }
            else if (!coroutineStart)
            {
                existingTarget = targetNode;
            }
        }
        else
        {
            if (grid.GetNodeFromWorldPos(transform.position) == startNode)
            {
                targetNode = endNode;
                
            }
            else if(grid.GetNodeFromWorldPos(transform.position) == endNode)
            {
                targetNode = startNode;
            }
            List<Node> back2Pos = pathFinder.FindPath(transform.position, targetNode.worldPosition);
            if (back2Pos.Count > 1)
            {
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
    }

    private IEnumerator move(List<Node> path)
    {
        //source: https://forum.unity.com/threads/transform-position-speed.744293/
        if (existingTarget == null)
        {
            existingTarget = targetNode;
        }
        Vector3 pos = transform.position;
        Node currentNode = grid.GetNodeFromWorldPos(transform.position);
        float goal;
        if (pos.x == path[1].worldPosition.x)
        {
            goal = path[1].worldPosition.y;
            while (pos.y != goal)
            {
                pos.y = Mathf.MoveTowards(pos.y, goal, 1 * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }
        }
        else if (pos.y == path[1].worldPosition.y)
        {
            goal = path[1].worldPosition.x;

            while (pos.x != goal)
            {
                pos.x = Mathf.MoveTowards(pos.x, goal, 1 * Time.deltaTime);
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
            if(neightbour == grid.GetNodeFromWorldPos(player.transform.position))
            {
                return true;
            }
        }
        return false;
    }
    IEnumerator hit()
    {
        player.GetComponent<Player>().reduceStrength(hitValue);
        yield return new WaitForSeconds(2);

        hitPlayer = false;
    }
    private void setPositionGhost(Node currentNode)
    {
        foreach(Node node in grid.grid)
        {
            if(node.onTop == "Enemy")
            {
                node.setItemOnTop(null);
            }
        }
        currentNode.setItemOnTop("Enemy");
    }
}
