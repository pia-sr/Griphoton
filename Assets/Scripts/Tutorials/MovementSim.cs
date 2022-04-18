using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSim : MonoBehaviour
{
    public GameObject touchSim;
    private bool go;

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
            this.transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(2, 2), 1f * Time.deltaTime);
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
        touchSim.SetActive(false);
        go = true;
    }
}
