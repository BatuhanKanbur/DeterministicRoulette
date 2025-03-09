using System;
using UnityEngine;

public class CameraManager : MonoBehaviour,IManager
{
    [Inject] private GameManager _gameManager;
    public Camera MainCamera { private set; get; }
    [SerializeField] private Transform cameraIdlePoint,cameraBettingPoint,cameraSpinningPoint;
    private void Start()
    {
        MainCamera = Camera.main;
        DIContainer.AutoInject(this);
        _gameManager.OnGameStateChanged += OnGameStateChanged;
    }
    private void OnDisable()
    {
        _gameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Idle:
                MainCamera.transform.TweenMove(cameraIdlePoint.position);
                MainCamera.transform.TweenRotate(cameraIdlePoint.rotation.eulerAngles);
                break;
            case GameState.Betting:
                MainCamera.transform.TweenMove(cameraBettingPoint.position);
                MainCamera.transform.TweenRotate(cameraBettingPoint.rotation.eulerAngles);
                break;
            case GameState.Spinning:
                MainCamera.transform.TweenMove(cameraSpinningPoint.position);
                MainCamera.transform.TweenRotate(cameraSpinningPoint.rotation.eulerAngles);
                break;
            case GameState.Result:
                goto case GameState.Idle;
        }
    }
}
