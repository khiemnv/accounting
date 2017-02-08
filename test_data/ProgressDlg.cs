using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_data
{
    public partial class ProgressDlg : Form
    {
        public object m_param;
        public Int64 m_maxRowid;
        public Int64 m_scale;
        public string m_descr;
        public myCursor m_cursor;
        public bool m_isCancel = false;
        enum state
        {
            init,
            completed,
            canceled,
            closed,
        };
        state m_state = state.init;

        Thread m_task;
        Int64 m_nStep;

        class myMutex
        {
            Mutex m_mutex;
            public myMutex()
            {
                m_mutex = new Mutex();
            }
            public void enter()
            {
                Debug.WriteLine("{0}.enter {1}", this, Thread.CurrentThread.ManagedThreadId);
                m_mutex.WaitOne();
                Debug.WriteLine("+ enter done");
            }
            public void leave()
            {
                Debug.WriteLine("{0}.leave {1}", this, Thread.CurrentThread.ManagedThreadId);
                m_mutex.ReleaseMutex();
                Debug.WriteLine("+ leave done");
            }
        }

        public ProgressDlg()
        {
            InitializeComponent();

            m_cancelBtn.Click += M_cancelBtn_Click;

            this.Load += ProgressDlg_Load;
        }

        void cancel()
        {
            m_isCancel = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            Debug.WriteLine("{0}.OnClosed cur thread {1}", this, Thread.CurrentThread.ManagedThreadId);
            if (m_state == state.init)
            {
                m_state = state.closed;
                cancel();
                m_task.Abort();
            }
        }

        private void M_cancelBtn_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("{0}.M_cancelBtn_Click cur thread {1}", this, Thread.CurrentThread.ManagedThreadId);
            m_state = state.canceled;
            cancel();
        }

        private void ProgressDlg_Load(object sender, EventArgs e)
        {
            m_nStep = m_maxRowid / m_scale;
            m_prg.Maximum = (int)m_nStep;
            m_prg.Value = 0;
            IAsyncResult t = (IAsyncResult)m_param;
            m_task = new Thread(new ThreadStart(() =>
            {
                //bool bLoop = true;
                for (int i = 1; !m_isCancel; i++)
                {
                    if (t.IsCompleted)
                    {
                        m_state = state.completed;
                        break;
                    }
                    Int64 cur = m_cursor.getPos() / m_scale;
#if false
                    if (m_isCancel)
                    {
                        bLoop = false;
                    }
                    else
                    {
                        lIncProgress d = new lIncProgress(this.incPrgCallback);
                        this.Invoke(d, new object[] { (int)cur, i / 10 });
                    }
#else
                    incPrgCallback((int)cur, i / 10);
#endif
                    Thread.Sleep(100);
                    Debug.WriteLine("{0}.m_task cur thread {1} elapsed {2} s",
                        this, 
                        Thread.CurrentThread.ManagedThreadId,
                        i / 10);
                }
                closeDlgCallback();
            }));
            m_task.Start();
        }

        //protected override void OnClose(EventArgs e)
        //{
        //    m_isCancel = true;
        //}

        delegate void lCloseDlg();
        void closeDlgCallback()
        {
            Debug.WriteLine("{0}.closeDlgCallback cur thread {1}", this,
                Thread.CurrentThread.ManagedThreadId);
            if (this.m_prg.InvokeRequired)
            {
                Debug.WriteLine("+ invoked req");
                //call it self in form thread
                lCloseDlg d = new lCloseDlg(this.closeDlgCallback);
                this.Invoke(d);
            }
            else
            {
                Debug.WriteLine("+ close");
                this.Close();
            }
        }

        delegate void lIncProgress(int iStep, int elapsed);
        void incPrgCallback(int iStep, int elapsed)
        {
            Debug.WriteLine("{0}.incPrgCallback cur thread {1}", this, Thread.CurrentThread.ManagedThreadId);
            if (this.m_prg.InvokeRequired)
            {
                Debug.WriteLine("+ invoke req");
                //call it self in form thread
                lIncProgress d = new lIncProgress(this.incPrgCallback);
                this.Invoke(d, new object[] { iStep, elapsed });
            }
            else
            {
                Debug.WriteLine("+ method body {0} {1}", iStep, elapsed);
                this.m_prg.Value = iStep;
                double percent = (double)iStep / m_nStep;
                m_percentTxt.Text = percent.ToString("0.00%");
                m_stepTxt.Text = string.Format("{0}/{1}", iStep, m_nStep);
                m_descrTxt.Text = string.Format("{0} \n  Elapsed {1} s", m_descr, elapsed);
            }
        }
    }
}
