using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCell : MonoBehaviour
{
    [SerializeField] private Image sStageImage;
    [SerializeField] private Text sText;

    [HideInInspector] public GameObject stageView;
    [HideInInspector] public Convert3d convert3D;

    [HideInInspector] public string categoryName;
    [HideInInspector] public string imageName;



    public void SetImage(Sprite sprite)
    {
        sStageImage.sprite = sprite;
    }

    public void SetText(string text)
    {
        sText.text = text;
    }

    public void OnClickCell()
    {

        convert3D.enabled = true;
        convert3D.PlayGame(categoryName, imageName);
        stageView.GetComponent<StageView>().ShowGameView();

    }


}
