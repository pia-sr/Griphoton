using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CMTutorial : MonoBehaviour
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
        options.SetActive(false);
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

            if (_counter > 13)
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
                    ghost.SetActive(false);
                    this.transform.parent.GetChild(0).gameObject.SetActive(true);
                    string sentence = "I have three snakes and three mice and I need to get all of them across the river.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "I can only carry two animals with me on the boat.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "The problem is that if there are more snakes than mice on either side, the snakes will eat the mice.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "Another problem is that the boat is not the best and can only go across the river 11 times.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "Can you help me get all the animals across the river?";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    sentence = "Just tap on the animal which you want to have on the boat.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    sentence = "If you want to remove an animal from the boat, just tap on that animal and it will be put back on the riverbank.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
                    sentence = "If there is at least one animal in the boat, you can press the button at the bottom middle of your screen to make the boat move.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 9:
                    sentence = "At the top of your screen, you see how many times the boat has crossed the river. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 10:
                    options.SetActive(true);
                    sentence = "If you should get stuck on the puzzle, you can try one of the hint cards.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 11:
                    sentence = "You can press the exit button to leave my house.| \nBut remember: your process will not be saved! ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 12:
                    sentence = "If you have any questions, just tap on the icon with the question mark.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 13:
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
        _counter = 14;
        this.transform.parent.GetChild(0).gameObject.SetActive(true);
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
        question.text = "What am I supposed to do again?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You need to get all the animals on the left riverside to the right riverside. But remember: the snakes will eat the mice if there are more snakes than the mice. Also, the boat can cross the river only 11 times.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Who wants to eat who again?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "The snakes will eat the mice, but only if there are more snakes than mice on the river bank.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I put an animal onto the boat?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the animal you want on the boat and it will be put on there unless the boat is full.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Can I remove an animal from the boat?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Yes, just tap in the animal that you want to remove and it will be put back on the riverside.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I move the boat?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Just press the button at the bottom middle of your screen and the boat will move to the other side as long there is at least one animal on the boat.";
    }
    public void Question6()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How many times can the boat cross the river?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "The boat can cross the river a maximum of 11 times.";
    }
    public void Question7()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How many animals can I put on the boat?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You can put a maximum of two animals on the boat. But remember that the boat will not move unless there is at least one animal on the boat.";
    }
    public void Question8()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the puzzle?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The lowest button is the exit button with which you can leave the puzzle.";
    }
    public void Question9()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I reset the puzzle?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The highst button is the reset button with which you can reset the whole puzzle.";
    }
    public void Question10()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Where are the hint cards?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You open the hint cards by tapping on the button with the magnifying glass on it. Then you can unlock one of the hint cards.";
    }
    public void Question11()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I unlock a hint card?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You can unlock a hint card with one of the keys you collected in the dungeon. If you do not have any keys, you can watch a video instead to get one.";
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
