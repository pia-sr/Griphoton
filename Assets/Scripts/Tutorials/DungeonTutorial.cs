using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonTutorial : MonoBehaviour
{
    public Text mainDialog;
    private Game data;
    private int counter;
    public GameObject player;
    public GameObject dungeon;
    public GameObject skipButton;
    public GameObject touchAni;
    private bool running;
    private bool start;
    public GameObject lexicon;
    public GameObject questions;
    public GameObject options;
    public GameObject message;
    private int clickIndex;
    public GameObject ghost;

    private void Awake()
    {
        data = GameObject.Find("GameData").GetComponent<Game>();
        data.loadGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (data.tutorial)
        {
            ghost.SetActive(true);
            data.tutorial = false;
            running = false;
            counter = 0;
            string firstSentence = "You came, " + data.namePlayer + "!";
            StartCoroutine(WordbyWord(firstSentence));

        }
        else
        {
            options.SetActive(true);
            this.transform.parent.transform.parent.gameObject.SetActive(false);
            player.SetActive(true);
            player.GetComponent<Player>().pause();
            player.GetComponent<Player>().unpause();
            running = true;
            start = false;
            dungeon.SetActive(true);
            this.transform.parent.transform.parent.gameObject.SetActive(false);

        }
        //else enable script and gameobjects;
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
        else if (start)
        {
            start = false;
            switch (counter)
            {
                case 1:
                    ghost.SetActive(false);
                    skipButton.SetActive(true);
                    dungeon.SetActive(true);
                    string sentence = "This here is the first room of the dungeon.| To go to the next room, you first need to defeat all the monsters in the room.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 2:
                    sentence = "Once you have defeated all the monsters, the door over there will open, and you do not have to fight them again.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 3:
                    sentence = "To attack the monsters with your sword, you need to get close to them and then press the button in the right corner of your screen.| You can also try to block their attacks by pressing the button in the left corner.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 4:
                    sentence = "You see here a Guardian of Answers.| They are the most harmless monster down here.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 5:
                    options.SetActive(true);
                    sentence = "If you want to know more about the monsters in the dungeon or if you have any questions, just press the button with the question mark.| You can leave the dungeon at any time by pressing the exit button.";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 6:
                    sentence = "Oh, before I forget: Beware of the spikes!";
                    StartCoroutine(WordbyWord(sentence));
                    break;
                case 7:
                    player.SetActive(true);
                    player.GetComponent<Player>().pause();
                    player.GetComponent<Player>().unpause();
                    this.transform.parent.transform.parent.gameObject.SetActive(false);
                    running = true;
                    start = false;
                    data.tutorial = false;
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
                mainDialog.text += " " + words[j];
            }
            yield return new WaitForSeconds(0.4f);
        }
        ++counter;
        running = false;
        touchAni.SetActive(true);
    }
    public void skipTutorial()
    {
        options.SetActive(true);
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
        answer.text = "You need to fight the monsters to reach the end of the dungeon with the portal back to your world. ";
    }
    public void Question3()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I get stronger?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Your sword can get stronger by solving the puzzles of Griphoton's residents. you can leave the dungeon at any time to solve a puzzle.";
    }
    public void Question4()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How do I fight again?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "You need to get close to the monster you want to fight and then press the button in the right corner of your screen to attack them. The button only works if you are standing next to a monster.";
    }
    public void Question5()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "Can I block the monster's attack?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "Yes, you can try blocking them by pressing the button in the left corner of the screen. The button only works if you are standing next to a monster.";
    }
    public void Question6()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How do I leave the dungeon?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "When you close the question overview, by tapping on the X in the right corner, you will go back to the game. There you will see on the right of your screen the exit icon. Just tap on it to leave the dungeon.";
    }
    public void Question7()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "How can I leave the game?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "You will find a button in the settings when you are back in Griphoton to leave the game.";
    }
    public void Question8()
    {
        questions.transform.GetChild(1).gameObject.SetActive(false);
        questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        Text question = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        question.text = "Where can I find the settings?";
        Text answer = questions.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        answer.text = "When you close the question overview, by tapping on the X in the right corner, you will go back to the game. There you will see on the right of your screen the home icon. When you click that you are back in griphoton and can access the setting by pressing the settings button in the right corner of your screen.";
    }

    public void CloseHelp()
    {
        if (questions.transform.GetChild(0).transform.GetChild(0).gameObject.activeSelf)
        {
            questions.transform.GetChild(1).gameObject.SetActive(true);
            questions.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            options.SetActive(true);
            Time.timeScale = 1;
            player.GetComponent<Player>().unpause();
            questions.SetActive(false);
        }

    }
    public void Help()
    {
        options.SetActive(false);
        player.GetComponent<Player>().pause();
        Time.timeScale = 0;
        questions.SetActive(true);
    }


    public void Exit()
    {
        message.SetActive(true);

    }


    public void Yes()
    {
        Time.timeScale = 1;
        data.yPos = 150;
        data.xPos = 150;
        data.SaveGame();
        SceneManager.LoadScene("Upperworld");

    }
    public void OpenLexicon()
    {
        clickIndex = 0;
        lexicon.SetActive(true);
    }

    public void CloseLexicon()
    {
       for(int i = 1; i < lexicon.transform.GetChild(0).transform.GetChild(0).transform.childCount; i++)
        {
            lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
        }
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        lexicon.SetActive(false);
    }

    public void No()
    {
        message.SetActive(false);
    }
    public void next()
    {
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(clickIndex).gameObject.SetActive(false);
        clickIndex++;
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(clickIndex).gameObject.SetActive(true);
    }
    public void previous()
    {
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(clickIndex).gameObject.SetActive(false);
        clickIndex--;
        lexicon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(clickIndex).gameObject.SetActive(true);
    }

}
