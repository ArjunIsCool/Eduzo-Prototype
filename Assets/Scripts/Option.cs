using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Threading;

public class Option : MonoBehaviour
{
    public RectTransform optionButton;
    public TMP_Text optionText;

    public GameObject correctUI;
    public ParticleSystem correctVFX;
    public GameObject wrongUI;

    Vector3 optionButtonStartPos;

    private void Awake()
    {
        optionButtonStartPos = optionButton.anchoredPosition;
    }

    public void ShowCorrectUI()
    {
        correctUI.SetActive(true);
    }

    public void ShowWrongUI()
    {
        wrongUI.SetActive(true);
    }

    public void HideFeedbackUI()
    {
        correctUI.SetActive(false);
        wrongUI.SetActive(false);
    }

    public void OnHoverBtnEnter()
    {
        optionButton.DOScale(1.3f, 0.3f);
    }

    public void OnHoverBtnExit()
    {
        optionButton.DOScale(1f, 0.3f);
    }

    public Task MoveTowardsTable(Vector2 targetPos, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        var reg = cancellationToken.Register(() =>
        {
            DOTween.Kill(this);
            tcs.TrySetCanceled(cancellationToken);
        });

        Sequence sequence = DOTween.Sequence();
        sequence.SetId(this);
        sequence.Append(optionButton.DOAnchorPos(targetPos, 1f).SetEase(Ease.OutBounce));
        sequence.AppendInterval(1f);
        sequence.Append(optionButton.DOScale(0f, 0.5f).SetEase(Ease.OutBounce));

        // Complete Task only when tween finishes
        sequence.OnComplete(() =>
        {
            reg.Dispose();
            tcs.TrySetResult(true);
        });

        return tcs.Task;
    }

    public void ResetOptionBtn()
    {
        optionButton.GetComponent<Button>().enabled = true;
        optionButton.localScale = Vector3.one;
        optionButton.anchoredPosition = optionButtonStartPos;
        correctUI.SetActive(false);
        wrongUI.SetActive(false);
    }
}
