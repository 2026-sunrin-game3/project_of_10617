using UnityEngine;
using System.Collections.Generic;
public enum MathType
{
    Increase,
    Decrease,
    Add,
    Remove
}

public class EntityStat : MonoBehaviour
{
    // 공격력, 방어력, 가하는 피해증가, 치명타 확률, 치명타 피해, 받는 피해증가, 공격속도, 이동속도, 
    public float attackDamage, defense, increaseDamage, critPer, critMul, hurtDamage, atkSpeed, moveSpeed;

    Dictionary<string, float> baseValue = new();
    Dictionary<string, float> resultValue = new();
    
    [SerializeField]
    public List<Buf> bufs = new();

    public struct Buf
    {
        public string Key;
        public float Value;
        public MathType MathType;
    }

    struct StatValue{
        public string Key;
        public float Value;
    }

    [SerializeField]

    List<StatValue> defaultStat = new()
    {
        new StatValue(Key="attackDamage", Value=0),
        new StatValue(Key="defense", Value=0),
        new StatValue(Key="increaseDamage", Value=0),
        new StatValue(Key="critPer", Value=30),
        new StatValue(Key="critMul", Value=0),
        new StatValue(Key="hurtDamage", Value=0),
        new StatValue(Key="atkSpeed", Value=0),
        new StatValue(Key="moveSpeed", Value=0)
    };

    void Start()
    {
        foreach (StatValue val in defaultStat)
        {
            baseValue[val.Key] = val.Value;
            Calc(Val.Key);
        }
    }


    public float Calc(string key)
    {
        float value = baseValue[key];
        float increase = 100;

        foreach (Buf buf in bufs)
        {
            switch (buf.MathType)
            {
                case MathType.Increase:
                increase += buf.Value;
                break;
                case MathType.Decrease:
                increase -= buf.Value;
                break;
                case MathType.Add:
                value += buf.Value;
                break;
                case MathType.Remove:
                value -= buf.Value;
                break;
            }
        }
        return resultValue[key] = value * increase * 0.01f;
    }
}
