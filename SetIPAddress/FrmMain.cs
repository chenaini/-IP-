using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SetIPAddress
{
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        string ipfile = "";
        string remotefile = "";
        public FrmMain()
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

        private void LoadDataRemote()
        {
            if (File.Exists(ipfile))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(remotefile, XmlReadMode.ReadSchema);
                gridControl2.DataSource = ds.Tables[0].DefaultView;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            ipfile = Application.StartupPath + "\\IP.txt";
            remotefile = Application.StartupPath + "\\RemoteFile.txt";
            LoadData();
            LoadDataRemote();
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
            if(gridView1.RowCount>0)
            {
                gridControl1_Click(null, null);
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
        

        private static void rdpProfile(string filename, string address, string username, string password, string colordepth)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            using (StreamWriter streamWriter = new StreamWriter(filename, true))
            {
                streamWriter.WriteLine("screen mode id:i:2");
                streamWriter.WriteLine("use multimon:i:0");
                streamWriter.WriteLine("desktopwidth:i:1920");
                streamWriter.WriteLine("desktopheight:i:1080");
                streamWriter.WriteLine("session bpp:i:" + colordepth);
                streamWriter.WriteLine("winposstr:s:0,3,0,0,800,600");
                streamWriter.WriteLine("compression:i:1");
                streamWriter.WriteLine("keyboardhook:i:2");
                streamWriter.WriteLine("audiocapturemode:i:0");
                streamWriter.WriteLine("videoplaybackmode:i:1");
                streamWriter.WriteLine("connection type:i:7");
                streamWriter.WriteLine("displayconnectionbar:i:1");
                streamWriter.WriteLine("disable wallpaper:i:0");
                streamWriter.WriteLine("allow font smoothing:i:0");
                streamWriter.WriteLine("allow desktop composition:i:0");
                streamWriter.WriteLine("disable full window drag:i:1");
                streamWriter.WriteLine("disable menu anims:i:1");
                streamWriter.WriteLine("disable themes:i:0");
                streamWriter.WriteLine("disable cursor setting:i:0");
                streamWriter.WriteLine("bitmapcachepersistenable:i:1");
                streamWriter.WriteLine("full address:s:" + address);
                streamWriter.WriteLine("audiomode:i:0");
                streamWriter.WriteLine("redirectprinters:i:1");
                streamWriter.WriteLine("redirectcomports:i:0");
                streamWriter.WriteLine("redirectsmartcards:i:1");
                streamWriter.WriteLine("redirectclipboard:i:1");
                streamWriter.WriteLine("redirectposdevices:i:0");
                streamWriter.WriteLine("redirectdirectx:i:1");
                streamWriter.WriteLine("drivestoredirect:s:");
                streamWriter.WriteLine("autoreconnection enabled:i:1");
                streamWriter.WriteLine("authentication level:i:2");
                streamWriter.WriteLine("prompt for credentials:i:0");
                streamWriter.WriteLine("negotiate security layer:i:1");
                streamWriter.WriteLine("remoteapplicationmode:i:0");
                streamWriter.WriteLine("alternate shell:s:");
                streamWriter.WriteLine("shell working directory:s:");
                streamWriter.WriteLine("gatewayhostname:s:");
                streamWriter.WriteLine("gatewayusagemethod:i:4");
                streamWriter.WriteLine("gatewaycredentialssource:i:4");
                streamWriter.WriteLine("gatewayprofileusagemethod:i:0");
                streamWriter.WriteLine("promptcredentialonce:i:1");
                streamWriter.WriteLine("use redirection server name:i:0");
                streamWriter.WriteLine("use multimon:i:0");
                if (!string.IsNullOrEmpty(username))
                {
                    streamWriter.WriteLine("username:s:" + username);
                }
                if (!string.IsNullOrEmpty(password))
                {
                    streamWriter.WriteLine("password 51:b:" + Encrypt(password));
                }
            }
        }

        private static string Encrypt(string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            DATA_BLOB dATA_BLOB = default(DATA_BLOB);
            DATA_BLOB dATA_BLOB2 = default(DATA_BLOB);
            DATA_BLOB dATA_BLOB3 = default(DATA_BLOB);
            dATA_BLOB.cbData = bytes.Length;
            dATA_BLOB.pbData = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, dATA_BLOB.pbData, bytes.Length);
            dATA_BLOB3.cbData = 0;
            dATA_BLOB3.pbData = IntPtr.Zero;
            dATA_BLOB2.cbData = 0;
            dATA_BLOB2.pbData = IntPtr.Zero;
            CRYPTPROTECT_PROMPTSTRUCT cRYPTPROTECT_PROMPTSTRUCT = new CRYPTPROTECT_PROMPTSTRUCT
            {
                cbSize = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT)),
                dwPromptFlags = 0,
                hwndApp = IntPtr.Zero,
                szPrompt = null
            };
            if (CryptProtectData(ref dATA_BLOB, "psw", ref dATA_BLOB3, IntPtr.Zero, ref cRYPTPROTECT_PROMPTSTRUCT, 1, ref dATA_BLOB2))
            {
                if (IntPtr.Zero != dATA_BLOB.pbData)
                {
                    Marshal.FreeHGlobal(dATA_BLOB.pbData);
                }
                if (IntPtr.Zero != dATA_BLOB3.pbData)
                {
                    Marshal.FreeHGlobal(dATA_BLOB3.pbData);
                }
                byte[] array = new byte[dATA_BLOB2.cbData];
                Marshal.Copy(dATA_BLOB2.pbData, array, 0, dATA_BLOB2.cbData);
                return BitConverter.ToString(array).Replace("-", string.Empty);
            }
            return string.Empty;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DATA_BLOB
        {
            public int cbData;

            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize;

            public int dwPromptFlags;

            public IntPtr hwndApp;

            public string szPrompt;
        }
        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CryptProtectData(ref DATA_BLOB pDataIn, string szDataDescr, ref DATA_BLOB pOptionalEntropy, IntPtr pvReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            FrmRemoteAddOrEdit frm = new FrmRemoteAddOrEdit();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(remotefile))
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(remotefile, XmlReadMode.ReadSchema);
                    DataRow row = ds.Tables[0].NewRow();
                    row["Id"] = Guid.NewGuid().ToString();
                    row["Name"] = frm.sName;
                    row["ServerIP"] = frm.sServerIP;
                    row["ServerPort"] = frm.sServerPort;
                    row["ServerUserName"] = frm.sServerUserName;
                    row["ServerPwd"] = frm.sServerPwd;
                    row["ServerRemark"] = frm.sServerRemark;
                    ds.Tables[0].Rows.Add(row);
                    ds.WriteXml(remotefile, XmlWriteMode.WriteSchema);
                    LoadDataRemote();
                }
                else
                {
                    DataSet ds = new DataSet();
                    DataTable table = new DataTable();
                    table.Columns.Add("Id");
                    table.Columns.Add("Name");
                    table.Columns.Add("ServerIP");
                    table.Columns.Add("ServerPort");
                    table.Columns.Add("ServerUserName");
                    table.Columns.Add("ServerPwd");
                    table.Columns.Add("ServerRemark");

                    DataRow row = table.NewRow();
                    row["Id"] = Guid.NewGuid().ToString();
                    row["Name"] = frm.sName;
                    row["ServerIP"] = frm.sServerIP;
                    row["ServerPort"] = frm.sServerPort;
                    row["ServerUserName"] = frm.sServerUserName;
                    row["ServerPwd"] = frm.sServerPwd;
                    row["ServerRemark"] = frm.sServerRemark;
                    table.Rows.Add(row);
                    ds.Tables.Add(table);
                    ds.WriteXml(remotefile, XmlWriteMode.WriteSchema);
                    LoadDataRemote();
                }
            }
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            if (gridView2.FocusedRowHandle > -1)
            {
                FrmRemoteAddOrEdit frm = new FrmRemoteAddOrEdit(gridView2.GetFocusedRowCellValue("Name").ToString(),
                    gridView2.GetFocusedRowCellValue("ServerIP").ToString(),
                    gridView2.GetFocusedRowCellValue("ServerPort").ToString(),
                    gridView2.GetFocusedRowCellValue("ServerUserName").ToString(),
                    gridView2.GetFocusedRowCellValue("ServerPwd").ToString(),
                    gridView2.GetFocusedRowCellValue("ServerRemark").ToString());
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(remotefile))
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(remotefile, XmlReadMode.ReadSchema);
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["Id"].ToString() == gridView2.GetFocusedRowCellValue("Id").ToString())
                            {
                                ds.Tables[0].Rows[i]["Name"] = frm.sName;
                                ds.Tables[0].Rows[i]["ServerIP"] = frm.sServerIP;
                                ds.Tables[0].Rows[i]["ServerPort"] = frm.sServerPort;
                                ds.Tables[0].Rows[i]["ServerUserName"] = frm.sServerUserName;
                                ds.Tables[0].Rows[i]["ServerPwd"] = frm.sServerPwd;
                                ds.Tables[0].Rows[i]["ServerRemark"] = frm.sServerRemark;
                                ds.WriteXml(remotefile, XmlWriteMode.WriteSchema);
                                LoadDataRemote();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            if (gridView2.FocusedRowHandle > -1)
            {
                if (MessageBox.Show("你确认要删除该信息吗？", "提示！", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(remotefile, XmlReadMode.ReadSchema);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i]["Id"].ToString() == gridView2.GetFocusedRowCellValue("Id").ToString())
                        {
                            ds.Tables[0].Rows.RemoveAt(i);
                            ds.WriteXml(remotefile, XmlWriteMode.WriteSchema);
                            LoadDataRemote();
                            break;
                        }
                    }
                }
            }
        }

        private void gridControl2_DoubleClick(object sender, EventArgs e)
        {
            if(gridView2.FocusedRowHandle>-1)
            {
                try
                {
                    if(Directory.Exists(Application.StartupPath + "/RDP")==false)
                    {
                        Directory.CreateDirectory(Application.StartupPath + "/RDP");
                    }
                    rdpProfile(Application.StartupPath+"/RDP/"+gridView2.GetFocusedRowCellValue("Name").ToString()+".rdp",
                        gridView2.GetFocusedRowCellValue("ServerIP").ToString()+":"+
                        gridView2.GetFocusedRowCellValue("ServerPort").ToString(),
                        gridView2.GetFocusedRowCellValue("ServerUserName").ToString(),
                        gridView2.GetFocusedRowCellValue("ServerPwd").ToString(), "32");

                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    string strOutput = string.Format("mstsc " + Application.StartupPath + "/RDP/"+ gridView2.GetFocusedRowCellValue("Name").ToString() + ".rdp");
                    p.StandardInput.WriteLine(strOutput);
                    p.StandardInput.WriteLine("exit");
                    while (p.StandardOutput.EndOfStream)
                    {
                        strOutput = p.StandardOutput.ReadLine();
                    }
                    p.WaitForExit();
                    p.Close();

                    
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
    }
}
