using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    //========================|   Variables   |===============================================================
    [SerializeField] Transform target;
    const float yOffset = 1.0f;
    const float yFloor = -5.0f;
    Camera cam;
    float z;

    //========================|   Start()   |===============================================================
    private void Start()
    {
        cam = Camera.main;
        z = cam.transform.position.z;
    }

    //========================|   Update()   |===============================================================
    void Update()
    {
        float x = target.position.x;

        float yMin = yFloor + cam.orthographicSize;
        float y = Mathf.Clamp(target.position.y + yOffset, yMin, float.MaxValue);
        transform.position = new Vector3(x, y, z);
    }

}