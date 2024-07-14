using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;
using static UnityEngine.Windows.WebCam.VideoCapture;

public class BackgroundMusic : NetworkBehaviour
{

    [SerializeField] AudioClip waitOST;
    [SerializeField] AudioClip phase1OST;
    [SerializeField] List<AudioClip> phase2OST;
    [SerializeField] AudioClip endroundOST;
    [SerializeField] AudioSource _audioSource = null;
    private GameState _audioState = GameState.EndMatch;
    private GameManager _gameManager = null;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<BackgroundMusic>().enabled = false;
            return;
        }
    }
    void Update()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        if (_gameManager == null)
            _gameManager = FindAnyObjectByType<GameManager>();
        else
        {
            if (_gameManager.game_state != _audioState)
            {
                _audioState = _gameManager.game_state;
                switch (_audioState)
                {
                    case GameState.WaitingOnClients:
                        //case GameState.WaitingFirstPhase:
                        if (_audioSource.isPlaying)
                        {
                            _audioSource.Stop();
                        }
                        _audioSource.loop=true;
                        _audioSource.clip = waitOST;
                        _audioSource.Play();
                        break;
                    case GameState.FirstPhase:
                        //case GameState.WaitingSecondPhase:
                        if (_audioSource.isPlaying)
                        {
                            _audioSource.Stop();
                        }
                        _audioSource.loop=true;
                        _audioSource.clip = phase1OST;
                        _audioSource.Play();
                        break;
                    case GameState.SecondPhase:
                        if (_audioSource.isPlaying)
                        {
                            _audioSource.Stop();
                        }
                        _audioSource.loop=true;
                        _audioSource.clip = phase2OST.ElementAt(_gameManager.roundNumber % phase2OST.Count);//una ost diversa per numero di round
                        _audioSource.Play();
                        break;
                    //case GameState.EndRound:
                    //    if (_audioSource.isPlaying)
                    //    {
                    //        _audioSource.Stop();
                    //        _audioSource.clip = endroundOST;
                    //        _audioSource.Play();
                    //    }
                    //    break;
                    default:
                        return;
                }
            }
        }
    }

}
