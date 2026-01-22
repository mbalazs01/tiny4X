using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    public float moveSpeed = 10f;       
    public float zoomSpeed = 5f;       
    public float minZoom = 2f;          
    public float maxZoom = 10f;         

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel"); 

        Vector2 move = new Vector2(horizontal, vertical);
        transform.position += (Vector3)(move * moveSpeed * Time.deltaTime);

        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    } 

    public void MoveCameraToHex(HexTile target)
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.transform.position.x,
            target.transform.position.y, 
            transform.position.z
        );
    }
}
