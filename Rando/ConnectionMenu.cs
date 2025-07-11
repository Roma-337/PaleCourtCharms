    using System.Collections.Generic;
    using UnityEngine;
    using MenuChanger;
    using MenuChanger.Extensions;
    using MenuChanger.MenuElements;
    using MenuChanger.MenuPanels;
    using RandomizerMod.Menu;

    namespace PaleCourtCharms.Rando
    {
        public class ConnectionMenu
        {
            public static ConnectionMenu Instance;

            private readonly SmallButton pageRootButton;
            private readonly MenuPage page;
            private readonly MenuElementFactory<RandoSettings> factory;

            public static void Hook()
            {
                RandomizerMenuAPI.AddMenuPage(Construct, HandleButton);
                MenuChangerMod.OnExitMainMenu += () => Instance = null;
            }

            private static bool HandleButton(MenuPage landing, out SmallButton button)
            {
                if (RandomizerMod.RandomizerMod.IsRandoSave)
                {
                    button = Instance.pageRootButton;
                    button.Text.color = RandoMenuProxy.RS.Enabled
                        ? Colors.TRUE_COLOR
                        : Colors.FALSE_COLOR;
                    return true;
                }

                button = null;
                return false;
            }

            private static void Construct(MenuPage landing)
            {
                Instance = new ConnectionMenu(landing);
            }

            private ConnectionMenu(MenuPage landing)
            {
                page = new MenuPage("Pale Court Charms", landing);
                factory = new MenuElementFactory<RandoSettings>(page, RandoMenuProxy.RS);

                new VerticalItemPanel(
                    page,
                    new Vector2(0, 200),
                    48f,
                    true,
                    factory.Elements
                );

                pageRootButton = new SmallButton(landing, "Pale Court Charms");
                pageRootButton.AddHideAndShowEvent(landing, page);
            }

            public void Apply(SaveModSettings save)
            {
               
                RandoMenuProxy.RS = RandoSettings.FromSaveSettings(save);
                factory.SetMenuValues(RandoMenuProxy.RS);
            }

            public void Disable()
            {
                RandoMenuProxy.RS = new RandoSettings { Enabled = false };
                factory.SetMenuValues(RandoMenuProxy.RS);
            }
        }
    }
