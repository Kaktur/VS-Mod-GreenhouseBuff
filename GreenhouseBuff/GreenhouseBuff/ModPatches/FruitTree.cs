﻿using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using GreenhouseBuff.ModConfig;


namespace GreenhouseBuff.ModPatches
{
    //WORKKS
    //A B TEST with and wiwout mod - works
    // TEST temp to high - works
    [HarmonyPatch]
    internal class FruitTree : ModSystem
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FruitTreeRootBH), "getGreenhouseTempBonus")]
        public static void tree(ref float __result)
        {
            // Check if the original return value is 5 and modify it to 10
            if (__result == 5)
            {
                __result = (float)GreenhouseBuffConfig.Loaded.FruitTreeTempMod;
            }
        }
    }
}
