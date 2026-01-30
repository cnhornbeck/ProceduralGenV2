using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 5.0f;
    private Vector2 rotation = Vector2.zero;
    private Camera mainCamera;
    private bool isCursorLocked = true;

    private void Start()
    {
        // Initialize camera and lock cursor
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {

        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        // If Escape key is pressed or left mouse button is clicked (and not over a UI element), toggle cursor lock
        if ((Input.GetKeyUp(KeyCode.Escape) || (Input.GetMouseButtonDown(0) && !isCursorLocked)) && !isPointerOverUI)
        {
            isCursorLocked = !isCursorLocked;
        }

        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLocked;

        if (!isCursorLocked)
        {
            return;
        }

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Jump") + Input.GetAxisRaw("Crouch");
        float z = Input.GetAxisRaw("Vertical");

        // Get value of scroll wheel
        float scroll = 0;
        scroll += Input.GetAxisRaw("Mouse ScrollWheel");
        speed = Mathf.Pow(2, scroll * 2) * speed;

        Vector3 moveDirection = transform.right * x + transform.up * y + mainCamera.transform.forward * z;
        transform.position += speed * Time.deltaTime * moveDirection;
    }

    private void HandleRotation()
    {
        rotation.y += Input.GetAxisRaw("Mouse X") * lookSensitivity;
        rotation.x -= Input.GetAxisRaw("Mouse Y") * lookSensitivity;
        rotation.x = Mathf.Clamp(rotation.x, -90, 90);

        // Apply rotation
        transform.localEulerAngles = new Vector3(0f, rotation.y, 0f);
        mainCamera.transform.localEulerAngles = new Vector3(rotation.x, 0f, 0f);
    }
}
