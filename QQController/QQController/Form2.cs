using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQController.BLL;
using QQController.Entity.ViewModel;

namespace QQController
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            manageService = new QQAcountManageService();
            AirPath = ConfigurationManager.AppSettings.Get("AirPath");
        }

        private QQAcountManageService manageService;

        private int pageIndex = 1;

        private int pageSize = 10;

        private string AirPath;

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
            this.listView1.CheckBoxes = true;
            GetAccountViewModels();
            int i = 0;
            while (i < 100)
            {
                i++;
                manageService.AddQQ("1","1");
            }
        }

        public void GetAccountViewModels()
        {
            var list = manageService.GetAccountViewModels(pageIndex, pageSize);
            if (list.Count > 0)
            {
                pageIndex++;
                BindingData(list);
            }
            else
            {
                MessageBox.Show("没有QQ号了！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void BindingData(List<QQAccountViewModel> list)
        {
            foreach (var item in list)
            {
                ListViewItem list_item = new ListViewItem();
                list_item.SubItems.Add(item.ID.ToString());//xx为相应属性
                list_item.SubItems.Add(item.QQAccount.ToString());
                list_item.SubItems.Add(item.Password);
                list_item.SubItems.Add(item.StatusDesc);
                listView1.Items.Add(list_item);
            }
        }

        public void UpdateStatusDesc(int id, string statusDesc)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[0].Text == id.ToString())
                {
                    item.SubItems[3].Text = statusDesc;
                }
            }
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

        private void getBtn_Click(object sender, EventArgs e)
        {
            GetAccountViewModels();
        }

        private void anchorBtn_Click(object sender, EventArgs e)
        {
            string findQQ = anchorQQTbx.Text.Trim();
            if (findQQ == "请输入QQ号来定位")
            {
                MessageBox.Show("请输入QQ号", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems[2].Text == findQQ)
                    {
                        item.Selected = true;
                        item.Checked = true;
                        item.EnsureVisible();
                        return;
                    }
                }
                MessageBox.Show("找不到" + findQQ, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
