using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;

namespace StartupQuestXPPatcher;

public static class Program
{
    public static Task<int> Main(string[] args)
    {
        return SynthesisPipeline.Instance
            .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
            .SetTypicalOpen(GameRelease.SkyrimSE, "StartupQuestXPPatcher.esp")
            .Run(args);
    }

    public static void RunPatch(
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        QuestTypePatcher.Run(state, Console.Out);
    }
}
