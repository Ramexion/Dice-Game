using UnityEngine;

public class DiceZoneCheckScript : MonoBehaviour
{
    Vector3 diceVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        diceVelocity = DiceScript.diceVelocity;
    }

    void OnTriggerStay(Collider col)
    {
        if (diceVelocity.x==0f && diceVelocity.y==0f && diceVelocity.z == 0f)
        {
            switch (col.gameObject.name)
            {
                case "Side1":
                    DiceNumScript.dicenum = 6;
                    break;

                case "Side2":
                    DiceNumScript.dicenum = 5;
                    break;

                case "Side3":
                    DiceNumScript.dicenum = 4;
                    break;

                case "Side4":
                    DiceNumScript.dicenum = 3;
                    break;

                case "Side5":
                    DiceNumScript.dicenum = 2;
                    break;

                case "Side6":
                    DiceNumScript.dicenum = 1;
                    break;
            }
        }
    }
}
