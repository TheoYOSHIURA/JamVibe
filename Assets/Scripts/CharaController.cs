using UnityEngine;
using System.Linq;

public class CharaController : MonoBehaviour
{
    [SerializeField] private KeyCode _keyCodeForward = KeyCode.Z;
    [SerializeField] private KeyCode _keyCodeLeft = KeyCode.Q;
    [SerializeField] private KeyCode _keyCodeRight = KeyCode.D;

    [SerializeField] private float _range = 3f;

    RaycastHit[] _audioHitsFront = null;
    RaycastHit[] _audioHitsLeft = null;
    RaycastHit[] _audioHitsRight = null;

    RaycastHit _moveHitsFront;
    RaycastHit _moveHitsLeft;
    RaycastHit _moveHitsRight;

    bool _moveBoolFront;
    bool _moveBoolLeft;
    bool _moveBoolRight;

    void Start()
    {
        
    }

    void Update()
    {
        DeactivateAudioOnHearableNodes();
        AudioHits();
        MoveHits();
        ActivateAudioOnHearableNodes();

        Rotate();
        Move();
    }

    private void AudioHits()
    {
        _audioHitsFront = Physics.RaycastAll(transform.position, transform.forward, _range);
        Debug.DrawRay(transform.position, transform.forward * _range);

        _audioHitsLeft = Physics.RaycastAll(transform.position, -transform.right, _range);
        Debug.DrawRay(transform.position, -transform.right * _range);

        _audioHitsRight = Physics.RaycastAll(transform.position, transform.right, _range);
        Debug.DrawRay(transform.position, transform.right * _range);
    }

    private void MoveHits()
    {
        _moveBoolFront = Physics.Raycast(transform.position, transform.forward, out _moveHitsFront, 1);
        Debug.DrawRay(transform.position, transform.forward * _range, Color.green);

        _moveBoolLeft = Physics.Raycast(transform.position, -transform.right, out _moveHitsLeft, 1);
        Debug.DrawRay(transform.position, -transform.right * _range, Color.red);

        _moveBoolRight = Physics.Raycast(transform.position, transform.right, out _moveHitsRight, 1);
        Debug.DrawRay(transform.position, transform.right * _range, Color.blue);
    }

    private void Rotate()
    {
        if (Input.GetKeyDown(_keyCodeLeft)) transform.Rotate(0, -90 , 0);
        if (Input.GetKeyDown(_keyCodeRight)) transform.Rotate(0, 90, 0);
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
            _audioHitsFront = _audioHitsFront.OrderBy((d) => (d.distance)).ToArray();
            foreach (RaycastHit hit in _audioHitsFront)
            {
                if (hit.transform.CompareTag("Node"))
                {
                    AudioSource audio = hit.transform.GetComponent<AudioSource>();
                    audio.Play();
                    //Debug
                    Debug.Log("Playing Sound");
                }
                else
                {
                    break;
                }
            }
        }
        
        if (_audioHitsLeft != null)
        {
            _audioHitsFront = _audioHitsLeft.OrderBy((d) => (d.distance)).ToArray();
            foreach (RaycastHit hit in _audioHitsLeft)
            {
                if (hit.transform.CompareTag("Node"))
                {
                    AudioSource audio = hit.transform.GetComponent<AudioSource>();
                    audio.Play();
                }
                else
                {
                    break;
                }
            }
        }
        
        if (_audioHitsRight != null)
        {
            _audioHitsFront = _audioHitsRight.OrderBy((d) => (d.distance)).ToArray();
            foreach (RaycastHit hit in _audioHitsRight)
            {
                if (hit.transform.CompareTag("Node"))
                {
                    AudioSource audio = hit.transform.GetComponent<AudioSource>();
                    audio.Play();
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void DeactivateAudioOnHearableNodes()
    {
        if (_audioHitsFront != null)
        {
            foreach (RaycastHit hit in _audioHitsFront)
            {
                AudioSource audio = hit.transform.GetComponent<AudioSource>();
                audio.Stop();
            }
        }

        if (_audioHitsLeft != null)
        {
            foreach (RaycastHit hit in _audioHitsLeft)
            {
                AudioSource audio = hit.transform.GetComponent<AudioSource>();
                audio.Stop();
            }
        }

        if (_audioHitsRight != null)
        {
            foreach (RaycastHit hit in _audioHitsRight)
            {
                AudioSource audio = hit.transform.GetComponent<AudioSource>();
                audio.Stop();
            }
        }

    }

}
