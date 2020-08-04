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
    public Image attackImage, skillImage;
    public Sprite pressKey, normalKey, disableKey;

    const float ATTACK_VALUE = 1.0f;//攻擊力
    const float DECREASE_ATK_RATIO = 0.1f;//減緩攻擊比例
    const float MAX_SKILL_VALUE = 100.0f;//技能最大值
    const float ADD_SKILL_RATIO = 100.0f;//累積技能比例
  

    float skillValue, addSkillValue;
    protected bool isSkilled;
   
    public event EventHandler<AttackEventArgs> attackEvent;
    public event EventHandler<SkillEventArgs> skillEvent;

    //public
    //設定累積技能速度
    public void SetAddSkillValue(float value)
    {
        addSkillValue = value;
    }

    //設定被使用技能
    public void SetIsSkilled(bool boolean)
    {
        isSkilled = boolean;
    }

    //取得是否被使用技能
    public bool GetIsSkilled()
    {
        return isSkilled;
    }

    //private
    protected void InitVariable()
    {
        skillValue = addSkillValue = 0.0f;
        isSkilled = false;
        skillImage.sprite = disableKey;
    }

    //更新技能值
    protected void UpdateSkillValue()
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
}
