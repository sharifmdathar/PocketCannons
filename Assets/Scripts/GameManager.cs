using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action<int> OnAngleChanged;
        public event Action<float> OnPowerChanged;

        [SerializeField] private int _currentAngle = 60;
        [SerializeField] private float _currentPower = 70f;

        public int CurrentAngle
        {
            get => _currentAngle;
            private set
            {
                _currentAngle = (value % 360 + 360) % 360;

                OnAngleChanged?.Invoke(_currentAngle);
            }
        }

        public float CurrentPower
        {
            get => _currentPower;
            private set
            {
                _currentPower = Mathf.Clamp(value, 0f, 100f);
                OnPowerChanged?.Invoke(_currentPower);
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

        public void SetAngle(int angle)
        {
            CurrentAngle = angle;
        }

        public void SetPower(float power)
        {
            CurrentPower = power;
        }
    }
}
