using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HamiltonianTutorial : MonoBehaviour
{
    public Text mainDialog;
    public Game data;
    private int counter;
    public GameObject skipButton;
    public GameObject touchAni;
    public GameObject options;
    private bool running;
    private bool start;
    public bool inactive;
    public GameObject questions;
    public GameObject ghost;

    public void setUp()
    {
        this.transform.parent.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.parent.transform.GetChild(1).gameObject.SetActive(false);
        options.SetActive(false);
        ghost.transform.localPosition = Vector3.zero;
        inactive = true;
        running = false;
        counter = 0;
        skipButton.SetActive(true);
        string firstSentence = "Thank you for your help, " + data.namePlayer + ".";
        StartCoroutine(WordbyWord(firstSentence));


    }

    // Start is called before the first frame update
    void Start()
    {
        setUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !running && EventSystem.current != GameObject.Find("Skip"))
        {

            touchAni.GetComponent<TouchAnimation>().running = false;
            if (touchAni.transform.childCount > 1)
            {
                for (int i = 1; i < touchAni.transform.childCount; i++)
                {
                    Destroy(touchAni.transform.GetChild(i).gameObject);
                }

            }
            touchAni.SetActive(false);
            start = true;
        }
        if (start)
        {
            start = false;
            switch (counter)
            {
                case 1:
                    ghost.transform.localPosition = new Vector3(-5, 0, 0);
                    this.transform.parent.transform.GetChild(0).gameObject.SetActive(true);
                    string sentence = "My problem consists of a pipe construct.| Between every pipe is a sphere to hold it together.| Sadly I do not know through which pipe the water flows.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "I am only certain about two things:| \nOne thing is that the water passes every sphere exactly once.| \nAnd the other thing is that water flows in one big circuit.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "Can you colour in all the pipes through which the water flows?| \nJust tap on the pipe through which you believe the water flows and it will be coloured in. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    options.SetActive(true);
                    sentence = "You can press the exit button to leave my house.| \nBut remember: your process will not be saved! ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "If you have any questions, just tap on the icon with the question mark.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    this.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
                    inactive = false;
                    this.gameObject.SetActive(false);
                    break;
            }
        }

    }


    //Source: https://answers.unity.com/questions/1424042/animated-text-word-by-word-not-letter-by-letter-co.html
    IEnumerator WordbyWord(string sentence)
    {
        running = true;
        string[] sentences = sentence.Split('|');
        mainDialog.text = "";
        for (int i = 0; i < sentences.Length; i++)
        {
            string[] words = sentences[i].Split(' ');
            mainDialog.text += words[0];
            for (int j = 1; j < words.Length; ++j)
            {
                yield return new WaitForSeconds(0.2f);
                //Typewriter sound
                mainDialog.text += " " + words[j];
            }
            yield return new WaitForSeconds(0.4f);
        }
        counter++;
        running = false;
        touchAni.SetActive(true);
    }

    public void skipTutorial()
    {
        this.transform.parent.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
        options.SetActive(true);
        inactive = false;
        this.gameObject.SetActive(false);
    }

    public void Question1()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I colour in a pipe?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the pipe you want to colour in and the colour will appear.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I remove the colour of a pipe?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the coloured-in pipe and the colour will be removed. If you want to remove the colour of all the pipes, you can just press the restart button.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the puzzle?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The lowest button is the exit button with which you can leave the puzzle.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I reset the puzzle?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The highst button is the reset button with which you can reset the whole puzzle.";
    }

    public void closeQuestionOverview()
    {
        options.SetActive(true);
        inactive = false;
        questions.SetActive(false);
    }
    public void closeQuestion()
    {
        questions.transform.GetChild(1).gameObject.SetActive(true);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
    }
    public void questionOverview()
    {
        options.SetActive(false);
        inactive = true;
        questions.SetActive(true);
    }
}
