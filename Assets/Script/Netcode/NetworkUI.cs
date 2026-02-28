using UnityEngine;
using TMPro;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private RelayManager relayManager;

    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text joinCodeText;

    public async void OnCreateHostClicked()
    {
        string code = await relayManager.CreateRelayAndSpawn(4);
        joinCodeText.text = "Code: " + code;
    }

    public async void OnJoinClicked()
    {
        string code = joinCodeInput.text;
        await relayManager.JoinRelayAndSpawn(code);
    }
}