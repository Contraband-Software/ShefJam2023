using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cyan;

namespace Managers
{
    public class BuildingSystem : MonoBehaviour
    {
        [SerializeField, RequireNotNull] Tilemap blockMap;
        [SerializeField, RequireNotNull] Tilemap ghostMap;
    }
}