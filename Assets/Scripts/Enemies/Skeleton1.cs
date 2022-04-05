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
    private bool startPos;
    public float hitValue = 25;
    private float healthValue;
    private int axis;
    private Vector2 diff;
    private bool hitPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        healthValue = 100;
        startNode = grid.grid[start[0], start[1]];
        endNode = grid.grid[end[0], end[1]];
        transform.position = startNode.worldPosition;
        startPos = true;
        if(start[1] -end[1] == 0)
        {
            axis = start[1];
        }
        else
        {
            axis = start[0];
        }

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
        else if(path2Player.Count < 5 && path2Player.Count > 1)
        {
            Node currentNode = grid.GetNodeFromWorldPos(transform.position);
            if (currentNode.gridX - path2Player[1].gridX == 0)
            {
                diff = new Vector2(0, path2Player[1].worldPosition.y - grid.GetNodeFromWorldPos(transform.position).worldPosition.y);
            }
            else
            {
                diff = new Vector2(path2Player[1].worldPosition.x - grid.GetNodeFromWorldPos(transform.position).worldPosition.x, 0);
            }

            setPositionGhost(path2Player[1]);
            transform.Translate(diff * 2f * Time.deltaTime);
        }
        else
        {
            if(grid.GetNodeFromWorldPos(transform.position).gridY != axis)
            {
                List<Node> back2Pos = pathFinder.FindPath(transform.position, startNode.worldPosition);
                if(back2Pos.Count > 1)
                {
                    Node currentNode = grid.GetNodeFromWorldPos(transform.position);
                    if (currentNode.gridX - back2Pos[1].gridX == 0)
                    {
                        diff = new Vector2(0, back2Pos[1].worldPosition.y - grid.GetNodeFromWorldPos(transform.position).worldPosition.y);
                    }
                    else
                    {
                        diff = new Vector2(back2Pos[1].worldPosition.x - grid.GetNodeFromWorldPos(transform.position).worldPosition.x, 0);
                    }
                    setPositionGhost(back2Pos[1]);

                    transform.Translate(diff * 2f * Time.deltaTime);
                }
                if(grid.GetNodeFromWorldPos(transform.position).gridY == axis)
                {
                    startPos = true;
                }
            }
            else if(startPos)
            {

                transform.position = Vector2.MoveTowards(transform.position, endNode.worldPosition, 1f * Time.deltaTime);
                if(grid.GetNodeFromWorldPos(transform.position) == endNode)
                {
                    startPos = false;
                }
                setPositionGhost(grid.GetNodeFromWorldPos(transform.position));
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, startNode.worldPosition, 1f * Time.deltaTime);
                if (grid.GetNodeFromWorldPos(transform.position) == startNode)
                {
                    startPos = true;
                }

                setPositionGhost(grid.GetNodeFromWorldPos(transform.position));
            }
        }
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
