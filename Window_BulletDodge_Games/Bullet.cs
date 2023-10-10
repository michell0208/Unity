using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //발사 속도
    public float speed;
    //발사 방향
    Vector3 vector;


    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// 탄환 발사 함수 vec은 방향
    /// </summary>
    /// <param name="vec"></param>
    public void Shoot(Vector3 vec)
    {
        vector = vec;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isOver) //게임이 진행 중인지 확인
        {
            //원점에서 탄환이 필드 바깥까지 나갔는 지 확인 후 넘어갔다면 비활성화
            if (Vector3.Distance(Vector3.zero, transform.position) <= 25f)
            {
                transform.Translate(vector * Time.deltaTime * speed);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }
        else //게임이 끝난 경우
        {
            transform.gameObject.SetActive(false);
        }
    }
}
