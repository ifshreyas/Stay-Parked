using System.Collections.Generic;

namespace SaveVehicleAnywhere.Models
{
    public class VehicleModData
    {
        public Dictionary<int, int> Mods { get; set; } =
            new Dictionary<int, int>();

        public Dictionary<int, bool> ToggleMods { get; set; } =
            new Dictionary<int, bool>();

        public int WheelType { get; set; }

        public int WheelIndex { get; set; }

        public int WindowTint { get; set; }

        public bool CustomTires { get; set; }

        public Dictionary<int, bool> Extras { get; set; } =
            new Dictionary<int, bool>();
    }
}