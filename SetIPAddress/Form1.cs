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
using System.Xml;

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
        

        private void Form1_Load(object sender, EventArgs e)
        {
            ipfile = Application.StartupPath + "\\IP.txt";
            LoadData();
            string networkName = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["NetConnectionID"] != null)
                {
                    txtNetwork.Properties.Items.Add(mo["NetConnectionID"].ToString());
                    if("Intel(R) 82579LM Gigabit Network Connection"== mo["Name"].ToString())
                    {
                        networkName = mo["NetConnectionID"].ToString();
                    }
                }
            }
            if (txtNetwork.Properties.Items.Count > 0)
            {
                txtNetwork.Text = networkName;
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

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            FrmAddOrEdit frm = new FrmAddOrEdit();
            if(frm.ShowDialog()== DialogResult.OK)
            {
                if (File.Exists(ipfile))
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(ipfile, XmlReadMode.ReadSchema);
                    DataRow row = ds.Tables[0].NewRow();
                    row["Id"] = Guid.NewGuid().ToString();
                    row["Name"] = frm.sName;
                    row["IP"] = frm.sIp;
                    row["Getway"] = frm.sGetway;
                    row["Submask"] = frm.sSubmmask;
                    row["DNS"] = frm.sDns;
                    row["Remark"] = frm.sRemark;
                    ds.Tables[0].Rows.Add(row);
                    ds.WriteXml(ipfile, XmlWriteMode.WriteSchema);
                    LoadData();
                }
                else
                {
                    DataSet ds = new DataSet();
                    DataTable table = new DataTable();
                    table.Columns.Add("Id");
                    table.Columns.Add("Name");
                    table.Columns.Add("IP");
                    table.Columns.Add("Getway");
                    table.Columns.Add("Submask");
                    table.Columns.Add("DNS");
                    table.Columns.Add("Remark");

                    DataRow row = table.NewRow();
                    row["Id"] = Guid.NewGuid().ToString();
                    row["Name"] = frm.sName;
                    row["IP"] = frm.sIp;
                    row["Getway"] = frm.sGetway;
                    row["Submask"] = frm.sSubmmask;
                    row["DNS"] = frm.sDns;
                    row["Remark"] = frm.sRemark;
                    table.Rows.Add(row);
                    ds.Tables.Add(table);
                    ds.WriteXml(ipfile, XmlWriteMode.WriteSchema);
                    LoadData();
                }
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if(gridView1.FocusedRowHandle>-1)
            {
                FrmAddOrEdit frm = new FrmAddOrEdit(gridView1.GetFocusedRowCellValue("Name").ToString(),
                    gridView1.GetFocusedRowCellValue("IP").ToString(),
                    gridView1.GetFocusedRowCellValue("Submask").ToString(),
                    gridView1.GetFocusedRowCellValue("Getway").ToString(),
                    gridView1.GetFocusedRowCellValue("DNS").ToString(),
                    gridView1.GetFocusedRowCellValue("Remark").ToString());
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(ipfile))
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(ipfile, XmlReadMode.ReadSchema);
                        for(int i=0;i<ds.Tables[0].Rows.Count;i++)
                        {
                            if(ds.Tables[0].Rows[i]["Id"].ToString()== gridView1.GetFocusedRowCellValue("Id").ToString())
                            {
                                ds.Tables[0].Rows[i]["Name"] = frm.sName;
                                ds.Tables[0].Rows[i]["IP"] = frm.sIp;
                                ds.Tables[0].Rows[i]["Getway"] = frm.sGetway;
                                ds.Tables[0].Rows[i]["Submask"] = frm.sSubmmask;
                                ds.Tables[0].Rows[i]["DNS"] = frm.sDns;
                                ds.Tables[0].Rows[i]["Remark"] = frm.sRemark;
                                ds.WriteXml(ipfile, XmlWriteMode.WriteSchema);
                                LoadData();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle > -1)
            {
                if(MessageBox.Show("你确认要删除该信息吗？","提示！",MessageBoxButtons.YesNo)== DialogResult.Yes)
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(ipfile, XmlReadMode.ReadSchema);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i]["Id"].ToString() == gridView1.GetFocusedRowCellValue("Id").ToString())
                        {
                            ds.Tables[0].Rows.RemoveAt(i);
                            ds.WriteXml(ipfile, XmlWriteMode.WriteSchema);
                            LoadData();
                            break;
                        }
                    }
                }
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            if(gridView1.FocusedRowHandle>-1)
            {
                txtName.Text = gridView1.GetFocusedRowCellValue("Name").ToString();
                txtIP.Text = gridView1.GetFocusedRowCellValue("IP").ToString();
                txtSubmask.Text = gridView1.GetFocusedRowCellValue("Submask").ToString();
                txtGetway.Text = gridView1.GetFocusedRowCellValue("Getway").ToString();
                txtDNS.Text = gridView1.GetFocusedRowCellValue("DNS").ToString();
                txtRemark.Text = gridView1.GetFocusedRowCellValue("Remark").ToString();
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
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
    }
}
