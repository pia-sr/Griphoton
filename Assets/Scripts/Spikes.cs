/*
 * Spikes.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    private GridField _grid;
    private Player _player;
    private bool _active;
    private bool _corountineStart;
    private bool _attackBool;
    public Animator animator;
    private float _hitValue;


    // Start is called before the first frame update
    void Start()
    {
        _active = false;
        _corountineStart = false;
        _grid = GameObject.Find("Background").GetComponent<GridField>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _hitValue = 5f * (int.Parse(this.transform.parent.parent.tag) - 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_corountineStart)
        {
            _corountineStart = true;
            StartCoroutine(ActivateSpikes());
        }

        if (!_attackBool) 
        {
            _attackBool = true;
            StartCoroutine(Attack());
        }
    }
    //Function to active the spikes after a specific amount of time
    IEnumerator ActivateSpikes()
    {
        _active = true;
        animator.SetBool("active", _active);
        yield return new WaitForSeconds(4);
        _active = false;
        animator.SetBool("active", _active);
        yield return new WaitForSeconds(4);
        _corountineStart = false;
    }
    //Function to attack the player if they are on top of an active spike
    IEnumerator Attack()
    {
        if (_grid.GetNodeFromWorldPos(_player.gameObject.transform.position) == _grid.GetNodeFromWorldPos(this.transform.localPosition) && _active)
        {

            _player.GetComponent<Player>().ReduceStrength(_hitValue);
            yield return new WaitForSeconds(0.75f);
        }
        
        yield return null;
        _attackBool = false;
    }
}
