/*
 * EvilGhost2.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilGhost2 : MonoBehaviour
{
    //Public variables
    public int[] startPos = new int[2];
    public GridField grid;
    public GameObject player;
    public Pathfinder pathFinder;
    public float hitValue;
    public Animator animator;
    public GameObject hintKey;

    //private variables
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
    private HealthBar healthBar;
    private int xInput;
    private int yInput;
    private List<Node> path2Player;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
        
    }

    //Function to bring the ghost to their start position
    private void SetUp()
    {
        StopAllCoroutines();
        moveAway = false;
        stay = false;
        attackPlayer = false;
        coroutineStart = false;
        healthValue = 750;
        Node startNode = grid.grid[startPos[0], startPos[1]];
        transform.position = startNode.worldPosition - new Vector3(0, 0, 1);
        targetNode = startNode;
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
        if (player.GetComponent<Player>().leaveLevel)
        {
            SetUp();
        }
        else if (xInput == 0 && yInput == 0)
        {

            xInput = 0;
            yInput = -1;


            animator.SetFloat("XInput", xInput);
            animator.SetFloat("YInput", yInput);
        }
        else if (targetNode == null)
        {

            animator.SetBool("isWalking", false);
        }
        else if (grid.GetNodeFromWorldPos(this.gameObject.transform.position) == playerPos)
        {
            moveAway = false;
        }

        path2Player = pathFinder.FindPathEnemies(transform.position, player.transform.position);
        if (Next2Player() && !moveAway && (int)healthValue > 0)
        {
            animator.SetBool("isWalking", true);
            attackPlayer = false;
            if (!stay)
            {
                stay = true;
                hitValue = 75 + (7.5f * hitSpeed);

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
                float healthReduc = playerHitvalue / 750;
                healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - healthReduc);
                if ((int)healthValue <= 0)
                {
                    StopAllCoroutines();
                    Hint();
                    this.gameObject.SetActive(false);
                }
            }

        }
        else if (path2Player.Count > 1 && !moveAway)
        {
            attackPlayer = true;
            targetNode = grid.GetNodeFromWorldPos(player.transform.position);

        }
        List<Node> back2Pos = pathFinder.FindPathEnemies(transform.position, targetNode.worldPosition);
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
        else
        {

            animator.SetBool("isWalking", false);
        }
    }

    //Monster movement
    //source: https://forum.unity.com/threads/transform-position-speed.744293/
    private IEnumerator move(List<Node> path)
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
        playerPos = targetNode;
        stay = false;
    }

    //Function to leave a hint after being destroyed
    private void Hint()
    {
        int rand = Random.Range(0, 7);
        if (rand == 3)
        {
            Instantiate(hintKey, transform.position, Quaternion.identity, transform.parent.parent);
        }
    }
}
