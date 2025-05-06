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
    public static float waveNum = 0;
      
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

    public List<Unit> Basophils;
    public List<Unit> BCells;
    public List<Unit> Eosinophils;
    public List<Unit> FatCells;
    public List<Unit> IntestinalCells;
    public List<Unit> KillerTCells;
    public List<Unit> Monocytes;
    public List<Unit> Neutrophils;
    public List<Unit> Plasmas;
    public List<Unit> SmoothMuscles;
    public List<Unit> Platelets;
    
    public int basophilDefenseUpgradeCost = 5;
    public int basophilOffenseUpgradeCost = 5;
    
    public int bCellDefenseUpgradeCost = 5;
    public int bCellOffenseUpgradeCost = 5;
    
    public int eosDefenseUpgradeCost = 5;
    public int eosOffenseUpgradeCost = 5;
    
    public int fatDefenseUpgradeCost = 5;
    public int fatOffenseUpgradeCost = 5;
    
    public int intestinalDefenseUpgradeCost = 5;
    public int intestinalOffenseUpgradeCost = 5;
    
    public int killerDefenseUpgradeCost = 5;
    public int killerOffenseUpgradeCost = 5;
    
    public int monoDefenseUpgradeCost = 5;
    public int monoOffenseUpgradeCost = 5;
    
    public int neutroDefenseUpgradeCost = 5;
    public int neutroOffenseUpgradeCost = 5;
    
    public int plasmaDefenseUpgradeCost = 5;
    public int plasmaOffenseUpgradeCost = 5;
    
    public int muscleDefenseUpgradeCost = 5;
    public int muscleOffenseUpgradeCost = 5;
    
    public int plateletDefenseUpgradeCost = 5;
    public int plateletOffenseUpgradeCost = 5;

    public AudioSource BrianAS;
    public List<AudioClip> BrianLines;

    private float pathogenSpeed;

    public GameObject pause;
    public AudioClip alertAlert;
    
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
        pathogenSpeed = 4;
        pause.SetActive(false);

    }

    IEnumerator GainDNA()
    {
        yield return new WaitForSeconds(30f);
        DNA++;
        dnaText.text = "DNA: " + DNA;
        StartCoroutine("GainDNA");

    }

    public void alert()
    {
        AS.PlayOneShot(alertAlert);
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
                pause.SetActive(true);
            }
            else
            {
                pause.SetActive(false);
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
        //((1f * (Mathf.Pow(2, (0.25f *waveNum))))+ 1) - ((additionalSpeedPercent/100)*((1f * (Mathf.Pow(2, (0.25f *waveNum))))+ 1))
         pathogenSpeed = pathogenSpeed * 1.07f;
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
            int defCost = 0;
            int offCost = 0;
             if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Basophil"))
            {
                defCost = basophilDefenseUpgradeCost;
                offCost = basophilOffenseUpgradeCost;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("BCellTest"))

            {
                defCost = bCellDefenseUpgradeCost;
                offCost = bCellOffenseUpgradeCost;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Eosinophil"))

            {
                defCost = eosDefenseUpgradeCost;
                offCost = eosOffenseUpgradeCost;
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Fat"))
            {
                defCost = fatDefenseUpgradeCost;
                offCost = fatOffenseUpgradeCost;
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Intestinal"))
            {
                defCost = intestinalDefenseUpgradeCost;
                offCost = intestinalOffenseUpgradeCost;
            } else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("KillerT"))

            {
                defCost = killerDefenseUpgradeCost;
                offCost = killerOffenseUpgradeCost;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Monocyte"))

            {
                defCost = monoDefenseUpgradeCost;
                offCost = monoOffenseUpgradeCost;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Neutrophil"))

            {
                defCost = neutroDefenseUpgradeCost;
                offCost = neutroOffenseUpgradeCost;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Plasma"))

            {
                defCost = plasmaDefenseUpgradeCost;
                offCost = plasmaOffenseUpgradeCost;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet"))

            {
                defCost = plateletDefenseUpgradeCost;
                offCost = plateletOffenseUpgradeCost;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("SmoothMuscle"))

            {
                defCost = muscleDefenseUpgradeCost;
                offCost = muscleOffenseUpgradeCost;
            }
            UpgradeMenu.SetActive(true);
            //speedUpgradeTxt.text = "Increase speed by 5%\nCosts "+ clickedUnit.speedUpgradeCost+" DNA";
            defenseUpgradeTxt.text = "Increase health by 5%\nCosts "+ defCost+" DNA";
            offenseUpgradeTxt.text = "Increase damage by 5%\nCosts "+ offCost+" DNA";
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

    // public void purchaseSpeedUpgrade()
    // {
    //     
    //     if ((DNA - clickedUnit.speedUpgradeCost) >=0)
    //     {
    //         DNA -= clickedUnit.speedUpgradeCost;
    //         dnaText.text = "DNA: " + DNA;
    //         AS.PlayOneShot(click);
    //         clickedUnit._cooldownTime = clickedUnit._cooldownTime - (clickedUnit._cooldownTime * 0.05f);
    //         clickedUnit.speedUpgradeCost = (clickedUnit.speedUpgradeCost * 2);
    //         speedUpgradeTxt.text = "Increase speed by 5%\nCosts "+ clickedUnit.speedUpgradeCost+" DNA";
    //         upgradesPurchased++;
    //     }
    //    
    //     
    // }

    public void purchaseDefenseUpgrade()
    {
        int defCost = 0;
        int offCost = 0;
        List<Unit> unitList = new List<Unit>();
         if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Basophil"))
            {
                defCost = basophilDefenseUpgradeCost;
                offCost = basophilOffenseUpgradeCost;
                unitList = Basophils;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("BCellTest"))

            {
                defCost = bCellDefenseUpgradeCost;
                offCost = bCellOffenseUpgradeCost;
                unitList = BCells;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Eosinophil"))

            {
                defCost = eosDefenseUpgradeCost;
                offCost = eosOffenseUpgradeCost;
                unitList = Eosinophils;
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Fat"))
            {
                defCost = fatDefenseUpgradeCost;
                offCost = fatOffenseUpgradeCost;
                unitList = FatCells;

            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Intestinal"))
            {
                defCost = intestinalDefenseUpgradeCost;
                offCost = intestinalOffenseUpgradeCost;
                unitList = IntestinalCells;
            } else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("KillerT"))

            {
                defCost = killerDefenseUpgradeCost;
                offCost = killerOffenseUpgradeCost;
                unitList = KillerTCells;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Monocyte"))

            {
                defCost = monoDefenseUpgradeCost;
                offCost = monoOffenseUpgradeCost;
                unitList = Monocytes;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Neutrophil"))

            {
                defCost = neutroDefenseUpgradeCost;
                offCost = neutroOffenseUpgradeCost;
                unitList = Neutrophils;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Plasma"))

            {
                defCost = plasmaDefenseUpgradeCost;
                offCost = plasmaOffenseUpgradeCost;
                unitList = Plasmas;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet"))

            {
                defCost = plateletDefenseUpgradeCost;
                offCost = plateletOffenseUpgradeCost;
                unitList = Platelets;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("SmoothMuscle"))

            {
                defCost = muscleDefenseUpgradeCost;
                offCost = muscleOffenseUpgradeCost;
                unitList = SmoothMuscles;
            }
        if ((DNA - defCost) >=0)
        {
            DNA -= defCost;
            dnaText.text = "DNA: " + DNA;
            AS.PlayOneShot(click);
            
            
            
           

            foreach (var unit in unitList)
            {
                unit._health = unit._health + (unit._health * 0.05f);
                unit._maxHealth = unit._maxHealth + (unit._maxHealth * 0.05f);
                unit.calcHealthBar();
                
            }
            
            defCost = (defCost * 2);
            defenseUpgradeTxt.text = "Increase health by 5%\nCosts "+ defCost+" DNA";
            upgradesPurchased++;
            
             if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Basophil"))
            {
                basophilDefenseUpgradeCost = defCost;
               
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("BCellTest"))

            {
                bCellDefenseUpgradeCost = defCost;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Eosinophil"))

            {
                eosDefenseUpgradeCost = defCost;
                offCost = eosOffenseUpgradeCost;
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Fat"))
            {
               fatDefenseUpgradeCost = defCost;
     
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Intestinal"))
            {
               intestinalDefenseUpgradeCost = defCost;
        
            } else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("KillerT"))

            {
               killerDefenseUpgradeCost = defCost;
        
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Monocyte"))

            { 
                monoDefenseUpgradeCost = defCost;
             
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Neutrophil"))

            {
                neutroDefenseUpgradeCost = defCost;
              
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Plasma"))

            {
                plasmaDefenseUpgradeCost = defCost;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet"))

            {
                plateletDefenseUpgradeCost = defCost;
             
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("SmoothMuscle"))

            {
               muscleDefenseUpgradeCost = defCost;
            
            }
            
        }
    }
    
    public void purchaseOffenseUpgrade()
    {
        
        int defCost = 0;
        int offCost = 0;
        List<Unit> unitList = new List<Unit>();
         if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Basophil"))
            {
                defCost = basophilDefenseUpgradeCost;
                offCost = basophilOffenseUpgradeCost;
                unitList = Basophils;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("BCellTest"))

            {
                defCost = bCellDefenseUpgradeCost;
                offCost = bCellOffenseUpgradeCost;
                unitList = BCells;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Eosinophil"))

            {
                defCost = eosDefenseUpgradeCost;
                offCost = eosOffenseUpgradeCost;
                unitList = Eosinophils;
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Fat"))
            {
                defCost = fatDefenseUpgradeCost;
                offCost = fatOffenseUpgradeCost;
                unitList = FatCells;

            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Intestinal"))
            {
                defCost = intestinalDefenseUpgradeCost;
                offCost = intestinalOffenseUpgradeCost;
                unitList = IntestinalCells;
            } else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("KillerT"))

            {
                defCost = killerDefenseUpgradeCost;
                offCost = killerOffenseUpgradeCost;
                unitList = KillerTCells;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Monocyte"))

            {
                defCost = monoDefenseUpgradeCost;
                offCost = monoOffenseUpgradeCost;
                unitList = Monocytes;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Neutrophil"))

            {
                defCost = neutroDefenseUpgradeCost;
                offCost = neutroOffenseUpgradeCost;
                unitList = Neutrophils;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Plasma"))

            {
                defCost = plasmaDefenseUpgradeCost;
                offCost = plasmaOffenseUpgradeCost;
                unitList = Plasmas;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet"))

            {
                defCost = plateletDefenseUpgradeCost;
                offCost = plateletOffenseUpgradeCost;
                unitList = Platelets;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("SmoothMuscle"))

            {
                defCost = muscleDefenseUpgradeCost;
                offCost = muscleOffenseUpgradeCost;
                unitList = SmoothMuscles;
            }
        if ((DNA - offCost) >=0)
        {
            DNA -= offCost;
            dnaText.text = "DNA: " + DNA;
            AS.PlayOneShot(click);
      
            foreach (var unit in unitList)
            {
                for (int i = 0; i < unit._unitDamages.Count; i++)
                {
                    var temp = (unit._unitDamages[i]) + (unit._unitDamages[i] * 0.05f);
                    unit._unitDamages[i] = temp;
                }
              
                
            }
            offCost = (offCost * 2);
            offenseUpgradeTxt.text = "Increase damage by 5%\nCosts "+ offCost+" DNA";
             upgradesPurchased++;
            
             if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Basophil"))
            {
                basophilOffenseUpgradeCost = offCost;
               
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("BCellTest"))

            {
                bCellOffenseUpgradeCost = offCost;
            } 
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Eosinophil"))

            {
                eosOffenseUpgradeCost = offCost;
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Fat"))
            {
                fatOffenseUpgradeCost = offCost;
     
            } else if (clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Intestinal"))
            {
                intestinalOffenseUpgradeCost = offCost;
        
            } else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("KillerT"))

            {
                killerOffenseUpgradeCost = offCost;
        
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Monocyte"))

            { 
                monoOffenseUpgradeCost = offCost;
             
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Neutrophil"))

            {
                neutroOffenseUpgradeCost = offCost;
              
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Plasma"))

            {
                plasmaOffenseUpgradeCost = offCost;
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet"))

            {
                plateletOffenseUpgradeCost = offCost;
             
            }
            else if(clickedUnit._anim.GetCurrentAnimatorStateInfo(0).IsName("SmoothMuscle"))

            {
                muscleOffenseUpgradeCost = offCost;
            
            }
            
        }
        
        
        
    //     print("p");
    //     if ((DNA - clickedUnit.OffenseUpgradeCost) >=0)
    //     {
    //         DNA -= clickedUnit.OffenseUpgradeCost;
    //         dnaText.text = "DNA: " + DNA;
    //         AS.PlayOneShot(click);
    //         for (int i = 0; i<clickedUnit._unitDamages.Count; i++)
    //         {
    //             var temp = (clickedUnit._unitDamages[i]) + (clickedUnit._unitDamages[i] * 0.05f);
    //             clickedUnit._unitDamages[i] = temp;
    //         }
    //         clickedUnit.OffenseUpgradeCost = (clickedUnit.OffenseUpgradeCost * 2);
    //         offenseUpgradeTxt.text = "Increase health by 5%\nCosts "+ clickedUnit.OffenseUpgradeCost+" DNA";
    //         upgradesPurchased++;
    //     }
    }

    
    private void EvalWaves()
    {   //this is kind of weird way to do it, but gives us complete control over the makeup of each "round"
        if (waveNum < 3)
        {
            if (waveNum == 1)
            {
                BrianAS.PlayOneShot(BrianLines[0]);
                brianText.text = "These guys are single celled organisms! They can be both helpful and harmful to your body. These bacteria are definitely harmful though!";
                rupertText.text= "Bacteria are going to try and destroy your cells while getting to the end of the map be careful!";
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
                BrianAS.PlayOneShot(BrianLines[1]);
                brianText.text = "Viruses are tiny microscopic organisms that can’t reproduce on their own! They go into organisms and use their equipment to reproduce and make more viruses! They are so small they are 100 to 1,000x smaller than your cells!";
                rupertText.text = "Virus are more dangerous than Bacteria and have more health! I hate Corona Virus, thankfully we have vaccines to help us combat it!";
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
                BrianAS.PlayOneShot(BrianLines[2]);
                brianText.text = "Fungi are a type of living creature that are easily identified by their way of spreading and growing through spores. They can become dangerous and infect your body if these spores are able to spread and grow on or inside you!";
                rupertText.text = "Despite their name I don’t like to have fun with these guys.";
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
                BrianAS.PlayOneShot(BrianLines[3]);
                brianText.text = "Allergies are actually not harmful! However, your body thinks they are harmful pathogens here to attack the body and thus attack back and try and defend your body.";
                rupertText.text = "Allergies, I have no idea why we hate these guys but attack!";
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
                BrianAS.PlayOneShot(BrianLines[4]);
                brianText.text = "Amoeba are a unicellular organism that are easily identifiable by their ability to form false feet! They are able to use extensions of their body to move around in what scientists think is one of the most primitive forms of animal locomotion. They are also one of the three types of parasites that can afflict your body!";
                rupertText.text = "I hate those single celled demons, stay away from me ew!";
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
                BrianAS.PlayOneShot(BrianLines[5]);
                brianText.text = "Parasites are a type of organism that needs a host to survive and reproduce. One of the three types of parasites are made primarily of worms that live in your digestive tract! They eat and take nutrients from your body as well reproduce and can even grow to be around 39 inches!";
                rupertText.text = "Look out there are parasites! I hate those gross worms.";

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
                BrianAS.PlayOneShot(BrianLines[6]);
                brianText.text = "Cancer is very dangerous and happens when your cells start reproducing and creating abnormal cells! This creates tumors which are clusters of these malfunctioning cells and can spread around the body!";
                rupertText.text = "Cancer is the worst of all pathogens because they are traitors and were once cells like you and me.";
            }
            
            pathogenProbability.Clear();
            pathogenProbability = new List<ScriptablePathogen>()
            {
                pathogenTypes[0], pathogenTypes[0], pathogenTypes[1], pathogenTypes[1], pathogenTypes[5], pathogenTypes[6], pathogenTypes[2],pathogenTypes[3], pathogenTypes[4]
            };
        }
        

    }
}
