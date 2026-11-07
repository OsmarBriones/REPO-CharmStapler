using System.Collections.Generic;

namespace CharmStapler.Domain
{
	/// <summary>
	/// Game-free rules of Fastener. Kept free of Unity and game types so they are unit tested.
	/// </summary>
	internal static class ReleaseRules
	{
		/// <summary>
		/// True when a forced release must be blocked: the grabber's owner carries the charm AND really holds an object.
		/// A release of an empty hand (or of a climb) is never blocked, so it never costs wear.
		/// </summary>
		internal static bool ShouldBlock(bool ownerCarriesCharm, bool ownerHoldsObject)
		{
			return ownerCarriesCharm && ownerHoldsObject;
		}

		/// <summary>True when the release asked for (-1 means "whatever is held") concerns the object that is held.</summary>
		internal static bool TargetsHeldObject(int releaseViewId, int heldViewId)
		{
			return heldViewId != -1 && (releaseViewId == -1 || releaseViewId == heldViewId);
		}
	}

	/// <summary>
	/// Marks the stretch of host code during which a forced release comes from a hit or a shove (the only releases the
	/// charm blocks). It is entered and left in matched pairs; a window is only valid in the frame it was entered, so a
	/// pair that was somehow never closed can not keep blocking releases of another origin in later frames.
	/// </summary>
	internal sealed class ReleaseWindow
	{
		private int depth;
		private int enterFrame;

		/// <summary>Current nesting depth (for tests).</summary>
		internal int Depth => depth;

		internal void Enter(int frame)
		{
			if (depth > 0 && frame != enterFrame)
			{
				depth = 0;
			}

			if (depth == 0)
			{
				enterFrame = frame;
			}

			depth++;
		}

		internal void Exit()
		{
			if (depth > 0)
			{
				depth--;
			}
		}

		internal bool IsActive(int frame)
		{
			return depth > 0 && frame == enterFrame;
		}
	}

	/// <summary>One wear per prevented release: a hit that keeps pushing for several ticks is billed once per cooldown.</summary>
	internal sealed class WearThrottle
	{
		private readonly Dictionary<int, float> lastCharge = new Dictionary<int, float>();

		/// <summary>True when this key may be charged now (and records it). A cooldown of 0 or less charges every time.</summary>
		internal bool TryCharge(int key, float now, float cooldownSeconds)
		{
			if (cooldownSeconds > 0f && lastCharge.TryGetValue(key, out float last) && now - last >= 0f && now - last < cooldownSeconds)
			{
				return false;
			}

			lastCharge[key] = now;
			return true;
		}

		internal void Clear()
		{
			lastCharge.Clear();
		}
	}
}
