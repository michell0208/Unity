using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBullet : MonoBehaviour
{
    /// <summary>
    /// źȯ Prefab
    /// </summary>
    GameObject bullet;
    /// <summary>
    /// źȯ ������ list
    /// </summary>
    List<Bullet> bList;

    /// <summary>
    /// źȯ ������ �ð�
    /// </summary>
    public float Rtime; 
    /// <summary>
    /// źȯ �������� �����ð�
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
        if(!GameManager.isOver) // ������ ������ ��
        {
            //�ð��� ��� üũ����
            Ltime += Time.deltaTime;
            if (Ltime >= Rtime) //źȯ�� ������� �ð��� �Ǿ��� ��
            {
                Ltime -= Rtime;

                // źȯ ���� ��ǥ���� �ʵ� ���� �����Ͽ� �������� ����
                float x = Random.Range(-14f, 14f);
                float z = Random.Range(-14f, 14f);                
                Vector3 RespawnPos;
                RespawnPos = new Vector3(x, 1f, z);

                if (bList.Count == 0) //Ǯ �������
                {
                    GameObject one = Instantiate<GameObject>(bullet);
                    Bullet b = one.GetComponent<Bullet>();
                    b.transform.position = RespawnPos;
                    Vector3 temp = player.transform.position;

                    //���� źȯ�� �÷��̾��� �������̰� ���� ��� źȯ�� �ٴ����� ������ �� �ֱ� ������ Y���� ����
                    temp.y = RespawnPos.y;
                    //���⺤�� ���
                    Vector3 dir = temp - RespawnPos;
                    b.Shoot(dir.normalized);

                    bList.Add(b);
                }
                else
                {
                    int count = 0;
                    for (int i = 0; i < bList.Count; i++) //Ǯ �� �˻�
                    {
                        if (bList[i].gameObject.activeSelf == false) //Ǯ �ȿ� �� ���� ������Ʈ ���� �� ����
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

                    if (count == bList.Count) // Ǯ���� �� ��� �� �϶� ���� ����
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
