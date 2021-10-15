using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


class Game : MonoBehaviour
{
    public const float UnitDistance = .5f;

    public static Game Instance;

    [Header("Settings")]
    public bool SwitchToGameMode = false;

    [Header("Colors")]
    public Color Blue;
    public Color Red;

    [Header("Input")]
    public InputActionReference Quit;

    [Header("Misc")]
    public Transform BallContainer;
    public Player Player; 
    
    [Header("Cameras")]
    public CinemachineVirtualCamera FirstPersonCamera;
    public CinemachineVirtualCamera DrivingCamera;
    
    void Start()
    {
        BallContainer = new GameObject().transform;
        Instance = this;
        Quit.action.performed += c => Application.Quit();

#if UNITY_EDITOR
        if (!SwitchToGameMode)
        {
            EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif
    }


}