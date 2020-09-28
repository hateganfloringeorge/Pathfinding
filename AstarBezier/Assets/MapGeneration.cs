using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGeneration : MonoBehaviour
{
    private Grid grid;
    public Transform backgroundPos;
    public Camera camera;
    private Vector2 objPos, mousePos;
    private float mousePosX, mousePosY;
    private Vector3 localScale;

    void Start()
    {
        grid = new Grid(10, 10, 10);
        backgroundPos = GetComponent<Transform>();
        objPos = backgroundPos.transform.position;
        localScale = backgroundPos.localScale;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            mousePos = camera.ScreenToWorldPoint (mousePos);
            
            mousePosX = mousePos.x - objPos.x + localScale.x * .5f;
            mousePosY = mousePos.y - objPos.y + localScale.y * .5f;
            
            grid.SetStartBlock(mousePosX, mousePosY);
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            mousePos = Input.mousePosition;
            mousePos = camera.ScreenToWorldPoint (mousePos);
            
            mousePosX = mousePos.x - objPos.x + localScale.x * .5f;
            mousePosY = mousePos.y - objPos.y + localScale.y * .5f;
            
            grid.SetEndBlock(mousePosX, mousePosY);
        }
    }
}
