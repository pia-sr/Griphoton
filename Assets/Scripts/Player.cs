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
    private Vector2 diff;
    private Node existingTarget;
    private float strength;
    public bool lost = false;
    private bool upperWorld = false;

    // Start is called before the first frame update
    void Start()
    {
        List<Node> path = new List<Node>();
        counter = 0;
        targetNode = null;
        existingTarget = null;
        strength = 100;
        if (!upperWorld)
        {
            int middleX = Mathf.RoundToInt(grid.getGridSizeX() / 2);
            transform.position = grid.grid[middleX, 1].worldPosition - new Vector3(0,0,1);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (dungeon == null && grid.dungeonNode() != null && upperWorld)
        {
            dungeon = grid.dungeonNode();
            Node startPos = grid.grid[dungeon.gridX, dungeon.gridY - 2];
            transform.position = startPos.worldPosition;
        }
        else if (targetNode != null)
        {
            path = pathFinder.FindPath(transform.position, targetNode.worldPosition);
            if(path.Count > 1)
            {
                Node currentNode = grid.GetNodeFromWorldPos(transform.position);
                if(currentNode.gridX - path[1].gridX == 0)
                {
                    diff = new Vector2(0, path[1].worldPosition.y - transform.position.y);
                }
                else
                {
                    diff = new Vector2(path[1].worldPosition.x - transform.position.x,0);
                }
           

                transform.Translate(diff * 4f * Time.deltaTime);

            }
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetNode = grid.GetNodeFromWorldPos(touchPosition);

        }

    }

    public void reduceStrength(float hit)
    {
        strength -= hit;
        if(strength <= 0)
        {
            lost = true;
        }

    }

}
