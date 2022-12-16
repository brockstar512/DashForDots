using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineTest : MonoBehaviour
{
    public List<RectTransform> points;
    [SerializeField] RectTransform graphContainer;
    [SerializeField] Transform parent;
    public Color32 color;
    private void Awake()
    {
        points = new List<RectTransform>();
        graphContainer = GetComponent<RectTransform>();
    }
    public void DrawLine()
    {
        //CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
        for(int i  =0; i < points.Count-1; i++)
        {
            CreateDotConnection(points[i].anchoredPosition, points[i+1].anchoredPosition);
        }

    }
    //private void ShowGraph(List<int> valueList)
    //{
    //    float graphHeight = graphContainer.sizeDelta.y;
    //    float yMaximum = 100f;
    //    float xSize = 50f;

    //    GameObject lastCircleGameObject = null;
    //    for (int i = 0; i < valueList.Count; i++)
    //    {
    //        float xPosition = xSize + i * xSize;
    //        float yPosition = (valueList[i] / yMaximum) * graphHeight;
    //        GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
    //        if (lastCircleGameObject != null)
    //        {
    //            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
    //        }
    //        lastCircleGameObject = circleGameObject;
    //    }
    //}

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
        //rectTransform.pivot = new Vector2(1f, 0f);
        //rectTransform.
        rectTransform.sizeDelta = new Vector2(distance, 10f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    public void Clear()
    {
        for(int i =0; i < graphContainer.transform.childCount; i++)
        {
            Destroy(graphContainer.GetChild(i).gameObject);
        }
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
