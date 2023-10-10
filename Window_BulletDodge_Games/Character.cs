using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    /// <summary>
    /// �̵��ӵ�
    /// </summary>
    public float Speed = 1f;
    /// <summary>
    /// Ÿ�� �����ð�, �������� źȯ�� ������ �°ų�, �ǵ�ġ �ʰ� �������� źȯ�� ���� ���� ����
    /// </summary>
    public float hitDuration;
    /// <summary>
    /// ���� ���� �ð�
    /// </summary>
    private float timeLeft = 0f;
    /// <summary>
    /// �¾Ҵ� �� ����, �����ð� ������ �°��ִ� ���϶� true, �ƴ� �� false
    /// </summary>
    private bool IsHit = false;

    public SkinnedMeshRenderer render;
    public PlayerAnim ani;
    private Material material;

    /// <summary>
    /// �÷��̾� �̵��� ����
    /// </summary>
    private Vector3 Vector = Vector3.zero;




    // Start is called before the first frame update
    void Start()
    {
        //���� �����ð��� �ܺο��� �������� ���� ��� ��ü ����
        if (hitDuration <= 0f)
            hitDuration = 0.5f;
        //�÷��̾� �̵� �ӵ��� ����
        if(Speed == 0f)
        {
            Speed = 5f;
        }

        material = render.material;
    }

    public void HitFX()
    {
        if (!IsHit) //�°� �ִ� ���� �ƴ��� Ȯ��
        {
            GameManager.HP -= 1f; //HP ó��
            material.SetColor("_Emission", new Color(0.5f, 0f, 0f, 1f)); //Ÿ�ݽ� ĳ������ Ÿ�� ����Ʈ ����
            IsHit = true; //�°��ִ� ó��
        }
    }

    private void OnTriggerEnter(Collider other) //�ٸ� ������Ʈ�� �浹���� ��
    {
        if (other.gameObject.tag.Equals("Mob")) //�ε��� �� źȯ���� üũ
        {
            if(!IsHit) //���� �����ð��� ������ �ʾҴ� �� üũ
            ani.Hit();
        }
        other.gameObject.SetActive(false); //źȯ�� ��Ȱ��ȭ ó��

    }


    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isOver) //������ ����������
        {
            //�÷��̾� �̵� �Լ�

            //����Ƽ���� Ű �Է��� �޾� ���� ����
            Vector.Set(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            //�ƹ� �Էµ� ���� �ʾ��� �� ������ �ְ� �Է��� �޾ƾ� �̵�
            if (Vector != Vector3.zero)
            {
                transform.Translate(Vector * Speed * Time.deltaTime, Space.World);
                ani.MoveCharacter(Vector.magnitude);
            }
            else
            {
                ani.StopCharacter();
            }

            Vector3 rVector = Vector3.RotateTowards(transform.forward, Vector, 10f * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(rVector);


        }

        if (IsHit) //�¾Ҵ��� Ȯ��
        {
            if (timeLeft < hitDuration) //���� ���� �����ð����� �ð��� ���� �귶���� Ȯ��
            {
                timeLeft += Time.deltaTime; //�����ð� �ջ�
            }
            else //���� ������ �����ð��� �����ð����� �����
            {
                material.SetColor("_Emission", Color.black);
                IsHit = false;
                timeLeft = 0f;
            }

        }

    }

}
