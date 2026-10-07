using UnityEngine;

public class RandomMaze : MonoBehaviour
{
    [Header("迷宫尺寸 (必须是奇数，如21, 31)")]
    public int width = 21;
    public int height = 21;

    [Header("路径与墙壁参数")]
    public float spacing = 4.0f;      // 间距
    public float wallThickness = 3.0f; // 墙壁厚度 (关键！)
    public float wallHeight = 1.5f;    // 墙高（压低一点，视野更开阔）

    private int[,] mazeMap; // 1是墙，0是路

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        // 1. 初始化全图都是墙
        mazeMap = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                mazeMap[x, z] = 1;

        // 2. 从 (1,1) 开始挖路
        CarvePassage(1, 1);

        // 👇 3. 强制在正中心挖一个大广场，并作为出口坐标
        int exitX = width / 2;
        int exitZ = height / 2;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int cx = exitX + i;
                int cz = exitZ + j;
                if (cx > 0 && cx < width - 1 && cz > 0 && cz < height - 1)
                {
                    mazeMap[cx, cz] = 0; // 强行把中心周围变成空地
                }
            }
        }

        // 4. 根据地图生成墙壁
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (mazeMap[x, z] == 1) // 如果是墙
                {
                    CreateWall(x, z);
                }
            }
        }

        // 👇 5. 最后在正中心召唤出口！
        CreateExit(exitX, exitZ);
        Debug.Log("迷宫已生成！出口在正中心！");
    }
    void CarvePassage(int x, int z)
    {
        mazeMap[x, z] = 0;
        int[] dirs = { 1, 2, 3, 4 };
        for (int i = 0; i < dirs.Length; i++)
        {
            int temp = dirs[i];
            int randomIndex = Random.Range(i, dirs.Length);
            dirs[i] = dirs[randomIndex];
            dirs[randomIndex] = temp;
        }

        foreach (int dir in dirs)
        {
            int nx = x, nz = z;
            int mx = x, mz = z;

            if (dir == 1) { nx += 2; mx += 1; }
            else if (dir == 2) { nx -= 2; mx -= 1; }
            else if (dir == 3) { nz += 2; mz += 1; }
            else if (dir == 4) { nz -= 2; mz -= 1; }

            if (nx > 0 && nx < width - 1 && nz > 0 && nz < height - 1 && mazeMap[nx, nz] == 1)
            {
                mazeMap[mx, mz] = 0;
                CarvePassage(nx, nz);
            }
        }
    }

    void CreateWall(int x, int z)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // 使用 wallThickness 来加厚墙壁，让它们重新连接起来
        wall.transform.localScale = new Vector3(wallThickness, wallHeight, wallThickness);
        wall.transform.position = new Vector3(x * spacing, wallHeight / 2f, z * spacing);
        wall.transform.SetParent(this.transform);
        wall.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.35f);
        wall.name = "Wall";
    }

    void CreateExit(int x, int z)
    {
        GameObject exitObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // 👇 修改：把它变成一座又高又大的发亮灯塔
        exitObj.transform.localScale = new Vector3(3f, 50f, 3f); // 高度变成50，直冲天际
        exitObj.transform.position = new Vector3(x * spacing, 25f, z * spacing); // 因为高度是50，中心Y轴在25

        exitObj.transform.SetParent(this.transform);
        exitObj.GetComponent<Renderer>().material.color = Color.yellow; // 换成耀眼的黄色
        exitObj.name = "Exit";

        Destroy(exitObj.GetComponent<Collider>());
    }
}