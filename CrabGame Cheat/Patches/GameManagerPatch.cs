﻿using HarmonyLib;
using SteamworksNative;
using UnityEngine;

namespace JNNJMods.CrabGameCheat.Patches
{
    [HarmonyPatch(typeof(GameManager))]
    public static class GameManagerPatch
    {
        public static bool NoPush;

        [HarmonyPatch("PunchPlayer")]
        [HarmonyPrefix]
        public static bool PunchPlayer(ulong puncher, ulong punched, Vector3 dir)
        {
            return !(NoPush && punched == SteamUser.GetSteamID().m_SteamID);
        }

    }
}
