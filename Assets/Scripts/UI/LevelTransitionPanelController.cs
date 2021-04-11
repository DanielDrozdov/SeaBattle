using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTransitionPanelController : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    private static LevelTransitionPanelController Instance;
    private Image selfImage;

    private bool IsPanelClosed;
    private Vector2 startPoint;
    private float ySpawnOffsetInPixels = 100f;
    private float speed = 30f;

    private LevelTransitionPanelController() { }

    private void Awake() {
        Instance = this;
        selfImage = GetComponent<Image>(); 
        float yAddedPos = canvas.transform.position.y - Camera.main.ScreenToWorldPoint(new Vector3(canvas.sizeDelta.x, canvas.sizeDelta.y * 2 + ySpawnOffsetInPixels, 0)).y;
        startPoint = new Vector3(canvas.transform.position.x,canvas.transform.position.y,0) + new Vector3(0, Mathf.Abs(yAddedPos), 0);
        transform.position = startPoint;
    }

    public static LevelTransitionPanelController GetInstance() {
        return Instance;
    }

    public void MoveToCanvasCenter() {
        selfImage.enabled = true;
        StartCoroutine(MoveToPointCoroutine(canvas.transform.position,true));
    }

    public void MoveToStartPoint() {
        StartCoroutine(MoveToPointCoroutine(startPoint,false));
    }

    public bool IsPanelInScreenCenter() {
        return IsPanelClosed;
    }

    private IEnumerator MoveToPointCoroutine(Vector3 targetPos,bool IsMovingToCanvasCenter) {
        targetPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        while(true) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if(transform.position == targetPos) {
                if(IsMovingToCanvasCenter) {
                    IsPanelClosed = true;
                } else {
                    IsPanelClosed = false;
                    selfImage.enabled = false;
                }
                yield break;
            }
            yield return null;
        }
    }
}
