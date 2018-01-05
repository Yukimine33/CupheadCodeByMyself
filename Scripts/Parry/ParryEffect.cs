using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryEffect : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private AnimatorStateInfo _stateInfo;
	
    public void OnParryEffect()
    {
        _animator.Play(0, 0, 0);
    }

	void Update ()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (_stateInfo.normalizedTime >= 1f)
        {
            Destroy(this.gameObject);
        }
    }
}
