using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CloudType
{
    UpperCloud01 = 0,
    UpperCloud02 = 1,
    UpperCloud03 = 2,
    MiddleCloud01 = 3,
    MiddleCloud02 = 4,
    LowerCloud01 = 5,
    LowerCloud02 = 6,
    LowerCloud03 = 7
}

public class Cloud : MonoBehaviour
{
    private const float UPPER_CLOUD_SPEED_1 = 0.5f;
    private const float UPPER_CLOUD_SPEED_2 = 0.6f;
    private const float UPPER_CLOUD_SPEED_3 = 0.7f;
    private const float MIDDLE_CLOUD_SPEED_1 = 0.8f;
    private const float MIDDLE_CLOUD_SPEED_2 = 0.8f;
    private const float LOWER_CLOUD_SPEED_1 = 0.5f;
    private const float LOWER_CLOUD_SPEED_2 = 0.6f;
    private const float LOWER_CLOUD_SPEED_3 = 0.7f;

    public const float END_POS_X = -15f;
    public const float CHECK_POS_X = -10f;

    public CloudType CurrentType;

    private CloudManager _cloudManager;

    private float _moveSpeed;

    private bool _isAlreadyCheck = false;

    private Vector3 _moveDirection = new Vector3(-1, 0, 0);

	void Start ()
    {
        _cloudManager = GetComponentInParent<CloudManager>();
        _GetMoveSpeed();
    }
	
	void FixedUpdate ()
    {
        transform.position += _moveDirection * _moveSpeed * Time.deltaTime;

        if (!_isAlreadyCheck && transform.position.x - CHECK_POS_X <= 0)
        {
            _cloudManager.GetCloudInstance((int)CurrentType, this.gameObject);
            _isAlreadyCheck = true;
        }

        if(transform.position.x - END_POS_X <= 0)
        {
            _cloudManager.ReturnInstance(this.gameObject);
        }
	}

    private void _GetMoveSpeed()
    {
        switch(CurrentType)
        {
            case CloudType.UpperCloud01:
                _moveSpeed = UPPER_CLOUD_SPEED_1;
                return;
            case CloudType.UpperCloud02:
                _moveSpeed = UPPER_CLOUD_SPEED_2;
                return;
            case CloudType.UpperCloud03:
                _moveSpeed = UPPER_CLOUD_SPEED_3;
                return;
            case CloudType.MiddleCloud01:
                _moveSpeed = MIDDLE_CLOUD_SPEED_1;
                return;
            case CloudType.MiddleCloud02:
                _moveSpeed = MIDDLE_CLOUD_SPEED_2;
                return;
            case CloudType.LowerCloud01:
                _moveSpeed = LOWER_CLOUD_SPEED_1;
                return;
            case CloudType.LowerCloud02:
                _moveSpeed = LOWER_CLOUD_SPEED_2;
                return;
            case CloudType.LowerCloud03:
                _moveSpeed = LOWER_CLOUD_SPEED_3;
                return;
        }
    }

    public void ResetPos(Vector3 pos, CloudType rType, CloudManager rManager)
    {
        _isAlreadyCheck = false;
        transform.position = pos;
        _cloudManager = rManager;
        CurrentType = rType;
    }
}
