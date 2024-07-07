using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static PowerBehavior;

public class PowerEffect : NetworkBehaviour
{
    [SerializeField] public List<GameObject> _hitEffects = new List<GameObject>();
    [HideInInspector] public List<(GameObject, float)> _listSpawned = new List<(GameObject, float)>();
    private ShieldPower _shield;
    private CanvasGroup statusGroup = null;
    private Dictionary<StatusType, Coroutine> _dictStatus = new Dictionary<StatusType, Coroutine>();
    enum StatusType
    {
        Frost = 1,
        Confusion = 2,
        IronStomach = 3,//effetto passivo da cibo, no effetti negativi da cibo
        UnlimitedPower = 4,//effetto passivo da cibo, mana infinito a tempo
    }
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
        if (!_dictStatus.ContainsKey(StatusType.Confusion))
        {
            _dictStatus.Add(StatusType.Confusion, StartCoroutine(UpdateStatusHUD(StatusType.Confusion, duration)));
            SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
            gameObject.GetComponent<PlayerController>().confusePlayerMovement = true;
            yield return new WaitForSeconds(duration);
            gameObject.GetComponent<PlayerController>().confusePlayerMovement = false;
            _dictStatus.Remove(StatusType.Confusion);
        }
        yield return null;
    }
    private IEnumerator IronStomach()
    {
        float duration = 10f;
        StatusType status = StatusType.IronStomach;
        if (!_dictStatus.ContainsKey(status))
        {
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            gameObject.GetComponent<FoodPicker>().IronStomach = true;
            yield return new WaitForSeconds(duration);
            gameObject.GetComponent<FoodPicker>().IronStomach = false;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator UnlimitedPower()
    {
        float duration = 10f;
        StatusType status = StatusType.UnlimitedPower;
        if (!_dictStatus.ContainsKey(status))
        {
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            gameObject.GetComponent<ManaController>().UnlimitedPower = true;
            yield return new WaitForSeconds(duration);
            gameObject.GetComponent<ManaController>().UnlimitedPower = false;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator IceBulletHit(PowerBehavior.PowerType powerHit)
    {
        float duration = 3f, alteredSensitivity = .5f, alteredSpeed = 1;
        if (!_dictStatus.ContainsKey(StatusType.Frost))
        {
            _dictStatus.Add(StatusType.Frost, StartCoroutine(UpdateStatusHUD(StatusType.Frost, duration)));
            SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
            var playerC = gameObject.GetComponent<PlayerController>();
            playerC.lookSpeed = alteredSensitivity;
            playerC.runningSpeed = alteredSpeed;
            playerC.walkingSpeed = alteredSpeed;
            yield return new WaitForSeconds(duration);
            playerC.lookSpeed = playerC.lookSpeedBackup;
            playerC.runningSpeed = playerC.runningSpeedBackup;
            playerC.walkingSpeed = playerC.walkingSpeedBackup;
            _dictStatus.Remove(StatusType.Frost);
        }
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

    void InitStatusGroup()
    {
        foreach (var s in Enum.GetValues(typeof(StatusType)))
            statusGroup.transform.Find(s.ToString()).gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (statusGroup == null)
        {
            statusGroup = GameObject.FindGameObjectWithTag("StatusGroup").GetComponent<CanvasGroup>();
            InitStatusGroup();
        }
        CheckEffectsDuration();
        if (_shield == null)
            _shield = gameObject.GetComponent<ShieldPower>();
        if (Input.GetKey(KeyCode.LeftShift)) //DEBUUUUUG
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(IceBulletHit(PowerType.IceBullet));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(MindBulletHit(PowerType.MindBullet));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(IronStomach());
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                StartCoroutine(UnlimitedPower());
            }
        }
    }

    private IEnumerator UpdateStatusHUD(StatusType eStatus, float duration, bool spawnText = true)
    {
        var oStatus = statusGroup.transform.Find(eStatus.ToString());//group con Background, Slider, Icon e Description
        oStatus.gameObject.SetActive(true);


        var slider = oStatus.transform.Find("Slider").GetComponent<UnityEngine.UI.Slider>();
        StartCoroutine(CountdownSlider(slider, duration));
        //_dictStatus.Add(eStatus, coroutine);
        var oText = oStatus.transform.Find("Description");
        var text = oText.GetComponent<TextMeshProUGUI>();
        if (spawnText)
            StartCoroutine(TextFadeOut(text, 1f));
        yield return new WaitForSeconds(duration);
        _dictStatus.Remove(eStatus);
        oStatus.gameObject.SetActive(false);//Se nessun thread sta girando per questo status, lo posso spegnere
        yield return null;
    }
    private IEnumerator CountdownSlider(UnityEngine.UI.Slider slider, float duration)
    {
        float startValue = slider.maxValue;
        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, 0, timeElapsed / duration);
            yield return null;  // Wait for the next frame
        }

        // Ensure the slider value is set to 0 at the end
        slider.value = 0;
        yield return null;  // Wait for the next frame
    }
    IEnumerator TextFadeOut(TextMeshProUGUI text, float duration)
    {
        float startAlpha = 1f;
        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, timeElapsed / duration);
            Color newColor = new Color(text.color.r, text.color.g, text.color.b, alpha);
            text.color = newColor;
            yield return null;  // Wait for the nnext frame
        }

        // Ensure the alpha is set to 0 at the end
        Color finalColor = new Color(text.color.r, text.color.g, text.color.b, 0);
        text.color = finalColor;
        yield return null;  // Wait for the nnext frame
    }
    private IEnumerator ImageFadeOut(UnityEngine.UI.Image image, float duration)
    {
        float startAlpha = 1f;
        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, timeElapsed / duration);
            Color newColor = new Color(image.color.r, image.color.g, image.color.b, alpha);
            image.color = newColor;
            yield return null;  // Wait for the next frame
        }

        // Ensure the alpha is set to 0 at the end
        Color finalColor = new Color(image.color.r, image.color.g, image.color.b, 0);
        image.color = finalColor;
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
