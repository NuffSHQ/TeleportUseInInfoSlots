using Terraria.ModLoader;

namespace TeleportUseInInfoSlots.Keybind.TeleportInfoKeybind
{
    // Class that defines a keybind for teleportation functionality in the mod
    public class TeleportInfoKeybind : ModSystem
    {
        public static ModKeybind TeleportHotkey { get; private set; }

        public override void Load() {
            // Registers a hotkey named "Teleport" bound to 'None' by default
            TeleportHotkey = KeybindLoader.RegisterKeybind(Mod, "Teleport", "H");
        }

        public override void Unload() {
            TeleportHotkey = null;
        }
    }
}