using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Cat : Player
{
    int canCounter;
    const int CAN_NUMBER = 10;//罐頭連打數

    public Image leftPawImage, rightPawImage, bodyImage;
    public Transform pawNormalPosition, pawSkillPosition, pawNormalPositionR, 
        pawSkilledPositionR, bodyNormalPosition, bodySkilledPosition;
    public Sprite normalPawLeft, normalPawRight, attackPawLeft, attackPawRight, 
        skilledBody, normalBody, skilledPawNormalRight, skilledPawAttackRight;
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
                if (isSkilled)
                {
                    rightPawImage.sprite = skilledPawAttackRight;
                    HandleBeSkilled();
                    audioSource.PlayOneShot(skilled);
                }
                else
                {
                    rightPawImage.sprite = attackPawRight;
                    audioSource.PlayOneShot(attack);
                }
                rightPawImage.SetNativeSize();
            }
            else if (Input.GetKeyUp(input.attack))
            {
                rightPawImage.sprite = isSkilled ? skilledPawNormalRight : normalPawRight;
                rightPawImage.SetNativeSize();
            }
        }
    }

    //設定使用技能
    public override void SetUseSkill(bool boolean)
    {
        if (boolean)
        {
            audioSource.PlayOneShot(skill);
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
            SetSkilledUI(boolean);
            canNumberText.text = "x" + CAN_NUMBER;
        }
    }

    void SetSkilledUI(bool boolean)
    {
        gameObjectPaper.SetActive(!boolean);
        leftPawImage.gameObject.SetActive(!boolean);
        bodyImage.sprite = boolean ? skilledBody: normalBody;
        bodyImage.SetNativeSize();
        bodyImage.transform.localPosition = boolean ? bodySkilledPosition.localPosition : bodyNormalPosition.localPosition;
        rightPawImage.sprite = boolean ? skilledPawNormalRight : normalPawRight;
        rightPawImage.SetNativeSize();
        rightPawImage.transform.localPosition = boolean ? pawSkilledPositionR.localPosition : pawNormalPositionR.localPosition;
        rightPawImage.transform.localRotation = boolean ? pawSkilledPositionR.localRotation : pawNormalPositionR.localRotation;
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
            SetSkilledUI(false);
            gameObjectSkilled.SetActive(false);
            UnSkilledEvent();
        }
    }
}
