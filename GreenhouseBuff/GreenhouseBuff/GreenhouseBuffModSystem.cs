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

using Vintagestory.API.Util;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Cairo;

// config stystem stolen from
// https://github.com/Foxcapades/vsmod-thrifty-smithing/blob/6f2b6a9b03d2823ee616314f4cd40753572b2bcc/src/main/thrifty/ThriftySmithing.cs#L13
// https://mods.vintagestory.at/thriftysmithing


namespace GreenhouseBuff
{
    public class GreenhouseBuffConfig
    {
        public int BeehiveTempMod { get; set; }
        public int BerryBushTempMod { get; set; }
        public int FarmlandTempMod { get; set; }
        public int FruitTreeTempMod { get; set; }
    }

    [HarmonyPatch] // Place on any class with harmony patches
    public class GreenhouseBuff : ModSystem
    {
        private const string ConfigFileName = "GreenhouseBuffConfig.json";

        public static ICoreAPI api;
        public Harmony harmony;

        private static GreenhouseBuffConfig loadedConfig;
        internal static GreenhouseBuffConfig Config => loadedConfig;

        public override void Start(ICoreAPI api)
        {
            GreenhouseBuff.api = api;
            loadConfig(api);
            // The mod is started once for the server and once for the client.
            // Prevent the patches from being applied by both in the same process.
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                harmony = new Harmony(Mod.Info.ModID);
                harmony.PatchAll(); // Applies all harmony patches
                api.Logger.Notification("[Greenhouse Buff] - patches applied");
            }
        }

        private static void loadConfig(ICoreAPI api)
        {
            GreenhouseBuffConfig defaultConfig = new GreenhouseBuffConfig
            {
                BeehiveTempMod = 0,
                BerryBushTempMod = 20,
                FruitTreeTempMod = 20,
                FarmlandTempMod = 20,
            };

            try
            {
                var configTemp = api.LoadModConfig<string>(ConfigFileName);

                if (configTemp == null)
            {
                    api.Logger.Notification("[Greenhouse Buff] - no config file found, defult one will be generated");
                    loadedConfig = defaultConfig;
                    api.StoreModConfig(JsonSerializer.Serialize(loadedConfig), ConfigFileName);
                }
                else
                {
                    var configJSON = JsonSerializer.Deserialize<GreenhouseBuffConfig>(configTemp);
                    api.Logger.Notification("[Greenhouse Buff] - config jason loaded, content {0}", configTemp);
                    loadedConfig = configJSON;
                }
        }
            catch (Exception e)
            {
                api.Logger.Notification("[Greenhouse Buff] - failed to load config JSON, loaded defult: {0}", e.Message);
                loadedConfig = defaultConfig;
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
