using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //�ܺο���
    public Character player;
    public PlayerAlongMove cam;
    public PlayerAlongMove hp_ui;
    public static float BestScore = 0f; //�ְ� ��� �����
    public UIManager ui;

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    private static float _MaxHP = 15f;
    /// <summary>
    /// ĳ���� �ʱ� ��ġ
    /// </summary>
    private static Vector3 Player_Pos = new Vector3(0f, 0.5f, 0f);
    //���� HP �ʱ�ȭ
    private static float hp = _MaxHP;

    /// <summary>
    /// ĳ������ ���� HP
    /// </summary>
    public static float HP 
    { 
        get 
        { return hp; }
        set
        {
            if (hp >= 0)
            {
                hp = value;
            }
            else
            {
                hp = 0;
            }
        }
    }

    /// <summary>
    /// ĳ������ �ʱ� HP
    /// </summary>
    public static float MaxHP
    {
        get
        { return _MaxHP; }
        set
        {
            if (_MaxHP >= 0)
            {
                _MaxHP = value;
            }
            else
            {
                _MaxHP = 0;
            }
        }

    }

    private float uptime = 0f;
    /// <summary>
    /// ������ ���� �ð�
    /// </summary>
    public float Uptime { get { return uptime; } }

    private static bool IsOver = false;
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public static bool isOver { get { return IsOver; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    public void StartNewGame()
    {
        //���� ���� �ð� �ʱ�ȭ
        uptime = 0f;

        //�÷��̾��� ��ġ �� ī�޶�, HP�� �ʱ�ȭ
        player.transform.position = Player_Pos;
        cam.SetPosition();
        hp_ui.SetPosition();

        player.ani.Respawn();
        hp = MaxHP;
        ui.GameStart();

        //���� ���� ó��
        IsOver = false;
    }

    public void GameOver()
    {
        //�ְ� ����� ����� ���
        if (uptime > BestScore)
            BestScore = uptime;

        ui.GameOver();
        player.ani.Die();

        IsOver = true;
    }

    private void Update()
    {
        if(!IsOver)
        {
            //���� ���� �ð� ���� �ջ�
            uptime += Time.deltaTime;
        }
        else
        {
            //���ӿ��� �� �ٽ� ������ ������, ������ ������ Ȯ��
            if(Input.GetKeyDown(KeyCode.R))
            {
                StartNewGame();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        if(hp <= 0) //HP�� 0���� �۾����� ���
        {
            GameOver();
        }
    }

    // Start is called before the first frame update

    // Update is called once per frame
}
