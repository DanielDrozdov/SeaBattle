using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransitionPanelController : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    private static LevelTransitionPanelController Instance;
    private Image selfImage;

    private bool IsCoroutineStarted;
    private bool IsPanelClosed;
    private Vector2 startPoint;
    private float ySpawnOffsetInPixels = 25f;
    private float speed = 40f;

    public delegate void LevelTransitionCloseMethod();
    public delegate void LevelTransitionOpenMethod();

    private LevelTransitionPanelController() { }

    private void Awake() {
        Instance = this;
        selfImage = GetComponent<Image>();
        float yAddedPos = Camera.main.ScreenToWorldPoint(new Vector3(0, canvas.sizeDelta.y + ySpawnOffsetInPixels, 0)).y
            - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        startPoint = new Vector3(canvas.transform.position.x,canvas.transform.position.y,0) + new Vector3(0, Mathf.Abs(yAddedPos), 0);
        transform.position = startPoint;
    }

    private void Start() {
        if(SceneManager.GetActiveScene().name != "MainMenu") {
            transform.position = canvas.transform.position;
            selfImage.enabled = true;
            IsPanelClosed = true;
            MoveToStartPoint();
        }
    }

    public static LevelTransitionPanelController GetInstance() {
        return Instance;
    }

    public bool MoveToCanvasCenter(LevelTransitionCloseMethod CloseWindowMethod = null) {
        if(!IsCoroutineStarted) {
            selfImage.enabled = true;
            StartCoroutine(MoveToPointCoroutine(canvas.transform.position, true, CloseWindowMethod));
            return true;
        }
        return false;
    }

    public bool MoveToStartPoint(LevelTransitionOpenMethod OpenWindowMethod = null) {
        if(!IsCoroutineStarted) {
            StartCoroutine(MoveToPointCoroutine(startPoint, false, OpenWindowDelegate: OpenWindowMethod));
            return true;
        }
        return false;
    }

    public bool IsPanelInScreenCenter() {
        return IsPanelClosed;
    }

    private IEnumerator MoveToPointCoroutine(Vector3 targetPos,bool IsMovingToCanvasCenter,
        LevelTransitionCloseMethod CloseWindowDelegate = null, LevelTransitionOpenMethod OpenWindowDelegate = null) {
        IsCoroutineStarted = true;
        targetPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        if(!IsMovingToCanvasCenter) {
            yield return new WaitForSeconds(0.25f);
        }
        while(true) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if(transform.position == targetPos) {
                IsCoroutineStarted = false;
                if(IsMovingToCanvasCenter) {
                    IsPanelClosed = true;
                    CloseWindowDelegate?.Invoke();
                } else {
                    OpenWindowDelegate?.Invoke();
                    IsPanelClosed = false;
                    selfImage.enabled = false;               
                }
                yield break;
            }
            yield return null;
        }
    }
   
}
