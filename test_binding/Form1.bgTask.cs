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
        public int type;
        public object data;
    }
    public class BgTask : myTask
    {
        public enum bgTaskType
        {
            bgExec = 0x00,     //execute callback no param
        }
        public bgTaskType eType { get { return (bgTaskType)type; } set{ type = (int)value; } }
    }
    public class FgTask : myTask
    {
        public enum fgTaskType
        {
            fgExec = 0x100,
        }
        public fgTaskType eType
        {
            get { return (fgTaskType)type; }
            set { type = (int)value; }
        }
    }
    public class myWorker
    {
        BackgroundWorker m_worker;
        Queue<BgTask> m_msgQueue;
        public myWorker()
        {
            m_msgQueue = new Queue<BgTask>();
            m_worker = new BackgroundWorker();
            m_worker.DoWork += msgLoop;
            m_worker.ProgressChanged += onProgressChanged;
            m_worker.RunWorkerCompleted += onCompleted;
            m_worker.WorkerReportsProgress = true;
        }

        private void onCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void onProgressChanged(object sender, ProgressChangedEventArgs e)
        {
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
                    OnBgProcess(msg);
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
            Debug.WriteLine("{0} qryBgTask {1}", this, task.type);
            m_msgQueue.Enqueue(task);
            if (!m_WorkerIsRunning)
            {
                m_worker.RunWorkerAsync();
            }
        }
        public void qryFgTask(FgTask task)
        {
            m_worker.ReportProgress(1, task);
        }

        public event EventHandler<BgTask> BgProcess;
        public event EventHandler<FgTask> FgProcess;
        protected virtual void OnBgProcess(BgTask task)
        {
            //BgProcess?.Invoke(this, task);
            if (BgProcess != null) BgProcess.Invoke(this, task);
        }
        protected virtual void OnFgProcess(FgTask task)
        {
            //FgProcess?.Invoke(this, task);
            if (FgProcess != null) FgProcess.Invoke(this, task);
        }

        //sleep ?ms
        public static void sleep(int timeout)
        {
            var t = Task.Run(() => Task.Delay(timeout));
            t.Wait();
        }
    }
}
