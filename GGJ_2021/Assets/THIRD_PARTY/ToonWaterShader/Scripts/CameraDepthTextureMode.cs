using UnityEngine;

public class CameraDepthTextureMode : MonoBehaviour 
{
    [SerializeField]
    private DepthTextureMode depthTextureMode;

    private void OnValidate()
    {
        SetCameraDepthTextureMode();
    }

    private void Awake()
    {
        SetCameraDepthTextureMode();
    }

    private void SetCameraDepthTextureMode()
    {
        GetComponent<Camera>().depthTextureMode = depthTextureMode;
        
        // #if UNITY_EDITOR
        // if (UnityEditor.SceneView.currentDrawingSceneView)
        // {
        //     UnityEditor.SceneView.currentDrawingSceneView.camera.depthTextureMode = depthTextureMode;
        // }
        // #endif
    }
}
