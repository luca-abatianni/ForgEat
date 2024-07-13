using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    [SerializeField] public List<AudioClip> statusClips;
    [HideInInspector] public List<(GameObject, float)> _listSpawned = new List<(GameObject, float)>();
    private ShieldPower _shield;
    private CanvasGroup statusGroup = null;
    private CanvasGroup hitFeedbackGroup = null;
    private Dictionary<StatusType, Coroutine> _dictStatus = new Dictionary<StatusType, Coroutine>();

    enum StatusType
    {
        Frost = 1,
        Confusion = 2,
        IronStomach = 3,//effetto passivo da cibo, no effetti negativi da cibo
        UnlimitedPower = 4,//effetto passivo da cibo, mana infinito a tempo
        Agility = 5,//effetto passivo da cibo, velocit� e salto ++
        Poisoning = 6,//effetto passivo da cibo, mana positivo diventa negativo
        Silence = 7,//effetto passivo da cibo, impossibile castare spells
        Heft = 8,//effetto passivo da cibo, slow down + gravity up
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

    //Controlla che il personaggio non sia gi� sotto effetto di questo potere
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

    private IEnumerator IceBulletHit(PowerBehavior.PowerType powerHit)
    {
        float duration = 3f;
        var status = StatusType.Frost;
        if (!_dictStatus.ContainsKey(status))
        {
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            StatusSound(status);
            var iceVisual = hitFeedbackGroup.transform.Find(status.ToString()).GetComponent<UnityEngine.UI.Image>();
            StartCoroutine(ImageFadeIn(iceVisual, 0f));
            StartCoroutine(HurtFlash());
            SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
            var playerC = gameObject.GetComponent<PlayerController>();
            playerC.frost = true;
            StartCoroutine(ImageFadeOut(iceVisual, duration));

            yield return new WaitForSeconds(duration);
            playerC.frost = false;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator WindBulletHit(PowerBehavior.PowerType powerHit)
    {//Effetto gestito da WindiImpact + PlayerController
        float duration = 1f;
        StartCoroutine(HurtFlash());
        SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
        yield return null;
    }
    private IEnumerator MindBulletHit(PowerBehavior.PowerType powerHit)
    {
        float duration = 4f;
        var status = StatusType.Confusion;
        if (!_dictStatus.ContainsKey(status))
        {
            StatusSound(status);
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            var mindVisual = hitFeedbackGroup.transform.Find(status.ToString()).GetComponent<UnityEngine.UI.Image>();
            Color color = new Color(mindVisual.color.r, mindVisual.color.g, mindVisual.color.b, .25f);
            mindVisual.color = color;
            StartCoroutine(HurtFlash());
            SRPC_SpawnHitEffect(this, _hitEffects[(int)powerHit], gameObject, duration);
            gameObject.GetComponent<PlayerController>().confusion = true;
            yield return new WaitForSeconds(duration);
            gameObject.GetComponent<PlayerController>().confusion = false;
            color = new Color(mindVisual.color.r, mindVisual.color.g, mindVisual.color.b, 0f);
            mindVisual.color = color;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator IronStomach()
    {
        float duration = 10f;
        StatusType status = StatusType.IronStomach;
        if (!_dictStatus.ContainsKey(status))
        {
            StatusSound(status);
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
            StatusSound(status);
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            var upVisual = hitFeedbackGroup.transform.Find(status.ToString()).GetComponent<UnityEngine.UI.RawImage>();
            upVisual.color = new Color(upVisual.color.r, upVisual.color.g, upVisual.color.b, .8f);
            gameObject.GetComponent<ManaController>().UnlimitedPower = true;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                upVisual.uvRect = new Rect(upVisual.uvRect.position + new Vector2(0f, -.8f) * Time.deltaTime, upVisual.uvRect.size);
                yield return null;
            }
            //yield return new WaitForSeconds(duration);
            upVisual.color = new Color(upVisual.color.r, upVisual.color.g, upVisual.color.b, 0f);
            gameObject.GetComponent<ManaController>().UnlimitedPower = false;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator Agility()
    {
        float duration = 10f;
        StatusType status = StatusType.Agility;
        if (!_dictStatus.ContainsKey(status))
        {
            StatusSound(status);
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            var playerC = gameObject.GetComponent<PlayerController>();
            playerC.Agility = true;
            yield return new WaitForSeconds(duration);
            playerC.Agility = false;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator Poisoning()
    {
        float duration = 5f;
        StatusType status = StatusType.Poisoning;
        if (!_dictStatus.ContainsKey(status))
        {
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            StatusSound(status);
            StartCoroutine(HurtFlash());
            gameObject.GetComponent<ManaController>().Poisoning = true;
            gameObject.GetComponent<ManaController>().UpdateMana(15f);

            var poisonVisual = hitFeedbackGroup.transform.Find(status.ToString()).GetComponent<UnityEngine.UI.Image>();
            for (int i = 0; i < 5; i++)
            {//Periodico blinking del veleno (gradient verde)
                StartCoroutine(ImageFadeIn(poisonVisual, .3f));
                yield return new WaitForSeconds(.3f);
                StartCoroutine(ImageFadeOut(poisonVisual, .3f));

                yield return new WaitForSeconds(.7f);
            }

            gameObject.GetComponent<ManaController>().Poisoning = false;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator Silence()
    {
        float duration = 7f;
        StatusType status = StatusType.Silence;
        if (!_dictStatus.ContainsKey(status))
        {
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            StatusSound(status);
            //TODO : OVATTARE I SUONI
            var silenceVisual = hitFeedbackGroup.transform.Find(status.ToString()).GetComponent<UnityEngine.UI.Image>();
            Color color = new Color(silenceVisual.color.r, silenceVisual.color.g, silenceVisual.color.b, .20f);
            silenceVisual.color = color;
            StartCoroutine(HurtFlash());
            gameObject.GetComponent<PrimaryPower>().Silence = true;
            yield return new WaitForSeconds(duration);
            gameObject.GetComponent<PrimaryPower>().Silence = false;
            color = new Color(silenceVisual.color.r, silenceVisual.color.g, silenceVisual.color.b, 0f);
            silenceVisual.color = color;
            _dictStatus.Remove(status);
        }
        yield return null;
    }
    private IEnumerator Heft()
    {
        float duration = 10f;
        StatusType status = StatusType.Heft;
        if (!_dictStatus.ContainsKey(status))
        {
            StatusSound(status);
            StartCoroutine(HurtFlash());
            _dictStatus.Add(status, StartCoroutine(UpdateStatusHUD(status, duration)));
            gameObject.GetComponent<PlayerController>().Heft = true;
            yield return new WaitForSeconds(duration);
            gameObject.GetComponent<PlayerController>().Heft = false;
            _dictStatus.Remove(status);
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

    void InitFeedbackGroup()
    {
        for (int i = 0; i < hitFeedbackGroup.transform.childCount; i++)
        {
            var image = hitFeedbackGroup.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>();
            if (image == null)
            {//unica eccezion è la scrolling texture di UnlimitedPower
                var rawimage = hitFeedbackGroup.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.RawImage>();
                rawimage.color = new Color(rawimage.color.r, rawimage.color.g, rawimage.color.b, 0);
            }
            else
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
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
        if (hitFeedbackGroup == null)
        {
            hitFeedbackGroup = GameObject.FindGameObjectWithTag("HitFeedback").GetComponent<CanvasGroup>();
            InitFeedbackGroup();
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
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                StartCoroutine(Agility());
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                StartCoroutine(Poisoning());
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                StartCoroutine(Silence());
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                StartCoroutine(Heft());
            }
        }
    }

    private IEnumerator UpdateStatusHUD(StatusType eStatus, float duration, bool spawnText = true)
    {
        var oStatus = statusGroup.transform.Find(eStatus.ToString());//Canvas group con Background, Slider, Icon e Description
        oStatus.gameObject.SetActive(true);
        var slider = oStatus.transform.Find("Slider").GetComponent<UnityEngine.UI.Slider>();
        StartCoroutine(CountdownSlider(slider, duration));
        var oText = oStatus.transform.Find("Description");
        var text = oText.GetComponent<TextMeshProUGUI>();
        if (spawnText)
            StartCoroutine(TextFadeOut(text, 1f));
        yield return new WaitForSeconds(duration);
        _dictStatus.Remove(eStatus);
        oStatus.gameObject.SetActive(false);
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
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;  // Wait for the nnext frame
        }

        // Ensure the alpha is set to 0 at the end
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        yield return null;  // Wait for the nnext frame
    }

    //List non � modificabile, per cui
    //ad ogni ciclo controllo le durate degli effetti e aggiorno sottraendo il tempo dall'ultimo frame
    //se il tempo � finito despawno l'oggetto,
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


    IEnumerator HurtFlash()
    {
        var camshake = FindFirstObjectByType<CameraShake>();
        camshake.Enable();
        camshake.shakeDuration = .2f;
        //CameraShake.Shake(.1f, 1f);
        var hurtFlash = hitFeedbackGroup.transform.Find("RadialGradient").GetComponent<UnityEngine.UI.Image>();
        hurtFlash.gameObject.SetActive(true);
        hurtFlash.enabled = true;
        StartCoroutine(ImageFadeIn(hurtFlash, .05f));
        yield return new WaitForSeconds(.1f);
        StartCoroutine(ImageFadeOut(hurtFlash, .05f));
        //hurtFlash.enabled = false;
        //hurtFlash.gameObject.SetActive(true);
        yield return null;
    }

    private IEnumerator ImageFadeOut(UnityEngine.UI.Image image, float duration)
    {
        float startAlpha = 1f;
        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, timeElapsed / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;  // Wait for the next frame
        }

        // Ensure the alpha is set to 0 at the end
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
    private IEnumerator ImageFadeIn(UnityEngine.UI.Image image, float duration)
    {
        float startAlpha = 0f;
        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 1, timeElapsed / duration);
            Color newColor = new Color(image.color.r, image.color.g, image.color.b, alpha);
            image.color = newColor;
            yield return null;  // Wait for the next frame
        }

        // Ensure the alpha is set to 0 at the end
        Color finalColor = new Color(image.color.r, image.color.g, image.color.b, 0);
        image.color = finalColor;
    }
    void StatusSound(StatusType status)
    {
        AudioClip clip = null;
        if (statusClips.Count >= (int)status)
            clip = statusClips.ElementAt((int)status);
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>("Sounds/placeholder.wav");
        }
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
}
