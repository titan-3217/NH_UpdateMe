using System;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;
using System.Linq;

namespace Events.Code
{
    public class Commands : CommandModule
    {
        public Commands()
        {
        }

        private bool coolDownHasPassed(string command)
        {
            bool exit = false;

            if (EventsPlugin.Instance._logic != null)
            {
                CommandConfig cmdConfig = null;
                 
                try
                {
                    cmdConfig = EventsPlugin.Instance.Config.Commands.First(c => c.Command == command);
                }
                catch { return exit; }

                if (base.Context.Player.PromoteLevel == MyPromoteLevel.Moderator || cmdConfig.PlayerCanUseIt)
                {
                    DateTime dateTime;
                    DateTime dateTime1;
                    ulong steamUserId = base.Context.Player.SteamUserId;
                    
                    if (base.Context.Player.PromoteLevel < MyPromoteLevel.Moderator && EventsPlugin.Instance._logic.spam_commandtimeout.TryGetValue(steamUserId, out dateTime) && (DateTime.Now - dateTime).TotalSeconds < (double)cmdConfig.Cooldown)
                    {
                        Util.SendMessage("Wait, you need to cooldown!", steamUserId);
                    }
                    else
                    {
                        //clear
                        if (!EventsPlugin.Instance._logic.spam_commandtimeout.TryGetValue(steamUserId, out dateTime1))
                        {
                            EventsPlugin.Instance._logic.spam_commandtimeout.Add(steamUserId, DateTime.Now);
                            exit = true;
                        }
                        else
                        {
                            EventsPlugin.Instance._logic.spam_commandtimeout[steamUserId] = DateTime.Now;
                            exit = true;
                        }
                    }
                }
            }

            return exit;
        }

        [Command("meteors", "A shower of meteors at player position.", null)]
        [Permission(MyPromoteLevel.None)] //Check here if player can use it.
        public void meteorShower()
        {
            if (coolDownHasPassed("meteors"))
            {
                EventsPlugin.Instance._logic.MessageQueue.Enqueue(new Util.Message()
                {
                    steamId = base.Context.Player.SteamUserId,
                    type = Util.MessageType.command
                });
                base.Context.Respond("Meteor shower incoming!", null, null);
                Util.meteorShower();
            }
        }


    }
}
