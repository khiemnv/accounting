using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_binding
{
    public delegate void taskCallback1(object data);
    public delegate void taskCallback0();
    public class myTask
    {
        public string sender;
        public string receiver;
        public int iType;
        public object data;
    }
    public class BgTask : myTask
    {
        public enum bgTaskType
        {
            bgExec = 0x00,      //execute callback no param
            DP_BG_SEARCH,       //<MOD>_BG_<TASK>
        }
        public bgTaskType eType { get { return (bgTaskType)iType; } set{ iType = (int)value; } }
    }
    public class FgTask : myTask
    {
        public object m_owner;
        public enum fgTaskType
        {
            fgExec = 0x10000,
            DP_BG_UPDATESTS
        }
        public fgTaskType eType
        {
            get { return (fgTaskType)iType; }
            set { iType = (int)value; }
        }
        public int percent;
    }
    public class myWorker
    {
        BackgroundWorker m_worker;
        Queue<myTask> m_msgQueue;
        private myWorker()
        {
            m_msgQueue = new Queue<myTask>();
            m_worker = new BackgroundWorker();
            m_worker.DoWork += msgLoop;
            m_worker.ProgressChanged += onProgressChanged;
            m_worker.RunWorkerCompleted += onCompleted;
            m_worker.WorkerReportsProgress = true;
        }

        public static myWorker s_worker;
        public static myWorker getWorker()
        {
            if (s_worker == null)
            {
                s_worker = new myWorker();
            }
            return s_worker;
        }

        private void onCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void onProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Debug.WriteLine("onProgressChanged" + e.UserState.ToString());
            OnFgProcess((FgTask)e.UserState);
        }

        private void msgLoop(object sender, DoWorkEventArgs e)
        {
            uint i = 0;
            for (; ; i++)
            {
                if (m_msgQueue.Count > 0)
                {
                    var msg = m_msgQueue.Dequeue();
                    
                    var bgTsk = msg as BgTask;
                    if (bgTsk != null)
                        OnBgProcess(bgTsk);

                    var fgTsk = msg as FgTask;
                    if (fgTsk != null)
                        onProgressChanged(this, new ProgressChangedEventArgs(1, fgTsk));
                }
                else
                {
                    Debug.WriteLine("M_srchWorker_DoWork fall sleep, i = {0}", i);
                    //sleep(1000);
                    break;
                }
            }
        }

        bool m_WorkerIsRunning { get { return m_worker.IsBusy; } }
        public void qryBgTask(BgTask task)
        {
            Debug.WriteLine("{0} qryBgTask {1}", this, task.iType);
            m_msgQueue.Enqueue(task);
            if (!m_WorkerIsRunning)
            {
                m_worker.RunWorkerAsync();
            }
        }
        public void qryFgTask(FgTask task)
        {
            Debug.WriteLine("{0} qryFgTask {1}", this, task.iType);
            m_msgQueue.Enqueue(task);
            if (!m_WorkerIsRunning)
            {
                m_worker.RunWorkerAsync();
            }
        }

        public class FillTableCompletedEventArgs : EventArgs
        {
            public Int64 Sum { get; set; }
            public DateTime TimeComplete { get; set; }
        }
        static Dictionary<string, EventHandler<myTask>> m_dict =
            new Dictionary<string, EventHandler<myTask>>();
        private void addEvent(string zType, ref EventHandler<myTask> handler, EventHandler<myTask> value)
        {
            string key = zType + value.Target.ToString();
            if (!m_dict.ContainsKey(key))
            {
                m_dict.Add(key, value);
                handler += value;
            }
            else
            {
                handler -= m_dict[key];
                m_dict[key] = value;
                handler += value;
            }
        }
        private EventHandler<myTask> mBgProcess;
        public event EventHandler<myTask> BgProcess
        {
            add { addEvent("BgProcess", ref mBgProcess, value); }
            remove { }
        }
        private EventHandler<myTask> mFgProcess;
        public event EventHandler<myTask> FgProcess
        {
            add { addEvent("FgProcess", ref mFgProcess, value); }
            remove { }
        }

        protected virtual void OnBgProcess(BgTask task)
        {
            Debug.WriteLine("onProgressChanged" + task.eType.ToString());
            //BgProcess?.Invoke(this, task);
            if (mBgProcess != null) mBgProcess.Invoke(this, task);
        }
        protected virtual void OnFgProcess(FgTask task)
        {
            //FgProcess?.Invoke(this, task);
            if (mFgProcess != null) mFgProcess.Invoke(task.m_owner, task);
        }

        //sleep ?ms
        public static void sleep(int timeout)
        {
            var t = Task.Run(() => Task.Delay(timeout));
            t.Wait();
        }
    }
}
