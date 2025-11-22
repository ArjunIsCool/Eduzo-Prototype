using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public float scaleAmt;
    public float duration;


    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(scaleAmt, duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, duration);
    }
}
