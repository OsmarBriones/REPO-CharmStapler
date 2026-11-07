using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmStapler.Tests;

/// <summary>
/// Tier 2: every game member the mod patches or reads still exists with the shape the code expects. The game's
/// assembly is read as plain metadata (nothing is loaded), so a game update that renames or removes something fails
/// here with a clear message instead of silently breaking the mod. Skipped when the game is not installed.
/// </summary>
[TestClass]
public class GameContractTests
{
	private static readonly Dictionary<string, TypeShape> Types = LoadTypes();

	private sealed class TypeShape
	{
		internal HashSet<string> Fields { get; } = new HashSet<string>();

		internal HashSet<string> Methods { get; } = new HashSet<string>();
	}

	// ---- Members we read or write ----

	[TestMethod]
	public void PhysGrabber_HasTheReleaseMembers()
	{
		// Patched (blockers) with their parameter counts: OverrideGrabRelease(int, float), ReleaseObject(int, float),
		// ReleaseObjectRPC(bool, float, int, int, PhotonMessageInfo). Fastener uses the positional argument __0 / __2.
		RequireMethod("PhysGrabber", "OverrideGrabRelease", 2);
		RequireMethod("PhysGrabber", "ReleaseObject", 2);
		RequireMethod("PhysGrabber", "ReleaseObjectRPC", 5);
	}

	[TestMethod]
	public void PhysGrabber_HasTheHoldingMembers()
	{
		RequireField("PhysGrabber", "playerAvatar");
		RequireField("PhysGrabber", "grabbed");
		RequireField("PhysGrabber", "grabbedPhysGrabObject");
		RequireField("PhysGrabber", "grabbedStaticGrabObject");
	}

	[TestMethod]
	public void HitAndShoveWindows_StillExist()
	{
		RequireMethod("PlayerTumble", "TumbleImpactRPC", 2);
		RequireMethod("HurtCollider", "PhysObjectHurt", 7);
		RequireField("HurtCollider", "physReleaseGrab");
		RequireMethod("HurtCollider", "ColliderCheck", 0);
	}

	[TestMethod]
	public void HurtCollider_ColliderCheckCoroutine_HasMoveNext()
	{
		// The coroutine class is compiler generated and patched by name prefix "<ColliderCheck>".
		RequireNestedMethod("HurtCollider", "<ColliderCheck>", "MoveNext");
	}

	// ---- Helpers ----

	private static void RequireMethod(string type, string method, int parameterCount)
	{
		TypeShape shape = RequireType(type);
		Assert.IsTrue(shape.Methods.Contains(method + "/" + parameterCount), $"{type}.{method}({parameterCount} parameters) was not found in the game assembly.");
	}

	private static void RequireNestedMethod(string parent, string namePrefix, string method)
	{
		RequireType(parent);
		bool found = Types.Any(pair => pair.Key.StartsWith(parent + "+" + namePrefix, StringComparison.Ordinal) && pair.Value.Methods.Contains(method + "/0"));
		Assert.IsTrue(found, $"No nested type {parent}+{namePrefix}* with {method}() was found in the game assembly.");
	}

	private static void RequireField(string type, string field)
	{
		TypeShape shape = RequireType(type);
		Assert.IsTrue(shape.Fields.Contains(field), $"{type}.{field} was not found in the game assembly.");
	}

	private static TypeShape RequireType(string type)
	{
		if (Types.Count == 0)
		{
			Assert.Inconclusive("The game assembly was not found. Install the game or set REPO_GAME_DIR to run the contract tests.");
		}

		Assert.IsTrue(Types.TryGetValue(type, out TypeShape? shape), $"Type {type} was not found in the game assembly.");
		return shape!;
	}

	private static Dictionary<string, TypeShape> LoadTypes()
	{
		Dictionary<string, TypeShape> types = new Dictionary<string, TypeShape>();
		string root = Environment.GetEnvironmentVariable("REPO_GAME_DIR") ?? @"C:\Program Files (x86)\Steam\steamapps\common\REPO";
		string path = Path.Combine(root, "REPO_Data", "Managed", "Assembly-CSharp.dll");
		if (!File.Exists(path))
		{
			return types;
		}

		using FileStream stream = File.OpenRead(path);
		using PEReader peReader = new PEReader(stream);
		MetadataReader reader = peReader.GetMetadataReader();
		foreach (TypeDefinitionHandle handle in reader.TypeDefinitions)
		{
			TypeDefinition definition = reader.GetTypeDefinition(handle);
			string key = reader.GetString(definition.Name);
			if (!definition.GetDeclaringType().IsNil)
			{
				TypeDefinition parent = reader.GetTypeDefinition(definition.GetDeclaringType());
				if (!parent.GetDeclaringType().IsNil || reader.GetString(parent.Namespace).Length != 0)
				{
					continue;
				}

				key = reader.GetString(parent.Name) + "+" + key;
			}
			else if (reader.GetString(definition.Namespace).Length != 0)
			{
				continue;
			}

			TypeShape shape = new TypeShape();
			foreach (FieldDefinitionHandle field in definition.GetFields())
			{
				shape.Fields.Add(reader.GetString(reader.GetFieldDefinition(field).Name));
			}

			foreach (MethodDefinitionHandle method in definition.GetMethods())
			{
				MethodDefinition methodDefinition = reader.GetMethodDefinition(method);
				int parameters = methodDefinition.GetParameters().Count(parameter => reader.GetParameter(parameter).SequenceNumber > 0);
				shape.Methods.Add(reader.GetString(methodDefinition.Name) + "/" + parameters);
			}

			types[key] = shape;
		}

		return types;
	}
}
