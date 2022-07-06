using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace JNNJMods.OrbtXLCheat.Patches
{
    [HarmonyPatch(typeof(GUIManager), "ButtonPressed")]
    public static class ExitPatch
    {

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            var thanks = AccessTools.Field(typeof(GUIManager), nameof(GUIManager.thanks));
            var getGameObject = AccessTools.Property(typeof(GameObject), nameof(GameObject.gameObject)).GetGetMethod();
            var setActive = AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive));
            var quit = AccessTools.Method(typeof(Application), nameof(Application.Quit));

            bool found = false;
            bool found2 = false;
            bool found3 = false;

            foreach (var instruction in instructionsList)
            {

                if(found3 && instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(setActive))
                {
                    found3 = false;

                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Call, quit);

                    continue;
                }

                if(found2 && instruction.opcode == OpCodes.Ldc_I4_1)
                {
                    found2 = false;
                    found3 = true;
                }

                if(found && instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(getGameObject))
                {
                    found = false;
                    found2 = true;
                }

                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(thanks))
                {
                    found = true;
                }

                yield return instruction;
            }
        }

    }
}
