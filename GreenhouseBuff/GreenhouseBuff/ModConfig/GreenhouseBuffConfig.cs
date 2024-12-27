using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseBuff.ModConfig
{
    public class GreenhouseBuffConfig
    {
        public static GreenhouseBuffConfig Loaded { get; set; } = new GreenhouseBuffConfig();

        public int BeehiveTempMod { get; set; } = 0;

        public int BerryBushTempMod { get; set; } = 20;
        public int FarmlandTempMod { get; set; } = 20;
        public int FruitTreeTempMod { get; set; } = 20;
    }
}
