using System;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    private CurrencySaveData _data;

    public int Gold => _data.gold;
    public int Gem => _data.gem;

    public event Action OnCurrencyChanged;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        DontDestroyOnLoad(gameObject);
        LoadCurrency();
    }

    private void LoadCurrency()
    {
        _data = CurrencySaveSystem.Load();
    }

    private void SaveCurrency()
    {
        CurrencySaveSystem.Save(_data);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        _data.gold += amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke();
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0)
            return false;

        if (_data.gold < amount)
            return false;

        _data.gold -= amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public void AddGem(int amount)
    {
        if (amount <= 0)
            return;

        _data.gem += amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke();
    }

    public bool SpendGem(int amount)
    {
        if (amount <= 0)
            return false;

        if (_data.gem < amount)
            return false;

        _data.gem -= amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke();
        return true;
    }

    public void SetCurrency(int gold, int gem)
    {
        _data.gold = Mathf.Max(0, gold);
        _data.gem = Mathf.Max(0, gem);
        SaveCurrency();
        OnCurrencyChanged?.Invoke();
    }
}