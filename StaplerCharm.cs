using CharmCore.Api;

namespace CharmStapler
{
	/// <summary>Fastener: hits and shoves cannot make you drop what you hold.</summary>
	internal sealed class StaplerCharm : Charm
	{
		internal const string PrefabName = "Valuable Arctic Stapler";

		internal StaplerCharm()
			: base("Stapler", "Stapler - Fastener", defaultWearPercent: 10f, defaultCooldownSeconds: null, PrefabName)
		{
			// Extra host-synced numbers for this power go here, for example:
			// Amount = AddSetting("Amount", 30f, 1f, 100f, "Description for the config file.");
		}
	}
}
