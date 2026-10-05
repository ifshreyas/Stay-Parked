# Stay Parked

A persistent vehicle saving mod for Grand Theft Auto V built using C# and ScriptHookVDotNet.

Ever parked a vehicle somewhere in GTA V and returned later only to discover that it had despawned?

**Stay Parked** allows players to save vehicles at their current location and automatically restore them when the game is restarted.

> Park your vehicle. Leave the game. Come back and help me with  Stay Parked.

---

## Features

- Save up to 50 vehicles
- Automatically restore saved vehicles after restarting GTA V
- Save vehicles anywhere in the game world
- Simple save and remove interaction
- A vehicle must be entered at least once before it can be saved
- Configurable save/remove key
- Optional map blips for saved vehicles
- GTA-style vehicle blip icons
- Persistent JSON-based save system

Vehicle data includes:

- Position
- Heading
- Primary and secondary colours
- Vehicle modifications
- Wheel type
- Custom tyres
- Vehicle extras
- Window tint
- Engine health
- Body health
- Petrol tank health
- Dirt level
- Vehicle driveability state

---

## How It Works

1. Enter a vehicle.
2. Exit the vehicle.
3. Stand near the vehicle.
4. Press the configured key to save it.
5. The vehicle is stored in the persistent save file.

To remove a saved vehicle, stand near it and press the same configured key again.

### Important

A vehicle must have been entered by the player at least once before it becomes eligible for saving.

---

## Vehicle Limit

Stay Parked supports a maximum of **50 saved vehicles**.

Remove an existing saved vehicle before saving another vehicle when the limit has been reached.

---

## Installation

### Requirements

- Grand Theft Auto V
- Script Hook V
- ScriptHookVDotNet v3

### Install

Copy the following files into your GTA V `scripts` directory:

```text
SaveVehicleAnywhere.dll
SaveVehicleAnywhere.ini
```

Example:

```text
Grand Theft Auto V/
|
└── scripts/
    ├── SaveVehicleAnywhere.dll
    └── SaveVehicleAnywhere.ini
```

The save file will be created automatically after saving your first vehicle:

```text
scripts/SavedVehicles.json
```

---

## Configuration

The configuration file is:

```text
SaveVehicleAnywhere.ini
```

Default configuration:

```ini
[Settings]
SaveKey=Y
Blips=True
```

### SaveKey

Changes the key used to save and remove vehicles.

Supported keys: `A` to `Z`

Example:

```ini
SaveKey=E
```

### Blips

Controls whether saved vehicles appear on the GTA V map.

```ini
Blips=True
```

Shows saved vehicle blips.

```ini
Blips=False
```

Disables saved vehicle blips.

---

## Project Structure

```text
StayParked/
|
├── Config/
│   └── ModConfig.cs
|
├── Models/
│   ├── SavedVehicleData.cs
│   └── VehicleModData.cs
|
├── Properties/
│   └── AssemblyInfo.cs
|
├── Services/
│   ├── ConfigService.cs
│   ├── SaveService.cs
│   └── VehicleService.cs
|
├── UI/
│   └── ContextUI.cs
|
├── Main.cs
└── SaveVehicleAnywhere.csproj
```

---

## Architecture

The project is separated into several components:

### Config

Handles mod configuration, including:

- Save key
- Blip settings

### Models

Contains data models used to store vehicle information.

### Services

Handles:

- Vehicle detection
- Vehicle state collection
- Vehicle spawning
- Persistent saving and loading
- JSON serialization

### UI

Handles the in-game context messages shown to the player.

### Main

Acts as the main ScriptHookVDotNet script and coordinates the mod's behaviour.

---

## Data Persistence

Vehicle information is stored locally in:

```text
SavedVehicles.json
```

When GTA V starts, Stay Parked:

1. Loads the saved vehicle data.
2. Recreates saved vehicles.
3. Places them at their saved locations.
4. Restores their saved state.
5. Creates vehicle blips if enabled.

---

## Development

This project was developed using:

- C#
- .NET Framework
- ScriptHookVDotNet
- Script Hook V
- JSON-based data persistence

### Build

Open the project in JetBrains Rider or Visual Studio and build the project.

The compiled DLL can then be placed inside:

```text
Grand Theft Auto V/scripts/
```

---

## Known Limitations

- A vehicle must be entered at least once before it can be saved.
- A maximum of 50 vehicles can be saved simultaneously.
- Other vehicle-related mods may interfere with vehicle persistence.
- Script Hook V and ScriptHookVDotNet must be installed correctly.

---

## Version

### v1.0

Initial public release.

Features include:

- Persistent vehicle saving
- Automatic vehicle restoration
- Configurable save key
- Optional vehicle blips
- Support for up to 50 saved vehicles

---

## Author

Created by **Shreyas**

---

# Stay Parked

**Park your vehicle.**

**Leave the game.**

**Come back.**

**Stay Parked.**
