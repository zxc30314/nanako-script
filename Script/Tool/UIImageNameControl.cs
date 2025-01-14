using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIImageNameControl : MonoBehaviour
{
    [Button]
    private void ChangeGameObjectNameToSpriteFileName()
    {
        if (TryGetComponent<Image>(out var component))
        {
            gameObject.name = component.sprite.name;
        }
    }
}