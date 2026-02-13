using UnityEngine;
using UnityEngine.UI;

public class VisualButtonUI : MonoBehaviour
{
    public Button[] visualButtons;

    private void Start()
    {
        for (int i = 0; i < visualButtons.Length; i++)
        {
            int index = i;
            visualButtons[i].onClick.AddListener(() =>
            {
                PlayerVisualManagerNetcode.Instance.ChooseVisual(index);
                Debug.Log("Выбран визуал " + index);
            });
        }
    }
}