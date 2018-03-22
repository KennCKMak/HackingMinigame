using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public GameObject middlePanel;
    public TextMeshProUGUI hackText;
    public Slider hackSlider;
    public TextMeshProUGUI lockText;
    public Slider lockSlider;
    public TextMeshProUGUI difficultyText;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameManager.instance.hackSkillLevel = (int)hackSlider.value;
        hackText.text = "Hack Skill: " + GameManager.instance.hackSkillLevel;
        GameManager.instance.lockSkillLevel = (int)lockSlider.value;
        lockText.text = "Lock Level: " + GameManager.instance.lockSkillLevel;

        GameManager.instance.UpdateDifficulty();
        difficultyText.text = "Difficulty: " + GameManager.CurrentDifficulty.ToString();

    }

    public void btnStartGame() {
        middlePanel.SetActive(false);
        GameManager.instance.StartGame(GameManager.CurrentDifficulty);
    }
}
