using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using SaveVehicleAnywhere.Models;

namespace SaveVehicleAnywhere.Services
{
    public class SaveService
    {
        public const int MaxSavedVehicles = 50;

        private readonly string _saveFilePath;

        public List<SavedVehicleData> Vehicles { get; private set; }

        private readonly JavaScriptSerializer _serializer;

        public SaveService()
        {
            _saveFilePath = Path.Combine(
                "scripts",
                "SavedVehicles.json"
            );

            _serializer = new JavaScriptSerializer
            {
                RecursionLimit = 100,
                MaxJsonLength = int.MaxValue
            };

            Vehicles = Load();
        }

        public List<SavedVehicleData> Load()
        {
            try
            {
                if (!File.Exists(_saveFilePath))
                {
                    return new List<SavedVehicleData>();
                }

                string json =
                    File.ReadAllText(
                        _saveFilePath
                    );

                List<SavedVehicleData> vehicles =
                    _serializer.Deserialize<
                        List<SavedVehicleData>
                    >(json);

                return vehicles ??
                       new List<SavedVehicleData>();
            }
            catch
            {
                return new List<SavedVehicleData>();
            }
        }

        public void Save()
        {
            try
            {
                string directory =
                    Path.GetDirectoryName(
                        _saveFilePath
                    );

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(
                        directory
                    );
                }

                string json =
                    _serializer.Serialize(
                        Vehicles
                    );

                File.WriteAllText(
                    _saveFilePath,
                    json
                );
            }
            catch
            {
            }
        }

        public bool CanAddVehicle()
        {
            return Vehicles.Count <
                   MaxSavedVehicles;
        }

        public bool Contains(string id)
        {
            return Vehicles.Exists(
                vehicle =>
                    vehicle.Id == id
            );
        }

        public void Add(
            SavedVehicleData vehicle)
        {
            if (vehicle == null)
            {
                return;
            }

            if (!CanAddVehicle())
            {
                return;
            }

            Vehicles.Add(vehicle);

            Save();
        }

        public bool Remove(string id)
        {
            SavedVehicleData vehicle =
                Vehicles.Find(
                    v => v.Id == id
                );

            if (vehicle == null)
            {
                return false;
            }

            Vehicles.Remove(vehicle);

            Save();

            return true;
        }

        public string CreateId()
        {
            return Guid.NewGuid()
                .ToString();
        }
    }
}