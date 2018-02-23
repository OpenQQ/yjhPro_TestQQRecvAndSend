using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQController.BLL;
using QQController.Common;
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
            CheckForIllegalCrossThreadCalls = false;
        }

        private QQAcountManageService manageService;

        private Form4 dialog;

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
            // 启动服务
            new Thread(() =>
            {
                var server = new SocketServer();
                server.StartServer(this);
            })
            {
                IsBackground = true
            }.Start();
            // 刷新状态
            new Thread(() =>
            {
                while (true)
                {
                    fresh();
                    Thread.Sleep(1000);
                }
            })
            {
                IsBackground = true
            }.Start();
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
                if (item.SubItems[1].Text == id.ToString())
                {
                    item.SubItems[4].Text = statusDesc;
                    break;
                }
            }
        }

        public void UpdateStatusDesc(string qq, string statusDesc)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[2].Text == qq)
                {
                    item.SubItems[4].Text = statusDesc;
                    if (statusDesc == "登陆成功")
                    {
                        manageService.UpdateQQIsLogin(Convert.ToInt32(item.SubItems[1].Text), true);
                    }
                    if (statusDesc == "掉线")
                    {
                        manageService.UpdateQQIsLogin(Convert.ToInt32(item.SubItems[1].Text), false);
                    }
                    break;
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
            else
            {
                foreach (var p in Process.GetProcessesByName("CQA"))
                {
                    p.Kill();
                    p.Close();
                }

                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems[4].Text == "登陆成功")
                    {
                        manageService.UpdateQQIsLogin(Convert.ToInt32(item.SubItems[1].Text), false);
                    }
                }
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Thread.Sleep(2000);
                System.Environment.Exit(0);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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

        private void loginBtn_Click(object sender, EventArgs e)
        {
            loginBtn.Enabled = false;
            var checkedItems = listView1.CheckedItems;
            List<QQAccountViewModel> list = new List<QQAccountViewModel>();
            foreach (ListViewItem item in checkedItems)
            {
                if (item.SubItems[4].Text != "登陆成功" && item.SubItems[4].Text != "已登陆")
                {
                    list.Add(new QQAccountViewModel()
                    {
                        Password = item.SubItems[3].Text,
                        QQAccount = item.SubItems[2].Text
                    });
                }
            }

            if (list.Count <= 0)
            {
                loginBtn.Enabled = true;
                MessageBox.Show("未选择账号", "消息");
                return;
            }
            new Thread(() =>
            {
                foreach (var item in list)
                {
                    Global.CreateQQProcess(item.QQAccount);
                    Global.LoginQQ(item.QQAccount, item.Password, this);
                }
                this.Activate();

                dialog.infoLab.Text = "所有操作完成！！！";
                if (!dialog.Visible)
                {
                    dialog.ShowDialog();
                }
                Thread.Sleep(2000);
                dialog.Close();
                loginBtn.Enabled = true;
            })
            { IsBackground = true}.Start();
            dialog = new Form4();
            dialog.infoLab.Text = "开始登陆操作。。。";
            dialog.ShowDialog();
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void 全部勾选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = true;
            }
        }

        private void 全部不选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = false;
            }
        }

        private void 从此目标开始向上勾选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flag = true;
            if (listView1.SelectedItems.Count > 0)
            {
                var selected = listView1.SelectedItems[0];
                foreach (ListViewItem item in listView1.Items)
                {
                    if (flag)
                    {
                        item.Checked = true;
                        if (item.SubItems[1].Text == selected.SubItems[1].Text)
                        {
                            flag = false;
                            item.Checked = true;
                        }
                    }
                    else
                    {
                        item.Checked = false;
                    }
                }
            }
            
        }

        private void 从此目标开始向下勾选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if (listView1.SelectedItems.Count > 0)
            {
                var selected = listView1.SelectedItems[0];
                foreach (ListViewItem item in listView1.Items)
                {
                    if (!flag)
                    {
                        item.Checked = false;
                        if (item.SubItems[1].Text == selected.SubItems[1].Text)
                        {
                            flag = true;
                            item.Checked = true;
                        }
                    }
                    else
                    {
                        item.Checked = true;
                    }
                }
            }
            fresh();
        }

        private void 勾选同此目标状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selected = listView1.SelectedItems[0];
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems[4].Text == selected.SubItems[4].Text)
                    {
                        item.Checked = true;
                    }
                    else
                    {
                        item.Checked = false;
                    }
                }
            }
        }

        private void 删除选中ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.CheckedItems)
            {
                listView1.Items.Remove(item);
            }
        }

        private void fresh()
        {
            ChangeGet();
            ChangeFailed();
            ChangeLogin();
            ChangeSelect();
            ChangeDown();
        }
        private void ChangeGet()
        {
            getLab.Text = listView1.Items.Count.ToString();
        }

        private void ChangeSelect()
        {
            selectLab.Text = listView1.CheckedItems.Count.ToString();
        }

        private void ChangeLogin()
        {
            int count = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[4].Text == "登陆成功")
                {
                    count++;
                }
            }
            loginLab.Text = count.ToString();
        }

        private void ChangeFailed()
        {
            int count = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[4].Text != "登陆成功" && item.SubItems[4].Text != "已登陆" && item.SubItems[4].Text != "未登录")
                {
                    count++;
                }
            }
            failLab.Text = count.ToString();
        }
        private void ChangeDown()
        {
            int count = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[4].Text == "掉线")
                {
                    count++;
                }
            }
            offLineLab.Text = count.ToString();
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            string tmp = "";
            string[] fileAllLines = File.ReadAllLines("log.txt");
            for (int i = 0; i < 100 && i < fileAllLines.Length; i++)
            {
                if (fileAllLines.Length < 100)
                {
                    tmp += fileAllLines[i] + "\r\n";
                }
                else
                {
                    tmp += fileAllLines[fileAllLines.Length - 100 + i] + "\r\n";
                }
            }

            logTbx.Text = tmp;
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
