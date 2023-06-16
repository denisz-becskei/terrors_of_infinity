using System.Collections;
using UnityEngine;

public class DiedZoopController : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(DelayedAnimationStart());
    }

    IEnumerator DelayedAnimationStart()
    {
        yield return new WaitForSeconds(10f);
        animator.Play("GeneralAnimationZoop");
    }
}
