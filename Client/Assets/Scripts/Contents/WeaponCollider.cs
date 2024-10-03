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
            // 부모 오브젝트의 OnTriggerEnter 메서드를 호출합니다.
            _player.OnChildTriggerEnter(other);
        }
    }
}
