using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using HarmonyLib;

using GreenhouseBuff.ModNetwork;
using Newtonsoft.Json;

// config stystem stolen from
// https://github.com/Foxcapades/vsmod-thrifty-smithing/blob/6f2b6a9b03d2823ee616314f4cd40753572b2bcc/src/main/thrifty/ThriftySmithing.cs#L13
// https://mods.vintagestory.at/thriftysmithing


namespace GreenhouseBuff
{
    [HarmonyPatch] // Place on any class with harmony patches
    public class GreenhouseBuff : ModSystem
    {
        private IServerNetworkChannel serverChannel;

        private ICoreAPI api;

        public Harmony harmony;

        public override void StartPre(ICoreAPI api)
        {
            string cfgFileName = "GreenhouseBuffConfig.json";

            try
            {
                GreenhouseBuffConfig cfgFromDisk;
                if ((cfgFromDisk = api.LoadModConfig<GreenhouseBuffConfig>(cfgFileName)) == null)
                {
                    this.Mod.Logger.Notification("No config file found, defult one will be generated");
                    api.StoreModConfig(GreenhouseBuffConfig.Loaded, cfgFileName);
                }
                else
                {
                    this.Mod.Logger.Notification("Config json loaded");
                    GreenhouseBuffConfig.Loaded = cfgFromDisk;
                }
            }
            catch
            {
                this.Mod.Logger.Error("Failed to load config JSON, loaded default");
                api.StoreModConfig(GreenhouseBuffConfig.Loaded, cfgFileName);
            }

            base.StartPre(api);
        }

        public override void Start(ICoreAPI api)
        {
            this.api = api;
            base.Start(api);

            // The mod is started once for the server and once for the client.
            // Prevent the patches from being applied by both in the same process.
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                harmony = new Harmony(Mod.Info.ModID);
                harmony.PatchAll(); // Applies all harmony patches
                this.Mod.Logger.Notification("Patches applied");
            }
        }

        private void OnPlayerJoin(IServerPlayer player)
        {
            // Send connecting players config settings
            this.serverChannel.SendPacket(
                new SyncConfigClientPacket
                {
                    BeehiveTempMod = GreenhouseBuffConfig.Loaded.BeehiveTempMod,
                    FarmlandTempMod = GreenhouseBuffConfig.Loaded.FarmlandTempMod,
                    BerryBushTempMod = GreenhouseBuffConfig.Loaded.BerryBushTempMod,
                    FruitTreeTempMod = GreenhouseBuffConfig.Loaded.FruitTreeTempMod
                }, player);
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            sapi.Event.PlayerJoin += this.OnPlayerJoin;

            // Create server channel for config data sync
            this.serverChannel = sapi.Network.RegisterChannel(Mod.Info.ModID)
                .RegisterMessageType<SyncConfigClientPacket>()
                .SetMessageHandler<SyncConfigClientPacket>((player, packet) => { });
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            // Sync config settings with clients
            capi.Network.RegisterChannel(Mod.Info.ModID)
                .RegisterMessageType<SyncConfigClientPacket>()
                .SetMessageHandler<SyncConfigClientPacket>(p => {
                    this.Mod.Logger.Event("Received config settings from server");
                    GreenhouseBuffConfig.Loaded.BeehiveTempMod = p.BeehiveTempMod;
                    GreenhouseBuffConfig.Loaded.FarmlandTempMod = p.FarmlandTempMod;
                    GreenhouseBuffConfig.Loaded.BerryBushTempMod = p.BerryBushTempMod;
                    GreenhouseBuffConfig.Loaded.FruitTreeTempMod = p.FruitTreeTempMod;
                });
        }

        public override void Dispose()
        {
            harmony?.UnpatchAll(Mod.Info.ModID);
            if (this.api is ICoreServerAPI sapi)
            {
                sapi.Event.PlayerJoin -= this.OnPlayerJoin;
            }
        }
    }
}
