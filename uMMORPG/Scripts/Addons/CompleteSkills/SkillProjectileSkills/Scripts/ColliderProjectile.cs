using Mirror;
using UnityEngine;

// COLLIDER PROJECTILE

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(NetworkIdentity))]
public partial class ColliderProjectile : Projectile
{
    protected float fDistance = 0;
    protected Vector3 targetPosition;
    protected Vector3 startPosition;

    // -----------------------------------------------------------------------------------
    // Init
    // -----------------------------------------------------------------------------------
    public override void Init(Entity target)
    {
        base.Init(target);
        startPosition = transform.position;
        targetPosition = target.collider.bounds.center;
    }

    // -----------------------------------------------------------------------------------
    // OnStartClient
    // -----------------------------------------------------------------------------------
    public override void OnStartClient()
    {
#if UNITY_6000_0_OR_NEWER
        GetComponent<Rigidbody>().linearVelocity
#else
        GetComponent<Rigidbody>().velocity
#endif
            = transform.forward * speed;
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerEnter
    // -----------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // -- Impact Check only Server Side
        if (!isServer || bArrivedAtTarget) return;

        if (wallTag != "" && other.gameObject.tag == wallTag)
        {
            OnDestroyDelayed();
            return;
        }

        Entity candidate = other.gameObject.GetComponentInParent<Entity>();
        //candidate.target
        Debug.Log("Candidat = " + candidate.target.name);
        if (candidate.target != null && candidate.target != caster && caster.CanAttack(candidate.target) && candidate.target.isAlive)
            OnProjectileImpact(candidate);
    }

    // -----------------------------------------------------------------------------------
    // FixedUpdate
    // -----------------------------------------------------------------------------------
    public override void FixedUpdate()
    {
        if (bArrivedAtTarget) return;

#if UNITY_6000_0_OR_NEWER
        GetComponent<Rigidbody>().linearVelocity
#else
        GetComponent<Rigidbody>().velocity
#endif
            = transform.forward * speed;

        fDistance = Vector3.Distance(transform.position, startPosition);

        // -- Impact Check only Server Side
        if (isServer && fDistance >= data.distance)
        {
            bArrivedAtTarget = true;

            if (destroyDelay != 0)
            {
                Invoke("OnDestroyDelayed", destroyDelay);
            }
            else
            {
                OnDestroyDelayed();
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // checkWall
    // -----------------------------------------------------------------------------------
    protected void checkWall()
    {
        if (!isServer || wallTag == "") return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.25f))
        {
            if (hit.collider.tag == wallTag)
            {
                if (data.impactEffect != null)
                {
                    GameObject go = Instantiate(data.impactEffect.gameObject, transform.position, Quaternion.identity);
                    go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
                    go.GetComponent<OneTimeTargetSkillEffect>().target = target;
                    NetworkServer.Spawn(go);
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
