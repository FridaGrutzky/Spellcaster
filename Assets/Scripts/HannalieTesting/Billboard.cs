using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
            mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // 1. Get the direction from the UI to the camera
            Vector3 directionToCamera = mainCameraTransform.position - transform.position;

            // 2. IMPORTANT: Keep the UI vertical (ignore head tilt)
            // If you want it to tilt up/down to face your eyes, keep this as is.
            // If you want it perfectly upright, uncomment the next line:
            // directionToCamera.y = 0; 

            // 3. Look at the camera position, not the camera's rotation
            if (directionToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-directionToCamera);
            }
        }
    }
}