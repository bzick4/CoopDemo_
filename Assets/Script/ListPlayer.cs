using UnityEngine;

public class ListPlayer : MonoBehaviour
{
    private GameObject playerListUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.playerListUI.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            this.playerListUI.SetActive(false);
        }
    }
}
