using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace JNNJMods.OrbtXLCheat.Patches
{
    [HarmonyPatch]
    public static class HighscorePatch
    {

        [HarmonyPatch(typeof(Gameplay), nameof(Gameplay.RoundEnd))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RoundEnd(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);

            var bestScore = AccessTools.Field(typeof(SaveManager), nameof(SaveManager.bestScore));

            CodeInstruction[] inst = new CodeInstruction[3];

            bool foundBestScore = false;
            bool foundLdc = false;

            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                if(foundLdc && instruction.opcode == OpCodes.Bgt)
                {
                    inst[2] = instruction;
                    break;
                }

                if(foundBestScore && instruction.opcode == OpCodes.Ldc_I4)
                {
                    foundBestScore = false;
                    foundLdc = true;
                    inst[1] = instruction;
                }

                if (instruction.opcode == OpCodes.Ldsfld && instruction.OperandIs(bestScore))
                {
                    foundBestScore = true;
                    inst[0] = instruction;
                }
            }

            foreach (var instr in inst)
            {
                instructionsList.Remove(instr);
            }

            return instructionsList;
        }
    }
}
