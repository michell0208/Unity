using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Convert3d _sConvert3D;
    [SerializeField] private StageView _sStageView;

    public List<StageData> StageDatas { set; get; } // 스테이지 데이타 저장용

    private int _currentStageNum = 2;   // 현재 플레이중인 스테이지 번호

    public int CurrentStageNum
    {
        set
        {
            _currentStageNum = value;
        }

        get
        {
            return _currentStageNum;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StageDatas = new List<StageData>();

        CSVFileLoad.OnLoadCSV("StageDatas", StageDatas);

        //
        PlayGame();
    }

    private void PlayGame()
    {

    }

    public void PlayNextStage()
    {
        _sStageView.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}