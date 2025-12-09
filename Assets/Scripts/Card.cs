using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject messageUI;
    public TMP_Text messageText;
    public enum DisplayStyle { Whoosh, Zoom}
    public DisplayStyle displayStyle;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        messageUI.SetActive(false);
    }

    public void DisplayMessage(string message, float duration)
    {
        messageText.text = message;
        rectTransform.localPosition = Vector2.zero; //Reset position

        switch (displayStyle)
        {
            case DisplayStyle.Whoosh:
                WhooshInCard();
                break;
            case DisplayStyle.Zoom:
                ZoomInCard();
                break;
            default:
                break;
        }

        if(duration > 0f) //Dont close if duration was not specified
        {
            StartCoroutine(CloseAfterDuration(duration));
        }
    }

    IEnumerator CloseAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        CloseMessage();
    }

    public void CloseMessage()
    {
        switch (displayStyle)
        {
            case DisplayStyle.Whoosh:
                WhooshOutCard();
                break;
            case DisplayStyle.Zoom:
                ZoomOutCard();
                break;
            default:
                break;
        }
    }

    public void CloseImmediate()
    {
        messageUI.SetActive(false);
    }

    void WhooshInCard()
    {
        rectTransform.transform.localPosition = new Vector3(-1000f, 0f, 0f);
        messageUI.SetActive(true);

        rectTransform.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutElastic);
    }
    void WhooshOutCard()
    {
        rectTransform.DOAnchorPos(new Vector3(1000f, 0f, 0f), 0.5f).SetEase(Ease.InElastic).onComplete += () => { messageUI.SetActive(false); };
    }

    void ZoomInCard()
    {
        rectTransform.localScale = Vector3.zero;
        messageUI.SetActive(true);

        rectTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
    }

    void ZoomOutCard()
    {
        rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce).onComplete += () => { messageUI.SetActive(false); };
    }
}
