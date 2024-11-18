using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// PROJECTILE

public abstract partial class Projectile : SkillEffect
{
    [Header("[-=-[ Projectile Settings ]-=-]")]
    [Tooltip("[Optional] Select a tag that act as wall and stops the projectile")]
    public string wallTag = "";

    [Tooltip("[Optional] Delay in seconds after which the projectile is destroyed on impact")]
    public int destroyDelay = 1;

    [SyncVar, HideInInspector] public float speed;

    public UnityEvent onSetInitialPosition;

    public Tools_DamageData data = new Tools_DamageData();

    protected List<Entity> targets = new List<Entity>();
    protected Entity currentTarget = null;
    protected bool bArrivedAtTarget = false;

    // -----------------------------------------------------------------------------------
    // OnStartClient
    // -----------------------------------------------------------------------------------
    public override void OnStartClient()
    {
        if (target != null && caster != null)
        {
            transform.position = caster.skills.effectMount.position;
            onSetInitialPosition.Invoke();
        }
    }

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public virtual void Init(Entity target)
    {
        SpawnSidekicks();
        RotateProjectile(target);
        FixedUpdate();
    }

    // -----------------------------------------------------------------------------------
    // FixedUpdate
    // -----------------------------------------------------------------------------------
    public virtual void FixedUpdate()
    {
        if (isServer) return;

        if (target != null && caster != null)
        {
            Vector3 goal = target.collider.bounds.center;
            transform.LookAt(goal);
        }
        else if (isServer && caster == null)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    // -----------------------------------------------------------------------------------
    // SpawnEffect
    // -----------------------------------------------------------------------------------
    public void SpawnEffect(Entity caster, Entity spawnTarget)
    {
        if (data.impactEffect != null)
        {
            GameObject go = Instantiate(data.impactEffect, transform.position, Quaternion.identity);

            if (go.GetComponent<OneTimeTargetSkillEffect>() != null)
            {
                go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
                go.GetComponent<OneTimeTargetSkillEffect>().target = spawnTarget;
            }

            NetworkServer.Spawn(go);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnProjectileImpact
    // Server
    // -----------------------------------------------------------------------------------
    protected void OnProjectileImpact(Entity _target = null)
    {
        // ----- Projectile arrived, no matter if damage is dealt
        if (bArrivedAtTarget) return;

        bArrivedAtTarget = true;

        if (_target != null)
            currentTarget = _target;
        else
            currentTarget = caster.target;

        // ------ spawn visual effect if any
        if (data.visualEffectOnMainTargetOnly || data.impactRadius <= 0)
            SpawnEffect(caster, currentTarget);

        // ------ get all valid targets
        if (data.impactRadius > 0)
        {
            if (caster is Player player)
                targets = player.Tools_GetCorrectedTargetsInSphere(currentTarget.transform, data.impactRadius, false, data.notAffectSelf, data.notAffectOwnParty, data.notAffectOwnGuild, data.notAffectOwnRealm, data.reverseTargeting, data.notAffectPlayers, data.notAffectNpcs, data.notAffectMonsters, data.notAffectPets);
            else
                targets = caster.Tools_GetCorrectedTargetsInSphere(currentTarget.transform, data.impactRadius, false, data.notAffectSelf, data.notAffectOwnParty, data.notAffectOwnGuild, data.notAffectOwnRealm, data.reverseTargeting, data.notAffectPlayers, data.notAffectNpcs, data.notAffectMonsters, data.notAffectPets);
        }
        else
        {
            targets.Add(currentTarget);
        }

        // ----- apply effects to targets
        foreach (Entity target in targets)
        {

            // ------ Spawn Visual Effect (if any)
            if (!data.visualEffectOnMainTargetOnly && data.impactRadius > 0)
                SpawnEffect(caster, target);

            // ------ Deal Damage

            int damage = data.damage;
            if (data.addCasterDamage) damage += caster.combat.damage;

            float stunChance = data.stunChance;
#if _iMMOATTRIBUTES
            if (data.stunAddAccuracy) stunChance = target.Tools_HarmonizeChance(stunChance, caster.combat.accuracy);
#endif
            caster.combat.DealDamageAt(target, damage, stunChance, UnityEngine.Random.Range(data.minStunTime, data.maxStunTime));

            // ------ Skip remaining calculations if target is dead already
            if (target == null) continue;

            // ------ Remove random Buff
            if (data.removeRandomBuff > 0 && caster.target.skills.buffs.Count > 0)
            {
                float removeChance = 0;
#if _iMMOATTRIBUTES
                if (data.removeAddAccuracy) removeChance = target.Tools_HarmonizeChance(removeChance, caster.combat.accuracy);
#endif
                caster.target.Tools_CleanupStatusBuffs(data.removeChance, removeChance, data.removeRandomBuff);
            }

            // ------ Recoil Target
            if (data.recoilChance > 0 && data.minRecoilTarget > -100f && data.maxRecoilTarget > -100f)
            {
                float recoilChnce = data.recoilChance;
#if _iMMOATTRIBUTES
                if (data.recoilAddAccuracy) recoilChnce = target.Tools_HarmonizeChance(recoilChnce, caster.combat.accuracy);
#endif
                if (UnityEngine.Random.value <= recoilChnce)
                    target.Tools_Recoil(caster, UnityEngine.Random.Range(data.minRecoilTarget, data.maxRecoilTarget));
            }

            // ------ Cooldown Target
            if (data.cooldownChance > 0)
            {
                float cldwnChnce = data.cooldownChance;
#if _iMMOATTRIBUTES
                if (data.cooldownAddAccuracy) cldwnChnce = target.Tools_HarmonizeChance(cldwnChnce, caster.combat.accuracy);
#endif
                for (int i = 0; i < target.skills.skills.Count; ++i)
                {
                    Skill skill = target.skills.skills[i];
                    if (skill.IsOnCooldown() && UnityEngine.Random.value <= cldwnChnce)
                    {
                        skill.cooldownEnd += data.cooldownDuration;
                        target.skills.skills[i] = skill;
                    }
                }
            }

            // ------ Apply Buff (if any)
            if (data.applyBuff != null)
            {
                float buffModifier = 0;
#if _iMMOATTRIBUTES
                if (data.buffAddAccuracy) buffModifier = target.Tools_HarmonizeChance(buffModifier, caster.combat.accuracy);
#endif
                target.Tools_ApplyBuff(data.applyBuff, data.buffLevel, data.buffChance, buffModifier);
            }

            // ------ Check for Aggro Trigger
            target.Tools_OnAggro(caster, data.triggerAggroChance);
        }

        // ------ create object at impact loaction
        if (data.createOnTarget.Length > 0 && data.createOnTarget.Length >= data.skillLevel - 1 && data.createOnTarget[data.skillLevel - 1] != null && UnityEngine.Random.value <= data.createChance)
        {
            GameObject go = Instantiate(data.createOnTarget[data.skillLevel - 1], caster.target.transform.position, caster.target.transform.rotation);
            NetworkServer.Spawn(go);
        }

        targets.Clear();

        // ----- Finally destroy the projectile itself

        if (destroyDelay != 0)
        {
            Invoke(nameof(OnDestroyDelayed), destroyDelay);
        }
        else
        {
            OnDestroyDelayed();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDestroyDelayed
    // -----------------------------------------------------------------------------------
    protected void OnDestroyDelayed()
    {
        CancelInvoke("OnDestroyDelayed");
        //SpawnEffect(caster, caster.target);
        NetworkServer.Destroy(gameObject);
    }

    // -----------------------------------------------------------------------------------
    // calcBallisticVelocityVector
    // -----------------------------------------------------------------------------------
    protected Vector3 CalcBallisticVelocityVector(Transform source, Vector3 target, float angle)
    {
        Vector3 direction = target - source.position;
        float h = direction.y;                                          // get height difference
        direction.y = 0;                                                // remove height
        float distance = direction.magnitude;                           // get horizontal distance
        float a = angle * Mathf.Deg2Rad;                                // Convert angle to radians
        direction.y = distance * Mathf.Tan(a);                          // Set direction to elevation angle
        distance += h / Mathf.Tan(a);                                     // Correction for small height differences
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    // -----------------------------------------------------------------------------------
    // SpawnSidekick
    // -----------------------------------------------------------------------------------
    protected void SpawnSidekicks()
    {
        if (data.sidekick || data.sidekickAmount <= 0) return;

        for (int i = 0; i < data.sidekickAmount; ++i)
        {
            if (UnityEngine.Random.value <= data.sidekickSpawnChance)
                Invoke(nameof(SpawnSidekick), i * data.sidekickSpawnDelay);
        }
    }

    // -----------------------------------------------------------------------------------
    // SpawnSidekick
    // -----------------------------------------------------------------------------------
    protected void SpawnSidekick()
    {
        GameObject go = Instantiate(this.gameObject, caster.skills.effectMount.position, transform.rotation);

        Projectile effect = go.GetComponent<Projectile>();

        effect.speed = data.speed;

        effect.target = target;
        effect.caster = caster;
        effect.data.speed = data.speed;
        effect.data.distance = data.distance;

        effect.data.skillLevel = data.skillLevel;
        effect.data.damage = data.damage;
        effect.data.addCasterDamage = data.addCasterDamage;

        if (!data.sidekicksDontStun)
        {
            effect.data.stunChance = data.stunChance;
            effect.data.stunAddAccuracy = data.stunAddAccuracy;
            effect.data.minStunTime = data.minStunTime;
            effect.data.maxStunTime = data.maxStunTime;
        }

        effect.data.sidekickSpawnChance = data.sidekickSpawnChance;
        effect.data.sidekickAmount = 0;
        effect.data.sidekickSpawnDelay = 0;
        effect.data.sidekickSpreadAngle = data.sidekickSpreadAngle;
        effect.data.sidekick = true;

        effect.data.cooldownChance = data.cooldownChance;
        effect.data.cooldownDuration = data.cooldownDuration;
        effect.data.cooldownAddAccuracy = data.cooldownAddAccuracy;

        if (data.applyBuff != null && data.buffLevel > 0 && !data.sidekicksDontBuff)
        {
            effect.data.applyBuff = data.applyBuff;
            effect.data.buffLevel = data.buffLevel;
            effect.data.buffChance = data.buffChance;
            effect.data.buffAddAccuracy = data.buffAddAccuracy;
        }

        if (!data.sidekicksDontAOE)
        {
            effect.data.impactEffect = data.impactEffect;
            effect.data.impactRadius = data.impactRadius;
            effect.data.triggerAggroChance = data.triggerAggroChance;
            effect.data.visualEffectOnMainTargetOnly = data.visualEffectOnMainTargetOnly;
            effect.data.reverseTargeting = data.reverseTargeting;
            effect.data.notAffectSelf = data.notAffectSelf;
            effect.data.notAffectOwnParty = data.notAffectOwnParty;
            effect.data.notAffectOwnGuild = data.notAffectOwnGuild;
            effect.data.notAffectOwnRealm = data.notAffectOwnRealm;
            effect.data.notAffectPlayers = data.notAffectPlayers;
            effect.data.notAffectNpcs = data.notAffectNpcs;
            effect.data.notAffectMonsters = data.notAffectMonsters;
            effect.data.notAffectPets = data.notAffectPets;
        }

        if (!data.sidekicksDontDebuff)
        {
            effect.data.removeRandomBuff = data.removeRandomBuff;
            effect.data.removeChance = data.removeChance;
            effect.data.removeAddAccuracy = data.removeAddAccuracy;
        }

        if (!data.sidekicksDontCreateObject)
        {
            effect.data.createOnTarget = data.createOnTarget;
            effect.data.createChance = data.createChance;
        }

        //effect.Init();
        RotateProjectile(target);

        NetworkServer.Spawn(go);
    }

    // -----------------------------------------------------------------------------------
    // RotateProjectile
    // -----------------------------------------------------------------------------------
    protected void RotateProjectile(Entity target)
    {
        if (data.sidekick)
        {
            float fDegrees = UnityEngine.Random.Range(data.sidekickSpreadAngle * -1, Mathf.Abs(data.sidekickSpreadAngle));
            this.gameObject.transform.Rotate(new Vector3(0, fDegrees, 0));
        }
        else
        {
            this.gameObject.transform.LookAt(target.collider.bounds.center);
        }
    }

    // -----------------------------------------------------------------------------------
}
