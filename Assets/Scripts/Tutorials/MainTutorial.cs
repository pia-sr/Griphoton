using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainTutorial : MonoBehaviour
{
    //UI
    public Text mainDialog;
    public Text inputText;
    public GameObject credits;
    public GameObject namePanel;
    public GameObject skipButton;
    public GameObject touchAni;
    public GameObject movement;
    public GameObject questions;
    public GameObject settings;
    public GameObject options;
    public GameObject map;
    public Text message;
    public GameObject soundButton;
    public Sprite soundOn;
    public Sprite soundOff;
    public GameObject Ghost;
    
    //sounds
    public AudioSource typewriter;

    //public variables to communicate with other scripts
    public GameObject player;
    public GameObject griphoton;
    public GridField grid;
    public Game data;

    //private variables for the tutorial flow
    private bool _running;
    private bool _start;
    private int _counter;
    private string _playerName;


    private void Awake()
    {
        //If a file already exists, the file is loaded
        string path = Application.persistentDataPath + "/gameData.game";
        if (File.Exists(path))
        {
            data.loadGame();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Ghost.SetActive(true);
        Ghost.transform.localPosition = Vector3.up;
        //if the user opens the app for the first time
        if (data.namePlayer == null || data.namePlayer.Length == 0)
        {
            data.sound = true;
            data.strenghtMultiplier = 0;
            _running = false;
            _counter = 0;
            string firstSentence = "Hello,| \nWho might you be?";
            StartCoroutine(WordbyWord(firstSentence));
            data.activeLevel = 1;
            data.tutorial = true;

        }
        //if the user has already opened the app at least once
        else
        {
            data.tutorial = false;
            _running = false;
            string firstSentence = "Welcome back, "+ data.namePlayer + "!";
            StartCoroutine(WordbyWord(firstSentence));
            int counterNode = 0;
            foreach (Node node in grid.grid)
            {
                node.SetItemOnTop(data.nodeTags[counterNode]);
                node.mapTag = data.mapTags[counterNode];
                counterNode++;
            } 
            if (data.sound)
            {
                soundButton.GetComponent<Image>().sprite = soundOn;
            }
            else
            {
                soundButton.GetComponent<Image>().sprite = soundOff;
                muteSound();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        //Code waits for the user's touch to go to the next text bit
        if (Input.touchCount > 0 && !_running && EventSystem.current != GameObject.Find("Skip"))
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
            _start = true;
            //if it is not the first time, the app does not start with the tutorial
            if (data.namePlayer.Length > 0 && _counter < 2)
            {
                griphoton.SetActive(true);
                options.SetActive(true);
                player.SetActive(true);
                player.GetComponent<Player>().SwitchCams();
                player.GetComponent<Player>().Pause();
                _running = true;
                _start = false;
                player.GetComponent<Player>().Unpause();
                this.transform.parent.parent.gameObject.SetActive(false);
                Ghost.SetActive(false);
            }
        }

        //Text sequences
        else if (_start)
        {
            _start = false;
            switch (_counter)
            {
                case 1:
                    Ghost.SetActive(false);
                    namePanel.SetActive(true);
                    this.gameObject.SetActive(false);
                    mainDialog.text = "";
                    break;
                case 2:
                    Ghost.SetActive(true);
                    skipButton.SetActive(true);
                    string sentence = "My name is Spencer.| I do not know how you ended up here, but welcome to Griphoton, " + _playerName + "!";
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
                    sentence = "What is so magical about it, you ask?";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
                    sentence = "You see, this world is a place where souls go who still have unsolved puzzles from their previous life.| These souls can strengthen this sword when you help them solve their puzzle.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 9:
                    sentence = "Every soul has their own house here in Griphoton.| \nJust drop by their houses, and they will explain their puzzle.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 10:
                    Ghost.SetActive(false);
                    movement.SetActive(true);
                    sentence = "Let me show you how you walk in this world.| \nSimply tap on the place where you want to go and that's it!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 11:
                    sentence = "You can also hold down your touch and you will walk continuosly into one direction.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 12:
                    movement.SetActive(false);
                    griphoton.SetActive(true);
                    sentence = "This here is my house.| You can recognise it by its black roof.| You can always return to my house if you press on 'Dungeon' in the settings.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 13:
                    movement.SetActive(false);
                    griphoton.SetActive(true);
                    sentence = "If you tap on my house, you can enter the dungeon.| I will explain more about the dungeon once you drop by.| But for now, I will let you explore Griphoton a bit. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 14:
                    options.SetActive(true);
                    sentence = "If you have any questions, there is a help button in the settings| \nYou can also access the setting by tapping the setting icon.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 15:
                    options.SetActive(true);
                    sentence = "You can leave the game by pressing the button with the house icon underneath the settings button.| \nGood luck, " + _playerName + "!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 16:
                    player.SetActive(true);
                    player.GetComponent<Player>().SwitchCams();
                    player.GetComponent<Player>().Pause();
                    player.GetComponent<Player>().Unpause();
                    this.transform.parent.parent.gameObject.SetActive(false);
                    _running = true;
                    _start = false;
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
        ++_counter;
        _running = false;
        touchAni.SetActive(true);
    }

    //Function for the input field
    //source: https://www.youtube.com/watch?v=guelZvubWFY&t=151s
    public void readInput(string s)
    {
        _playerName = s;
    }
    public void enter()
    {
        if(name.Length != 0)
        {
            griphoton.GetComponent<Upperworld>().playerName = _playerName;
            namePanel.SetActive(false);
            this.gameObject.SetActive(true);
            _counter++;
            _start = true;
        }
        
    }

    //Function to skip the tutorial
    public void skipTutorial()
    {
        options.SetActive(true);
        griphoton.SetActive(true);
        this.transform.parent.parent.gameObject.SetActive(false);
        player.SetActive(true);
        player.GetComponent<Player>().Pause();
        player.GetComponent<Player>().Unpause();
        _running = true;
        _start = false;
        player.GetComponent<Player>().SwitchCams();
        Ghost.SetActive(false);

    }


    //Questions and Answers for the help section
    public void Question1()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How do I walk again?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the screen and you will walk over to the place you tapped on. You can also hold your touch to have the player walk contuiously towards a direction.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "What am I supposed to do?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You can either go to a house to solve a puzzle, or you can go to the dungeon to fight the monsters. You could also just explore Griphoton a bit.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Where do I find the dungeon?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "The entrance to the dungeon is in Spencer's house. Just find the house with the black roof or simply press on dungeon in the settings and you will be teleported back to Spencer's house.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Where do I find the puzzles?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Every house except Spencer's has a ghost with a puzzle in it. Just tap on a house and you can try to solve it. The owner of the house will explain the rules of the puzzle.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the game?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You can leave the game by pressing the button underneath the settings button in the right corner of your screen. You recognise the button by the house icon on top of it.";
    }
    public void Question6()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Where can I find the settings?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "When you close the question overview, by tapping on the X in the right corner, you go back to the game. There you will see on the right of your screen the settings icon. Just tap on it to access the settings.";
    }


    //close function for either closing a specific question or the question overview
    public void CloseHelp()
    {
        if (questions.transform.GetChild(0).GetChild(0).gameObject.activeSelf)
        {
            questions.transform.GetChild(1).gameObject.SetActive(true);
            questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            settings.SetActive(true);
            questions.SetActive(false);
        }

    }

    //Function to close the settings
    public void CloseSettings()
    {
        options.SetActive(true);
        player.GetComponent<Player>().Unpause();
        settings.SetActive(false);
        player.GetComponent<Player>().DoNotShowOptions = false;
    }

    //Function to open the question overview
    public void Help()
    {
        settings.SetActive(false);
        questions.SetActive(true);
    }

    //Function to open the settings
    public void Settings()
    {
        options.SetActive(false);
        player.GetComponent<Player>().Pause();
        settings.SetActive(true);
        player.GetComponent<Player>().DoNotShowOptions = true;
    }


    //Function for the exit button
    //Open up a panel to check if the user really wants to leave
    public void Exit()
    {

        player.GetComponent<Player>().Pause();
        message.transform.parent.transform.parent.gameObject.SetActive(true);
        message.text = "Do you really want to leave the game? \nYour progress will be safed!";

    }

    //Function for the reset button
    public void Reset()
    {
        settings.SetActive(false);
        message.transform.parent.transform.parent.gameObject.SetActive(true);
        message.text = "Do you really want to reset the game? All your progress will be lost!";
    }

    //Function for the yes button
    //Depending on the question, the player can either reset or leave the game
    public void Yes()
    {
        if(message.text == "Do you really want to leave the game? \nYour progress will be safed!")
        {

            data.xPos = grid.GetNodeFromWorldPos(player.transform.position).gridX;
            data.yPos = grid.GetNodeFromWorldPos(player.transform.position).gridY;
            data.SaveGame();
            Application.Quit();

        }
        else
        {
            string path = Application.persistentDataPath + "/gameData.game";
            File.Delete(path);
            Application.Quit();

        }
    }

    //Function for the no button
    public void No()
    {
        if (message.text != "Do you really want to leave the game? \nYour progress will be safed!")
        {
            settings.SetActive(true);

        }
        else
        {
            player.GetComponent<Player>().Unpause();
        }
        message.transform.parent.transform.parent.gameObject.SetActive(false);
    }

    //Function for the sound button
    //It is saved if the user likes to have the sound enabled or not
    public void sound()
    {
        if (data.sound)
        {
            data.sound = false;
            soundButton.GetComponent<Image>().sprite = soundOff;
            muteSound();
        }
        else
        {
            data.sound = true;
            soundButton.GetComponent<Image>().sprite = soundOn;
            muteSound();
        }
    }

    //Function to mute all the sounds
    private void muteSound()
    {
        GameObject sound = GameObject.Find("Sounds");
        for(int i = 0; i < sound.transform.childCount; i++)
        {
            var audiosource = sound.transform.GetChild(i).gameObject.GetComponent<AudioSource>();
            if (audiosource.mute)
            {
                audiosource.mute = false;
            }
            else
            {
                audiosource.mute = true;
            }
        }
    }

    //Function for the dungeon button
    //If pressed brings the player right in front the house with the dungeon entrance
    public void Dungeon()
    {

        Node dungeon = grid.TagToNode("Dungeon");
        player.transform.position = grid.grid[dungeon.gridX, dungeon.gridY - 2].worldPosition;
    }

    //Function to close the credits
    public void CloseCredits()
    {

        settings.SetActive(true);
        credits.SetActive(false);
    }

    //Function to open the credits
    public void Credits()
    {
        settings.SetActive(false);
        credits.SetActive(true);
    }

    public void Menu()
    {
        bool active = true;
        if (options.transform.GetChild(1).gameObject.activeSelf)
        {
            active = false;
        }
        for(int i = 1; i < options.transform.childCount; i++)
        {
            options.transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    public void OpenMap()
    {

        data.xPos = grid.GetNodeFromWorldPos(player.transform.position).gridX;
        data.yPos = grid.GetNodeFromWorldPos(player.transform.position).gridY;
        GameObject.Find("Map").GetComponent<Map>().update = true;
        options.SetActive(false);
        player.GetComponent<Player>().Pause();
        map.SetActive(true);
        player.GetComponent<Player>().DoNotShowOptions = true;
    }

    public void CloseMap()
    {

        options.SetActive(true);
        player.GetComponent<Player>().Unpause();
        map.SetActive(false);
        player.GetComponent<Player>().DoNotShowOptions = false;
    }

}
