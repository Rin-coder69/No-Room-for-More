using UnityEngine;
using Unity.Cinemachine;

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
        // re-lock if clicked (e.g. after alt-tabbing)
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            LockCursor();

        // unlock on escape
        if (Input.GetKeyDown(KeyCode.Escape))
            UnlockCursor();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        axisController.enabled = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        axisController.enabled = false;
    }
}
