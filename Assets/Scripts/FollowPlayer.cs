using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    // camera will follow this object
    public GameObject player;
    private Transform Target;
    //camera transform
    private Transform camTransform;
    // offset between camera and target
    private Vector3 Offset;
    // change this value to get desired smoothness
    public float SmoothTime;
    public GridField grid;

    // This value will change at the runtime depending on target movement. Initialize with zero vector.
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        camTransform = this.transform;
        Target = player.transform;
        Offset = camTransform.position - Target.position;
    }

    private void LateUpdate()
    {
        //Whenever the player is active the camera follows them around
        if(player.activeSelf)
        {
            // update position
            var CamRect = this.GetComponent<Camera>().rect;
            Vector3 targetPosition = Target.position + Offset;
            //source; https://forum.unity.com/threads/convert-screen-width-to-distance-in-world-space.1072694/
            float aspect = (float)Screen.width / Screen.height;

            float worldHeight = Camera.main.orthographicSize * 2;

            float worldWidth = (worldHeight * aspect) /2;
            Vector3 targetBoundsMax = Target.position + new Vector3(worldWidth, worldHeight/2, 0);
            Vector3 targetBoundsMin = Target.position - new Vector3(worldWidth, worldHeight/2, 0);
            if (targetBoundsMax.x < grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.x && targetBoundsMin.y > grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.y &&
                targetBoundsMax.y < grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.y && targetBoundsMin.x > grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.x)
            {
                camTransform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 2 * Time.deltaTime);
            }
            else if (targetBoundsMax.x < grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.x && targetBoundsMin.x > grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.x)
            {
                camTransform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPosition.x, transform.localPosition.y, targetPosition.z), ref velocity, 2f * Time.deltaTime);
            }
            else if (targetBoundsMin.y > grid.grid[grid.GetGridSizeX() - 1, 0].worldPosition.y && targetBoundsMax.y < grid.grid[0, grid.GetGridSizeY() - 1].worldPosition.y)
            {
                camTransform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.localPosition.x, targetPosition.y, targetPosition.z), ref velocity, 2f * Time.deltaTime);
            }
        }
        else
        {
            camTransform.position = new Vector3(0, 0,-10);
        }
        
            

    }
}
