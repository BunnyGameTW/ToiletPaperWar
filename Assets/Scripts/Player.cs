using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class AttackEventArgs : EventArgs
{
    public float attackValue;
    public PlayerType playerType;
    public AttackEventArgs(float v, PlayerType t)
    {
        attackValue = v;
        playerType = t;
    }
}

public class SkillEventArgs : EventArgs
{
    public PlayerType playerType;
    public SkillEventArgs(PlayerType t)
    {
        playerType = t;
    }
}

public class Player : MonoBehaviour
{
   
    protected PlayerType type;
    [Serializable]
    public struct PlayerInput
    {
        public KeyCode attack;
        public KeyCode skill;
    }
    public PlayerInput input;
    public Image attackKeyImage, skillKeyImage;
    public Sprite pressKey, normalKey, disableKey;
    public Image toiletPaperRollImage, toiletPaperImage;
    public GameObject toiletPaperParent, gameObjectSkilled;
    public Sprite emptyToiletPaperRoll;

    const float ATTACK_VALUE = 1.0f;//攻擊力
    const float DECREASE_ATK_RATIO = 0.1f;//減緩攻擊比例
    const float MAX_SKILL_VALUE = 100.0f;//技能最大值
    const float ADD_SKILL_RATIO = 100.0f;//累積技能比例
    const float TOILET_PAPER_OFFSET_Y = 250.0f;
    const float MIN_PAPER_ROLL_SCALE = 0.5f;
    const float MAX_PAPER_ROLL_SCALE = 1.0f;

    float skillValue, addSkillValue;
    protected bool isSkilled, isGameOver;
    RectTransform toiletPaperParentTransform;
    public event EventHandler<AttackEventArgs> attackEvent;
    public event EventHandler<SkillEventArgs> skillEvent;
    public event EventHandler<SkillEventArgs> unskilledEvent;

    //public
    //設定累積技能速度
    public void SetAddSkillValue(float value)
    {
        addSkillValue = value;
    }

    //設定被使用技能
    public virtual void SetIsSkilled(bool boolean)
    {
        isSkilled = boolean;
        gameObjectSkilled.SetActive(true);
    }

    //取得是否被使用技能
    public bool GetIsSkilled()
    {
        return isSkilled;
    }
  
    //更新衛生紙動態
    public void UpdateToiletPaper(float toiletPaperValue)
    {
        float y = Mathf.Lerp(
            toiletPaperParentTransform.localPosition.y, 
            -toiletPaperValue / GameManager.ATTACK_RATIO * TOILET_PAPER_OFFSET_Y, 
            0.5f
        );
        toiletPaperParentTransform.localPosition = new Vector3(0, y, 0);

        if (!isGameOver)
        {
            float scaleY = Mathf.Lerp(
                toiletPaperRollImage.rectTransform.localScale.y,
                MIN_PAPER_ROLL_SCALE + (MAX_PAPER_ROLL_SCALE - MIN_PAPER_ROLL_SCALE) * (GameManager.MAX_TOILET_VALUE - toiletPaperValue) / GameManager.MAX_TOILET_VALUE,
                0.5f
            );
            toiletPaperRollImage.rectTransform.localScale = new Vector3(
                toiletPaperRollImage.rectTransform.localScale.x,
                scaleY,
                toiletPaperRollImage.rectTransform.localScale.z
            );
        }
        if (toiletPaperValue == GameManager.MAX_TOILET_VALUE)
        {
            toiletPaperRollImage.rectTransform.localScale = new Vector3(
                toiletPaperRollImage.rectTransform.localScale.x,
                MAX_PAPER_ROLL_SCALE,
                toiletPaperRollImage.rectTransform.localScale.z
            );
            toiletPaperRollImage.sprite = emptyToiletPaperRoll;
            toiletPaperRollImage.SetNativeSize();
        }
    }

    public void SetGameOver()
    {
        isGameOver = true;
    }

    //private
    protected void Init()
    {
        skillValue = addSkillValue = 0.0f;
        isSkilled = isGameOver = false;
        skillKeyImage.sprite = disableKey;
        toiletPaperParentTransform = toiletPaperParent.GetComponent<RectTransform>();
        InitToiletPapers();
    }

    //更新技能值
    protected void UpdateSkillValue()
    {
        skillValue += addSkillValue * ADD_SKILL_RATIO * Time.deltaTime;
        if (skillValue >= MAX_SKILL_VALUE)
        {
            skillValue = MAX_SKILL_VALUE;
            skillKeyImage.sprite = normalKey;
        }
        skillKeyImage.fillAmount = skillValue / MAX_SKILL_VALUE;
    }

    //偵測輸入
    protected void DetectInput()
    {
        CheckAttack();
        CheckSkill();
    }

    void CheckAttack()
    {
        if (Input.GetKeyDown(input.attack))
        {
            float attackValue = GetAttackValue();
            if (attackValue > 0)
            {
                attackEvent.Invoke(this, new AttackEventArgs(attackValue, type));
            }
            attackKeyImage.sprite = pressKey;
        }
        else if (Input.GetKeyUp(input.attack))
        {
            attackKeyImage.sprite = normalKey;
        }
    }

    void CheckSkill()
    {
        if (Input.GetKeyDown(input.skill) && skillValue == MAX_SKILL_VALUE)
        {
            
            SetUseSkill(true);
            skillKeyImage.sprite = pressKey;
            skillValue = 0.0f;
            skillEvent.Invoke(this, new SkillEventArgs(type));
        }
        else if (Input.GetKeyUp(input.skill) && skillValue != MAX_SKILL_VALUE)
        {
            skillKeyImage.sprite = disableKey;
        }
    }

    //取得攻擊力
    float GetAttackValue()
    {
        if (isSkilled)
        {
            return type == PlayerType.CAT ? 0.0f : ATTACK_VALUE * DECREASE_ATK_RATIO;
        }
        return ATTACK_VALUE;
    }

    //設定使用技能
    public virtual void SetUseSkill(bool boolean)
    {
    }

    //被使用技能結束事件
    protected void UnSkilledEvent()
    {
        unskilledEvent.Invoke(this, new SkillEventArgs(type));
    }

    void InitToiletPapers()//先設預設攻擊力為一張衛生紙長度250 unit
    {
        for (int i = 0; i < Mathf.Floor(GameManager.MAX_TOILET_VALUE / GameManager.ATTACK_RATIO); i++)
        {
            RectTransform rectTransform = Instantiate(toiletPaperImage, toiletPaperParent.transform).rectTransform;
            rectTransform.SetAsFirstSibling();
            rectTransform.localPosition = toiletPaperImage.rectTransform.localPosition + new Vector3(0.0f, TOILET_PAPER_OFFSET_Y * (i + 1), 0.0f);
        }
    }
}
