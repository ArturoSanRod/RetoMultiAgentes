using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StepCounter : MonoBehaviour
{
    private int steps = 0;
    public TextMeshProUGUI stepText;

    void Start()
    {
        UpdateStepText();
    }

    public void IncrementStep()
    {
        steps++;
        UpdateStepText();
    }

    void UpdateStepText()
    {
        stepText.text = "Steps: " + steps;
    }
}
