using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Warhead;
using Exiled.Events.EventArgs.Server;
using CommandSystem;

namespace DMS_toggle
{
    public class StopWarheadPlugin : Plugin<Config>
    {
        public static StopWarheadPlugin Instance { get; private set; }

        public override string Name => "DMS-toggle";
        public override string Author => "HectorGH";
        public override Version Version => new Version(1, 1, 0);
        public override Version RequiredExiledVersion => new Version(9, 1, 1);

        private bool isWarheadDisabledThisRound;

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Warhead.Starting += OnWarheadStarting;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Warhead.Starting -= OnWarheadStarting;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Instance = null;
            base.OnDisabled();
        }

        private void OnWarheadStarting(StartingEventArgs ev)
        {
            if (isWarheadDisabledThisRound)
            {
                ev.IsAllowed = false;
                Log.Info("DMS has been blocked.");
            }
        }

        private void OnRoundStarted()
        {
            isWarheadDisabledThisRound = false;
        }

        public void SetWarheadStateForRound(bool state)
        {
            isWarheadDisabledThisRound = state;
        }

        public bool GetWarheadStateForRound()
        {
            return isWarheadDisabledThisRound;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ComandoEjemplo : ICommand
    {
        public string Command { get; } = "togglewarhead";
        public string[] Aliases { get; } = { "dms" };
        public string Description { get; } = "Block DMS during this round.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            if (StopWarheadPlugin.Instance == null)
            {
                response = "Plugin is not loaded.";
                return false;
            }

            bool newState = !StopWarheadPlugin.Instance.GetWarheadStateForRound();
            StopWarheadPlugin.Instance.SetWarheadStateForRound(newState);

            response = $"DMS change to {(newState ? "OFF" : "ON")} this round.";
            return true;
        }
    }

    public class Config : Exiled.API.Interfaces.IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
    }
}
