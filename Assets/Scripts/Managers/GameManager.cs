using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : HookBehaviour,IManager
{
    #region Properties
    [HookVar(nameof(MoneyChanged))]
    public int Money{private set;get;}=-1;
    [HookVar(nameof(EarnedMoneyChanged))]
    public int EarnedMoney{private set;get;}=-1;
    [HookVar(nameof(TotalBetChanged))]
    public int TotalBet{private set;get;}=-1;
    [HookVar(nameof(GameStateChanged))]
    public GameState GameState { set; get; } = GameState.Cheating;
    public int SpinResult {private set; get; }
    public ChipObject[] ChipObjects { get; private set;}
    private readonly List<Bet> _bets = new List<Bet>();
    private List<int> _oldNumbers { get; } = new();

    #endregion

    #region Delegates
    public Action OnChipObjectsLoaded;
    public Action<int> OnMoneyChanged;
    public Action<int> OnEarnedMoneyChanged;
    public Action<int> OnTotalBetChanged;
    public Action<GameState> OnGameStateChanged;
    public Action<PlayerState,int> OnTurnCompleted;
    public Action<List<int>> OnOldNumbersChanged;
    #endregion

    #region HookMethods
    private void MoneyChanged(int oldValue, int newValue)
    {
        OnMoneyChanged?.Invoke(newValue);
    }
    private void EarnedMoneyChanged(int oldValue, int newValue)
    {
        OnEarnedMoneyChanged?.Invoke(newValue);
    }
    private void TotalBetChanged(int oldValue, int newValue)
    {
        OnTotalBetChanged?.Invoke(newValue);
    }
    private void GameStateChanged(GameState oldValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(newValue);
        switch (newValue)
        {
            case GameState.Cheating:
                break;
            case GameState.Idle:
                break;
            case GameState.Betting:
                break;
            case GameState.Spinning:
                break;
            case GameState.Result:
                SpinCompleted();
                break;
        }
    }
    #endregion
    private async void Start()
    {
        DIContainer.AutoInject(this);
        ChipObjects =(await AssetManager<ChipObject>.LoadObjects(AssetConstants.ChipObjectsLabel)).ToArray();
        var chipObjectTasks = ChipObjects.Select(chipObject => chipObject.LoadChipAssets()).ToList();
        await Task.WhenAll(chipObjectTasks);
        OnChipObjectsLoaded?.Invoke();
        LoadSaveData();
    }
    private void LoadSaveData()
    {
        GameState = GameState.Idle;
        Money = 23000;
        EarnedMoney = 0;
        TotalBet = 0;
    }

    public void AddBet(Bet bet)
    {
        if(bet == null) return;
        bet.InitChipObject();
        _bets.Add(bet);
        TotalBet += bet.BetValue;
        Money -= bet.BetValue;
    }

    public void StartSpin()
    {
        GameState = GameState.Spinning;
        SpinResult = UnityEngine.Random.Range(0, 37);
    }
    private async void SpinCompleted()
    {
        var earnedMoney = 0;
        foreach (var bet in _bets)
        {
            if (bet.BetNumbers.Contains(SpinResult))
            {
                earnedMoney += bet.BetValue * bet.BetMultiplier;
                _ = ObjectManager.GetObject(AssetConstants.GetChipParticleName(bet.BetMultiplier), bet.BetPosition);
            }
            else
            {
                earnedMoney -= bet.BetValue;
                _ = ObjectManager.GetObject(AssetConstants.ChipLostParticle01, bet.BetPosition);
            }
            bet.Dispose();
        }
        var playerState = earnedMoney > 0 ? PlayerState.Win : earnedMoney == 0 ? PlayerState.Pass : PlayerState.Lose;
        EarnedMoney += earnedMoney;
        Money += earnedMoney;
        TotalBet = 0;
        _bets.Clear();
        AddHistoryNumber(SpinResult);
        await Task.Delay(TimeSpan.FromSeconds(2));
        OnTurnCompleted?.Invoke(playerState,earnedMoney);
        GameState = GameState.Idle;
    }
    private void AddHistoryNumber(int spinResult)
    {
        if(_oldNumbers.Count>16)
            _oldNumbers.Clear();
        _oldNumbers.Add(spinResult);
        OnOldNumbersChanged?.Invoke(_oldNumbers);
    }
}
