namespace SaveVehicleAnywhere.Models
{
    public class SavedVehicleData
    {
        public string Id { get; set; }

        public int ModelHash { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float Heading { get; set; }

        public int PrimaryColor { get; set; }

        public int SecondaryColor { get; set; }

        public int PearlescentColor { get; set; }

        public int WheelColor { get; set; }

        public float EngineHealth { get; set; }

        public float BodyHealth { get; set; }

        public float PetrolTankHealth { get; set; }

        public float DirtLevel { get; set; }
        
        public bool EngineRunning { get; set; }

        public bool IsUndriveable { get; set; }
        
        public VehicleModData ModData { get; set; } =
            new VehicleModData();
    }
}