using UnityEngine;

namespace Eduzo.Games.Tables.Utility
{
    public class TablesAutoDestruct : MonoBehaviour
    {
        public float time;

        void Start()
        {
            Destroy(gameObject, time);
        }
    }
}
