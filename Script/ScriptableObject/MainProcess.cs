using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class MainProcess : ScriptableObject
{
    [Required] [SerializeReference] public List<ProcessData> processData;
}