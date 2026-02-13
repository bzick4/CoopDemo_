using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;           // на этом же объекте
    [SerializeField] private Image progressFill;                // optional, если есть полоска
    [SerializeField] private TextMeshProUGUI statusText;        // optional

    [SerializeField] private float minShowTime = 1.5f;          // минимум покажем, даже если быстро
    [SerializeField] private float fakeSpeed = 0.4f;            // если полоска

    private bool isShown = true;
    private float startTime;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        startTime = Time.time;

        // Показываем сразу
        Show(true);
    }

    private void Update()
    {
        // Проверяем каждый кадр — дешёво и просто
        if (!isShown) return;

        // 1. Есть ли уже локальный игрок?
        bool playerReady = IsLocalPlayerSpawned();

        float elapsed = Time.time - startTime;

        // Если игрок готов И прошло минимум времени → скрываем
        if (playerReady && elapsed >= minShowTime)
        {
            StartCoroutine(HideWithFade(0.5f));
            return;
        }

        // Fake прогресс (если есть полоска)
        if (progressFill != null)
        {
            float fakeProg = Mathf.Clamp01((elapsed * fakeSpeed) % 1f);  // циклический или линейный
            progressFill.fillAmount = fakeProg;
        }

        if (statusText != null)
        {
            statusText.text = playerReady ? "Готово!" : "Ожидание спавна персонажа...";
        }
    }

    private bool IsLocalPlayerSpawned()
    {
        var manager = PlayerVisualManagerNetcode.Instance;
        if (manager == null) return false;

        return manager.HasLocalPlayerVisual();  // или manager.localPlayerVisual != null
    }

    private IEnumerator HideWithFade(float duration)
    {
        isShown = false;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Debug.Log("[LoadingScreen] Экран загрузки скрыт — локальный игрок заспавнился");
    }

    private void Show(bool show)
    {
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.blocksRaycasts = show;
        canvasGroup.interactable = show;
    }
}