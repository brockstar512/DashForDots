using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
public class CenterPlayerDemo : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera camController;
    [SerializeField] Transform  dot;
    public float zoom = 0;

    // Start is called before the first frame update
    void Start()
    {
        zoom = camController.m_Lens.OrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        camController.m_Lens.OrthographicSize = zoom;
    }
    [ContextMenu("Center player")]
    public void CenterPlayer()
    {
        Debug.Log("dfgdhdfgd");
        //camController.transform.position
        Vector3 newPos = new Vector3(dot.transform.position.x, dot.transform.position.y,-10);
        zoom = camController.m_Lens.OrthographicSize;
        DOTween.To(() => zoom, x => zoom = x, 5, .75f).SetEase(Ease.InOutSine);
        camController.transform.DOMove(newPos, .75f).SetEase(Ease.InOutSine);//.SetEase(Ease.InOutSine);
    }
}
