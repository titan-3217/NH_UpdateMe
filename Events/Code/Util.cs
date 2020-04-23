using Sandbox.Game.Entities;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.Managers;
using VRage.Game;

namespace Events.Code
{
    public static class Util
    {
        private static IChatManagerServer chatManager;


        static Util()
        {
        }

        public static void SendMessage(string msg, ulong steamId)
        {
            IChatManagerServer manager = null;
            if (Util.chatManager == null)
            {
                ITorchSession currentSession = EventsPlugin.Instance.Torch.CurrentSession;
                if (currentSession != null)
                {
                    IDependencyManager managers = currentSession.Managers;
                    if (managers != null)
                    {
                        manager = DependencyProviderExtensions.GetManager<IChatManagerServer>(managers);
                    }
                }

                Util.chatManager = manager;
            }
            try
            {
                Util.chatManager.SendMessageAsOther("Server", msg, "Blue", steamId);
            }
            catch
            {
            }
        }

        public static void Log(string msg)
        {
            if (EventsPlugin.Instance.Config.LogEnabled)
            {
                EventsPlugin.Log.Info(msg);
            }
        }

        public struct Message
        {
            public ulong steamId;

            public Util.MessageType type;
        }

        public enum MessageType
        {
            none,
            command,
            evento
        }

        public static void meteorShower()
        {
            //I need access to an instance of the game that only Torch can see, so... not possible.
            MyGlobalEvents.EnableEvents();
            MyGlobalEventBase myGlbEvent = MyGlobalEventFactory.CreateEvent(new MyDefinitionId(typeof(MyObjectBuilder_GlobalEventBase), "MeteorWave"));
            MyGlobalEvents.AddGlobalEvent(myGlbEvent);
            MyGlobalEvents.RescheduleEvent(myGlbEvent, new System.TimeSpan(5000));
            MyGlobalEvents.EnableEvents();
            //MyObjectBuilder_Sector aa; aa.Environment
            //VRage.Game.MyObjectBuilder_GlobalEventBase myGlbEvent2 = new MyObjectBuilder_GlobalEventBase();
            //MyAPIGateway.Session.GetWorld().Sector.SectorEvents.Events.Add(myGlbEvent2);
            //MyAPIGateway.Session.GetWorld().Sector.SectorEvents.Save(
            MyGlobalEvents.LoadEvents(MyAPIGateway.Session.GetWorld().Sector.SectorEvents);
            MyGlobalEvents.EnableEvents();
        }

        public static void executeCommandsInTorch()
        {
            //Torch.Server.TorchServer.Instance.
            //Torch.Server.TorchServer ee; ee.GameThread.



        }

       
    }
}
