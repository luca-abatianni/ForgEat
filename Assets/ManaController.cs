using FishNet.Managing.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaController : MonoBehaviour
{
    [Header("Mana Main Parameters")]
    public float playerMana = 100f;
    [SerializeField] private float maxMana = 100f;
    [HideInInspector] public bool isFull = true;
    [HideInInspector] public bool isShielding = false;
    [HideInInspector] public bool UnlimitedPower = false;

    [Header("Mana Regen Parameters")]
    [Range(0, 50)][SerializeField] public float shieldCost = 10f;
    [Range(0, 50)][SerializeField] private float manaRegen = 3f;

    [Header("Mana UI Elements")]
    [SerializeField] private Image manaProgressUI = null;
    [SerializeField] private CanvasGroup sliderCanvasGroup = null;

    //TODO https://www.youtube.com/watch?v=Fs2YCoamO_U
    private FirstPersonController fpc = null;
    private float regenDelay = 0f;
    private Image _manaNotEnough = null;
    private void Start()
    {
    }
    public void UpdateMana(float mana)
    {
        if (UnlimitedPower)//mana massimo e non decrementa
            playerMana = maxMana;
        else
            playerMana += mana;
        if (playerMana > maxMana)
            playerMana = maxMana;
    }
    // Update is called once per frame
    void Update()
    {
        if (fpc == null)
            fpc = GetComponent<FirstPersonController>();
        if (sliderCanvasGroup == null)
            sliderCanvasGroup = GameObject.FindGameObjectWithTag("ManaCanvasGroup").GetComponent<CanvasGroup>();
        if (manaProgressUI == null)
            manaProgressUI = GameObject.FindGameObjectWithTag("ManaImageSlider").GetComponent<Image>();
        if (_manaNotEnough == null)
            _manaNotEnough = sliderCanvasGroup.transform.Find("ManaNotEnough").GetComponent<Image>();



        if (isShielding)
        {
            UpdateMana(-shieldCost * Time.deltaTime);
            regenDelay = Time.time + .1f;
        }
        else
        {
            if (playerMana <= maxMana - 0.01 && Time.time > regenDelay)
            {
                UpdateMana(manaRegen * Time.deltaTime);
            }
        }

        manaProgressUI.fillAmount = playerMana / maxMana;
    }
    private IEnumerator ThreadNotEnoughMana()
    {
        float t = 0f;
        for (int i = 0; i < 4; i++)
        {
            if (i % 2 == 0)
            {
                Color newColor = new Color(1, 0, 0, t);
                _manaNotEnough.color = newColor;

            }
            else
            {
                Color newColor = new Color(1, 0, 0, 1f - t);
                _manaNotEnough.color = newColor;
            }
            yield return new WaitForSeconds(.3f);
        }
        _manaNotEnough.color = new Color(1, 0, 0, 0);
        yield return null;
    }
    public void NotEnoughMana()
    {
        StartCoroutine(ThreadNotEnoughMana());

    }
}
