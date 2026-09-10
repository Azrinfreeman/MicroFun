using UnityEngine;


[System.Serializable]
public class ScenarioData
{
    [Header("Scenario Info")]
    public string scenarioName;

    [TextArea(2, 4)]
    public string scenarioDescription;

    public Sprite scenarioImage;

    [Header("Correct Answer")]
    public FactorType correctDominantFactor;

    [Header("Explanation")]
    [TextArea(2, 4)]
    public string correctReason;
}
   

