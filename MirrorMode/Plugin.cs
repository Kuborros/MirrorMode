using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace MirrorMode
{
    [BepInPlugin("com.kuborro.plugins.fp2.mirrormode", "MirrorMode", "1.0.0")]
    [BepInProcess("FP2.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> configReverseControlls;
        private void Awake()
        {

            configReverseControlls = Config.Bind("General", "Enabled", false, "Enable mirrored controlls");

            var harmony = new Harmony("com.kuborro.plugins.fp2.mirrormode");
            harmony.PatchAll(typeof(PatchMerchant));
            harmony.PatchAll(typeof(PatchPowerupSprite));

            if (!configReverseControlls.Value)
            {
                harmony.PatchAll(typeof(PatchControlls));
                harmony.PatchAll(typeof(PatchControllsRewired));
            }
        }
    }

    class PatchMerchant
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPHubNPC), "Start", MethodType.Normal)]
        static void Postfix(string ___NPCName,ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts, ref int[] ___starCardRequirements, ref FPMusicTrack[] ___musicID)
        {
            if (___NPCName == "Chloe")
            {
                if (___itemCosts != null && ___itemsForSale != null)
                {
                    if (!___itemsForSale.Contains(FPPowerup.MIRROR_LENS)) {
                        ___itemsForSale = ___itemsForSale.AddToArray(FPPowerup.MIRROR_LENS);
                        ___itemCosts = ___itemCosts.AddToArray(2);
                        ___starCardRequirements = ___starCardRequirements.AddToArray(2);
                        ___musicID = ___musicID.AddToArray(FPMusicTrack.NONE);
                    }
                }
            }
        }
    }
    class PatchPowerupSprite
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudDigit), "SetDigitValue", MethodType.Normal)]
        static void PreFix(ref Sprite[] ___digitFrames)
        {
            if (___digitFrames != null && ___digitFrames.Length > 40)
            {
                if (___digitFrames[18] == null)
                {
                    ___digitFrames[18] = ___digitFrames[30];
                }
            }
        }
    }
    class PatchControlls
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessInputControl", MethodType.Normal)]
        static bool Prefix(FPPlayer __instance)
        {
            float axis = InputControl.GetAxis(Controls.axes.horizontal, false);
            float axis2 = InputControl.GetAxis(Controls.axes.vertical, false);
            __instance.input.upPress = false;
            __instance.input.downPress = false;
            __instance.input.leftPress = false;
            __instance.input.rightPress = false;
            if (__instance.IsPowerupActive(FPPowerup.MIRROR_LENS))
            {
                if (axis > InputControl.joystickThreshold)
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (axis < -InputControl.joystickThreshold)
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            else
            {
                if (axis > InputControl.joystickThreshold)
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (axis < -InputControl.joystickThreshold)
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            if (axis2 > InputControl.joystickThreshold)
            {
                if (!__instance.input.up)
                {
                    __instance.input.upPress = true;
                }
                __instance.input.up = true;
            }
            else
            {
                __instance.input.up = false;
            }
            if (axis2 < -InputControl.joystickThreshold)
            {
                if (!__instance.input.down)
                {
                    __instance.input.downPress = true;
                }
                __instance.input.down = true;
            }
            else
            {
                __instance.input.down = false;
            }
            __instance.input.jumpPress = InputControl.GetButtonDown(Controls.buttons.jump, false);
            __instance.input.jumpHold = InputControl.GetButton(Controls.buttons.jump, false);
            __instance.input.attackPress = InputControl.GetButtonDown(Controls.buttons.attack, false);
            __instance.input.attackHold = InputControl.GetButton(Controls.buttons.attack, false);
            __instance.input.specialPress = InputControl.GetButtonDown(Controls.buttons.special, false);
            __instance.input.specialHold = InputControl.GetButton(Controls.buttons.special, false);
            __instance.input.guardPress = InputControl.GetButtonDown(Controls.buttons.guard, false);
            __instance.input.guardHold = InputControl.GetButton(Controls.buttons.guard, false);
            __instance.input.confirm = (__instance.input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause, false));
            __instance.input.cancel = (__instance.input.attackPress | Input.GetKey(KeyCode.Escape));

            return false;
        }
    }

    class PatchControllsRewired
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), nameof(FPPlayer.ProcessRewired), MethodType.Normal)]

        static bool Prefix(FPPlayer __instance)
        {
            __instance.input.upPress = false;
            __instance.input.downPress = false;
            __instance.input.leftPress = false;
            __instance.input.rightPress = false;
            if (__instance.IsPowerupActive(FPPowerup.MIRROR_LENS))
            {
                if (FPPlayer.rewiredPlayerInput.GetButton("Left"))
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (FPPlayer.rewiredPlayerInput.GetButton("Right"))
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            else
            {
                if (FPPlayer.rewiredPlayerInput.GetButton("Right"))
                {
                    if (!__instance.input.right)
                    {
                        __instance.input.rightPress = true;
                    }
                    __instance.input.right = true;
                }
                else
                {
                    __instance.input.right = false;
                }
                if (FPPlayer.rewiredPlayerInput.GetButton("Left"))
                {
                    if (!__instance.input.left)
                    {
                        __instance.input.leftPress = true;
                    }
                    __instance.input.left = true;
                }
                else
                {
                    __instance.input.left = false;
                }
            }
            if (FPPlayer.rewiredPlayerInput.GetButton("Up"))
            {
                if (!__instance.input.up)
                {
                    __instance.input.upPress = true;
                }
                __instance.input.up = true;
            }
            else
            {
                __instance.input.up = false;
            }
            if (FPPlayer.rewiredPlayerInput.GetButton("Down"))
            {
                if (!__instance.input.down)
                {
                    __instance.input.downPress = true;
                }
                __instance.input.down = true;
            }
            else
            {
                __instance.input.down = false;
            }
            __instance.input.jumpPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Jump");
            __instance.input.jumpHold = FPPlayer.rewiredPlayerInput.GetButton("Jump");
            __instance.input.attackPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Attack");
            __instance.input.attackHold = FPPlayer.rewiredPlayerInput.GetButton("Attack");
            __instance.input.specialPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Special");
            __instance.input.specialHold = FPPlayer.rewiredPlayerInput.GetButton("Special");
            __instance.input.guardPress = FPPlayer.rewiredPlayerInput.GetButtonDown("Guard");
            __instance.input.guardHold = FPPlayer.rewiredPlayerInput.GetButton("Guard");
            __instance.input.confirm = (__instance.input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause, false));
            __instance.input.cancel = (__instance.input.attackPress | Input.GetKey(KeyCode.Escape));

            return false;
        }
    }

}
