using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CharaController : MonoBehaviour
{
    #region Attributes
    [Header("Instance")]
    private static CharaController _instance;

    [Header("Key bindings")]
    [SerializeField] private KeyCode _keyCodeForward = KeyCode.Z;
    [SerializeField] private KeyCode _keyCodeLeft = KeyCode.Q;
    [SerializeField] private KeyCode _keyCodeRight = KeyCode.D;
    private float _stickLeftForward = Input.GetAxis("Vertical");
    private float _stickLeft = Input.GetAxis("Horizontal");
    private float _stickRightForward = Input.GetAxis("Mouse Y");
    private float _stickRight = Input.GetAxis("Mouse X");
    private bool _leftInput = false;
    private bool _rightInput = false;
    private bool _forwardInput = false;
    [Header("Hearing range")]
    [SerializeField] private float _range = 3f;

    [Header("Raycasting")]
    private RaycastHit[] _audioHitsFront = null;
    private RaycastHit _moveHitsFront;
    private bool _moveBoolFront;
    private RaycastHit _moveHitsBack;
    private bool _moveBoolBack;
    private List<RaycastHit> _playingHits = new List<RaycastHit>();
    [Header("Character Stats")]
    [SerializeField] private int _hp = 5;
    [SerializeField] private int _armorClass = 0;
    [SerializeField] private int _strength = 0;
    [SerializeField] private int _gold = 0;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private Armor _armor;

    private bool _cantMove = false;
    
   

    #endregion Attributes

    #region Properties
    public static CharaController Instance { get => _instance; set => _instance = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public int ArmorClass { get => _armorClass; set => _armorClass = value; }
    public int Strength { get => _strength; set => _strength = value; }
    public int Gold { get => _gold; set => _gold = value; }
    public Weapon Weapon { get => _weapon; set => _weapon = value; }
    public Armor Armor { get => _armor; set => _armor = value; }
    public bool CantMove { get => _cantMove; set => _cantMove = value; }
    #endregion Properties

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
        ActivateAudioOnHearableNodes();
        DeactivateAudioOnHearableNodes();
        AudioHits();
        MoveHits();
        if (CantMove != true)
        {
        Rotate();
        Move();
        }

        

        _stickLeftForward = Input.GetAxis("Vertical");
        _stickLeft = Input.GetAxis("Horizontal");
        _stickRightForward = Input.GetAxis("Mouse Y");
        _stickRight = Input.GetAxis("Mouse X");

        if (Input.GetKeyDown(_keyCodeLeft) || _stickRight < -0.7f || _stickLeft < -0.7f)
        {
            _leftInput = true;
        }
        else
        {
            _leftInput = false;
        }
         if (Input.GetKeyDown(_keyCodeRight) || _stickRight > 0.7f || _stickLeft > 0.7f)
        {
            _rightInput = true;
        }
        else
        {
            _rightInput = false;
        }
        if (Input.GetKeyDown(_keyCodeForward) || _stickLeftForward > 0.7f || _stickRightForward > 0.7f)
        {
            _forwardInput = true;
        }
        else
        {
            _forwardInput = false;
        }
        
    }

    private void AudioHits()
    {
        _audioHitsFront = Physics.RaycastAll(transform.position, transform.forward, _range);
        Debug.DrawRay(transform.position, transform.forward * _range);
    }

    private void MoveHits()
    {
        _moveBoolFront = Physics.Raycast(transform.position, transform.forward, out _moveHitsFront, 1);
        Debug.DrawRay(transform.position, transform.forward * _range, Color.green);
    }

    private void Rotate()
    {
        if (_leftInput)
        {
            transform.Rotate(0, -90, 0);
        }
        if (_rightInput)
        {
            transform.Rotate(0, 90, 0);
        }
    }

    public void ForceRotate()
    {
        transform.Rotate(0, -90, 0);
    }

    private void Move()
    {
        if (_forwardInput)
        {
            if ((_moveBoolFront) && (_moveHitsFront.transform.CompareTag("Node")))
            {
                transform.position = _moveHitsFront.transform.position;
            }
        }
    }

    public void MoveBackwards()
    {
        _moveBoolBack = Physics.Raycast(transform.position, transform.forward, out _moveHitsBack, 1);
        Debug.DrawRay(transform.position, -transform.forward * _range, Color.green);
        transform.position = _moveHitsBack.transform.position;
    }

    public void EquipItem(Weapon weapon = null, Armor armor = null)
    {
        if (weapon != null)
        {
            if (_weapon != null) _strength -= weapon.Strength;
            _weapon = weapon;
            _strength = _strength * _strength;
        }

        if (armor != null)
        {
            if (_armor != null) _armorClass -= _armor.ArmorClass;
            _armor = armor;
            _armorClass += _armor.ArmorClass;
        }
    }

    private void ActivateAudioOnHearableNodes()
    {
        if (_audioHitsFront != null)
        {
            Array.Sort(_audioHitsFront, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in _audioHitsFront)
            {
                if (!_playingHits.Contains(hit))
                {
                    if (hit.transform.CompareTag("Wall")) break;
                    _playingHits.Add(hit);
                }
            }
            foreach (RaycastHit hit in _playingHits)
            {
                AudioSource audio = hit.transform.GetComponent<AudioSource>();
                if ((audio != null) && (!audio.isPlaying))
                {
                    audio.Play();
                }
            }
        }
    }

    private void DeactivateAudioOnHearableNodes()
    {
        if (_playingHits != null)
        {
            List<RaycastHit> toRemove = new List<RaycastHit>();
            foreach (RaycastHit hit in _playingHits)
            {
                if (!_audioHitsFront.Contains(hit))
                {
                    toRemove.Add(hit);
                }
            }

            foreach (RaycastHit hit in toRemove)
            {
                if (_playingHits.Contains(hit))
                {
                    AudioSource audio = hit.transform.GetComponent<AudioSource>();
                    if (audio != null) audio.Stop();
                    _playingHits.Remove(hit);
                }
            }
        }
    }
}
