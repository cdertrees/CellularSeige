using TMPro;
using UnityEngine;

public class End : MonoBehaviour
{

    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var score = GameManager.score;
        text.text = "Great Job!\nYou defended against \n"+ score +" waves of pathogens.\n\n\n Units Placed: " + GameManager.unitsPlaced + "\n\nUpgrades Purchased: "+ GameManager.upgradesPurchased + "\n\nPathogens Defeated: " + GameManager.pathsKilled + "\n\n\nYour units attacked a total number of "+GameManager.timesAttacked + " times!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
