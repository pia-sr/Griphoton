using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    // camera will follow this object
    public Transform Target;
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
        Offset = camTransform.position - Target.position;
    }

    private void LateUpdate()
    {
        // update position
        var CamRect = this.GetComponent<Camera>().rect;
        Vector3 targetPosition = Target.position + Offset;
        Vector3 targetBoundsMax = Target.position + new Vector3(8.6f, 4.5f, 0);
        Vector3 targetBoundsMin = Target.position - new Vector3(8.6f, 4.5f, 0);
        if (targetBoundsMax.x  < grid.grid[grid.getGridSizeX()-1,0].worldPosition.x && targetBoundsMin.y > grid.grid[grid.getGridSizeX()-1, 0].worldPosition.y &&
            targetBoundsMax.y < grid.grid[0,grid.getGridSizeY()-1].worldPosition.y && targetBoundsMin.x > grid.grid[0, grid.getGridSizeY()-1].worldPosition.x)
        {
            camTransform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1 * Time.deltaTime);
        }
        else if(targetBoundsMax.x < grid.grid[grid.getGridSizeX() - 1, 0].worldPosition.x && targetBoundsMin.x > grid.grid[0, grid.getGridSizeY() - 1].worldPosition.x)
        {
            camTransform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPosition.x,transform.localPosition.y,targetPosition.z), ref velocity, 1.5f * Time.deltaTime);
        }
        else if(targetBoundsMin.y > grid.grid[grid.getGridSizeX() - 1, 0].worldPosition.y && targetBoundsMax.y < grid.grid[0, grid.getGridSizeY() - 1].worldPosition.y)
        {
            camTransform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.localPosition.x,targetPosition.y,targetPosition.z), ref velocity, 1.5f * Time.deltaTime);
        }
            

    }
}
