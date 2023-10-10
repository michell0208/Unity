using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    /// <summary>
    /// ��� ��� ���ð�
    /// </summary>
    public float AFKCool = 5f;
    private Animator anim;
    /// <summary>
    /// ���� ��� �ð�
    /// </summary>
    private float AFKTime = 0f;



    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// ĳ���� �̵� ��� ��� ���� �� �ȴ� �ִϸ��̼� ���
    /// </summary>
    /// <param name="speed"></param>
    public void MoveCharacter(float speed)
    {
        anim.SetBool("AFK", false);
        anim.SetBool("IsMoving", true);
        anim.SetFloat("Speed", speed);
        AFKTime = 0f;
    }

    /// <summary>
    /// ĳ���� �̵� ���� �ִϸ��̼� ���� ���¸� idle�� ����
    /// </summary>
    public void StopCharacter()
    {
        anim.SetBool("IsMoving", false);
        anim.SetFloat("Speed", 0f);
    }

    public void Hit()
    {
        anim.SetTrigger("IsHit");
    }

    public void Die()
    {
        anim.SetBool("Die", true);
    }

    /// <summary>
    /// ��� �÷��̾� �ִϸ��̼� ���� ���¸� �ʱ�ȭ
    /// </summary>
    public void Respawn()
    {
        anim.SetBool("Die", false);
        anim.SetBool("IsMoving", false);
        anim.SetBool("AFK", false);
        anim.SetFloat("Speed", 0f);
    }


    // Update is called once per frame
    void Update()
    {
        //���� ���°� ���߾� �ִ� ��Ȳ�� �� ���� ��� �ð��� ���
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
        {
            AFKTime += Time.deltaTime;
        }

        if (AFKTime >= 5f)
        {
            anim.SetBool("AFK", true);
            AFKTime = 0f;
        }


    }
}
