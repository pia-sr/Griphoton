using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MirrorTutorial : MonoBehaviour
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
        string firstSentence = "Hello " + data.namePlayer + ",| \nWelcome to my home.";
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
                    string sentence = "I want to build a room full of mirrors as an attraction.| People who visit my room, wait no| let's call it my hall of mirrors, need to find the way out of the room without running into mirrors.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "I made some kind of blueprint for the hall, but I forgot to put in some important parts.| \nCan you maybe help me to get the full design for my hall of mirrors?";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "I divided the room into squares.| On every square should either be a person lost in my hall of mirrors or a diagonal mirror. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "The numbers next to the room indicate how many people can be seen from that position.| Do not forget to count the reflections of people, too.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "If you tap on a square, I will put a person on that square.| If you tap on that square again, I will put one of the diagonal mirrors on it.| If you tap another time, the other diagonal mirror will appear.| If you tap one last time all the symbols will disappear on that square.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    options.SetActive(true);
                    sentence = "You can press the exit button to leave my house.| \nBut remember: your process will not be saved! ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    sentence = "If you have any questions, just tap on the icon with the question mark.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
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
        question.text = "How can I put a person on a square?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Tap on the square on which you would like the person to be and the person will appear.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I put a diagonal mirror on a square?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "There are two types of diagonal mirrors. If you tap on an empty square two times one of them will appear. If you tap another time, the other mirror will appear.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I remove a person or mirror on top of a square?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on that square until it is empty. If you want to remove all the people and mirrors, you can press the reset button.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the puzzle?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The lowest button is the exit button with which you can leave the puzzle.";
    }
    public void Question5()
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