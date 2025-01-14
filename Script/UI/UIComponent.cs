using DG.Tweening;
using UnityEngine;

namespace pruss.Tool.UI
{
    internal abstract class UIComponent : MonoBehaviour
    {
        public abstract Tween Show();
        public abstract Tween Hide();
    }
}