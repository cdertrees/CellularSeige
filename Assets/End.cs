using TMPro;
using UnityEngine;

public class End : MonoBehaviour
{

    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var score = GameManager.score;
        text.text = "Great Job!\nYou defended against \n"+ score +" waves of pathogens.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
