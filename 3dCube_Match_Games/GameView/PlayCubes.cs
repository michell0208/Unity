using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCubes : MonoBehaviour
{
    [SerializeField] private StageManager _sStageManager;
    public void ClearAnimationEnd()
    {
        _sStageManager.PlayNextStage();
    }
}
