using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using DG.Tweening;

public class GraphAnim : MonoBehaviour {

    public List<UILineRenderer> Lines;
    public bool isOpen;
    public float[] linesWidth;
    public float[] indicators;
    public int m_linesCount;
    public float coef;
    GameObject m_graph;
    float ind = 0;
    float x = 65f;
    public void Awake()
    {
        m_graph = Resources.Load("graph_line") as GameObject;
        
    }

    // Use this for initialization
    void Start () {
        for (int i = 0; i < m_linesCount; i++)
        {
            GameObject newLine = Instantiate(m_graph) as GameObject;
            newLine.transform.SetParent(GetComponentInChildren<VerticalLayoutGroup>().transform, false);
            Lines.Add(newLine.GetComponent<UILineRenderer>());
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (isOpen)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (indicators[i] < 0)
                {
                    if (Lines[i].Points[1].x > indicators[i] / coef) Lines[i].Points[1].x -= 20;
                }
                else
                {
                    if (ind <= indicators[i])
                    {
                        ind += coef;
                        Lines[i].Points[1].x = ind / coef;
                        x += 1;
                        
                        Lines[i].GetComponentInChildren<Text>().text = ind.ToString();
                        Lines[i].GetComponentInChildren<Text>().GetComponent<RectTransform>().DOAnchorPos(new Vector2(x, -7f), 5);
                    }
                }
                Lines[i].SetAllDirty();
            }
        }
    }
}
