using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> OnAngleChanged;

    [SerializeField] private int currentAngle = 0;
    public int CurrentAngle 
    {
        get => currentAngle;
        private set
        {
            currentAngle = value;
            OnAngleChanged?.Invoke(currentAngle);
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
    }

    public void IncrementAngle()
    {
        CurrentAngle++;
    }

    public void DecrementAngle()
    {
        CurrentAngle--;
    }
}
