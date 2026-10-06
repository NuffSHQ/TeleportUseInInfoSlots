using System.IO;
using System.Text.Json;
using Terraria.ModLoader;
using TeleportUseInInfoSlots.Data.TeleportDataStructure;

namespace TeleportUseInInfoSlots.Content.Systems
{
    public class TeleportInfoSystem : ModSystem
    {
        public static ModKeybind TeleportHotkey { get; private set; }
        public static TeleportDataStructure AllowedTeleportItems { get; private set; }
        public static readonly string TeleportItemJsonPath = "Data/TeleportItem.json"; // Path to the JSON file

        // Loads the embedded JSON file and deserializes it into the AllowedTeleportItems property
        public override void Load() 
        {
            
            string manifestName = TeleportItemJsonPath; // Update if inside a subfolder (e.g., YourModName.Data.teleport_items.json)
            using Stream stream = Mod.GetFileStream(manifestName);
            if (stream != null) {
                using StreamReader reader = new StreamReader(stream);
                string jsonText = reader.ReadToEnd();
                AllowedTeleportItems = JsonSerializer.Deserialize<TeleportDataStructure>(jsonText);
            } else {
                // Fallback safe defaults if file reading fails
                AllowedTeleportItems = new TeleportDataStructure();
            }
        }

        public override void Unload() {
            AllowedTeleportItems = null;
        }
    }
}
