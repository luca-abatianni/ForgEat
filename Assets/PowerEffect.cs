using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PowerBehavior;

public class PowerEffect : NetworkBehaviour
{
    [SerializeField] public List<GameObject> _hitEffects = new List<GameObject>();
    [HideInInspector] public List<(GameObject, float)> _listSpawned = new List<(GameObject, float)>();
    private ShieldPower _shield;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<PowerEffect>().enabled = false;
            return;
        }
    }
    //Controlla che il personaggio non sia già sotto effetto di questo potere
    public bool CheckHitList(PowerBehavior.PowerType powerType)
    {
        foreach (var spawned in _listSpawned)
        {
            var pb = spawned.Item1;
            if (pb != null && pb.GetComponent<PowerBehavior>()._powerType == powerType)
                return true;
        }
        return false;
    }

    [TargetRpc]
    public void Hit(NetworkConnection owner, PowerBehavior.PowerType powerType)
    {
        if (_shield != null && _shield._isShielded)//Client spawna comunque l'effetto di Hit
            return;
        if (powerType == PowerBehavior.PowerType.IceBullet)
        {
            StartCoroutine(IceBulletHit(powerType));
        }
        if (powerType == PowerBehavior.PowerType.MindBullet)
        {
            StartCoroutine(MindBulletHit(powerType));
        }
        if (powerType == PowerBehavior.PowerType.WindBullet)
        {
            Debug.Log("Hit WIND" + powerType);
            StartCoroutine(WindBulletHit(powerType));
        }
    }
    private IEnumerator WindBulletHit(PowerBehavior.PowerType powerHit)
    {//Effetto gestito da WindiImpact + PlayerController
        float duration = 1f;
        SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
        yield return null;
    }
    private IEnumerator MindBulletHit(PowerBehavior.PowerType powerHit)
    {
        float duration = 4f;
        SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
        gameObject.GetComponent<PlayerController>().confusePlayerMovement = true;
        yield return new WaitForSeconds(duration);
        gameObject.GetComponent<PlayerController>().confusePlayerMovement = false;
        yield return null;
    }
    private IEnumerator IceBulletHit(PowerBehavior.PowerType powerHit)
    {
        float duration = 3f, alteredSensitivity = .5f, alteredSpeed = 1;
        SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
        var playerC = gameObject.GetComponent<PlayerController>();
        var originalWalking = playerC.walkingSpeed;
        var originalRunning = playerC.runningSpeed;
        var originalSensitivity = playerC.lookSpeed;
        playerC.lookSpeed = alteredSensitivity;
        playerC.runningSpeed = alteredSpeed;
        playerC.walkingSpeed = alteredSpeed;
        yield return new WaitForSeconds(duration);
        playerC.lookSpeed = originalSensitivity;
        playerC.runningSpeed = originalRunning;
        playerC.walkingSpeed = originalWalking;
        yield return null;
    }

    [ServerRpc]
    void SRPC_DespawnHitEffect(GameObject _effectToSpawn)
    {
        ServerManager.Despawn(_effectToSpawn);
    }
    [ServerRpc]
    void SRPC_SpawnHitEffect(PowerEffect script, GameObject _effectToSpawn, GameObject _spawnPoint, float duration)
    {
        if (_effectToSpawn != null)
        {

            var spawned = Instantiate(_effectToSpawn, _spawnPoint.transform.position, _spawnPoint.transform.rotation);

            ServerManager.Spawn(spawned);
            spawned.transform.SetParent(_spawnPoint.transform);
            ORPC_SetSpawnedEffect(script, _spawnPoint, spawned, duration);
        }
        else
            Debug.LogError("HitEffect is Null");
    }
    [ObserversRpc]
    void ORPC_SetSpawnedEffect(PowerEffect script, GameObject _spawnPoint, GameObject spawned, float duration)
    {
        spawned.transform.SetParent(_spawnPoint.transform);
        script._listSpawned.Add((spawned, duration));
    }
    // Update is called once per frame
    void Update()
    {
        CheckEffectsDuration();
        if (_shield == null)
            _shield = gameObject.GetComponent<ShieldPower>();

    }
    //List non è modificabile, per cui
    //ad ogni ciclo controllo le durate degli effetti e aggiorno sottraendo il tempo dall'ultimo frame
    //se il tempo è finito despawno l'oggetto,
    //altrimenti lo rimetto in lista (con sostituzione dell'intera lista per il motivo di cui sopra)
    void CheckEffectsDuration()
    {
        List<(GameObject, float)> spawned = new List<(GameObject, float)>();
        foreach (var el in _listSpawned)
        {
            float duration = el.Item2 - Time.deltaTime;
            GameObject effect = el.Item1;
            if (duration < 0)
            {
                SRPC_DespawnHitEffect(effect);
            }
            else
            {
                spawned.Add((effect, duration));
            }

        }
        _listSpawned.Clear();
        _listSpawned = spawned;
    }
}
