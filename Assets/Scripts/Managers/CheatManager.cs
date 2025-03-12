using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheatManager : MonoBehaviour,IManager
{
    [Inject] private GameManager _gameManager;
    [SerializeField] private Text totalMoneyText,targetNumberText;

    private void Start()
    {
        DIContainer.AutoInject(this);
    }

    public void SaveButtonClicked()
    {
        _gameManager.SaveData();
    }
    public void SetRandomNumber()
    {
        _gameManager.CheatNumber = -1;
    }
    public void AddMoney()
    {
        _gameManager.AddMoney(1000);
    }
    public void SubtractMoney()
    {
        if(_gameManager.Money > 2000)
            _gameManager.AddMoney(-1000);
    }
    public void AddTargetNumber()
    {
        _gameManager.CheatNumber++;
        if(_gameManager.CheatNumber > 36)
            _gameManager.CheatNumber = 0;
    }
    public void SubtractTargetNumber()
    {
        _gameManager.CheatNumber--;
        if(_gameManager.CheatNumber < 0)
            _gameManager.CheatNumber = 36;
    }
    public void CloseButtonClicked()
    {
        gameObject.SetActive(false);
    }
    public void DeleteSaveData()
    {
        _gameManager.DeleteSaveData();
    }
    private void Update()
    {
        totalMoneyText.text = _gameManager.Money.ToString();
        targetNumberText.text = _gameManager.CheatNumber.ToString();
    }
}
