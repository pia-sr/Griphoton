using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceOf8Tutorial : MonoBehaviour
{
    //UI
    public Text mainDialog;
    public GameObject skipButton;
    public GameObject touchAni;
    public GameObject options;
    public GameObject questions;
    public GameObject ghost;
    public GameObject hints;

    //Sounds
    public AudioSource typewriter;
    public AudioSource puzzleSound;

    //public variables to communicate with other scripts
    public Player player;
    public Game data;
    public bool inactive;
    public GameObject griphoton;

    //private variables for the tutorial flow
    private int _counter;
    private bool _running;
    private bool _start;


    //Function to set up the tutorial to its start state
    private void SetUp()
    {
        this.transform.parent.GetChild(0).gameObject.SetActive(false);
        this.transform.parent.GetChild(1).gameObject.SetActive(false);
        options.SetActive(false);
        ghost.transform.localPosition = Vector3.up;
        inactive = true;
        _running = false;
        _counter = 0;
        skipButton.SetActive(true);
        string firstSentence = "Thank you for your help, " + data.namePlayer + "!";
        StartCoroutine(WordbyWord(firstSentence));
    }

    // Update is called once per frame
    void Update()
    {
        //The tutorial sets itself to its intial state when the player sets its bool to true
        if (this.gameObject.activeSelf && player.activateTutorial)
        {
            player.activateTutorial = false;
            SetUp();
        }
        //As soon as the puzzle is closed, the sound stops
        if (player.gameObject.activeSelf)
        {
            puzzleSound.Pause();
        }
        //Code waits for the user's touch to go to the next text bit
        if (Input.touchCount > 0 && !_running && EventSystem.current != GameObject.Find("Skip"))
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
            _start = true;
            if (_counter > 8)
            {
                griphoton.SetActive(true);
                player.gameObject.SetActive(true);
                player.SwitchCams();
                player.Unpause();
                griphoton.GetComponent<Upperworld>().SetHouseSolved(this.transform.parent.gameObject.tag);
                this.transform.parent.gameObject.SetActive(false);
            }
        }

        //Text sequences
        if (_start)
        {
            _start = false;
            switch (_counter)
            {
                case 1:
                    ghost.transform.localPosition = new Vector3(-5, 1, 0);
                    this.transform.parent.GetChild(0).gameObject.SetActive(true);
                    string sentence = "Do you see the grid there?";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "I need to fill it with numbers from 1 to 8.| There need to be one 1, two 2s, three 3s and so on...| All the twos need to be next to each other, so do the threes and the fours,... ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "But I cannot put these numbers down randomly.| \nThe numbers which are in a row or column with a number on the side need to be added up to that exact number.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "Next to the field is the button with which you can change the number. The number on top of the button indicates which number you currently use. If you press the button it will change to the next number.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "To put a number onto the field, just tap on the square where the number should be.";
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
                    this.transform.parent.GetChild(1).gameObject.SetActive(true);
                    inactive = false;
                    this.gameObject.SetActive(false);
                    puzzleSound.Play();
                    _counter++;
                    break;
            }
        }

    }


    //Function to type every sentences one word at a time
    //Source: https://answers.unity.com/questions/1424042/animated-text-word-by-word-not-letter-by-letter-co.html
    IEnumerator WordbyWord(string sentence)
    {
        _running = true;
        string[] sentences = sentence.Split('|');
        mainDialog.text = "";
        for (int i = 0; i < sentences.Length; i++)
        {
            string[] words = sentences[i].Split(' ');
            typewriter.Play();
            mainDialog.text += words[0];
            yield return new WaitForSeconds(0.33f);
            for (int j = 1; j < words.Length; ++j)
            {
                typewriter.Play();
                mainDialog.text += " " + words[j];
                yield return new WaitForSeconds(0.33f);
            }
            yield return new WaitForSeconds(0.4f);
        }
        _counter++;
        _running = false;
        touchAni.SetActive(true);
    }

    //Function to skip the tutorial
    public void skipTutorial()
    {
        _counter = 8;
        this.transform.parent.GetChild(0).gameObject.SetActive(true);
        this.transform.parent.GetChild(1).gameObject.SetActive(true);
        options.SetActive(true);
        inactive = false;
        this.gameObject.SetActive(false);
        puzzleSound.Play();
    }

    //Questions and Answers for the help section
    public void Question1()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How do I put a number onto a square?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the square and the number shown on the left button will appear on the square.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Can I remove a number?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Yes, just tap on the square again and the number will disappear.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I change the number?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Tap on the button next to the field to change the number. The number on the button is the number that is currently in use.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the puzzle?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The lowest button is the exit button with which you can leave the puzzle.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I reset the puzzle?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The highst button is the reset button with which you can reset the whole puzzle.";
    }

    //close function for either closing a specific question or the question overview
    public void close()
    {
        if (questions.transform.GetChild(0).GetChild(0).gameObject.activeSelf)
        {
            questions.transform.GetChild(1).gameObject.SetActive(true);
            questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            options.SetActive(true);
            inactive = false;
            questions.SetActive(false);
        }

    }

    //Function to open the question overview
    public void questionOverview()
    {
        options.SetActive(false);
        inactive = true;
        questions.SetActive(true);
    }

    public void WonPuzzle()
    {
        this.gameObject.SetActive(true);
        skipButton.SetActive(false);
        ghost.SetActive(true);
        ghost.transform.localPosition = new Vector3(-5, 1, 0);
        this.transform.parent.GetChild(1).gameObject.SetActive(false);
        int randIndex = Random.Range(0, player.WonSentences().Count);
        StartCoroutine(WordbyWord(player.WonSentences()[randIndex]));

    }

    public void OpenHints()
    {
        options.SetActive(false);
        inactive = true;
        hints.SetActive(true);
    }

    public void CloseHints()
    {
        options.SetActive(true);
        inactive = false;
        hints.SetActive(false);
    }
}
