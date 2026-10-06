using Terraria;
using Terraria.ModLoader;
using TeleportUseInInfoSlots.Content.Systems;
using Terraria.GameInput;
using TeleportUseInInfoSlots.Keybind.TeleportInfoKeybind;
using Terraria.ID;
using System.Reflection;

namespace TeleportUseInInfoSlots.Content.Players.TeleportInfoPlayer
{
    // Class that handles the teleportation logic when the player presses the teleport hotkey. 
    // It checks for teleportation devices in custom info accessory slots, 
    // and executes the appropriate teleportation behavior based on the item found.
    public class TeleportInfoPlayer : ModPlayer
    {
        private Item _backupItem = null;
        private int _originalSlot = -1;

        // Processes the teleportation hotkey input and triggers the teleportation behavior if a valid teleportation device is found in the player's equipped items.
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TeleportInfoKeybind.TeleportHotkey.JustPressed)
            {
                // Prevents teleport when player is holding item with mouse cursor
                if (Main.mouseItem != null && !Main.mouseItem.IsAir)
                { return; }

                Item teleportItem = FindTeleportItemInInfoSlots();

                if (teleportItem != null && !teleportItem.IsAir)
                {
                    TriggerTeleportBehavior(teleportItem);
                }
            }
        }

        // Triggers the teleportation behavior based on the provided teleportation item. 
        // It checks if the player is dead or if the item is null or air, and then determines if the item is a modded accessory. 
        // If it is, it handles the modded teleportation logic; otherwise, it executes a fallback teleportation behavior.
        private void TriggerTeleportBehavior(Item teleportItem)
        {
            if (Player.ItemAnimationActive || Player.dead || teleportItem == null || teleportItem.IsAir)
            { return; }

            // Backup the item in player's selected inventory slot then replaces the slot with the teleport accessory item
            _originalSlot = Player.selectedItem;
            _backupItem = Player.inventory[_originalSlot].Clone();
            Player.inventory[_originalSlot] = teleportItem.Clone();

            Player.controlUseItem = true;
            Player.ItemCheck();

            MutiplayerTeleportSync();
        }

        // Monitors active animation frames and restores the original item once item use concludes.
        public override void PostUpdate()
        {
            if (_backupItem != null && Player.itemAnimation == 0)
            {
                Player.inventory[_originalSlot] = _backupItem;
                _backupItem = null;
                _originalSlot = -1;
            }
        }

        // Synchronizes the teleportation action across multiplayer clients, ensuring that all players see the teleportation effect and state change
        private void MutiplayerTeleportSync()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Player.whoAmI);
            }
        }

        // Determines if the provided item is a valid teleportation device by checking against the allowed teleport items defined in the "TeleportInfoSystem" class.
        private bool IsATeleportDevice(Item item)
        {
            var data = TeleportInfoSystem.AllowedTeleportItems;

            if (data == null)
            {
                return false;
            }

            if (item.ModItem == null)
            {
                return data.VanillaItemIDs.Contains(item.type);
            }

            string currentMod = item.ModItem.Mod.Name;
            string currentItemName = item.ModItem.Name;

            return data.ModdedItems.Exists(x => x.ModName == currentMod && x.ItemName == currentItemName);
        }

        // Scans the custom info accessory slots for a valid teleportation device by using reflection to access the fields of the "InfoAccessorySlots" ModPlayer.
        private Item FindTeleportItemInInfoSlots()
        {
            ModPlayer infoModPlayer = null;

            // Iterate through all ModPlayer instances attached to the player to find the one related to "InfoAccessorySlots"
            foreach (var modPlayer in Player.ModPlayers)
            {
                if (modPlayer.GetType().FullName.Contains("InfoAccessorySlots"))
                {
                    infoModPlayer = modPlayer;
                    break;
                }
            }

            if (infoModPlayer == null)
            {
                return null;
            }

            // Use reflection to access the fields of the "InfoAccessorySlots" ModPlayer to find the inventory array
            var fields = infoModPlayer.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo targetInventoryField = null;

            // Iterate through the fields to find the one that holds the inventory of items inside the array of Items
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(Item[]))
                {
                    targetInventoryField = f;
                    break;
                }
            }

            // If the target inventory field is found, retrieve its value and iterate through the items to find a valid teleportation device
            if (targetInventoryField?.GetValue(infoModPlayer) is System.Collections.IEnumerable rawCollection)
            {
                foreach (object element in rawCollection)
                {
                    if (element is Item infoItem && infoItem != null && !infoItem.IsAir && IsATeleportDevice(infoItem))
                    {
                        return infoItem;
                    }
                }
            }
            return null;
        }
    }
}

