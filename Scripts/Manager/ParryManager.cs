using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryManager : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter _player;
    [SerializeField]
    private GameObject _parryEffectPrefab;
    [SerializeField]
    private GameObject _parryHitPrefab;

	void Start ()
    {
        _player.OnEnableParryEffect += _OnEnableParryEffect;
    }
	
    private GameObject _GetInstance(GameObject prefabObject)
    {
        return Instantiate(prefabObject);
    }

	private void _OnEnableParryEffect(Vector2 effectPos, Vector2 hitPos)
    {
        var parryEffect = _GetInstance(_parryEffectPrefab);
        parryEffect.transform.position = new Vector3(effectPos.x, effectPos.y, -1.5f);
        parryEffect.GetComponent<ParryEffect>().OnParryEffect();

        var parryHit = _GetInstance(_parryHitPrefab);
        parryHit.transform.position = new Vector3(hitPos.x, hitPos.y, -1f);
        parryHit.GetComponent<ParryEffect>().OnParryEffect();
    }
}
