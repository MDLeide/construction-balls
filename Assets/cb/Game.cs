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
    public const float SmallModifier = .99f;
    public const float UnitDistance = .5f;
    public const float HalfUnitDistance = .25f;
    public const float SmallUnitDistance = .495f;

    public static readonly Vector3 UnitCube = new Vector3(
        UnitDistance * SmallModifier,
        UnitDistance * SmallModifier,
        UnitDistance * SmallModifier);

    public static readonly Vector3 SmallUnitCube = new Vector3(
        UnitDistance,
        UnitDistance,
        UnitDistance);

    public static readonly Vector3 HalfUnitCube = new Vector3(
        HalfUnitDistance,
        HalfUnitDistance,
        HalfUnitDistance);

    /// <summary>
    /// this can be used to test if a cell has a block or obstruction in it. it
    /// is more reliable than using the regular version, since it is less likely
    /// to get interference from a neighboring cell.
    /// </summary>
    public static readonly Vector3 SmallHalfUnitCube = new Vector3(
        HalfUnitDistance * SmallModifier,
        HalfUnitDistance * SmallModifier,
        HalfUnitDistance * SmallModifier);

    public static Game Instance;

    [Header("Settings")]
    public bool SwitchToGameMode = false;

    [Header("Warnings")]
    public bool WarnOnInvalidID = true;

    public bool WarnOnMissingHologram = true;

    [Header("Balls")]
    public BallPrototypes BallPrototypes;
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