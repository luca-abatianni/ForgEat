using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.VFX;
public class PrimaryPower : MonoBehaviour
{
    public GameObject _firePoint;
    public List<GameObject> _listVFX = new List<GameObject>();
    private GameObject _effectToSpawn;

    private float _cooldown = 0f;
    void Start()
    {
        //base.OnStartClient();
        if (false)//(!base.IsOwner)
        {
            GetComponent<PrimaryPower>().enabled = false;
            return;
        }
        if (_listVFX.Count == 0)
            return;
        _effectToSpawn = _listVFX[0];
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time > _cooldown)
        {
            _cooldown = Time.time + 1f;
            SpawnMagic();
        }
    }
    void SpawnMagic()
    {
        GameObject vfx;
        if (_firePoint != null)
        {
            vfx = Instantiate(_effectToSpawn, _firePoint.transform.position, Quaternion.identity);
            vfx.GetComponent<PowerBehavior>().SetDirection(Camera.main.transform.forward);
        }
        else
        {
            Debug.Log("ERR PrimaryPower - Missing FirePoint");
        }
    }
}
