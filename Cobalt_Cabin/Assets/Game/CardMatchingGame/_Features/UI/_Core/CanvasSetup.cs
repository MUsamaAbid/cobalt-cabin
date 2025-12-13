using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasSetup : MonoBehaviour
{
    [Header("Canvas Settings")]
    [Tooltip("The camera to render this canvas with")]
    public Camera renderCamera;
    
    [Tooltip("Distance from camera to canvas plane")]
    [Range(1f, 100f)]
    public float planeDistance = 10f;

    [Header("Auto Setup")]
    [Tooltip("Automatically find Main Camera if not assigned")]
    public bool autoFindCamera = true;

    void Awake()
    {
        SetupCanvas();
    }

    void SetupCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        
        if (canvas == null)
        {
            Debug.LogError("[CanvasSetup] No Canvas component found!");
            return;
        }

        if (renderCamera == null && autoFindCamera)
        {
            renderCamera = Camera.main;
            
            if (renderCamera == null)
            {
                Debug.LogError("[CanvasSetup] No Main Camera found! Please assign render camera manually.");
                return;
            }
        }

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = renderCamera;
        canvas.planeDistance = planeDistance;

        Debug.Log($"[CanvasSetup] Canvas configured: Camera={renderCamera.name}, Plane Distance={planeDistance}");
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetupCanvas();
        }
    }
}
