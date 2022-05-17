using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rose.Daphne.ARCollect.Utility
{
    public class ImageColorHelper : MonoBehaviour
    {
        [SerializeField]
        private Image target = null;

        [SerializeField]
        private List<Color> colors = new List<Color>();

        public void SetColor(int index)
        {
            if (index < colors.Count)
            {
                target.color = colors[index];
            }
        }
    }
}

