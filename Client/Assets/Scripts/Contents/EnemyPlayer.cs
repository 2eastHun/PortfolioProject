using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyPlayer : PlayerController
{
    private Color _color;
    private SkinnedMeshRenderer _meshRenderer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponent<SkinnedMeshRenderer>();
        _color = _meshRenderer.material.color;
        //_animator.SetBool("Defend", true);

        StateMapping();
    }

    private void Update()
    {
        StateUpdate();
    }

    public PlayerState GetPlayerState()
    {
        return _playerState;
    }

    public void TakeDamage(int damage)
    {
        if (_playerState == PlayerState.Defend)
        {
            _DefenceCount++;

            if (_DefenceCount > 3)
            {
                _DefenceCount = 0;
                _animator.SetTrigger("Dizzy");
            }
            else
            {
                _animator.SetTrigger("DefendHit");
                Debug.Log("DefendHit");
            }

           // Debug.Log($"TakeDamage: {_DefenceCount}");
        }
        else
        {
            _animator.SetTrigger("Hit");

            Debug.Log("Hit");
        }
    }

    private IEnumerator OnHit()
    {
        _meshRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        _meshRenderer.material.color = _color;
    }

}

