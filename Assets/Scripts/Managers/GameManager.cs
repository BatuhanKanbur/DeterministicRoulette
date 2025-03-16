using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : HookBehaviour,IManager
{
    #region Properties
    [HookVar(nameof(OnMoneyChanged))] public int Money{ private set;get;}=-1;
    [HookVar(nameof(OnEarnedMoneyChanged))] private int EarnedMoney{ set;get;}=-1;
    [HookVar(nameof(OnTotalBetChanged))] private int TotalBet{ set;get;}=-1;
    [HookVar(nameof(GameStateChanged))] public GameState GameState { private set; get; } = GameState.Cheating;
    public int SpinResult {private set; get; }
    public ChipObject[] ChipObjects { get; private set;}
    private readonly List<Bet> _bets = new();
    private List<int> OldNumbers { get; set; } = new();
    private PlayerData _playerData;
    #endregion

    #region Delegates
    public event Action OnChipObjectsLoaded;
    public event Action<int,int> OnMoneyChanged;
    public event Action<int,int> OnEarnedMoneyChanged;
    public event Action<int,int> OnTotalBetChanged;
    public event Action<GameState> OnGameStateChanged;
    public event Action<PlayerState,int> OnTurnCompleted;
    public Action<List<int>> OnOldNumbersChanged;
    public Action<List<Bet>> OnBetsChanged;
    #endregion

    #region HookMethods
    private void GameStateChanged(GameState oldValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(newValue);
        if(newValue == GameState.Result) 
            SpinCompleted();
        SaveData();
    }
    #endregion

    #region Fields
    [Inject] private AudioManager _audioManager;
    [Inject] private BetManager _betManager;
    #endregion
    private async void Start()
    {
        DIContainer.AutoInject(this);
        ChipObjects =(await AssetManager<ChipObject>.LoadObjects(AssetConstants.ChipObjectsLabel)).ToArray();
        var chipObjectTasks = ChipObjects.Select(chipObject => chipObject.LoadChipAssets()).ToList();
        await Task.WhenAll(chipObjectTasks);
        OnChipObjectsLoaded?.Invoke();
        LoadData();
    }
    private void LoadData()
    {
        _playerData = SaveLoadSystem.Load<PlayerData>();
        Money = _playerData.totalMoney;
        EarnedMoney = _playerData.earnedMoney;
        TotalBet = 0;
        OldNumbers = _playerData.oldNumbers;
        OnOldNumbersChanged?.Invoke(OldNumbers);
        GameState = GameState.Idle;
    }

    internal void SaveData()
    {
        _playerData.UpdateData(Money,EarnedMoney,OldNumbers);
    }
    internal void DeleteSaveData()
    {
        SaveLoadSystem.Save(new PlayerData());
        LoadData();
    }
    internal void SetGameState(GameState gameState)
    {
        GameState = gameState;
    }
    
    internal void SetMoney(int amount)
    {
        Money = amount;
    }
    

    public void AddBet(Bet bet)
    {
        if(bet == null) return;
        bet.InitChipObject();
        _bets.Add(bet);
        OnBetsChanged?.Invoke(_bets);
        TotalBet += bet.BetValue;
        Money -= bet.BetValue;
    }
    public void RemoveBet(GameObject betTransform)
    {
        var bet = _bets.FirstOrDefault(b => b.BetChipObject == betTransform);
        if(bet == null) return;
        TotalBet -= bet.BetValue;
        Money += bet.BetValue;
        bet.Dispose();
        _bets.Remove(bet);
        OnBetsChanged?.Invoke(_bets);
    }

    public void StartSpin()
    {
        GameState = GameState.Spinning;
        SpinResult = _betManager.GetSpinResult(_bets);
        _audioManager.PlaySound(AssetConstants.WheelAudio);
    }
    private async void SpinCompleted()
    {
        _audioManager.StopSound();
        var earnedMoney = 0;
        foreach (var bet in _bets)
        {
            if (bet.BetNumbers.Contains(SpinResult))
            {
                earnedMoney += bet.TotalBetValue;
                _ = ObjectManager.GetObject(AssetConstants.GetChipParticleName(bet.BetMultiplier), bet.BetPosition);
                _audioManager.PlaySound(AssetConstants.ChipWinAudio);
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
        OnBetsChanged?.Invoke(_bets);
        OnTurnCompleted?.Invoke(playerState,earnedMoney);
        GameState = GameState.Idle;
    }
    private void AddHistoryNumber(int spinResult)
    {
        if(OldNumbers.Count>16)
            OldNumbers.Clear();
        OldNumbers.Add(spinResult);
        OnOldNumbersChanged?.Invoke(OldNumbers);
    }
}
