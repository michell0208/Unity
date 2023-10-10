using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlongMove : MonoBehaviour
{
    public Transform Player;
    public float Speed;
    public Vector3 Offset;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = Player.position + Offset;
        if ( Speed == 0f )
        {
            Speed = 5f;
        }
    }

    public void SetPosition()
    {
        transform.position = Player.position + Offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.position + Offset, Speed * Time.deltaTime);
    }
}
