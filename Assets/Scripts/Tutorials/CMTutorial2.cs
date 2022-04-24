using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CMTutorial2 : MonoBehaviour
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
        ghost.SetActive(true);
        ghost.transform.localPosition = Vector3.zero;
        inactive = true;
        running = false;
        counter = 0;
        skipButton.SetActive(true);
        string firstSentence = "Thank you for your help, " + data.namePlayer + "!";
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
                    ghost.SetActive(false);
                    this.transform.parent.transform.GetChild(0).gameObject.SetActive(true);
                    string sentence = "I have three wolves, three snakes and three mice and I need to get all of them across the river.| I can only carry three animals with me on the boat.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "The problem is that if there are more wolves than snakes on either side, the wolves will eat the snakes.| And of course, if there are more snakes than mice on either side, the snakes will eat the mice.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "For some reason the wolves will not eat the mice.| But that makes traveling with them a bit easier.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "Another problem is that the boat is not the best and can only go across the river 13 times.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "Can you help me get all the animals across the river?";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    sentence = "Just tap on the animal which you want to have on the boat.| If you want to remove an animal from the boat, just tap on that animal and it will be put back on the riverbank.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    sentence = "If there are at least two animals in the boat, you can press the button at the bottom middle of your screen to make the boat move.| At the top of your screen, you see how many times the boat has crossed the river. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
                    options.SetActive(true);
                    sentence = "You can press the exit button to leave my house.| \nBut remember: your process will not be saved! ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 9:
                    sentence = "If you have any questions, just tap on the icon with the question mark.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 10:
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
        options.SetActive(true);
        inactive = false;ghost.SetActive(false);
        this.gameObject.SetActive(false);

    }

    public void Question1()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "What am I supposed to do again?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "You need to get all the animals on the left riverside to the right riverside. But remember: the wolves will eat the snakes if there are more wolves than snakes. And the snakes will eat the mice if there are more snakes than mice. Also, the boat can cross the river only 13 times.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "Who wants to eat who again?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "The wolves will eat the snakes, but only if there are more wolves than snakes on the river bank. And the snakes will eat the mice, if there are more snakes than mice on the riverbank. Luckly, the wolves will not eat the mice, no matter how many wolves or mice are on the same riverbank.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I put an animal onto the boat?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the animal you want on the boat and it will be put on there unless the boat is full.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "Can I remove an animal from the boat?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Yes, just tap in the animal that you want to remove and it will be put back on the riverside.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I move the boat?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Just press the button at the bottom middle of your screen and the boat will move to the other side as long there are at least two animals on the boat.";
    }
    public void Question6()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How many times can the boat cross the river?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "The boat can cross the river a maximum of 13 times.";
    }
    public void Question7()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How many animals can I put on the boat?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "You can put a maximum of three animals on the boat. But remember that the boat will not move unless there are at least two animals on the boat.";
    }
    public void Question8()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the puzzle?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Exit the question overview and go back to the puzzle. On the top right side of your screen you will find three buttons. The lowest button is the exit button with which you can leave the puzzle.";
    }
    public void Question9()
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
