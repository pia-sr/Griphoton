using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public GridField grid;
    private Node targetNode;
    public Pathfinder pathFinder;
    private Node dungeon;
    private List<Node> path;
    private int counter;
    private Vector3 diff;
    private Node existingTarget;
    private float strength;
    public bool lost = false;
    public bool upperWorld;
    public int hitValue;
    public bool enemyHit;
    public bool blockEnemy = false;
    private bool ready = true;
    private bool foundPos = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyHit = false;
        counter = 0;
        hitValue = 25;
        targetNode = null;
        existingTarget = null;
        strength = 100;

    }

    // Update is called once per frame
    void Update()
    {
        if (!foundPos)
        {
            if (dungeon == null && grid.dungeonNode() != null && upperWorld)
            {
                dungeon = grid.dungeonNode();
                Node startPos = grid.grid[dungeon.gridX, dungeon.gridY - 2];
                transform.position = startPos.worldPosition;
                foundPos = true;
            }
            else if (!upperWorld)
            {
                List<Node> entraceNodes = new List<Node>();
                foreach (Node node in grid.grid)
                {
                    if(node.onTop == "Entrance")
                    {
                        entraceNodes.Add(node);
                    }
                }
                if (entraceNodes.Count != 0)
                {
                    transform.position = entraceNodes[1].worldPosition - new Vector3(0,0,1);
                    foundPos = true;
                }
            }
        }
        
        else if (targetNode != null)
        {
            path = pathFinder.FindPathPlayer(transform.position, targetNode.worldPosition);
            if(path.Count > 1)
            {
                
                Node currentNode = grid.GetNodeFromWorldPos(transform.position);
                if(currentNode.gridX - path[1].gridX == 0)
                {
                    diff = new Vector2(0, path[1].worldPosition.y - grid.GetNodeFromWorldPos(transform.position).worldPosition.y);
                }
                else
                {
                    diff = new Vector2(path[1].worldPosition.x - grid.GetNodeFromWorldPos(transform.position).worldPosition.x, 0);
                }

                transform.Translate(diff * 3f * Time.deltaTime);

            }
        }
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (grid.bounds().Contains(touchPosition))
            {
                targetNode = grid.GetNodeFromWorldPos(touchPosition);
                Debug.Log("Coords: " + targetNode.gridX + "," + targetNode.gridY);

            }

            
        }

    }

    public void reduceStrength(float hit)
    {

        
        strength -= hit;
        if(strength <= 0)
        {
            lost = true;
        }
        Debug.Log("Player: " + strength);

    }

    public void attack()
    {
        foreach(Node neighbour in grid.GetNodeNeighbours(grid.GetNodeFromWorldPos(transform.position)))
        {
            if(neighbour.onTop == "Enemy" && ready)
            {
                StartCoroutine(wait2Hit());
            }
        }
    }
    public void block()
    {
        if (ready)
        {
            StartCoroutine(wait2Block());
        }
    }

    IEnumerator wait2Hit()
    {
        ready = false;
        enemyHit = true;
        yield return new WaitForSeconds(1);
        ready = true;
    }

    IEnumerator wait2Block()
    {
        ready = false;
        blockEnemy = true;
        yield return new WaitForSeconds(0.5f);
        blockEnemy = false;
        yield return new WaitForSeconds(2);
        ready = true;
    }
}
