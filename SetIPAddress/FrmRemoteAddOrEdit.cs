using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SetIPAddress
{
    public partial class FrmRemoteAddOrEdit : DevExpress.XtraEditors.XtraForm
    {
        public string sName = "";
        public string sServerIP = "";
        public string sServerPort = "";
        public string sServerUserName = "";
        public string sServerPwd = "";
        public string sServerRemark = "";

        public FrmRemoteAddOrEdit()
        {
            InitializeComponent();
        }

        public FrmRemoteAddOrEdit(string name,string ip,string port,string username,string pwd,string remark)
        {
            InitializeComponent();
            this.txtIP.Text = ip;
            this.txtName.Text = name;
            this.txtPort.Text = port;
            this.txtPwd.Text = pwd;
            this.txtRemark.Text = remark;
            this.txtUserName.Text = username;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtIP.Text == "" || txtPort.Text == "" || txtUserName.Text == "")
            {
                MessageBox.Show("设置错误！");
                return;
            }
            sServerIP = txtIP.Text;
            sName = txtName.Text;
            sServerPort = txtPort.Text;
            sServerPwd = txtPwd.Text;
            sServerRemark = txtRemark.Text;
            sServerUserName = txtUserName.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
