using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static TransformConstants;

public class UIManager : MonoBehaviour,IManager
{
    [SerializeField] private TextMeshProUGUI totalBetText,earnedText,totalMoneyText;
    [SerializeField] private Transform chipLayout,topPanel,bottomPanel;
    [Inject] private GameManager _gameManager;
    [Inject] private TableManager _tableManager;
    private ChipObject[] _chipObjects;
    private ChipObject SelectedChipObject { get; set; }
    private void Start()
    {        
        DIContainer.AutoInject(this);
        _gameManager.OnGameStateChanged += OnGameStateChanged;
        _gameManager.OnChipObjectsLoaded += UpdateChipsUI;
        _gameManager.OnTotalBetChanged += OnTotalBetChanged;
        _gameManager.OnEarnedMoneyChanged += OnEarnedMoneyChanged;
        _gameManager.OnMoneyChanged += OnMoneyChanged;
    }
    private void OnDisable()
    {
        _gameManager.OnGameStateChanged -= OnGameStateChanged;
        _gameManager.OnChipObjectsLoaded -= UpdateChipsUI;
        _gameManager.OnTotalBetChanged -= OnTotalBetChanged;
        _gameManager.OnEarnedMoneyChanged -= OnEarnedMoneyChanged;
        _gameManager.OnMoneyChanged -= OnMoneyChanged;
    }
    private void UpdateChipsUI()
    {
        var currentMoney = _gameManager.Money;
        foreach (var chipObject in _gameManager.ChipObjects)
        {
            if (chipObject.chipPrice > currentMoney)
            {
                chipObject.Chip2DPrefab.gameObject.SetActive(false);
                chipObject.EventEntry.callback.RemoveListener(_ => OnChipClick(chipObject));
            }
            else if(!chipObject.Chip2DPrefab.activeSelf)
            {
                chipObject.Chip2DPrefab.gameObject.SetActive(true);
                chipObject.Chip2DPrefab.transform.SetParent(chipLayout);
                chipObject.EventEntry.callback.AddListener(_ => OnChipClick(chipObject));
            }
            currentMoney -= chipObject.chipPrice;
        }
    }
    private void OnChipClick(ChipObject chipObject)
    {
        if(_gameManager.GameState == GameState.Betting) return;
        _gameManager.GameState = GameState.Betting;
        SelectedChipObject = chipObject;
        SelectedChipObject.Chip3DSetActive(true);
    }
    public void OnChipDrag(Vector3 targetPosition,Vector3 targetRotation)
    {
        SelectedChipObject?.SetPositionAndRotation(targetPosition,targetRotation);
    }
    public void OnChipDrop()
    {
        var oldChipBet = SelectedChipObject?.ChipBet;
        _gameManager.AddBet(oldChipBet);
        SelectedChipObject?.DisposeChip();
        _gameManager.GameState = GameState.Idle;
    }

    public void OnSpinButtonClicked()
    {
        if(_gameManager.GameState != GameState.Idle) return;
        _gameManager.StartSpin();
    }
    public void SetBetChipObject(Transform hitPoint)
    {
        if(hitPoint)
            SelectedChipObject.ChipBet = _tableManager.GetBetFromTable(SelectedChipObject,hitPoint);
        else
            SelectedChipObject.DisposeBet();
    }
    private void OnGameStateChanged(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Idle:
                topPanel.TweenMove(TopBarIdlePosition);
                bottomPanel.TweenMove(BottomBarIdlePosition);
                break;
            case GameState.Betting:
                bottomPanel.TweenMove(BottomBarBettingPosition,0.25f);
                break;
            case GameState.Spinning:
                bottomPanel.TweenMove(BottomBarBettingPosition);
                break;
            case GameState.Result:
                // goto case GameState.Idle;
                break;
        }
    }
    private void OnEarnedMoneyChanged(int value) => earnedText.TweenText(value.ToString());
    private void OnMoneyChanged(int value)
    {
        totalMoneyText.TweenText(value.ToString());
        UpdateChipsUI();
    }
    private void OnTotalBetChanged(int value) => totalBetText.TweenText(value.ToString());
}
