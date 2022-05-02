using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper : MonoBehaviour
{
    //public variables
    public GridField grid;
    public GameObject player;
    public float hitValue = 500;
    public Animator animator;
    
    //private variables
    private float healthValue;
    private bool hitPlayer = false;
    private HealthBar healthBar;

    void Start()
    {

        int middleY = Mathf.RoundToInt(grid.GetGridSizeY() / 2);
        transform.position = grid.grid[grid.GetGridSizeX() - 2, middleY].worldPosition + new Vector3(0, 0, -1f);
        SetUp();
    }

    //Function to bring the reaper to their start position
    private void SetUp()
    {
        StopAllCoroutines();
        hitPlayer = false;
        healthValue = 1000;
        healthBar = transform.GetChild(0).GetComponentInChildren<HealthBar>();
        healthBar.SetHealthBarValue(1);
        grid.GetNodeFromWorldPos(transform.position).isWalkable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Next2Player())
        {
            if (!hitPlayer)
            {
                hitPlayer = true;
                animator.SetBool("attack", hitPlayer);
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
                    this.gameObject.SetActive(false);
                }
            }
        }
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


    //Function to attack the player
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Player>().ReduceStrength(hitValue);
        animator.SetBool("attack", false);
        yield return new WaitForSeconds(2.5f);

        hitPlayer = false;
    }
}
