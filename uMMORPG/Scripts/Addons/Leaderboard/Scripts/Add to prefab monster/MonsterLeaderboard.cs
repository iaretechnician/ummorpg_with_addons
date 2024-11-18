using UnityEngine;

// ===================================================================================
// Player
// ===================================================================================
public class MonsterLeaderboard : MonoBehaviour
{
    public Monster monster;
    public Health health;

#if _SERVER
    private void Start()
    {
        if(monster == null) monster = GetComponent<Monster>();
        if(health == null) health = GetComponent<Health>();
        health.onEmpty.AddListener(OnDeathLeaderboard);
    }
    
    private void OnDeathLeaderboard()
    {
        if(monster.lastAggressor != null)
        {
            if(monster.lastAggressor is Player player)
            {
                ++player.playerAddonsConfigurator.monsterkill;
            }

        }
    }
#endif
}