using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace UnityEngine.UI.Extensions
{
    //[RequireComponent(typeof(ScrollRect))]
    public class HorisontalScrollSnaper2 : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        //private Transform _screensContainer;
        public RectTransform Container;
        public int elements;
        public float ElementsWidth;

        float[] steps;

        float LerpTo;

        public int CurrentTarget = 0;

        float CurrentPos = 0;
        bool Lerp = false;

        bool active = false;
        // Use this for initialization
        void Awake()
        {
            SetElements(elements);
        }

        void Start()
        {
           // Сontainer = this.GetComponent<ScrollRect>().content;
            
        }

        public void SetElements(int value)
        {
            elements = value;
            if (elements != 0)
            {
                steps = new float[elements];
                for (int i = 0; i < steps.Length; i++)
                {
                    steps[i] = 0 - ElementsWidth * i;
                    //Debug.Log(steps[i]);
                }
                active = true;
            }
        }

        float ClosestAnchor(float posX)
        {
            if (active)
            {
                // float smallest = posY-steps[0];
                if (Math.Abs(posX - CurrentPos) < (steps[1]) * 0.08f)
                {
                    float smallest = Math.Abs(posX - steps[0]);
                    float target = 0;
                    for (int i = 0; i < steps.Length; i++)

                    {
                        if (Math.Abs(posX - steps[i]) < smallest)
                        {
                            smallest = Math.Abs(posX - steps[i]);
                            target = steps[i];
                            CurrentTarget = i;
                        }
                    }

                    return target;
                }
                else
                {
                    if (CurrentPos - posX > 0)
                    {
                        if (CurrentTarget + 1 < steps.Length)
                        {
                            CurrentTarget++;
                            return steps[CurrentTarget];
                        }
                        else
                        {
                            return steps[CurrentTarget];
                        }
                    }
                    else
                    {
                        if (CurrentTarget != 0)
                        {
                            CurrentTarget--;
                            return steps[CurrentTarget];
                        }
                        else
                            return steps[CurrentTarget];
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        public void GoToZero()
        {
            CurrentTarget = 0;
            LerpTo = steps[0];
            Lerp = true;
        }

        public void GoToEnd()
        {
            CurrentTarget = elements - 1;
            LerpTo = steps[elements-1];
            Lerp = true;
        }

        public void GoToPoint(int index)
        {
            CurrentTarget = index;
            LerpTo = steps[index];
            Lerp = true;
        }

        public void Next()
        {
            if (CurrentTarget + 1 < steps.Length)
            {
                CurrentTarget++;
                LerpTo = steps[CurrentTarget];
            }
            else
            {
                GoToZero();
                //LerpTo = steps[CurrentTarget];
            }
            Lerp = true;
            Sfx.GalleryClick();
        }

        public void Previous()
        {
            if (CurrentTarget != 0)
            {
                CurrentTarget--;
                LerpTo = steps[CurrentTarget];
            }
            else
            {
                GoToEnd();
                //LerpTo = steps[CurrentTarget];
            }
            Lerp = true;
            Sfx.GalleryClick();
        }
        // Update is called once per frame
        void Update()
        {
            if (Lerp)
            {
                //Container.anchoredPosition = new Vector2(Container.anchoredPosition.x, Mathf.Lerp(Container.anchoredPosition.y, LerpTo, Time.deltaTime * 2));
                Container.anchoredPosition = new Vector2(Mathf.Lerp(Container.anchoredPosition.x, LerpTo, Time.deltaTime * 1.5f), Container.anchoredPosition.y);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Next();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Previous();
            }

            int number = CurrentTarget + 1;
        }
        #region Interfaces
        public void OnBeginDrag(PointerEventData eventData)
        {
            // StopAllCoroutines();
            Lerp = false;
            CurrentPos = Container.anchoredPosition.x;
            //Container.DOKill();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // StartCoroutine(WaitForLerp());
            LerpTo = ClosestAnchor(Container.anchoredPosition.x);
            Lerp = true;

            //Container.DOAnchorPosY(LerpTo, 0.5f);
            //Debug.Log(LerpTo);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }
        #endregion

    }
}
