using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Convert3d : MonoBehaviour
{

    private int _currentColumn = 0; // 현재 스테이지의 가로, 세로 값.
    private int _currentRow = 0;

    [SerializeField] private GameObject _sCubePrefab;   // 큐브 Prefab
    [SerializeField] private GameObject _sMainObject;   // 큐브 블럭들의 부모오브젝트
    [SerializeField] private GameObject _sBackObject;   // 백큐브 블럭들의 부모 오브젝트

    [SerializeField] private Material _sCubeMaterial;   // 큐브오브젝트의 Material
    [SerializeField] private Material _sBackMaterial;   // Back큐브오브젝트의 Material

    [SerializeField] private StageManager _sStageManager;
    [SerializeField] private GameObject _gameView;


    private GameObject[,] _board = null;    // 게임보드
    private GameObject[,] _backBoard = null;    // 백보드

    private float xBaseWidth = 500; // 이미지가 모두 정방형이라고 간주한다.
    private float yBaseHeight = 500;

    private List<Color> _colorList = new List<Color>(); // 유니크한 color 테이블 작성용.
    private List<Block> _backBlockList = new List<Block>(); // 블럭을 저장한 컬러값 구별용.

    private bool _isMouseDrag = false; //마우스 버튼을 누른 상태에서 마우스 이동을 체크

    private GameObject _target = null; //클릭된 큐브블럭 저장용
    private Vector3 _screenPos;
    private Vector3 _offset;

    private float CLICKYOFFSET = 1.3f;
    private float CLICKZOFFSET = -0.5f;

    private bool _isAllMadeToggle = false; //모든 블럭이 매칭됐을 때 update함수가 계속 실행되는 것을 막기 위해 사용


    // Start is called before the first frame update
    void Start()
    {
        PlayGame("Test", "TopSpin");
    }

    /// <summary>
    /// categoryName과 ImageName의 경로에 해당하는 이미지를 읽어서
    /// 스테이지를 구성한다.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="imageName"></param>
    public void PlayGame(string categoryName, string imageName)
    {
        Texture2D texture = null;

        string path = "StageImages/" + categoryName + "/" + imageName;

        texture = Resources.Load(path, typeof(Texture2D)) as Texture2D;

        Build2DConvert3D(texture);
    }

    private List<Color32> GenerateColors(Color32[] colorBuffer, int height, int width)
    {
        List<Color32> vertextColors = new List<Color32>(height * width);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 c = colorBuffer[j + i * width];

                vertextColors.Add(c);
            }
        }

        return vertextColors;
    }


    private void Build2DConvert3D(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;

        var textureFormat = texture.format;
        if (textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("32비트 컬러를 사용");
        }

        int height = texture.height;    // 이미지의 세로 정보
        int width = texture.width;      // 이미지의 가로 정보

        // 이미지의 가로/세로의 값을 저장
        _currentColumn = height;
        _currentRow = width;

        Color32[] colorBuffer = texture.GetPixels32();

        var TextureColors = GenerateColors(colorBuffer, height, width);

        Create3DCube(TextureColors, height, width);

    }

    private void Create3DCube(List<Color32> colors, int height, int width)
    {
        //게임보드 초기화 작업
        if(_board != null)
        {
            foreach(GameObject obj in _board)
            {
                if(obj != null)
                {
                    Destroy(obj);
                }
            }

            foreach(GameObject obj in _backBoard)
            {
                if(obj != null)
                {
                    Destroy(obj);
                }
            }

            _board = null;
            _backBoard = null;

            _colorList.Clear();
            _backBlockList.Clear();

            _sMainObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            _sMainObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

            _sBackObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            _sBackObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);



        }

        _isAllMadeToggle = false;

        // 블럭 저장용 게임보드를 생성한다.
        _board = new GameObject[height, width];
        _backBoard = new GameObject[height, width];

        // 화면 출력되는 이미지의 사이즈 계산
        // 이미지파일에서 픽셀의 색깔값이 투명인 픽셀값을 제외하고 출력되는
        // 가로열 / 세로열의 최대, 최소값을 구하기 위한 변수
        int columnMin = height;
        int columnMax = 0;

        int rowMin = width;
        int rowMax = 0;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 color = colors[i * width + j];

                // 투명컬러가 아닌경우에 블럭이 있는 경우
                if (color.a != 0.0f)
                {
                    GameObject obj = Instantiate(_sCubePrefab) as GameObject;
                    obj.name = $"Cube[{i}, {j}]";

                    // Material의 Cube용으로 만든 Material로 변경한다.
                    obj.GetComponent<MeshRenderer>().material = _sCubeMaterial;

                    // Material의 Color 값을 변경한다.
                    obj.GetComponent<MeshRenderer>().material.color = color;

                    obj.GetComponent<Block>().col = i;  // 블럭의 세로열
                    obj.GetComponent<Block>().row = j;  // 블럭의 가로열

                    // 블럭에 해당위치의 컬러값을 기록
                    obj.GetComponent<Block>().OriginColor = color;

                    // 만들어진 블럭의 부모오브젝트를 설정한다.
                    obj.transform.SetParent(_sMainObject.transform);

                    // 블럭의 위치값을 설정
                    obj.transform.position = new Vector3(j, i, 0.0f);

                    // 사용되어진 Color값 리스트를 만든다.
                    _colorList.Add(color);

                    // 게임보드에 블럭을 추가한다.
                    _board[i, j] = obj;

                    if (columnMin > i)
                    {
                        columnMin = i;
                    }

                    if (columnMax < i)
                    {
                        columnMax = i;
                    }

                    if (rowMin > j)
                    {
                        rowMin = j;
                    }

                    if (rowMax < j)
                    {
                        rowMax = j;
                    }
                }

                // 백보드용
                if (color.a != 0.0f)
                {
                    GameObject backobj = Instantiate(_sCubePrefab) as GameObject;

                    backobj.name = $"CubeBack[{i}, {j}]";

                    backobj.GetComponent<MeshRenderer>().material = _sBackMaterial;
                    backobj.GetComponent<MeshRenderer>().material.color = color;

                    backobj.GetComponent<Block>().col = i;  // 블럭의 세로열
                    backobj.GetComponent<Block>().row = j;  // 블럭의 가로열

                    backobj.GetComponent<Block>().OriginColor = color;

                    // 백큐브 오브젝트의 RigidBody 컴포넌트 제거한다.
                    Rigidbody body = backobj.GetComponent<Rigidbody>();
                    Destroy(body);

                    // 백큐브 오브젝트의 Collider 컴포넌트 제거한다.
                    Collider collider = backobj.GetComponent<Collider>();
                    Destroy(collider);

                    // 백큐브를 _sBackObject의 자식으로 Attach한다.
                    backobj.transform.parent = _sBackObject.transform;
                    backobj.transform.position = new Vector3(j, i, 0.0f);

                    // 백보드용 블럭을 리스트 저장.
                    _backBlockList.Add(backobj.GetComponent<Block>());

                    _backBoard[i, j] = backobj;

                }
            }
        }

        // CubeMainPos를 중심으로 생성된 블럭을 이동시킨다.
        float midXvalue = width / 2.0f;
        float midYvalue = height / 2.0f;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (_board[i, j] != null)
                {

                    Vector3 calposition = _board[i, j].transform.position;

                    // 큐브 보드의 중심을 기준으로 큐브 블럭 위치를 변경
                    _board[i, j].transform.position =
                        new Vector3(calposition.x - midXvalue + 0.5f, calposition.y - midYvalue + 0.5f, 0.0f);

                    // 백큐브 보드의 중심을 기준으로 백큐브 블럭 위치를 변경
                    _backBoard[i, j].transform.position =
                        new Vector3(calposition.x - midXvalue + 0.5f, calposition.y - midYvalue + 0.5f, 0.0f);
                }
            }
        }

        // 실제 출력되는 세로값
        int calColumnCount = columnMax - columnMin + 1;
        // 실제 출력되는 가로값
        int calRowCount = rowMax - rowMin + 1;

        float xscale = 0.0f;

        // 세로가 가로 보다 작은 경우
        if (calColumnCount < calRowCount)
        {
            xscale = xBaseWidth / 100.0f;
            xscale = xscale / calRowCount;
        }
        else
        {
            xscale = yBaseHeight / 100.0f;
            xscale = xscale / calColumnCount;
        }

        if (xscale > 0.4f)
        {
            xscale = 0.4f;
        }

        // 큐브블럭의 비율 변경
        _sMainObject.transform.localScale = new Vector3(xscale, xscale, xscale);

        // 백큐브블럭의 비율 변경
        _sBackObject.transform.localScale = new Vector3(xscale, xscale, xscale);


        // 블럭의 스케일 값이 변경된 후에
        // 변경된 스케일 값을 기록한다.
        for(int i = 0; i < _currentColumn; i++)
        {
            for(int j = 0; j < _currentRow; j++)
            {
                if (_board[i, j] != null)
                {
                    _board[i, j].GetComponent<Block>().OriginScale = _board[i, j].transform.localScale;
                }
            }
        }

        Vector3 mainPosition = _sMainObject.transform.position;

        // CubeBoard의 위치를 상단쪽으로 이동
        _sMainObject.transform.position = new Vector3(mainPosition.x, mainPosition.y + 2.0f, 0.0f);
        _sBackObject.transform.position = new Vector3(mainPosition.x, mainPosition.y + 2.0f, 0.0f);

        // ColorList에 입려된 컬러값을 중복을 제거한다.
        _colorList = _colorList.Distinct().ToList();

        // 백블럭에 입력된 컬러값을 넘버링한다.
        foreach (var block in _backBlockList)
        {
            int index = _colorList.FindIndex(x => x == block.OriginColor);
            block.BlockNumber = index;
            block.NumberText = index.ToString();

            block.OriginPosition = block.transform.position;


            _board[block.col, block.row].GetComponent<Block>().NumberText = index.ToString();
            _board[block.col, block.row].GetComponent<Block>().BlockNumber = index;

            _board[block.col, block.row].GetComponent<Block>().OriginPosition =
                _board[block.col, block.row].GetComponent<Transform>().position;

            block.ShowOnOffNumberText(true);
        }

        LeanTween.delayedCall(1.2f, () =>
        {
            BombBlock();
        });
    }

    /// <summary>
    /// 블럭의 폭발처리
    /// </summary>
    public void BombBlock()
    {
        //블럭을 드롭
        foreach (GameObject obj in _board)
        {
            if (obj != null)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -0.001f);
                obj.GetComponent<Block>().OriginPosition = obj.transform.position;
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                obj.GetComponent<Rigidbody>().useGravity = true;

                _backBoard[obj.GetComponent<Block>().col, obj.GetComponent<Block>().row]
                    .GetComponent<Block>().ShowOnOffNumberText(true);

                _backBoard[obj.GetComponent<Block>().col, obj.GetComponent<Block>().row]
                    .GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    /// <summary>
    /// 스크린상에 마우스 버튼 클릭할 경우 클릭된 위치에서
    /// 발사된 광선(레이저)과 교차하는 오브젝트를 찾아주는 함수
    /// </summary>
    /// <returns></returns>
    private GameObject ReturnClickedObjects()
    {
        GameObject target = null; //클릭된 오브젝트 저장용

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction * 10/*광선의 길이(직선*/);

        foreach(RaycastHit rch in hits)
        {
            if(rch.collider != null && rch.transform.tag == "CubeBlock")
            {
                target = rch.collider.gameObject;
                break;
            }
        }

        return target;
    }

    /// <summary>
    /// 보드상에 선택된 블럭의 컬러에 해당하는 블럭을 모두 찾아서 위치시킴
    /// </summary>
    /// <param name="index"></param>
    private void AllBlockFixedPosition(int index)
    {
        List<GameObject> matchIndexBlockList = new List<GameObject>();

        //백보드상에 index와 동일한 블럭을 찾아서 matchIndexBlockList에 추가
        for(int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if(_backBoard[i,j] != null)
                {
                    var Block = _backBoard[i, j].GetComponent<Block>();

                    if(Block.BlockNumber == index &&
                        Block.CurrentState != Block.State.FIXED)
                    {
                        matchIndexBlockList.Add(_backBoard[i, j]);
                    }
                }
            }
        }

        //입력받은 인덱스번호와 같은 큐브블럭을 찾는다
        //백보드상의 블럭위치로 각각의 블럭을 날린다
        int count = 0;
        for (int i = 0; i < _currentColumn; i++)
        {
            for(int j = 0; j < _currentRow; j++)
            {
                if(_board[i,j] != null)
                {
                    if(_board[i,j].GetComponent<Block>().BlockNumber == index &&
                        _board[i,j].GetComponent<Block>().CurrentState != Block.State.FIXED)
                    {
                        Destroy(_board[i, j].GetComponent<Collider>());
                        _board[i, j].GetComponent<Block>().MoveToFixedPosition(matchIndexBlockList[count++]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 클릭된 큐브와 같은 Color 백보드의 블럭들을 빨간색으로 표시
    /// </summary>
    /// <param name="index"></param>
    /// <param name="color"></param>
    private void ChangeBlockTextColor(int index, Color color)
    {
        foreach(GameObject obj in _backBoard)
        {
            if(obj != null)
            {
                if(obj.GetComponent<Block>().BlockNumber == index)
                {
                    obj.GetComponent<Block>().SetNumberTextColor(color);
                }
            }
        }
    }


    /// <summary>
    /// 보드상의 블럭이 다 매칭처리가 되었는지 판단
    /// </summary>
    /// <returns></returns>
    private bool IsAllBlockMade()
    {
        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if(_board[i,j] != null)
                {
                    if(_board[i,j].GetComponent<Block>().CurrentState != Block.State.FIXED)
                    {
                        return false; //보드상에 블럭이 fixed되지 않은 경우
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 드래그 하는 블럭과 일치하는 백보드 블럭을 찾는다
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private GameObject IsMatchPositionColorBlock(GameObject target)
    {
        foreach(Block bc in _backBlockList)
        {
            if(bc.CurrentState != Block.State.FIXED && bc.CheckMatchPosition(target) && bc.CheckMatchColor(target))
            {
                return bc.gameObject;
            }
        }

        return null;
    }
    private void MouseEventProcess()
    {
        //마우스의 오른쪽 버튼이 눌렀을 때
        if(Input.GetMouseButtonDown(0))
        {
            _target = ReturnClickedObjects();


            if(_target != null)
            {
                bool _isMagicRod = true;
                if(_isMagicRod)
                {
                    AllBlockFixedPosition(_target.GetComponent<Block>().BlockNumber);
                    _target = null;
                    _isMouseDrag = false;
                }
                else
                {
                    _isMouseDrag = true;

                    ChangeBlockTextColor(_target.GetComponent<Block>().BlockNumber, Color.red);

                    Vector3 clickObjectPosition = _target.transform.position; //현재 클릭 큐브의 위치를 기록

                    _target.transform.position = new Vector3(clickObjectPosition.x, clickObjectPosition.y, 0.0f);

                    Destroy(_target.GetComponent<Rigidbody>()); //물리엔진의 영향을 받지 않도록 rigidbody remove

                    _target.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

                    //선택된 블럭이 클릭된 위치의 위쪽에 표시되도록 보정
                    Vector3 pos = _target.transform.position;
                    _target.transform.position =
                        new Vector3(pos.x, pos.y + CLICKYOFFSET, pos.z - CLICKZOFFSET);

                    //선택된 블럭이 조금 더 커보이게 보정
                    _target.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                    _screenPos = Camera.main.WorldToScreenPoint(_target.transform.position);

                    //마우스로 타겟을 잡았을 때의 위치와 블럭의 움직이는 축의 차이값
                    _offset = _target.transform.position - Camera.main.ScreenToWorldPoint
                        (new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPos.z));

                }


            }

        }

        //오른쪽 마우스 버튼이 놓였을 때
        if(Input.GetMouseButtonUp(0))
        {
            _isMouseDrag = false;

            if(_target != null)
            {
                GameObject matchObject = IsMatchPositionColorBlock(_target);
                ChangeBlockTextColor(_target.GetComponent<Block>().BlockNumber, Color.black);

                if (matchObject != null)
                {
                    _target.transform.position = matchObject.GetComponent<Block>().OriginPosition;
                    _target.transform.localScale = _target.GetComponent<Block>().OriginScale;

                    matchObject.SetActive(false);

                    _target.GetComponent<Block>().CurrentState = Block.State.FIXED;
                    matchObject.GetComponent<Block>().CurrentState = Block.State.FIXED;

                    Destroy(_target.GetComponent<Rigidbody>());
                    Destroy(_target.GetComponent<Collider>());

                    _target.GetComponent<Block>().MatchBlockAnimationStart();
                }
                else
                {
                    _target.AddComponent<Rigidbody>(); //삭제했던 rigidBody컴포넌트를 게임오브젝트에 추가
                    _target.GetComponent<Rigidbody>().useGravity = true;

                    _target.transform.localScale = _target.GetComponent<Block>().OriginScale;
                }
            }

        }

        //마우스 버튼이 눌린 상태에서 움직이는 경우 처리
        if(_isMouseDrag)
        {
            Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPos.z);
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPos) + _offset;
            currentPosition.z = -0.5f; //펜스 안에서 x,y 움직이라고 z 고정

            if(_target != null)
            {
                _target.transform.position = currentPosition;

                GameObject matchObject = IsMatchPositionColorBlock(_target);

                if (matchObject != null)
                {
                    _target.transform.position = matchObject.GetComponent<Block>().OriginPosition;
                    _target.transform.localScale = _target.GetComponent<Block>().OriginScale;

                    matchObject.SetActive(false);

                    _target.GetComponent<Block>().CurrentState = Block.State.FIXED;
                    matchObject.GetComponent<Block>().CurrentState = Block.State.FIXED;

                    Destroy(_target.GetComponent<Rigidbody>());
                    Destroy(_target.GetComponent<Collider>());

                    _target.GetComponent<Block>().MatchBlockAnimationStart();

                    ChangeBlockTextColor(_target.GetComponent<Block>().BlockNumber, Color.black);

                    _target = null;
                    _isMouseDrag = false;


                }

            }
        }

    }

    private void AllBlockComplete()
    {
        ClearAnimation();
    }

    private void ClearAnimation()
    {
        float animScale = xBaseWidth / _currentRow;

        animScale /= 100.0f;

        animScale /= 2.0f;

        Vector2 mainScale = new Vector3(animScale, animScale, animScale);

        LeanTween.scale(_sMainObject, mainScale, 0.5f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(ClearAnimationComplete);
    }

    public void ClearAnimationComplete()
    {
        _sMainObject.GetComponent<Animator>().enabled = true;
        _sMainObject.GetComponent<Animator>().SetTrigger("ClearAnimationAnim");


        LeanTween.delayedCall(2.0f, () =>
        {
            _gameView.SetActive(false);
            _sStageManager.PlayNextStage();
        });
    }

        // Update is called once per frame
        void Update()
        {
            MouseEventProcess();

            if(!_isAllMadeToggle && IsAllBlockMade())
        {
            _isAllMadeToggle = true;
            Debug.Log("-------------All Block Made--------------");
            AllBlockComplete();
        }

        }
    }
