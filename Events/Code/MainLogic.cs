using System;
using System.Collections.Generic;
using System.Threading;
using Torch.API.Managers;
using Torch.API.Session;

namespace Events.Code
{
    public class MainLogic
    {
        public Queue<Util.Message> MessageQueue = new Queue<Util.Message>();
        public Dictionary<ulong, DateTime> spam_commandtimeout = new Dictionary<ulong, DateTime>();
        private Thread _queueMsg;
        private bool _processing;

        public MainLogic()
        {
            ITorchSession currentSession = EventsPlugin.Instance.Torch.CurrentSession;
            if (currentSession != null)
            {
                IDependencyManager managers = currentSession.Managers;
                if (managers != null)
                {
                    DependencyProviderExtensions.GetManager<IMultiplayerManagerServer>(managers);
                }
            }

            this._queueMsg = new Thread(new ThreadStart(this.mainProcess))
            {
                Priority = ThreadPriority.BelowNormal
            };
            this._queueMsg.Start();
        }

        private void mainProcess()
        {
            Thread.Sleep(3000);
            Util.Log("Events plugin started!");

            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    if (EventsPlugin.Instance._logic.MessageQueue.Count != 0)
                    {
                        if (!EventsPlugin.Instance._logic._processing)
                        {
                            Util.Message msg = new Util.Message();
                            if (QueueExtensions.TryDequeue<Util.Message>(EventsPlugin.Instance._logic.MessageQueue, out msg))
                            {
                                EventsPlugin.Instance._logic._processing = true;

                                //Do some process here if you want to.


                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.Log(string.Concat("Events plugin catched an exception!", ex, ex.StackTrace));
                }
                finally
                {
                    EventsPlugin.Instance._logic._processing = false;
                }
            }
        }

    }
}
