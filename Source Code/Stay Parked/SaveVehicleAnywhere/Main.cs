using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using SaveVehicleAnywhere.Config;
using SaveVehicleAnywhere.Models;
using SaveVehicleAnywhere.Services;
using SaveVehicleAnywhere.UI;

namespace SaveVehicleAnywhere
{
    public class Main : Script
    {
        private const float InteractionDistance = 4.0f;

        private readonly ModConfig _config;
        private readonly SaveService _saveService;
        private readonly VehicleService _vehicleService;

        // Vehicles the player has sat in during the current game session.
        private readonly HashSet<int> _drivenVehicleHandles =
            new HashSet<int>();

        // Runtime mapping:
        // GTA vehicle handle -> permanent saved vehicle ID.
        private readonly Dictionary<int, string> _savedVehicleIds =
            new Dictionary<int, string>();

        private bool _wasInVehicle;

        private Vehicle _lastVehicle;

        private bool _keyWasDown;

        public Main()
        {
            _config = ConfigService.Load();

            _saveService = new SaveService();

            _vehicleService = new VehicleService();

            Tick += OnTick;

            Aborted += OnAborted;

            RestoreSavedVehicles();

            ContextUI.Notify(
                "Save Vehicle Anywhere loaded."
            );
        }

        private void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player == null || !player.Exists())
            {
                return;
            }

            TrackDrivenVehicle(player);

            HandleVehicleInteraction(player);

            CleanupInvalidVehicleReferences();

            _keyWasDown =
                Game.IsKeyPressed(_config.SaveKey);
        }

        private void TrackDrivenVehicle(Ped player)
        {
            bool currentlyInVehicle =
                player.IsInVehicle();

            if (currentlyInVehicle)
            {
                Vehicle vehicle =
                    player.CurrentVehicle;

                if (vehicle != null &&
                    vehicle.Exists())
                {
                    _drivenVehicleHandles.Add(
                        vehicle.Handle
                    );

                    _lastVehicle =
                        vehicle;
                }
            }

            _wasInVehicle =
                currentlyInVehicle;
        }

        private void HandleVehicleInteraction(
            Ped player)
        {
            if (player.IsInVehicle())
            {
                return;
            }

            Vehicle vehicle =
                _vehicleService.GetNearbyVehicle(
                    player,
                    InteractionDistance
                );

            if (vehicle == null ||
                !vehicle.Exists())
            {
                return;
            }

            // The player must have sat inside this vehicle
            // at least once during the current session.
            bool hasDrivenVehicle =
                _drivenVehicleHandles.Contains(
                    vehicle.Handle
                );

            // Restored saved vehicles are also valid
            // for removal even if the player hasn't driven
            // them during this session.
            bool isSaved =
                _savedVehicleIds.ContainsKey(
                    vehicle.Handle
                );

            if (!hasDrivenVehicle && !isSaved)
            {
                return;
            }

            if (isSaved)
            {
                ContextUI.ShowRemovePrompt(
                    _config.SaveKey
                );
            }
            else
            {
                if (!_saveService.CanAddVehicle())
                {
                    ContextUI.ShowHelp(
                        "Maximum saved vehicle limit reached (50)"
                    );

                    return;
                }

                ContextUI.ShowSavePrompt(
                    _config.SaveKey
                );
            }

            bool keyPressed =
                Game.IsKeyPressed(
                    _config.SaveKey
                );

            if (keyPressed && !_keyWasDown)
            {
                if (isSaved)
                {
                    RemoveSavedVehicle(
                        vehicle
                    );
                }
                else
                {
                    SaveVehicle(
                        vehicle
                    );
                }
            }
        }

        private void SaveVehicle(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
            {
                return;
            }

            if (!_saveService.CanAddVehicle())
            {
                ContextUI.Notify(
                    "~r~Maximum of 50 vehicles can be saved."
                );

                return;
            }

            string id =
                _saveService.CreateId();

            SavedVehicleData data =
                _vehicleService.CaptureVehicle(
                    vehicle,
                    id
                );

            if (data == null)
            {
                ContextUI.Notify(
                    "~r~Failed to save vehicle."
                );

                return;
            }

            _saveService.Add(
                data
            );

            _savedVehicleIds[
                vehicle.Handle
            ] = id;

            _vehicleService.MakePersistent(
                vehicle
            );

            if (_config.BlipsEnabled)
            {
                _vehicleService.CreateBlip(
                    vehicle
                );
            }

            ContextUI.Notify(
                "~g~Vehicle saved successfully."
            );
        }

        private void RemoveSavedVehicle(
            Vehicle vehicle)
        {
            if (vehicle == null ||
                !vehicle.Exists())
            {
                return;
            }

            string id;

            if (!_savedVehicleIds.TryGetValue(
                    vehicle.Handle,
                    out id))
            {
                return;
            }

            bool removed =
                _saveService.Remove(
                    id
                );

            if (!removed)
            {
                ContextUI.Notify(
                    "~r~Failed to remove saved vehicle."
                );

                return;
            }

            _vehicleService.RemoveBlip(
                vehicle
            );

            _vehicleService.ReleaseVehicle(
                vehicle
            );

            _savedVehicleIds.Remove(
                vehicle.Handle
            );

            ContextUI.Notify(
                "~y~Vehicle removed from saved vehicles."
            );
        }

        private void RestoreSavedVehicles()
        {
            foreach (
                SavedVehicleData data
                in _saveService.Vehicles)
            {
                try
                {
                    Vehicle vehicle =
                        _vehicleService.RestoreVehicle(
                            data
                        );

                    if (vehicle == null ||
                        !vehicle.Exists())
                    {
                        continue;
                    }

                    _savedVehicleIds[
                        vehicle.Handle
                    ] = data.Id;

                    // Restored vehicles are eligible
                    // for interaction/removal.
                    _drivenVehicleHandles.Add(
                        vehicle.Handle
                    );

                    if (_config.BlipsEnabled)
                    {
                        _vehicleService.CreateBlip(
                            vehicle
                        );
                    }
                }
                catch
                {
                    // One broken vehicle record should
                    // never stop the mod from loading.
                }
            }
        }

        private void CleanupInvalidVehicleReferences()
        {
            List<int> invalidHandles =
                new List<int>();

            foreach (
                KeyValuePair<int, string> item
                in _savedVehicleIds)
            {
                int handle =
                    item.Key;

                Entity entity =
                    Entity.FromHandle(
                        handle
                    );

                if (entity == null ||
                    !entity.Exists())
                {
                    invalidHandles.Add(
                        handle
                    );
                }
            }

            foreach (
                int handle
                in invalidHandles)
            {
                _savedVehicleIds.Remove(
                    handle
                );

                _drivenVehicleHandles.Remove(
                    handle
                );
            }
        }

        private void OnAborted(
            object sender,
            EventArgs e)
        {
            _vehicleService.RemoveAllBlips();
        }
    }
}