using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQController.BLL;

namespace QQController
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseCapture();

        private LoginService m_LoginService;

        public Form1()
        {
            InitializeComponent();
            m_LoginService = new LoginService();
        }
        /// <summary>
        /// 登陆
        /// </summary>
        private bool Login()
        {
            var account = this.accountTbx.Text.Trim();
            var password = this.pwdTbx.Text.Trim();
            if (string.IsNullOrEmpty(account)|| account=="请输入账号")
            {
                MessageBox.Show("账号不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(password)|| password == "请输入密码")
            {
                MessageBox.Show("密码不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (m_LoginService.CheckAccount(account, password))
            {
                return true;
            }
            else
            {
                MessageBox.Show("账号或密码错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Type(this, 25, 0.1);
        }
        private void Type(Control sender, int p_1, double p_2)
        {
            GraphicsPath oPath = new GraphicsPath();
            oPath.AddClosedCurve(new Point[] {
                new Point(0, sender.Height / p_1),
                new Point(sender.Width / p_1, 0),
                new Point(sender.Width - sender.Width / p_1, 0),
                new Point(sender.Width, sender.Height / p_1),
                new Point(sender.Width, sender.Height - sender.Height / p_1),
                new Point(sender.Width - sender.Width / p_1, sender.Height),
                new Point(sender.Width / p_1, sender.Height),
                new Point(0, sender.Height - sender.Height / p_1) }, (float)p_2);
            sender.Region = new Region(oPath);
        }

        private void BeautiLoginForm_Resize(object sender, EventArgs e)
        {
            Type(this, 25, 0.1);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 2;

            if (e.Button == MouseButtons.Left)  // 按下的是鼠标左键  
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);    // 拖动窗体   
            }
        }

        private void accountTbx_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == 13)
            {
                // TODO：登陆操作
                if (Login())
                {
                    new Form2().Show();
                    this.Hide();
                }
                e.SuppressKeyPress = true;
            }
        }

        private void pwdTbx_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == 13)
            {
                // TODO：登陆操作
                if (Login())
                {
                    new Form2().Show();
                    this.Hide();
                }
                e.SuppressKeyPress = true;
            }
        }

        private void accountTbx_Enter(object sender, EventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx.Text == "请输入账号")
            {
                tbx.Text = "";
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            loginBtn.Focus();
        }

        private void accountTbx_Leave(object sender, EventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx.Text == "")
            {
                tbx.Text = "请输入账号";
            }
        }

        private void pwdTbx_Enter(object sender, EventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx.Text == "请输入密码")
            {
                tbx.Text = "";
            }
        }

        private void pwdTbx_Leave(object sender, EventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx.Text == "")
            {
                tbx.Text = "请输入密码";
            }
        }

        private void Draw(Rectangle rectangle, Graphics g, int _radius, bool cusp, Color begin_color, Color end_color)
        {
            int span = 2;
            //抗锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //渐变填充
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush(rectangle, begin_color, end_color, LinearGradientMode.Vertical);
            //画尖角
            if (cusp)
            {
                span = 10;
                PointF p1 = new PointF(rectangle.Width - 12, rectangle.Y + 10);
                PointF p2 = new PointF(rectangle.Width - 12, rectangle.Y + 30);
                PointF p3 = new PointF(rectangle.Width, rectangle.Y + 20);
                PointF[] ptsArray = { p1, p2, p3 };
                g.FillPolygon(myLinearGradientBrush, ptsArray);
            }
            //填充
            g.FillPath(myLinearGradientBrush, DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - span, rectangle.Height - 1, _radius));
        }
        public static GraphicsPath DrawRoundRect(int x, int y, int width, int height, int radius)
        {
            //四边圆角
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(x, y, radius, radius, 180, 90);
            gp.AddArc(width - radius, y, radius, radius, 270, 90);
            gp.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            gp.AddArc(x, height - radius, radius, radius, 90, 90);
            gp.CloseAllFigures();
            return gp;
        }

        private void loginBtn_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(255, 255, 255), Color.FromArgb(192, 192, 192));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("登陆", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(23, 6));
        }

        private void exitBtn_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.ClipRectangle, e.Graphics, 18, false, Color.FromArgb(255, 255, 255), Color.FromArgb(192, 192, 192));
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawString("退出", new Font("微软雅黑", 9, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(23, 6));
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            if (Login())
            {
                new Form2().Show();
                this.Hide();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
