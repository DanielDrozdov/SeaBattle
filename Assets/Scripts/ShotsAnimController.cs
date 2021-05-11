using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotsAnimController : MonoBehaviour
{
    private Animator animator;
    private bool IsFirstInit;

    private void OnDisable() {
        if(IsFirstInit == false) {
            IsFirstInit = true;
            animator = GetComponent<Animator>();
        }
    }

    public void OnAnimEnd() {
        animator.enabled = false;
    }
}
