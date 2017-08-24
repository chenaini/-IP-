using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetIPAddress
{
    public partial class Form1 : Form
    {
        string ipfile = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            if (File.Exists(ipfile))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(ipfile, XmlReadMode.ReadSchema);
                gridControl1.DataSource = ds.Tables[0].DefaultView;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if(txtName.Text=="" || txtIP.Text=="" || txtGetway.Text=="" || txtSubmask.Text=="")
            {
                MessageBox.Show("设置错误！");
            }
            if(File.Exists(ipfile))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(ipfile, XmlReadMode.ReadSchema);
                DataRow row = ds.Tables[0].NewRow();
                row["Name"] = txtName.Text;
                row["IP"] = txtIP.Text;
                row["Getway"] = txtGetway.Text;
                row["Submask"] = txtSubmask.Text;
                row["DNS"] = txtDNS.Text;
                ds.Tables[0].Rows.Add(row);
                ds.WriteXml(ipfile, XmlWriteMode.WriteSchema);
                LoadData();
                txtDNS.Text = "";
                txtGetway.Text = "";
                txtDNS.Text = "";
                txtIP.Text = "";
                txtName.Text = "";
                txtSubmask.Text = "255.255.255.0";
            }
            else
            {
                DataSet ds = new DataSet();
                DataTable table = new DataTable();
                table.Columns.Add("Name");
                table.Columns.Add("IP");
                table.Columns.Add("Getway");
                table.Columns.Add("Submask");
                table.Columns.Add("DNS");

                DataRow row = table.NewRow();
                row["Name"] = txtName.Text;
                row["IP"] = txtIP.Text;
                row["Getway"] = txtGetway.Text;
                row["Submask"] = txtSubmask.Text;
                row["DNS"] = txtDNS.Text;
                table.Rows.Add(row);
                ds.Tables.Add(table);
                ds.WriteXml(ipfile, XmlWriteMode.WriteSchema);
                LoadData();
                txtDNS.Text = "";
                txtGetway.Text = "";
                txtDNS.Text = "";
                txtIP.Text = "";
                txtName.Text = "";
                txtSubmask.Text = "255.255.255.0";
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(gridView1.FocusedRowHandle>-1)
            {
                if (File.Exists(ipfile))
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(ipfile, XmlReadMode.ReadSchema);
                    ds.Tables[0].Rows.RemoveAt(gridView1.FocusedRowHandle);
                    ds.WriteXml(ipfile, XmlWriteMode.WriteSchema);
                    LoadData();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ipfile = Application.StartupPath + "\\IP.txt";
            LoadData();
            //ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //ManagementObjectCollection moc = wmi.GetInstances();
            //foreach (ManagementObject mo in moc)
            //{
            //    if(mo.Properties["MACAddress"].Value!=null && mo.Properties["MACAddress"].Value.ToString()!="")
            //    {
            //        txtNetwork.Properties.Items.Add(mo.Properties["Description"].Value.ToString());
            //    }
            //}
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["NetConnectionID"] != null)
                {
                    txtNetwork.Properties.Items.Add(mo["NetConnectionID"].ToString());
                }
            }
            if(txtNetwork.Properties.Items.Count>0)
            {
                txtNetwork.SelectedIndex = 0;
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            if (txtNetwork.Text == "")
            {
                MessageBox.Show("请选择网卡！");
                return;
            }
            if (gridView1.FocusedRowHandle > -1)
            {
                string _ipaddress = gridView1.GetFocusedRowCellValue("IP").ToString();
                string _submask = gridView1.GetFocusedRowCellValue("Submask").ToString();
                string _gateway = gridView1.GetFocusedRowCellValue("Getway").ToString();
                string _dns1 = gridView1.GetFocusedRowCellValue("DNS").ToString();
                //NetworkSetting.SetIPAddress(_ipaddress, _submask, _gateway);
                //if (_dns1 != "")
                //{
                //    NetworkSetting.SetDNS(new string[] { _dns1 });
                //}

                string _doscmd = "netsh interface ip set address " + txtNetwork.Text + " static " + _ipaddress + " " + _submask + " " + _gateway + " 2";
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardInput.WriteLine(_doscmd.ToString());
                _doscmd = "netsh interface ip set dns " + txtNetwork.Text + " static " + _dns1;
                p.StandardInput.WriteLine(_doscmd.ToString());

                p.StandardInput.WriteLine("exit");

                MessageBox.Show("设置完成！");
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //NetworkSetting.EnableDHCP();
            if(txtNetwork.Text=="")
            {
                MessageBox.Show("请选择网卡！");
                return;
            }
            string _doscmd = "netsh interface ip set address  " + txtNetwork.Text + " DHCP";
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(_doscmd.ToString());
            _doscmd = "netsh interface ip set dns  " + txtNetwork.Text + " DHCP";
            p.StandardInput.WriteLine(_doscmd.ToString());
            p.StandardInput.WriteLine("exit");


            MessageBox.Show("设置完成！");
        }

        private void txtNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            NetworkSetting.NetworkName = txtNetwork.Text;
        }


    }
}
