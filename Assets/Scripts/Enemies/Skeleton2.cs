using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton2 : MonoBehaviour
{
    public int[] xPos;
    public int[] yPos;
    public List<int[]> pos;
    public GridField grid;
    public GameObject player;
    public Pathfinder pathFinder;
    private List<Node> path2Player;
    public float hitValue;
    private float healthValue;
    private bool blockPlayer = false;
    private Vector2 diff;
    private bool hitPlayer = false;
    private int posCounter;

    // Start is called before the first frame update
    void Start()
    {
        hitValue = 50;
        pos = new List<int[]>();
        for(int i = 0; i < xPos.Length; i++)
        {
            int[] coords = new int[] { xPos[i], yPos[i] };
            pos.Add(coords);
        }
        healthValue = 200;
        posCounter = 0;
        Node startNode = grid.grid[pos[posCounter][0],pos[posCounter][1]];
        transform.position = startNode.worldPosition - new Vector3(0,0,1);

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
                StartCoroutine(block());
            }
            if (player.GetComponent<Player>().enemyHit)
            {
                if (!blockPlayer)
                {
                    player.GetComponent<Player>().enemyHit = false;
                    healthValue -= player.GetComponent<Player>().hitValue;
                    if(healthValue <= 0)
                    {
                        Destroy(this.gameObject);
                    }

                }
            }
        }
        else if(path2Player.Count < 9 && path2Player.Count > 1)
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
            transform.Translate(diff * 2.5f * Time.deltaTime);
        }
        else
        {
            if(grid.GetNodeFromWorldPos(transform.position) != grid.grid[pos[posCounter][0], pos[posCounter][1]])
            {
                Node targetNode = grid.grid[pos[posCounter][0], pos[posCounter][1]];
                List<Node> back2Pos = pathFinder.FindPath(transform.position, targetNode.worldPosition);
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

                    transform.Translate(diff * 2.5f * Time.deltaTime);
                }
            }
            else
            {
                posCounter++;
                if(posCounter == pos.Count)
                {
                    posCounter = 0;
                }
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
    IEnumerator block()
    {
        yield return new WaitForSeconds(3);
        blockPlayer = true;
        yield return new WaitForSeconds(1);
        blockPlayer = false;

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
