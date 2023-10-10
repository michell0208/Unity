using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageView : MonoBehaviour
{
    [SerializeField] private GameObject sStageCellPrefab; //스테이지셀 prefab
    [SerializeField] private StageManager sStageManager;
    [SerializeField] private GameObject sContents; //스테이지 셀의 부모 오브젝트
    [SerializeField] private Convert3d sConvert3d;
    [SerializeField] private GameObject sGameView;

    // Start is called before the first frame update
    void Start()
    {
        MakeStageCells();
    }

    private void MakeStageCells()
    {

        //로드한 csv파일의 데이터로 스크롤 뷰에 스테이지 셀을 생성
        foreach(StageData sd in sStageManager.StageDatas)
        {
            GameObject StageCell = Instantiate(sStageCellPrefab);

            string path = string.Format("StageImages/{0}/{1}", sd.CategoryName, sd.ImageName);
            Sprite sprite = Resources.Load<Sprite>(path); //이미지 파일 로드

            var stageCellcom = StageCell.GetComponent<StageCell>();

            stageCellcom.SetImage(sprite);
            stageCellcom.SetText(sd.ImageName);
            stageCellcom.categoryName = sd.CategoryName;
            stageCellcom.imageName = sd.ImageName;
            stageCellcom.stageView = this.gameObject;
            stageCellcom.convert3D = sConvert3d;

            StageCell.transform.SetParent(sContents.transform);

        }
        //기준점을 기준으로 한 좌표.(얼마나 멀어져 있는가)
        Vector2 pos = sContents.GetComponent<RectTransform>().anchoredPosition;
        int count = sStageManager.StageDatas.Count;

        float ypos = count / 2 * 250;
        sContents.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, -ypos);
    }

    public void ShowGameView()
    {
        this.gameObject.SetActive(false);
        sGameView.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
