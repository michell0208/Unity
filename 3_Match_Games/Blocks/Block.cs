using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    NONE
};

public enum BLOCKTYPE
{
    NORMAL,
    EVENT,
    BOMB
};

public enum BLOCKSTATE
{
    STOP,
    MOVE
};

public class Block : MonoBehaviour
{

    public SpriteRenderer _Image;
    public int Type { set; get; }//블럭 타입
    public BLOCKSTATE State { set; get; }//블럭 상태

    public float Width { set; get; } //블럭 넓이
    public float Speed { set; get; } = 0.05f; //속도

    private Vector3 _movePos; // 움직일 위치
    public Vector3 MovePos
    {
        get => _movePos;
        set => _movePos = value;
    }

    private DIRECTION _direct = DIRECTION.NONE;

    public int Column { set; get; }
    public int Row { set; get; }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(int column, int row, int type, Sprite sprite)
    {
        Column = column;
        Row = row;
        Type = type;
        _Image.sprite = sprite;
    }

    public void Move(DIRECTION direct, int moveCount)
    {
        switch(direct)
        {
            case DIRECTION.LEFT:
                {
                    _direct = DIRECTION.LEFT;
                    State = BLOCKSTATE.MOVE;

                }
                break;

            case DIRECTION.RIGHT:
                {
                    _direct = DIRECTION.RIGHT;
                    State = BLOCKSTATE.MOVE;
                }
                break;

            case DIRECTION.UP:
                {
                    _direct = DIRECTION.UP;
                    State = BLOCKSTATE.MOVE;
                }
                break;

            case DIRECTION.DOWN:
                {
                    _direct = DIRECTION.DOWN;
                    State = BLOCKSTATE.MOVE;

                }
                break;
        }
    }

    public void Move(DIRECTION direct)
    {
        switch (direct)
        {
            case DIRECTION.LEFT:
                _movePos = transform.position;
                _movePos.x -= Width;
                _direct = DIRECTION.LEFT;
                State = BLOCKSTATE.MOVE;
                break;

            case DIRECTION.RIGHT:
                _movePos = transform.position;
                _movePos.x += Width;
                _direct = DIRECTION.RIGHT;
                State = BLOCKSTATE.MOVE;
                break;

            case DIRECTION.UP:
                _movePos = transform.position;
                _movePos.y += Width;
                _direct = DIRECTION.UP;
                State = BLOCKSTATE.MOVE;
                break;

            case DIRECTION.DOWN:
                _movePos = transform.position;
                _direct = DIRECTION.DOWN;
                _movePos.y -= Width;
                State = BLOCKSTATE.MOVE;
                break;

        }



    }

    // Update is called once per frame
    void Update()
    {
        //블럭이 MOVE 상태에서만 움직임 처리
        if(State == BLOCKSTATE.MOVE)
        {
            switch (_direct)
            {
                case DIRECTION.LEFT:
                    if (transform.position.x >= _movePos.x)
                    {
                        transform.Translate(Vector3.left * Speed);
                    }
                    else
                    {
                        transform.position = _movePos;
                        State = BLOCKSTATE.STOP;
                    }
                    break;

                case DIRECTION.RIGHT:
                    if (transform.position.x <= _movePos.x)
                    {
                        transform.Translate(Vector3.right * Speed);
                    }
                    else
                    {
                        transform.position = _movePos;
                        State = BLOCKSTATE.STOP;
                    }
                    break;

                case DIRECTION.UP:
                    if (transform.position.y <= _movePos.y)
                    {
                        transform.Translate(Vector3.up * Speed);
                    }
                    else
                    {
                        transform.position = _movePos;
                        State = BLOCKSTATE.STOP;
                    }
                    break;

                case DIRECTION.DOWN:
                    if (transform.position.y >= _movePos.y)
                    {
                        transform.Translate(Vector3.down * Speed);
                    }
                    else
                    {
                        transform.position = _movePos;
                        State = BLOCKSTATE.STOP;
                    }
                    break;
            }

        }
    }
}
