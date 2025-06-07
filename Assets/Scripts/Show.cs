using System;
using UnityEngine;

public class Show : MonoBehaviour
{

    public GameObject screen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        screen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
