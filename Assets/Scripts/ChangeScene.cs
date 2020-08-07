using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    GameObject gameObjectFade;
    float fadeTime = 1.0f;//TODO 照順序打開
    bool isStartFade, isStartFadeOut;
    float timer;
    string sceneName;

    void Awake()
    {
        Debug.Log("Awake");
        GameObject[] objs = GameObject.FindGameObjectsWithTag("changeScene");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        objs = GameObject.FindGameObjectsWithTag("fade");
        if (objs.Length > 1)
        {
            Destroy(objs[1].gameObject);
        }
        DontDestroyOnLoad(objs[0]);
        gameObjectFade = objs[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        isStartFadeOut = isStartFade = false;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartFade)
        {
            timer += Time.deltaTime;
            if (timer >= fadeTime)
            {
                isStartFade = false;
                timer = 0.0f;
                SceneManager.LoadScene(sceneName);
                for (int i = 0; i < gameObjectFade.transform.childCount; i++)
                {
                    gameObjectFade.transform.GetChild(i).gameObject.GetComponent<Animation>().Play("fadeOut");
                }
                isStartFadeOut = true;
            }
        }
        else if (isStartFadeOut)
        {
            timer += Time.deltaTime;
            if (timer >= fadeTime)
            {
                isStartFadeOut = false;
                timer = 0.0f;
                for (int i = 0; i < gameObjectFade.transform.childCount; i++)
                {
                    gameObjectFade.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
    public void Change(string name)
    {
        sceneName = name;
        for (int i = 0; i < gameObjectFade.transform.childCount; i++)
        {
            gameObjectFade.transform.GetChild(i).gameObject.SetActive(true);
            gameObjectFade.transform.GetChild(i).gameObject.GetComponent<Animation>().Play("fadeIn");
        }
        isStartFade = true;
    }
}
