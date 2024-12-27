//DOESNT DO ANYTHING
//suspecting bad implemetation in game sas nothing but the code mensiones bees in Greenhouse
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Vintagestory.GameContent;
using System.Linq;
using Vintagestory.API.Common;


namespace GreenhouseBuff.ModPatches
{
    [HarmonyPatch]
    internal class Beehive : ModSystem
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BlockEntityBeehive), "TestHarvestable")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                // Locate the sequence: temp += 5
                if (codes[i].opcode == OpCodes.Ldloc_S && codes[i + 1].opcode == OpCodes.Ldc_R4 &&
                    (float)codes[i + 1].operand == 5f && codes[i + 2].opcode == OpCodes.Add)
                {
                    // Replace it with temp += beeTempBonus
                    codes[i + 1].operand = (float)GreenhouseBuffConfig.Loaded.BeehiveTempMod;  // Change the operand to the loaded config value
                }
            }

            return codes.AsEnumerable();
        }
    }
}
