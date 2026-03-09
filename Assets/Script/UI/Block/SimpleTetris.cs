using UnityEngine;
using UnityEngine.UI;

public class SimpleTetris : MonoBehaviour
{
    [Header("UI 组件引用")]
    public RectTransform gameBoardPanel; 
    public GameObject blockPrefab;       
    public Button btnLeft;
    public Button btnRight;
    public Button btnDown;
    public Button btnReset;

    [Header("游戏设置")]
    public int width = 10;   
    public int height = 20;  
    public float fallSpeed = 0.8f; 
    public float cellSize = 40f; // 新增：手动指定格子大小，用于计算面板尺寸

    private int[,] gridData;
    private Image[,] gridImages;
    
    private int currentX;
    private int currentY;
    
    private float fallTimer;
    private bool isGameOver = false;

    void Start()
    {
        btnLeft.onClick.AddListener(() => TryMove(-1, 0));
        btnRight.onClick.AddListener(() => TryMove(1, 0));
        btnDown.onClick.AddListener(() => TryMove(0, 1)); 
        btnReset.onClick.AddListener(InitGame);

        InitGame();
    }

    void Update()
    {
        if (isGameOver) return;

        fallTimer += Time.deltaTime;
        if (fallTimer >= fallSpeed)
        {
            fallTimer = 0;
            TryMove(0, 1); 
        }
    }

    void InitGame()
    {
        isGameOver = false;
        fallTimer = 0;

        // ---------------------------------------------------
        // 关键修正：强制设置 GridLayoutGroup 属性，防止自动排列导致错位
        // ---------------------------------------------------
        GridLayoutGroup glg = gameBoardPanel.GetComponent<GridLayoutGroup>();
        if (glg == null) glg = gameBoardPanel.gameObject.AddComponent<GridLayoutGroup>();

        glg.cellSize = new Vector2(cellSize, cellSize);
        glg.spacing = Vector2.zero; // 间距设为0
        glg.startCorner = GridLayoutGroup.Corner.UpperLeft; // 从左上角开始
        glg.childAlignment = TextAnchor.UpperLeft; // 内容对齐左上
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount; // 强制固定列数
        glg.constraintCount = width; // 列数严格等于 width

        // 强制设置面板大小，使其刚好容纳所有格子（防止面板太宽导致自动换行计算错误）
        gameBoardPanel.sizeDelta = new Vector2(width * cellSize, height * cellSize);

        // ---------------------------------------------------
        // 清理旧格子
        // ---------------------------------------------------
        foreach (Transform child in gameBoardPanel)
        {
            Destroy(child.gameObject);
        }

        // ---------------------------------------------------
        // 初始化数组
        // ---------------------------------------------------
        gridData = new int[width, height];
        gridImages = new Image[width, height];

        // ---------------------------------------------------
        // 生成格子
        // 顺序：先遍历行，再遍历列 -> 对应 UI 从左上到右下的排列
        // y=0 是顶部，y=height-1 是底部
        // ---------------------------------------------------
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject go = Instantiate(blockPrefab, gameBoardPanel);
                gridImages[x, y] = go.GetComponent<Image>();
                gridData[x, y] = 0; 
            }
        }

        SpawnBlock();
        DrawGrid();
    }

    void SpawnBlock()
    {
        currentX = width / 2; 
        currentY = 0;         

        if (gridData[currentX, currentY] == 1)
        {
            isGameOver = true;
            //Debug.Log("游戏结束！");
            return;
        }

        gridData[currentX, currentY] = 2; 
    }

    void TryMove(int dx, int dy)
    {
        if (isGameOver) return;

        gridData[currentX, currentY] = 0;

        int newX = currentX + dx;
        int newY = currentY + dy;

        bool canMove = true;

        // 边界检测
        if (newX < 0 || newX >= width) canMove = false;
        if (newY >= height) canMove = false; // 触底

        // 碰撞检测 (只有当坐标合法时才检测数组)
        if (canMove && gridData[newX, newY] == 1)
        {
            canMove = false;
        }

        if (canMove)
        {
            currentX = newX;
            currentY = newY;
            gridData[currentX, currentY] = 2;
        }
        else
        {
            if (dy > 0) // 如果是下落失败
            {
                gridData[currentX, currentY] = 1; // 变成堆叠方块
                CheckLines();   
                SpawnBlock();   
            }
            else // 如果是左右移动失败
            {
                gridData[currentX, currentY] = 2; // 恢复原状
            }
        }

        DrawGrid();
    }

    void CheckLines()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            bool isFull = true;
            for (int x = 0; x < width; x++)
            {
                if (gridData[x, y] == 0 || gridData[x, y] == 2)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                ClearLine(y);
                y++; // 重新检查当前行（因为上面的行落下来了）
            }
        }
    }

    void ClearLine(int row)
    {
        for (int y = row; y > 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                gridData[x, y] = gridData[x, y - 1];
            }
        }
        // 最顶行清空
        for (int x = 0; x < width; x++)
        {
            gridData[x, 0] = 0;
        }
    }

    void DrawGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int val = gridData[x, y];
                Image img = gridImages[x, y];

                if (val == 0)
                {
                    img.color = new Color(0, 0, 0, 0); 
                }
                else if (val == 1)
                {
                    img.color = Color.white; 
                }
                else if (val == 2)
                {
                    img.color = Color.cyan; 
                }
            }
        }
    }
}
