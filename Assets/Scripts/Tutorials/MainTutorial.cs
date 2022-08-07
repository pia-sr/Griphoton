using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
            data.hintKeys = 0;

        }
        //if the user has already opened the app at least once
        else
        {
            _running = false;
            string firstSentence = "Welcome back, "+ data.namePlayer + "!";
            StartCoroutine(WordbyWord(firstSentence));
            int counterNode = 0;
            foreach (Node node in grid.grid)
            {
                node.SetItemOnTop(data.nodeTags[counterNode]);
                node.mapTag = data.mapTags[counterNode];
                node.status = data.mapStatus[counterNode];
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
                Ghost.SetActive(false);
                player.GetComponent<Player>().Unpause();
                this.transform.parent.parent.gameObject.SetActive(false);
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
                    sentence = "This world is not ready for you, yet.| There are still things you need to do in your world.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "But don't worry, there is a way for you to go back. | My house has a portal that leads to your world.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "However, this portal is at the end of a dungeon full of monsters.| You will have to fight them to reach it.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    sentence = "Hmm, it looks like you are not armed.| You won't come far in the dungeon without a sword.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    sentence = "Lucky for you, I have a spare.| It is even magical!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
                    sentence = "What is so magical about it, you ask?";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 9:
                    sentence = "You see, this world is a place where souls go who still have unsolved puzzles from their previous life.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 10:
                    sentence = "These souls can strengthen this sword when you help them solve their puzzle.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 11:
                    sentence = "Every soul has their own house here in Griphoton.| Just drop by their houses, and they will explain their puzzle.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 12:
                    Ghost.SetActive(false);
                    movement.SetActive(true);
                    sentence = "Let me show you how you walk in this world.| Simply tap on the place where you want to go and that's it!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 13:
                    sentence = "You can also hold down your touch and you will walk continuosly towards one direction.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 14:
                    movement.SetActive(false);
                    griphoton.SetActive(true);
                    sentence = "This here is my house.| You can recognise it by its black roof.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 15:
                    sentence = "You can always return to my house if you press on 'Dungeon' in the settings.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 16:
                    sentence = "If you tap on my house, you can enter the dungeon.| I will explain more about the dungeon once you drop by.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 17:
                    sentence = "But for now, I will let you explore Griphoton a bit. ";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 18:
                    options.SetActive(true);
                    sentence = "If you tap on the menu icon in the top right corner of your screen, three new icons will appear.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 19:
                    Menu();
                    sentence = "One icon is the settings icon through which you access the settings.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 20:
                    sentence = "There you can turn off the music, access a Q&A to answer all your questions and more.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 21:
                    sentence = "The other icon is the compass icon through which a map of Griphoton will appear.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 22:
                    sentence = "It is a magical map and currently empty except for the dungeon.| Once you have visited a house, it will appear on the map.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 23:
                    sentence = "You can leave the game by pressing the button with the house icon underneath the settings button.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 24:
                    Menu();
                    sentence = "Good luck, " + _playerName + "!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 25:
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
        if(_playerName.Length > 0)
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
        answer.text = "You can either go to a house to solve a puzzle, or you can go to the dungeon to fight the monsters. You could also just explore Griphoton for a bit.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Where do I find the dungeon?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "The entrance to the dungeon is in Spencer's house. Just find the house with the black roof or simply press on 'Dungeon' in the settings and you will be teleported back to Spencer's house.";
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
        answer.text = "Tap on the menu button on the top right corner of your screen and three new buttons will appear. You can leave the game by pressing the button underneath the settings button in the right corner of your screen. You recognise the button by the house icon on top of it.";
    }
    public void Question6()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Where can I find the settings?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "If you close the question overview by tapping on the X in the right corner, you will go back to the settings.";
    }
    public void Question7()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I access the map of Griphoton?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "One of the three buttons underneath the menu button in Griphoton is a button with a compass on it. If tap on the button, a map will open. The names of the houses will only appear on the map once you have visited the house.";
    }
    public void Question8()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Why are there no names on the map?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "The names of the houses in Griphoton will only appear on the map once you visit them. If you solve a puzzle, the name will still be on the map but crossed out.";
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
        
        options.SetActive(false);
        player.GetComponent<Player>().Pause();
        map.SetActive(true);
        GameObject.Find("Map").GetComponent<Map>().update = true;
        player.GetComponent<Player>().DoNotShowOptions = true;
    }

    public void CloseMap()
    {

        options.SetActive(true);
        map.SetActive(false);
        player.GetComponent<Player>().Unpause();
        player.GetComponent<Player>().DoNotShowOptions = false;
    }

}
