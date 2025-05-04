using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    
    public static int timesAttacked = 0;
    public static int pathsKilled = 0;
    public static int upgradesPurchased = 0;
    public static int unitsPlaced = 0;
    
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
    public TextMeshProUGUI rupertText;
    
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

    public TextMeshProUGUI speedUpgradeTxt;
    public TextMeshProUGUI defenseUpgradeTxt;
    public TextMeshProUGUI offenseUpgradeTxt;
    
   
    public GameObject UpgradeMenu;

    public AudioSource music;

    public static int score = 0;
    
    //public Animator shopAnim;
    void Start()
    {
        StartCoroutine("GainDNA");
        brianText.text = "";
        rupertText.text = "";
        inst = this;
        upgradesBucket.SetActive(true);
        briansBattalion.SetActive(false);
        clickedUnit = null;
        //StartCoroutine("StartWave");
        additionalHealthPercent = 0;

    }

    IEnumerator GainDNA()
    {
        yield return new WaitForSeconds(30f);
        DNA++;
        dnaText.text = "DNA: " + DNA;
        StartCoroutine("GainDNA");

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            if (Time.timeScale == 0)
            {
                music.Pause();
            }
            else
            {
                music.Play();
            }
        }
        
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
          cameraMove(true);
        } else if (Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.S))
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
        float pathogenSpeed = ((1f * (Mathf.Pow(2, (0.25f *waveNum))))+ 1) - ((additionalSpeedPercent/100)*((1f * (Mathf.Pow(2, (0.25f *waveNum))))+ 1));
        float pathogenNum = Mathf.Pow(1.5f, waveNum) + 2;

        for (int i = 0; i < pathogenNum; i++)
        {
            //spawn pathogen
            //This will need to be changed to adjust for the different types of enemies.
            GameObject pathogen = Instantiate(basicPathogen);
            Enemy pathogenScript = pathogen.GetComponent<Enemy>();
            //add to list : )
            currentPathogens.Add(pathogen);
            //change speed
            pathogenScript.speed = Mathf.Abs(pathogenSpeed);
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
        if (health <= 0)
        {
            score = (int)waveNum;
            SceneManager.LoadScene("End");
        }
    }

    public void end(){SceneManager.LoadScene("End");}
    
    
    public void SomethingBought(int cost)
    {
        if ((DNA - cost >= 0))
        {
            unitsPlaced++;
            AS.PlayOneShot(click);
            DNA -= cost;
            dnaText.text = "DNA: " + DNA;
            clickedUnit.ReevaluateType(nextChanged);
            briansBattalion.SetActive(false);
            rupertText.text = "";
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

    public void changeRupertText(string info)
    {
        rupertText.text = info;
    }
    
    public void resetUnit()
    {
        UpgradeMenu.SetActive(false);
        clickedUnit.Reset();
    }
    
    public void startShopping(GameObject selectedUnit)
    {
        AS.PlayOneShot(click);
        briansBattalion.SetActive(true);
        clickedUnitOBJ = selectedUnit;
        clickedUnit = selectedUnit.GetComponent<Unit>();
        if (!clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Stem"))
        {
            UpgradeMenu.SetActive(true);
            speedUpgradeTxt.text = "Increase speed by 5%\nCosts "+ clickedUnit.speedUpgradeCost+" DNA";
            defenseUpgradeTxt.text = "Increase health by 5%\nCosts "+ clickedUnit.defenseUpgradeCost+" DNA";
            offenseUpgradeTxt.text = "Increase health by 5%\nCosts "+ clickedUnit.OffenseUpgradeCost+" DNA";
        }
        else
        {
            UpgradeMenu.SetActive(false);
        }
    }
    
    
    
    public void FinishShopping()
    {
        briansBattalion.SetActive(false);
        
        clickedUnit = null;
        StartCoroutine("StartWave");
    }


    public void Unclick()
    {
        clickedUnit = null;
    }

    public void purchaseSpeedUpgrade()
    {
        if ((DNA - clickedUnit.speedUpgradeCost) >=0)
        {
            DNA -= clickedUnit.speedUpgradeCost;
            dnaText.text = "DNA: " + DNA;
            AS.PlayOneShot(click);
            clickedUnit._cooldownTime = clickedUnit._cooldownTime - (clickedUnit._cooldownTime * 0.05f);
            clickedUnit.speedUpgradeCost = (clickedUnit.speedUpgradeCost * 2);
            speedUpgradeTxt.text = "Increase speed by 5%\nCosts "+ clickedUnit.speedUpgradeCost+" DNA";
            upgradesPurchased++;
        }
       
        
    }

    public void purchaseDefenseUpgrade()
    {
        if ((DNA - clickedUnit.defenseUpgradeCost) >=0)
        {
            DNA -= clickedUnit.defenseUpgradeCost;
            dnaText.text = "DNA: " + DNA;
            AS.PlayOneShot(click);
            clickedUnit._health = clickedUnit._health + (clickedUnit._health * 0.05f);
            clickedUnit._maxHealth = clickedUnit._maxHealth + (clickedUnit._maxHealth * 0.05f);
            clickedUnit.calcHealthBar();
            clickedUnit.defenseUpgradeCost = (clickedUnit.defenseUpgradeCost * 2);
            defenseUpgradeTxt.text = "Increase health by 5%\nCosts "+ clickedUnit.defenseUpgradeCost+" DNA";
            upgradesPurchased++;
        }
    }
    
    public void purchaseOffenseUpgrade()
    {
        print("p");
        if ((DNA - clickedUnit.OffenseUpgradeCost) >=0)
        {
            DNA -= clickedUnit.OffenseUpgradeCost;
            dnaText.text = "DNA: " + DNA;
            AS.PlayOneShot(click);
            for (int i = 0; i<clickedUnit._unitDamages.Count; i++)
            {
                var temp = (clickedUnit._unitDamages[i]) + (clickedUnit._unitDamages[i] * 0.05f);
                clickedUnit._unitDamages[i] = temp;
            }
            //clickedUnit = clickedUnit._health + (clickedUnit._health * 0.05f);
            clickedUnit.OffenseUpgradeCost = (clickedUnit.OffenseUpgradeCost * 2);
            offenseUpgradeTxt.text = "Increase health by 5%\nCosts "+ clickedUnit.OffenseUpgradeCost+" DNA";
            upgradesPurchased++;
        }
    }

    
    private void EvalWaves()
    {   //this is kind of weird way to do it, but gives us complete control over the makeup of each "round"
        if (waveNum < 3)
        {
            if (waveNum == 1)
            {
                brianText.text = "These guys are single celled organisms! They can be both helpful and harmful to your body. These bacteria are definitely harmful though!";
            }
            
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
            if (waveNum ==3)
            {
                brianText.text = "Viruses are tiny microscopic organisms that can’t reproduce on their own! They go into organisms and use their equipment to reproduce and make more viruses! They are so small they are 100 to 1,000x smaller than your cells!";
                // Something cool about viruses is that scientists can’t even classify them under the current conditions of being alive!
            }
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1],
            };
            print("imrunning2");
        } 
        else if (waveNum < 9)
        {
            if (waveNum ==6)
            {
                brianText.text = "Fungi are a type of living creature that are easily identified by their way of spreading and growing through spores. They can become dangerous and infect your body if these spores are able to spread and grow on or inside you!";
            }
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[6],
            };
            print("imrunning3");
        } 
        else if (waveNum < 11)
        {
            if (waveNum == 9)
            {
                brianText.text = "Allergies are actually not harmful! However, your body thinks they are harmful pathogens here to attack the body and thus attack back and try and defend your body.";
            }
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6],
            };
            print("imrunning3");
        } 
        else if (waveNum < 14)
        {
            if (waveNum == 11)
            {
                brianText.text = "Amoeba are a unicellular organism that are easily identifiable by their ability to form false feet! They are able to use extensions of their body to move around in what scientists think is one of the most primitive forms of animal locomotion. They are also one of the three types of parasites that can afflict your body!";
            }
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6], pathogenTypes[3],
            };
        }
        else if (waveNum < 16)
        {
            if (waveNum == 11)
            {
                brianText.text = "Parasites are a type of organism that needs a host to survive and reproduce. One of the three types of parasites are made primarily of worms that live in your digestive tract! They eat and take nutrients from your body as well reproduce and can even grow to be around 39 inches!";
            }
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6], pathogenTypes[2],pathogenTypes[3],
            };
        }
        else 
        {
            if (waveNum ==  16)
            {
                brianText.text = "Cancer is very dangerous and happens when your cells start reproducing and creating abnormal cells! This creates tumors which are clusters of these malfunctioning cells and can spread around the body!";
            }
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6], pathogenTypes[2],pathogenTypes[3], pathogenTypes[4]
            };
        }
        

    }
}
