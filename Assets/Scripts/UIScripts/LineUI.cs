using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LineUI : MonoBehaviour
{
    [SerializeField] Color32 color;
    public List<RectTransform> points;
    RectTransform lineUI;

    private void Awake()
    {
        points = new List<RectTransform>();
        lineUI = GetComponent<RectTransform>();
    }
    public void DrawLine()
    {
        if (points.Count < 2)
            return;

        for (int i = points.Count - 2; i < points.Count - 1; i++)
        {
            CreateDotConnection(points[i].anchoredPosition, points[i + 1].anchoredPosition);
        }
    }
    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(this.transform, false);
        gameObject.GetComponent<Image>().color = this.color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.sizeDelta = new Vector2(distance, 10f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    public void Clear()
    {
        points.Clear();
        points = new List<RectTransform>();
        for (int i = 0; i < lineUI.transform.childCount; i++)
        {
            Destroy(lineUI.GetChild(i).gameObject);
            //turn off
        }
    }

    public void AddLine(RectTransform point)
    {
        points.Add(point);
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
