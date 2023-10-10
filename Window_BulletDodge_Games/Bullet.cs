using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //�߻� �ӵ�
    public float speed;
    //�߻� ����
    Vector3 vector;


    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// źȯ �߻� �Լ� vec�� ����
    /// </summary>
    /// <param name="vec"></param>
    public void Shoot(Vector3 vec)
    {
        vector = vec;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isOver) //������ ���� ������ Ȯ��
        {
            //�������� źȯ�� �ʵ� �ٱ����� ������ �� Ȯ�� �� �Ѿ�ٸ� ��Ȱ��ȭ
            if (Vector3.Distance(Vector3.zero, transform.position) <= 25f)
            {
                transform.Translate(vector * Time.deltaTime * speed);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }
        else //������ ���� ���
        {
            transform.gameObject.SetActive(false);
        }
    }
}
