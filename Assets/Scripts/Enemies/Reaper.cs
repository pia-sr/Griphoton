using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper : MonoBehaviour
{

    public GridField grid;
    public GameObject player;
    public float hitValue = 500;
    private float healthValue;
    private bool hitPlayer = false;
    private HealthBar healthBar;
    public Animator animator;

    void Start()
    {

        int middleY = Mathf.RoundToInt(grid.getGridSizeY() / 2);
        transform.position = grid.grid[grid.getGridSizeX() - 2, middleY].worldPosition + new Vector3(0, 0, -1f);
        begin();
    }
    private void begin()
    {
        StopAllCoroutines();
        hitPlayer = false;
        healthValue = 100;
        healthBar = transform.GetChild(0).GetComponentInChildren<HealthBar>();
        healthBar.SetHealthBarValue(1);
        grid.GetNodeFromWorldPos(transform.position).isWalkable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (next2Player())
        {
            if (!hitPlayer)
            {
                hitPlayer = true;
                animator.SetBool("attack", hitPlayer);
                StartCoroutine(hit());
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
    IEnumerator hit()
    {

        //
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Player>().reduceStrength(hitValue);
        animator.SetBool("attack", false);
        yield return new WaitForSeconds(2.5f);

        hitPlayer = false;
    }
}
