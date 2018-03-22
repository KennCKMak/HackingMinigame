using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public bool GameIsRunning = false;
    public static GameManager instance = null;

    public int hackSkillLevel = 50;
    public int lockSkillLevel = 50;

    [Header("Timer/Success bars")]
    public HealthBar timerBar;
    public float currTimer;
    public float maxTimer  = 30.0f;
    public HealthBar successBar1;
    public float currSuccess1 = 0.0f;
    public float maxSuccess1 = 100.0f;
    public HealthBar successBar2;
    public float currSuccess2 = 0.0f;
    public float maxSuccess2 = 100.0f;

    [Header("Click Game Management")]
    public GameObject clickGamePrefab;
    public GameObject clickGame;
    public int clickGameX = 0;
    public int clickGameY = 0;

    [Header("Row Management")]
    public int yRowTop = 250;
    public int yRowDist = 125;
    public int numRows;
    public int maxRows = 6;
    public int remainingRows = 10;
    public int maxRowsToSpawn = 10;
    public RowGame[] rowArray = new RowGame[6];
    public GameObject rowPrefab;



    public enum Difficulty {
        Easy,
        Normal,
        Hard,
        Xtreme
    };
    public static Difficulty CurrentDifficulty = Difficulty.Easy;

    void Awake() {
        Screen.SetResolution(1024, 768, true);
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


	// Use this for initialization
	void Start () {

        

        //GameIsRunning = true;
        //StartGame(Difficulty.Easy);
        //StartRowGame();
    }


	
	// Update is called once per frame
	void Update () {


        timerBar.UpdateBar(currTimer, maxTimer);
        successBar1.UpdateBar(currSuccess1, maxSuccess1);
        successBar2.UpdateBar(currSuccess2, maxSuccess2);

        if (GameIsRunning) {
            currTimer -= Time.deltaTime;
            if (currTimer < 0)
                EndGame(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="num"></param>
    public void StartGame(int num) {
        StartGame((Difficulty)num);
    }

    public void StartGame(Difficulty newDifficulty) {
        
        CurrentDifficulty = newDifficulty;

        GameIsRunning = true;

        currTimer = maxTimer;
        currSuccess1 = currSuccess2 = 0;
        successBar1.UpdateBarNoLerp(currSuccess1, maxSuccess1);
        successBar2.UpdateBarNoLerp(currSuccess2, maxSuccess2);
        StartClickGame();
    }
    /// <summary>
    /// Ends the game
    /// </summary>
    /// <param name="b">true if win</param>
    public void EndGame(bool b) {
        //game was ended
        GameIsRunning = false;
        if (b) {
            //win
            AudioManager.instance.PlaySFX("Victory");
        } else {
            AudioManager.instance.PlaySFX("Alert");
            AudioManager.instance.PlaySFX("Defeat");
            //lose
        }


        if (clickGame)
            Destroy(clickGame);
        for(int i = 0; i < 6; i++) {
            if (rowArray[i])
                Destroy(rowArray[i].gameObject);
            numRows = 0;
        
        }

        StartCoroutine(ShowMiddlePanel(0.8f));
    }
    IEnumerator ShowMiddlePanel(float time) {
        yield return new WaitForSeconds(time);
        UIManager.instance.middlePanel.SetActive(true);
    }
    public void HideMiddlePanel() {
        UIManager.instance.middlePanel.SetActive(false);
    }

    public void StartClickGame() {
        GameObject newClickGame = Instantiate(clickGamePrefab, GameObject.Find("Canvas").transform) as GameObject;
        newClickGame.GetComponent<RectTransform>().localPosition = new Vector3(clickGameX, clickGameY);
        newClickGame.transform.SetSiblingIndex(0);
        newClickGame.GetComponent<ClickGame>().SetActive(true);

        AudioManager.instance.PlaySFX("RowEnter");

        clickGame = newClickGame;
        currSuccess1 = 0;
        timerBar.UpdateBarNoLerp(currTimer, maxTimer);
        successBar1.UpdateBarNoLerp(currSuccess1, maxSuccess1);
    }

    /// <summary>
    /// Starts the row mini game
    /// </summary>
    void StartRowGame() {
        switch (CurrentDifficulty) {
            case Difficulty.Easy:
                maxRowsToSpawn = 4;
                break;
            case Difficulty.Normal:
                maxRowsToSpawn = 5;
                break;
            case Difficulty.Hard:
                maxRowsToSpawn = 6;
                break;
            case Difficulty.Xtreme:
                maxRowsToSpawn = 6;
                break;
            default:
                break;
        }
        remainingRows = maxRowsToSpawn;
        currSuccess2 = 0.0f;
        timerBar.UpdateBar(currTimer, maxTimer);
        successBar2.UpdateBarNoLerp(currSuccess2, maxSuccess2);
        for (int i = 0; i < 5; i++) {
            SpawnRow();
        }

    }


    /// <summary>
    /// spawn a row at very bottom, assign them a position
    /// </summary>
    public void SpawnRow() {
        if (numRows >= maxRows || remainingRows <= 0) 
            return;
        GameObject newRow = Instantiate(rowPrefab, GameObject.Find("Canvas").transform) as GameObject;
        newRow.GetComponent<RectTransform>().localPosition = new Vector3(0, -1000);
        newRow.transform.SetSiblingIndex(0);
        rowArray[numRows] = newRow.GetComponent<RowGame>();
        rowArray[numRows].yDestLoc = yRowTop - yRowDist * numRows;
        AudioManager.instance.PlaySFX("RowEnter");

        if (numRows != 0) //this is the first row
            rowArray[0].SetActive(true);

        numRows++;
        remainingRows--;
    }

    /// <summary>
    /// row completed, shift all rows up by one
    /// </summary>
    public void ClearRow() {
        if (numRows <= 0)
            return;
        //need to remove first obj and move everything up
        rowArray[0].yDestLoc += 300;
        Destroy(rowArray[0].gameObject, 2.0f);
        AudioManager.instance.PlaySFX("RowLeave");

        for (int i = 0; i < numRows - 1; i++) {
            if (rowArray[i + 1] == null) {
                break;
            }
            rowArray[i] = rowArray[i + 1];
            rowArray[i + 1] = null;
            rowArray[i].yDestLoc += yRowDist;
        }
        if (numRows == 1) {
            rowArray[0].yDestLoc += yRowDist;
            rowArray[0] = null;
        }

        numRows--;
        if (numRows > 0)//checking if there is any row left
            rowArray[0].SetActive(true);

        if (numRows == 0 && remainingRows == 0) {
            Debug.Log("Cleared row");
            EndGame(true);
        }
    }

    IEnumerator RestartGame() {
        yield return new WaitForSeconds(0.5f);
        StartRowGame();
    }


    public void UpdateDifficulty() {
        //hackSkillLevel, lockSkillLevel
        float skillDiff = hackSkillLevel - lockSkillLevel;

        if(skillDiff >= 30) {
            CurrentDifficulty = Difficulty.Easy;
        } else if (-10 <= skillDiff && skillDiff < 30) {
            CurrentDifficulty = Difficulty.Normal;
        } else if (-35 <= skillDiff && skillDiff < -10) {
            CurrentDifficulty = Difficulty.Hard;
        } else {
            CurrentDifficulty = Difficulty.Xtreme;
        }
    }

    public void IncreaseDifficulty() {
        switch (CurrentDifficulty) {
            case Difficulty.Easy:
                CurrentDifficulty = Difficulty.Normal;
                break;
            case Difficulty.Normal:
                CurrentDifficulty = Difficulty.Hard;
                break;
            case Difficulty.Hard:
                CurrentDifficulty = Difficulty.Xtreme;
                break;
            case Difficulty.Xtreme:
            default:
                break;
        }
    }

    public void DecreaseDifficulty() {
        switch (CurrentDifficulty) {
            case Difficulty.Normal:
                CurrentDifficulty = Difficulty.Easy;
                break;
            case Difficulty.Hard:
                CurrentDifficulty = Difficulty.Normal;
                break;
            case Difficulty.Xtreme:
                CurrentDifficulty = Difficulty.Hard;
                break;
            case Difficulty.Easy:
            default:
                break;
        }
    }

    public void ReceiveRowInfo(bool result) {
        Debug.Log("Row cleared");

        currTimer = Mathf.Clamp(currTimer + 5.0f, 0.0f, maxTimer);
        currSuccess2 += (maxSuccess2 / maxRowsToSpawn);
        ClearRow();

        SpawnRow();
    }


    public void ReceiveClickGameInfo(ClickGame game) {
        Debug.Log("Click game cleared");
        StartCoroutine(DelayedClickGameEnd(game));

    }

    IEnumerator DelayedClickGameEnd(ClickGame game) {
        currSuccess1 += (maxSuccess1);
        currTimer += 10;

        yield return new WaitForSeconds(0.6f);

        currTimer = Mathf.Clamp(currTimer + 5.0f, 0.0f, maxTimer);
        if (GameIsRunning) ;

        game.yDestLoc += 1000;
        if(game.gameObject)
            Destroy(game.gameObject, 1.0f);

        AudioManager.instance.PlaySFX("MoveCircuit");
        StartRowGame();
    }
}
