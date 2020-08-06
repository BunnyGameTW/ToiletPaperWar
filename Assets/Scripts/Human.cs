using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Player
{
    const float DECREASE_TIME = 3.0f;//減緩時間
    float skillTimer;
    Animator skilledAnimator;
    // Start is called before the first frame update
    void Start()
    {
        type = PlayerType.HUMAN;
        skillTimer = 0.0f;
        skilledAnimator = gameObjectSkilled.GetComponentInChildren<Animator>();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
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
        if(boolean) skilledAnimator.SetBool("isExit", false);
    }

    //設定使用技能
    public override void SetUseSkill(bool boolean)
    {
        if (boolean)
        {
           //TODO 使用技能時人類的顯示
        }
        else
        {
           
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
            gameObjectSkilled.SetActive(false);
            UnSkilledEvent();
        }
        else if(skillTimer >= DECREASE_TIME - 0.25f)//TODO 優化
        {
            skilledAnimator.SetBool("isExit", true);
        }
    }
}
