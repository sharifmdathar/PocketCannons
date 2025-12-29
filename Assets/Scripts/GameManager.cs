using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Turn
    {
        Player1,
        Player2
    }

    public enum AttackType
    {
        SingleShot,
        TripleShot,
        TerrainDeform
    }

    public static GameManager Instance { get; private set; }

    public event Action<int> OnAngleChanged;
    public event Action<float> OnPowerChanged;
    public event Action OnFire;
    public event Action<float> OnMove;
    public event Action<Turn> OnTurnChanged;
    public event Action<Turn, float> OnHealthChanged;
    public event Action<Turn> OnGameOver;
    public event Action<AttackType> OnAttackTypeChanged;
    public event Action<float> OnWindChanged;

    [SerializeField] private int _p1Angle = 60;
    [SerializeField] private float _p1Power = 70f;
    [SerializeField] private float _p1Health = 100f;

    [SerializeField] private int _p2Angle = 120;
    [SerializeField] private float _p2Power = 70f;
    [SerializeField] private float _p2Health = 100f;

    [SerializeField] private AttackType _p1AttackType = AttackType.SingleShot;
    [SerializeField] private AttackType _p2AttackType = AttackType.SingleShot;

    [Header("Wind Settings")]
    [SerializeField] private float minWindStrength = -5f;
    [SerializeField] private float maxWindStrength = 5f;
    [SerializeField] private bool windChangesPerTurn = true;

    private float _currentWindStrength = 0f;

    public Turn CurrentTurn { get; private set; } = Turn.Player1;

    public float WindStrength => _currentWindStrength;

    public int CurrentAngle
    {
        get => CurrentTurn == Turn.Player1 ? _p1Angle : _p2Angle;
        private set
        {
            var clampedValue = (value % 360 + 360) % 360;
            if (CurrentTurn == Turn.Player1) _p1Angle = clampedValue;
            else _p2Angle = clampedValue;

            OnAngleChanged?.Invoke(clampedValue);
        }
    }

    public float CurrentPower
    {
        get => CurrentTurn == Turn.Player1 ? _p1Power : _p2Power;
        private set
        {
            var clampedValue = Mathf.Clamp(value, 0f, 100f);
            if (CurrentTurn == Turn.Player1) _p1Power = clampedValue;
            else _p2Power = clampedValue;

            OnPowerChanged?.Invoke(clampedValue);
        }
    }

    public AttackType CurrentAttackType
    {
        get => CurrentTurn == Turn.Player1 ? _p1AttackType : _p2AttackType;
        set
        {
            if (CurrentTurn == Turn.Player1) _p1AttackType = value;
            else _p2AttackType = value;

            OnAttackTypeChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        GenerateNewWind();
    }

    public void IncrementAngle()
    {
        CurrentAngle++;
    }

    public void DecrementAngle()
    {
        CurrentAngle--;
    }

    public void SetAngle(int angle)
    {
        CurrentAngle = angle;
    }

    public void SetPower(float power)
    {
        CurrentPower = power;
    }

    public void Move(float direction)
    {
        OnMove?.Invoke(direction);
    }

    public void Fire()
    {
        OnFire?.Invoke();
        SwitchTurn();
    }

    public float GetHealth(Turn player) => player == Turn.Player1 ? _p1Health : _p2Health;

    public void TakeDamage(Turn player, float damage)
    {
        if (player == Turn.Player1)
        {
            _p1Health = Mathf.Max(0, _p1Health - damage);
            OnHealthChanged?.Invoke(Turn.Player1, _p1Health);

            if (_p1Health <= 0)
            {
                OnGameOver?.Invoke(Turn.Player2);
            }
        }
        else
        {
            _p2Health = Mathf.Max(0, _p2Health - damage);
            OnHealthChanged?.Invoke(Turn.Player2, _p2Health);

            if (_p2Health <= 0)
            {
                OnGameOver?.Invoke(Turn.Player1);
            }
        }
    }

    private void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == Turn.Player1 ? Turn.Player2 : Turn.Player1;
        OnTurnChanged?.Invoke(CurrentTurn);

        if (windChangesPerTurn)
        {
            GenerateNewWind();
        }

        OnAngleChanged?.Invoke(CurrentAngle);
        OnPowerChanged?.Invoke(CurrentPower);
        OnAttackTypeChanged?.Invoke(CurrentAttackType);
    }

    private void GenerateNewWind()
    {
        _currentWindStrength = UnityEngine.Random.Range(minWindStrength, maxWindStrength);
        OnWindChanged?.Invoke(_currentWindStrength);
    }
}