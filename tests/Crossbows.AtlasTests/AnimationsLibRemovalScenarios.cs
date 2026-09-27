using System.Reflection;
using Atlas.XUnit;
using Vintagestory.API.Common;
using Xunit;

namespace Crossbows.AtlasTests;

[Trait("Category", "E2E")]
public sealed class AnimationsLibRemovalScenarios : AtlasScenarioBase
{
    private const string CrossbowsAssemblyName = "CrossbowsFork";
    private const string IdleAnimationsInterface = "CombatOverhaul.Animations.IHasIdleAnimations";
    private const string MoveAnimationsInterface = "CombatOverhaul.Animations.IHasMoveAnimations";
    private const BindingFlags DeclaredInstanceMethods = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    [AtlasScenario(TimeoutMs = 120000)]
    public async Task Crossbows_Should_Load_Without_AnimationsLib()
    {
        await World.Ticks(5);

        Assert.True(World.Api.ModLoader.IsModEnabled("overhaulliblegacycompat"));
        Assert.True(World.Api.ModLoader.IsModEnabled("crossbowsfork"));
        Assert.False(World.Api.ModLoader.IsModEnabled("animationslib"));

        Assert.Equal("Crossbows.CrossbowItem", RequireItem("maltiezcrossbows:crossbow-simple-wood").GetType().FullName);
        Assert.Equal("Crossbows.MagazineCrossbowItem", RequireItem("maltiezcrossbows:crossbow-repeating-copper").GetType().FullName);

        Assembly crossbows = RequireAssembly(CrossbowsAssemblyName);
        Assert.DoesNotContain(crossbows.GetReferencedAssemblies(), reference =>
            reference.Name?.Contains("animationslib", StringComparison.OrdinalIgnoreCase) == true);

        string stagedModsDirectory = Path.Combine(
            Path.GetDirectoryName(typeof(AnimationsLibRemovalScenarios).Assembly.Location)
                ?? throw new InvalidOperationException("Test assembly directory could not be resolved."),
            "mods");
        Assert.False(Directory.Exists(Path.Combine(stagedModsDirectory, "animationslib")));
        Assert.False(File.Exists(Path.Combine(stagedModsDirectory, "animationslib.zip")));
    }

    [AtlasScenario(TimeoutMs = 120000)]
    public async Task Crossbow_Items_Should_Use_Overhaullib_Animation_Contracts()
    {
        await World.Ticks(5);

        Assembly crossbows = RequireAssembly(CrossbowsAssemblyName);
        Type crossbowItem = RequireType(crossbows, "Crossbows.CrossbowItem");
        Type magazineCrossbowItem = RequireType(crossbows, "Crossbows.MagazineCrossbowItem");

        Assert.True(ImplementsInterface(crossbowItem, MoveAnimationsInterface));
        Assert.True(ImplementsInterface(crossbowItem, IdleAnimationsInterface));
        Assert.True(ImplementsInterface(magazineCrossbowItem, IdleAnimationsInterface));
        Assert.False(ImplementsInterface(magazineCrossbowItem, MoveAnimationsInterface));

        Assert.DoesNotContain(crossbowItem.GetMethods(DeclaredInstanceMethods), method => method.Name == "GetIdleAnimation");
        Assert.DoesNotContain(magazineCrossbowItem.GetMethods(DeclaredInstanceMethods), method => method.Name == "GetIdleAnimation");
    }

    private Item RequireItem(string code)
    {
        return World.Api.World.GetItem(new AssetLocation(code))
            ?? throw new InvalidOperationException($"{code} was not loaded.");
    }

    private static Assembly RequireAssembly(string assemblyName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == assemblyName)
            ?? throw new InvalidOperationException($"{assemblyName} was not loaded by Atlas.");
    }

    private static Type RequireType(Assembly assembly, string fullName)
    {
        return assembly.GetType(fullName, throwOnError: false)
            ?? throw new InvalidOperationException($"{fullName} was not found in {assembly.GetName().Name}.");
    }

    private static bool ImplementsInterface(Type type, string interfaceFullName)
    {
        return type.GetInterfaces().Any(candidate => candidate.FullName == interfaceFullName);
    }
}
