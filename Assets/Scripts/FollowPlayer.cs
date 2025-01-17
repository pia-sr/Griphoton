using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    // camera will follow this object
    public GameObject player;
    private Transform Target;
    //camera transform
    private Transform camTransform;
    // change this value to get desired smoothness
    public float SmoothTime;
    public GridField grid;


    private void Start()
    {
        camTransform = this.transform;
        Target = player.transform;
    }

    private void LateUpdate()
    {
        //Whenever the player is active the camera follows them around
        if(player.activeSelf)
        {
            // update position
            var CamRect = this.GetComponent<Camera>().rect;
            Vector3 targetPosition = Target.position;
            targetPosition.z = this.transform.position.z;
            //source; https://forum.unity.com/threads/convert-screen-width-to-distance-in-world-space.1072694/
            float aspect = (float)Screen.width / Screen.height;

            float worldHeight = Camera.current.orthographicSize * 2;

            float worldWidth = (worldHeight * aspect) /2;
            Vector3 targetBoundsMax = Target.position + new Vector3(worldWidth, worldHeight/2, 0);
            Vector3 targetBoundsMin = Target.position - new Vector3(worldWidth, worldHeight/2, 0);
            if (targetBoundsMax.x < grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.x && targetBoundsMin.y > grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.y &&
                targetBoundsMax.y < grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.y && targetBoundsMin.x > grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.x)
            {
                camTransform.position = targetPosition;
            }
            else if (targetBoundsMax.x < grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.x && targetBoundsMin.x > grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.x)
            {
                camTransform.position = new Vector3(targetPosition.x, transform.localPosition.y, targetPosition.z);
            }
            else if (targetBoundsMin.y > grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.y && targetBoundsMax.y < grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.y)
            {
                camTransform.position = new Vector3(transform.localPosition.x, targetPosition.y, targetPosition.z);
            }
        }
        else
        {
            camTransform.position = new Vector3(0, 0,-10);
        }
        
            

    }
}
