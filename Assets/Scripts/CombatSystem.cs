using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CombatAction
{
    public string actionType;
    public int damage;
    public float cooldown;
    public float currentCooldown;
}

[System.Serializable]
public class CombatStats
{
    public int hp;
    public int maxHp;
    public int mp;
    public int maxMp;
    public int attack;
    public int defense;
    public int magic;
    public int speed;
}

public class CombatSystem : MonoBehaviour
{
    private CombatStats _playerStats;
    private CombatStats _enemyStats;
    private CombatAction[] _playerActions;
    private bool _inCombat = false;
    private float _combatTickTimer = 0f;
    private float _combatTickRate = 0.1f;
    
    public UnityEvent<int> OnPlayerDamage = new UnityEvent<int>();
    public UnityEvent<int> OnEnemyDamage = new UnityEvent<int>();
    public UnityEvent OnCombatStart = new UnityEvent();
    public UnityEvent OnCombatEnd = new UnityEvent();
    
    private void Start()
    {
        InitializePlayerStats();
        InitializePlayerActions();
    }
    
    private void InitializePlayerStats()
    {
        CharacterData character = CharacterManager.Instance.GetCurrentCharacter();
        if (character != null)
        {
            _playerStats = new CombatStats
            {
                hp = character.hp,
                maxHp = character.hp,
                mp = character.mp,
                maxMp = character.mp,
                attack = 15,
                defense = 10,
                magic = 8,
                speed = 12
            };
        }
    }
    
    private void InitializePlayerActions()
    {
        _playerActions = new CombatAction[]
        {
            new CombatAction { actionType = "attack", damage = 10, cooldown = 0.5f, currentCooldown = 0f },
            new CombatAction { actionType = "skill1", damage = 20, cooldown = 2f, currentCooldown = 0f },
            new CombatAction { actionType = "skill2", damage = 15, cooldown = 1.5f, currentCooldown = 0f }
        };
    }
    
    public void StartCombat(CombatStats enemyStats)
    {
        _inCombat = true;
        _enemyStats = enemyStats;
        OnCombatStart.Invoke();
    }
    
    public void ExecuteAction(string actionType)
    {
        if (!_inCombat)
            return;
        
        CombatAction action = System.Array.Find(_playerActions, a => a.actionType == actionType);
        if (action == null || action.currentCooldown > 0)
            return;
        
        int damage = CalculateDamage(_playerStats, _enemyStats, action);
        _enemyStats.hp -= damage;
        OnPlayerDamage.Invoke(damage);
        
        action.currentCooldown = action.cooldown;
        
        if (_enemyStats.hp <= 0)
        {
            EndCombat(true);
            return;
        }
        
        EnemyAction();
    }
    
    private void EnemyAction()
    {
        if (!_inCombat || _enemyStats.hp <= 0)
            return;
        
        int damage = CalculateDamage(_enemyStats, _playerStats, null);
        _playerStats.hp -= damage;
        OnEnemyDamage.Invoke(damage);
        
        if (_playerStats.hp <= 0)
        {
            EndCombat(false);
        }
    }
    
    private int CalculateDamage(CombatStats attacker, CombatStats defender, CombatAction action)
    {
        int baseDamage = action != null ? action.damage : attacker.attack;
        int damage = baseDamage + Random.Range(-3, 4);
        damage = Mathf.Max(1, damage - (defender.defense / 2));
        
        if (Random.value < 0.15f)
        {
            damage = (int)(damage * 1.5f);
        }
        
        return damage;
    }
    
    private void Update()
    {
        if (!_inCombat)
            return;
        
        _combatTickTimer += Time.deltaTime;
        if (_combatTickTimer >= _combatTickRate)
        {
            UpdateCooldowns();
            _combatTickTimer = 0f;
        }
    }
    
    private void UpdateCooldowns()
    {
        foreach (var action in _playerActions)
        {
            if (action.currentCooldown > 0)
            {
                action.currentCooldown -= _combatTickRate;
            }
        }
    }
    
    private void EndCombat(bool playerWon)
    {
        _inCombat = false;
        OnCombatEnd.Invoke();
        
        if (playerWon)
        {
            Debug.Log("Vitória!");
        }
        else
        {
            Debug.Log("Derrota!");
        }
    }
    
    public bool IsInCombat => _inCombat;
    public CombatStats GetPlayerStats() => _playerStats;
    public CombatStats GetEnemyStats() => _enemyStats;
    public CombatAction[] GetPlayerActions() => _playerActions;
}
