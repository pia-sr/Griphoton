using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    private GridField grid;
    private Player player;
    private bool active;
    private bool corountineStart;
    private bool attackBool;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        active = false;
        corountineStart = false;
        grid = GameObject.Find("Background").GetComponent<GridField>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!corountineStart)
        {
            corountineStart = true;
            StartCoroutine(activateSpikes());
        }

        if (!attackBool) 
        {
            attackBool = true;
            StartCoroutine(attack());
        }
    }

    IEnumerator activateSpikes()
    {
        active = true;
        animator.SetBool("active", active);
        yield return new WaitForSeconds(4);
        active = false;
        animator.SetBool("active", active);
        yield return new WaitForSeconds(4);
        corountineStart = false;
    }
    IEnumerator attack()
    {
        if (grid.GetNodeFromWorldPos(player.gameObject.transform.position) == grid.GetNodeFromWorldPos(this.transform.localPosition) && active)
        {

            player.GetComponent<Player>().reduceStrength(10);
            yield return new WaitForSeconds(0.75f);
        }
        
        yield return null;
        attackBool = false;
    }
}
