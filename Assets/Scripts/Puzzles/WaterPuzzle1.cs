using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPuzzle1 : MonoBehaviour
{
    //public field objects
    public GameObject messageExit;
    public List<GameObject> glasses;
    public Text counterText;
    public List<Text> litres;

    //public variables to communicate with other scripts
    public GameObject griphoton;
    public GameObject player;
    public PolyAddTutorial tutorial;

    private int[] maxValue;
    private int[] currentValue;
    private List<Vector3> positions;
    private List<GameObject> glassesInUse;
    private List<float> steps;
    private int counterMovements;

    public void SetUp()
    {
        maxValue = new int[] { 8, 5, 3 };
        currentValue = new int[] { 8, 0, 0 };
        glassesInUse = new List<GameObject>();
        counterMovements = 0;
        counterText.text = counterMovements.ToString();
        int counter = 0;
        foreach(GameObject glass in glasses)
        {
            GlassFill glassfill = glass.GetComponent<GlassFill>();
            glassfill.SetGlassValue(currentValue[counter] / maxValue[counter]);
            glassfill.RotateOriginal();
            glass.transform.localRotation = Quaternion.Euler(Vector3.zero);
            glass.transform.position = positions[counter];
            litres[counter].text = currentValue[counter].ToString();
            counter++;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector3>();
        foreach(GameObject glass in glasses)
        {
            positions.Add( glass.transform.position);
        }
        steps = new List<float>() { 0.01f, 0.014f, 0.025f };
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        //if (Input.touchCount > 0 && !tutorial.inactive)
        {

            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Touch touch = Input.GetTouch(0);
            //Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (Input.GetMouseButtonDown(0))
            //if (touch.phase == TouchPhase.Began)
            {
                if(glassesInUse.Count == 0)
                {
                    foreach (GameObject glass in glasses)
                    {
                        var bounds = glass.GetComponent<SpriteRenderer>().bounds;
                        if (bounds.Contains(touchPosition))
                        {
                            glassesInUse.Add(glass);
                            StartCoroutine(MoveUp(glass));

                        }

                    }
                }

                else if(glassesInUse.Count == 1)
                {
                    foreach (GameObject glass in glasses)
                    {
                        var bounds = glass.GetComponent<SpriteRenderer>().bounds;
                        if (bounds.Contains(touchPosition))
                        {
                            if(glass == glassesInUse[0])
                            {
                                StartCoroutine(MoveDown(glass));
                                glassesInUse.Remove(glass);
                            }
                            else
                            {
                                glassesInUse.Add(glass);
                                StartCoroutine(Move2Glass());

                            }

                        }

                    }


                }

            }



        }
        if(glassesInUse.Count == 2 && glassesInUse[0].GetComponent<GlassFill>().done == true && glassesInUse[1].GetComponent<GlassFill>().done == true)
        {
            glassesInUse[0].GetComponent<GlassFill>().done = false;
            glassesInUse[1].GetComponent<GlassFill>().done = false;
            StartCoroutine(RotateBack(glassesInUse[0]));
        }
        if (CheckWin() && glassesInUse.Count == 0)
        {
            Debug.Log("Won");
            //tutorial.inactive = true;
            //tutorial.WonPuzzle();
        }

        
    }

    IEnumerator MoveUp(GameObject glass)
    {
        Vector3 pos = glass.transform.position;
        while(pos.y != 0.5f)
        {
            pos.y = Mathf.MoveTowards(pos.y, 0.5f, 1.5f * Time.deltaTime);
            glass.transform.position = pos;
            yield return null;
        }
    }
    IEnumerator Move2Glass()
    {
        GameObject glass = glassesInUse[0];
        counterMovements++;
        counterText.text = counterMovements.ToString();
        yield return new WaitUntil(() => glass.transform.position.y == 0.5f);
        Vector3 pos = glass.transform.position;
        int index2 = glasses.IndexOf(glassesInUse[1]);
        int index1 = glasses.IndexOf(glass);
        int dis = Mathf.Abs(index2 - glasses.IndexOf(glass));
        float goal = -(0.5f * (index1 + 1) + (3* (int)(dis/2)));
        int rotationValue = 90;
        if(index2 > glasses.IndexOf(glass))
        {
            goal *= -1;
            rotationValue = -90;
        }
        goal += pos.x; 
        while (pos.x != goal)
        {
            pos.x = Mathf.MoveTowards(pos.x, goal, 1.5f * Time.deltaTime);
            glass.transform.position = pos;
            yield return null;
        }
        StartCoroutine(RotateAndFill(glass, glassesInUse[1], rotationValue));
    }

    IEnumerator RotateAndFill(GameObject glass1, GameObject glass2, float goal)
    {
        Vector3 rot = glass1.transform.localEulerAngles;
        GlassFill glassFill1 = glass1.GetComponent<GlassFill>();
        GlassFill glassFill2 = glass2.GetComponent<GlassFill>();
        GameObject spill = glass1.transform.GetChild(1).gameObject;
        glassFill1.Rotate90();
        if( goal == -90)
        {
            spill = glass1.transform.GetChild(0).gameObject;
            glassFill1.Rotate270();
        }
        while (rot.z != goal)
        {
            float rotationZ = rot.z;
            rotationZ = Mathf.MoveTowards(rotationZ, goal, 45f * Time.deltaTime);
            rot.z = rotationZ;
            glass1.transform.localRotation = Quaternion.Euler(rot);
            yield return null;
        }
        int index1 = glasses.IndexOf(glass1);
        int index2 = glasses.IndexOf(glass2);

        if(currentValue[index1] > 0)
        {
            int dif = maxValue[index2] - currentValue[index2];
            if(currentValue[index1] > dif)
            {
                int addUp = dif; 
                spill.SetActive(true);
                float result = (float) addUp / maxValue[index1];
                StartCoroutine(glassFill1.DryUp(1 - result, steps[index1]));
                StartCoroutine(glassFill2.FillUp(1, steps[index2]));
                currentValue[index1] = currentValue[index1] - addUp;
                currentValue[index2] = currentValue[index2] + addUp;
            }
            else
            {

                int addUp = currentValue[index1];
                spill.SetActive(true);
                int preResult = currentValue[index2] + addUp;
                float result = (float)preResult / maxValue[index2];
                StartCoroutine(glassFill1.DryUp(0, steps[index1]));
                StartCoroutine(glassFill2.FillUp(result, steps[index2]));
                currentValue[index1] = currentValue[index1] - addUp;
                currentValue[index2] = currentValue[index2] + addUp;
            }

        }
        else
        {
            glassFill1.done = true;
            glassFill2.done = true;
        }
       
    }

    IEnumerator RotateBack(GameObject glass)
    {
        Vector3 rot = glass.transform.localEulerAngles;
        int index1 = glasses.IndexOf(glass);
        int index2 = glasses.IndexOf(glassesInUse[1]);
        litres[index1].text = currentValue[index1].ToString();
        litres[index2].text = currentValue[index2].ToString();
        int goalR = 0;
        if(rot.z == 270)
        {

            glass.transform.GetChild(0).gameObject.SetActive(false);
            goalR = 360;
        }
        else
        {

            glass.transform.GetChild(1).gameObject.SetActive(false);
        }
        GlassFill glassFill = glass.GetComponent<GlassFill>();
        while (!(rot.z == 0  || rot.z == 360))
        {
            float rotationZ = rot.z;
            rotationZ = Mathf.MoveTowards(rotationZ, goalR, 45 * Time.deltaTime);
            rot.z = rotationZ;
            glass.transform.localRotation = Quaternion.Euler(rot);
            yield return null;
        }
        glassFill.RotateOriginal();

        Vector3 pos = glass.transform.position;
        float goal = positions[index1].x;
        while (pos.x != goal)
        {
            pos.x = Mathf.MoveTowards(pos.x, goal, 1.5f * Time.deltaTime);
            glass.transform.position = pos;
            yield return null;
        }
        glassesInUse.Clear();
        StartCoroutine(MoveDown(glass));
    }

    IEnumerator MoveDown(GameObject glass)
    {
        Vector3 pos = glass.transform.position;
        while (pos.y != -2.5)
        {
            pos.y = Mathf.MoveTowards(pos.y, -2.5f, 1.5f * Time.deltaTime);
            glass.transform.position = pos;
            yield return null;
        }
        glassesInUse.Clear();
    }

    private bool CheckWin()
    {
        if(counterMovements > 7)
        {
            return false;
        }
        int counter = 0;
        foreach(int value in currentValue)
        {
            if(value == 4)
            {
                counter++;
            }
        }
        if(counter == 2)
        {
            return true;
        }
        return false;
    }

    //Function for the restart button
    public void restart()
    {
        if (!tutorial.inactive)
        {
            SetUp();
        }
    }

    //Function for the leave button
    //Open up a panel to check if the user really wants to leave
    public void leave()
    {
        if (!tutorial.inactive)
        {
            tutorial.inactive = true;
            messageExit.SetActive(true);
        }

    }

    //Function for the yes button, if the user really wants to leave
    public void yes()
    {
        SetUp();
        this.transform.parent.transform.parent.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(true);
        griphoton.SetActive(true);
        player.SetActive(true);
        player.GetComponent<Player>().SwitchCams();
        player.GetComponent<Player>().Unpause();
        messageExit.SetActive(false);
    }

    //Function for the no button, if the user does not want to leave
    public void no()
    {
        tutorial.inactive = false;
        messageExit.SetActive(false);
    }
}
