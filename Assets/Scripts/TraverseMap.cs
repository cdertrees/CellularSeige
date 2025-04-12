using System;
using UnityEngine;

public class TraverseMap : MonoBehaviour
{
    public bool isUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnMouseOver()
    {
        GameManager.inst.cameraMove(isUp);
    }
}
