using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace JNNJMods.OrbtXLCheat.Patches
{
    [HarmonyPatch]
    public static class GodmodePatch
    {
        public static bool Godmode;

        [HarmonyPatch(typeof(PlayerActualScript), "OnTriggerEnter")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter()
        {
            return !Godmode;
        }

        [HarmonyPatch(typeof(PlayerScript), "Update")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Update(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            var playerDeath = AccessTools.Field(typeof(PlayerScript), nameof(PlayerScript.playerDeath));
            var playerDeath2 = AccessTools.Field(typeof(PlayerScript), nameof(PlayerScript.playerDeath2));

            var godMode = AccessTools.Field(typeof(GodmodePatch), nameof(Godmode));


            bool foundPlayerDeath = false;
            bool foundCheck = false;
            object brFalseOperand = null;

            foreach (var instruction in instructionsList)
            {

                if(foundCheck)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, godMode);
                    yield return new CodeInstruction(OpCodes.Brtrue, brFalseOperand);
                    yield return instruction;

                    foundCheck = false;
                    foundPlayerDeath = false;
                    brFalseOperand = null;
                    continue;
                }

                if(foundPlayerDeath && instruction.opcode == OpCodes.Brfalse)
                {
                    brFalseOperand = instruction.operand;
                    foundPlayerDeath = false;
                    foundCheck = true;
                }

                if(instruction.opcode == OpCodes.Ldsfld && instruction.OperandIs(playerDeath) || instruction.OperandIs(playerDeath2))
                {
                    foundPlayerDeath = true;
                }

                yield return instruction;
            }
        }
    }
}
