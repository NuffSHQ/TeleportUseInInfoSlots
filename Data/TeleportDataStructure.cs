using System.Collections.Generic;

namespace TeleportUseInInfoSlots.Data.TeleportDataStructure
{
    public class TeleportDataStructure
    {
        public List<int> VanillaItemIDs { get; set; } = new();
        public List<ModdedItemEntry> ModdedItems { get; set; } = new();
    }

    public class ModdedItemEntry
    {
        public string ModName { get; set; }
        public string ItemName { get; set; }
    }
}
