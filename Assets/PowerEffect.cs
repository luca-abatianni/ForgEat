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
    public void Hit(PowerBehavior.PowerType powerType)
    {
        if (powerType == PowerBehavior.PowerType.IceBullet)
        {
            Debug.Log("Hit ICE" + powerType);
            StartCoroutine(IceBulletHit(powerType));
        }
        if (powerType == PowerBehavior.PowerType.MindBullet)
        {
            Debug.Log("Hit MIND" + powerType);
            StartCoroutine(MindBulletHit(powerType));
        }
    }
    private IEnumerator MindBulletHit(PowerBehavior.PowerType powerHit)
    {
        float duration = 10f;
        SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
        gameObject.GetComponent<FirstPersonController>().confusePlayerMovement = true;
        yield return new WaitForSeconds(duration);
        gameObject.GetComponent<FirstPersonController>().confusePlayerMovement = false;
        yield return null;
    }
    private IEnumerator IceBulletHit(PowerBehavior.PowerType powerHit)
    {
        float duration = 3f, alteredSensitivity = .5f;
        SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
        gameObject.GetComponent<FirstPersonController>().SetPlayerCanMove(false);
        var originalSensitivity = gameObject.GetComponent<FirstPersonController>().mouseSensitivity;
        gameObject.GetComponent<FirstPersonController>().mouseSensitivity = alteredSensitivity;
        yield return new WaitForSeconds(duration);
        gameObject.GetComponent<FirstPersonController>().mouseSensitivity = originalSensitivity;
        gameObject.GetComponent<FirstPersonController>().SetPlayerCanMove(true);
        yield return null;
    }

    [ServerRpc]
    void SRPC_DespawnHitEffect(GameObject _effectToSpawn)
    {
        ServerManager.Despawn(_effectToSpawn);
    }
    [ServerRpc(RequireOwnership = false)]
    void SRPC_SpawnHitEffect(PowerEffect script, GameObject _effectToSpawn, GameObject _spawnPoint, float duration)
    {
        if (_effectToSpawn != null)
        {

            var spawned = Instantiate(_effectToSpawn, _spawnPoint.transform.position, _spawnPoint.transform.rotation);

            ServerManager.Spawn(spawned);
            ORPC_SetSpawnedEffect(script, spawned, duration);
            spawned.transform.SetParent(_spawnPoint.transform);
        }
        else
            Debug.LogError("HitEffect is Null");
    }
    [ObserversRpc]
    void ORPC_SetSpawnedEffect(PowerEffect script, GameObject spawned, float duration)
    {
        script._listSpawned.Add((spawned, duration));
    }
    // Update is called once per frame
    void Update()
    {
        CheckEffectsDuration();
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
