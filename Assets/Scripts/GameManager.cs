using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    [Header("Shop")]
    public float DNA = 100f;
    public float health = 100f;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI dnaText;
    
    
    [Header("Enemies")] public GameObject basicPathogen;
    public List<GameObject> currentPathogens;

    [Header("Level")] 
    public List<Vector3> mapPoints;
    public LineRenderer Map;

    [Header("Camera")] 
    public GameObject Camera;
    public float cameraMoveSpeed = 1;
    [Header("Wave Calculations")]
    [SerializeField] float waitTime = 0.6f;
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
        //float waitTime = ((1.4f * (Mathf.Pow(2, (0.35f *waveNum))))* 0.25f);
        waitTime = waitTime * 0.95f;
        float pathogenSpeed = ((1.4f * (Mathf.Pow(2, (0.35f *waveNum))))+ 1) ;
        float pathogenNum = Mathf.Pow(2, waveNum) + 6;

//        print(waitTime);
//        print(pathogenSpeed);
        for (int i = 0; i < pathogenNum; i++)
        {
            //spawn pathogen
            //This will need to be changed to adjust for the different types of enemies.
            GameObject pathogen = Instantiate(basicPathogen);
            Enemy pathogenScript = pathogen.GetComponent<Enemy>();
            //add to list : )
            currentPathogens.Add(pathogen);
            //change animation speed
            pathogenScript.speed = pathogenSpeed;
            //wait for next one
            yield return new WaitForSeconds(waitTime);
            

        }
        
        yield return new WaitUntil(() =>(currentPathogens.Count <= 0));
        yield return new WaitForSeconds(1f);
        StartCoroutine("StartWave");

    }

    public void PathogenEnters()
    {
     
        health--;
        healthText.text = "HEALTH: " + health;
    }

    public void SomethingBought(int cost)
    {
        DNA -= cost;
        dnaText.text = "DNA: " + DNA;

    }

    public void cameraMove(bool isUp)
    {
        
        var pos = transform.position;
         pos.y += isUp ? cameraMoveSpeed*Time.deltaTime : -cameraMoveSpeed*Time.deltaTime;
         transform.position = pos;
    }
    
  
}
