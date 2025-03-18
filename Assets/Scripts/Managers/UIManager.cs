using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static TransformConstants;

public class UIManager : MonoBehaviour,IManager
{
    [SerializeField] private TextMeshProUGUI totalBetText,earnedText,totalMoneyText;
    [SerializeField] private TextMeshProUGUI winText,loseText;
    [SerializeField] private GameObject cheatPanel,cheatButton;
    [SerializeField] private Transform winPanel,losePanel;
    [SerializeField] private Transform chipLayout,topPanel,bottomPanel;
    [Inject] private GameManager _gameManager;
    [Inject] private TableManager _tableManager;
    [Inject] private AudioManager _audioManager;
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
        _gameManager.OnTurnCompleted += ShowResultScene;
    }
    private void OnDestroy()
    {
        _gameManager.OnGameStateChanged -= OnGameStateChanged;
        _gameManager.OnChipObjectsLoaded -= UpdateChipsUI;
        _gameManager.OnTotalBetChanged -= OnTotalBetChanged;
        _gameManager.OnEarnedMoneyChanged -= OnEarnedMoneyChanged;
        _gameManager.OnMoneyChanged -= OnMoneyChanged;
        _gameManager.OnTurnCompleted -= ShowResultScene;
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
        _audioManager.PlaySound(AssetConstants.UIButtonAudio);
        _gameManager.SetGameState(GameState.Betting);
        SelectedChipObject = chipObject;
        SelectedChipObject.Chip3DSetActive(true);
    }
    public void OnChipDrag(Vector3 targetPosition,Vector3 targetRotation)
    {
        SelectedChipObject?.SetPositionAndRotation(targetPosition,targetRotation);
    }
    public void OnChipDrop()
    {
        _audioManager.PlaySound(AssetConstants.ChipAudio);
        var oldChipBet = SelectedChipObject?.ChipBet;
        _gameManager.AddBet(oldChipBet);
        SelectedChipObject?.DisposeChip();
        _gameManager.SetGameState(GameState.Idle);
    }
    public void OnSpinButtonClicked()
    {
        if(_gameManager.GameState != GameState.Idle) return;
        _gameManager.StartSpin();
    }
    private void ShowResultScene(PlayerState playerState, int diffMoney)
    {
        switch (playerState)
        {
            case PlayerState.Pass:
                break;
            case PlayerState.Win:
                winPanel.gameObject.SetActive(true);
                winPanel.TweenPunch(1.25f,1,true);
                winText.text = $"+{diffMoney}";
                _audioManager.PlaySound(AssetConstants.WinAudio);
                break;
            case PlayerState.Lose:
                losePanel.gameObject.SetActive(true);
                losePanel.TweenPunch(1.25f,1,true);
                loseText.text = diffMoney.ToString();
                _audioManager.PlaySound(AssetConstants.LoseAudio);
                break;
        }
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
        cheatPanel.SetActive(gameState == GameState.Cheating);
        cheatButton.SetActive(gameState == GameState.Idle);
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
                topPanel.TweenMove(TopBarIdlePosition);
                bottomPanel.TweenMove(BottomBarIdlePosition);
                break;
        }
    }
    public void OpenCheatPanel()
    {
        _gameManager.SetGameState(GameState.Cheating);
    }
    public void CloseChestPanel()
    {
        _gameManager.SetGameState(GameState.Idle);
    }
    private void OnEarnedMoneyChanged(int oldValue,int value) => earnedText.TweenText(value.ToString());
    private void OnMoneyChanged(int oldValue,int value)
    {
        totalMoneyText.TweenText(value.ToString());
        UpdateChipsUI();
    }
    private void OnTotalBetChanged(int oldValue,int value) => totalBetText.TweenText(value.ToString());
}
