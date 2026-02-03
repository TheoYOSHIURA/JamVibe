using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharaController : MonoBehaviour
{
    [Header("Key bindings")]
    [SerializeField] private KeyCode _keyCodeForward = KeyCode.Z;
    [SerializeField] private KeyCode _keyCodeLeft = KeyCode.Q;
    [SerializeField] private KeyCode _keyCodeRight = KeyCode.D;

    [Header("Hearing range")]
    [SerializeField] private float _range = 3f;

    // 
    private RaycastHit[] _audioHitsFront = null;
    private RaycastHit _moveHitsFront;
    private bool _moveBoolFront;
    private List<RaycastHit> _playingHits = new List<RaycastHit>();

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
