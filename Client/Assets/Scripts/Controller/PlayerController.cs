using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle = 0,
    Walk = 1,
    Run = 2,
    Sprint = 3,
    Roll = 4,
    Defend = 5,
    Attack1 = 6,
    Attack2 = 7,
    ComboAttack1 = 8,
    ComboAttack2 = 9,
    ComboAttack3 = 10,
    Hit = 11,
    DefendHit = 12,
};


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6.0f;        // 이동 속도
    public float dashSpeed = 2.0f;
    public float rotationSpeed = 300.0f;  // 회전 속도

    protected bool isDash = false;
    protected bool isRotating = false;    // 현재 회전 중인지 여부
    protected Quaternion targetRotation;  // 목표 회전

    protected PlayerState _playerState = PlayerState.Idle;

    PositionInfo _positionInfo = new PositionInfo();
    
    protected Animator _animator;
    protected string currentState;

    protected bool _canMove = true;

    protected Dictionary<string, PlayerState> _stateMapping;

    public bool _isAttack = false;
    public bool _isRolling = false;
    public bool _isComboAttack = false;

    public TrailRenderer _trailRenderer;
    protected int _DefenceCount = 0;


    private void Start()
    {
        
    }
    protected void StateMapping()
    {
        _stateMapping = new Dictionary<string, PlayerState>
        {
            {"Idle_SwordShield", PlayerState.Idle},
            {"WalkBack_SwordShield", PlayerState.Walk},
            {"WalkLeft_SwordShield", PlayerState.Walk},
            {"WalkRight_SwordShield", PlayerState.Walk},
            {"Run_SwordShield", PlayerState.Run},
            {"Sprint_SwordShield", PlayerState.Sprint},
            {"Roll_SwordShield", PlayerState.Roll},
            {"RollBack_SwordShield", PlayerState.Roll},
            {"RollLeft_SwordShield", PlayerState.Roll},
            {"RollRight_SwordShield", PlayerState.Roll},
            {"Defend_SwordShield", PlayerState.Defend},
            {"Attack01", PlayerState.Attack1},
            {"Attack02", PlayerState.Attack2},
            {"Combo01_SwordShield", PlayerState.ComboAttack1},
            {"Combo02_SwordShield", PlayerState.ComboAttack2},
            {"Combo03_SwordShield", PlayerState.ComboAttack3},
            {"GetHit_SwordShield", PlayerState.Hit},
            {"DefendHit_SwordShield", PlayerState.DefendHit},
        };
    }

    protected void StateUpdate()
    {
        AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        string clipName = clipInfo[0].clip.name;

        if (_stateMapping.TryGetValue(clipName, out PlayerState newState))
        {
            _playerState = newState;
            //Debug.Log(_playerState);
        }

     
        if(_playerState == PlayerState.Attack1 || _playerState == PlayerState.Attack2 || _playerState == PlayerState.Defend
            || _playerState == PlayerState.Roll || _playerState == PlayerState.ComboAttack1 || _playerState == PlayerState.ComboAttack2 ||
            _playerState == PlayerState.ComboAttack3)
        {
            _canMove = false;
        }
        else
        {
            _canMove = true;
        }

        if (_playerState == PlayerState.Attack1)
        {
            _trailRenderer.startColor = Color.yellow; 
            _trailRenderer.endColor = Color.white;
        }
        else if(_playerState == PlayerState.ComboAttack1)
        {
            _trailRenderer.startColor = Color.red;
            _trailRenderer.endColor = Color.yellow;
        }
    }

    public void OnAttackAnimationStart()
    {
        _isAttack = true;
    }

    public void OnAttackAnimationEnd()
    {
        _isAttack = false;
    }

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            Pos = new Vector3(value.PosX, value.PosY, value.PosZ);
            Dir = new Vector3(value.MoveDirPosX, value.PosY, value.PosZ);
        }
    }

    public Vector3 Pos
    {
        get
        {
            return new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
        }

        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y && PosInfo.PosZ == value.z)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            PosInfo.PosZ = value.z;
        }
    }

    public Vector3 Dir
    {
        get
        {
            return new Vector3(PosInfo.MoveDirPosX, PosInfo.MoveDirPosY, PosInfo.MoveDirPosZ);
        }

        set
        {
            if (PosInfo.MoveDirPosX == value.x && PosInfo.MoveDirPosY == value.y && PosInfo.MoveDirPosZ == value.z)
                return;

            PosInfo.MoveDirPosX = value.x;
            PosInfo.MoveDirPosY = value.y;
            PosInfo.MoveDirPosZ = value.z;
        }
    }

    
}
