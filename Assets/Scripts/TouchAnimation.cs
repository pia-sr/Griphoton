using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAnimation : MonoBehaviour
{
    public GameObject line;
    public bool running;
    

    // Start is called before the first frame update
    void Start()
    {
        GameObject circle = Instantiate(line, transform.position, Quaternion.identity, this.transform);
        float radius = 0.15f;
        DrawCircle(circle, radius);
        circle.transform.Rotate(90f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!running)
        {
            StartCoroutine(increaseCircle());
        }
    }
    
    //Function draw increasingly bigger circles so it seems like an animation
    IEnumerator increaseCircle()
    {
        running = true;
        int counter = 0;
        for (int i = 0; i < 5; i++)
        {
            GameObject circle = Instantiate(line, transform.position, Quaternion.identity, this.transform);
            float radius = 0.2f + (counter * 0.05f);
            DrawCircle(circle, radius); 
            circle.transform.Rotate(90f, 0, 0);
            yield return new WaitForSeconds(0.2f);
            Destroy(circle);
            counter++;

        }
        running = false;
    }

    //Function to drwa a circle
    //source https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html
    private void DrawCircle(GameObject circle, float radius)
    {
        var segments = 360;
        var circleRend = circle.AddComponent<LineRenderer>();
        circleRend.useWorldSpace = false;
        circleRend.startWidth = 0.05f;
        circleRend.endWidth = 0.05f;
        circleRend.positionCount = segments + 1;
        circleRend.material.color = Color.black;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        circleRend.SetPositions(points);
    }
}
