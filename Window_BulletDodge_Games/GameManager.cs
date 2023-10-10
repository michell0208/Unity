using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //외부연결
    public Character player;
    public PlayerAlongMove cam;
    public PlayerAlongMove hp_ui;
    public static float BestScore = 0f; //최고 기록 저장용
    public UIManager ui;

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    private static float _MaxHP = 15f;
    /// <summary>
    /// 캐릭터 초기 위치
    /// </summary>
    private static Vector3 Player_Pos = new Vector3(0f, 0.5f, 0f);
    //현재 HP 초기화
    private static float hp = _MaxHP;

    /// <summary>
    /// 캐릭터의 현재 HP
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
    /// 캐릭터의 초기 HP
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
    /// 게임의 진행 시간
    /// </summary>
    public float Uptime { get { return uptime; } }

    private static bool IsOver = false;
    /// <summary>
    /// 게임 진행 여부
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
        //게임 진행 시간 초기화
        uptime = 0f;

        //플레이어의 위치 및 카메라, HP의 초기화
        player.transform.position = Player_Pos;
        cam.SetPosition();
        hp_ui.SetPosition();

        player.ani.Respawn();
        hp = MaxHP;
        ui.GameStart();

        //게임 시작 처리
        IsOver = false;
    }

    public void GameOver()
    {
        //최고 기록을 경신한 경우
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
            //게임 진행 시간 누적 합산
            uptime += Time.deltaTime;
        }
        else
        {
            //게임오버 시 다시 시작할 것인지, 종료할 것인지 확인
            if(Input.GetKeyDown(KeyCode.R))
            {
                StartNewGame();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        if(hp <= 0) //HP가 0보다 작아지는 경우
        {
            GameOver();
        }
    }

    // Start is called before the first frame update

    // Update is called once per frame
}
