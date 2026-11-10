
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceGameManager : MonoBehaviour
{
    [Header("Dice")]
    [SerializeField] private DiceController[] dice;
    [SerializeField] private AudioSource Winsound;
    [SerializeField] private AudioSource Losesound;

    [Header("UI")]
    [SerializeField] private Button rollButton;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text diceResultText;
    [SerializeField] private TMP_Text attemptsText;

    [Header("Rolling Settings")]
    [SerializeField] private float minimumRollDuration = 1.25f;
    [SerializeField] private float maximumRollDuration = 4f;
    [SerializeField] private float stableDuration = 0.4f;

    [Header("Guaranteed Win")]
    [SerializeField] private float correctionDuration = 0.3f;

    private int consecutiveLosses = 0;
    private bool isRolling = false;

    private void Start()
    {
        if (dice == null || dice.Length != 3)
        {
            Debug.LogError("Exactly three dice must be assigned.");
            return;
        }

        rollButton.onClick.AddListener(StartRoll);

        resultText.text = "READY TO ROLL!";
        diceResultText.text = "0 - 0 - 0";

        UpdateAttemptsUI();
    }

    public void StartRoll()
    {
        if (isRolling)
            return;

        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        isRolling = true;
        rollButton.interactable = false;

        resultText.text = "ROLLING...";

        bool forceJackpot = consecutiveLosses >= 3;
        

        // Physically roll all three dice.
        foreach (DiceController die in dice)
        {
            die.Roll();
        }

        yield return StartCoroutine(WaitForDiceToSettle());

        // Freeze physics before any final correction.
        /*foreach (DiceController die in dice)
        {
            die.StopPhysics();
        }*/
        // Guaranteed jackpot after three consecutive losses.
        if (forceJackpot)
        {
            int winningNumber = Random.Range(1, 7);

            yield return StartCoroutine(
                AlignDiceToWinningNumber(winningNumber)
            );
        }

        // Detect the actual final upward-facing values.
        int[] results = new int[3];

        for (int i = 0; i < dice.Length; i++)
        {
            results[i] = dice[i].GetTopFaceValue();
        }

        diceResultText.text = results[0] + " - " + results[1] + " - " + results[2];

        bool isWin =
            results[0] == results[1] && results[1] == results[2];

        EvaluateResult(isWin);

        rollButton.interactable = true;
        isRolling = false;
    }

    private IEnumerator WaitForDiceToSettle()
    {
        float elapsed = 0f;
        float stableTimer = 0f;

        while (elapsed < maximumRollDuration)
        {
            elapsed += Time.deltaTime;

            bool allSettled = true;

            foreach (DiceController die in dice)
            {
                if (!die.IsSettled())
                {
                    allSettled = false;
                    break;
                }
            }

            if (allSettled && elapsed >= minimumRollDuration)
            {
                stableTimer += Time.deltaTime;

                if (stableTimer >= stableDuration)
                    yield break;
            }
            else
            {
                stableTimer = 0f;
            }
            yield return null;
        }

        // Timeout safety: the dice will be frozen
        // by the calling coroutine.
    }

    private IEnumerator AlignDiceToWinningNumber(int targetValue)
    {
        Quaternion[] startRotations = new Quaternion[3];
        Quaternion[] targetRotations = new Quaternion[3];

        for (int i = 0; i < dice.Length; i++)
        {
            startRotations[i] = dice[i].transform.rotation;

            targetRotations[i] =
                dice[i].GetTargetRotation(targetValue);
        }

        float elapsed = 0f;

        while (elapsed < correctionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / correctionDuration
            );

            // Smooth easing.
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < dice.Length; i++)
            {
                dice[i].SetRotation(
                    Quaternion.Slerp(
                        startRotations[i],
                        targetRotations[i],
                        easedT
                    )
                );
            }

            yield return null;
        }

        for (int i = 0; i < dice.Length; i++)
        {
            dice[i].SetRotation(targetRotations[i]);
        }
    }

    private void EvaluateResult(bool isWin)
    {
        if (isWin)
        {
            consecutiveLosses = 0;

            resultText.text = "JACKPOT!";

            Debug.Log("JACKPOT!");
            Winsound.Play();
        }
        else
        {
            consecutiveLosses++;

            resultText.text = "TRY AGAIN!";

            Debug.Log("You lost this round.");
            Losesound.Play();
        }

        UpdateAttemptsUI();
    }

    private void UpdateAttemptsUI()
    {
        attemptsText.text =
            "Consecutive Losses: " +
            consecutiveLosses + " / 3";
    }
}
