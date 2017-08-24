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
    public partial class FrmAddOrEdit : Form
    {
        public string sName = "";
        public string sIp = "";
        public string sSubmmask = "";
        public string sGetway = "";
        public string sDns = "";
        public string sRemark = "";
        public FrmAddOrEdit()
        {
            InitializeComponent();
        }

        public FrmAddOrEdit(string name,string ip,string submmask, string getway,string dns,string remark)
        {
            InitializeComponent();
            this.txtName.Text = name;
            this.txtIP.Text = ip;
            this.txtSubmask.Text = submmask;
            this.txtGetway.Text = getway;
            this.txtDNS.Text = dns;
            this.txtRemark.Text = remark;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtIP.Text == "" || txtGetway.Text == "" || txtSubmask.Text == "")
            {
                MessageBox.Show("设置错误！");
                return;
            }
            sName = txtName.Text;
            sIp = txtIP.Text;
            sSubmmask = txtSubmask.Text;
            sGetway = txtGetway.Text;
            sDns = txtDNS.Text;
            sRemark = txtRemark.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
