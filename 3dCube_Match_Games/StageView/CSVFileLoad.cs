using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData
{
    public int StageNum { set; get; } //스테이지 번호
    public string CategoryName { set; get; } //폴더명

    public string ImageName { set; get; }//이미지 파일명

    public int DropColorCount { set; get; }//드롭블럭 갯수
    public int EndingAnimationNum { set; get; }//스테이지 클리어시 보여줄 애니메이션 번호

    public StageData() { }
    public StageData(int Stagenum, string Categoryname, string Imagename, int Dropcolorcount, int endinganimationnum)
    {
        StageNum = Stagenum;
        CategoryName = Categoryname;
        ImageName = Imagename;
        DropColorCount = Dropcolorcount;
        EndingAnimationNum = endinganimationnum;
    }
}


/// <summary>
/// Resources 폴더의 csv파일을 로드한다.
/// </summary>
public class CSVFileLoad
{
    public static void OnLoadCSV(string filename, List<StageData> stageDatas)
    {
        string file_path = "CSV/";
        file_path = string.Concat(file_path, filename);

        TextAsset ta = Resources.Load<TextAsset>(file_path);

        OnLoadTextAsset(ta.text, stageDatas);

        Resources.UnloadAsset(ta);

        ta = null;
    }

    static public void OnLoadTextAsset(string data, List<StageData> stageDatas)
    {
        string[] str_lines = data.Split('\n');

        //첫 라인(0번째 줄)은 설명이라 제외
        for(int i = 1; i < str_lines.Length - 1; i++)
        {
            string[] values = str_lines[i].Split(',');

            StageData sd = new StageData();

            sd.StageNum = int.Parse(values[0]);
            sd.CategoryName = values[1];
            sd.ImageName = values[2];
            sd.DropColorCount = int.Parse(values[3]);
            sd.EndingAnimationNum = int.Parse(values[4]);

            stageDatas.Add(sd);
        }

    }

}
