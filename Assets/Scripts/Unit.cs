using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int health;
    public List<int> damages;
    // public int dmgTypes = 6;
    //Damages 1. Bacteria 2. Virus 3. Parasite 4. Cancer 5. Allergies 6. Fungi
    
    private Animator anim;
    
    void Start()
    {
        reevaluateType();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void reevaluateType()
    {
        //Here will be depositing the new health and dmg and stuff in there
    }
    
    
}
// public enum Types
// {
//     Stem,
//     Basophil,
//     BCell,
//     Bone,
//     Eosinophil,
//     Fat,
//     Intestinal,
//     KillerT,
//     Monocyte,
//     Nerve,
//     Neutrophil,
//     Plasma,
//     Platelet,
//     RedBlood,
//
// }
