using System;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Hearing range")]
    [SerializeField] private float _range = 3f;

    [Header("Raycasting")]
    private RaycastHit[] _audioHitsFront = null;
    private RaycastHit _moveHitsFront;
    private bool _moveBoolFront;
    private List<RaycastHit> _playingHits = new List<RaycastHit>();

    [Header("Character Stats")]
    [SerializeField] private int _hp = 5;
    [SerializeField] private int _armorClass = 5;
    [SerializeField] private int _strength = 1;
    [SerializeField] private int _gold = 0;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private Armor _armor;
    #endregion Attributes

    #region Properties
    public static CharaController Instance { get => _instance; set => _instance = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public int ArmorClass { get => _armorClass; set => _armorClass = value; }
    public int Strength { get => _strength; set => _strength = value; }
    public int Gold { get => _gold; set => _gold = value; }
    public Weapon Weapon { get => _weapon; set => _weapon = value; }
    public Armor Armor { get => _armor; set => _armor = value; }
    #endregion Properties

    void Start()
    {
        
    }

    void Update()
    {
        ActivateAudioOnHearableNodes();

        AudioHits();
        MoveHits();

        Rotate();
        Move();

        DeactivateAudioOnHearableNodes();
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
        if (Input.GetKeyDown(_keyCodeLeft))
        {
            transform.Rotate(0, -90, 0);
        }
        if (Input.GetKeyDown(_keyCodeRight))
        {
            transform.Rotate(0, 90, 0);
        }
    }

    private void Move()
    {
        if (Input.GetKeyDown(_keyCodeForward))
        {
            if ((_moveBoolFront) && (_moveHitsFront.transform.CompareTag("Node")))
            {
                transform.position = _moveHitsFront.transform.position;
            }
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
