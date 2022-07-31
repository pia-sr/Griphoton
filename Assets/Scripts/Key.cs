using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private GameObject player;
    private Game data;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player");
        data = GameObject.Find("GameData").GetComponent<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<SpriteRenderer>().bounds.Contains(player.transform.position))
        {
            Destroy(this.gameObject);
            data.KeyIncrease();
        }
    }
}
