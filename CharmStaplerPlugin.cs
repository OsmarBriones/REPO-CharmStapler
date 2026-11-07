using BepInEx;
using CharmCore.Api;
using CharmStapler.Patches;
using HarmonyLib;
using UnityEngine;

namespace CharmStapler
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("com.osmar.Pockets", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.osmar.CharmCore", BepInDependency.DependencyFlags.HardDependency)]
	public class CharmStaplerPlugin : BaseUnityPlugin
	{
		public const string PluginGuid = "com.osmar.CharmStapler";
		public const string PluginName = "CharmStapler";
		public const string PluginVersion = "1.0.0";

		internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; } = null!;

		/// <summary>The registered charm, for the patches.</summary>
		internal static StaplerCharm Charm { get; private set; } = null!;

		private void Awake()
		{
			Logger = base.Logger;

			// Prevent the plugin from being deleted
			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;

			// Registering binds Enabled, WearPercent, CooldownSeconds and the Debug section (DebugMode) in this mod's config file.
			Charm = new StaplerCharm();
			CharmsApi.Register(Charm, Config);

			try
			{
				Harmony harmony = new Harmony(PluginGuid);
				harmony.PatchAll();
				if (!HurtCollider_ColliderCheck_Patch.Apply(harmony))
				{
					Logger.LogError("Fastener: the hit-on-held-object hook was not applied (HurtCollider coroutine ColliderCheck not found). Those hits will still make you drop things.");
				}
			}
			catch (System.Exception exception)
			{
				Logger.LogError($"Fastener: applying the patches failed: {exception}");
			}

			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
		}
	}
}
