using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton1 : MonoBehaviour
{
    public int[] start = new int[2];
    public int[] end = new int[2];
    public GridField grid;
    public GameObject player;
    private Node startNode;
    private Node endNode;
    public Pathfinder pathFinder;
    private List<Node> path2Player;
    private bool startPos;
    public float levelStrength;
    private int axis;

    // Start is called before the first frame update
    void Start()
    {
        startNode = grid.grid[start[0], start[1]];
        endNode = grid.grid[end[0], end[1]];
        transform.position = startNode.worldPosition;
        startPos = true;
        if(start[1] -end[1] == 0)
        {
            axis = start[1];
        }
        else
        {
            axis = start[0];
        }

    }

    // Update is called once per frame
    void Update()
    {
        path2Player = pathFinder.FindPath(transform.position, player.transform.position);
        if(path2Player.Count < 9 && player.active == true)
        {
            //transform.Translate(diff * 20f * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, path2Player[1].worldPosition, 1f * Time.deltaTime);
        }
        else
        {
            if(grid.GetNodeFromWorldPos(transform.position).gridY != axis)
            {
                Debug.Log(23456);
                List<Node> back2Pos = pathFinder.FindPath(transform.position, startNode.worldPosition);
                transform.position = Vector2.MoveTowards(transform.position, back2Pos[0].worldPosition, 1f * Time.deltaTime);
            }
            if(startPos)
            {

                transform.position = Vector2.MoveTowards(transform.position, endNode.worldPosition, 1f * Time.deltaTime);
                if(grid.GetNodeFromWorldPos(transform.position) == endNode)
                {
                    startPos = false;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, startNode.worldPosition, 1f * Time.deltaTime);
                if (grid.GetNodeFromWorldPos(transform.position) == startNode)
                {
                    startPos = true;
                }
            }
        }
        foreach(Node neighbour in grid.GetNodeNeighbours(grid.GetNodeFromWorldPos(transform.position)))
        {
            if(neighbour == grid.GetNodeFromWorldPos(player.transform.position))
            {
                player.GetComponent<Player>().reduceStrength(levelStrength);
            }
        }
    }
}
