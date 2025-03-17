using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TableManager : MonoBehaviour,IManager
{
    [Inject] private GameManager _gameManager;
    [Inject] private AudioManager _audioManager;
    [SerializeField] private Transform[] wheelPoints;
    [SerializeField] private Transform betArea,wheel,ballParent;
    [SerializeField] private Rigidbody ball;
    [SerializeField] private TextMeshPro currentBetsText;
    private bool _isSpinning,_isSlowingDown;
    private float _currentSpeed,_elapsedTime;
    [SerializeField] private Transform[] historySpawnPoints;
    private List<GameObject> _oldNumberList = new List<GameObject>();
    public InsideBetArea[] InsideBetAreas;
    public OutsideBetArea[] OutsideBetAreas;
    private Vector3 _ballTargetPosition;

    private void Start()
    {
        DIContainer.AutoInject(this);
        _gameManager.OnGameStateChanged += OnGameStateChanged;
        _gameManager.OnBetsChanged += OnBetsChanged;
        _gameManager.OnOldNumbersChanged += UpdateHistory;
        GameConstants.BallStartRotation = ball.transform.localEulerAngles;
    }

    private void OnBetsChanged(List<Bet> betList)
    {
        var newFrameText = "";
        foreach (var bet in betList)
            newFrameText += GameConstants.GetBetFrameText(bet.BetNumbers, bet.BetMultiplier);
        currentBetsText.text = newFrameText;
    }

    private void OnDestroy()
    {
        _gameManager.OnGameStateChanged -= OnGameStateChanged;
        _gameManager.OnBetsChanged -= OnBetsChanged;
        _gameManager.OnOldNumbersChanged -= UpdateHistory;
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
    private async void UpdateHistory(List<int> historyList)
    {
        if(historyList.Count < _oldNumberList.Count)
        {
            foreach (var oldNumberObject in _oldNumberList)
                oldNumberObject.SetActive(false);
            _oldNumberList.Clear();
        }
        for (var i = 0; i < historyList.Count; i++)
        {
            if (i <= _oldNumberList.Count - 1) continue;
            var newHistoryBall = await ObjectManager.GetObject(AssetConstants.BallHistoryPrefab, historySpawnPoints[i].position);
            newHistoryBall.GetComponent<HistoryBall>().Init(historyList[i]);
            _oldNumberList.Add(newHistoryBall.gameObject);
        }
    }
    
    private IEnumerator SlowDownWheel()
    {
        StartCoroutine(JumpBall());
        var startSpeed = _currentSpeed;
        var time = 0f;
        var wheelOldRotation = wheel.rotation;
        var totalRotationOffset = startSpeed * GameConstants.SpinSlowdownDuration * GameConstants.SpinRotationMultiplier;
        var finalWheelRotation = wheel.rotation * Quaternion.Euler(0, 0, -totalRotationOffset);
        wheel.rotation = finalWheelRotation;
        _ballTargetPosition = wheelPoints[_gameManager.SpinResult].position;
        wheel.rotation = wheelOldRotation;
        while (time < GameConstants.SpinSlowdownDuration)
        {
            _currentSpeed = Mathf.Lerp(startSpeed, 0, time / GameConstants.SpinSlowdownDuration);
            wheel.Rotate(0, 0, -_currentSpeed * Time.deltaTime);
            ballParent.Rotate(0, _currentSpeed * Time.deltaTime, 0);
            time += Time.deltaTime;
            yield return null;
        }
        _currentSpeed = 0;
    }

    private IEnumerator JumpBall()
    {
        ball.isKinematic = false;
        ball.AddForce(transform.forward * GameConstants.BallSpeed);
        yield return new WaitForSeconds(1f);
        ball.isKinematic = true;
        StartCoroutine(SnapBall());
    }

    private IEnumerator SnapBall()
    {
        ball.isKinematic = true;
        var startPos = ball.transform.position;
        var startPosXZ = new Vector3(startPos.x, 0f, startPos.z);
        var endPosXZ = new Vector3(_ballTargetPosition.x, 0f, _ballTargetPosition.z);
        var arcCenter = new Vector3(wheel.position.x, 0f, wheel.position.z);
        var startRelative = startPosXZ - arcCenter;
        var endRelative = endPosXZ - arcCenter;
        var totalAngle = Mathf.Abs(Vector3.SignedAngle(startRelative, endRelative, Vector3.up));
        var numberOfJumps = (int)totalAngle.RemapClamped(GameConstants.BallNumberOfJumps);
        var jumpDuration = totalAngle.RemapClamped(GameConstants.BallJumpDuration);
        var heightFactor = totalAngle.RemapClamped(GameConstants.BallHeightFactor);
        for (var i = 0; i < numberOfJumps; i++)
        {
            var elapsedTime = 0f;
            var baseYStart = Mathf.Lerp(startPos.y, _ballTargetPosition.y, (float) i / numberOfJumps);
            var baseYEnd = Mathf.Lerp(startPos.y, _ballTargetPosition.y, (float) (i + 1) / numberOfJumps);
            _audioManager.PlaySound(AssetConstants.BallBounceAudio);
            if (i == numberOfJumps - 1)
            {
                var segmentStartAngle = Mathf.Abs(Mathf.Lerp(0, totalAngle, (float) i / numberOfJumps));
                var segmentStartPos = arcCenter + Quaternion.AngleAxis(segmentStartAngle, Vector3.up) * startRelative;
                while (elapsedTime < jumpDuration)
                {
                    elapsedTime += Time.deltaTime;
                    var t = elapsedTime / jumpDuration;
                    var currentHorizontal =
                        arcCenter + Vector3.Slerp(segmentStartPos - arcCenter, endPosXZ - arcCenter, t);
                    var yOffset = Mathf.Sin(Mathf.PI * t) * heightFactor;
                    var baseY = Mathf.Lerp(baseYStart, baseYEnd, t);
                    ball.transform.position = new Vector3(currentHorizontal.x, baseY + yOffset, currentHorizontal.z);
                    yield return null;
                }

                ball.transform.position = _ballTargetPosition;
            }
            else
            {
                _audioManager.PlaySound(AssetConstants.BallBounceAudio);
                var segmentStartAngle = Mathf.Lerp(0, totalAngle, (float) i / numberOfJumps);
                var segmentEndAngle = Mathf.Lerp(0, totalAngle, (float) (i + 1) / numberOfJumps);
                var segmentStartPos = arcCenter + Quaternion.AngleAxis(segmentStartAngle, Vector3.up) * startRelative;
                var segmentEndPos = arcCenter + Quaternion.AngleAxis(segmentEndAngle, Vector3.up) * startRelative;
                while (elapsedTime < jumpDuration)
                {
                    elapsedTime += Time.deltaTime;
                    var t = elapsedTime / jumpDuration;
                    var currentAngle = Mathf.Lerp(segmentStartAngle, segmentEndAngle, t);
                    var currentHorizontal = arcCenter + Quaternion.AngleAxis(currentAngle, Vector3.up) * startRelative;
                    var yOffset = Mathf.Sin(Mathf.PI * t) * heightFactor * (1f - (float) i / numberOfJumps);
                    var baseY = Mathf.Lerp(baseYStart, baseYEnd, t);
                    ball.transform.position = new Vector3(currentHorizontal.x, baseY + yOffset, currentHorizontal.z);
                    yield return null;
                }
                ball.transform.position = new Vector3(segmentEndPos.x, baseYEnd, segmentEndPos.z);
            }
        }
        _audioManager.StopSound();
        yield return new WaitForSeconds(2);
        _gameManager.SetGameState(GameState.Result);
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
                ball.transform.localEulerAngles = GameConstants.BallStartRotation;
                break;
            case GameState.Betting:
                break;
            case GameState.Spinning:
                _currentSpeed = Random.Range(250, 500);
                break;
            case GameState.Result:
                break;
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
