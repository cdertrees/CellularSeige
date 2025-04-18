using System;
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
    
    [Header("Enemies")] 
    public GameObject basicPathogen;
    public List<GameObject> currentPathogens;
    [Header("1. Bacteria 2. Virus 3. Parasite 4. Amoeba 5. Cancer 6. Allergy 7. Fungi")] public List<ScriptablePathogen> pathogenTypes;
    [Header("Level")] 
    public List<Vector3> mapPoints;
    public LineRenderer Map;

    [Header("Camera")] 
    public float cameraMoveSpeed = 1;
    
    [Header("Wave Calculations")]
    [SerializeField] float waitTime = 0.6f;
    [SerializeField]public float waveNum = 0;
      
    public List<ScriptablePathogen> pathogenProbability;
    
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
        waitTime = waitTime * 0.95f;
        float pathogenSpeed = ((1.4f * (Mathf.Pow(2, (0.35f *waveNum))))+ 1) ;
        float pathogenNum = Mathf.Pow(2, waveNum) + 6;

        for (int i = 0; i < pathogenNum; i++)
        {
            //spawn pathogen
            //This will need to be changed to adjust for the different types of enemies.
            GameObject pathogen = Instantiate(basicPathogen);
            Enemy pathogenScript = pathogen.GetComponent<Enemy>();
            //add to list : )
            currentPathogens.Add(pathogen);
            //change speed
            pathogenScript.speed = pathogenSpeed;
            //wait for next one
            yield return new WaitForSeconds(waitTime);
            

        }
        //wait til round is over
        yield return new WaitUntil(() =>(currentPathogens.Count <= 0));
        yield return new WaitForSeconds(1f);
        //need to add in "shop" screen round between this
        EvalWaves();
        StartCoroutine("StartWave");

    }

    public void PathogenEnters(int dmg)
    {
        //Take Damage (Refers to the ORGAN not the individual unit.)
        health -= dmg;
        healthText.text = "HEALTH: " + health;
    }

    public void SomethingBought(int cost)
    {
        DNA -= cost;
        dnaText.text = "DNA: " + DNA;

    }

    public void cameraMove(bool isUp)
    {
        if ((isUp && transform.position.y < 4.7f) || (!isUp && transform.position.y > -8f))
        {
            var pos = transform.position;
            pos.y += isUp ? cameraMoveSpeed*Time.deltaTime : -cameraMoveSpeed*Time.deltaTime;
            transform.position = pos;
        }
        
    }


    private void FixedUpdate()
    {
       
    }

    private void EvalWaves()
    {
        if (waveNum < 6)
        {
            pathogenProbability.Clear();
            //pathogenProbability = new List<ScriptablePathogen>();
            //pathogenProbability
        } 
        else if (waveNum < 11)
        {
            
        } 
        else if (waveNum < 16)
        {
            
        } 
        else if (waveNum < 21)
        {
            
        }
        else
        {
            
        }
        
    }
}
