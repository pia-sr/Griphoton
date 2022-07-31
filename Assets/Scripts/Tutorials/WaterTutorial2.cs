using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaterTutorial2 : MonoBehaviour
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
        ghost.SetActive(true);
        ghost.transform.localPosition = Vector3.up;
        options.SetActive(false);
        inactive = true;
        _running = false;
        _counter = 0;
        skipButton.SetActive(true);
        string firstSentence = "Hi there, " + data.namePlayer + "!| \nDo you want a glass of water?";
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
        if (!this.transform.parent.gameObject.activeSelf)
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

            if (_counter > 11)
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
                    string sentence = "You see, I have three glasses of water and I know for a fact that one glass has a capacity of 7 litres, one has a capacity of 4 litres and the last one has a capacity of 3 litres. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "The 7-litre glass is filled to the brink, but the other two are empty.| I want to use those three glasses to get exactly 2 litres in two glasses and 3 litres in the third one. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "There is a number underneath each glass to show how much litre is the glass.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "Just tap on the glass with water and then tap on a glass that you want to fill with the water from the first glass.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "Once the water from the first glass pours into the second one, it will only stop when the second glass is full or when the first glass is empty.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    sentence = "The catch is that there is a limit on how many times you can pour water into a glass.| You need to get the two 2 litres and the one 3 litres in 6 moves. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    sentence = "If you choose a glass and it moves up, but you change your mind and you put it down again, it does not count as a move. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
                    sentence = "However, if you use an empty glass to attempt to pour water into another one, it will count as a move.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 9:
                    options.SetActive(true);
                    sentence = "You can press the exit button to leave my house.| \nBut remember: your process will not be saved! ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 10:
                    sentence = "If you have any questions, just tap on the icon with the question mark.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 11:
                    inactive = false;
                    this.gameObject.SetActive(false);
                    this.transform.parent.GetChild(1).gameObject.SetActive(true);
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
            mainDialog.text += words[0];
            typewriter.Play();
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
        _counter = 11;
        this.transform.parent.GetChild(0).gameObject.SetActive(true);
        this.transform.parent.GetChild(1).gameObject.SetActive(true);
        options.SetActive(true);
        inactive = false;
        this.gameObject.SetActive(false);
        ghost.SetActive(false);
        puzzleSound.Play();
    }


    //Questions and Answers for the help section
    public void Question1()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I pour water into a glass?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on a glass. This glass will move up. Then choose another glass to which the first glass will move and start to fill up the second glass. After that, the first glass will move back to its starting position.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Can I put down a glass after it already moved up?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Yes, of course. Just tap on that glass again and it will move down. This will not count as movement.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How do I know how much water is in a glass?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Every glass has a number underneath them. This number indicates how much water is in the glass above.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How do I know what the capacity of each glass is?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Each glass has a number written on them. This number indicates the maximum capacity of water the glass has.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the puzzle?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The lowest button is the exit button with which you can leave the puzzle.";
    }
    public void Question6()
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