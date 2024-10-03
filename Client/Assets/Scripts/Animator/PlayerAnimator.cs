using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    

    // Update is called once per frame
    public void OnMovement(float horizontal, float vertical)
    {
        _animator.SetFloat("horizontal", horizontal);
        _animator.SetFloat("vertical", vertical);
    }

    public void Roll()
    {
        _animator.SetTrigger("Roll");
    }

    public void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    public void ComboAttack()
    {
        _animator.SetTrigger("ComboAttack");
    }

    public void Hit()
    {
        _animator.SetTrigger("Hit");
    }

    public void Defend(bool key)
    {         
        _animator.SetBool("Defend", key);
    }

    public void DefendHit()
    {
        _animator.SetTrigger("DefendHit");
    }

    public void Dizzy()
    {
        _animator.SetTrigger("Dizzy");
    }


}
