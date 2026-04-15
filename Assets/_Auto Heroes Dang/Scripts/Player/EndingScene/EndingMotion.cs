using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingMotion : MonoBehaviour
{
    [SerializeField] private float _delayTime = 2f;
    private Animator _animator;

    private float _timer = 0f;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // _animator.SetTrigger("Victory");
        // Invoke("Stop", _delayTime);
    }
    void Update()
    {
        //_timer += Time.deltaTime;

        //if (_timer < _delayTime)
        //{
        //    return;
        //}

        //_timer = 0f;
        //_animator.SetTrigger("Victory");

        
    }

    private void Stop()
    {
        _animator.enabled = false;
    }

    public void MotionStart()
    {
        _animator.SetTrigger("Victory");
        Invoke("Stop", _delayTime);
    }
}
