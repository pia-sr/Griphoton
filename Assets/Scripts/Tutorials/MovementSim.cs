using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSim : MonoBehaviour
{
    public GameObject touchSim;
    private bool go;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        touchSim.SetActive(true);
        StartCoroutine(wait());
    }

    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            animator.SetFloat("YInput", 0);
            animator.SetFloat("XInput", 1);
            animator.SetBool("isWalking", true);

            this.transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(2, 3), 1f * Time.deltaTime);
            if(transform.localPosition == new Vector3(2, 3, 0))
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
        touchSim.SetActive(false);
        go = true;
    }
}
