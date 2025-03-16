using UnityEngine;

public class CheatManager : MonoBehaviour,IManager
{
    [Inject] private GameManager _gameManager;
    [Inject] private BetManager _betManager;
    [Inject] private UIManager _uiManager;
    [SerializeField] private CheatUI playerMoney,casinoMoney,rewardPoolMoney,returnToPlayer;
    [SerializeField] private CheatUI casinoBudgetText,rewardPoolBudgetText;
    [SerializeField] private CheatUI targetSpin,greenProbability,redProbability,blackProbability;
    
    
    private void Start()
    {
        DIContainer.AutoInject(this);
        playerMoney.OnValueChanged += OnPlayerMoneyChanged;
        casinoMoney.OnValueChanged += OnCasinoMoneyChanged;
        returnToPlayer.OnValueChanged += OnReturnToPlayerChanged;
        rewardPoolMoney.OnValueChanged += OnRewardPoolBudgetChanged;
        targetSpin.OnValueChanged += OnTargetSpinChanged;
        greenProbability.OnValueChanged += OnGreenProbabilityChanged;
        redProbability.OnValueChanged += OnRedProbabilityChanged;
        blackProbability.OnValueChanged += OnBlackProbabilityChanged;
        _gameManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
    }
    private void OnDestroy()
    {
        playerMoney.OnValueChanged -= OnPlayerMoneyChanged;
        casinoMoney.OnValueChanged -= OnCasinoMoneyChanged;
        returnToPlayer.OnValueChanged -= OnReturnToPlayerChanged;
        rewardPoolMoney.OnValueChanged -= OnRewardPoolBudgetChanged;
        targetSpin.OnValueChanged -= OnTargetSpinChanged;
        greenProbability.OnValueChanged -= OnGreenProbabilityChanged;
        redProbability.OnValueChanged -= OnRedProbabilityChanged;
        blackProbability.OnValueChanged -= OnBlackProbabilityChanged;
        _gameManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
    }
    
    public void UpdateCheatManagerUI()
    {
        casinoMoney.SetValue(_betManager.BetParameterData.casinoBudget);
        returnToPlayer.SetValue(_betManager.BetParameterData.returnToPlayer);
        rewardPoolMoney.SetValue(_betManager.BetParameterData.rewardPoolMoney);
        targetSpin.SetValue(_betManager.BetParameterData.targetSpin);
        greenProbability.SetValue(_betManager.BetParameterData.greenProbability);
        redProbability.SetValue(_betManager.BetParameterData.redProbability);
        blackProbability.SetValue(_betManager.BetParameterData.blackProbability);
    }
    private void OnPlayerDataLoaded() => playerMoney.SetValue(_gameManager.Money);
    private void OnPlayerMoneyChanged(float oldValue, float newValue) =>_gameManager.SetMoney((int)newValue);
    private void OnCasinoMoneyChanged(float oldValue, float newValue)
    {
        casinoBudgetText.SetValue(newValue);
        _betManager.SetCasinoBudget(newValue);
    }
    private void OnReturnToPlayerChanged(float oldValue, float newValue) => _betManager.SetReturnToPlayer(newValue);
    private void OnRewardPoolBudgetChanged(float oldValue, float newValue)
    {
        rewardPoolBudgetText.SetValue(newValue);
        _betManager.SetRewardPoolMoney(newValue);
    }
    private void OnTargetSpinChanged(float oldValue, float newValue) => _betManager.SetTargetSpin(newValue);
    private void OnGreenProbabilityChanged(float oldValue, float newValue) => _betManager.SetGreenProbability(newValue);
    private void OnRedProbabilityChanged(float oldValue, float newValue) =>  _betManager.SetRedProbability(newValue);
    private void OnBlackProbabilityChanged(float oldValue, float newValue) => _betManager.SetBlackProbability(newValue);
    public void SaveButtonClicked()
    {
        _betManager.SaveBetParameterData();
        _gameManager.SaveData();
    }
    
    public void CloseButtonClicked()
    {
        _betManager.SaveBetParameterData();
        _uiManager.CloseChestPanel();
    }
    
    public void DeleteSaveData() => _gameManager.DeleteSaveData();
    public void ResetBetParameter() => _betManager.ResetBetParameterData();
}
