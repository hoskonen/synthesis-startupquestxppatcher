using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace StartupQuestXPPatcher;

public static class QuestTypePatcher
{
    public static void Run(
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
        TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(output);

        int supportedPluginsDetected = 0;
        int supportedPluginsSkipped = 0;
        int questsExamined = 0;
        int questsPatched = 0;
        int questsAlreadyNone = 0;
        int failures = 0;
        var resolvedQuests =
            new List<(SupportedQuest Metadata, IQuestGetter Winner)>();
        var pluginDetections = SupportedQuestCatalog.Plugins
            .Select(plugin => (
                Plugin: plugin,
                IsActive: state.LoadOrder.ListedOrder.Any(
                    listing =>
                        listing.ModKey == plugin.ModKey &&
                        listing.Mod is not null)))
            .ToArray();

        output.WriteLine("Detected supported mods:");
        foreach ((SupportedPlugin plugin, bool isActive) in pluginDetections)
        {
            output.WriteLine(
                $"  [{(isActive ? "✓" : " ")}] {plugin.DisplayName}");
        }
        output.WriteLine();

        foreach ((SupportedPlugin plugin, bool isActive) in pluginDetections)
        {
            if (!isActive)
            {
                supportedPluginsSkipped++;
                continue;
            }

            supportedPluginsDetected++;

            foreach (SupportedQuest quest in plugin.Quests)
            {
                questsExamined++;
                FormKey formKey = quest.GetFormKey(plugin.ModKey);

                if (!state.LinkCache.TryResolve<IQuestGetter>(
                        formKey,
                        out var winningQuest,
                        ResolveTarget.Winner))
                {
                    failures++;
                    output.WriteLine(
                        $"ERROR: Could not resolve QUST {formKey} from expected plugin " +
                        $"'{plugin.ModKey.FileName.String}' (local FormID {quest.LocalFormId:X8}, " +
                        $"diagnostic EditorID '{quest.EditorId}', name '{quest.Name}').");
                    continue;
                }

                if (winningQuest.IsDeleted)
                {
                    failures++;
                    output.WriteLine(
                        $"ERROR: Winning QUST {formKey} is deleted. Expected plugin " +
                        $"'{plugin.ModKey.FileName.String}' (local FormID {quest.LocalFormId:X8}, " +
                        $"diagnostic EditorID '{quest.EditorId}', name '{quest.Name}').");
                    continue;
                }

                resolvedQuests.Add((quest, winningQuest));
            }
        }

        if (failures != 0)
        {
            WriteSummary(
                output,
                supportedPluginsDetected,
                supportedPluginsSkipped,
                questsExamined,
                questsPatched,
                questsAlreadyNone,
                failures);
            throw new InvalidOperationException(
                "Startup Quest XP Patcher could not resolve every known quest for an active supported plugin. " +
                "No compatibility patch was produced.");
        }

        foreach ((SupportedQuest metadata, IQuestGetter winningQuest) in
                 resolvedQuests)
        {
            if (winningQuest.Type == Quest.TypeEnum.None)
            {
                questsAlreadyNone++;
                output.WriteLine(
                    $"Already correct {metadata.EditorId}: None");
                continue;
            }

            Quest.TypeEnum previousType = winningQuest.Type;
            Quest overrideQuest =
                state.PatchMod.Quests.GetOrAddAsOverride(winningQuest);
            overrideQuest.Type = Quest.TypeEnum.None;
            questsPatched++;
            output.WriteLine(
                $"Patched {metadata.EditorId}: " +
                $"{FormatQuestType(previousType)} -> None");
        }

        WriteSummary(
            output,
            supportedPluginsDetected,
            supportedPluginsSkipped,
            questsExamined,
            questsPatched,
            questsAlreadyNone,
            failures);
    }

    private static void WriteSummary(
        TextWriter output,
        int supportedPluginsDetected,
        int supportedPluginsSkipped,
        int questsExamined,
        int questsPatched,
        int questsAlreadyNone,
        int failures)
    {
        output.WriteLine($"""
            Startup Quest XP Patcher summary
            Supported plugins detected: {supportedPluginsDetected}
            Supported plugins skipped: {supportedPluginsSkipped}
            Quests examined: {questsExamined}
            Quests patched: {questsPatched}
            Quests already None: {questsAlreadyNone}
            Failures: {failures}
            """);
    }

    private static string FormatQuestType(Quest.TypeEnum type)
    {
        return type switch
        {
            Quest.TypeEnum.MainQuest => "Main Quest",
            Quest.TypeEnum.SideQuest => "Side Quest",
            _ => type.ToString(),
        };
    }
}
