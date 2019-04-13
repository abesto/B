using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeFlyCamera : MonoBehaviour
{
    [Space]

    [SerializeField]
    private bool _active = true;

    [Space]

    [SerializeField]
    private bool _enableRotation = true;

    [SerializeField]
    private float _mouseSense = 0.8f;

    [Space]

    [SerializeField]
    private bool _enableTranslation = true;

    [SerializeField]
    private float _translationSpeed = 55f;

    [Space]

    [SerializeField]
    private bool _enableMovement = true;

    [SerializeField]
    private float _movementSpeed = 10f;

    [SerializeField]
    private float _boostedSpeed = 50f;

    [Space]

    [SerializeField]
    private bool _enableSpeedAcceleration = true;

    [SerializeField]
    private float _speedAccelerationFactor = 1.5f;

    [Space]

    [SerializeField]
    private KeyCode _initPositonButton = KeyCode.R;
    
    private CursorLockMode _wantedMode;
    private float _currentIncrease;

    private Vector3 _initPosition;
    private Vector3 _initRotation;


    private void Start()
    {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;
    }

    private void OnEnable()
    {
        if (_active)
            _wantedMode = CursorLockMode.Locked;
    }

    // Apply requested cursor state
    private void SetCursorState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = _wantedMode = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _wantedMode = CursorLockMode.Locked;
        }

        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }

    private void Update()
    {
        if (!_active)
            return;

        SetCursorState();

        if (Cursor.visible)
            return;

        // Rotation
        if (_enableRotation)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x -= Input.GetAxis("Mouse Y") * _mouseSense;
            eulerAngles.y += Input.GetAxis("Mouse X") * _mouseSense;
            transform.eulerAngles = eulerAngles;
        }

        // Translation
        if (_enableTranslation)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
        }

        // Movement
        if (_enableMovement)
        {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = _movementSpeed;

            if (Input.GetKey(KeyCode.W))
                deltaPosition += transform.forward;

            if (Input.GetKey(KeyCode.S))
                deltaPosition -= transform.forward;

            if (Input.GetKey(KeyCode.A))
                deltaPosition -= transform.right;

            if (Input.GetKey(KeyCode.D))
                deltaPosition += transform.right;

            if (Input.GetKey(KeyCode.Space))
                deltaPosition += transform.up;

            if (Input.GetKey(KeyCode.LeftControl))
                deltaPosition -= transform.up;

            if (Input.GetKey(KeyCode.LeftShift))
                currentSpeed = _boostedSpeed;

            // Calc acceleration
            if (_enableSpeedAcceleration)
            {
                if (deltaPosition != Vector3.zero)
                    _currentIncrease += _currentIncrease * _speedAccelerationFactor * Time.deltaTime;
                else
                    _currentIncrease = _speedAccelerationFactor;
            }

            transform.position += deltaPosition * (currentSpeed + _currentIncrease) * Time.deltaTime;
        }

        // Return to init position
        if (Input.GetKeyDown(_initPositonButton))
        {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
        }
    }
}
