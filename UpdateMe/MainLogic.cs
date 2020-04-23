using Sandbox.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.Utils;

namespace UpdateMe
{
    public class MainLogic
    {
        private bool _processing;
        private Thread _mainProcessThread;

        public MainLogic()
        {
            ITorchSession currentSession = UpdateMePlugin.Instance.Torch.CurrentSession;
            
            if (currentSession != null)
            {
                IDependencyManager managers = currentSession.Managers;
                if (managers != null)
                {
                    DependencyProviderExtensions.GetManager<IMultiplayerManagerServer>(managers);
                }
            }

            this._mainProcessThread = new Thread(new ThreadStart(this.mainProcess))
            {
                Priority = ThreadPriority.BelowNormal
            };
            this._mainProcessThread.Start();
        }

        private void mainProcess()
        {
            if (UpdateMePlugin.Instance.Config.Enabled)
            {
                int updateInterval = 1000 * 60 * UpdateMePlugin.Instance.Config.CheckFrequencyInMinutes;
                Thread.Sleep(10000);
                Util.Log("UpdateMe started!");

                while (true)
                {
                    Thread.Sleep(updateInterval);
                    try
                    {
                        if (UpdateMePlugin.Instance.Config.Enabled)
                        {
                            if (!UpdateMePlugin.Instance._logic._processing)
                            {
                                UpdateMePlugin.Instance._logic._processing = true;
                                //Util.setUpAndRunGuardian(); //-->Not ready yet.

                                if (!Util.checkDSUpdatesByAppInfo())
                                    Util.checkTorchUpdatesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.Log(string.Concat("UpdateMe catched an exception!", ex, ex.StackTrace));
                    }
                    finally
                    {
                        UpdateMePlugin.Instance._logic._processing = false;
                    }
                }
            }
        }
    }
}
