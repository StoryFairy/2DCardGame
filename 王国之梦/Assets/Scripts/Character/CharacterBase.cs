using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public int maxHp;
    public IntVariable hp;
    public IntVariable defense;
    public IntVariable buffRound;

    public int CurrentHp
    {
        get => hp.currentValue;
        set => hp.SetValue(value);
    }

    public int MaxHp
    {
        get => hp.maxValue;
    }

    protected Animator animator;

    public bool isDead;

    public GameObject buff;
    public GameObject debuff;

    [Header("力量相关")] 
    public float baseStrength = 1f;
    private float strengthEffect = 0.5f;

    [Header("事件广播")] 
    public ObjectEventSO characterDeadEvent;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        hp.maxValue = maxHp;
        CurrentHp = MaxHp;
        buffRound.currentValue = buffRound.maxValue;
        ResetDefense();
    }

    protected virtual void Update()
    {
       animator.SetBool("isDead",isDead); 
    }

    public virtual void TakeDamage(int damage)
    {
        var currentDamage = (damage - defense.currentValue) >= 0 ? (damage - defense.currentValue) : 0;
        var currentDefense = (damage - defense.currentValue) >= 0 ? 0 : (defense.currentValue - damage);
        defense.SetValue(currentDefense);
        if (CurrentHp > currentDamage)
        {
            CurrentHp -= currentDamage;
            animator.SetTrigger("hit");
        }
        else
        {
            CurrentHp = 0;
            isDead = true;
            characterDeadEvent.RaisEvent(this, this);
        }
    }

    public void UpdateDefense(int amount)
    {
        var value = defense.currentValue + amount;
        defense.SetValue(value);
    }

    public void ResetDefense()
    {
        defense.SetValue(0);
    }

    public void HealHealth(int amount)
    {
        CurrentHp += amount;
        CurrentHp = Mathf.Min(CurrentHp, MaxHp);
        buff.SetActive(true);
    }

    public void SetupStrength(int round, bool isPositive)
    {
        if (isPositive)
        {
            float newStrength = baseStrength + strengthEffect;
            baseStrength = Mathf.Min(newStrength, 1.5f);
            buff.SetActive(true);
        }
        else
        {
            debuff.SetActive(true);
            baseStrength = 1 - strengthEffect;
        }

        var currentRound = buffRound.currentValue + round;
        if (baseStrength == 1)
            buffRound.SetValue(0);
        else
            buffRound.SetValue(currentRound);
    }

    /// <summary>
    /// 回合转换事件函数
    /// </summary>
    public void UpdateStrengthRound()
    {
        buffRound.SetValue(buffRound.currentValue-1);
        if (buffRound.currentValue <= 0)
        {
            buffRound.SetValue(0);
            baseStrength = 1;
        }
    }
}
