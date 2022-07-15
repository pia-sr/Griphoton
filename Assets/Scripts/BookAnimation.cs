using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimation : MonoBehaviour
{
    public Animator animator;

    public void OpenBook()
    {
        animator.SetTrigger("Open");
    }

    public void CloseBook()
    {
        animator.SetTrigger("Close");
    }
}
