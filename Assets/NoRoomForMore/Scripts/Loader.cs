using UnityEngine;
using CGL.Events;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private StringEventSO onSceneLoadEvent;
    [SerializeField] private string menuSceneName = "MenuBackground";

    void Start()
    {
        // Wait one frame for GameManager to initialize
        Invoke("LoadMenu", 0.1f);
    }

    void LoadMenu()
    {
        if (onSceneLoadEvent != null)
        {
            onSceneLoadEvent.RaiseEvent(menuSceneName);
        }
    }
}