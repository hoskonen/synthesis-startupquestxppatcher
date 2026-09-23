using System.Collections.Immutable;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace StartupQuestXPPatcher;

public sealed record SupportedQuest(
    uint LocalFormId,
    string EditorId,
    string Name,
    Quest.TypeEnum ExpectedOriginalType)
{
    public FormKey GetFormKey(ModKey modKey) =>
        modKey.MakeFormKey(LocalFormId);
}

public sealed record SupportedPlugin(
    string DisplayName,
    ModKey ModKey,
    ImmutableArray<SupportedQuest> Quests);

public static class SupportedQuestCatalog
{
    public static readonly SupportedPlugin AlternateStart = new(
        "Alternate Start - Live Another Life",
        ModKey.FromFileName("Alternate Start - Live Another Life.esp"),
        [
            new SupportedQuest(
                0x00000DAF,
                "ARTHLALChargenQuest",
                "A Second Chance",
                Quest.TypeEnum.MainQuest),
            new SupportedQuest(
                0x0007A334,
                "ARTHLALRumorsOfWarQuest",
                "Live Another Life",
                Quest.TypeEnum.MainQuest),
        ]);

    public static readonly SupportedPlugin Frostfall = new(
        "Frostfall",
        ModKey.FromFileName("Frostfall.esp"),
        [
            new SupportedQuest(
                0x000177D7,
                "_Frost_TrackingQuest",
                "Frostfall",
                Quest.TypeEnum.SideQuest),
        ]);

    public static readonly SupportedPlugin SunHelm = new(
        "SunHelm",
        ModKey.FromFileName("SunHelmSurvival.esp"),
        [
            new SupportedQuest(
                0x0041E37C,
                "_SHStartPrompt",
                "SunHelm",
                Quest.TypeEnum.SideQuest),
        ]);

    public static readonly ImmutableArray<SupportedPlugin> Plugins =
        [AlternateStart, Frostfall, SunHelm];
}
