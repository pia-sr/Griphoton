using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public GameObject questions;
    public GameObject settings;
    public GameObject options;
    public Text message;
    public GridField grid;

    private void Awake()
    {
        string path = Application.persistentDataPath + "/gameData.game";
        if (File.Exists(path))
        {
            data.loadGame();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //data.namePlayer = null;
        if (data.namePlayer == null || data.namePlayer.Length == 0)
        {
            running = false;
            counter = 0;
            string firstSentence = "Hello,| \nWho might you be?";
            StartCoroutine(WordbyWord(firstSentence));
            data.activeLevel = 1;
            data.tutorial = true;

        }
        else
        {
            running = false;
            string firstSentence = "Welcome back, "+ data.namePlayer + "!";
            StartCoroutine(WordbyWord(firstSentence));
            int counterNode = 0;
            foreach (Node node in grid.grid)
            {
                node.setItemOnTop(data.nodeTags[counterNode]);
                counterNode++;
            };

        }
        //else enable script and gameobjects;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && !running && EventSystem.current != GameObject.Find("Skip"))
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
            if (data.namePlayer.Length > 0 && counter < 2)
            {
                griphoton.SetActive(true);
                options.SetActive(true);
                player.SetActive(true);
                player.GetComponent<Player>().pause();
                player.GetComponent<Player>().unpause();
                running = true;
                start = false;
                this.transform.parent.transform.parent.gameObject.SetActive(false);
            }
        }
        else if (start)
        {
            start = false;
            switch (counter)
            {
                case 1:
                    TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
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
                    sentence = "What is so magical about it, you ask?| \nYou see, this world is a place where souls go who still have unsolved puzzles from their previous life.| These souls can strengthen this sword when you help them solve their puzzle.";
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
                    options.SetActive(true);
                    sentence = "If you have any questions, just tap on the icon with the question mark.| \nYou can also access the setting or leave the game by tapping the setting icon.| \nGood luck, " + playerName + "!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 12:
                    player.SetActive(true);
                    player.GetComponent<Player>().pause();
                    player.GetComponent<Player>().unpause();
                    this.transform.parent.transform.parent.gameObject.SetActive(false);
                    running = true;
                    start = false;
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
        ++counter;
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
        options.SetActive(true);
        griphoton.SetActive(true);
        this.transform.parent.transform.parent.gameObject.SetActive(false);
        player.SetActive(true);
        player.GetComponent<Player>().pause();
        player.GetComponent<Player>().unpause();
        running = true;
        start = false;

    }

    public void Question1()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How do I walk again?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the screen and you will walk over to the place you tapped on.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "What am I supposed to do?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "You can either go to a house to solve a puzzle, or you can go to the dungeon to fight the monsters. You could also just explore Griphoton a bit. ";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "Where do I find the dungeon?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "The entrance to the dungeon is in Spencer's house. Just find the house with Spencer's name on it.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "Where do I find the puzzles?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Every house except Spencer's has a ghost with a puzzle in it. Just tap on a house and you can try to solve it. The owner of the house will explain the rules of the puzzle.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the game?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "You will find an exit button in the settings to leave the game.";
    }
    public void Question6()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "Where can I find the settings?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "When you close the question overview, by tapping on the X in the right corner, you go back to the game. There you will see on the right of your screen the settings icon. Just tap on it to access the settings.";
    }

    public void CloseHelp()
    {
        options.SetActive(true);
        player.GetComponent<Player>().unpause();
        questions.SetActive(false);
    }
    public void CloseSettings()
    {
        options.SetActive(true);
        player.GetComponent<Player>().unpause();
        settings.SetActive(false);
    }
    public void closeQuestion()
    {
        questions.transform.GetChild(1).gameObject.SetActive(true);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
    }
    public void Help()
    {
        options.SetActive(false);
        player.GetComponent<Player>().pause();
        questions.SetActive(true);
    }

    public void Settings()
    {
        options.SetActive(false);
        player.GetComponent<Player>().pause();
        settings.SetActive(true);
    }

    public void Exit()
    {
        message.transform.parent.transform.parent.gameObject.SetActive(true);
        message.text = "Do you really want to leave the game? \nYour progress will be safed!";

    }

    public void Reset()
    {
        message.transform.parent.transform.parent.gameObject.SetActive(true);
        message.text = "Do you really want to reset the game? All your progress will be lost!";
    }

    public void Yes()
    {
        if(message.text == "Do you really want to leave the game? \nYour progress will be safed!")
        {
            data.SaveGame();
            Application.Quit();

        }
        else
        {
            data.tutorial = true;
            data.SaveGame();
            Application.Quit();

        }
    }

    public void No()
    {
        message.transform.parent.transform.parent.gameObject.SetActive(false);
    }

    public void sound()
    {
        if (data.sound)
        {
            data.sound = false;
        }
        else
        {
            data.sound = true;
        }
    }
}
