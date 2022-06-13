using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton2 : MonoBehaviour
{
    //public variables
    public int[] xPos;
    public int[] yPos;
    public List<int[]> pos;
    public GridField grid;
    public GameObject player;
    public Pathfinder pathFinder;
    public float hitValue;
    public Animator animator;

    //private variables
    private float healthValue;
    private bool blockPlayer = false;
    private bool hitPlayer = false;
    private int posCounter;
    private Node existingTarget;
    private Node targetNode;
    private bool coroutineStart;
    private HealthBar healthBar;
    private int xInput;
    private int yInput;
    private List<Node> path2Player;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();

    }

    //Function to bring the skeleton to their start position
    private void SetUp()
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
        xInput = 0;
        yInput = -1;


        animator.SetFloat("XInput", xInput);
        animator.SetFloat("YInput", yInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (xInput == 0 && yInput == 0)
        {

            xInput = 0;
            yInput = -1;


            animator.SetFloat("XInput", xInput);
            animator.SetFloat("YInput", yInput);
        }
        if (player.GetComponent<Player>().leaveLevel)
        {
            SetUp();
        }
        path2Player = pathFinder.FindPathEnemies(transform.position, player.transform.position);
        if (Next2Player())
        {
            if (!hitPlayer && !player.GetComponent<Player>().blockEnemy)
            {
                hitPlayer = true;
                StartCoroutine(Attack());
                StartCoroutine(Block());
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
                        this.gameObject.SetActive(false);
                    }

                }
            }
        }
        //if the player is 8 nodes away, they will follow the player
        else if (path2Player.Count < 9 && path2Player.Count > 1)
        {
            animator.SetBool("isWalking", true);
            if (targetNode == existingTarget)
            {
                if (!coroutineStart)
                {
                    coroutineStart = true;
                    StartCoroutine(Move(path2Player));
                }

            }
            else if (!coroutineStart)
            {
                existingTarget = targetNode;
            }
        }
        //If not they will walk between a given list of points
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
                            StartCoroutine(Move(back2Pos));
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


    //Monster movement
    //source: https://forum.unity.com/threads/transform-position-speed.744293/
    private IEnumerator Move(List<Node> path)
    {
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
                pos.y = Mathf.MoveTowards(pos.y, goal, 2f * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }
        }
        else if (pos.y == path[1].worldPosition.y)
        {
            goal = path[1].worldPosition.x;

            while (pos.x != goal)
            {
                pos.x = Mathf.MoveTowards(pos.x, goal, 2f * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }


        }
        coroutineStart = false;
        SetPositionGhost(path[1]);

    }


    //Checks if they are next to the player
    private bool Next2Player()
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


    //Function to attack the player
    IEnumerator Attack()
    {

        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.1f);

        player.GetComponent<Player>().ReduceStrength(hitValue);
        yield return new WaitForSeconds(2);

        hitPlayer = false;
    }

    //Function to block the player's attack
    IEnumerator Block()
    {
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(3);
        blockPlayer = true;
        yield return new WaitForSeconds(1);
        blockPlayer = false;

    }

    //Sets the tag on the given node as enemy
    private void SetPositionGhost(Node currentNode)
    {
        foreach(Node node in grid.grid)
        {
            if(node.onTop == "Enemy")
            {
                node.SetItemOnTop(null);
            }
        }
        currentNode.SetItemOnTop("Enemy");
    }
}
