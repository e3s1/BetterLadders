using System.Reflection;
using System.Reflection.Emit;
using BetterLadders.Config;
using HarmonyLib;
using UnityEngine;

namespace BetterLadders.Patches;

internal static class TransitionSpeed
{
    private static readonly MethodInfo DeltaTimeGetter = AccessTools.PropertyGetter(typeof(Time), nameof(Time.deltaTime));
    private static readonly MethodInfo MultiplierGetter = AccessTools.PropertyGetter(typeof(TransitionSpeed), nameof(Multiplier));
    
    public static float Multiplier => LocalConfig.Instance.TransitionSpeedMultiplier.Value;
    
    [HarmonyTranspiler, HarmonyPatch(typeof(InteractTrigger), nameof(InteractTrigger.ladderClimbAnimation), MethodType.Enumerator)]
    internal static IEnumerable<CodeInstruction> TransitionSpeedTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return TranspilerHelper.Patch(instructions, [
            (code, logIndices) =>
            {
                var modifications = 0;
                
                for (var i = 0; i < code.Count; i++)
                {
                    // matches Time.deltaTime
                    if (code[i].opcode == OpCodes.Call && 
                        code[i].operand is MethodInfo method && method == DeltaTimeGetter)
                    {
                        // multiplies Time.deltaTime by Multiplier
                        code.Insert(i + 1, new CodeInstruction(OpCodes.Call, MultiplierGetter));
                        code.Insert(i + 2, new CodeInstruction(OpCodes.Mul));
                        
                        modifications++;
                        logIndices.Add((i, i + 3));
                    }
                }
                
                return modifications > 0;
            }
        ]);
    }
}
