using Mirror;

// NPC
public class MonsterHonorShop : NetworkBehaviour
{
    public Monster monster;
    public Health health;

    private void Start()
    {
        if(monster == null) monster = GetComponent<Monster>();
        if(health == null) health = GetComponent<Health>();
    }

    public override void OnStartServer()
    {
        health.onEmpty.AddListener(monster.OnDeath_HonorShop);
    }
}