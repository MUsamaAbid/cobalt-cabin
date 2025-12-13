using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCameraController : MonoBehaviour
{
    [SerializeField] private CameraConfig cameraConfig;
    [SerializeField] private bool adjustOnStart = false;

    private Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        
        if (!_camera.orthographic)
        {
            _camera.orthographic = true;
        }
    }

    void Start()
    {
        if (adjustOnStart && cameraConfig != null)
        {
            LevelDataConfig levelData = FindObjectOfType<GameManager>()?.GetCurrentLevelData();
            if (levelData != null)
            {
                AdjustCameraToGrid(levelData.rows, levelData.columns);
            }
        }
    }

    public void AdjustCameraToGrid(int rows, int columns)
    {
        if (cameraConfig == null || _camera == null)
        {
            Debug.LogWarning("DynamicCameraController: Missing camera config or camera component!");
            return;
        }

        float gridWidth = (columns - 1) * cameraConfig.cardSpacing + cameraConfig.cardWidth;
        float gridHeight = (rows - 1) * cameraConfig.cardSpacing + cameraConfig.cardHeight;

        float totalWidth = gridWidth + cameraConfig.leftPadding + cameraConfig.rightPadding;
        float totalHeight = gridHeight + cameraConfig.topPadding + cameraConfig.bottomPadding;

        float screenAspect = (float)Screen.width / Screen.height;
        float requiredAspect = totalWidth / totalHeight;

        float orthographicSize;

        if (screenAspect > requiredAspect)
        {
            orthographicSize = totalHeight / 2f;
        }
        else
        {
            orthographicSize = (totalWidth / screenAspect) / 2f;
        }

        _camera.orthographicSize = orthographicSize;

        float verticalCenter = (cameraConfig.topPadding - cameraConfig.bottomPadding) / 2f;
        float horizontalCenter = (cameraConfig.rightPadding - cameraConfig.leftPadding) / 2f;

        Vector3 newPosition = transform.position;
        newPosition.y = verticalCenter;
        newPosition.x = horizontalCenter;
        transform.position = newPosition;

        Debug.Log($"Camera adjusted for {rows}x{columns} grid. Orthographic size: {orthographicSize:F2}");
    }

    public void SetCameraConfig(CameraConfig config)
    {
        cameraConfig = config;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        if (_camera != null && !_camera.orthographic)
        {
            _camera.orthographic = true;
        }
    }
#endif
}
