using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableUnit : ScriptableObject
{
   public String name;
   public int health;
   [Header("1. Bacteria 2. Virus 3. Parasite 4. Amoeba 5. Cancer 6. Allergy 7. Fungi")]public List<int> damages;
   public Animation Animation;
}
