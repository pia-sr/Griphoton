/*
 * Reaper.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper : MonoBehaviour
{
    //public variables
    public GridField grid;
    public GameObject player;
    public float hitValue = 750;
    public Animator animator;
    
    //private variables
    private float healthValue;
    private bool hitPlayer = false;
    private HealthBar healthBar;

    void Start()
    {

        int middleY = Mathf.RoundToInt(grid.GetGridSizeY() / 2);
        transform.position = grid.grid[grid.GetGridSizeX() - 2, middleY].worldPosition;
        grid.grid[grid.GetGridSizeX() - 2, middleY].isWalkable = false;
        SetUp();
    }

    //Function to bring the reaper to their start position
    private void SetUp()
    {
        StopAllCoroutines();
        hitPlayer = false;
        healthValue = 1500;
        healthBar = this.transform.GetChild(0).GetChild(0).GetComponent<HealthBar>();
        healthBar.SetHealthBarValue(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Next2Player())
        {
            if (!hitPlayer)
            {
                hitPlayer = true;
                StartCoroutine(Attack());
            }
            if (player.GetComponent<Player>().enemyHit)
            {

                player.GetComponent<Player>().enemyHit = false;
                healthValue -= player.GetComponent<Player>().hitValue;
                float playerHitvalue = player.GetComponent<Player>().hitValue;
                float healthReduc = playerHitvalue / 1500;
                healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - healthReduc);
                if (healthValue <= 0)
                {
                    StopAllCoroutines();
                    this.gameObject.SetActive(false);
                }
            }
        }
    }


    //Checks if they are next to the player
    private bool Next2Player()
    {
        foreach (Node neightbour in grid.GetNodeNeighboursDiagonal(grid.GetNodeFromWorldPos(this.transform.position)))
        {
            if (neightbour == grid.GetNodeFromWorldPos(player.transform.position))
            {
                return true;
            }
        }
        return false;
    }


    //Function to attack the player
    IEnumerator Attack()
    {
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Player>().ReduceStrength(hitValue);
        yield return new WaitForSeconds(2.5f);

        hitPlayer = false;
    }
}
