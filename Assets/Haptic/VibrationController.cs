using UnityEngine;
using UnityEngine.InputSystem;
using XInputDotNetPure;

public class VibrationController : MonoBehaviour
{
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    [SerializeField] private float _deadZone = 0.2f;
    [SerializeField] private float _chargeSpeed = 0.5f;
    [SerializeField] private float _decaySpeed = 1f;

    [SerializeField] private float _vibrationStrength = 0.8f;
    [SerializeField] private float _vibrationDuration = 0.15f;
    [SerializeField] private float _pauseBetweenVibrations = 0.15f;

    enum EState { Idle, Vibrating, Pausing }
    EState _state = EState.Idle;

    int _remainingVibrations;
    float _timer;

    float _charge = 0f;

    // Update is called once per frame
    void Update()
    {
        // Joystick
        if (Gamepad.current == null) return;

        Vector2 stick = Gamepad.current.leftStick.ReadValue();
        bool stickActive = stick.magnitude > _deadZone;

        if (stickActive)
        {
            _charge += _chargeSpeed * Time.deltaTime;
        }
        else
        {
            _charge -= _decaySpeed * Time.deltaTime;
        }

        _charge = Mathf.Clamp01(_charge);

        Gamepad.current.SetMotorSpeeds(_charge, _charge);

        // Lancer le dé
        if (_state == EState.Idle &&
            Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            _remainingVibrations = Random.Range(1, 7);
            Debug.Log("🎲 Dé : " + _remainingVibrations);

            _state = EState.Vibrating;
            _timer = _vibrationDuration;
        }

        switch (_state)
        {
            case EState.Vibrating:
                // 🔥 VIBRATION CHAQUE FRAME
                Gamepad.current.SetMotorSpeeds(_vibrationStrength, _vibrationStrength);

                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    Gamepad.current.SetMotorSpeeds(0f, 0f);
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
                Gamepad.current.SetMotorSpeeds(0f, 0f);

                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    _state = EState.Vibrating;
                    _timer = _vibrationDuration;
                }
                break;

            case EState.Idle:
                Gamepad.current.SetMotorSpeeds(0f, 0f);
                break;
        }
    }
    void OnDisable()
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }
}
