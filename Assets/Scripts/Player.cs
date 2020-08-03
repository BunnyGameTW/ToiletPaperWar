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
   
    public PlayerType type;
    [Serializable]
    public struct PlayerInput
    {
        public KeyCode attack;
        public KeyCode skill;
    }
    public PlayerInput input;
    public Image attackImage, skillImage;
    public Sprite pressKey, normalKey, disableKey;

    const float ATTACK_VALUE = 1.0f;//攻擊力
    const float DECREASE_ATK_RATIO = 0.1f;//減緩攻擊比例
    const int CAN_NUMBER = 10;//罐頭連打數
    const float MAX_SKILL_VALUE = 100.0f;//技能最大值
    const float ADD_SKILL_RATIO = 10.0f;//累積技能比例
    const float DECREASE_TIME = 3.0f;//減緩時間

    float skillValue, addSkillValue;
    bool isSkilled;
    float skillTimer;
    int canCounter;
    public event EventHandler<AttackEventArgs> attackEvent;
    public event EventHandler<SkillEventArgs> skillEvent;

    // Start is called before the first frame update
    void Start()
    {
        skillValue = addSkillValue = 0.0f;
        skillTimer = 0.0f;
        canCounter = 0;
        isSkilled = false;
        skillImage.sprite = disableKey;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCatSkill();
        UpdateSkillValue();
        DetectInput();
    }
   
    //public
    //設定累積技能速度
    public void SetAddSkillValue(float value)
    {
        addSkillValue = value;
    }

    //設定被攻擊
    public void SetIsSkilled(bool boolean)
    {
        isSkilled = boolean;
    }

    //private
    //更新技能值
    void UpdateSkillValue()
    {
        skillValue += addSkillValue * ADD_SKILL_RATIO * Time.deltaTime;
        if (skillValue >= MAX_SKILL_VALUE)
        {
            skillValue = MAX_SKILL_VALUE;
            skillImage.sprite = normalKey;
        }
        skillImage.fillAmount = skillValue / MAX_SKILL_VALUE;
    }

    //偵測輸入
    void DetectInput()
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
            CheckHumanSkill();
            attackImage.sprite = pressKey;
        }
        else if (Input.GetKeyUp(input.attack))
        {
            attackImage.sprite = normalKey;
        }
    }

    void CheckSkill()
    {
        if (Input.GetKeyDown(input.skill) && skillValue == MAX_SKILL_VALUE)
        {
            skillImage.sprite = pressKey;
            skillValue = 0.0f;
            skillEvent.Invoke(this, new SkillEventArgs(type));
        }
        else if (Input.GetKeyUp(input.skill) && skillValue != MAX_SKILL_VALUE)
        {
            skillImage.sprite = disableKey;
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

    //檢查人類技能
    void CheckHumanSkill()
    {
        if (isSkilled && type == PlayerType.CAT)
        {
            canCounter++;
            if (canCounter == CAN_NUMBER)
            {
                canCounter = 0;
                isSkilled = false;
            }
        }
    }

    //檢查貓咪技能
    void CheckCatSkill()
    {
        if (isSkilled && type == PlayerType.HUMAN)
        {
            skillTimer += Time.deltaTime;
            if (skillTimer >= DECREASE_TIME)
            {
                skillTimer = 0.0f;
                isSkilled = false;
            }
        }
    }
}
