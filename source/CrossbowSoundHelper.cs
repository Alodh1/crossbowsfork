using CombatOverhaul;
using CombatOverhaul.Animations;
using Vintagestory.API.Client;

namespace Crossbows;

internal static class CrossbowSoundHelper
{
    private const float ReleaseRange = 64f;
    private const float ReleaseVolume = 1.35f;
    private const float LatchReleaseVolume = 1.45f;
    private const float LockSpinningRange = 32f;
    private const float LockSpinningVolume = 0.8f;

    private static readonly string[] SimpleReleaseSounds =
    [
        "maltiezcrossbows:sounds/release/simple-0",
        "maltiezcrossbows:sounds/release/simple-1",
        "maltiezcrossbows:sounds/release/simple-2",
        "maltiezcrossbows:sounds/release/simple-3"
    ];

    private static readonly string[] StirrupReleaseSounds =
    [
        "maltiezcrossbows:sounds/release/stirrup-0",
        "maltiezcrossbows:sounds/release/stirrup-1",
        "maltiezcrossbows:sounds/release/stirrup-2",
        "maltiezcrossbows:sounds/release/stirrup-3",
        "maltiezcrossbows:sounds/release/stirrup-4",
        "maltiezcrossbows:sounds/release/stirrup-5",
        "maltiezcrossbows:sounds/release/stirrup-6"
    ];

    private static readonly string[] GoatsfootReleaseSounds =
    [
        "maltiezcrossbows:sounds/release/goatsfoot-0",
        "maltiezcrossbows:sounds/release/goatsfoot-1",
        "maltiezcrossbows:sounds/release/goatsfoot-2",
        "maltiezcrossbows:sounds/release/goatsfoot-3",
        "maltiezcrossbows:sounds/release/goatsfoot-4",
        "maltiezcrossbows:sounds/release/goatsfoot-5",
        "maltiezcrossbows:sounds/release/goatsfoot-6",
        "maltiezcrossbows:sounds/release/goatsfoot-7",
        "maltiezcrossbows:sounds/release/goatsfoot-8",
        "maltiezcrossbows:sounds/release/goatsfoot-9",
        "maltiezcrossbows:sounds/release/goatsfoot-10",
        "maltiezcrossbows:sounds/release/goatsfoot-11"
    ];

    private static readonly string[] WindlassReleaseSounds =
    [
        "maltiezcrossbows:sounds/release/windlass-0",
        "maltiezcrossbows:sounds/release/windlass-1",
        "maltiezcrossbows:sounds/release/windlass-2",
        "maltiezcrossbows:sounds/release/windlass-3",
        "maltiezcrossbows:sounds/release/windlass-4",
        "maltiezcrossbows:sounds/release/windlass-5",
        "maltiezcrossbows:sounds/release/windlass-6",
        "maltiezcrossbows:sounds/release/windlass-7",
        "maltiezcrossbows:sounds/release/windlass-8",
        "maltiezcrossbows:sounds/release/windlass-9",
        "maltiezcrossbows:sounds/release/windlass-10",
        "maltiezcrossbows:sounds/release/windlass-11"
    ];

    private static readonly string[] LockSpinningSounds =
    [
        "maltiezcrossbows:sounds/release/lock-spinning",
        "maltiezcrossbows:sounds/release/lock-spinning-2",
        "maltiezcrossbows:sounds/release/lock-spinning-3",
        "maltiezcrossbows:sounds/release/lock-spinning-4"
    ];

    public static void PlayRelease(ICoreClientAPI api, string animationCode)
    {
        (string[] codes, float volume, bool playLockSpinning) = GetReleaseSounds(animationCode);
        if (codes.Length == 0) return;

        SoundsSynchronizerClient? sounds = api.ModLoader.GetModSystem<CombatOverhaulSystem>().ClientSoundsSynchronizer;
        if (sounds == null) return;

        sounds.Play(new SoundFrame(codes, 0, randomizePitch: false, range: ReleaseRange, volume: volume, synchronize: true));
        if (playLockSpinning)
        {
            sounds.Play(new SoundFrame(LockSpinningSounds, 0, randomizePitch: false, range: LockSpinningRange, volume: LockSpinningVolume, synchronize: true));
        }
    }

    private static (string[] Codes, float Volume, bool PlayLockSpinning) GetReleaseSounds(string animationCode)
    {
        if (animationCode.Contains("latch", StringComparison.OrdinalIgnoreCase))
        {
            return (StirrupReleaseSounds, LatchReleaseVolume, false);
        }

        if (animationCode.Contains("stirrup", StringComparison.OrdinalIgnoreCase))
        {
            return (StirrupReleaseSounds, ReleaseVolume, true);
        }

        if (animationCode.Contains("goatsfoot", StringComparison.OrdinalIgnoreCase))
        {
            return (GoatsfootReleaseSounds, ReleaseVolume, true);
        }

        if (animationCode.Contains("windlass", StringComparison.OrdinalIgnoreCase))
        {
            return (WindlassReleaseSounds, ReleaseVolume, true);
        }

        return (SimpleReleaseSounds, ReleaseVolume, false);
    }
}
