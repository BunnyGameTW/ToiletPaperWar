using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    CAT, HUMAN
}



public class GameManager : MonoBehaviour
{
    PlayerType winner;
    public float gameTime;
    public Text timeText;
    public Image leftBarImage, rightBarImage;
    public Player cat, human;

    float timer;
    float toiletValue;
    const float toiletPaperRatio = 0.5f;
    const float MAX_TOILET_VALUE = 100.0f;
    const float MIN_TOILET_VALUE = 0.0f;
    bool isGameOver;
    
    // Start is called before the first frame update
    void Start()
    {
        toiletValue = toiletPaperRatio * MAX_TOILET_VALUE;
        isGameOver = false;
        cat.attackEvent += OnPlayerAttack;
        human.attackEvent += OnPlayerAttack;
        cat.skillEvent += OnPlayerSkill;
        human.skillEvent += OnPlayerSkill;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            float fillValue = toiletValue / MAX_TOILET_VALUE;
            cat.SetAddSkillValue(1.0f - fillValue);
            human.SetAddSkillValue(fillValue);

            RefreshTimer();
            UpdateUI();
        }
        else
        {
            //TODO 顯示贏家
            Debug.Log("winner->" + winner.ToString());
        }
    }

    void RefreshTimer()
    {
        gameTime -= Time.deltaTime; 
        if(gameTime <= 0.0f)
        {
            gameTime = 0.0f;
            isGameOver = true;
            winner = toiletValue >= MAX_TOILET_VALUE / 2 ? PlayerType.CAT : PlayerType.HUMAN;
        }
    }

    const float ATTACK_RATIO = 3.0f;

    //event
    void OnPlayerAttack(object sender, AttackEventArgs param)
    {
        float minus = param.playerType == PlayerType.CAT ? 1 : -1;
        toiletValue += minus * param.attackValue * ATTACK_RATIO;
        if (toiletValue >= MAX_TOILET_VALUE)
        {
            toiletValue = MAX_TOILET_VALUE;
            winner = PlayerType.CAT;
            isGameOver = true;
        }
        else if (toiletValue <= MIN_TOILET_VALUE)
        {
            toiletValue = MIN_TOILET_VALUE;
            winner = PlayerType.HUMAN;
            isGameOver = true;
        }
    }

    void OnPlayerSkill(object sender, SkillEventArgs param)
    {
        if(param.playerType == PlayerType.CAT)
        {
            human.SetIsSkilled(true);
        }
        else
        {
            cat.SetIsSkilled(true);
        }
    }

    void UpdateUI()
    {
        timeText.text = Mathf.Ceil(gameTime).ToString();
        float fillValue = toiletValue / MAX_TOILET_VALUE;
        leftBarImage.fillAmount = fillValue;
        rightBarImage.fillAmount = 1.0f - fillValue;
    }
        
}
