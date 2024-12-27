using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;

namespace GreenhouseBuff.ModNetwork
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SyncConfigClientPacket
    {
        public int BeehiveTempMod;
        public int BerryBushTempMod;
        public int FarmlandTempMod;
        public int FruitTreeTempMod;
    }
}
