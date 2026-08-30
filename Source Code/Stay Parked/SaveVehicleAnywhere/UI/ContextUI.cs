using System.Windows.Forms;
using GTA.Native;

namespace SaveVehicleAnywhere.UI
{
    public static class ContextUI
    {
        public static void ShowHelp(string message)
        {
            Function.Call(
                Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP,
                "STRING"
            );

            Function.Call(
                Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME,
                message
            );

            Function.Call(
                Hash.END_TEXT_COMMAND_DISPLAY_HELP,
                0,
                false,
                true,
                -1
            );
        }

        public static void ShowSavePrompt(Keys key)
        {
            ShowHelp(
                $"Press ~y~{key}~s~ to save vehicle"
            );
        }

        public static void ShowRemovePrompt(Keys key)
        {
            ShowHelp(
                $"Press ~y~{key}~s~ to remove saved vehicle"
            );
        }

        public static void Notify(string message)
        {
            Function.Call(
                (Hash)0x202709F4C58A0424,
                "STRING"
            );

            Function.Call(
                (Hash)0x6C188BE134E074AA,
                message
            );

            Function.Call(
                (Hash)0x2ED7843F8F801023,
                false,
                false
            );
        }
    }
}