using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour
{
    //[SerializeField]
    private PlayerCharacter _player;
    [SerializeField]
    private GameObject _beforeParry;
    [SerializeField]
    private GameObject _afterParry;
    [SerializeField]
    private Transform _parryHitPos;

    private bool _isParry;
    private bool _isAlreadyParry = false;

    private LayerMask _playerLayer;

    void Start ()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        _playerLayer = LayerMask.NameToLayer("Player");
        _player.OnParry += _SetParryState;
    }

    public void _SetParryState(bool isParry)
    {
        _isParry = isParry;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _CheckCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _CheckCollision(collision);
    }

    private void _CheckCollision(Collider2D collision)
    {
        if (collision.gameObject.layer == _playerLayer.value && _isParry && !_isAlreadyParry)
        {
            _isAlreadyParry = true;
            _beforeParry.SetActive(false);
            _afterParry.SetActive(true);
            _player.EnterPause(_parryHitPos.position);
        }
    }
}
