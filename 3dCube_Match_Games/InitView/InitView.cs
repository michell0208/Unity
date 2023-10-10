using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitView : MonoBehaviour
{
    [SerializeField] private GameObject _StageView;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnClickStartbutton()
    {
        this.gameObject.SetActive(false);
        _StageView.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
