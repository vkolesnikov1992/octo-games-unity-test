using System;
using UnityEngine;

namespace OctoGamesTest.PopupSystem
{
    public abstract class PopupViewBase : MonoBehaviour
    {
        public event Action<PopupViewBase> Disabled;

        public event Action<PopupViewBase> Destroyed;

        public abstract Type ParametersType { get; }

        public GameObject GameObject => gameObject;

        public Transform Transform => transform;

        public virtual void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        protected virtual void OnDisable()
        {
            Disabled?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
            Disabled = null;
            Destroyed = null;
        }
    }

    public abstract class PopupViewBase<TParameters> : PopupViewBase
        where TParameters : PopupParameters
    {
        public sealed override Type ParametersType => typeof(TParameters);
    }
}
