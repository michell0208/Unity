using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    /// <summary>
    /// 잠수 모션 대기시간
    /// </summary>
    public float AFKCool = 5f;
    private Animator anim;
    /// <summary>
    /// 누적 잠수 시간
    /// </summary>
    private float AFKTime = 0f;



    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 캐릭터 이동 모션 잠수 해제 후 걷는 애니메이션 재생
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
    /// 캐릭터 이동 멈춤 애니메이션 현재 상태를 idle로 설정
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
    /// 모든 플레이어 애니메이션 현재 상태를 초기화
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
        //현재 상태가 멈추어 있는 상황일 때 누적 잠수 시간을 계산
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
