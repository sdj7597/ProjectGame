using System.ComponentModel;
using UnityEngine;

public enum PlayerType { Melee, Ranged, Magic }

public class PlayerStats : MonoBehaviour
{
    public PlayerType playerType;

    public int maxHP;
    public int currentHP;

    public int maxMP;
    public int currentMP;

    public float expMax;
    public float currentExp;

    public int attackPower;
    public float attackRange;
    public float attackCooldown;
    public float criticalChance;

    public int level = 0;
    public int skillPoint = 1;
    public int hpUp;
    public int damageUp;

    public bool isDie = false;

    private void Awake()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        currentExp = expMax;

        attackRange = attackRange * 3;
    }

    #region 레벨 업
    public void GetExp(float value)
    {
        currentExp += value;
        if (currentExp >= expMax)
        {
            currentExp -= expMax;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        if (level > 20)
            level = 20;

        if (level % 2 == 0)
            skillPoint++;

        expMax *= 1.5f;

        maxHP += hpUp;
        attackPower += damageUp;
        currentHP = maxHP;
    }
    #endregion

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            isDie = true;
        }
        // Debug.Log($"{playerName} 데미지 {damage}. 남은 HP: {currentHP}");
    }

    public bool UseMP(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            // Debug.Log($"{playerName} MP {amount} 사용. 남은 MP: {currentMP}");
            return true;
        }
        return false;
    }

    public void RegenerateHP(float percentPerSecond, float deltaTime)
    {
        if (currentHP < maxHP)
        {
            currentHP = Mathf.Min(maxHP, currentHP + Mathf.RoundToInt(maxHP * percentPerSecond * deltaTime));
        }
    }

    // MP 재생 (항상)
    public void RegenerateMP(float percentPerSecond, float deltaTime)
    {
        if (currentMP < maxMP)
        {
            currentMP = Mathf.Min(maxMP, currentMP + Mathf.RoundToInt(maxMP * percentPerSecond * deltaTime));
        }
    }
}