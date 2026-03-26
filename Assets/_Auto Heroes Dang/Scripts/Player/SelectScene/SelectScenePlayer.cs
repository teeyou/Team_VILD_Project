using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScenePlayer : Unit
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger("Attack");
    }

    public void PlayMoveAnimationToggle(bool flag)
    {
        _animator.SetBool("Move", flag);
    }

    public void PlaySkillAnimation()
    {
        _animator.SetTrigger("Skill");
    }

    public void PlayVictoryAnimation()
    {
        _animator.SetTrigger("Victory");
    }
    
    public void OnOffAnimator(bool flag)
    {
        _animator.enabled = flag;
    }

    //public void ReturnTpose()
    //{
    //    _animator.Rebind();
    //    _animator.Update(0f);
    //}

    public override void Attack()
    {

    }

    public override void Skill()
    {

    }
 
}
