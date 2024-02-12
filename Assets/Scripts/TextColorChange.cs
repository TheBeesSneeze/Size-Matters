using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextColorChange : MonoBehaviour
{
    public string LevelName;
    public TMP_Text LevelNumber;
    public Color CompletedColor;
    public Color ImcompleteColor;

    private void Start()
    {
        ColorSetting();
    }

    public void ColorSetting()
    {
        bool levelDone = PlayerPrefs.GetInt(LevelName + "isDone", 0) == 1;

        if (levelDone)
        {
            LevelNumber.color = CompletedColor;
        }
        else
        {
            LevelNumber.color = ImcompleteColor;
        }
    }
}
