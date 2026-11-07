using HarmonyLib;

namespace CharmStapler.Patches
{
	// Blockers. They do nothing unless Fastener's window is open (a hit, knock-out or shove is asking for the release).
	// Multiplayer: the host asks through OverrideGrabRelease (it never reaches ReleaseObject on its own machine).
	// Singleplayer: the hit calls ReleaseObject, the knock-out and the shove call ReleaseObjectRPC directly.

	[HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.OverrideGrabRelease))]
	internal static class PhysGrabber_OverrideGrabRelease_Patch
	{
		[HarmonyPrefix]
		private static bool Prefix(PhysGrabber __instance, int __0)
		{
			return !Fastener.ShouldBlock(__instance, __0);
		}
	}

	[HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.ReleaseObject))]
	internal static class PhysGrabber_ReleaseObject_Patch
	{
		[HarmonyPrefix]
		private static bool Prefix(PhysGrabber __instance, int __0)
		{
			return !Fastener.ShouldBlock(__instance, __0);
		}
	}

	[HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.ReleaseObjectRPC))]
	internal static class PhysGrabber_ReleaseObjectRPC_Patch
	{
		[HarmonyPrefix]
		private static bool Prefix(PhysGrabber __instance, int __2)
		{
			return !Fastener.ShouldBlock(__instance, __2);
		}
	}
}
