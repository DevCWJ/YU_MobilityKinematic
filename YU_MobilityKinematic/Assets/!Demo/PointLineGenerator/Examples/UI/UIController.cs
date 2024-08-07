using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowMaker.Examples
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject animatedArrows;
        [SerializeField] private GameObject simpleArrows;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var animated = root.Q<Toggle>("animated-toggle");
            animated.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue)
                {
                    animatedArrows.SetActive(true);
                    simpleArrows.SetActive(false);
                }
                else
                {
                    animatedArrows.SetActive(false);
                    simpleArrows.SetActive(true);
                }
            });

        }
    }
}