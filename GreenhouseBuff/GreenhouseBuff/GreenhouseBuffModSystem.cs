using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using Vintagestory.ClientNative;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using System.Text;
using System.Linq;



namespace GreenhouseBuff
{
    [HarmonyPatch] // Place on any class with harmony patches
    public class MyModSystem : ModSystem
    {
        public static ICoreAPI api;
        public Harmony harmony;

        public override void Start(ICoreAPI api)
        {
            MyModSystem.api = api;
            // The mod is started once for the server and once for the client.
            // Prevent the patches from being applied by both in the same process.
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                harmony = new Harmony(Mod.Info.ModID);
                harmony.PatchAll(); // Applies all harmony patches
                api.Logger.Event("Greenhouse Buff - patches applied");
            }
        }
        public override void Dispose()
        {
            harmony?.UnpatchAll(Mod.Info.ModID);
        }


        ////TEST PACH
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Entity), "ReceiveDamage")]
        //// Note that the name of the function does not matter
        //public static void test(Entity __instance, float damage)
        //{ // For methods, use __instance to obtain the caller object
        //    api.Logger.Event("{0} is about to take {1} damage!", __instance, damage);
        //}


    }

}
