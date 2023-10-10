using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBullet : MonoBehaviour
{
    /// <summary>
    /// 탄환 Prefab
    /// </summary>
    GameObject bullet;
    /// <summary>
    /// 탄환 관리용 list
    /// </summary>
    List<Bullet> bList;

    /// <summary>
    /// 탄환 리스폰 시간
    /// </summary>
    public float Rtime; 
    /// <summary>
    /// 탄환 생성까지 남은시간
    /// </summary>
    private float Ltime = 0f;

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        bullet = Resources.Load<GameObject>("Prefab/Sphere");
        bList = new List<Bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isOver) // 게임이 끝났는 지
        {
            //시간을 계속 체크해줌
            Ltime += Time.deltaTime;
            if (Ltime >= Rtime) //탄환을 재생성할 시간이 되었을 때
            {
                Ltime -= Rtime;

                // 탄환 생성 좌표값을 필드 내에 한정하여 랜덤으로 산출
                float x = Random.Range(-14f, 14f);
                float z = Random.Range(-14f, 14f);                
                Vector3 RespawnPos;
                RespawnPos = new Vector3(x, 1f, z);

                if (bList.Count == 0) //풀 비었을때
                {
                    GameObject one = Instantiate<GameObject>(bullet);
                    Bullet b = one.GetComponent<Bullet>();
                    b.transform.position = RespawnPos;
                    Vector3 temp = player.transform.position;

                    //만약 탄환과 플레이어의 고저차이가 나는 경우 탄환이 바닥으로 떨어질 수 있기 때문에 Y값을 조정
                    temp.y = RespawnPos.y;
                    //방향벡터 계산
                    Vector3 dir = temp - RespawnPos;
                    b.Shoot(dir.normalized);

                    bList.Add(b);
                }
                else
                {
                    int count = 0;
                    for (int i = 0; i < bList.Count; i++) //풀 안 검색
                    {
                        if (bList[i].gameObject.activeSelf == false) //풀 안에 안 쓰는 오브젝트 있을 때 재사용
                        {
                            bList[i].transform.position = RespawnPos;
                            Vector3 temp = player.transform.position;
                            temp.y = RespawnPos.y;
                            Vector3 dir = temp - RespawnPos;
                            bList[i].Shoot(dir.normalized);
                            bList[i].gameObject.SetActive(true);
                        }
                        else { count++; }
                    }

                    if (count == bList.Count) // 풀안이 다 사용 중 일때 새로 생성
                    {
                        GameObject one = Instantiate<GameObject>(bullet);
                        Bullet b = one.GetComponent<Bullet>();
                        b.transform.position = RespawnPos;
                        Vector3 temp = player.transform.position;
                        temp.y = RespawnPos.y;
                        Vector3 dir = temp - RespawnPos;
                        b.Shoot(dir.normalized);

                        bList.Add(b);

                    }
                }
            }

        }
        else
        {
        }
    }
}
