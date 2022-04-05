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
            float speed;
            if (attackPlayer)
            {

                speed = 2.5f + (0.15f * hitSpeed);
            }
            else
            {

                speed = 2.5f;
            }
            transform.Translate(diff * speed * Time.deltaTime);
            if(grid.GetNodeFromWorldPos(transform.position) == targetNode)
            {
                moveAway = false;
            }
        }
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
        stay = false;
    }
}
