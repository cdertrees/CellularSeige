using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public Animator anim;
    public List<String> paths;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var rand = Random.Range(0, paths.Count);
        //anim = GetComponent<Animator>();
        anim.Play(paths[rand]);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
