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
        int questsExamined = 0;
        int questsPatched = 0;
        int questsAlreadyNone = 0;
        int failures = 0;
        var resolvedQuests = new List<IQuestGetter>();

        foreach (SupportedPlugin plugin in SupportedQuestCatalog.Plugins)
        {
            bool isActive = state.LoadOrder.ListedOrder.Any(
                listing => listing.ModKey == plugin.ModKey && listing.Mod is not null);

            if (!isActive)
            {
                output.WriteLine(
                    $"{plugin.ModKey.FileName.String} was not detected; no quest overrides were created.");
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

                resolvedQuests.Add(winningQuest);
            }
        }

        if (failures != 0)
        {
            WriteSummary(
                output,
                supportedPluginsDetected,
                questsExamined,
                questsPatched,
                questsAlreadyNone,
                failures);
            throw new InvalidOperationException(
                "Startup Quest XP Patcher could not resolve every known quest for an active supported plugin. " +
                "No compatibility patch was produced.");
        }

        foreach (IQuestGetter winningQuest in resolvedQuests)
        {
            if (winningQuest.Type == Quest.TypeEnum.None)
            {
                questsAlreadyNone++;
                continue;
            }

            Quest overrideQuest =
                state.PatchMod.Quests.GetOrAddAsOverride(winningQuest);
            overrideQuest.Type = Quest.TypeEnum.None;
            questsPatched++;
        }

        WriteSummary(
            output,
            supportedPluginsDetected,
            questsExamined,
            questsPatched,
            questsAlreadyNone,
            failures);
    }

    private static void WriteSummary(
        TextWriter output,
        int supportedPluginsDetected,
        int questsExamined,
        int questsPatched,
        int questsAlreadyNone,
        int failures)
    {
        output.WriteLine($"""
            Startup Quest XP Patcher summary
            Supported plugins detected: {supportedPluginsDetected}
            Quests examined: {questsExamined}
            Quests patched: {questsPatched}
            Quests already None: {questsAlreadyNone}
            Failures: {failures}
            """);
    }
}
