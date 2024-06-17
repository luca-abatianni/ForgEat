using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowerBehavior;

public class BulletCollision : NetworkBehaviour
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletCollision : MonoBehaviour
>>>>>>> Stashed changes
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");
        Destroy(gameObject);
    }

        Debug.Log("Bullet Collision!");
        Destroy(this.gameObject);
    }
}
