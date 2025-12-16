using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Eduzo.Games.Tables.UI
{
    public class TablesAnimatedBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
}