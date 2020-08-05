using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Player
{
    const float DECREASE_TIME = 3.0f;//減緩時間
    float skillTimer;

    // Start is called before the first frame update
    void Start()
    {
        type = PlayerType.HUMAN;
        skillTimer = 0.0f;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSkillValue();
        DetectInput();
        if (isSkilled)
        {
            HandleBeSkilled();
        }
    }


    public float GetSkillTimer()
    {
        return skillTimer;
    }

    public override void SetUseSkill(bool boolean)
    {
        Debug.Log("human SetUseSkill overriled" + boolean);
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
            UnSkilledEvent();
        }
    }
}
