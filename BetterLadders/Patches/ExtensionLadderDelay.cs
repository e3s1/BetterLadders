using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using BetterLadders.Config;
using HarmonyLib;
using UnityEngine;

namespace BetterLadders.Patches;

public static class ExtensionLadderDelay
{
    private static readonly ConstructorInfo WaitForSecondsConstructor = AccessTools.Constructor(typeof(WaitForSeconds), [ typeof(float) ]);
    private static readonly MethodInfo ExtensionDelayGetter = AccessTools.PropertyGetter(typeof(ExtensionLadderDelay), nameof(ExtensionDelay));
    private static readonly MethodInfo FallDelayGetter = AccessTools.PropertyGetter(typeof(ExtensionLadderDelay), nameof(FallDelay));
    
    private static float ExtensionDelay => LocalConfig.Instance.ExtensionDelay.Value;
    private static float FallDelay => LocalConfig.Instance.FallDelay.Value;
    
    [HarmonyTranspiler, HarmonyPatch(typeof(ExtensionLadderItem), nameof(ExtensionLadderItem.LadderAnimation), MethodType.Enumerator)]
    internal static IEnumerable<CodeInstruction> ExtensionDelayTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return CommonTranspiler(instructions, LocalConfig.Instance.ExtensionDelay.VanillaValue, ExtensionDelayGetter);
    }

    [HarmonyTranspiler, HarmonyPatch(typeof(ExtensionLadderItem), nameof(ExtensionLadderItem.LadderAnimation), MethodType.Enumerator)]
    internal static IEnumerable<CodeInstruction> FallDelayTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return CommonTranspiler(instructions, LocalConfig.Instance.FallDelay.VanillaValue, FallDelayGetter);
    }

    private static IEnumerable<CodeInstruction> CommonTranspiler(IEnumerable<CodeInstruction> instructions, float vanillaValue, MethodInfo getter, [CallerMemberName] string memberName = "")
    {
        return TranspilerHelper.Patch(instructions, [
            (code, logIndices) =>
            {
                for (var i = 0; i < code.Count; i++)
                {
                    // target: yield return new WaitForSeconds(1f or 0.4f);
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (code[i].opcode == OpCodes.Ldc_R4 && code[i].operand is float f && f == vanillaValue &&
                        code[i + 1].opcode == OpCodes.Newobj && code[i + 1].operand is ConstructorInfo constructor && constructor == WaitForSecondsConstructor)
                    {
                        code.InsertRange(i + 1, [
                            new(OpCodes.Pop),
                            new(OpCodes.Call, getter)
                        ]);
                        logIndices.Add((i, i + 3));
                        return true;
                    }
                }
                
                return false;
            }
        ], memberName);
    }

}