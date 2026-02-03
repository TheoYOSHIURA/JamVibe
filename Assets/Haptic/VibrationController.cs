using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class VibrationController : MonoBehaviour
{
    [Header("Instance")]
    [SerializeField] private static VibrationController _instance;

    [Header("Joystick vibration")]
    [SerializeField] private float _deadZone = 0.2f;       // IMPORTANT : 0–1
    [SerializeField] private float _chargeSpeed = 0.5f;
    [SerializeField] private float _decaySpeed = 1f;

    [Header("Dice haptic vibration")]
    [SerializeField] private float _vibrationStrength = 0.8f;
    [SerializeField] private float _vibrationDuration = 0.15f;
    [SerializeField] private float _pauseBetweenVibrations = 0.15f;

    float _chargeRight = 0f;
    float _chargeLeft = 0f;

    public static VibrationController Instance { get { return _instance; } }

    #region Events
    private event Action _onConfirmRight;
    public event Action OnConfirmRight
    {
        add
        {
            _onConfirmRight -= value;
            _onConfirmRight += value;
        }
        remove
        {
            _onConfirmRight -= value;
        }
    }

    private event Action _onConfirmLeft;
    public event Action OnConfirmLeft
    {
        add
        {
            _onConfirmLeft -= value;
            _onConfirmLeft += value;
        }
        remove
        {
            _onConfirmLeft -= value;
        }
    }

    #endregion Events
    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        
    }

    void Update()
    {
        Confirm();
    }

    void OnDisable()
    {
        if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    public void Confirm()
    {
        if (Gamepad.current == null) return;

        Vector2 stick = Gamepad.current.leftStick.ReadValue();
        bool stickRightActive = stick.x > _deadZone;
        bool stickLeftActive = stick.x < - _deadZone;

        if (stickRightActive)
        {
            if (_onConfirmRight != null || _onConfirmLeft != null) _chargeRight += _chargeSpeed * Time.deltaTime;
        }
        else
        {
            _chargeRight -= _decaySpeed * Time.deltaTime;
        }

        if (stickLeftActive)
        {
            if (_onConfirmRight != null || _onConfirmLeft != null) _chargeLeft += _chargeSpeed * Time.deltaTime;
        }
        else
        {
            _chargeLeft -= _decaySpeed * Time.deltaTime;
        }

        _chargeRight = Mathf.Clamp01(_chargeRight);
        _chargeLeft = Mathf.Clamp01(_chargeLeft);
        
       Gamepad.current.SetMotorSpeeds(_chargeLeft, _chargeRight);

        if (_chargeRight == 1)
        {
            _chargeRight = 0f;
            _onConfirmRight?.Invoke();
        }

        if (_chargeLeft == 1)
        {
            _chargeLeft = 0f;
            _onConfirmLeft?.Invoke();
        }
    }

    public void RumbleXTime(int times)
    {
        StartCoroutine(Rumble(times));
    }

    public IEnumerator Rumble(int times)
    {
        if (Gamepad.current == null)
            yield break;

        for (int i = 0; i < times; i++)
        {
            Gamepad.current.SetMotorSpeeds(_vibrationStrength, 0);
            yield return new WaitForSeconds(_vibrationDuration);

            Gamepad.current.SetMotorSpeeds(0f, 0f);
            yield return new WaitForSeconds(_pauseBetweenVibrations);
        }
    }
}
