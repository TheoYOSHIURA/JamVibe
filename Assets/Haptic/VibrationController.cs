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

    float _charge = 0f;

    void Update()
    {
        if (Gamepad.current == null) return;

        /* =========================
         * JOYSTICK (ANALOGIQUE)
         * ========================= */
        Vector2 stick = Gamepad.current.leftStick.ReadValue();
        bool stickActive = stick.magnitude > _deadZone;

        if (stickActive)
            _charge += _chargeSpeed * Time.deltaTime;
        else
            _charge -= _decaySpeed * Time.deltaTime;

        _charge = Mathf.Clamp01(_charge);
        float joystickVibration = _charge;

        /* =========================
         * LANCER LE DÉ HAPTIQUE
         * ========================= */
        if (_state == EState.Idle &&
            Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            _remainingVibrations = Random.Range(1, 7);
            _state = EState.Vibrating;
            _timer = _vibrationDuration;

            Debug.Log("🎲 Dé haptique : " + _remainingVibrations);
        }

        /* =========================
         * DÉ HAPTIQUE (STATE MACHINE)
         * ========================= */
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

        /* =========================
         * MIX FINAL DES VIBRATIONS
         * ========================= */
        float finalVibration = Mathf.Max(joystickVibration, diceVibration);
        Gamepad.current.SetMotorSpeeds(finalVibration, finalVibration);
    }

    void OnDisable()
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }
}
