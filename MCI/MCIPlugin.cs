﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;
using Reactor;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace MCI
{
    [BepInAutoPlugin("dragonbreath.au.mci", "MCI", VersionString)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ReactorPlugin.Id, BepInDependency.DependencyFlags.SoftDependency)]
    public partial class MCIPlugin : BasePlugin
    {
        public const string VersionString = "0.0.6";
        internal static Version vVersion = new(VersionString);
        public Harmony Harmony { get; } = new(Id);

        public static MCIPlugin Singleton { get; private set; } = null;

        public static string RobotName { get; set; } = "Bot";

        public static bool Enabled { get; set; } = true;
        public static bool IKnowWhatImDoing { get; set; } = false;
        public override void Load()
        {
            if (Singleton != null) return;
            Singleton = this;

            Harmony.PatchAll();
            UpdateChecker.CheckForUpdate();

            ClassInjector.RegisterTypeInIl2Cpp<DebuggerBehaviour>();

            this.AddComponent<DebuggerBehaviour>();

            SubmergedCompatibility.Initialize();

            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
            {
                if (scene.name == "MainMenu")
                {
                    ModManager.Instance.ShowModStamp();
                }
            }));
        }

        internal static bool Persistence = true;
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class CountdownPatch
    {
        public static void Prefix(GameStartManager __instance)
        {
            __instance.countDownTimer = 0;
        }
    }
}
