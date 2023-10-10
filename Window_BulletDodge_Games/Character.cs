using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    /// <summary>
    /// 이동속도
    /// </summary>
    public float Speed = 1f;
    /// <summary>
    /// 타격 무적시간, 연속으로 탄환을 여러개 맞거나, 의도치 않게 겹쳐지는 탄환에 의한 피해 무시
    /// </summary>
    public float hitDuration;
    /// <summary>
    /// 무적 남은 시간
    /// </summary>
    private float timeLeft = 0f;
    /// <summary>
    /// 맞았는 지 여부, 무적시간 등으로 맞고있는 중일땐 true, 아닐 시 false
    /// </summary>
    private bool IsHit = false;

    public SkinnedMeshRenderer render;
    public PlayerAnim ani;
    private Material material;

    /// <summary>
    /// 플레이어 이동용 벡터
    /// </summary>
    private Vector3 Vector = Vector3.zero;




    // Start is called before the first frame update
    void Start()
    {
        //만약 무적시간을 외부에서 설정하지 않은 경우 자체 설정
        if (hitDuration <= 0f)
            hitDuration = 0.5f;
        //플레이어 이동 속도도 설정
        if(Speed == 0f)
        {
            Speed = 5f;
        }

        material = render.material;
    }

    public void HitFX()
    {
        if (!IsHit) //맞고 있는 중이 아닌지 확인
        {
            GameManager.HP -= 1f; //HP 처리
            material.SetColor("_Emission", new Color(0.5f, 0f, 0f, 1f)); //타격시 캐릭터의 타격 이펙트 설정
            IsHit = true; //맞고있는 처리
        }
    }

    private void OnTriggerEnter(Collider other) //다른 오브젝트랑 충돌했을 때
    {
        if (other.gameObject.tag.Equals("Mob")) //부딪힌 게 탄환인지 체크
        {
            if(!IsHit) //아직 무적시간이 끝나지 않았는 지 체크
            ani.Hit();
        }
        other.gameObject.SetActive(false); //탄환의 비활성화 처리

    }


    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isOver) //게임이 진행중인지
        {
            //플레이어 이동 함수

            //유니티에서 키 입력을 받아 방향 설정
            Vector.Set(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            //아무 입력도 하지 않았을 때 가만히 있고 입력을 받아야 이동
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

        if (IsHit) //맞았는지 확인
        {
            if (timeLeft < hitDuration) //맞은 이후 무적시간보다 시간이 적게 흘렀는지 확인
            {
                timeLeft += Time.deltaTime; //누적시간 합산
            }
            else //맞은 이후의 누적시간이 무적시간보다 길어짐
            {
                material.SetColor("_Emission", Color.black);
                IsHit = false;
                timeLeft = 0f;
            }

        }

    }

}
