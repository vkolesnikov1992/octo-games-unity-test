using System;
using UnityEngine;

namespace OctoGamesTest.RefactoringTask
{
    public sealed class Character : MonoBehaviour
    {
        [SerializeField]
        private float _value;

        public event Action ValueChanged;

        public float Value
        {
            get => _value;
            set
            {
                if (Mathf.Approximately(_value, value))
                {
                    return;
                }

                _value = value;
                ValueChanged?.Invoke();
            }
        }
    }
}
