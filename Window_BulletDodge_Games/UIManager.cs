using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI retryMessage;
    public TextMeshProUGUI score;

    public Image hp;
    

    // Start is called before the first frame update
    void Start()
    {
        //리트라이시의 메시지 및 점수표기 비활성화
        retryMessage.gameObject.SetActive(false);
        score.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        //현재 표시중이던 캐릭터의 hp와 현재 시간 UI 표시 비활성화
        hp.transform.parent.gameObject.SetActive(false);
        time.gameObject.SetActive(false);


        //리트라이시 표기할 메시지 및 점수 표시
        retryMessage.gameObject.SetActive(true);
        score.text = $"Score : {GameManager.Instance.Uptime.ToString("F2")} " +
            $"\n Best Score : {GameManager.BestScore.ToString("F2")}";
        score.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        //리트라이시 나타날 UI와 게임 진행시 나타날 UI의 초기화
        retryMessage.gameObject.SetActive(false);
        score.gameObject.SetActive(false);

        hp.fillAmount = 1f;
        hp.transform.parent.gameObject.SetActive(true);
        time.gameObject.SetActive(true);

    }


    // Update is called once per frame
    void Update()
    {
        time.text = $"Time : {GameManager.Instance.Uptime.ToString("F2")}";

        //hp바의 업데이트
        hp.fillAmount = GameManager.HP / GameManager.MaxHP;
    }
}
