using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public event Action<float> OnGetDamage;
    public event Action<bool> OnActiveMask;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void GetDamage(float damage)
    {
        if(OnGetDamage != null)
        {
            OnGetDamage(damage);
        }
    }

    public void ActiveMask(bool isActive)
    {
        if(OnActiveMask != null)
        {
            OnActiveMask(isActive);
        }
    }
}
