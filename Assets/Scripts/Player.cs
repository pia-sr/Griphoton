using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //public variables
    public GridField grid;
    public GameObject options;
    public GameObject puzzles;
    public GameObject griphoton;
    public Pathfinder pathFinder;
    public AudioSource griphotonSound;
    public AudioSource dungeonSound;
    public AudioSource puzzleSound;
    public bool activateTutorial;
    public bool lost = false;
    public bool upperWorld;
    public int hitValue;
    public bool enemyHit;
    public bool blockEnemy = false;
    public bool leaveLevel;
    public float speed;
    public GameObject messageSimple;
    public Text message2Options;
    public Animator animator;
    public HealthBar healthBar;
    public GameObject playerCam;
    public GameObject centerCam;
    public bool DoNotShowOptions;

    //private variables
    private Node _existingTarget;
    private List<Node> _path;
    private float _strength;
    private bool _blockBool;
    private bool heal;
    private Node _targetNode;
    private bool _foundPos;
    private bool _coroutineStart;
    private float _fullHealth;
    private Game _data;
    private bool _chooseExit;
    private bool _attackBool;
    private int _xInput;
    private int _yInput;

    //On awke the player looks for the game data
    private void Awake()
    {
        _data = GameObject.Find("GameData").GetComponent<Game>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("isWalking", false);
        enemyHit = false;
        hitValue = 25 + (15 * _data.strenghtMultiplier);
        _targetNode = null;
        _existingTarget = null;
        _strength = 100 + (100 * _data.strenghtMultiplier);
        _fullHealth = _strength;
        if (!upperWorld)
        {
            healthBar.SetHealthBarValue(1);
            _foundPos = false;
            dungeonSound.Play();
            _attackBool = false;
        }
        else
        {
            //griphotonSound.Play();
            transform.position = grid.grid[_data.xPos, _data.yPos].worldPosition;

            
        }
        _xInput = 0;
        _yInput = -1;
        Unpause();

        animator.SetFloat("XInput", _xInput);
        animator.SetFloat("YInput", _yInput);
        heal = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!upperWorld && !_foundPos)
        {
            _foundPos = true;
            EntranceLevel();
        }
        //The player looks downwards if they do not have a direction
        if(_xInput == 0 && _yInput == 0)
        {

            _xInput = 0;
            _yInput = -1;


            animator.SetFloat("XInput", _xInput);
            animator.SetFloat("YInput", _yInput);
        }

        //if the player is not active in Griphoton, the sound is turned off
        if (isInvisiable() && !this.gameObject.activeSelf && griphotonSound.isPlaying)
        {
            griphotonSound.Pause();
        }
        
        //The animation of walkíng stops if the player has no target
        if(_targetNode == null)
        {

            animator.SetBool("isWalking", false);
        }
        //the user can leave the game by pressing the exit button on their phone, the player will then save their progress
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(upperWorld)
            {
                _data.xPos = grid.GetNodeFromWorldPos(transform.position).gridX;
                _data.yPos = grid.GetNodeFromWorldPos(transform.position).gridY;
                
            }
            _data.SaveGame();
            Application.Quit();
        }

        //sets all things active that need to be active in Griphoton
        if(!isInvisiable()&& this.gameObject.activeSelf && upperWorld)
        {
            if (!options.activeSelf && !DoNotShowOptions)
            {
                options.SetActive(true);

            }
            if (!griphotonSound.isPlaying)
            {
                griphotonSound.Play();
            }
            if(puzzleSound.isPlaying && griphotonSound.isPlaying)
            {
                puzzleSound.Pause();
            }
        }
        else if (!upperWorld && heal)
        {
            heal = false;
            StartCoroutine(SelfHeal());
        }
        //After dungeon lost to find their position
        RestartLevel();
        if (_targetNode != null)
        {
            _path = pathFinder.FindPathPlayer(transform.position, _targetNode.worldPosition);
            if (_path.Count > 1)
            {
                if (_targetNode == _existingTarget)
                {
                    if (!_coroutineStart)
                    {
                        _coroutineStart = true;
                        StartCoroutine(Move());
                    }


                }
                else if (!_coroutineStart)
                {
                    _existingTarget = _targetNode;
                }

            }
            //if the player reached the portal, the game is over
            else if (_targetNode.onTop == "Portal")
            {
                Pause();
                int unsolvedPuzzles = 30 - _data.strenghtMultiplier;
                if (unsolvedPuzzles == 0)
                {
                    messageSimple.SetActive(true);
                }
                else
                {
                    message2Options.transform.parent.parent.gameObject.SetActive(true);
                    message2Options.text = "You still have " + unsolvedPuzzles + " unsolved Puzzles left. \n Do you want to stay and solve them or do you want to leave?";
                }
            }
            //if the player reached an entrance in the dungeon, they will walk to the previous level
            else if (grid.GetNodeFromWorldPos(transform.position).onTop == "Entrance" && _targetNode.onTop == "Entrance")
            {

                animator.SetBool("isWalking", false);
                PreviousLevel();
                _data.SaveGame();
            }
            //if the player reached an exit in the dungeon, they will walk into the next level
            else if (grid.GetNodeFromWorldPos(transform.position).onTop == "ExitOpen")
            {

                animator.SetBool("isWalking", false);
                NextLevel();
                _data.SaveGame();
            }
            //if the player walks into a house the specific house with its puzzle will be set as active
            else if (grid.ghostNames().Contains(_targetNode.onTop) || grid.ghostNames().Contains(_targetNode.owner))
            {
                griphotonSound.Pause();
                animator.SetBool("isWalking", false);
                activateTutorial = true;
                options.SetActive(false);
                string ghostName = grid.grid[_path[0].gridX, _path[0].gridY + 2].onTop;
                if(grid.grid[_path[0].gridX, _path[0].gridY].mapTag != ghostName)
                {
                    grid.grid[_path[0].gridX, _path[0].gridY].mapTag = ghostName;
                    GameObject.Find("Map").GetComponent<Map>().AddTag(_path[0].gridX, _path[0].gridY, ghostName);

                }
                GameObject ghostHouse = FindWithTag(ghostName);
                griphoton.SetActive(false);
                ghostHouse.SetActive(true);
                _targetNode = null;
                SetAllBoolsFalse();
                SwitchCams();
                Pause();
                _data.SaveGame();
                this.gameObject.SetActive(false);
            }
            //if the player enters the dungeon, all the data is saved and the dungeon loaded
            else if (_targetNode.onTop == "Dungeon" || _targetNode.owner == "Dungeon")
            {
                _data.xPos = grid.TagToNode("Dungeon").gridX;
                _data.yPos = grid.TagToNode("Dungeon").gridY - 2;
                animator.SetBool("isWalking", false);
                _data.SaveGame();
                SceneManager.LoadScene("Dungeon");
            }
            //if the player reached their target the walking animation stops
            else if (_targetNode == grid.GetNodeFromWorldPos(transform.position))
            {
                animator.SetBool("isWalking", false);
            }
        }
        //waits for user input to find target
        if (Input.touchCount > 0 && EventSystem.current.currentSelectedGameObject == null)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.current.ScreenToWorldPoint(Input.GetTouch(0).position);
            if(grid.Bounds().Contains(touchPosition))
            {
                _targetNode = grid.GetNodeFromWorldPos(touchPosition);
            }

            
        }

    }

    //Function to set all important bools as false
    private void SetAllBoolsFalse()
    {
        blockEnemy = false;
        _blockBool = false;
        _coroutineStart = false;
        _attackBool = false;
    }

    //Player entering a room by its entrance
    private void EntranceLevel()
    {
        List<Node> entraceNodes = new List<Node>();
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "Entrance")
            {
                entraceNodes.Add(node);
            }
        }
        if (entraceNodes.Count == 3)
        {
            transform.position = entraceNodes[1].worldPosition - new Vector3(0, 0, 1);
        }
    }

    //PLayer entering a room by its exit
    private void ExitLevel()
    {
        List<Node> entraceNodes = new List<Node>();
        foreach (Node node in grid.grid)
        {
            if (node.onTop == "ExitOpen")
            {
                entraceNodes.Add(node);
            }
        }
        if (entraceNodes.Count == 3)
        {
            transform.position = entraceNodes[1].worldPosition - new Vector3(0, 0, 1);
        }
    }

    //Player going to the previous dungeon level
    private void PreviousLevel()
    {
        int levelNr = -1;
        GameObject levels = GameObject.FindWithTag("Levels");
        for (int i = 1; i < levels.transform.childCount; i++)
        {
            if (levels.transform.GetChild(i).gameObject.activeSelf)
            {
                levelNr = i;
            }
        }
        if (levelNr != -1)
        {
            leaveLevel = true;
            SetAllBoolsFalse();
            Pause();
            levels.transform.GetChild(levelNr).gameObject.SetActive(false);
            levels.transform.GetChild(levelNr - 1).gameObject.SetActive(true);
            _chooseExit = true;
            StartCoroutine(Wait());
            Unpause();
        }
    }

    //Player going to the next level
    private void NextLevel()
    {
        int levelNr = -1;
        GameObject levels = GameObject.FindWithTag("Levels");
        for(int i = 0; i < levels.transform.childCount-1 ; i++)
        {
            if (levels.transform.GetChild(i).gameObject.activeSelf)
            {
                levelNr = i;
            }
        }
        if(levelNr != -1)
        {
            leaveLevel = true;
            SetAllBoolsFalse();
            Pause();
            levels.transform.GetChild(levelNr).gameObject.SetActive(false);
            levels.transform.GetChild(levelNr + 1).gameObject.SetActive(true);
            StartCoroutine(Wait());
            Unpause();
        }
    }
    
    //Function for the player to move
    //source: https://forum.unity.com/threads/transform-position-speed.744293/
    private IEnumerator Move()
    {

        
        if (_existingTarget == null)
        {
            _existingTarget = _targetNode;
        }
        Vector3 pos = transform.position;
        _xInput = _path[1].gridX - grid.GetNodeFromWorldPos(pos).gridX;
        _yInput = _path[1].gridY - grid.GetNodeFromWorldPos(pos).gridY;

        
        animator.SetFloat("XInput", _xInput);
        animator.SetFloat("YInput", _yInput);
        animator.SetBool("isWalking", true);
        float goal;
        if (pos.x == _path[1].worldPosition.x)
        {
            goal = _path[1].worldPosition.y;
            while (pos.y != goal)
            {
                pos.y = Mathf.MoveTowards(pos.y, goal, 2.75f * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }
        }
        else if(pos.y == _path[1].worldPosition.y)
        {
            goal = _path[1].worldPosition.x;

            while (pos.x != goal)
            {
                pos.x = Mathf.MoveTowards(pos.x, goal, 2.75f * Time.deltaTime);
                transform.localPosition = pos;
                yield return null;
            }


        }
        _coroutineStart = false;

    }

    //Function to reduce the player's health
    public void ReduceStrength(float hit)
    {
        float healthReduc = hit / _fullHealth;
        
        _strength -= hit;
        healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - healthReduc);
        if(_strength <= 0)
        {
            lost = true;
        }

    }

    //Function to restart a dungeon level
    private void RestartLevel()
    {
        if (lost)
        {
            Pause();
            lost = false;
            GameObject levels = GameObject.FindWithTag("Levels");
            _targetNode = null;
            StopAllCoroutines();
            SetAllBoolsFalse();

            if(_data.activeLevel != 1) 
            { 
                levels.transform.GetChild(_data.activeLevel-1).gameObject.SetActive(false);
                levels.transform.GetChild(_data.activeLevel-2).gameObject.SetActive(true);
            }
            leaveLevel = true;
            healthBar.SetHealthBarValue(1);
            _strength = _fullHealth;
            StartCoroutine(Wait());
            Unpause();
            _foundPos = false;
        }
    }

    //Function for the attack button to attack a monster
    public void Attack()
    {
        animator.SetBool("isWalking", false);
        _targetNode = null;
        _coroutineStart = false;
        if (MonsterNearby() && !_attackBool)
        {
            animator.SetTrigger("Attack");
            StartCoroutine(WaitToAttack());
        }
    }

    //Function for the block button to block a monster
    public void block()
    {

        animator.SetBool("isWalking", false);
        _targetNode = null;
        _coroutineStart = false;
        if (!_blockBool && MonsterNearby())
        {
            StartCoroutine(WaitToBlock());
        }
    }

    //Function to check if a monster is next to the player
    private bool MonsterNearby()
    {
        foreach (Node neighbour in grid.GetNodeNeighbours(grid.GetNodeFromWorldPos(transform.position)))
        {
            if (grid.GetEnemiesPos().Contains(neighbour))
            {
                _xInput = neighbour.gridX - grid.GetNodeFromWorldPos(transform.position).gridX;
                _yInput = neighbour.gridY - grid.GetNodeFromWorldPos(transform.position).gridY;

                animator.SetFloat("XInput", _xInput);
                animator.SetFloat("YInput", _yInput);

                return true;
            }
        }

        return false;
    }

    //Function to make the player wait until he can attack again
    IEnumerator WaitToAttack()
    {
        yield return new WaitForEndOfFrame();
        _attackBool = true;
        enemyHit = true;
        yield return new WaitForSeconds(1);
        _attackBool = false;
    }

    //Function to make the player wait until he can block again
    IEnumerator WaitToBlock()
    {
        _blockBool = true;
        blockEnemy = true;
        yield return new WaitForSeconds(0.5f);
        blockEnemy = false;
        yield return new WaitForSeconds(2);
        _blockBool = false;
    }
    //Function to wait
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        leaveLevel = false; 
        if (!_chooseExit)
        {

            EntranceLevel();
        }
        else
        {
            _chooseExit = false;
            ExitLevel();
        }
    }

    //Function to find a puzzle by its tag
    private GameObject FindWithTag(string tag)
    {
        GameObject foundPuzzles = null;
        for(int i = 0; i < puzzles.transform.childCount; i++)
        {
            if(puzzles.transform.GetChild(i).CompareTag(tag))
            {
                foundPuzzles = puzzles.transform.GetChild(i).gameObject;
            }
        }
        return foundPuzzles;
    }

    //Function to pause the player
    public void Pause()
    {
        StopAllCoroutines();
        _coroutineStart = true;
        _targetNode = null;
        animator.SetBool("isWalking", false);
    }

    //Function to unpause the player
    public void Unpause()
    {
        animator.SetBool("isWalking", false);
        StartCoroutine(WaitToMove());
    }

    //Function to make the player wait until they can move again
    IEnumerator WaitToMove()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _targetNode = null;
        _coroutineStart = false;

    }

    //Function for the restart button to restart the whole game
    public void Restart()
    {
        _data.namePlayer = null;
        _data.SaveGame();
        Application.Quit();
    }

    public void PlayerInvisiable()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        Pause();
    }

    public void PlayerVisiable()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    public bool isInvisiable()
    {
        if(this.GetComponent<SpriteRenderer>().enabled == true)
        {
            return false;
        }
        return true;
    }

    public void SwitchCams()
    {
        playerCam.SetActive(!playerCam.activeSelf);
        centerCam.SetActive(!centerCam.activeSelf);
    }


    public List<string> WonSentences()
    {

        List<string> sentences = new List<string>()
        {
            "Congratulations! \nYou solved the puzzle!",
            "Thank you so much for your help, " + _data.namePlayer + "!",
            "Goodbye, " + _data.namePlayer + ".\nGood luck with the dungeon!",
            "Brilliant! \nI knew you could help me.",
            "Now I can finally pass on. \nThank you, " + _data.namePlayer + "!"

        };

        return sentences;

    }

    IEnumerator SelfHeal()
    {
        yield return new WaitForSeconds(4);
        float hit = (_fullHealth / 100) * 3;
        if(_strength + hit >_fullHealth)
        {
            hit = _fullHealth - _strength;
        }
        float healthReduc = hit / _fullHealth;

        _strength += hit;
        healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() + healthReduc);
        heal = true;

    }

    public void Stay()
    {

        message2Options.transform.parent.parent.gameObject.SetActive(false);
        Unpause();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (upperWorld)
        {
            _data.xPos = grid.GetNodeFromWorldPos(transform.position).gridX;
            _data.yPos = grid.GetNodeFromWorldPos(transform.position).gridY;

        }
        _data.SaveGame();
    }
    private void OnApplicationPause(bool pause)
    {
        if (upperWorld)
        {
            _data.xPos = grid.GetNodeFromWorldPos(transform.position).gridX;
            _data.yPos = grid.GetNodeFromWorldPos(transform.position).gridY;

        }
        _data.SaveGame();
    }
}
