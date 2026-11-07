using System;
using System.Reflection;
using HarmonyLib;

namespace CharmStapler.Patches
{
	// Windows. Open while host code asks for a release because of a shove or a hit. They only mark the context.

	/// <summary>A shove: the player hit by a tumbling player lets go (PlayerTumble about :595-599).</summary>
	[HarmonyPatch(typeof(PlayerTumble), nameof(PlayerTumble.TumbleImpactRPC))]
	internal static class PlayerTumble_TumbleImpactRPC_Patch
	{
		[HarmonyPrefix]
		private static void Prefix() => Fastener.EnterWindow();

		[HarmonyFinalizer]
		private static void Finalizer() => Fastener.ExitWindow();
	}

	/// <summary>A knock-out: a hard hit on a held object (HurtCollider about :886-896).</summary>
	[HarmonyPatch(typeof(HurtCollider), "PhysObjectHurt")]
	internal static class HurtCollider_PhysObjectHurt_Patch
	{
		[HarmonyPrefix]
		private static void Prefix() => Fastener.EnterWindow();

		[HarmonyFinalizer]
		private static void Finalizer() => Fastener.ExitWindow();
	}

	/// <summary>
	/// A hit on a held object with <c>physReleaseGrab</c> (HurtCollider about :509-519). That code sits in the coroutine
	/// <c>ColliderCheck</c>, so the window is placed around its compiler-generated <c>MoveNext</c> (the part of the loop
	/// that releases contains no yield). Patched by hand so a game update that renames the coroutine only disables this
	/// one hook and logs an error instead of failing the whole patch run.
	/// </summary>
	internal static class HurtCollider_ColliderCheck_Patch
	{
		internal static bool Apply(Harmony harmony)
		{
			Type? coroutine = AccessTools.FirstInner(typeof(HurtCollider), type => type.Name.StartsWith("<ColliderCheck>", StringComparison.Ordinal));
			MethodInfo? moveNext = coroutine != null ? AccessTools.Method(coroutine, "MoveNext") : null;
			if (moveNext == null)
			{
				return false;
			}

			const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
			harmony.Patch(
				moveNext,
				prefix: new HarmonyMethod(typeof(HurtCollider_ColliderCheck_Patch).GetMethod(nameof(Prefix), flags)),
				finalizer: new HarmonyMethod(typeof(HurtCollider_ColliderCheck_Patch).GetMethod(nameof(Finalizer), flags)));
			return true;
		}

		private static void Prefix() => Fastener.EnterWindow();

		private static void Finalizer() => Fastener.ExitWindow();
	}
}
