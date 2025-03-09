using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableManager : MonoBehaviour,IManager
{
    [Inject] private GameManager _gameManager;
    [SerializeField] private Transform[] wheelPoints;
    [SerializeField] private Transform betArea,wheel,ballParent;
    [SerializeField] private Rigidbody ball;
    private bool _isSpinning,_isSlowingDown;
    private float _currentSpeed,_elapsedTime;
    public InsideBetArea[] InsideBetAreas;
    public OutsideBetArea[] OutsideBetAreas;

    private void Start()
    {
        DIContainer.AutoInject(this);
        _gameManager.OnGameStateChanged += OnGameStateChanged;
    }
    private void OnDisable()
    {
        _gameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        if(!_isSpinning) return;
        if (_elapsedTime < GameConstants.SpinDuration)
        {
            wheel.Rotate(0, 0, -_currentSpeed * Time.deltaTime);
            ballParent.Rotate(0,  _currentSpeed * Time.deltaTime,0);
            _elapsedTime += Time.deltaTime;
        }
        else if (!_isSlowingDown)
        {
            _isSlowingDown = true;
            StartCoroutine(SlowDownWheel());
        }
    }
    private IEnumerator SlowDownWheel()
    {
        var startSpeed = _currentSpeed;
        var time = 0f;
        StartCoroutine(SnapBall());
        while (time < GameConstants.SpinSlowdownDuration)
        {
            _currentSpeed = Mathf.Lerp(startSpeed, 0, time / GameConstants.SpinSlowdownDuration);
            wheel.Rotate(0, 0, -_currentSpeed * Time.deltaTime);
            ballParent.Rotate(0,  _currentSpeed * Time.deltaTime,0);
            time += Time.deltaTime;
            yield return null;
        }
        _currentSpeed = 0;
    }
    private IEnumerator SnapBall()
    {
        ball.isKinematic = false;
        ball.AddForce(Vector3.up * 15);
        yield return new WaitForSeconds(1);
        var ballDistance = Vector3.Distance(ball.position, wheelPoints[_gameManager.SpinResult].position);
        while (ballDistance > 0.01f)
        {
            ball.position = Vector3.MoveTowards(ball.position, wheelPoints[_gameManager.SpinResult].position, Time.deltaTime * GameConstants.BallSnapSpeed);
            ballDistance = Vector3.Distance(ball.position, wheelPoints[_gameManager.SpinResult].position);
            yield return null;
        }
        ball.isKinematic = true;
        _gameManager.GameState = GameState.Result;
    }
    private void OnGameStateChanged(GameState gameState)
    {
        betArea.gameObject.SetActive(gameState == GameState.Betting);
        _isSpinning = gameState == GameState.Spinning;
        switch (gameState)
        {
            case GameState.Idle:
                _elapsedTime = 0;
                _isSlowingDown = false;
                _currentSpeed = 0;
                ball.transform.localPosition = GameConstants.BallStartPosition;
                break;
            case GameState.Betting:
                break;
            case GameState.Spinning:
                _currentSpeed = UnityEngine.Random.Range(250, 500);
                break;
            case GameState.Result:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    public Bet GetBetFromTable(ChipObject chipObject, Transform betPoint)
    {
        var newBetNumbers = new HashSet<int>();
        var betMultiplier = 0;
        ProcessBetAreas(InsideBetAreas, betPoint, newBetNumbers, ref betMultiplier);
        ProcessBetAreas(OutsideBetAreas, betPoint, newBetNumbers, ref betMultiplier);
        return new Bet(chipObject, newBetNumbers.ToArray(), betMultiplier);
    }
    private void ProcessBetAreas(IEnumerable<IBetArea> betAreas, Transform betPoint, HashSet<int> betNumbers, ref int multiplier)
    {
        foreach (var currentBetArea in betAreas)
        {
            currentBetArea.CheckBet(betPoint, out var isBetValid, out var areaMultiplier, out var numbers);
            if (!isBetValid) continue;
            foreach (var num in numbers)
                betNumbers.Add(num);
            multiplier = areaMultiplier;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (var i = 0; i < wheelPoints.Length; i++)
        {
            var wheel = wheelPoints[i];
            if (wheel == null) continue;
            Gizmos.color = GetRouletteColor(i);
            Gizmos.DrawSphere(wheel.position, 0.05f);
        }
    }
    private Color GetRouletteColor(int number)
    {
        if (number == 0) return Color.green;
        return GameConstants.RedNumbers.Contains(number) ? Color.red : Color.black;
    }
    private void OnValidate()
    {
        for (int i = 0; i < wheelPoints.Length; i++)
        {
            wheelPoints[i].gameObject.name = i + "_Point";
        }
    }
#endif
}
