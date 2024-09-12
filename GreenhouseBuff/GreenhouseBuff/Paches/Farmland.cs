using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
using Vintagestory.API.Common;



namespace GreenhouseBuff
{
    [HarmonyPatch]
    internal class Farmland : ModSystem
    {
        //20 grows
        //15 dies
        
        public static float farmlandTempBonus = GreenhouseBuff.Config.FarmlandTempMod;

        //WORKS
        //A B TEST with and wiwout mod - works
        // TEST temp to high - works
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BlockEntityFarmland), "Update")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                // Locate the sequence: conds.Temperature += 5
                if (codes[i].opcode == OpCodes.Ldfld && codes[i + 1].opcode == OpCodes.Ldc_R4 &&
                    (float)codes[i + 1].operand == 5f && codes[i + 2].opcode == OpCodes.Add)
                {
                    // Replace it with conds.Temperature += farmlandTempBonus
                    codes[i + 1].operand = farmlandTempBonus;  // Change the operand to the loaded config value
                }
            }

            return codes.AsEnumerable();
        }

        //WORKS
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityFarmland), "GetBlockInfo")]
        static void Postfix(StringBuilder dsc)
        {

            // Replace the '5' in the greenhouse temp bonus string
            string originalString = Lang.Get("greenhousetempbonus");
            string modifiedString = originalString.Replace("5", farmlandTempBonus.ToString());

            // Modify the description
            if (dsc.ToString().Contains(originalString))
            {
                dsc.Replace(originalString, modifiedString);
            }
        }

    }
}
