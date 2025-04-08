using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    [Header("Enemies")] public GameObject basicPathogen;
    public List<GameObject> currentPathogens;
    [Header("Wave Calculations")]
    [SerializeField]public float waveNum = 0;
    //[SerializeField] private float waitMultiplier;
    
    
      
    void Start()
    {
        inst = this;
        StartCoroutine("StartWave");
    }
    
    
    IEnumerator StartWave()
    {
        //Keep track of the number of waves
        waveNum++;
        
        //Calculate number of enemies and their types, needs to be complicated later on w/ different enemy types
        //float waitTime = Mathf.Pow(2, (0.2f * waveNum)) - 0.9f;
        float waitTime = ((1.4f * (Mathf.Pow(2, (0.2f *waveNum))))* 0.5f);
        float pathogenSpeed = ((1.4f * (Mathf.Pow(2, (0.2f *waveNum))))+ 1) ;
        float pathogenNum = Mathf.Pow(2, waveNum) + 6;

        print(waitTime);
        print(pathogenSpeed);
        for (int i = 0; i < pathogenNum; i++)
        {
            //spawn pathogen
            //This will need to be changed to adjust for the different types of enemies.
            GameObject pathogen = Instantiate(basicPathogen);
            Enemy pathogenScript = pathogen.GetComponent<Enemy>();
            //add to list : )
            currentPathogens.Add(pathogen);
            //change animation speed
            pathogenScript.anim.speed = pathogenSpeed;
            //wait for next one
            yield return new WaitForSeconds(waitTime);
            

        }

    }
    
  
}
