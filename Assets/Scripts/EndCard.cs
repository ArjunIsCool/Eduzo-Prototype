using DG.Tweening;
using TMPro;
using UnityEngine;

public class EndCard : MonoBehaviour
{
    public GameObject messageUI;

    public TMP_Text messageText;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        messageUI.SetActive(false);
    }

    public void DisplayMessage(string message)
    {
        messageText.text = message;

        rectTransform.transform.localPosition = new Vector3(-1000f, 0f, 0f);
        messageUI.SetActive(true);

        rectTransform.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutElastic);
    }
}
