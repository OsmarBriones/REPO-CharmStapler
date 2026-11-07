using BepInEx;
using CharmCore.Api;
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

		private void Awake()
		{
			Logger = base.Logger;

			// Prevent the plugin from being deleted
			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;

			// Registering binds Enabled, WearPercent, CooldownSeconds and the Debug section (DebugMode) in this mod's config file.
			StaplerCharm charm = new StaplerCharm();
			CharmsApi.Register(charm, Config);
			this.gameObject.AddComponent<Fastener>().Charm = charm;

			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
		}
	}
}
