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
    ModKey ModKey,
    ImmutableArray<SupportedQuest> Quests);

public static class SupportedQuestCatalog
{
    public static readonly SupportedPlugin AlternateStart = new(
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

    public static readonly ImmutableArray<SupportedPlugin> Plugins =
        [AlternateStart];
}
