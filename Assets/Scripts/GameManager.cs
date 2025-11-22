using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action<int> OnAngleChanged;

        [SerializeField] private int _currentAngle;

        public int CurrentAngle
        {
            get => _currentAngle;
            private set
            {
                _currentAngle = value;

                switch (_currentAngle)
                {
                    case >= 360:
                        _currentAngle -= 360;
                        break;
                    case < 0:
                        _currentAngle += 360;
                        break;
                }

                OnAngleChanged?.Invoke(_currentAngle);
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
}
