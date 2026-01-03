using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using BetterLadders.Config;
using HarmonyLib;

namespace BetterLadders.Patches;

public static class ExtensionLadderSpeed
{
    private static readonly MethodInfo ExtensionSpeedMultiplierGetter = AccessTools.PropertyGetter(typeof(ExtensionLadderSpeed), nameof(ExtensionSpeedMultiplier));
    private static readonly MethodInfo FallSpeedMultiplierGetter = AccessTools.PropertyGetter(typeof(ExtensionLadderSpeed), nameof(FallSpeedMultiplier));
    
    private static float ExtensionSpeedMultiplier => LocalConfig.Instance.ExtensionSpeedMultiplier.Value;
    private static float FallSpeedMultiplier => LocalConfig.Instance.FallSpeedMultiplier.Value;
    
    
    [HarmonyTranspiler, HarmonyPatch(typeof(ExtensionLadderItem), nameof(ExtensionLadderItem.LadderAnimation), MethodType.Enumerator)]
    internal static IEnumerable<CodeInstruction> ExtensionSpeedTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return CommonTranspiler(instructions, ExtensionSpeedMultiplierGetter);
    }
    
    [HarmonyTranspiler, HarmonyPatch(typeof(ExtensionLadderItem), nameof(ExtensionLadderItem.LadderAnimation), MethodType.Enumerator)]
    internal static IEnumerable<CodeInstruction> FallSpeedTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return CommonTranspiler(instructions, FallSpeedMultiplierGetter);
    }

    private static IEnumerable<CodeInstruction> CommonTranspiler(IEnumerable<CodeInstruction> instructions, MethodInfo getter, [CallerMemberName] string memberName = "")
    {
        return TranspilerHelper.Patch(instructions, [
            (code, logIndices) =>
            {
                for (var i = 0; i < code.Count; i++)
                {
                    // target: speedMultiplier += Time.deltaTime * 2f;
                    if (code[i].opcode == OpCodes.Ldfld && code[i].operand is FieldInfo field && field.Name.Contains("speedMultiplier"))
                    {
                        for (var j = i; j < i + 10 && j < code.Count; j++)
                        {
                            if (code[j - 1].opcode != OpCodes.Call && // prevent double patch
                                code[j].opcode == OpCodes.Mul &&
                                code[j + 1].opcode == OpCodes.Add &&
                                code[j + 2].opcode == OpCodes.Stfld && code[j + 2].operand is FieldInfo field2 && field2.Name.Contains("speedMultiplier"))
                            {
                                code.InsertRange(j + 1, [
                                    new(OpCodes.Call, getter),
                                    new(OpCodes.Mul)
                                ]);
                                logIndices.Add((i, j + 5));
                                return true;
                            }
                        }
                    }
                }
                
                return false;
            }
        ], memberName);
    }
}