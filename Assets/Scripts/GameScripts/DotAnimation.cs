using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OnlineSubMenu;
using UnityEngine.UI;
using DG.Tweening;
namespace DashForDots
{
    public class DotAnimation : MonoBehaviour
    {
        [SerializeField] public List<GameObject> loadingView;
        [SerializeField] public Color darkColor = Color.black;
        [SerializeField] public Ease ease = Ease.InOutExpo;
        [Range(1.2f, 3f)]
        public float scaleLenth = 1.2f;
        [Range(0.2f, 0.5f)]
        public float time;
        public bool changeDotColor = true;
        private Coroutine coroutine;
        private Tween tween;
        private void OnEnable()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(LoadingAnimation());
        }
        int index = 0;
        public void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            for (int i = 0; i < loadingView.Count; i++)
            {
                if (changeDotColor)
                {
                    Graphic graphic = loadingView[i].GetComponent<Graphic>();
                    if (graphic != null)
                    {
                        graphic.color = darkColor;
                    }
                }
                loadingView[i].transform.localScale = Vector3.one;
            }
        }
        private IEnumerator LoadingAnimation()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                index = (index + 1) % loadingView.Count;
                for (int i = 0; i < loadingView.Count; i++)
                {
                    Graphic graphic = loadingView[i].GetComponent<Graphic>();
                    if (graphic != null)
                    {
                        if (index == i)
                        {
                            if (changeDotColor)
                            {
                                graphic.color = darkColor;
                            }
                            loadingView[i].transform.localScale = Vector3.one * scaleLenth;
                            loadingView[i].transform.DOScale(Vector3.one, time).SetEase(ease);

                        }
                        else
                        {
                            graphic.color = Color.white;
                            loadingView[i].transform.localScale = Vector3.one;
                        }
                    }
                }
            }
        }
    }
}