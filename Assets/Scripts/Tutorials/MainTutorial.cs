using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainTutorial : MonoBehaviour
{
    public Text mainDialog;
    public Text inputText;
    public Game data;
    private int counter;
    public GameObject namePanel;
    public GameObject player;
    public GameObject griphoton;
    public GameObject skipButton;
    public GameObject touchAni;
    public GameObject movement;
    private bool running;
    private bool start;
    private string playerName;

    // Start is called before the first frame update
    void Start()
    {
        //If tutorial is true
        data.tutorial = true;
        running = false;
        counter = 0;
        string firstSentence = "Hello,| \nWho might you be?";
        StartCoroutine(WordbyWord(firstSentence));
        //else enable script and gameobjects;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && !running && EventSystem.current != GameObject.Find("Skip"))
        {

            touchAni.GetComponent<TouchAnimation>().running = false;
            if(touchAni.transform.childCount > 1)
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
                    namePanel.SetActive(true);
                    this.gameObject.SetActive(false);
                    mainDialog.text = "";
                    break;
                case 2:
                    skipButton.SetActive(true);
                    string sentence = "My name is Spencer.| I do not know how you ended up here, but welcome to Griphoton, " + playerName + "!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "This world is not ready for you, yet.| There are still things you need to do in your world.| But don't worry, there is a way for you to go back.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "My house has a portal that leads to your world.| However, this portal is at the end of a dungeon full of monsters.| You will have to fight them to reach it.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "Hmm, it looks like you are not armed.| You will not come far in the dungeon without a sword.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    sentence = "Lucky for you, I have a spare.| It is even magical!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    sentence = "What is so magical about them, you ask?| \nYou see, this world is a place where souls go who still have unsolved puzzles from their previous life.| These souls can strengthen this sword when you help them solve their puzzle.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
                    sentence = "Every soul has their own house here in Griphoton.| \nJust drop by their houses, and they will explain their puzzle.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 9:
                    movement.SetActive(true);
                    sentence = "Let me show you how you walk in this world.| \nSimply tap on the place where you want to go and that's it!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 10:
                    movement.SetActive(false);
                    griphoton.SetActive(true);
                    sentence = "This here is my house.| It even has my name on it!| \nIf you tap on my house, you can enter the dungeon.| I will explain more about the dungeon once you drop by.| But for now, I will let you explore Griphoton a bit. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 11:
                    sentence = "If you have any questions, just tap on the icon with the question mark.| \nYou can also access the setting or leave the game by tapping this icon.| \nGood luck, " + playerName + "!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 12:
                    player.SetActive(true);
                    this.transform.parent.transform.parent.gameObject.SetActive(false);
                    this.enabled = false;
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

    public void readInput(string s)
    {
        playerName = s;
    }
    public void enter()
    {
        if(name.Length != 0)
        {
            griphoton.GetComponent<Upperworld>().playerName = playerName;
            namePanel.SetActive(false);
            this.gameObject.SetActive(true);
            counter++;
            start = true;
        }
        
    }
    public void skipTutorial()
    {
        griphoton.SetActive(true);
        this.transform.parent.transform.parent.gameObject.SetActive(false);
        player.SetActive(true);
        this.enabled = false;
    }
}
