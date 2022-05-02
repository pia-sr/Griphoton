using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilGhost1 : MonoBehaviour
{
    //public variables
    public int[] startPos = new int[2];
    public GridField grid;
    public GameObject player;
    public Pathfinder pathFinder;
    public float hitValue;
    public Animator animator;

    //private variables
    private List<Node> path2Player;
    private float healthValue;
    private float hitSpeed;
    private Node targetNode;
    private bool moveAway = false;
    private bool stay = false;
    private bool attackPlayer = false;
    private List<Node> visitedNodes = new List<Node>();
    private Node existingTarget;
    private bool coroutineStart;
    private HealthBar healthBar;
    private int xInput;
    private int yInput;


    // Start is called before the first frame update
    void Start()
    {

        SetUp();
    }

    //Function to bring the ghost to their start position
    private void SetUp()
    {
        moveAway = false;
        stay = false;
        attackPlayer = false;
        healthValue = 300;
        Node startNode = grid.grid[startPos[0], startPos[1]];
        transform.position = startNode.worldPosition - new Vector3(0, 0, 1);
        targetNode = startNode;
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
        if (targetNode == null)
        {

            animator.SetBool("isWalking", false);
        }
        path2Player = pathFinder.FindPathEnemies(transform.position, player.transform.position);
        if (Next2Player() && !moveAway)
        {
            attackPlayer = false;
            if (!stay)
            {
                stay = true;
                hitValue = 50 + (10 * hitSpeed);

                if (player.GetComponent<Player>().blockEnemy)
                {
                    player.GetComponent<Player>().ReduceStrength(hitValue / 2);
                }
                else
                {
                    player.GetComponent<Player>().ReduceStrength(hitValue);
                }
                StartCoroutine(Attack());
            }
            if (player.GetComponent<Player>().enemyHit)
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
        //if the player is less than 9 nodes away, the ghost will chase them
        else if (path2Player.Count < 9 && path2Player.Count > 1 && !moveAway)
        {
            attackPlayer = true;
            targetNode = grid.GetNodeFromWorldPos(player.transform.position);

        }
        else
        {
            if (grid.GetNodeFromWorldPos(transform.position) == targetNode)
            {
                moveAway = false;
                hitSpeed = 0;
                visitedNodes.Clear();
                targetNode = grid.grid[Random.Range(0, grid.GetGridSizeX() - 1), Random.Range(0, grid.GetGridSizeY() - 1)];
                while (!(targetNode.onTop == "Floor" || targetNode.onTop == "Spikes"))
                {
                    targetNode = grid.grid[Random.Range(0, grid.GetGridSizeX() - 1), Random.Range(0, grid.GetGridSizeY() - 1)];
                }
            }

        }
        List<Node> back2Pos = pathFinder.FindPathEnemies(transform.position, targetNode.worldPosition);
        if (back2Pos.Count > 1 && !stay)
        {

            animator.SetBool("isWalking", true);
            hitSpeed = visitedNodes.Count;
            if (visitedNodes.Count == 0 || visitedNodes[visitedNodes.Count - 1] != grid.GetNodeFromWorldPos(transform.position) && attackPlayer)
            {
                visitedNodes.Add(grid.GetNodeFromWorldPos(transform.position));
            }
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
        else
        {

            animator.SetBool("isWalking", false);
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
        float speed;
        if (attackPlayer)
        {

            speed = 1 + (0.1f * hitSpeed);
        }
        else
        {

            speed = 1.5f;
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
        SetPositionGhost(path[1]);

    }

    //Checks if they are next to the player
    private bool Next2Player()
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

    //Sets the tag on the given node as enemy
    private void SetPositionGhost(Node currentNode)
    {
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Enemy")
            {
                node.SetItemOnTop(null);
            }
        }
        currentNode.SetItemOnTop("Enemy");
    }

    //Function to attack the player
    IEnumerator Attack()
    {

        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(2);
        moveAway = true;
        hitSpeed = 0;
        visitedNodes.Clear();
        targetNode = grid.grid[Random.Range(0, grid.GetGridSizeX() - 1), Random.Range(0, grid.GetGridSizeY() - 1)];
        while (!(targetNode.onTop == "Floor" || targetNode.onTop == "Spikes"))
        {
            targetNode = grid.grid[Random.Range(0, grid.GetGridSizeX() - 1), Random.Range(0, grid.GetGridSizeY() - 1)];
        }
        stay = false;
    }
}
