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
    private Node existingTarget;
    private Node targetNode;
    private bool coroutineStart;
    private HealthBar healthBar;
    public Animator animator;
    private int xInput;
    private int yInput;

    // Start is called before the first frame update
    void Start()
    {
        begin();

    }
    private void begin()
    {
        if (targetNode == null)
        {

            animator.SetBool("isWalking", false);
        }
        blockPlayer = false;
        hitPlayer = false;
        hitValue = 100;
        pos = new List<int[]>();
        for (int i = 0; i < xPos.Length; i++)
        {
            int[] coords = new int[] { xPos[i], yPos[i] };
            pos.Add(coords);
        }
        healthValue = 200;
        posCounter = 0;
        Node startNode = grid.grid[pos[posCounter][0], pos[posCounter][1]];
        transform.position = startNode.worldPosition - new Vector3(0, 0, 1);
        coroutineStart = false;
        healthBar = this.transform.GetChild(0).GetChild(0).GetComponent<HealthBar>();
        healthBar.SetHealthBarValue(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Player>().leaveLevel)
        {
            begin();
        }
        path2Player = pathFinder.FindPathEnemies(transform.position, player.transform.position);
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
                    float playerHitvalue = player.GetComponent<Player>().hitValue;
                    float healthReduc = playerHitvalue / 100;
                    healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - healthReduc);
                    if (healthValue <= 0)
                    {
                        Destroy(this.gameObject);
                    }

                }
            }
        }
        else if(path2Player.Count < 9 && path2Player.Count > 1)
        {
            animator.SetBool("isWalking", true);
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
            animator.SetBool("isWalking", true);
            if (grid.GetNodeFromWorldPos(transform.position) != grid.grid[pos[posCounter][0], pos[posCounter][1]])
            {
                Node targetNode = grid.grid[pos[posCounter][0], pos[posCounter][1]];
                List<Node> back2Pos = pathFinder.FindPath(transform.position, targetNode.worldPosition);
                if(back2Pos.Count > 1)
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

    private IEnumerator move(List<Node> path)
    {
        //source: https://forum.unity.com/threads/transform-position-speed.744293/
        if (existingTarget == null)
        {
            existingTarget = targetNode;
        }
        Vector3 pos = transform.position;

        xInput = path[1].gridX - grid.GetNodeFromWorldPos(pos).gridX;
        yInput = path[1].gridY - grid.GetNodeFromWorldPos(pos).gridY;


        animator.SetFloat("XInput", xInput);
        animator.SetFloat("YInput", yInput);
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
        else if (pos.y == path[1].worldPosition.y)
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
        setPositionGhost(path[1]);

    }

    private bool next2Player()
    {
        foreach (Node neighbour in grid.GetNodeNeighbours(grid.GetNodeFromWorldPos(transform.position)))
        {
            if(neighbour == grid.GetNodeFromWorldPos(player.transform.position))
            {

                xInput = neighbour.gridX - grid.GetNodeFromWorldPos(transform.position).gridX;
                yInput = neighbour.gridY - grid.GetNodeFromWorldPos(transform.position).gridY;

                animator.SetFloat("XInput", xInput);
                animator.SetFloat("YInput", yInput);
                return true;
            }
        }
        return false;
    }
    IEnumerator hit()
    {

        animator.SetBool("isWalking", false);

        animator.SetTrigger("attack");
        player.GetComponent<Player>().reduceStrength(hitValue);
        yield return new WaitForSeconds(2);

        hitPlayer = false;
    }
    IEnumerator block()
    {
        animator.SetBool("isWalking", false);
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
