using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	private Vector3 location;

    

	private float percentage;
	private GameObject HPImg; //the inner part of health bar
	private float HPImgWidth; //57, controls inner hp placement
	private RectTransform HPImgTransform;
	private float updateSpeed = 5.0f;

    private float startingXScale;
    private float startingYScale;

    // Use this for initialization
    void Awake() {

        Initialize();
    }

    void Start () {


    }

    public void Initialize() {
        if(!HPImg)
            HPImg = transform.GetChild(1).gameObject;
        HPImgTransform = HPImg.GetComponent<RectTransform>();

        startingXScale = HPImgTransform.localScale.x;
        startingYScale = HPImgTransform.localScale.y;

        HPImgWidth = HPImgTransform.rect.width * startingXScale;


        //UpdateHealthBarNoLerp(tempHealth, tempMaxHealth);
    }


	// Update is called once per frame
	void Update () {
        //testHealth -= Time.deltaTime;

        //UpdateHealthBar(tempHealth, tempMaxHealth);
		//UpdateBarSize ();
        //CheckHPVisible();
		
	}
    
    public void UpdateBarNoLerp(float health, float maxHealth) {
        if (maxHealth <= 0)
            return;
        percentage = Mathf.Clamp(health / maxHealth, 0.0f, 1.0f);
        if (percentage < 0.0f)
            percentage = 0.0f;
        if (!HPImgTransform)
            HPImgTransform = transform.GetChild(1).gameObject.GetComponent<RectTransform>();


        float curScaleX = HPImgTransform.localScale.x;
        HPImgTransform.localScale = new Vector3(startingXScale * percentage, startingYScale, 1.0f);

        float curPosX = HPImgTransform.localPosition.x;
        HPImgTransform.localPosition = new Vector3((-HPImgWidth / 2 * (1 - percentage)), 0, 0);


        //		Vector3 newPos = new Vector3 (leftMost + -leftMost*percentage, 0.0f, 0.0f);
        //		HPImg.localPosition = Vector3.Lerp (HPImg.localPosition, newPos, updateSpeed*Time.deltaTime);
        //HPImg.localPosition = newPos;
        if (!HPImg)
            HPImg = transform.GetChild(1).gameObject;


        if (percentage > 0.50f) {
            HPImg.GetComponent<Image>().color = Color.Lerp(
                new Color(63.0f / 255.0f, 191.0f / 255.0f, 63.0f / 255.0f, 1.0f)
                , Color.yellow, (maxHealth - health) / (maxHealth / 2));
        } else if (percentage <= 0.50f) {
            HPImg.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.red, (maxHealth / 2 - health) / (maxHealth / 2));
        }
    }

	public void UpdateBar(float health, float maxHealth) {
        if (maxHealth <= 0) 
            return;
        percentage = Mathf.Clamp(health / maxHealth, 0.0f, 1.0f);
		if (percentage < 0.0f)
			percentage = 0.0f;
		if(!HPImgTransform)
			HPImgTransform = transform.GetChild (1).gameObject.GetComponent<RectTransform> ();


		float curScaleX = HPImgTransform.localScale.x;
		HPImgTransform.localScale = new Vector3(Mathf.Lerp (
			curScaleX, startingXScale * percentage, updateSpeed * Time.deltaTime),
			startingYScale, 1.0f);

		float curPosX = HPImgTransform.localPosition.x;
		HPImgTransform.localPosition = new Vector3(Mathf.Lerp (
			curPosX, 
			(-HPImgWidth/2 * (1-percentage)), 
			updateSpeed * Time.deltaTime), 
			0, 0);



//		Vector3 newPos = new Vector3 (leftMost + -leftMost*percentage, 0.0f, 0.0f);
//		HPImg.localPosition = Vector3.Lerp (HPImg.localPosition, newPos, updateSpeed*Time.deltaTime);
		//HPImg.localPosition = newPos;
		if (!HPImg)
            HPImg = transform.GetChild(1).gameObject;


        if (percentage > 0.50f) {
			HPImg.GetComponent<Image>().color = Color.Lerp (
                new Color(63.0f / 255.0f, 191.0f / 255.0f, 63.0f / 255.0f,1.0f), 
                Color.yellow, (maxHealth - health) / (maxHealth / 2));
		} else if (percentage <= 0.50f) {
			HPImg.GetComponent<Image>().color = Color.Lerp (Color.yellow, Color.red, (maxHealth / 2 - health) / (maxHealth / 2));
		}
	}
}
