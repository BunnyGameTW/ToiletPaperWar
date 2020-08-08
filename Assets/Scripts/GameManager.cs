using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    public Color humanSkilledBarColor, humanNormalBarColor;
    public AudioSource audioSource;
    public AudioClip tick, end;
    public GameObject winGameObject;

    float timer;
    float toiletValue;
    bool hasPlayEndAlert;

    const float TOILET_PAPER_RATIO = 0.5f;//衛生紙量
    const float MIN_TOILET_VALUE = 0.0f;//衛生紙最小值
    public const float MAX_TOILET_VALUE = 100.0f;//衛生紙最大值
    public const float ATTACK_RATIO = 3.0f;//攻擊比例
    enum GAME_STATE
    {
        PREPARE, PLAY, OVER, SHOW_WINNER, FADE
    }
    GAME_STATE gameState;

    // Start is called before the first frame update
    void Start()
    {
        toiletValue = TOILET_PAPER_RATIO * MAX_TOILET_VALUE;
        gameState = GAME_STATE.PREPARE;
        hasPlayEndAlert = false;
        cat.attackEvent += OnPlayerAttack;
        human.attackEvent += OnPlayerAttack;
        cat.skillEvent += OnPlayerSkill;
        human.skillEvent += OnPlayerSkill;
        cat.unskilledEvent += OnPlayerUnSkilled;
        human.unskilledEvent += OnPlayerUnSkilled;
    }

    const float SHOW_WINNER_TIME = 3.0f;
    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GAME_STATE.PREPARE:
                //TODO 倒數 或一些表演
                gameState = GAME_STATE.PLAY;
                break;
            case GAME_STATE.PLAY:
                float fillValue = toiletValue / MAX_TOILET_VALUE;
                cat.SetAddSkillValue(1.0f - fillValue);
                human.SetAddSkillValue(fillValue);
                RefreshTimer();
                UpdateUI();
                break;
            case GAME_STATE.OVER:
                gameState = GAME_STATE.SHOW_WINNER;
                audioSource.PlayOneShot(end);
                timer = 0.0f;
                winGameObject.SetActive(true);
                winGameObject.transform.localPosition =
                    winner == PlayerType.CAT ?
                    cat.gameObject.transform.localPosition : human.gameObject.transform.localPosition;
                break;
            case GAME_STATE.SHOW_WINNER:
                UpdateToiletPaper();
                UpdateBarColor();
                timer += Time.deltaTime;
                if(timer >= SHOW_WINNER_TIME && GameObject.FindGameObjectWithTag("changeScene") != null)
                {
                    GameObject.FindGameObjectWithTag("changeScene").GetComponent<ChangeScene>().Change("Login");
                    gameState = GAME_STATE.FADE;
                }
                break;
            case GAME_STATE.FADE:
                break;
            default:
                break;
        }
    }

    void RefreshTimer()
    {
        gameTime -= Time.deltaTime; 
        if(gameTime <= 0.0f)
        {
            gameTime = 0.0f;
            SetGameOver();
            winner = toiletValue >= MAX_TOILET_VALUE / 2 ? PlayerType.CAT : PlayerType.HUMAN;
        }
        else if (gameTime <= 5.0f && !hasPlayEndAlert)
        {
            hasPlayEndAlert = true;
            audioSource.PlayOneShot(tick);
        }
    }

    void SetGameOver()
    {
        gameState = GAME_STATE.OVER;
        cat.SetGameOver();
        human.SetGameOver();
    }

    void UpdateUI()
    {
        timeText.text = Mathf.Ceil(gameTime).ToString();
        float fillValue = toiletValue / MAX_TOILET_VALUE;
        leftBarImage.fillAmount = DOVirtual.EasedValue(leftBarImage.fillAmount, fillValue, 0.5f, Ease.InOutBack);
        rightBarImage.fillAmount = DOVirtual.EasedValue(rightBarImage.fillAmount, 1.0f - fillValue, 0.5f, Ease.InOutBack);//感覺沒效
        UpdateToiletPaper();
        UpdateBarColor();
    }

    void UpdateToiletPaper()
    {
        human.UpdateToiletPaper(toiletValue);
        cat.UpdateToiletPaper(toiletValue);
    }

    void UpdateBarColor()
    {
        if (human.GetIsSkilled())
        {
            rightBarImage.color = Color.Lerp(Color.white, humanSkilledBarColor, Mathf.PingPong(Time.time, 1));
        }
        else
        {
            rightBarImage.color = humanNormalBarColor;
        }
    }

    //event
    void OnPlayerAttack(object sender, AttackEventArgs param)
    {
        float minus = param.playerType == PlayerType.CAT ? 1 : -1;
        toiletValue += minus * param.attackValue * ATTACK_RATIO;
        if (toiletValue >= MAX_TOILET_VALUE)
        {
            toiletValue = MAX_TOILET_VALUE;
            winner = PlayerType.CAT;
            SetGameOver();
          
        }
        else if (toiletValue <= MIN_TOILET_VALUE)
        {
            toiletValue = MIN_TOILET_VALUE;
            winner = PlayerType.HUMAN;
            SetGameOver();
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

    void OnPlayerUnSkilled(object sender, SkillEventArgs param)
    {
        if (param.playerType == PlayerType.CAT)
        {
            human.SetUseSkill(false);
        }
        else
        {
            cat.SetUseSkill(false);
        }
    }
}
