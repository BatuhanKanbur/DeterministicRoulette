using System;
using UnityEngine;
using static TransformConstants;

public class ControllerManager : MonoBehaviour,IManager
{
    [Inject] private GameManager _gameManager;
    [Inject] private UIManager _uiManager;
    [Inject] private CameraManager _cameraManager;
    private Transform _lastBetArea;
    private void Start()
    {
        DIContainer.AutoInject(this);
    }

    private void Update()
    {
        var inputState = GetInputState();
        if(inputState == InputState.None) return;
        var clickPosition = GetPosition(inputState);
        var ray = _cameraManager.MainCamera.ScreenPointToRay(clickPosition);
        if (_gameManager.GameState == GameState.Betting)
        {
            switch (inputState)
            {
                case InputState.Press:
                    Vector3 targetPosition, targetRotation;
                    if(Physics.Raycast(ray, out var hit,10,BetAreaLayerMask))
                    {
                        targetPosition = hit.transform.position;
                        targetRotation = ChipHitRotation;
                        _uiManager.SetBetChipObject(hit.transform);
                    }
                    else
                    {
                        var worldPosition = _cameraManager.MainCamera.ScreenToWorldPoint(new Vector3(clickPosition.x,clickPosition.y,1));
                        targetPosition = worldPosition;
                        targetRotation = ChipIdleRotation;
                        _lastBetArea = null;
                        _uiManager.SetBetChipObject(null);
                    }
                    _uiManager.OnChipDrag(targetPosition,targetRotation);
                    break;
                case InputState.Release:
                    _uiManager.OnChipDrop();
                    break;
            }
        }
    }
    private InputState GetInputState()
    {
        return Input.GetMouseButton(0) ? InputState.Press : Input.GetMouseButtonUp(0) ? InputState.Release : InputState.None;
    }
    private Vector3 GetPosition(InputState inputState)
    {
        return inputState == InputState.Press ?  Input.mousePosition : Vector3.zero;
    }
}
