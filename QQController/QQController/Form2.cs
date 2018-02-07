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

namespace QQController
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public static int RunningInstanceCount(out int id)
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            var sortList = processes.ToList();
            sortList.Sort(new CompareProcess());
            id = sortList.FindIndex(p => p.Id == current.Id) + 1;
            return processes.Length;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (this.InvokeRequired)
                    {
                        void ActionDelegate()
                        {
                            int total = RunningInstanceCount(out var id);
                            this.Text = $"登陆软件 普通版  已开启{total}个,当前编号{id}";
                        }

                        this.Invoke((Action) ActionDelegate);
                    }
                    else
                    {
                        int total = RunningInstanceCount(out var id);
                        this.Text = $"登陆软件 普通版  已开启{total}个,当前编号{id}";
                    }
                    Thread.Sleep(10000);
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void anchorQQTbx_Enter(object sender, EventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx.Text == "请输入QQ号来定位")
            {
                tbx.Text = "";
            }
        }

        private void anchorQQTbx_Leave(object sender, EventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx.Text == "")
            {
                tbx.Text = "请输入QQ号来定位";
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = new Form3().ShowDialog();
            if (result != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
            this.Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
        }
    }
    public class CompareProcess : IComparer<Process>
    {
        public int Compare(Process x, Process y)
        {
            return x.Id - y.Id;
        }
    }
}
