using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("UI/Battalion Stuffs")] 
    public GameObject upgradesBucket;

    public GameObject briansBattalion;
    public Unit clickedUnit;
    public GameObject clickedUnitOBJ;
    public bool inWave;

    public TextMeshProUGUI brianText;
    
    [Header("AH")] 
    public ScriptableUnit nextChanged;

    public float additionalHealthPercent;
    public int fatCells = 0;
    public int intestinalCells = 0;

    public float additionalSpeedPercent=0;

    public float additionalAttackSpeed = 0;

    public AudioSource AS;
    public AudioClip click;
    public int plasmaCells;
    //public Animator shopAnim;
    void Start()
    {
        brianText.text = "";
        inst = this;
        upgradesBucket.SetActive(true);
        briansBattalion.SetActive(false);
        clickedUnit = null;
        //StartCoroutine("StartWave");
        additionalHealthPercent = 0;

    }

    public void AddPercent(int add)
    {
        fatCells += add;
        additionalHealthPercent += 5*add;
    }

    public void AddIntestinal(int add)
    {
        intestinalCells+= add;
        additionalSpeedPercent += 5 * add;
    }

    public void AddPlasma(int add)
    {
        plasmaCells += add;
        additionalAttackSpeed += 5 * add;
    }
    
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
          cameraMove(true);
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraMove(false);
        }
    }


    IEnumerator StartWave()
    {
        upgradesBucket.SetActive(false);
        inWave = true;
        //Keep track of the number of waves
        waveNum++;
        EvalWaves();
        //Calculate number of enemies and their types, needs to be complicated later on w/ different enemy types
        waitTime = waitTime * 0.95f;
        float pathogenSpeed = ((1.4f * (Mathf.Pow(2, (0.35f *waveNum))))+ 1) - ((additionalSpeedPercent/100)*((1.4f * (Mathf.Pow(2, (0.35f *waveNum))))+ 1));
        float pathogenNum = Mathf.Pow(2, waveNum) + 4;

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
      
        upgradesBucket.SetActive(true);
        inWave = false;
    }

    public void PathogenEnters(int dmg)
    {
        //Take Damage (Refers to the ORGAN not the individual unit.)
        
        health -= dmg;
        healthText.text = "HEALTH: " + health;
        if (health >= 0)
        {
            SceneManager.LoadScene("End");
        }
    }

    public void SomethingBought(int cost)
    {
        if ((DNA - cost >= 0))
        {
            
            AS.PlayOneShot(click);
            DNA -= cost;
            dnaText.text = "DNA: " + DNA;
            clickedUnit.ReevaluateType(nextChanged);
            briansBattalion.SetActive(false);
            clickedUnit = null;
        }
       
        
    
    }

    public void CellChanged(ScriptableUnit type)
    {
        nextChanged = type;

    }

    public void cameraMove(bool isUp)
    {
        //move the camera within the bounds of the map
        if ((isUp && transform.position.y < 4.7f) || (!isUp && transform.position.y > -8f))
        {
            var pos = transform.position;
            pos.y += isUp ? cameraMoveSpeed*Time.deltaTime : -cameraMoveSpeed*Time.deltaTime;
            transform.position = pos;
        }
        
    }

    public void changeBrianText(string info)
    {
        brianText.text = info;
    }

    public void startShopping(GameObject selectedUnit)
    {
        AS.PlayOneShot(click);
        briansBattalion.SetActive(true);
        clickedUnitOBJ = selectedUnit;
        clickedUnit = selectedUnit.GetComponent<Unit>();
    }
    public void FinishShopping()
    {
        briansBattalion.SetActive(false);
        clickedUnit = null;
        StartCoroutine("StartWave");
    }
    
    

    private void EvalWaves()
    {   //this is kind of weird way to do it, but gives us complete control over the makeup of each "round"
        if (waveNum < 6)
        {
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
               pathogenTypes[0]
            };
           print("imrunning");
            //pathogenProbability
        } 
        else if (waveNum < 6)
        {
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1],
            };
            print("imrunning2");
        } 
        else if (waveNum < 11)
        {
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6],
            };
            print("imrunning3");
        } 
        else if (waveNum < 16)
        {
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6], pathogenTypes[2],pathogenTypes[3],
            };
        }
        else 
        {
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6], pathogenTypes[2],pathogenTypes[3], pathogenTypes[4]
            };
        }
        

    }
}
