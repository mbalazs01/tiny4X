using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton
public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance { get; private set; }
    private Camera cam;
    void Start()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

         cam = GetComponent<Camera>();
    }
    public HexTile GetHex()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
        Vector2 mousePos2D = mouseWorld;

        Collider2D col = Physics2D.OverlapPoint(mousePos2D);
        if (col != null)
        {
            HexTile hex = col.GetComponent<HexTile>();
            return hex; 
        }

        return null;
    }
    public static HexTile GetClickedHex()
    {
        if (Instance != null)
            return Instance.GetHex();
        return null;
    }
}
