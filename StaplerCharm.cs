using CharmCore.Api;
using CharmStapler.Domain;

namespace CharmStapler
{
	/// <summary>Fastener: hits and shoves cannot make you drop what you hold.</summary>
	internal sealed class StaplerCharm : Charm
	{
		internal const string PrefabName = "Valuable Arctic Stapler";

		internal StaplerCharm()
			: base("Stapler", "Stapler - Fastener", defaultWearPercent: 10f, defaultCooldownSeconds: null, PrefabName)
		{
			WearCooldownSeconds = AddSetting("WearCooldownSeconds", 0.5f, 0f, 5f, "After a hit or shove was stopped from making you drop something, further stopped drops within this many seconds cost no extra wear (so a hit that lasts several moments is charged once). 0 charges every stopped drop.");
			EnableBlocking = AddSetting("EnableBlocking", 1f, 0f, 1f, "1 lets the Stapler stop hits and shoves from making you drop what you hold, 0 switches that off (a safety switch). The host's choice applies to everyone.");
		}

		/// <summary>Per-player wear records of the current level; cleared when the level ends.</summary>
		internal WearThrottle Throttle { get; } = new WearThrottle();

		protected override void OnLevelEnded()
		{
			Throttle.Clear();
		}

		/// <summary>Seconds (0 to 5) during which further prevented releases of the same player cost no extra wear.</summary>
		internal CharmSetting WearCooldownSeconds { get; }

		/// <summary>1 = the block works, 0 = the charm does nothing.</summary>
		internal CharmSetting EnableBlocking { get; }

		internal bool BlockingEnabled => EnableBlocking.Value >= 0.5f;
	}
}
