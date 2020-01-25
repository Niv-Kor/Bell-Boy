using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplierIntensity : MonoBehaviour
{
    [System.Serializable]
    public struct IntensityLevel
    {
        [Tooltip("The minimum value from which to apply the color.")]
        [SerializeField] public int minValue;

        [Tooltip("The color to apply the multiplier.")]
        [SerializeField] public Color color;
    }

    [SerializeField] List<IntensityLevel> levels;

    private TextMeshProUGUI textMesh;
    private Stack<IntensityLevel> levelsStack;

    private void Start() {
        this.textMesh = GetComponent<TextMeshProUGUI>();

        //push all levels to stack
        this.levelsStack = new Stack<IntensityLevel>();
        for (int i = levels.Count - 1; i >= 0; i--)
            levelsStack.Push(levels[i]);

        //modify the first level so it can be used automatically
        IntensityLevel firstLevel = levelsStack.Peek();
        firstLevel.minValue = 0;
        Set(1);
    }

    public void Set(int value) {
        textMesh.text = "x" + value;

        if (levelsStack.Count > 0) {
            IntensityLevel level = levelsStack.Peek();

            if (value >= level.minValue) {
                levelsStack.Pop();
                textMesh.color = level.color;
            }
        }
    }
}