using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform _objectToFollow;
    public float _followSpeed = 20f;
    public float _sensitivity = 100f;
    public float _clampAngle = 70f;

    public Transform _realCamera;
    public Vector3 _dirNormalized;
    public Vector3 _finalDir;
    public float _minDistance, _maxDistance;
    public float _finalDistance;
    public float _smoothness = 10f;

    private float _rotX, _rotY;


    // Start is called before the first frame update
    void Start()
    {
        _rotX = transform.localRotation.eulerAngles.x;
        _rotY = transform.localRotation.eulerAngles.y;

        _dirNormalized = _realCamera.localPosition.normalized;
        _finalDistance = _realCamera.localPosition.magnitude;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        _rotX += -(Input.GetAxis("Mouse Y")) * _sensitivity * Time.deltaTime;
        _rotY += Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;

        _rotX = Mathf.Clamp(_rotX, -_clampAngle, _clampAngle);
        Quaternion rot = Quaternion.Euler(_rotX, _rotY, 0);
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _objectToFollow.position, _followSpeed * Time.deltaTime);

        _finalDir = transform.TransformPoint(_dirNormalized * _maxDistance);

        RaycastHit hit;

        if (Physics.Linecast(transform.position, _finalDir, out hit))
            _finalDistance = Mathf.Clamp(hit.distance, _minDistance, _maxDistance);
        else
            _finalDistance = _maxDistance;

        _realCamera.localPosition = Vector3.Lerp(_realCamera.localPosition, _dirNormalized * _finalDistance, Time.deltaTime * _smoothness);
        
    }
}
