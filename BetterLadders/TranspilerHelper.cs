using System.Runtime.CompilerServices;
using BetterLadders.Config;
using HarmonyLib;
using UnityEngine;

namespace BetterLadders;

public static class TranspilerHelper
{
    public static IEnumerable<CodeInstruction> Patch(
        IEnumerable<CodeInstruction> instructions,
        Func<List<CodeInstruction>, List<(int logStartIndex, int logEndIndex)>, bool>[] patches,
        [CallerMemberName] string memberName = ""
    )
    {
        BetterLaddersPlugin.Logger.LogInfo($"Starting {memberName}");
        
        var code = new List<CodeInstruction>(instructions);

        for (var i = 0; i < patches.Length; i++)
        {
            try
            {
                var logIndices = new List<(int logStartIndex, int logEndIndex)>();
                var result = patches[i](code, logIndices);

                if (!result)
                {
                    BetterLaddersPlugin.Logger.LogError($"No IL match found for patch #{i} in {memberName}");
                    continue;
                }
                
                BetterLaddersPlugin.Logger.LogInfo($"Completed applying patch #{i} in {memberName}");
                if (!LocalConfig.Instance.DebugMode.Value)
                    continue;

                var longestLineLength = logIndices
                    .Select(s => code.GetRange(s.logStartIndex, s.logEndIndex - s.logStartIndex))
                    .Max(codes => codes.Max(c => c.ToString().Length));

                var borderWidth = longestLineLength + 2;

                for (var j = 0; j < logIndices.Count; j++)
                {
                    var (start, end) = logIndices[j];
                    var topBorderCharacters = "╞═╡";
                    if (j == 0)
                        topBorderCharacters = "┌─┐";

                    BetterLaddersPlugin.Logger.LogInfo(topBorderCharacters[0] +
                                                       new string(topBorderCharacters[1], borderWidth) +
                                                       topBorderCharacters[2]);
                    for (var k = start; k < end; k++)
                    {
                        var codeString = code[k].operand == null ? code[k].opcode.ToString() : code[k].ToString();
                        BetterLaddersPlugin.Logger.LogInfo("│ " + codeString.PadRight(longestLineLength) + " │");
                    }

                    if (j == logIndices.Count - 1)
                        BetterLaddersPlugin.Logger.LogInfo("└" + new string('─', borderWidth) + "┘");
                }
            }
            catch (Exception e)
            {
                BetterLaddersPlugin.Logger.LogError($"Error running patch #{i} in {memberName}: {e}");
            }
        }
        
        BetterLaddersPlugin.Logger.LogInfo($"Finished all patches in {memberName}");
        BetterLaddersPlugin.Logger.LogInfo("");

        return code;
    }
}