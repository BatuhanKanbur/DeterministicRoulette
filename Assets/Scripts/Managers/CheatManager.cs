using System;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : MonoBehaviour,IManager
{
    [Inject] private GameManager _gameManager;
    [Inject] private BetManager _betManager;
    [Inject] private UIManager _uiManager;
    [SerializeField] private CheatUI playerMoney,casinoMoney,rewardPoolMoney,returnToPlayer;
    [SerializeField] private CheatUI casinoBudgetText,rewardPoolBudgetText;
    [SerializeField] private CheatUI targetSpin,greenProbability,redProbability,blackProbability;
    [SerializeField] private Button saveButton,closeButton,deleteSaveDataButton,resetBetParameterButton;
    
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
        saveButton.onClick.AddListener(SaveButtonClicked);
        closeButton.onClick.AddListener(CloseButtonClicked);
        deleteSaveDataButton.onClick.AddListener(DeleteSaveData);
        resetBetParameterButton.onClick.AddListener(ResetBetParameter);
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
        saveButton.onClick.RemoveListener(SaveButtonClicked);
        closeButton.onClick.RemoveListener(CloseButtonClicked);
        deleteSaveDataButton.onClick.RemoveListener(DeleteSaveData);
        resetBetParameterButton.onClick.RemoveListener(ResetBetParameter);
    }

    private void OnEnable()
    {
        UpdateCheatManagerUI();
    }

    private void UpdateCheatManagerUI()
    {
        _betManager ??= DIContainer.Resolve<BetManager>();
        _gameManager ??= DIContainer.Resolve<GameManager>();
        casinoMoney.SetValue(_betManager.BetParameterData.casinoBudget);
        returnToPlayer.SetValue(_betManager.BetParameterData.returnToPlayer);
        rewardPoolMoney.SetValue(_betManager.BetParameterData.rewardPoolMoney);
        targetSpin.SetValue(_betManager.BetParameterData.targetSpin);
        greenProbability.SetValue(_betManager.BetParameterData.greenProbability);
        redProbability.SetValue(_betManager.BetParameterData.redProbability);
        blackProbability.SetValue(_betManager.BetParameterData.blackProbability);
        playerMoney.SetValue(_gameManager.Money);
    }

    private void UpdateProbabilities(ColorType changedColor)
    {
        if (changedColor == ColorType.Number)
        {
            var targetColor = GameConstants.GetColorTypeFromNumber((int)targetSpin.GetValue);
            greenProbability.SetValue(targetColor == ColorType.Green ? 100 : 0);
            redProbability.SetValue(targetColor == ColorType.Red ? 100 : 0);
            blackProbability.SetValue(targetColor == ColorType.Black ? 100 : 0);
            return;
        }
        var remainingTotal = 100f;
        var newGreenProbability = greenProbability.GetValue;
        var newRedProbability = redProbability.GetValue;
        var newBlackProbability = blackProbability.GetValue;
        switch (changedColor)
        {
            case ColorType.Green:
                newGreenProbability = greenProbability.GetValue;
                remainingTotal -= newGreenProbability;
                AdjustOtherTwo(ref newRedProbability, ref newBlackProbability, remainingTotal);
                if(newGreenProbability is > 0 and < 100)
                    targetSpin.SetValue(-1);
                break;

            case ColorType.Red:
                newRedProbability = redProbability.GetValue;
                remainingTotal -= newRedProbability;
                AdjustOtherTwo(ref newGreenProbability, ref newBlackProbability, remainingTotal);
                if(newRedProbability is > 0 and < 100)
                    targetSpin.SetValue(-1);
                break;

            case ColorType.Black:
                newBlackProbability = blackProbability.GetValue;
                remainingTotal -= newBlackProbability;
                AdjustOtherTwo(ref newGreenProbability, ref newRedProbability, remainingTotal);
                if(newBlackProbability is > 0 and < 100)
                    targetSpin.SetValue(-1);
                break;
        }
        
        greenProbability.SetValue(newGreenProbability);
        redProbability.SetValue(newRedProbability);
        blackProbability.SetValue(newBlackProbability);
    }

    private void AdjustOtherTwo(ref float firstColor, ref float secondColor, float remainingTotal)
    {
        var oldTotal = firstColor + secondColor;
        if(oldTotal == 0) oldTotal = 1;
        firstColor = (firstColor / oldTotal) * remainingTotal;
        secondColor = (secondColor / oldTotal) * remainingTotal;
    }
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
    private void OnTargetSpinChanged(float oldValue, float newValue)
    {
        if(newValue < 0) return;
        UpdateProbabilities(ColorType.Number);
        _betManager.SetTargetSpin(newValue);
    }

    private void OnGreenProbabilityChanged(float oldValue, float newValue)
    {
        _betManager.SetGreenProbability(newValue);
        UpdateProbabilities(ColorType.Green);
    }

    private void OnRedProbabilityChanged(float oldValue, float newValue)
    {
        _betManager.SetRedProbability(newValue);
        UpdateProbabilities(ColorType.Red);
    }

    private void OnBlackProbabilityChanged(float oldValue, float newValue)
    {
        _betManager.SetBlackProbability(newValue);
        UpdateProbabilities(ColorType.Black);
    }

    private void SaveButtonClicked()
    {
        _betManager.SaveBetParameterData();
        _gameManager.SaveData();
    }
    
    private void CloseButtonClicked()
    {
        _betManager.SaveBetParameterData();
        _uiManager.CloseChestPanel();
    }
    
    private void DeleteSaveData() => _gameManager.DeleteSaveData();
    private void ResetBetParameter() => _betManager.ResetBetParameterData();
}
