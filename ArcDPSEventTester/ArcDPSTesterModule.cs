using System;
using System.ComponentModel.Composition;
using Blish_HUD;
using Blish_HUD.ArcDps;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Newtonsoft.Json;

namespace ArcDPSEventTester
{
    [Export(typeof(Module))]
    public class ArcDPSTesterModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<ArcDPSTesterModule>();
        private ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;

        private WindowTab Tab;
        private MainView MainView;

        [ImportingConstructor]
        public ArcDPSTesterModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void OnModuleLoaded(EventArgs e)
        {
            this.MainView = new MainView();
            this.MainView.Built += (o, ee) =>
            {
                GameService.ArcDps.RawCombatEvent += ArcDps_RawCombatEvent;
            };

            this.Tab = GameService.Overlay.BlishHudWindow.AddTab(
                name: "ArcDPS Events Tester",
                icon: this.ContentsManager.GetTexture("power.png"),
                viewFunc: () => this.MainView
            );

            GameService.ArcDps.Common.Activate();

            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private void ArcDps_RawCombatEvent(object sender, RawCombatEventArgs e)
        {
            var json = JsonConvert.SerializeObject(e);

            this.MainView.AddLine(json);
            Logger.Debug(json);
        }

        protected override void Unload()
        {
            GameService.Overlay.BlishHudWindow.RemoveTab(this.Tab);
        }
    }

    internal class MainView : View
    {
        private MultilineTextBox TextBox;

        protected override void Build(Container buildPanel)
        {
            this.TextBox = new MultilineTextBox()
            {
                Parent = buildPanel,
                Location = buildPanel.ContentRegion.Location,
                Size = buildPanel.ContentRegion.Size,
            };
        }

        internal void AddLine(string line)
        {
            this.TextBox.Text += line + Environment.NewLine;
        }
    }
}
