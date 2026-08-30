using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using SaveVehicleAnywhere.Models;

namespace SaveVehicleAnywhere.Services
{
    public class VehicleService
    {
        private readonly Dictionary<int, Blip> _vehicleBlips =
            new Dictionary<int, Blip>();

        public Vehicle GetNearbyVehicle(Ped player, float maxDistance)
        {
            if (player == null || !player.Exists())
            {
                return null;
            }

            Vehicle closestVehicle = null;
            float closestDistance = maxDistance;

            Vehicle[] vehicles = World.GetAllVehicles();

            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle == null || !vehicle.Exists())
                {
                    continue;
                }

                float distance =
                    player.Position.DistanceTo(vehicle.Position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestVehicle = vehicle;
                }
            }

            return closestVehicle;
        }

        public SavedVehicleData CaptureVehicle(
            Vehicle vehicle,
            string id)
        {
            if (vehicle == null || !vehicle.Exists())
            {
                return null;
            }

            VehicleModData modData = CaptureModData(vehicle);

            OutputArgument primaryColorArgument =
                new OutputArgument();

            OutputArgument secondaryColorArgument =
                new OutputArgument();

            Function.Call(
                Hash.GET_VEHICLE_COLOURS,
                vehicle.Handle,
                primaryColorArgument,
                secondaryColorArgument
            );

            int primaryColor =
                primaryColorArgument.GetResult<int>();

            int secondaryColor =
                secondaryColorArgument.GetResult<int>();


            OutputArgument pearlescentColorArgument =
                new OutputArgument();

            OutputArgument wheelColorArgument =
                new OutputArgument();

            Function.Call(
                Hash.GET_VEHICLE_EXTRA_COLOURS,
                vehicle.Handle,
                pearlescentColorArgument,
                wheelColorArgument
            );

            int pearlescentColor =
                pearlescentColorArgument.GetResult<int>();

            int wheelColor =
                wheelColorArgument.GetResult<int>();

            return new SavedVehicleData
            {
                Id = id,

                ModelHash = vehicle.Model.Hash,

                X = vehicle.Position.X,
                Y = vehicle.Position.Y,
                Z = vehicle.Position.Z,

                Heading = vehicle.Heading,

                PrimaryColor = primaryColor,
                SecondaryColor = secondaryColor,

                PearlescentColor = pearlescentColor,
                WheelColor = wheelColor,

                EngineHealth = vehicle.EngineHealth,
                BodyHealth = vehicle.BodyHealth,
                PetrolTankHealth = vehicle.PetrolTankHealth,

                DirtLevel = vehicle.DirtLevel,

                

                EngineRunning = vehicle.IsEngineRunning,

                IsUndriveable = !vehicle.IsDriveable,

                ModData = modData
            };
        }

        public Vehicle RestoreVehicle(
            SavedVehicleData data)
        {
            if (data == null)
            {
                return null;
            }

            Model model = new Model(data.ModelHash);

            if (!model.IsInCdImage ||
                !model.IsValid ||
                !model.IsVehicle)
            {
                return null;
            }

            model.Request(10000);

            if (!model.IsLoaded)
            {
                return null;
            }

            Vector3 position = new Vector3(
                data.X,
                data.Y,
                data.Z
            );

            Vehicle vehicle =
                Vehicle.Create(
                    model,
                    position,
                    data.Heading
                );

            model.MarkAsNoLongerNeeded();

            if (vehicle == null || !vehicle.Exists())
            {
                return null;
            }

            ApplyVehicleData(
                vehicle,
                data
            );

            vehicle.IsPersistent = true;

            return vehicle;
        }

        public void ApplyVehicleData(
            Vehicle vehicle,
            SavedVehicleData data)
        {
            if (vehicle == null ||
                !vehicle.Exists() ||
                data == null)
            {
                return;
            }

            vehicle.Position = new Vector3(
                data.X,
                data.Y,
                data.Z
            );

            vehicle.Heading = data.Heading;

            Function.Call(
                Hash.SET_VEHICLE_COLOURS,
                vehicle.Handle,
                data.PrimaryColor,
                data.SecondaryColor
            );

            Function.Call(
                Hash.SET_VEHICLE_EXTRA_COLOURS,
                vehicle.Handle,
                data.PearlescentColor,
                data.WheelColor
            );

            vehicle.EngineHealth =
                data.EngineHealth;

            vehicle.BodyHealth =
                data.BodyHealth;

            vehicle.PetrolTankHealth =
                data.PetrolTankHealth;

            vehicle.DirtLevel =
                data.DirtLevel;
            

            vehicle.IsUndriveable =
                data.IsUndriveable;

            ApplyModData(
                vehicle,
                data.ModData
            );

            vehicle.IsEngineRunning =
                data.EngineRunning;
        }

        private VehicleModData CaptureModData(
            Vehicle vehicle)
        {
            VehicleModData data =
                new VehicleModData();

            if (vehicle == null ||
                !vehicle.Exists())
            {
                return data;
            }

            Function.Call(
                Hash.SET_VEHICLE_MOD_KIT,
                vehicle.Handle,
                0
            );

            data.WheelType = Convert.ToInt32(
                Function.Call<int>(
                    Hash.GET_VEHICLE_WHEEL_TYPE,
                    vehicle.Handle
                )
            );

            data.WindowTint = Convert.ToInt32(
                Function.Call<int>(
                    Hash.GET_VEHICLE_WINDOW_TINT,
                    vehicle.Handle
                )
            );

            for (int modType = 0; modType <= 49; modType++)
            {
                int modIndex = Function.Call<int>(
                    Hash.GET_VEHICLE_MOD,
                    vehicle.Handle,
                    modType
                );

                if (modIndex != -1)
                {
                    data.Mods[modType] =
                        modIndex;
                }

                bool toggleState =
                    Function.Call<bool>(
                        Hash.IS_TOGGLE_MOD_ON,
                        vehicle.Handle,
                        modType
                    );

                if (toggleState)
                {
                    data.ToggleMods[modType] =
                        true;
                }
            }

            data.WheelIndex =
                Function.Call<int>(
                    Hash.GET_VEHICLE_MOD,
                    vehicle.Handle,
                    23
                );

            data.CustomTires =
                Function.Call<bool>(
                    Hash.GET_VEHICLE_MOD_VARIATION,
                    vehicle.Handle,
                    23
                );

            for (int extra = 0; extra <= 20; extra++)
            {
                bool exists =
                    Function.Call<bool>(
                        Hash.DOES_EXTRA_EXIST,
                        vehicle.Handle,
                        extra
                    );

                if (!exists)
                {
                    continue;
                }

                bool enabled =
                    !Function.Call<bool>(
                        Hash.IS_VEHICLE_EXTRA_TURNED_ON,
                        vehicle.Handle,
                        extra
                    );

                data.Extras[extra] =
                    enabled;
            }

            return data;
        }

        private void ApplyModData(
            Vehicle vehicle,
            VehicleModData data)
        {
            if (vehicle == null ||
                !vehicle.Exists() ||
                data == null)
            {
                return;
            }

            Function.Call(
                Hash.SET_VEHICLE_MOD_KIT,
                vehicle.Handle,
                0
            );

            Function.Call(
                Hash.SET_VEHICLE_WHEEL_TYPE,
                vehicle.Handle,
                data.WheelType
            );

            foreach (
                KeyValuePair<int, int> mod
                in data.Mods)
            {
                bool customTires = false;

                if (mod.Key == 23)
                {
                    customTires =
                        data.CustomTires;
                }

                Function.Call(
                    Hash.SET_VEHICLE_MOD,
                    vehicle.Handle,
                    mod.Key,
                    mod.Value,
                    customTires
                );
            }

            foreach (
                KeyValuePair<int, bool> toggle
                in data.ToggleMods)
            {
                Function.Call(
                    Hash.TOGGLE_VEHICLE_MOD,
                    vehicle.Handle,
                    toggle.Key,
                    toggle.Value
                );
            }

            Function.Call(
                Hash.SET_VEHICLE_WINDOW_TINT,
                vehicle.Handle,
                data.WindowTint
            );

            foreach (
                KeyValuePair<int, bool> extra
                in data.Extras)
            {
                Function.Call(
                    Hash.SET_VEHICLE_EXTRA,
                    vehicle.Handle,
                    extra.Key,
                    extra.Value ? 0 : 1
                );
            }
        }

        public void CreateBlip(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
            {
                return;
            }

            RemoveBlip(vehicle);

            Blip blip =
                vehicle.AddBlip();

            if (blip == null ||
                !blip.Exists())
            {
                return;
            }

            // GTA V personal vehicle car icon
            Function.Call(
                Hash.SET_BLIP_SPRITE,
                blip.Handle,
                225
            );

            // Blue colour
            Function.Call(
                Hash.SET_BLIP_COLOUR,
                blip.Handle,
                3
            );

            blip.Name = "Saved Vehicle";

            blip.IsShortRange = false;

            _vehicleBlips[vehicle.Handle] =
                blip;
        }

        public void RemoveBlip(
            Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return;
            }

            Blip blip;

            if (_vehicleBlips.TryGetValue(
                    vehicle.Handle,
                    out blip))
            {
                if (blip != null &&
                    blip.Exists())
                {
                    blip.Delete();
                }

                _vehicleBlips.Remove(
                    vehicle.Handle
                );
            }
        }

        public void RemoveAllBlips()
        {
            foreach (
                KeyValuePair<int, Blip> item
                in _vehicleBlips)
            {
                Blip blip =
                    item.Value;

                if (blip != null &&
                    blip.Exists())
                {
                    blip.Delete();
                }
            }

            _vehicleBlips.Clear();
        }

        public void MakePersistent(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
            {
                return;
            }

            vehicle.IsPersistent = true;
        }

        public void ReleaseVehicle(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
            {
                return;
            }

            vehicle.IsPersistent = false;
        }

        
    }
}