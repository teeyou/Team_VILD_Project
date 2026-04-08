using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCharacterAnimator : MonoBehaviour
{

    [SerializeField] private Animator[] _animator; 

    private void Start()
    {
        for(int i = 0; i< _animator.Length; i++)
            _animator[i].SetBool("Move", true);
    }

}
