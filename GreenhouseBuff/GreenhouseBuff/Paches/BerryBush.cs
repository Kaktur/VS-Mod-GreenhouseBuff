using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace GreenhouseBuff
{
    [HarmonyPatch]
    internal class BerryBush : ModSystem
    {
        public static float bushTempBonus = 20;

        //WORKS ?
        //A B TEST with and wiwout mod on 80 - works
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BlockEntityBerryBush), "CheckGrow")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                // Locate the sequence: temperature += 5
                if (codes[i].opcode == OpCodes.Ldloc_S && codes[i + 1].opcode == OpCodes.Ldc_R4 &&
                    (float)codes[i + 1].operand == 5f && codes[i + 2].opcode == OpCodes.Add)
                {
                    // Replace it with temperature += bushTempBonus
                    codes[i + 1].operand = bushTempBonus;  // Change the operand to the loaded config value
                }
            }

            return codes.AsEnumerable();
        }

        //WORKKS
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityBerryBush), "GetBlockInfo")]
        static void Postfix(StringBuilder sb)
        {

            // Replace the '5' in the greenhouse temp bonus string
            string originalString = Lang.Get("greenhousetempbonus");
            string modifiedString = originalString.Replace("5", bushTempBonus.ToString());

            // Modify the description
            if (sb.ToString().Contains(originalString))
            {
                sb.Replace(originalString, modifiedString);
            }
        }

    }
}
