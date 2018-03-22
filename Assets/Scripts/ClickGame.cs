using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickGame : MonoBehaviour {


    [Header("Line properties")]
    public GameObject linePrefab;
    public float lineWidth = 8.0f;
    public int lineCount;
    public GameObject[] lines = new GameObject[12];

    [Header("ClickBox Info")]
    public GameObject clickBoxPrefab;
    public int boxMargin = 70 +5; //width of the box + add

    [Header("ClickBox Array")]
    public GameObject[] boxObjArray = new GameObject[12];
    public ClickBox[] boxArray = new ClickBox[12];

    [Header("Number and Logic")]
    public bool initialized = false;
    public bool isActive = false;
    public int currentBoxIndex;
    public int activeBoxes; //changed by difficulty

    [HideInInspector]
    //where should this guy be going to??
    public int yDestLoc = 0;


    // Use this for initialization
    void Start() {
        Initialize();
    }
    
    /// <summary>
    /// Place blocks
    /// </summary>
    void Initialize() {
        for (int i = 0; i < 12; i++) {
            GameObject newBlock = Instantiate(clickBoxPrefab, transform);
            newBlock.GetComponent<RectTransform>().localPosition = 
                GetRandomLocation(gameObject.GetComponent<RectTransform>().rect.width,
                    gameObject.GetComponent<RectTransform>().rect.height);

            boxObjArray[i] = newBlock;
            boxArray[i] = newBlock.GetComponent<ClickBox>();
        }
        SetNumbers();
        isActive = true;
    }

    /// <summary>
    /// set numbers and disable unused box according to difficulty
    /// </summary>
    void SetNumbers() {
        switch (GameManager.CurrentDifficulty) {
            case GameManager.Difficulty.Easy:
                activeBoxes = 6;
                break;
            case GameManager.Difficulty.Normal:
                activeBoxes = 8;
                break;
            case GameManager.Difficulty.Hard:
                activeBoxes = 10;
                break;
            case GameManager.Difficulty.Xtreme:
                activeBoxes = 12;
                break;
            default:
                activeBoxes = 6;
                break;
        }

        for (int i = 0; i < activeBoxes; i++) {
            boxArray[i].SetText((i+1).ToString()); //add 1 so it starts from 1 -> 12
            boxArray[i].owningClickGame = this;
        }

        if (activeBoxes == 12)
            return;

        for (int i = activeBoxes; i < 12; i++) {
            boxArray[i].Disable();
        }
        currentBoxIndex = 0;
    }
    
    /// <summary>
    /// get a random location with give width and length of transform object
    /// </summary>
    /// <param name="xMax"></param>
    /// <param name="yMax"></param>
    /// <returns></returns>
    Vector3 GetRandomLocation(float xMax, float yMax) {
        //x = 0, xMax... y = 0, yMax
        Vector3 tempVec = new Vector3();
        bool validPos = false;
        while (!validPos) {
            float x = Random.Range(0 + boxMargin/2, xMax - boxMargin/2);
            float y = Random.Range(0 + boxMargin/2, yMax - boxMargin/2);
            tempVec.x = x; tempVec.y = y;
            tempVec.x -= xMax / 2; //move it back to the left 
            tempVec.y -= yMax / 2; //move it back down 
            validPos = checkBoxPlacement(tempVec);
        }
        return tempVec;
    }

    /// <summary>
    /// compares parameter vec to see if it will collide with any present boxes already there
    /// </summary>
    /// <param name="newVec"></param>
    /// <returns></returns>
    bool checkBoxPlacement(Vector3 newVec) {
        for (int i = 0; i < 12; i++) {
            if (!boxObjArray[i])
                break;
            Vector3 boxPos = boxObjArray[i].transform.localPosition;
            float xDist = Mathf.Abs(boxPos.x - newVec.x);
            float yDist = Mathf.Abs(boxPos.y - newVec.y);

            if(xDist <= boxMargin && yDist <= boxMargin) {
                Debug.Log("Collision");
                return false;
            }
        }
        return true;
    }

    void Update() {

        if (gameObject.GetComponent<RectTransform>().localPosition.y != yDestLoc) {
            gameObject.GetComponent<RectTransform>().localPosition =
                Vector3.Lerp(gameObject.GetComponent<RectTransform>().localPosition,
                new Vector3(0, yDestLoc, 0), 5.0f * Time.deltaTime);
        }

        if (!isActive)
            return;
    }

    //compares the input with what we have
    public void receiveClickFrom(ClickBox b) {
        if (!isActive)
            return;

        //add onen to index since its starting from 1 -> 12
        if (b.letter == (currentBoxIndex+1).ToString()) {
            AudioManager.instance.PlaySFX("Correct");
            //inputted correct letter
            Highlight(boxArray[currentBoxIndex], Color.green);
            boxArray[currentBoxIndex].finished = true; //force it to stay green!
            boxArray[currentBoxIndex].isSelected(false);
            
            if (currentBoxIndex < 11 && boxArray[currentBoxIndex + 1])
                boxArray[currentBoxIndex + 1].isSelected(true);

            //draw a line
            if (currentBoxIndex > 0 && currentBoxIndex < activeBoxes)
                DrawLine(boxObjArray[currentBoxIndex-1].transform.localPosition,
                    boxObjArray[currentBoxIndex].transform.localPosition);

            AudioManager.instance.PlaySFX("ConnectCircuit");

            currentBoxIndex++;


            CheckGameIsDone();
        } else {
            //failed input
            Flash(b, Color.red);
            AudioManager.instance.PlaySFX("Error");
        }
    }

    void CheckGameIsDone() {
        if(currentBoxIndex == activeBoxes) {
            GameManager.instance.ReceiveClickGameInfo(this); //we cleared it!
            AudioManager.instance.PlaySFX("ClearCircuit");
            isActive = false;
        }
    }


    /// <summary>
    /// Draw Line using GameObject parameters
    /// </summary>
    /// <param name="start"></param>
    /// <param name="stop"></param>
    void DrawLine(GameObject start, GameObject stop) {
        DrawLine(start.transform.localPosition, stop.transform.localPosition);
    }

    /// <summary>
    /// Draw line using Vector parameters and places it under this object's transform
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    void DrawLine(Vector3 startPos, Vector3 endPos) {

        GameObject newLine = Instantiate(linePrefab, this.transform) as GameObject;
        newLine.transform.SetAsFirstSibling();
        newLine.GetComponent<RawImage>().color = Color.cyan;
        RectTransform imageRectTransform = newLine.GetComponent<RectTransform>();

        Vector3 differenceVector = endPos - startPos;
        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        imageRectTransform.localPosition = startPos + differenceVector / 2;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        newLine.transform.localRotation = Quaternion.Euler(0, 0, angle);

        lines[lineCount] = newLine;
        lineCount += 1;
    }
     
    /// <summary>
    /// Destroys and removes all lines
    /// </summary>
    void DestroyAllLines() {
        for(int i = 0; i < lineCount; i++) {
            Destroy(lines[i].gameObject);
            lines[i] = null;
        }
        lineCount = 0;
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

        boxArray[0].isSelected(true);
    }

    /// <summary>
    /// permanently sets the color of a block
    /// </summary>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public void Highlight(ClickBox b, Color c) {
        b.Highlight(c);
    }

    public void HighlightAll(Color c) {
        for (int i = 0; i < activeBoxes; i++) {
            Highlight(boxArray[i], c);
        }
    }
    public void Flash(ClickBox b, Color c) {
        b.Flash(c);

    }
    public void FlashAll(Color c) {
        for (int i = 0; i < activeBoxes; i++) {
            Flash(boxArray[i], c);
        }
    }
}