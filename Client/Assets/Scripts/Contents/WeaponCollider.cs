using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    private MyPlayer _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponentInParent<MyPlayer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_player != null && other.CompareTag("Enemy"))
        {
            // �θ� ������Ʈ�� OnTriggerEnter �޼��带 ȣ���մϴ�.
            _player.OnChildTriggerEnter(other);
        }
    }
}
