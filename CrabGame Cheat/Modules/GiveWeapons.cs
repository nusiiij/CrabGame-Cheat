using Il2CppSystem.Collections.Generic;
using JNNJMods.UI;
using JNNJMods.UI.Elements;
using Newtonsoft.Json;
using SteamworksNative;
using JNNJMods.CrabGameCheat.Util;

namespace JNNJMods.CrabGameCheat.Modules
{
    [CheatModule]
    public class SpawnerModule : MultiElementModuleBase
    {
        [JsonIgnore]
        private bool init;

        public SpawnerModule(ClickGUI gui) : base("Spawner", gui, WindowIDs.ITEM_SPAWNER)
        {

        }

        public static void SpawnItem(ItemData data)
        {
            ServerSend.ForceGiveItem(SteamUser.GetSteamID().m_SteamID, data.itemID, data.objectID);
        }

        public override void Update()
        {

            if (InGame && !init && ItemManager.Instance != null)
            {
                init = true;

                foreach (KeyValuePair<int, ItemData> entry in ItemManager.idToItem)
                {
                    ButtonInfo info = new ButtonInfo(ID, "Spawn " + entry.value.name, true);
                    info.ButtonPress += () =>
                    {
                        if (!InGame)
                            return;
                        SpawnItem(entry.value);
                    };

                    Elements.Add(info);
                }

                foreach (ElementInfo eInfo in Elements)
                {
                    Gui.AddElement(eInfo);
                }
            }
        }

    }
}