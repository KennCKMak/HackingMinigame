using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowGame : MonoBehaviour {
    [Header("Row variables")]
    //where should this guy be going to??
    public int yDestLoc = 250;

    [Header("Block info")]
    public int xBlockLoc = -435;
    public int xBlockDist = 80;
    public GameObject blockPrefab;

    [Header("Block array")]
    public GameObject[] blockObjArray = new GameObject[12];
    public Block[] blockArray = new Block[12];

    [Header("Letters array")]
    public string[] inputArray = new string[12];

    [Header("Logic variables")]
    public bool initialized = false;
    public bool isActive = false;
    public int currentBlockIndex;
    public int activeBlocks;
    public int maxPresses;


	void Start () {
        Initialize();
	}

    void Initialize() {
        for(int i =0; i < 12; i++) {
            GameObject newBlock = Instantiate(blockPrefab, transform);
            newBlock.GetComponent<RectTransform>().localPosition = new Vector3(xBlockLoc + xBlockDist * i, 0);
            blockObjArray[i] = newBlock;
            blockArray[i] = newBlock.GetComponent<Block>();
        }
        SetLetters();
    }

    /// <summary>
    /// Adds letters to each block, disables the unneeded characters, and 
    /// </summary>
    void SetLetters() {
        switch (GameManager.CurrentDifficulty) {
            case GameManager.Difficulty.Easy:
                activeBlocks = 4;
                maxPresses = 2;
                break;
            case GameManager.Difficulty.Normal:
                activeBlocks = 6;
                maxPresses = 2;
                break;
            case GameManager.Difficulty.Hard:
                activeBlocks = 8;
                maxPresses = 3;
                break;
            case GameManager.Difficulty.Xtreme:
                activeBlocks = 12;
                maxPresses = 4;
                break;
            default:
                activeBlocks = 4;
                break;
        }

        for(int i = 0; i < activeBlocks; i++) {
            string letter = GetRandomLetter();
            blockArray[i].SetText(letter);
            inputArray[i] = letter;


            blockArray[i].SetNumMax(Random.Range(1, maxPresses));

            blockArray[i].Enable();
        }

        if (activeBlocks == 12)
            return;

        for(int i = activeBlocks; i < 12; i++) {
            blockArray[i].Disable();
        }
        currentBlockIndex = 0;
    }



    /// <summary>
    /// Returns a random letter of W, A, S or D
    /// </summary>
    public string GetRandomLetter() {
        int ran = Random.Range(1, 4);
        switch (ran) {
            case 1:
                return "W";
            case 2:
                return "A";
            case 3:
                return "S";
            case 4:
                return "D";
            default:
                Debug.Log("Not WASD input...?");
                return "W";
        }
    }


    // Update is called once per frame
    void Update() {
        if (gameObject.GetComponent<RectTransform>().localPosition.y != yDestLoc) {
            gameObject.GetComponent<RectTransform>().localPosition =
                Vector3.Lerp(gameObject.GetComponent<RectTransform>().localPosition,
                new Vector3(0, yDestLoc, 0), 5.0f * Time.deltaTime);
        }

        if (!isActive)
            return;


        if (Input.GetKeyDown(KeyCode.W)) {
            ReadInput("W");
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            ReadInput("A");
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            ReadInput("S");
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            ReadInput("D");
        }
    }
        
    //compares the input with what we have
    public void ReadInput(string letter) {
        
        if(blockArray[currentBlockIndex].letter == letter) {
            AudioManager.instance.PlaySFX("Correct");
            //inputted correct letter
            if (blockArray[currentBlockIndex].numTimesPressed > 1) {
                blockArray[currentBlockIndex].SetNum(blockArray[currentBlockIndex].numTimesPressed -1);
                Flash(blockArray[currentBlockIndex], Color.green);
                return;
            }
            Highlight(blockArray[currentBlockIndex], Color.green);
            blockArray[currentBlockIndex].finished = true; //force it to stay green!
            blockArray[currentBlockIndex].isSelected(false);
            blockArray[currentBlockIndex].SetNum(blockArray[currentBlockIndex].numTimesPressed - 1);

            
            if (currentBlockIndex < 11 && blockArray[currentBlockIndex+1])
                blockArray[currentBlockIndex+1].isSelected(true);

            currentBlockIndex++;


            CheckRowIsDone();
        } else {
            //failed input
            Flash(blockArray[currentBlockIndex], Color.red);
            AudioManager.instance.PlaySFX("Error");
        }
    }

    public void CheckRowIsDone() {
        if (currentBlockIndex >= activeBlocks) {
            GameManager.instance.ReceiveRowInfo(true); //we cleared it!
            isActive = false;
        }
    }
    
    public void SetActive(bool b) {
        if (b)
            StartCoroutine(SetActiveTimer());
        else
            isActive = false;
    }

    IEnumerator SetActiveTimer() {
        yield return new WaitForSeconds(0.15f);
        isActive = true;

        blockArray[0].isSelected(true);
    }

    /// <summary>
    /// permanently sets the color of a block
    /// </summary>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public void Highlight(Block b, Color c) {
        b.Highlight(c);
    }

    public void HighlightAll(Color c) {
        for(int i = 0; i < activeBlocks; i++) {
            Highlight(blockArray[i], c);
        }
    }
    public void Flash(Block b, Color c) {
        b.Flash(c);
        
    }
    public void FlashAll(Color c) {
        for(int i = 0; i < activeBlocks; i++) {
            Flash(blockArray[i], c);
        }
    }
}
