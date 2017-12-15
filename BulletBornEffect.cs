using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBornEffect : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private AnimatorStateInfo _stateInfo;

    private void Update()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if(_stateInfo.normalizedTime >= 1f)
        {
            Destroy(this.gameObject);
        }
    }
}
