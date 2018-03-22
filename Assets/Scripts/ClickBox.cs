using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickBox : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler {

    public ClickGame owningClickGame;

    public bool finished = false;
    public string letter;

    protected Color startColor;
    protected Color destColor;

    protected bool flashing = false;

    TMP_Text letterText;
    RawImage img;

    void Awake() {

        img = transform.GetChild(0).GetComponent<RawImage>();
        letterText = transform.GetChild(1).GetComponent<TMP_Text>();
        startColor = img.color;
        destColor = startColor;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!owningClickGame)
            return;
        if (img.color != destColor && !flashing) {
            UpdateColorChange();
        }

        //force to green if we finished this
        if (finished && startColor != destColor) {
            startColor = destColor;
            UpdateColorChange();
        }

    }


    public void OnPointerEnter(PointerEventData eventData) {

    }

    public void OnPointerExit(PointerEventData eventData) {
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (owningClickGame)
            owningClickGame.receiveClickFrom(this);
        else
            Debug.Log("Missing click game");
    }

    public void OnPointerUp(PointerEventData eventData) {
    }


    void UpdateColorChange() {
        //color speed
        img.color = Color.Lerp(img.color, destColor, 8.0f * Time.deltaTime);

    }

    public void SetText(string s) {
        letter = s;
        letterText.text = letter;
    }
    /// <summary>
    /// Disables the object
    /// </summary>
    public void Enable() {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// Disables img
    /// </summary>
    public void Disable() {
        gameObject.SetActive(false);
    }

    public void ResetColor() {
        destColor = startColor;
    }

    /// <summary>
    /// Highlights a certain colour permanently
    /// </summary>
    /// <param name="b"></param>
    public void Highlight(Color c) {
        destColor = c;
    }
    /// <summary>
    /// Flashes a certain colour temporarily
    /// </summary>
    /// <param name="color"></param>
    public void Flash(Color color) {
        StartCoroutine(FlashCoroutine(color));
    }

    IEnumerator FlashCoroutine(Color color) {
        flashing = true;
        img.color = Color.Lerp(img.color, color, 30.0f * Time.deltaTime);
        yield return new WaitForSeconds(0.1f);

        //if we didn't complete this, return to normal colour
        //otherwise just go to the new destcolour set by Row.cs
        if (!finished)
            destColor = startColor;

        flashing = false;
    }

    /// <summary>
    /// changes the outer ring colour of the block
    /// </summary>
    /// <param name="b"></param>
    public void isSelected(bool b) {
        if (b)
            transform.GetComponent<RawImage>().color = Color.blue;
        else
            transform.GetComponent<RawImage>().color = Color.black;
    }
}
