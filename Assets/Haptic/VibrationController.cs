using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class VibrationController : MonoBehaviour
{
    [Header("Joystick vibration")]
    [SerializeField] private float _deadZone = 0.2f;       // IMPORTANT : 0–1
    [SerializeField] private float _chargeSpeed = 0.5f;
    [SerializeField] private float _decaySpeed = 1f;

    [Header("Dice haptic vibration")]
    [SerializeField] private float _vibrationStrength = 0.8f;
    [SerializeField] private float _vibrationDuration = 0.15f;
    [SerializeField] private float _pauseBetweenVibrations = 0.15f;

    enum EState { Idle, Vibrating, Pausing }
    EState _state = EState.Idle;

    int _remainingVibrations;
    float _timer;

    float _chargeRight = 0f;
    float _chargeLeft = 0f;

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
            _chargeRight += _chargeSpeed * Time.deltaTime;
        }
        else
        {
            _chargeRight -= _decaySpeed * Time.deltaTime;
        }

        if (stickLeftActive)
        {
            _chargeLeft += _chargeSpeed * Time.deltaTime;
        }
        else
        {
            _chargeLeft -= _decaySpeed * Time.deltaTime;
        }

        _chargeRight = Mathf.Clamp01(_chargeRight);
        _chargeLeft = Mathf.Clamp01(_chargeLeft);
        
        float finalCharge = Mathf.Max(_chargeLeft, _chargeRight);
        Gamepad.current.SetMotorSpeeds(finalCharge, finalCharge);

        if (_chargeRight == 1)
        {
            _onConfirmRight?.Invoke();
        }

        if (_chargeLeft == 1)
        {
            _onConfirmLeft?.Invoke();
        }
    }

    public void RumbleXTimes(int times)
    {
        if (_state == EState.Idle)
        {
            _remainingVibrations = times;
            _state = EState.Vibrating;
            _timer = _vibrationDuration;
        }

        float diceVibration = 0f;

        switch (_state)
        {
            case EState.Vibrating:
                diceVibration = _vibrationStrength;

                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    _remainingVibrations--;

                    if (_remainingVibrations > 0)
                    {
                        _state = EState.Pausing;
                        _timer = _pauseBetweenVibrations;
                    }
                    else
                    {
                        _state = EState.Idle;
                    }
                }
                break;

            case EState.Pausing:
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    _state = EState.Vibrating;
                    _timer = _vibrationDuration;
                }
                break;
        }

        Gamepad.current.SetMotorSpeeds(diceVibration, diceVibration);
    }
}
