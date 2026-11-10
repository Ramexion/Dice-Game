using UnityEngine;
using UnityEngine.UI;

public class DiceNumScript: MonoBehaviour
{
    public GameObject Dice;
    Text text;
    public static int dicenum;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = dicenum.ToString();
    }
}
