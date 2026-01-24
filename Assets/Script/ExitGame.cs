using UnityEngine;
using UnityEngine.UI;

public class ExitGame : MonoBehaviour
{

    private Button exitButton => GetComponent<Button>();

    private void Awake()
    {
        exitButton.onClick.AddListener(QuitGame);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
