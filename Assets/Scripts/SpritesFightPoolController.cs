using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesFightPoolController : MonoBehaviour
{
    [SerializeField] private GameObject HitCrossSprite;
    [SerializeField] private GameObject NothingInCellSprite;
    private Queue<GameObject> hitCrossSpritesPool;
    private Queue<GameObject> nothingInCellSpritesPool;
    private readonly int hitCrossSpritesCount = 40;
    private readonly int nothingInCellSpritesCount = 160;

    private static SpritesFightPoolController Instance;

    private SpritesFightPoolController() { }

    private void Awake() {
        Instance = this;
        FillPools();
    }

    public static SpritesFightPoolController GetInstance() {
        return Instance;
    }

    public GameObject GetHitCrossSprite() {
        return hitCrossSpritesPool.Dequeue();
    }

    public GameObject GetNothingInCellSprite() {
        return nothingInCellSpritesPool.Dequeue();
    }

    private void FillPools() {
        hitCrossSpritesPool = new Queue<GameObject>(hitCrossSpritesCount);
        nothingInCellSpritesPool = new Queue<GameObject>(nothingInCellSpritesCount);
        for(int i = 0;i < hitCrossSpritesCount;i++) {
            GameObject sprite = Instantiate(HitCrossSprite, transform);
            sprite.SetActive(false);
            hitCrossSpritesPool.Enqueue(sprite);
        }
        for(int i = 0; i < nothingInCellSpritesCount; i++) {
            GameObject sprite = Instantiate(NothingInCellSprite, transform);
            sprite.SetActive(false);
            nothingInCellSpritesPool.Enqueue(sprite);
        }
    }
}
