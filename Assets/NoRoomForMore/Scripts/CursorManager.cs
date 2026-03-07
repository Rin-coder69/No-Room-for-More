using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private CinemachineInputAxisController axisController;

    void Start()
    {
        axisController.enabled = false;
        LockCursor();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            LockCursor();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            UnlockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        axisController.enabled = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        axisController.enabled = false;
    }
}