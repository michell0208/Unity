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
        //��Ʈ���̽��� �޽��� �� ����ǥ�� ��Ȱ��ȭ
        retryMessage.gameObject.SetActive(false);
        score.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        //���� ǥ�����̴� ĳ������ hp�� ���� �ð� UI ǥ�� ��Ȱ��ȭ
        hp.transform.parent.gameObject.SetActive(false);
        time.gameObject.SetActive(false);


        //��Ʈ���̽� ǥ���� �޽��� �� ���� ǥ��
        retryMessage.gameObject.SetActive(true);
        score.text = $"Score : {GameManager.Instance.Uptime.ToString("F2")} " +
            $"\n Best Score : {GameManager.BestScore.ToString("F2")}";
        score.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        //��Ʈ���̽� ��Ÿ�� UI�� ���� ����� ��Ÿ�� UI�� �ʱ�ȭ
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

        //hp���� ������Ʈ
        hp.fillAmount = GameManager.HP / GameManager.MaxHP;
    }
}
