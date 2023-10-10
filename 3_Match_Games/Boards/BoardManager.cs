using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//마우스가 움직이는 방향
public enum MouseMoveDirection
{
    MOUSEMOVEUP,
    MOUSEMOVEDOWN,
    MOUSEMOVERIGHT,
    MOUSEMOVELEFT
};


//게임의 흐름 상태
public enum GamePlayState
{
    INPUTOK, //입력제어(게임진행)
    AFTERINPUTMOVECHECK, //입력이 된 후에 블럭의 움직임이 끝났는지 체크
    MATCHCHECK, //매치 블럭(3개) 처리
    AFTERMATCHCHECK_MOVECHECK, //매치체크한 후에 블럭이 움직이는 상태인지 체크
    DROPBLOCK, //새로운 블럭 배치
    AFTERDROPBLOCK_MOVECHECK, //새로운 블럭 배치 후에 블럭이 움직이는 상태인지 체크
    INPUTCANCEL //입력취소
};

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _BlockPreFab;

    //public int _Chain { set; get; }

    private float _BlockWidth; //블럭 넓이
    private Vector2 _ScreenPos; //스크린 좌표계 위치

    private float _ScreenWidth = 0.0f;
    private float _BlockScale = 0.0f; //블럭의 화면 비율에 따른 스케일 값

    [SerializeField] private float _Xmargin = 0.5f; //x축 넓이 여백
    [SerializeField] private float _Ymargin = 2.0f; //y축 넓이 여백

    private GameObject[,] _GameBoard; //게임 블럭보드
    [SerializeField] private Sprite[] _Sprites; //블럭 이미지 배열(연결)

    private bool _MouseClick = false; //마우스 클릭 여부true 누른상태, false 떨어진상태
    private Vector3 _StartPos, _EndPos; //마우스의 클릭시 위치값=Startpos / 이동한 위치값 =Endpos
    private GameObject _ClickObject; //마우스로 클릭된 블럭 저장

    private int _Column; //현재 게임보드의 column행값
    private int _Row; //현재 게임보드의 row열값

    private bool _IsOver = false; //클릭된 블럭이 있는지 여부 저장

    [SerializeField] private float _MoveDistance = 0.1f; //마우스 움직임 체크 값(마우스 민감도)

    private GamePlayState PlayState { set; get; } //현재 게임의 상태 저장

    private List<GameObject> _RemovingBlocks = new List<GameObject>(); //삭제될 블럭 저장

    //삭제된 블럭 저장 이후에 새로 만들 때 사용할 블럭을 저장
    private List<GameObject> _RemovedBlocks = new List<GameObject>();

    private int TYPECOUNT = 5;

    private int MATCHCOUNT = 3;

    [SerializeField] private float _Ypos = 3.0f; //블럭이 출현할 때의 출현 시작 위치


    // Start is called before the first frame update
    void Start()
    {
        _ScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        Debug.Log("_ScreenPos : " + _ScreenPos);

        _ScreenPos.y = -_ScreenPos.y;

        _ScreenWidth = Mathf.Abs(_ScreenPos.x * 2); //스크린의 넓이를 절대값으로 계산
        _ScreenWidth -= _Xmargin * 2;

        _BlockWidth = _BlockPreFab.GetComponent<Block>()._Image.sprite.rect.size.x / 100;
        MakeBoard(5,5);
       // _Chain = 0;
    }

    private void MakeBoard(int column, int row)
    {
        _Column = column;
        _Row = row;

        float BlockWidth = _BlockWidth * row;
        float ScreenWidth = _ScreenWidth;

        _BlockScale = ScreenWidth / BlockWidth;

        _GameBoard = new GameObject[column, row];

        for(int col = 0; col < column; col++)
        {
            for(int ro = 0; ro < row; ro++)
            {
                //이미 만들어진 겜오브를 필요할 때 마다 실시간 생성
                _GameBoard[col,ro] = Instantiate(_BlockPreFab) as GameObject;
                _GameBoard[col,ro].transform.localScale = new Vector3(_BlockScale, _BlockScale, 0.0f);
                //화면 사이즈 비율에 따른 블럭의 스케일값 지정


                _GameBoard[col, ro].GetComponent<Block>().Width = _BlockWidth;
                _GameBoard[col, ro].transform.position =
                    new Vector3(_ScreenPos.x + _Xmargin + (ro * _BlockWidth * _BlockScale) + (_BlockWidth * _BlockScale / 2),
                                _ScreenPos.y - _Ymargin + (-col * _BlockWidth * _BlockScale) - (_BlockWidth * _BlockScale / 2), 0.0f);


                //타입카운트를 조절

                //해당 블럭에 출력할 이미지를 랜덤으로 구한다
                int type = UnityEngine.Random.Range(0, TYPECOUNT);

                var block = _GameBoard[col, ro].GetComponent<Block>();
                block.Init(col, ro, type, _Sprites[type]);
                block.name = string.Format("Block[{0}, {1}]", col, ro);

            }
        }
    }

    #region Test
    public void MoveAllBlock(DIRECTION direct)
    {
        foreach(var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(direct);
        }
    }

    public void OnClickMoveAllBlockLeft()
    {
        MoveAllBlock(DIRECTION.LEFT);
    }
    public void OnClickMoveAllBlockRight()
    {
        MoveAllBlock(DIRECTION.RIGHT);
    }
    public void OnClickMoveAllBlockUp()
    {
        MoveAllBlock(DIRECTION.UP);
    }
    public void OnClickMoveAllBlockDown()
    {
        MoveAllBlock(DIRECTION.DOWN);
    }
    #endregion


    //게임보드 상에 블럭들이 move 상태인지 체크 true = 움직이는 블럭 O false = 움직이는 블럭 X
    private bool CheckBlockMove()
    {
        foreach(var obj in _GameBoard)
        {
            if(obj != null)
            {
                if(obj.GetComponent<Block>().State == BLOCKSTATE.MOVE)
                {
                    return true;
                }
            }
        }

        return false;
    }


    // 3개 이상인 매치될 블럭을 찾는다.
    private void CheckMatchBlock()
    {
        List<GameObject> matchList = new List<GameObject>(); //match된 블럭 저장
        // == List<Block> match new List<Block>();
        List<GameObject> tempMatchList = new List<GameObject>();

        int CheckType = 0; //비교할 블럭의 타입을 저장

        if (_RemovingBlocks.Count > 0)
        {
            _RemovingBlocks.Clear(); //삭제처리될 블럭 리스트를 초기화한다.
        }


        //세로 방향으로 Match 블럭을 체크
        for(int row = 0; row < _Row; row++)
        {
            if(_GameBoard[0,row] == null)
            {
                continue;
            }

            CheckType = _GameBoard[0, row].GetComponent<Block>().Type; //첫 행의 블럭의 타입 값을 저장

            tempMatchList.Add(_GameBoard[0, row]); //첫 블럭을 임시블럭 리스트에 저장

            for(int col = 1; col < _Column; col++)
            {
                if(_GameBoard[col, row] == null) continue;

                if(CheckType == _GameBoard[col,row].GetComponent<Block>().Type)
                {
                    tempMatchList.Add(_GameBoard[col, row]);
                }
                else //블럭 타입이 다른 경우
                {
                    //현재 블럭타입이 이전 블럭타입과 다른 경우
                    //현재 임시 블럭 리스트(temp)에 저장된 블럭의 갯수가 3개 이상인 지 체크한다.
                    if(tempMatchList.Count >= 3)
                    {
                        matchList.AddRange(tempMatchList);
                        tempMatchList.Clear();

                        //불일치가 일어난 위치에서 체크타입을 재설정
                        CheckType = _GameBoard[col, row].GetComponent<Block>().Type;
                        tempMatchList.Add(_GameBoard[col, row]);
                    }
                    else
                    {
                        tempMatchList.Clear();
                        //불일치가 일어난 위치에서 체크타입을 재설정
                        CheckType = _GameBoard[col, row].GetComponent<Block>().Type;
                        tempMatchList.Add(_GameBoard[col, row]);

                    }
                }
            }

            // 행을 다 처리한 후
            // tempMatchList를 다시 체크한다
            if (tempMatchList.Count >= 3)
            {
                matchList.AddRange(tempMatchList);
                tempMatchList.Clear();
            }
            else //블럭의 갯수가 3보다 작은 경우
            {
                tempMatchList.Clear();
            }
        }

        //가로 방향 매치 블럭 처리

        for (int col = 0; col < _Column; col++)
        {
            if (_GameBoard[col, 0] == null)
            {
                continue;
            }

            //첫 행의 블럭타입을 가지고온다.
            CheckType = _GameBoard[col, 0].GetComponent<Block>().Type;
            tempMatchList.Add(_GameBoard[col, 0]);

            for (int row = 1; row < _Row; row++)
            {
                if (_GameBoard[col, row] == null) continue;

                if(CheckType == _GameBoard[col, row].GetComponent<Block>().Type)
                {
                    tempMatchList.Add(_GameBoard[col, row]); //체크타입 같으면 추가
                }
                else //타입이 같지 않음
                {
                    if(tempMatchList.Count >= 3)
                    {
                        //현재까지 일치한 블럭을 matchlist에 추가
                        matchList.AddRange(tempMatchList);
                        tempMatchList.Clear();
                        CheckType = _GameBoard[col, row].GetComponent<Block>().Type;
                        tempMatchList.Add(_GameBoard[col, row]);
                    }
                    else
                    {
                        tempMatchList.Clear();
                        CheckType = _GameBoard[col, row].GetComponent<Block>().Type;
                        tempMatchList.Add(_GameBoard[col, row]);

                    }
                }
            }

            //열을 다 처리한 후에(마지막이 같은 타입이었을 경우)
            if (tempMatchList.Count >= 3)
            {
                matchList.AddRange(tempMatchList);
                tempMatchList.Clear();
            }
            else
            {
                tempMatchList.Clear();
            }
        }

        //matchlist에 중복 포함된 블럭 참조를 정리
        matchList = matchList.Distinct().ToList();

        if(matchList.Count > 0)
        {
            foreach (var obj in matchList)
            {
                var block = obj.GetComponent<Block>();

                //게임보드상에서 매치된 블럭을 제거한다.
                _GameBoard[block.Column, block.Row] = null;

                //매치된 블럭을 비활성화 처리하여
                //화면에 보이지 않도록 한다.
                obj.SetActive(false);
            }

            _RemovingBlocks.AddRange(matchList);
        }

        //매취된 블럭을 제거처리하여
        //제거리스트에 옮긴다.
        //이후에 새로운 블럭을 만들 때 재사용 목적
        _RemovedBlocks.AddRange(_RemovingBlocks);


        //불럭 다운 처리
        DownMoveBlocks();
    }



    private void DownMoveBlocks()
    {
        int moveCount = 0; //움직여야하는 칸수 계산용 변수

        for(int row = 0; row < _Row; row++)
        {
            for(int col = _Column - 1; col >= 0; col--)
            {
                if(_GameBoard[col, row] == null)
                {
                    moveCount++;
                }else
                {
                    if(moveCount > 0)
                    {
                        var block = _GameBoard[col, row].GetComponent<Block>();
                        block.MovePos = block.transform.position; //현재 위치값을 가져옴.
                        block.MovePos = new Vector3(block.MovePos.x, block.MovePos.y - block.Width * moveCount,
                            block.MovePos.z);

                        //이전에 있던 게임보드상의 위치를 초기화
                        _GameBoard[col, row] = null;

                        block.Column = block.Column + moveCount;
                        block.gameObject.name = $"Block[{block.Column}, {block.Row}]";
                        _GameBoard[block.Column, block.Row] = block.gameObject; //게임보드상에 움직일 위치에 블럭 연결

                        block.Move(DIRECTION.DOWN, moveCount);
                    }
                }
            }

            moveCount = 0;
        }

    }


    //게임보드에 모든 블럭이 있는지 체크/ 없음 : false , 있음 : true
    private bool CheckAllBlockInGameBoard()
    {
        int count = _GameBoard.Length;

        foreach(var obj in _GameBoard)
        {
            if(obj == null)
            {
                return false; //보드 상에 블럭이 없음
            }

        }

        return true; //보드 상에 모든 블럭이 있음
    }


    /// <summary>
    /// 임의의 블럭을 움직여서 매치되는 지 여부를 확인하는 함수
    /// 블럭을 움직여서 매치되는 블럭이 있으면 true 없으면 false
    /// </summary>
    /// <returns></returns>
    private bool CheckAfterMoveMatchBlock()
    {
        int checkType = -1; //비교하는 블럭의 타입 저장
        int matchCount = 0; //매치되는 블럭 갯수 체크

        for(int row = 0; row< _Row; row++)
        {
            for(int col = _Column - 1; col >= (MATCHCOUNT - 1); col--)
            {
                //하단에서 상단으로 진행
                //상단 이동시 우측에서 이동했을 때 매칭 되는 경우
                if(row >= 0 && row < (_Row - 1))
                {
                    //하단 우측 체크
                    checkType = _GameBoard[row + 1, col].GetComponent<Block>().Type; //첫 행의 블럭의 type 값 지정

                    if((checkType == _GameBoard[row, col - 1].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row, col - 2].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //중간 우측 체크
                    checkType = _GameBoard[row + 1, col - 1].GetComponent<Block>().Type;

                    if((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row, col - 2].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //상단 우측 체크
                    checkType = _GameBoard[row + 1, col - 2].GetComponent<Block>().Type;

                    if((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row, col - 1].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                }


                //상단 이동시 좌측 블럭 이동했을 때 매칭되는 경우
                if((row > 0 ) && (row <= _Row - 1))
                {
                    //상단 좌측 체크
                    checkType = _GameBoard[row - 1, col].GetComponent<Block>().Type;

                    if((checkType == _GameBoard[row, col - 1].GetComponent<Block>().Type)&&
                        (checkType == _GameBoard[row, col - 2].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //중단 좌측 체크
                    checkType = _GameBoard[row - 1, col - 1].GetComponent<Block>().Type;

                    if((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row, col - 2].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //상단 좌측 체크
                    checkType = _GameBoard[row - 1, col - 2].GetComponent<Block>().Type;

                    if ((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row, col - 1].GetComponent<Block>().Type))
                    {
                        return true;
                    }
                }


            }
        }

        //가로 Match 블럭 Check
        for(int col = 0; col < _Column; col++)
        {
            for(int row = 0; row < (_Row - MATCHCOUNT); row++)
            {

                //좌측에서 우측으로 진행
                //우측 이동시 하단 블럭을 움직였을 때 매칭되는경우
                if(col >= 0 && col < (_Column - 1))
                {
                    //좌측 하단 체크
                    checkType = _GameBoard[row, col + 1].GetComponent<Block>().Type;

                    if((checkType == _GameBoard[row + 1, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row + 2,col].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //중측 하단 체크
                    checkType = _GameBoard[row + 1, col + 1].GetComponent<Block>().Type;

                    if ((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row + 2, col].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //우측 하단 체크
                    checkType = _GameBoard[row + 2, col + 1].GetComponent<Block>().Type;

                    if ((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row + 1, col].GetComponent<Block>().Type))
                    {
                        return true;
                    }
                }

                //좌측에서 우측으로 진행
                //우측 이동 시 상단 블럭을 움직였을 때 매칭되는 경우
                if((col > 0) && col <= _Column - 1)
                {
                    //좌측 상단 체크
                    checkType = _GameBoard[row, col - 1].GetComponent<Block>().Type;

                    if ((checkType == _GameBoard[row + 1, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row + 2, col].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //중축 상단 체크
                    checkType = _GameBoard[row + 1, col - 1].GetComponent<Block>().Type;

                    if ((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row + 2, col].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                    //우측 상단 체크
                    checkType = _GameBoard[row + 2, col - 1].GetComponent<Block>().Type;

                    if ((checkType == _GameBoard[row, col].GetComponent<Block>().Type) &&
                        (checkType == _GameBoard[row + 1, col].GetComponent<Block>().Type))
                    {
                        return true;
                    }

                }
            }
        }

        return false;

    }


    ///제거리스트에 저장된 매칭되어서 게임보드상에서 제거된 블럭 가져와
    ///새로운 블럭을 생성시 사용하는함수
    private GameObject GetNewBlock(int column, int row, int type)
    {
        if(_RemovedBlocks.Count <= 0)
        {
            return null;
        }

        GameObject obj = _RemovedBlocks[0];

        obj.GetComponent<Block>().Init(column, row, type, _Sprites[type]);

        _RemovedBlocks.Remove(obj);

        return obj;
    }

    //새로운 블럭을 보드에 배치한다
    private void CreateNewBlock()
    {
        int moveCount = 0;

        for(int row = 0; row < _Row; row++)
        {
            for(int col = _Column - 1; col >= 0; col--)
            {
                if(_GameBoard[col, row] == null)
                {
                    int type = UnityEngine.Random.Range(0, TYPECOUNT); //블럭의 타입을 랜덤으로 계산
                    _GameBoard[col, row] = GetNewBlock(col, row, type); //새로운 블럭을 가져온다
                    _GameBoard[col, row].name = $"Block[{col}, {row}]";
                    _GameBoard[col, row].gameObject.SetActive(true);
                    var block = _GameBoard[col, row].GetComponent<Block>();

                    //블럭의 출현 위치 계산
                    _GameBoard[col, row].transform.position =
                        new Vector3(_ScreenPos.x + _Xmargin + (_BlockWidth * _BlockScale) / 2
                        + row * (_BlockWidth * _BlockScale), _ScreenPos.y - _Ymargin - col *
                        (_BlockWidth * _BlockScale) - (_BlockWidth * _BlockScale) / 2, 0.0f);

                    block.MovePos = block.transform.position;//이동할 위치값 계산

                    float MoveYpos = _GameBoard[col, row].GetComponent<Block>().MovePos.y +
                        (_BlockWidth * _BlockScale) * moveCount++ + _Ypos;

                    //생성될 위치로 블럭을 이동
                    _GameBoard[col, row].transform.position =
                        new Vector3(_GameBoard[col, row].GetComponent<Block>().MovePos.x, MoveYpos,
                        _GameBoard[col, row].GetComponent<Block>().MovePos.z);

                    block.Move(DIRECTION.DOWN, moveCount);
                }
            }
        }
    }

    //Y축 업벡터와 비교해서 이동위치 벡터 그 사이 각도 계산
    private float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    //각도에 따라 방향을 계산
    private MouseMoveDirection CalculateDirection()
    {
        float _angle = CalculateAngle(_StartPos, _EndPos);

        if(_angle >= 315.0f && _angle <= 360 || _angle >= 0 && _angle < 45.0f)
        {
            return MouseMoveDirection.MOUSEMOVEUP;
        }
        else if(_angle >= 45.0f && _angle < 135.0f)
        {
            return MouseMoveDirection.MOUSEMOVELEFT;
        }
        else if(_angle >= 135.0f && _angle < 225.0f)
        {
            return MouseMoveDirection.MOUSEMOVEDOWN;
        }
        else if(_angle >= 225.0f && _angle < 315.0f)
        {
            return MouseMoveDirection.MOUSEMOVERIGHT;
        }

        return MouseMoveDirection.MOUSEMOVEDOWN;
    }

    //마우스 클릭 후 커서의 움직임에 따라 호출, 마우스 이동시
    private void MouseMove()
    {
        float diff = Vector2.Distance(_StartPos, _EndPos);

        if(diff > _MoveDistance && _ClickObject != null)
        {
            MouseMoveDirection dir = CalculateDirection();

            Debug.Log(dir);

            //클릭된 블럭의 열과 행값을 가지고 온다.
            int column = _ClickObject.GetComponent<Block>().Column;
            int row = _ClickObject.GetComponent<Block>().Row;

            switch (dir)
            {
                //왼쪽 방향으로 움직임
                case MouseMoveDirection.MOUSEMOVELEFT:
                    {
                        //열값이 0보다 큰 경우에 좌측 이동이 가능(왼쪽 모서리애들은 왼쪽이동 불가)
                        if(row > 0)
                        {
                            //이동할 위치의 행값과 열값으로 위치 값을 갱신
                            _GameBoard[column, row].GetComponent<Block>().Row = row - 1;
                            _GameBoard[column, row - 1].GetComponent<Block>().Row = row;

                            //게임보드상의 참조가 엮인 위치 값도 변경
                            _GameBoard[column, row] = _GameBoard[column, row - 1];
                            _GameBoard[column, row - 1] = _ClickObject;

                            //블럭이 움직이도록 명령
                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.RIGHT);
                            _GameBoard[column, row - 1].GetComponent<Block>().Move(DIRECTION.LEFT);

                            PlayState = GamePlayState.AFTERINPUTMOVECHECK;
                        }
                    }

                    break;

                case MouseMoveDirection.MOUSEMOVEUP:
                    {
                        if(column > 0)
                        {
                            _GameBoard[column, row].GetComponent<Block>().Column = column - 1;
                            _GameBoard[column - 1, row].GetComponent<Block>().Column = column;

                            _GameBoard[column, row] = _GameBoard[column - 1, row];
                            _GameBoard[column - 1, row] = _ClickObject;

                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.DOWN);
                            _GameBoard[column - 1, row].GetComponent<Block>().Move(DIRECTION.UP);

                            PlayState = GamePlayState.AFTERINPUTMOVECHECK;
                        }
                    }

                    break;

                case MouseMoveDirection.MOUSEMOVEDOWN:
                    {
                        if(column < (_Column - 1))
                        {
                            _GameBoard[column, row].GetComponent<Block>().Column = column + 1;
                            _GameBoard[column + 1, row].GetComponent<Block>().Column = column;

                            _GameBoard[column, row] = _GameBoard[column + 1, row];
                            _GameBoard[column + 1, row] = _ClickObject;

                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.UP);
                            _GameBoard[column + 1, row].GetComponent<Block>().Move(DIRECTION.DOWN);

                            PlayState = GamePlayState.AFTERINPUTMOVECHECK;
                        }
                    }

                    break;

                case MouseMoveDirection.MOUSEMOVERIGHT:
                    {
                        if(row < (_Row - 1))
                        {
                            _GameBoard[column, row].GetComponent<Block>().Row = row + 1;
                            _GameBoard[column, row + 1].GetComponent<Block>().Row = row;

                            _GameBoard[column, row] = _GameBoard[column, row + 1];
                            _GameBoard[column, row + 1] = _ClickObject;

                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.LEFT);
                            _GameBoard[column, row + 1].GetComponent<Block>().Move(DIRECTION.RIGHT);

                            PlayState = GamePlayState.AFTERINPUTMOVECHECK;
                        }
                    }

                    break;
            }


            _StartPos = _EndPos = Vector3.zero;
            _ClickObject = null;
            _MouseClick = false;

        }


    }

    // Update is called once per frame
    void Update()
    {
        switch(PlayState)
        {
            case GamePlayState.INPUTOK:

                //Debug.Log($"{_Chain} Chain");

                if (Input.GetMouseButtonDown(0)) //마우스 버튼 눌렀을 때
                {
                    _MouseClick = true;
                    _EndPos = _StartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _EndPos.z = _StartPos.z = 0.0f;

                    _IsOver = false;
                    for (int col = 0; col < _Column; col++)
                    {
                        for (int ro = 0; ro < _Row; ro++)
                        {
                            if (_GameBoard[col, ro] != null)
                            {
                                //블럭의 이미지 사각영역에 클릭된 좌표값이 포함되는지 확인.
                                _IsOver = _GameBoard[col, ro].GetComponent<Block>()._Image.bounds.Contains(_StartPos);
                            }
                            //클릭된 블럭이 있음
                            if (_IsOver)
                            {
                                _ClickObject = _GameBoard[col, ro]; //클릭된 블럭을 _ClickObject에 저장
                                                                    //Destroy(_ClickObject);
                                goto SearchExit;
                            }
                        }
                    }

                SearchExit:;
                }

                if (Input.GetMouseButtonUp(0)) //마우스 버튼 놨을때
                {
                    _MouseClick = false;

                    _StartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _EndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _ClickObject = null;
                }

                //마우스 클릭된 상태에서 마우스 커서 움직임(mousemove)
                if ((_MouseClick == true) && ((Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0) ||
                           (Input.GetAxis("Mouse Y") < 0 || Input.GetAxis("Mouse Y") > 0)))
                {
                    _EndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _EndPos.z = 0.0f;

                    Debug.Log("Mouse Move");

                    MouseMove();
                }

                break;

            case GamePlayState.AFTERINPUTMOVECHECK: //입력 후에 블럭의 움직임 체크
                {
                    //checkblockmove : false (움직이는 블럭이 없음)
                    if(!CheckBlockMove())
                    {
                        //PlayState = GamePlayState.INPUTOK;
                        PlayState = GamePlayState.MATCHCHECK;
                    }
                }
                break;


            case GamePlayState.MATCHCHECK: //3개이상 매치된 블럭이 있는지 체크한다

                CheckMatchBlock();

                //매치블럭을 체크 후에
                //보드상에 빈 블럭이 있는 경우에는 상태를 DROPBLOCK으로
                //모든 블럭이 있는 경우에는 PLAY상태로.
                PlayState = GamePlayState.AFTERMATCHCHECK_MOVECHECK;

                break;

            case GamePlayState.DROPBLOCK:
                {
                    //새로운 블럭을 만들어서 하강시킨다
                    CreateNewBlock();

                    PlayState = GamePlayState.AFTERDROPBLOCK_MOVECHECK;

                }
                break;

            case GamePlayState.AFTERMATCHCHECK_MOVECHECK:
            case GamePlayState.AFTERDROPBLOCK_MOVECHECK:
                {
                    //블럭의 움직임이 끝났는지 체크
                    if(!CheckBlockMove())
                    {
                        if(PlayState == GamePlayState.AFTERMATCHCHECK_MOVECHECK)
                        {
                            //보드상에 모든 블럭이 있음
                            //더 이상 매치 되는 블럭이 없음.
                            if(CheckAllBlockInGameBoard())
                            {
                                if(CheckAfterMoveMatchBlock())
                                {
                                    //보드상에 블럭이 다 있으면 게임 진행.
                                    PlayState = GamePlayState.INPUTOK;
                                }
                                else
                                {
                                    //블럭 하나를 움직여서 매칭 되지 않는 경우
                                    //다시 섞던지 게임을 종료
                                    Debug.Log("더이상 움직일 수 있는 블럭이 없습니다");
                                }
                            } else
                            {
                                //_Chain++;
                                //보드상에 블럭이 없으면 채워줘야함.
                                PlayState = GamePlayState.DROPBLOCK;
                            }
                        }
                        else if (PlayState == GamePlayState.AFTERDROPBLOCK_MOVECHECK)
                        {
                            //DropBlock한 후에 움직임 완료
                            //다시 MatchBlock을 체크
                            PlayState = GamePlayState.MATCHCHECK;
                        }

                    }
                }
                break;
        }


    }
}
