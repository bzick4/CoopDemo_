using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NameInputUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;

    private PlayerNameDisplay _targetPlayer;

    public void ShowForPlayer(PlayerNameDisplay player)
    {
        _targetPlayer = player;
        gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    private void OnConfirmClicked()
    {
        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        Debug.Log($"[NameInputUI] Подтверждено имя: '{playerName}' для игрока {_targetPlayer?.name}");

        // Присваиваем имя персонажу
        if (_targetPlayer != null)
        {
            _targetPlayer.SetName(playerName);
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }
}