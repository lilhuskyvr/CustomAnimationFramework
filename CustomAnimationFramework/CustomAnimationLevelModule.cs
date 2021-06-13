using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using ThunderRoad;
using UnityEngine;

namespace CustomAnimationFramework
{
    public class CustomAnimationLevelModule : LevelModule
    {
        private Harmony _harmony;

        public override IEnumerator OnLoadCoroutine(Level level)
        {
            try
            {
                _harmony = new Harmony("CustomAnimation");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                Debug.Log("Custom Animation Loaded");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;
            return base.OnLoadCoroutine(level);
        }

        private void EventManagerOnonCreatureSpawn(Creature creature)
        {
            creature.ragdoll.isGrabbed = false;
        }

        [HarmonyPatch(typeof(Equipment))]
        [HarmonyPatch("EquipWardrobe")]
        // ReSharper disable once UnusedType.Local
        private static class EquipmentEquipWardrobePatch
        {
            // ReSharper disable once InconsistentNaming
            private static IEnumerator ChangeAnimation(Equipment __instance, ContainerData.Content content)
            {
                var itemData = content.itemData;
                if (itemData == null)
                    yield break;

                var itemModuleAnimation = itemData.GetModule<ItemModuleAnimation>();

                if (itemModuleAnimation == null)
                    yield break;

                var creature = __instance.creature;

                Catalog.GetData<ItemData>(itemModuleAnimation.animationContainerItemId).SpawnAsync(item =>
                {
                    item.disallowDespawn = true;
                    AnimatorOverrideController animatorOverrideController =
                        new AnimatorOverrideController(creature.animator.runtimeAnimatorController);

                    var avatarAnimator = item.gameObject.GetComponent<Animator>();


                    foreach (var animation in itemModuleAnimation.animations)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(animation.Value))
                                continue;
                            var currentAnimation = animation;

                            animatorOverrideController[currentAnimation.Key] =
                                avatarAnimator.runtimeAnimatorController.animationClips.First(
                                    clip => clip.name == currentAnimation.Value);
                        }
                        catch (Exception exception)
                        {
                            Debug.Log(exception.Message);
                        }
                    }

                    creature.animator.runtimeAnimatorController = animatorOverrideController;
                });


                yield return null;
            }

            [HarmonyPostfix]
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedMember.Local
            private static void Postfix(Equipment __instance, ContainerData.Content content)
            {
                try
                {
                    GameManager.local.StartCoroutine(ChangeAnimation(__instance, content));
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                }
            }
        }
    }
}