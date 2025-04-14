using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableUnit", order = 1)]
public class ScriptableUnit : ScriptableObject
{
   public String name;
   public int health;
   public float coolDown;
   [Header("1. Bacteria 2. Virus 3. Parasite 4. Amoeba 5. Cancer 6. Allergy 7. Fungi")]public List<int> damages;
   public AnimationClip animation;
   public UnitSpecial special ;
}
public enum AttackType
{
    Melee,
    Still,
}
public enum UnitSpecial
{
    Slow,
    Antibodies,
    Heal,
    Generate,
    None
}