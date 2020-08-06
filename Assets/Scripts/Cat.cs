using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Cat : Player
{
    int canCounter;
    const int CAN_NUMBER = 10;//罐頭連打數

    public Image leftPawImage, rightPawImage;
    public Transform pawNormalPosition, pawSkillPosition;
    public Sprite normalPawLeft, normalPawRight, attackPawLeft, attackPawRight;
    public GameObject gameObjectPaper;
    public Text canNumberText;

    // Start is called before the first frame update
    void Start()
    {
        type = PlayerType.CAT;
        canCounter = 0;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            UpdateSkillValue();
            DetectInput();
            if (Input.GetKeyDown(input.attack))
            {
                rightPawImage.sprite = attackPawRight;
                if (isSkilled) HandleBeSkilled();
            }
            else if (Input.GetKeyUp(input.attack))
            {
                rightPawImage.sprite = normalPawRight;
            }
        }
    }

    //設定使用技能
    public override void SetUseSkill(bool boolean)
    {
        if (boolean)
        {
            leftPawImage.sprite = attackPawLeft;
            leftPawImage.transform.position = pawSkillPosition.position;
        }
        else
        {
            leftPawImage.sprite = normalPawLeft;
            leftPawImage.transform.position = pawNormalPosition.position;
        }
        leftPawImage.SetNativeSize();
    }

    //設定被使用技能
    public override void SetIsSkilled(bool boolean)
    {
        base.SetIsSkilled(boolean);
        if (boolean)
        {

            canNumberText.text = "x" + CAN_NUMBER;
            gameObjectPaper.SetActive(false);
        }
    }

    //處理被使用技能
    void HandleBeSkilled()
    {
        canCounter++;
        canNumberText.text = "x" + (CAN_NUMBER - canCounter);
        if (canCounter == CAN_NUMBER)
        {
            canCounter = 0;
            isSkilled = false;
            gameObjectSkilled.SetActive(false);
            gameObjectPaper.SetActive(true);
            UnSkilledEvent();
        }
    }
}
