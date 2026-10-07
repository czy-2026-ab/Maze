using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 150.0f;
    public GameObject winPanel; // 用来拖拽UI面板
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 隐藏鼠标并锁定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 游戏开始时，隐藏通关面板（防开局弹窗）
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 1. 鼠标转头
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);

        // 2. 空格向前冲
        if (Input.GetKey(KeyCode.Space))
        {
            controller.Move(transform.forward * speed * Time.deltaTime);
        }

        // 3. 按 Esc 解锁鼠标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 4. 传送作弊键（按 F 直接飞到出口附近）
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject exit = GameObject.Find("Exit");
            if (exit != null)
            {
                controller.enabled = false; // 传送前关掉碰撞，防止卡墙
                transform.position = exit.transform.position + new Vector3(3, 1, 3);
                controller.enabled = true;
            }
        }

        // 👇 5. 核心！检测是否靠近出口，并弹出 UI！
        GameObject exitObj = GameObject.Find("Exit");
        if (exitObj != null)
        {
            // 计算玩家和出口的距离
            float dist = Vector3.Distance(transform.position, exitObj.transform.position);

            // 距离小于 2 米就触发（走到黄方块脸上即可）
            if (dist < 2.0f)
            {
                if (winPanel != null)
                {
                    winPanel.SetActive(true); // 弹出通关面板
                }
            }
        }
    }
}