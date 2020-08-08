using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Human : Player
{
    public Sprite normalSpriteL, attackSpriteL, normalSpriteR, attackSpriteR;
    public RectTransform normalPositionL, normalPositionR, attackPositionL, attackPositionR;
    public Image skillImage, attackImage;
    public Text skillText;
    const float DECREASE_TIME = 3.0f;//減緩時間
    float skillTimer;
    Animator skilledAnimator;
    Vector3 originalSkillTextPosition;

    // Start is called before the first frame update
    void Start()
    {
        type = PlayerType.HUMAN;
        skillTimer = 0.0f;
        skilledAnimator = gameObjectSkilled.GetComponentInChildren<Animator>();
        originalSkillTextPosition = skillText.transform.localPosition;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GAME_STATE.PLAY)
        {
            UpdateSkillValue();
            DetectInput();
            if (isSkilled)
            {
                HandleBeSkilled();
            }
        }
    }

   
    public float GetSkillTimer()
    {
        return skillTimer;
    }

    //設定被使用技能
    public override void SetIsSkilled(bool boolean)
    {
        base.SetIsSkilled(boolean);
        if (boolean)
        {
            skilledAnimator.SetBool("isExit", false);
            skillText.transform.localRotation = Quaternion.identity;
            skillText.transform.localPosition = originalSkillTextPosition;
        }
    }

    //設定使用技能
    public override void SetUseSkill(bool boolean)
    {
        if (boolean)
        {
            audioSource.PlayOneShot(skill);
            skillImage.sprite = attackSpriteR;
            skillImage.SetNativeSize();
            skillImage.rectTransform.localPosition = attackPositionR.localPosition;
        }
        else
        {
            skillImage.sprite = normalSpriteR;
            skillImage.SetNativeSize();
            skillImage.rectTransform.localPosition = normalPositionR.localPosition;
        }
    }

    public override void DetectInput()
    {
        base.DetectInput();
        if (Input.GetKeyDown(input.attack))
        {
            if (isSkilled)
            {
                skillText.transform.DOShakePosition(0.2f, 2, 20);
                skillText.transform.DOShakeRotation(0.2f, 45);
            }
            audioSource.PlayOneShot(isSkilled ? skilled : attack);
            attackImage.sprite = attackSpriteL;
            attackImage.SetNativeSize();
            attackImage.rectTransform.localPosition = attackPositionL.localPosition;
        }
        else if (Input.GetKeyUp(input.attack))
        {
            attackImage.sprite = normalSpriteL;
            attackImage.SetNativeSize();
            attackImage.rectTransform.localPosition = normalPositionL.localPosition;
        }
    }

    //檢查貓咪技能
    void HandleBeSkilled()
    {
        skillTimer += Time.deltaTime;
        if (skillTimer >= DECREASE_TIME)
        {
            skillTimer = 0.0f;
            isSkilled = false;
            skillText.transform.localRotation = Quaternion.identity;
            skillText.transform.localPosition = originalSkillTextPosition;
            gameObjectSkilled.SetActive(false);
            UnSkilledEvent();
        }
        else if(skillTimer >= DECREASE_TIME - 0.25f)//TODO 優化
        {
            skilledAnimator.SetBool("isExit", true);
        }
    }
}
