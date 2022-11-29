/*
 * DungeonTutorial.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonTutorial : MonoBehaviour
{
    //UI
    public Text mainDialog;
    public GameObject skipButton;
    public GameObject touchAni;
    public GameObject lexicon;
    public GameObject questions;
    public GameObject options;
    public GameObject message;
    public GameObject ghost;

    //sounds
    public AudioSource typewriter;


    //public variables to communicate with other scripts
    public GameObject player;
    public GameObject dungeon;

    //private variables
    private bool _running;
    private bool _start;
    private int _clickIndex;
    private int _counter;
    private Game _data;

    //on awake it will look for the game data and load the game
    private void Awake()
    {
        _data = GameObject.Find("GameData").GetComponent<Game>();
        _data.loadGame();
    }

    // Start is called before the first frame update
    void Start()
    {

        //If the user is for the first time in the dungeon, it will go through the dungeon
        if (_data.tutorial)
        {
            ghost.SetActive(true);
            ghost.transform.position = Vector3.up;
            _data.tutorial = false;
            _running = false;
            _counter = 0;
            string firstSentence = "You came, " + _data.namePlayer + "!";
            StartCoroutine(WordbyWord(firstSentence));

        }
        //if not no tutorial will start
        else
        {
            options.SetActive(true);
            this.transform.parent.parent.gameObject.SetActive(false);

            player.SetActive(true);
            player.GetComponent<Player>().PlayerInvisiable();
            player.GetComponent<Player>().PlayerVisiable();
            player.GetComponent<Player>().Pause();
            player.GetComponent<Player>().Unpause();
            _running = true;
            _start = false;
            dungeon.SetActive(true);
            this.transform.parent.parent.gameObject.SetActive(false);

        }
        //if the sound is supposed to be off, the sound will be muted
        if (!_data.sound)
        {
            muteSound();
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        }


        //Text sequences
        else if (_start)
        {
            _start = false;
            switch (_counter)
            {
                case 1:
                    ghost.SetActive(false);
                    dungeon.SetActive(true);
                    skipButton.SetActive(true);
                    player.SetActive(true);
                    player.GetComponent<Player>().Pause();
                    player.GetComponent<Player>().PlayerInvisiable();
                    string sentence = "This here is the first room of the dungeon.| To go to the next room, you first need to defeat all the monsters in the room.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "Once you have defeated all the monsters, the door over there will open, and you do not have to fight them again.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "To attack the monsters with your sword, you need to get close to them and then press the sword in the right corner of your screen.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "You can also try to block their attacks by pressing the shield in the left corner.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    sentence = "You see here a Guardian of Answers.| They are the most harmless monster down here.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    options.SetActive(true);
                    sentence = "If you want to know more about the monsters in the dungeon or if you have any questions, just press the button with the question mark.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    sentence = "Sometimes after you defeated a monster, they will drop a key.| These keys are hint-keys.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 8:
                    sentence = "They can unlock hint cards to help you solve puzzles.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 9:
                    sentence = "You can leave the dungeon at any time by pressing the exit button.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 10:
                    sentence = "Oh, before I forget: Beware of the spikes!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 11:
                    player.GetComponent<Player>().PlayerVisiable();
                    player.GetComponent<Player>().Unpause();
                    this.transform.parent.parent.gameObject.SetActive(false);
                    _running = true;
                    _start = false;
                    _data.tutorial = false;
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

    //Function to skip the tutorial
    public void skipTutorial()
    {
        options.SetActive(true);
        StopAllCoroutines();
        this.transform.parent.parent.gameObject.SetActive(false);
        player.GetComponent<Player>().PlayerVisiable();
        player.GetComponent<Player>().Pause();
        player.GetComponent<Player>().Unpause();
        _running = true;
        _start = false;

    }

    //Questions and Answers for the help section
    public void Question1()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How do I walk again?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Just tap on the screen and you will walk over to the place you tapped on.";
    }
    public void Question2()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "What am I supposed to do?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You need to fight the monsters to reach the end of the dungeon with the portal that will take you back to your world.";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I get stronger?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Your sword can get stronger by solving the puzzles of Griphoton's residents. You can leave the dungeon at any time to solve a puzzle.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How do I fight again?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You need to get close to the monster you want to fight and then press the sword in the right corner of your screen to attack them. The sword only works if you are standing next to a monster.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Can I block the monster's attack?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Yes, you can try blocking them by pressing the shield in the left corner of the screen. The shield only works if you are standing next to a monster.";
    }
    public void Question6()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How do I leave the dungeon?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "When you close the question overview, by tapping on the X in the right corner, you will go back to the game. There you will see on the right of your screen the exit icon. Just tap on it to leave the dungeon.";
    }
    public void Question9()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "What are the keys that some monsters drop after being defeated?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "Those keys are hint keys. With them, you can unlock hint cards in the houses in Griphoton if you should ever get stuck on a puzzle.";
    }
    public void Question7()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the game?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "You will find a button in the settings when you are back in Griphoton to leave the game.";
    }
    public void Question8()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        question.text = "Where can I find the settings?";
        Text answer = questions.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        answer.text = "When you close the question overview, by tapping on the X in the right corner, you will go back to the game. There you will see on the right of your screen the home icon. When you click that you are back in griphoton and can access the setting by pressing the settings button in the right corner of your screen.";
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
            options.SetActive(true);
            Time.timeScale = 1;
            player.GetComponent<Player>().Unpause();
            questions.SetActive(false);
        }

    }

    //Function to open the question overview
    public void Help()
    {
        options.SetActive(false);
        player.GetComponent<Player>().Pause();
        Time.timeScale = 0;
        questions.SetActive(true);
    }


    //Function for the exit button
    //Open up a panel to check if the user really wants to leave
    public void Exit()
    {
        message.SetActive(true);

    }

    //Function for the yes button
    //The game will be saved and the app will be closed
    public void Yes()
    {
        Time.timeScale = 1;
        _data.SaveGame();
        SceneManager.LoadScene("Upperworld");

    }

    //Function to open the monster overview
    public void OpenLexicon()
    {
        questions.SetActive(false);
        _clickIndex = 0;
        lexicon.SetActive(true);
    }

    //Function to close the monster overview
    public void CloseLexicon()
    {
        for (int i = 1; i < lexicon.transform.GetChild(0).transform.GetChild(0).transform.childCount; i++)
        {
            lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
        }
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        lexicon.SetActive(false);
        questions.SetActive(true);
    }

    //Function for the no button
    public void No()
    {
        message.SetActive(false);
    }

    //Function of the next button in the monster overview to go to the next monster
    public void next()
    {
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(_clickIndex).gameObject.SetActive(false);
        _clickIndex++;
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(_clickIndex).gameObject.SetActive(true);
    }

    //Function of the previous button in the monster overview to go to the previous monster
    public void previous()
    {
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(_clickIndex).gameObject.SetActive(false);
        _clickIndex--;
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(_clickIndex).gameObject.SetActive(true);
    }

    //Function to mute all the sounds
    private void muteSound()
    {
        GameObject sound = GameObject.Find("Sounds");
        for (int i = 0; i < sound.transform.childCount; i++)
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
}
