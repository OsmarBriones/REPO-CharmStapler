using System;
using CharmCore.Api;
using CharmStapler.Domain;
using UnityEngine;

namespace CharmStapler
{
	/// <summary>
	/// Fastener: hits and shoves cannot make you drop what you hold.
	/// The game releases a grab through three methods of <c>PhysGrabber</c>, which many unrelated features also use
	/// (monsters, traps, the shop...). The patches in <c>Patches/</c> therefore only mark the stretch of host code where a
	/// hit, a knock-out or a shove asks for the release (<see cref="Window"/>); inside it, <see cref="ShouldBlock"/> decides.
	/// The decision is taken where the release is requested (the host, or singleplayer), using the pockets that Pockets
	/// tracks for every player, so it works for any player who carries the Stapler.
	/// </summary>
	internal static class Fastener
	{
		internal static readonly ReleaseWindow Window = new ReleaseWindow();

		private static bool errorReported;

		/// <summary>True when the release this grabber is about to undergo must be skipped. Wears the Stapler once per prevented release.</summary>
		internal static bool ShouldBlock(PhysGrabber grabber, int releaseViewId)
		{
			try
			{
				StaplerCharm charm = CharmStaplerPlugin.Charm;
				if (charm == null || grabber == null || !Window.IsActive(Time.frameCount) || !charm.BlockingEnabled)
				{
					return false;
				}

				PlayerAvatar avatar = grabber.playerAvatar;
				if (avatar == null || !HoldsTarget(grabber, releaseViewId))
				{
					return false;
				}

				ValuableObject? carried = CarriedStapler(charm, avatar);
				if (!ReleaseRules.ShouldBlock(carried != null, true))
				{
					return false;
				}

				if (charm.Throttle.TryCharge(avatar.GetInstanceID(), Time.time, charm.WearCooldownSeconds.Value))
				{
					charm.WearValuable(carried!, charm.WearPercent);
					if (charm.DebugMode)
					{
						charm.LogDebug($"Prevented a forced release for {avatar.name} (wear {charm.WearPercent:0.#}%)");
					}
				}
				else
				{
					if (charm.DebugMode)
					{
						charm.LogDebug($"Prevented a forced release for {avatar.name} (no wear, cooldown)");
					}
				}

				return true;
			}
			catch (Exception exception)
			{
				if (!errorReported)
				{
					errorReported = true;
					CharmStaplerPlugin.Logger.LogError($"Fastener failed and lets the release happen: {exception}");
				}

				return false;
			}
		}

		internal static void EnterWindow()
		{
			Window.Enter(Time.frameCount);
		}

		internal static void ExitWindow()
		{
			Window.Exit();
		}

		/// <summary>The same test the game's <c>ReleaseObject</c> does: something is grabbed and the release concerns it. A climb holds no object.</summary>
		private static bool HoldsTarget(PhysGrabber grabber, int releaseViewId)
		{
			if (!grabber.grabbed)
			{
				return false;
			}

			int held = -1;
			if (grabber.grabbedPhysGrabObject != null && grabber.grabbedPhysGrabObject.photonView != null)
			{
				held = grabber.grabbedPhysGrabObject.photonView.ViewID;
			}
			else if (grabber.grabbedStaticGrabObject != null && grabber.grabbedStaticGrabObject.photonView != null)
			{
				held = grabber.grabbedStaticGrabObject.photonView.ViewID;
			}

			return ReleaseRules.TargetsHeldObject(releaseViewId, held);
		}

		private static ValuableObject? CarriedStapler(StaplerCharm charm, PlayerAvatar avatar)
		{
			ValuableObject? found = null;
			CharmsApi.ForEachCarrier(charm, (carrier, valuable) =>
			{
				if (carrier == avatar)
				{
					found = valuable;
				}
			});
			return found;
		}
	}
}
