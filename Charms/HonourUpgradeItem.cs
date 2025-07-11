using ItemChanger;
using ItemChanger.UIDefs;
using UnityEngine;

namespace PaleCourtCharms
{
    public class HonourUpgradeItem : AbstractItem
    {
        public HonourUpgradeItem()
        {
            name = "Kings_Honour";
            UIDef = new MsgUIDef
            {
                name     = new LanguageString("PaleCourtCharms", "KING_HONOUR_NAME"),
                shopDesc = new LanguageString("PaleCourtCharms", "KING_HONOUR_DESC"),
                sprite   = new ICShiny.ItemChangerSprite("Kings_Honour", PaleCourtCharms.SPRITES["Kings_Honour"])
            };
        }

        public override void GiveImmediate(GiveInfo info)
        {
           
            PaleCourtCharms.Settings.upgradedCharm_10 = true;
            PlayerData.instance.SetBool("upgradedCharm_10", true);
            Modding.Logger.Log("[PaleCourtCharms] King’s Honour upgrade granted.");
        }
    }
}
