using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace CustomAnimationFramework
{
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemModuleAnimation : ItemModule
    {
        public string animationContainerItemId;
        public Dictionary<string, string> animations = new Dictionary<string, string>();
    }
}