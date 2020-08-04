using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Player
{
    int canCounter;
    const int CAN_NUMBER = 10;//罐頭連打數

    // Start is called before the first frame update
    void Start()
    {
        type = PlayerType.CAT;
        InitVariable();
        canCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSkillValue();
        DetectInput();
        if (Input.GetKeyDown(input.attack) && isSkilled)
        {
            HandleBeSkilled();
        }
    }

    //處理被使用技能
    void HandleBeSkilled()
    {
        canCounter++;
        if (canCounter == CAN_NUMBER)
        {
            canCounter = 0;
            isSkilled = false;
        }
    }
}
