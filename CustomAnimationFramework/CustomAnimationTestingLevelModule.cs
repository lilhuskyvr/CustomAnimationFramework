using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using IngameDebugConsole;
using ThunderRoad;
using UnityEngine;

namespace CustomAnimationFramework
{
    public class CustomAnimationTestingLevelModule : LevelModule
    {
        private Creature _creature;

        public override IEnumerator OnLoadCoroutine(Level level)
        {
            DebugLogConsole.AddCommandInstance("custom_animation_spawn_creature",
                "Custom Animation Spawn Creature", "SpawnCreature",
                this);
            return base.OnLoadCoroutine(level);
        }

        public override void Update(Level level)
        {
            if (_creature == null)
                return;

            if (_creature.state != Creature.State.Alive)
                return;

            if (Time.timeScale == 0)
                return;

            try
            {
                for (int i = 0; i < _creature.animator.layerCount; i++)
                {
                    foreach (var animatorClipInfo in _creature.animator.GetCurrentAnimatorClipInfo(i))
                    {
                        Debug.Log(animatorClipInfo.clip.name);
                    }
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public void SpawnCreature(string creatureId, int factionId, string containerId, string brainId)
        {
            var creatureData = Catalog.GetData<CreatureData>(creatureId);
            creatureData.containerID = containerId;
            creatureData.brainId = brainId;
            GameManager.local.StartCoroutine(creatureData.SpawnCoroutine(
                Player.local.creature.transform.position + Player.local.creature.transform.forward,
                Player.local.creature.transform.rotation,
                null,
                rsCreature =>
                {
                    rsCreature.SetFaction(factionId);
                    _creature = rsCreature;
                }
            ));
        }
    }
}