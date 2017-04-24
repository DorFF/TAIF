using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Gallery : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector]
    public PButton[] m_PButtons;
    public Color32 m_PButtonColor;
    [HideInInspector]
    public CPButton[] m_CPButton;
    [HideInInspector]
    public RectTransform Container;
    [HideInInspector]
    public int elements;
    float ElementsWidth;
    public bool isOpen;

    float currentPos = 0;
    float timer;
    Tween fade;
    float[] steps;

    float LerpTo;
    public int CurrentTarget = 0;
    bool Lerp = false;

    bool active = false;

    // Use this for initialization
    private void Awake()
    {
        m_PButtons = GetComponentsInChildren<PButton>();
        m_CPButton = GetComponentsInChildren<CPButton>();
        if (m_PButtons.Length > 0) m_PButtons[m_PButtons.Length - 1].GetComponent<Image>().color = m_PButtonColor;
        Container = transform.FindChild("Content").GetComponent<RectTransform>();
        ElementsWidth = Container.GetComponent<GridLayoutGroup>().cellSize.x;
        elements = Container.GetComponentsInChildren<Image>().Length;
        float heigth = Container.sizeDelta.y;
        Container.sizeDelta = new Vector2(2300 * elements, heigth);
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

    void Start()
    {
        ResetTimer();
        SetElements(elements);
    }

    float ClosestAnchor(float posX)
    {
        if (active)
        {
            if (Mathf.Abs(posX - currentPos) < (steps[1]) * 0.08f)
            {
                float smallest = Mathf.Abs(posX - steps[0]);
                float target = 0;
                for (int i = 0; i < steps.Length; i++)

                {
                    if (Mathf.Abs(posX - steps[i]) < smallest)
                    {
                        smallest = Mathf.Abs(posX - steps[i]);
                        target = steps[i];
                        CurrentTarget = i;
                    }
                }

                return target;
            }
            else
            {
                if (currentPos - posX > 0)
                {
                    if (CurrentTarget + 1 < steps.Length)
                    {
                        CurrentTarget++;
                //        return steps[CurrentTarget];
                    }
                    return steps[CurrentTarget];
                }
                else
                {
                    if (CurrentTarget != 0)
                    {
                        CurrentTarget--;
                 //       return steps[CurrentTarget];
                    }
                    return steps[CurrentTarget];
                }
            }
        }
        else return 0;
    }

    public void GoToZero()
    {
        CurrentTarget = 0;
        LerpTo = steps[0];
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
            LerpTo = steps[CurrentTarget];
        }
        Lerp = true;
    }

    public void Previous()
    {
        if (CurrentTarget != 0)
        {
            CurrentTarget--;
            LerpTo = steps[CurrentTarget];
        }
        else
            LerpTo = steps[CurrentTarget];
        Lerp = true;

    }

    void ResetTimer()
    {
        timer = (float)Settings.SettingsManager.GetInt("timer.gallery");
        foreach (CPButton btn in m_CPButton)
        {
            btn.SendMessage("UpdateCurrent");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Lerp)
        {
            Container.anchoredPosition = new Vector2(Mathf.Lerp(Container.anchoredPosition.x, LerpTo, Time.deltaTime * 1.5f), Container.anchoredPosition.y);
        }
        if (isOpen)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                AutoChangePict(CurrentTarget);
                ResetTimer();
            }
        }
    }

    public void AutoChangePict(int pos)
    {
        CurrentTarget = (int)Mathf.Repeat(pos + 1, elements);

        GoToPoint(CurrentTarget);
        if (m_PButtons.Length > 0)
        {
            foreach (PButton btn in m_PButtons)
            {
                btn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            m_PButtons[m_PButtons.Length - 1 - CurrentTarget].GetComponent<Image>().DOBlendableColor(m_PButtonColor, 0.25f);
        }
    }

    public void ChangePict(int pos)
    {
        foreach (PButton btn in m_PButtons)
        {
            btn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        StartCoroutine(Change(pos));
    }

    void ChangeColor()
    {
        if (m_PButtons.Length > 0)
        {
            foreach (PButton btn in m_PButtons)
            {
                btn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            m_PButtons[m_PButtons.Length - CurrentTarget - 1].GetComponent<Image>().DOBlendableColor(m_PButtonColor, 0.5f);
        }
    }

    IEnumerator Change(int pos)
    {
        currentPos = Container.anchoredPosition.x;
        CurrentTarget = pos;
        if (CurrentTarget < steps.Length)
        {
            LerpTo = steps[CurrentTarget];
        }
        else GoToZero();
        Lerp = true;
        if (m_PButtons.Length > 0) m_PButtons[m_PButtons.Length - CurrentTarget - 1].GetComponent<Image>().DOBlendableColor(m_PButtonColor, 0.5f);
        yield return null;
    }

    #region Interfaces
    public void OnBeginDrag(PointerEventData eventData)
    {
        Lerp = false;
        currentPos = Container.anchoredPosition.x;
        ResetTimer();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LerpTo = ClosestAnchor(Container.anchoredPosition.x);
        Lerp = true;
        ChangeColor();
        ResetTimer();
    }

    public void OnDrag(PointerEventData eventData)
    {
        ResetTimer();
    }
    #endregion
}
