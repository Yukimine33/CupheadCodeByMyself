using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHitFinish : MonoBehaviour
{
    [SerializeField]
    private PotatoBullet _bullet;

	private void _OnHitFinish()
    {
        _bullet.BulletHitFinish();
    }
}
