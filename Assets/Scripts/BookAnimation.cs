using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimation : MonoBehaviour
{
    public Animator animator;

    //Function to start the animation for opening the book
    public void OpenBook()
    {
        animator.SetTrigger("Open");
    }

    //Function to start the animation for closing the book
    public void CloseBook()
    {
        animator.SetTrigger("Close");
    }
}
