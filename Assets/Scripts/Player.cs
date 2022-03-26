using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GridField grid;
    private Node targetNode;
    public Pathfinder pathFinder;
    private Node dungeon;
    private List<Node> path;
    private int counter;
    private CharacterController controller;
    private Vector2 diff;
    private Node existingTarget;

    // Start is called before the first frame update
    void Start()
    {
        List<Node> path = new List<Node>();
        counter = 0;
        targetNode = null;
        existingTarget = null;
        //controller = gameObject.AddComponent<CharacterController>();
        /*
        Node dungeon = grid.dungeonNode();
        Node startNode = grid.grid[dungeon.gridX, dungeon.gridY - 2];
        transform.position = startNode.worldPosition;*/
        //pathFinder = grid.GetComponent<Pathfinder>();
    }

    // Update is called once per frame
    void Update()
    {

        if (dungeon == null && grid.dungeonNode() != null)
        {
            dungeon = grid.dungeonNode();
            Node startPos = grid.grid[dungeon.gridX, dungeon.gridY - 2];
            transform.position = startPos.worldPosition;
        }
        else if (targetNode != null)
        {
            if (path == null || (targetNode != existingTarget && diff == Vector2.zero))
            {
                existingTarget = targetNode;
                path = pathFinder.FindPath(transform.position, targetNode.worldPosition);
                counter = 0;
            }
            else if (counter < path.Count)
            {
                diff = path[counter].worldPosition - transform.position;
                if (diff == Vector2.zero && targetNode == existingTarget)
                {
                    counter++;
                    diff = path[counter].worldPosition - transform.position;
                }

                //controller.Move(diff * Time.deltaTime * 20);
                //transform.Translate(diff * 20f * Time.deltaTime);
                transform.position = Vector2.MoveTowards(transform.position, path[counter].worldPosition, 0.05f);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetNode = grid.GetNodeFromWorldPos(touchPosition);

        }

    }

}
