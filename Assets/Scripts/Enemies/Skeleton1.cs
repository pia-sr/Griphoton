using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton1 : MonoBehaviour
{
    //public variables
    public int[] start = new int[2];
    public int[] end = new int[2];
    public GridField grid;
    public GameObject player;
    public Pathfinder pathFinder;
    public Animator animator;
    public float hitValue = 25;
    public GameObject hintKey;

    //private variables
    private List<Node> path2Player;
    private float healthValue;
    private bool hitPlayer = false;
    private Node existingTarget;
    private Node targetNode;
    private bool coroutineStart;
    private HealthBar healthBar;
    private int xInput;
    private int yInput;
    private Node aim;
    private Node startNode;
    private Node endNode;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }


    //Function to bring the skeleton to their start position
    private void SetUp()
    {
        StopAllCoroutines();
        hitPlayer = false;
        healthValue = 100;
        startNode = grid.grid[start[0], start[1]];
        endNode = grid.grid[end[0], end[1]];
        transform.position = startNode.worldPosition - new Vector3(0, 0, 1);
        existingTarget = null;
        coroutineStart = false;
        healthBar = transform.GetChild(0).GetComponentInChildren<HealthBar>();
        healthBar.SetHealthBarValue(1);
        aim = endNode;
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
        if (targetNode == null)
        {

            animator.SetBool("isWalking", false);
        }
        if (player.GetComponent<Player>().leaveLevel)
        {
            SetUp();
        }
        SetPositionGhost(grid.GetNodeFromWorldPos(transform.position));
        path2Player = pathFinder.FindPathEnemies(transform.position, player.transform.position);
        if (Next2Player())
        {
            if (!hitPlayer && !player.GetComponent<Player>().blockEnemy)
            {
                hitPlayer = true;
                StartCoroutine(Attack());
            }
            if (player.GetComponent<Player>().enemyHit)
            {

                player.GetComponent<Player>().enemyHit = false;
                healthValue -= player.GetComponent<Player>().hitValue;
                float playerHitvalue = player.GetComponent<Player>().hitValue;
                float healthReduc = playerHitvalue/ 100;
                healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - healthReduc);
                if (healthValue <= 0)
                {
                    Hint();
                    this.gameObject.SetActive(false);
                }
            }
        }
        //if the player is 5 nodes away, they will follow the player
        else if (path2Player.Count < 6 && path2Player.Count > 1)
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
        //If not they will walk between two given points
        else
        {
            animator.SetBool("isWalking", true);
            if (grid.GetNodeFromWorldPos(transform.position) == startNode)
            {
                aim = endNode;
                
            }
            else if(grid.GetNodeFromWorldPos(transform.position) == endNode)
            {
                aim = startNode;
                
            }
            List<Node> back2Pos = pathFinder.FindPath(transform.position, aim.worldPosition);
            if (back2Pos.Count > 1)
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


    private void Hint()
    {
        Instantiate(hintKey, transform.position, Quaternion.identity, transform.parent.parent);
        /*
        int rand = Random.Range(0, 3);
        if (rand == 1)
        {
            Instantiate(hintKey, transform.position, Quaternion.identity, transform.parent.parent);
        }*/
    }
}
