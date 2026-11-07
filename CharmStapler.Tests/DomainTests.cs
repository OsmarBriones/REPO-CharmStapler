using CharmStapler.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmStapler.Tests;

[TestClass]
public class DomainTests
{
	[TestMethod]
	public void ShouldBlock_NeedsCarrierAndHeldObject()
	{
		Assert.IsTrue(ReleaseRules.ShouldBlock(true, true));
		Assert.IsFalse(ReleaseRules.ShouldBlock(true, false));
		Assert.IsFalse(ReleaseRules.ShouldBlock(false, true));
		Assert.IsFalse(ReleaseRules.ShouldBlock(false, false));
	}

	[TestMethod]
	public void TargetsHeldObject_MatchesIdOrAny()
	{
		Assert.IsTrue(ReleaseRules.TargetsHeldObject(-1, 5));
		Assert.IsTrue(ReleaseRules.TargetsHeldObject(5, 5));
		Assert.IsFalse(ReleaseRules.TargetsHeldObject(6, 5));
		Assert.IsFalse(ReleaseRules.TargetsHeldObject(-1, -1));
		Assert.IsFalse(ReleaseRules.TargetsHeldObject(5, -1));
	}

	[TestMethod]
	public void Window_IsActiveOnlyInsideAndInTheSameFrame()
	{
		ReleaseWindow window = new ReleaseWindow();
		Assert.IsFalse(window.IsActive(1));
		window.Enter(1);
		Assert.IsTrue(window.IsActive(1));
		Assert.IsFalse(window.IsActive(2));
		window.Enter(1);
		window.Exit();
		Assert.IsTrue(window.IsActive(1));
		window.Exit();
		Assert.IsFalse(window.IsActive(1));
	}

	[TestMethod]
	public void Window_ExitNeverGoesNegative_AndLeakedWindowIsDropped()
	{
		ReleaseWindow window = new ReleaseWindow();
		window.Exit();
		Assert.AreEqual(0, window.Depth);
		window.Enter(1);
		window.Enter(7);
		Assert.AreEqual(1, window.Depth);
		Assert.IsTrue(window.IsActive(7));
	}

	[TestMethod]
	public void Throttle_ChargesOncePerCooldownPerKey()
	{
		WearThrottle throttle = new WearThrottle();
		Assert.IsTrue(throttle.TryCharge(1, 10f, 0.5f));
		Assert.IsFalse(throttle.TryCharge(1, 10.2f, 0.5f));
		Assert.IsTrue(throttle.TryCharge(2, 10.2f, 0.5f));
		Assert.IsTrue(throttle.TryCharge(1, 10.5f, 0.5f));
	}

	[TestMethod]
	public void Throttle_ZeroCooldownAlwaysCharges()
	{
		WearThrottle throttle = new WearThrottle();
		Assert.IsTrue(throttle.TryCharge(1, 1f, 0f));
		Assert.IsTrue(throttle.TryCharge(1, 1f, 0f));
	}
}
