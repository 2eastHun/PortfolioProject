using Google.Protobuf.Protocol;
using System.Collections;
using UnityEngine;

public class MyPlayer : PlayerController
{
    Camera _camera;
    CharacterController _characterController;

    protected float _runSpeed = 5.0f;
    protected float _sprintSpeed = 15.0f;
    protected float _walkSpeed = 3.0f;
    protected float _finalSpeed;
    
    float _smoothness = 10.0f;

    public bool _toggleCameraRotation;

    private PlayerAnimator _playerAnimator;

    Vector3 _moveDirection;

    // 구르기 이동 속도와 거리 설정
    public float rollSpeed = 10.0f;
    public float rollDistance = 10.0f;
    
    private Vector3 rollDirection;

    private float savedHorizontalInput;
    private float savedVerticalInput;

    // 전진 이동 속도와 거리 설정
    public float attackMoveSpeed = 1500.0f;
    public float attackMoveDistance = 1000.0f;
    private float attackDirection;



    // Start is called before the first frame update
    void Start()
    {
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _characterController = this.GetComponent<CharacterController>();

        _playerAnimator = GetComponent<PlayerAnimator>();

        StateMapping();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
            _toggleCameraRotation = true;
        else
            _toggleCameraRotation = false;

        InputKey();
        StateUpdate();

        if(_playerState != PlayerState.Attack1)
            _animator.SetFloat("reverse", 1f);
    }

    private void LateUpdate()
    {
        if (_toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * _smoothness);
        }
    }

    private void InputKey()
    {
        if (_canMove == true)
            InputMovement();

        if (Input.GetMouseButtonDown(0))
        {
            _isAttack = true;
            InputAttack();
        }
        
        if (Input.GetMouseButtonDown(1))
            InputComboAttack();

        if (Input.GetKeyDown(KeyCode.Q))
            InputDefence();
        else if (Input.GetKeyUp(KeyCode.Q))
            _playerAnimator.Defend(false);

        if (Input.GetKeyDown(KeyCode.Space) && _playerState == PlayerState.Walk)
            Roll();

        if (Input.GetKey(KeyCode.E))
            TakeDamage(10);


    }

    void TakeDamage(int damage)
    {
        if(_playerState == PlayerState.Defend)
        {
            _DefenceCount++;
            _playerState = PlayerState.DefendHit;

            _animator.SetTrigger("DefendHit");

            if (_DefenceCount >= 3)
            {
                _DefenceCount = 0;
                _animator.SetTrigger("Dizzy");
            }
        }
        else
        {
            _playerState = PlayerState.Hit;
            _animator.SetTrigger("Hit");
        }
    }

    private void InputMovement()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") == 1)
        {
            _animator.SetBool("Sprint", true);
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _animator.SetBool("Sprint", false);
        }

        if(_playerState == PlayerState.Walk)
            _finalSpeed = _walkSpeed;
        else if(_playerState == PlayerState.Run)
            _finalSpeed = _runSpeed;
        else if (_playerState == PlayerState.Sprint)
            _finalSpeed = _sprintSpeed;
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        _moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        if (!_isRolling)
        {
            _playerAnimator.OnMovement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            // 구르기 중이면 저장된 입력 값을 사용
            _playerAnimator.OnMovement(savedHorizontalInput, savedVerticalInput);
        }

        _characterController.Move(_moveDirection.normalized * _finalSpeed * Time.deltaTime);
    }

    private void InputDefence()
    {
        _playerAnimator.Defend(true);
    }

    private void InputAttack()
    {
        _playerAnimator.Attack();
    }

    private void InputComboAttack()
    {
        _playerAnimator.ComboAttack();

        StartCoroutine(SmoothMoveForward());
    }

    private IEnumerator SmoothMoveForward()
    {
        if (_playerState != PlayerState.ComboAttack1 || _playerState != PlayerState.ComboAttack2 ||
            _playerState != PlayerState.ComboAttack3)
            yield return null;

        float elapsedTime = 0f;
        float duration = attackMoveDistance / attackMoveSpeed;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + transform.forward * attackMoveDistance;

        while (elapsedTime < duration)
        {
            _characterController.Move(transform.forward * attackMoveSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void Roll()
    {
        _isRolling = true;

        _playerAnimator.Roll();

        float step = rollSpeed * Time.deltaTime;
        _characterController.Move(rollDirection * step);

        savedHorizontalInput = Input.GetAxisRaw("Horizontal");
        savedVerticalInput = Input.GetAxisRaw("Vertical");
    }


    private void OnCollisionEnter(Collision collision)
    {

    }

    public void OnChildTriggerEnter(Collider other)
    {
        EnemyPlayer enemy = other.GetComponent<EnemyPlayer>();

        if (_isAttack || _isComboAttack && other.CompareTag("Enemy"))
        {
            if(enemy.GetPlayerState () == PlayerState.Defend)
            {
                _animator.SetFloat("reverse", -1f);
                _animator.SetTrigger("Return");
            }
            
            enemy.TakeDamage(10);
        }
    }
}
