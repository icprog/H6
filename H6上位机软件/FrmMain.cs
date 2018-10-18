﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Splash.IO.PORTS;
using System.Management;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;

namespace H6
{
    public partial class FrmMain : Form
    {
        USB ezUSB = new USB();
        public static string AAA = string.Empty;
        public static IntPtr BCHandle = IntPtr.Zero;
        public static string IDCode = string.Empty; //执法仪识别码

                    //设置密码
        public static string DevicePassword = string.Empty;
            //设备初始化返回值
           // int Init_Device_iRet = -1;
            //IDCode = "";
            //执法仪电量读取返回值
        private string DestinFolder = string.Empty;//目的文件夹
        private System.Threading.Thread thdAddFile; //创建一个线程
        private string str = "";
        FileStream FormerOpen;//实例化FileStream类
        FileStream ToFileOpen;//实例化FileStream类


        public enum ServerType
        {
            CMSV6 = 0,
            GB281811,
            NetCheckServer
        }


        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;

        DeviceType LoginDevice;

        bool bRestart = false;


        /// <summary>
        /// 登陆的设备类型
        /// </summary>
        public enum DeviceType
        {
            NA,
            EasyStorage,
            Cammpro
        }

        /// <summary>
        /// 设备的分辨率
        /// </summary>
        public class DeviceResolution
        {
            public int Resolution_Width { get; set; }
            public int Resolution_Height {get;set;}
            public int Bps { get; set; }
            public int Fps {get;set;}
        }

        /// <summary>
        /// 设备的信息(序列号等等)
        /// </summary>
        public class DeviceInfo
        {
            public string cSerial { get; set; }
            public string userNo { get; set; }
            public string userName { get; set; }
            public string unitNo {get;set;}
            public string unitName { get; set; }

        }


        public enum WiFiModeType
        {
            AP,
            STA  
        }

        /// <summary>
        ///设备WiFi & server & port ect
        /// </summary>
        public class WiFiInfo
        {
            public  WiFiModeType WiFiMode { set; get; }
            public  string WiFiSSID { set; get; }
            public  string WiFiPassword { set; get; }
            public  string ServerIP { set; get; }
            public  string ServerPort { set; get; }
            public string APN { set; get; }
            public string PIN { set; get; }
        }



        /// <summary>
        /// Only Wifi
        /// </summary>
        public class WiFi
        {
            public WiFiModeType WiFiMode { set; get; }
            public string WiFiSSID { set; get; }
            public string WiFiPassword { set; get; }

        }


        /// <summary>
        /// APN
        /// </summary>
        public class APN
        {
            public string ApnName { set; get; }
            public string ApnPin { set; get; }
            public string ApnUser { set; get; }
            public string ApnPwd { set; get; }
        }


        /// <summary>
        ///  CMCSV6 Server
        /// </summary>
        public class CMCSV6Server
        {
            public string ServerIP { set; get; }
            public string ServerPort { set; get; }
            public int ReportTime { set; get; }
            public string DevNo{set;get;}
            public int Enable { set; get; }
        }


        /// <summary>
        /// GB28181
        /// </summary>
        public class GB28181Server
        {
            public string ServerIP { set; get; }
            public string ServerPort { set; get; }
            public int Enable { set; get; }
            public string DeviceID { set; get; }
            public string DevicePassword { set; get; }
            public string ChannelName { set; get; }
            public string ServerID { set; get; }
            public string ChannelID{set;get;}
            public string GPSIP { set; get; }
            public string GPSPort { set; get; }
        }


        /// <summary>
        /// NetCheckServer
        /// </summary>
        public class NetCheckServer
        {
            public string IP { set; get; }
            public string Port { set; get; }
        }

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //禁止Form窗口调整大小方法：
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //不能使用最大化窗口： 
            this.MaximizeBox = false;
            //this.Width = 940;
            //this.Height = 540;

            this.Location = new Point(
            (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
            (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
             );
            
            //MessageBox.Show(this.Width.ToString() + "X" + this.Height.ToString());

            //groupBox7
            //updateMessage(lb_StateInfo, "状态命令框" + gb_StatusCommand.Width.ToString()+"X"+gb_StatusCommand.Height.ToString());
            //updateMessage(lb_StateInfo, "无线信息" + gb_Wireless.Location.X.ToString() + "X" + gb_Wireless.Location.Y.ToString());

            //隐藏无线参数框
            txtApnUser.Visible = true;
            ////设定状态显示窗体大小
            //gb_StatusCommand.Width = 458;
            //gb_StatusCommand.Height = 475;
            ////设定状态显示窗口位置
            ////gb_StatusCommand.Location = new Point(453,11);
            //gb_StatusCommand.Location = new Point(453, 90);
            ////状态命令大小
            //lb_StateInfo.Width = 440;
            //lb_StateInfo.Height = 450;

            //LibBodycam.log
            //删除文件
            string FileName = Application.StartupPath + "\\" + "LibBodycam.log";
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            //不能使用最小化窗口： 
            this.MinimizeBox = false;
            //私版
            this.Text = "恒安警用执法记录仪管理软件,Ver:" + Application.ProductVersion; 
            //公版
            //this.Text = "执法仪上位机 V1.0.4";
            ezUSB.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 3));
            InitUI();
            //
            LoginDevice = DeviceType.NA;
            bRestart = false;

          
        }

        private void USBEventHandler(Object sender, EventArrivedEventArgs e)
        {
            /*

            this.SetText("" + "\r\n");
            this.SetText("*******************************************************" + "\r\n");
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                this.SetText("USB插入时间：" + DateTime.Now + "\r\n");
                //init();
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {
                this.SetText("USB拔出时间：" + DateTime.Now + "\r\n");
                //init();
            }

            foreach (USBControllerDevice Device in USB.WhoUSBControllerDevice(e))
            {
                this.SetText("\tAntecedent：" + Device.Antecedent + "\r\n");
                this.SetText("\tDependent：" + Device.Dependent + "\r\n");
                AAA += Device.Dependent + ";" + Device.Antecedent;
                //init();
            }


            */
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                //this.SetListText("USB设备插入时间：" + DateTime.Now + "\r\n");
                //init();
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {
                //this.SetListText("USB设备拔出时间：" + DateTime.Now + "\r\n");
                //init();
            }
            /*
            foreach (USBControllerDevice Device in USB.WhoUSBControllerDevice(e))
            {
                this.SetListText("\tAntecedent：" + Device.Antecedent + "\r\n");
                this.SetListText("\tDependent：" + Device.Dependent + "\r\n");
                AAA += Device.Dependent + ";" + Device.Antecedent;
                //init();
            }
            */
        }


        #region 更新信息
        /// <summary>
        /// 更新信息到listbox中
        /// </summary>
        /// <param name="listbox">listbox name</param>
        /// <param name="message">message</param>
        public static void updateMessage(ListBox listbox, string message)
        {
            if (listbox.Items.Count > 1000)
                listbox.Items.RemoveAt(0);

            string item = string.Empty;
            //listbox.Items.Add("");
            item = DateTime.Now.ToString("HH:mm:ss") + " " + @message;
            listbox.Items.Add(item);
            if (listbox.Items.Count > 1)
            {
                listbox.TopIndex = listbox.Items.Count - 1;
                listbox.SetSelected(listbox.Items.Count - 1, true);
            }
        }
        #endregion


        /// <summary>
        /// 延時子程序
        /// </summary>
        /// <param name="interval">延時的時間，单位毫秒</param>
        private void Delay(double interval)
        {
            DateTime time = DateTime.Now;
            double span = interval * 10000;
            while (DateTime.Now.Ticks - time.Ticks < span)
            {
                Application.DoEvents();
            }

        }





        // 对 Windows 窗体控件进行线程安全调用
        private void SetText(String text)
        {
            if (this.tb_DevID.InvokeRequired)
            {
                this.tb_DevID.BeginInvoke(new Action<String>((msg) =>
                {
                    this.tb_DevID.AppendText(msg);
                }), text);
            }
            else
            {
                this.tb_DevID.AppendText(text);
            }
        }


        // 对 Windows 窗体控件进行线程安全调用
        private void SetListText(String text)
        {
            if (this.lb_StateInfo.InvokeRequired)
            {
                this.lb_StateInfo.BeginInvoke(new Action<String>((msg) =>
                {
                    //this.tb_DevID.AppendText(msg);
                    this.lb_StateInfo.Items.Add(msg);
                }), text);
            }
            else
            {
                //this.tb_DevID.AppendText(text);
                this.lb_StateInfo.Items.Add(text);
            }
        }


        public delegate void AddFile();//定度委托
        /// <summary>
        /// 在线程上执行委托
        /// </summary>
        public void SetAddFile()
        {
            this.Invoke(new AddFile(RunAddFile));//在线程上执行指定的委托
        }

        /// <summary>
        /// 对文件进行复制，并在复制完成后关闭线程
        /// </summary>
        public void RunAddFile()
        {
            str = tb_FilePath.Text;//记录源文件的路径
            str = "\\" + str.Substring(str.LastIndexOf('\\') + 1, str.Length - str.LastIndexOf('\\') - 1);//获取源文件的名称

            //MessageBox.Show(DestinFolder + str);

            string ToFile = string.Empty;
            ToFile = DestinFolder + str;

            if (File.Exists(@tb_FilePath.Text))
            {
                CopyFile(tb_FilePath.Text, ToFile, 1024, pg_Updata);//复制文件
            }
            else
            {
                updateMessage(lb_StateInfo, "文件不存在.");
                //lb_StateInfo.Items.Add("");
                //lb_StateInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " 文件不存在.");
            }

            thdAddFile.Abort();//关闭线程
        }

        /// <summary>
        /// 文件的复制
        /// </summary>
        /// <param FormerFile="string">源文件路径</param>
        /// <param toFile="string">目的文件路径</param> 
        /// <param SectSize="int">传输大小</param> 
        /// <param progressBar="ProgressBar">ProgressBar控件</param> 
        public void CopyFile(string FormerFile, string toFile, int SectSize, ProgressBar progressBar1)
        {
            progressBar1.Value = 0;//设置进度栏的当前位置为0
            progressBar1.Minimum = 0;//设置进度栏的最小值为0
            FileStream fileToCreate = new FileStream(toFile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
            fileToCreate.Close();//关闭所有资源
            fileToCreate.Dispose();//释放所有资源
            FormerOpen = new FileStream(FormerFile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
            ToFileOpen = new FileStream(toFile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
            int max = Convert.ToInt32(Math.Ceiling((double)FormerOpen.Length / (double)SectSize));//根据一次传输的大小，计算传输的个数
            progressBar1.Maximum = max;//设置进度栏的最大值
            int FileSize;//要拷贝的文件的大小
            if (SectSize < FormerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
            {
                byte[] buffer = new byte[SectSize];//根据传输的大小，定义一个字节数组
                int copied = 0;//记录传输的大小
                int tem_n = 1;//设置进度栏中进度块的增加个数
                while (copied <= ((int)FormerOpen.Length - SectSize))//拷贝主体部分
                {
                    FileSize = FormerOpen.Read(buffer, 0, SectSize);//从0开始读，每次最大读SectSize
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, SectSize);//向目的文件写入字节
                    ToFileOpen.Flush();//清空缓存
                    ToFileOpen.Position = FormerOpen.Position;//使源文件和目的文件流的位置相同
                    copied += FileSize;//记录已拷贝的大小
                    progressBar1.Value = progressBar1.Value + tem_n;//增加进度栏的进度块
                }
                int left = (int)FormerOpen.Length - copied;//获取剩余大小
                FileSize = FormerOpen.Read(buffer, 0, left);//读取剩余的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, left);//写入剩余的部分
                ToFileOpen.Flush();//清空缓存
            }
            else//如果整体拷贝，即每次拷贝内容大于文件总长度
            {
                byte[] buffer = new byte[FormerOpen.Length];//获取文件的大小
                FormerOpen.Read(buffer, 0, (int)FormerOpen.Length);//读取源文件的字节
                FormerOpen.Flush();//清空缓存
                ToFileOpen.Write(buffer, 0, (int)FormerOpen.Length);//写放字节
                ToFileOpen.Flush();//清空缓存
            }
            FormerOpen.Close();//释放所有资源
            ToFileOpen.Close();//释放所有资源


            //progressBar1.Value = 0;//设置进度栏的当有位置为0
            //tb_FilePath.Clear();//清空文本
            //DestinFolder = string.Empty;
            //str = "";
            updateMessage(lb_StateInfo, "升级文件拷贝完成.....");
            updateMessage(lb_StateInfo, "请拔掉USB数据线，静待系统自动升级.....");

 

            /*
            if (MessageBox.Show("复制完成") == DialogResult.OK)//显示"复制完成"提示对话框
            {
                progressBar1.Value = 0;//设置进度栏的当有位置为0
                tb_FilePath.Clear();//清空文本
                DestinFolder=string.Empty;
                str = "";
            }
            */
        }


        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL://U盘插入
                            DriveInfo[] s = DriveInfo.GetDrives();
                            foreach (DriveInfo drive in s)
                            {
                                if (drive.DriveType == DriveType.Removable)
                                {

                                    updateMessage(lb_StateInfo, "U盘已插入，盘符为:" + drive.Name.ToString());
                                    this.btn_EcjetSD.Enabled = true;
                                    Thread.Sleep(1000);
                                    //DestinFolder = @"D:\";
                                    DestinFolder = drive.Name.ToString();
                                    ///lb_StateInfo.Items.Add(DestinFolder);
                                    /*
                                    if (!isCopyEnd)
                                    {
                                        isCopy = true;
                                        CopyFile(drive.Name + ConfigurationManager.AppSettings["sourcedir"].ToString());
                                    }
                                    */
                                    break;
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE: //U盘卸载
                            updateMessage(lb_StateInfo, "U盘已卸载！");
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                updateMessage(lb_StateInfo, "Error:" + ex.Message);

            }
            base.WndProc(ref m);
        }






        //int icheckdev = 0;

        #region 按钮操作
        private void btn_CheckDev_Click(object sender, EventArgs e)
        {

            int Init_Device_iRet = -1;
            byte[] _IDCode = new byte[5];


            try
            {
                 //Commpro
                 ZFYDLL_API_MC.Init_Device(IDCode, ref  Init_Device_iRet);
                 if (Init_Device_iRet == 1)
                 {
                     LoginDevice = DeviceType.Cammpro;
                     updateMessage(lb_StateInfo, "初始化设备成功.");
                     this.btn_Logon.Enabled = true;
                     this.btn_CheckDev.Enabled = false;
                     this.tb_Password.Enabled = true;
                     this.tb_Password.Focus();
                     this.comboUserID.SelectedIndex = 0;
                     comboUserID.Enabled = true;
                     return;
                 }

                //Eeay Storage
                Init_Device_iRet = BODYCAMDLL_API_YZ.BC_ProbeDevEx (out _IDCode[0]);

                if (Init_Device_iRet == 1)
                {
                    BCHandle = BODYCAMDLL_API_YZ.BC_InitDevEx(_IDCode);
                    IDCode = System.Text.Encoding.Default.GetString(_IDCode, 0, _IDCode.Length);

                    //int iRet = -1;
                    //BCHandle =  BODYCAMDLL_API_YZ.Init_Device(IDCode, ref  iRet);
                    
                    if (BCHandle != IntPtr.Zero  )
                    {
                        updateMessage(lb_StateInfo, "检测到设备" + IDCode + ".");
                        LoginDevice = DeviceType.EasyStorage;
                        this.btn_Logon.Enabled = true;
                        this.btn_CheckDev.Enabled = false;
                        this.tb_Password.Enabled = true;
                        this.tb_Password.Focus();
                        this.comboUserID.SelectedIndex = 0;
                        comboUserID.Enabled = true;
                    }
                }
                else
                {
                    updateMessage(lb_StateInfo, "未发现可登陆的设备,请重新连接设备重试.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }




   /// <summary>
   /// 获取登陆设备的分辨率
   /// </summary>
   /// <param name="devicetype"></param>
   /// <param name="password"></param>
   /// <param name="deviceresolution"></param>
   /// <returns>true 成功,false 失败</returns>
        private bool  GetDeviceResolution(DeviceType devicetype, string password,out DeviceResolution deviceresolution)
        {
            deviceresolution = new DeviceResolution();
            int Resolution_Width = -1;
            int Resolution_Height = -1;
            int fps = -1;
            int bps = -1;
            int _ReadDeviceResolution_iRet = -1;


            if (LoginDevice == DeviceType.Cammpro)
                ZFYDLL_API_MC.ReadDeviceResolution(ref  Resolution_Width, ref  Resolution_Height , password, ref _ReadDeviceResolution_iRet);
            //if (LoginDevice == DeviceType.H8)
            //    BODYCAMDLL_API_YZ.ReadDeviceResolution(ref  Resolution_Width, ref  Resolution_Height, password, ref _ReadDeviceResolution_iRet);

            if (LoginDevice == DeviceType.EasyStorage )
              _ReadDeviceResolution_iRet =  BODYCAMDLL_API_YZ.BC_GetMasterVEInfo(BCHandle, DevicePassword , out Resolution_Width,out  Resolution_Height, out fps, out bps);


            if (_ReadDeviceResolution_iRet == 1)
            {
                // this.tb_Resolution.Text = Resolution_Width.ToString() + " X " + Resolution_Height.ToString();
                //updateMessage(lb_StateInfo, "获取视频分辨率参数成功.");
                deviceresolution.Resolution_Width = Resolution_Width;
                deviceresolution.Resolution_Height = Resolution_Height;

                if (LoginDevice == DeviceType.EasyStorage )
                {
                    deviceresolution.Fps = fps;
                    deviceresolution.Bps = bps;
                }

                return true;
            }

            return false;
        }



        /// <summary>
        /// 获取设备硬件信息,序列号等
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <param name="deviceinfo"></param>
        /// <returns>true 成功,false 不成功</returns>
        private bool GetDeviceInfo(DeviceType devicetype, string password, out DeviceInfo deviceinfo)
        {

            deviceinfo = new DeviceInfo();
            int GetZFYInfo_iRet = -1;
            if (LoginDevice == DeviceType.Cammpro)
            {
                ZFYDLL_API_MC.ZFY_INFO uuDevice = new ZFYDLL_API_MC.ZFY_INFO();//执法仪结构信息定义
                ZFYDLL_API_MC.GetZFYInfo(ref uuDevice, password , ref GetZFYInfo_iRet);
                if (GetZFYInfo_iRet == 1)
                {

                    deviceinfo.cSerial = System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                    deviceinfo.userNo = System.Text.Encoding.Default.GetString(uuDevice.userNo);
                    deviceinfo.userName = System.Text.Encoding.Default.GetString(uuDevice.userName);
                    deviceinfo.unitNo = System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                    deviceinfo.unitName = System.Text.Encoding.Default.GetString(uuDevice.unitName);
                    return true;
                }
                else
                    return false;
            }
            if (LoginDevice == DeviceType.EasyStorage )
            {
                BODYCAMDLL_API_YZ.ZFY_INFO uuDevice = new BODYCAMDLL_API_YZ.ZFY_INFO();//执法仪结构信息定义
                GetZFYInfo_iRet =  BODYCAMDLL_API_YZ.BC_GetDevInfo(BCHandle, password, out uuDevice);
               // BODYCAMDLL_API_YZ.GetZFYInfo(ref uuDevice, password, ref GetZFYInfo_iRet);
                if (GetZFYInfo_iRet == 1)
                {
                    //updateMessage(lb_StateInfo, "获取执法仪本机信息 成功.");
                    deviceinfo.cSerial = System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                    deviceinfo.userNo = System.Text.Encoding.Default.GetString(uuDevice.userNo);
                    deviceinfo.userName = System.Text.Encoding.Default.GetString(uuDevice.userName);
                    deviceinfo.unitNo = System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                    deviceinfo.unitName = System.Text.Encoding.Default.GetString(uuDevice.unitName);
                    return true;
                }
                else
                    return false;
            }
            return false;
            
        }


        /// <summary>
        /// 同步时间
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <returns>true 成功,false 失败</returns>
        private bool SyncDeviceTime(DeviceType  devicetype, string password)
        {
            int SyncDevTime_iRet = -1;
            if (LoginDevice == DeviceType.Cammpro )
            {
                ZFYDLL_API_MC.SyncDevTime(password, ref SyncDevTime_iRet);
                if (SyncDevTime_iRet == 5)
                    return true;
            }
            //if (LoginDevice == DeviceType.H8)
            //{
            //    BODYCAMDLL_API_YZ.SyncDevTime(password, ref SyncDevTime_iRet);
            //    if (SyncDevTime_iRet == 5)
            //        return true;
            //}

            if (LoginDevice == DeviceType.EasyStorage )
            {
                SyncDevTime_iRet = BODYCAMDLL_API_YZ.BC_SetDevTime(BCHandle, DevicePassword);
                if (SyncDevTime_iRet == 1)
                    return true;
            }



            return false;
        }




        /// <summary>
        /// 点击登陆
        /// </summary>
        private void LogIn()
        {
            DevicePassword = tb_Password.Text;
            if (string.IsNullOrEmpty(DevicePassword))
            {
                updateMessage(lb_StateInfo, "密码不能为空,请重新输入.");
                tb_Password.Focus();
                return;
            }

            int Battery_iRet = -1;
            int BatteryLevel = -1;
            switch (LoginDevice)
            {
                case DeviceType.NA:
                    break;
                case DeviceType.Cammpro:
                    ZFYDLL_API_MC.ReadDeviceBatteryDumpEnergy(ref BatteryLevel, DevicePassword, ref  Battery_iRet);
                    break;
                //case DeviceType.H8:
                //    BODYCAMDLL_API_YZ.ReadDeviceBatteryDumpEnergy(ref BatteryLevel, DevicePassword, ref Battery_iRet);
                   //break;
                case DeviceType.EasyStorage:
                    if (comboUserID.SelectedIndex == 0)
                        Battery_iRet =  BODYCAMDLL_API_YZ.BC_LoginEx(BCHandle, "admin", DevicePassword);
                    if (comboUserID.SelectedIndex == 1)
                        Battery_iRet = BODYCAMDLL_API_YZ.BC_LoginEx(BCHandle, "user", DevicePassword);
                    break;
                default:
                    break;
            }



            if (Battery_iRet != 1)
            {
                updateMessage(lb_StateInfo, "密码错误,登录失败.");
                tb_Password.SelectAll();
                tb_Password.Focus();
                return;
            }



            if (LoginDevice == DeviceType.EasyStorage)
                BODYCAMDLL_API_YZ.BC_GetBatVal(BCHandle, DevicePassword, out BatteryLevel);


            //登陆成功
            LogonInitUI();

            //同步时间
            if (SyncDeviceTime(LoginDevice, DevicePassword))
                updateMessage(lb_StateInfo, "自动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            else
                updateMessage(lb_StateInfo, "自动设备时间失败.");


                //////////////////////////////////////////////////////////////////////////////
                //执法仪电量
                this.tb_Battery.Text = BatteryLevel.ToString() + " %";
                updateMessage(lb_StateInfo, "获取电量值成功("+ BatteryLevel.ToString() + " %).");
                /////////////////////////////////////////////////////////////////////////////
                //获取执法记录仪分辨率
                DeviceResolution DR = new DeviceResolution();
                if (GetDeviceResolution(LoginDevice, DevicePassword, out DR))
                {
                    this.tb_Resolution.Text = DR.Resolution_Width.ToString() + "*" + DR.Resolution_Height.ToString();
                    updateMessage(lb_StateInfo, "获取视频分辨率参数成功(" + DR.Resolution_Width.ToString() + "*" + DR.Resolution_Height.ToString()+").");

                    if (LoginDevice == DeviceType.EasyStorage) 
                        updateMessage(lb_StateInfo, "获取视频码流帧数参数成功(Bps=" + DR.Bps +"bit/s,Fps="+ DR.Fps +"帧/s).");
                }
                ///////////////////////////////////////////////////////////////////////////
                //执法仪信息读取返回值
                DeviceInfo DI = new DeviceInfo();
                if (GetDeviceInfo(LoginDevice, DevicePassword, out DI))
                {
                    updateMessage(lb_StateInfo, "获取执法仪本机信息成功.");
                    this.tb_DevID.Text = DI.cSerial; //System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                    this.tb_UserID.Text = DI.userNo; ///System.Text.Encoding.Default.GetString(uuDevice.userNo);
                    this.tb_UserName.Text = DI.userName; // System.Text.Encoding.Default.GetString(uuDevice.userName);
                    this.tb_UnitID.Text = DI.unitNo;  //System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                    this.tb_UnitName.Text = DI.unitName; //System.Text.Encoding.Default.GetString(uuDevice.unitName);        
                }



                /////////////////////////////////////////////////////
                //读取执法仪wifi等信息
                List<string> WiFiList = wifi.EnumerateAvailableNetwork(lb_StateInfo);
                WiFiInfo DeviceWiFiInfo = new WiFiInfo();
                if (ReadDeviceWiFiInfo(LoginDevice, DevicePassword, out DeviceWiFiInfo))
                {
                    if (WiFiList.Count > 0)
                    {
                        foreach (string item in WiFiList)
                        {
                            comboWifiName.Items.Add(item);
                        }
                    }

                    if (string.IsNullOrEmpty(DeviceWiFiInfo.WiFiSSID) && comboWifiName.Items.Count > 0)
                        comboWifiName.SelectedIndex = 0;
                    else
                        comboWifiName.Text = DeviceWiFiInfo.WiFiSSID;

                        if (DeviceWiFiInfo.WiFiMode == WiFiModeType.AP)
                            Lb_WifiMode.SelectedIndex = 0;
                        if (DeviceWiFiInfo.WiFiMode == WiFiModeType.STA)
                            Lb_WifiMode.SelectedIndex = 1;

                    lb_WifiPassWord.Text = DeviceWiFiInfo.WiFiPassword;
                    tb_ServerIP.Text = DeviceWiFiInfo.ServerIP;
                    tb_ServerPort.Text = DeviceWiFiInfo.ServerPort;
                }




            this.btn_OK.Enabled = false;
            this.btnReadWireless.Enabled = true;
            this.btnRefreshWifi.Enabled = true;
            this.btn_Wireles_Edit.Enabled = true;
            return;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <param name="wifiinfo"></param>
        /// <returns></returns>
        private bool ReadDeviceWiFiInfo(DeviceType devicetype, string password, out WiFiInfo wifiinfo)
        {
            wifiinfo = new WiFiInfo();

            if (devicetype == DeviceType.Cammpro )
            {
                try
                {
                   // List<string> WiFiList =  wifi.EnumerateAvailableNetwork(lb_StateInfo);
                    //读取Wifi SSID
                    Byte[] WifiSSID = new Byte[20]; //新建字节数组
                    int iRet_ReadWifiSSID = -1;
                    ZFYDLL_API_MC.ReadWifiSSID(ref WifiSSID[0] , password, ref iRet_ReadWifiSSID);
                    wifiinfo.WiFiSSID = System.Text.Encoding.Default.GetString(WifiSSID, 0, WifiSSID.Length);    //将字节数组转换为字符串
         

                    //读取Wifi 密码
                    Byte[] WiFiPassword = new Byte[20];
                    int iRet_ReadWifiPSW = -1;
                    ZFYDLL_API_MC.ReadWifiPSW(ref WiFiPassword[0], DevicePassword, ref iRet_ReadWifiPSW);
                    wifiinfo.WiFiPassword = System.Text.Encoding.Default.GetString(WiFiPassword, 0, WiFiPassword.Length);    //将字节数组转换为字符串

                    //读取WiFi Mode
                    int mode = -1;
                    int iRet_ReadWifiMode = -1;
                    ZFYDLL_API_MC.ReadWifiMode(ref mode, DevicePassword, ref iRet_ReadWifiMode);
                    wifiinfo.WiFiMode = (WiFiModeType)Enum.ToObject(typeof(WiFiModeType), mode);

                    //读取ServerIP
                    //获取服务器IP地址
                    Byte[] IP = new Byte[16];
                    int iRet_ReadServerIP = -1;
                    ZFYDLL_API_MC.ReadServerIP(ref IP[0], DevicePassword, ref iRet_ReadServerIP);
                    wifiinfo.ServerIP = System.Text.Encoding.Default.GetString(IP, 0, IP.Length);    //将字节数组转换为字符串

                    //获取服务器端口
                    Byte[] Port = new Byte[6];
                    int iRet_ReadServerPort = -1;
                    ZFYDLL_API_MC.ReadServerPort(ref Port[0], DevicePassword, ref iRet_ReadServerPort);
                    wifiinfo.ServerPort = System.Text.Encoding.Default.GetString(Port, 0, Port.Length);    //将字节数组转换为字符串


                    byte[] PIN = new byte[32];
                    int iRet_ReadPIN = -1;
                    ZFYDLL_API_MC.Read4GPIN(ref PIN[0], password, ref iRet_ReadPIN);
                    wifiinfo.PIN = System.Text.Encoding.Default.GetString(PIN, 0, PIN.Length);

                    byte[] APN = new byte[32];
                    int iRet_ReadAPN = -1;
                    ZFYDLL_API_MC.Read4GAPN(ref APN[0], password, ref iRet_ReadAPN);
                    wifiinfo.APN = System.Text.Encoding.Default.GetString(APN, 0, APN.Length);

                    updateMessage(lb_StateInfo, "获取执法仪无线信息成功.");
                    return true;
                }
                catch (Exception ex)
                {
                    updateMessage(lb_StateInfo, "获取执法仪无线信息失败," + ex.Message);
                    return false;
                }

            }



            if (devicetype == DeviceType.EasyStorage )
            {



            }


            return false;
        }


        private void btn_Logon_Click(object sender, EventArgs e)
        {

            LogIn();

        }


        



        private void btn_Edit_Click(object sender, EventArgs e)
        {
            if (this.btn_Edit.Text == "编辑")
            {
                this.btn_Edit.Enabled = true;
                this.btn_Edit.Text = "取消";
                this.tb_DevID.Enabled = true;
                this.tb_UserID.Enabled = true;
                this.tb_UserName.Enabled = true;
                this.tb_UnitID.Enabled = true;
                this.tb_UnitName.Enabled = true;
                this.btn_OK.Enabled = true;
            }
            else if(this.btn_Edit.Text == "取消")
            {
                this.tb_DevID.Enabled = false;
                this.tb_UserID.Enabled = false;
                this.tb_UserName.Enabled = false;
                this.tb_UnitID.Enabled = false;
                this.tb_UnitName.Enabled = false;
                this.btn_Edit.Text = "编辑";
                this.btn_OK.Enabled = false;
            }




        }



        /// <summary>
        /// 向执法仪写入信息
        /// </summary>
        /// <param name="devicetype"></param>
        /// <param name="password"></param>
        /// <param name="deviceinfo"></param>
        /// <returns>true,成功,false失败</returns>
        private bool WriteDeviceInfo(DeviceType devicetype,string password, DeviceInfo deviceinfo)
        {
            int WriteZFYInfo_iRet = -1;
            if (LoginDevice == DeviceType.Cammpro) 
            {
                ZFYDLL_API_MC.ZFY_INFO info = new ZFYDLL_API_MC.ZFY_INFO();
                info.cSerial = Encoding.Default.GetBytes(deviceinfo.cSerial.PadRight(8, '\0').ToArray());
                info.userNo = Encoding.Default.GetBytes(deviceinfo.userNo.PadRight(7, '\0').ToArray());
                info.userName  = Encoding.Default.GetBytes(deviceinfo.userName .PadRight(33, '\0').ToArray());
                info.unitNo = Encoding.Default.GetBytes(deviceinfo.unitNo.PadRight(13, '\0').ToArray());
                info.unitName = Encoding.Default.GetBytes(deviceinfo.unitName.PadRight(33, '\0').ToArray());
                ZFYDLL_API_MC.WriteZFYInfo(ref info, password, ref WriteZFYInfo_iRet);
            }
            if (LoginDevice == DeviceType.EasyStorage ) /// debug
            {
                BODYCAMDLL_API_YZ.ZFY_INFO info = new BODYCAMDLL_API_YZ.ZFY_INFO();
                info.cSerial = Encoding.Default.GetBytes(deviceinfo.cSerial.PadRight(8, '\0').ToArray());
                info.userNo = Encoding.Default.GetBytes(deviceinfo.userNo.PadRight(7, '\0').ToArray());
                info.userName  = Encoding.Default.GetBytes(deviceinfo.userName  .PadRight(33, '\0').ToArray());
                info.unitNo = Encoding.Default.GetBytes(deviceinfo.unitNo.PadRight(13, '\0').ToArray());
                info.unitName = Encoding.Default.GetBytes(deviceinfo.unitName.PadRight(33, '\0').ToArray());
                //BODYCAMDLL_API_YZ.WriteZFYInfo(ref info, password, ref WriteZFYInfo_iRet);
                //byte[] _psw = Encoding.Default.GetBytes(password);
                WriteZFYInfo_iRet =  BODYCAMDLL_API_YZ.BC_SetDevInfo(BCHandle, password,info);
                
            }
            if (WriteZFYInfo_iRet == 1)
                return true;
            return false;
           
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.tb_DevID.Enabled = false;
            this.tb_UserID.Enabled = false;
            this.tb_UserName.Enabled = false;
            this.tb_UnitID.Enabled = false;
            this.tb_UnitName.Enabled = false;
            this.btn_OK.Enabled = false;
            this.btn_Edit.Text = "编辑";

            DeviceInfo di = new DeviceInfo ();
            di.cSerial = this.tb_DevID.Text;
            di.userNo = this.tb_UserID.Text;
            di.userName = this.tb_UserName.Text;
            di.unitNo = this.tb_UnitID.Text;
            di.unitName = this.tb_UnitName.Text;
                  
               
            if (WriteDeviceInfo (LoginDevice,DevicePassword,di))
                updateMessage(lb_StateInfo, "向执法仪写入信息成功.");

       }

        private void btn_Format_Click(object sender, EventArgs e)
        {

        }

        private void btn_SyncDevTime_Click(object sender, EventArgs e)
        {
            //DevicePassword = tb_Password.Text;
            //int H6SyncDevTime_iRet=-1;
            //ZFYDLL_API_MC.Init_Device(IDCode, ref  H6Init_Device_iRet);
            //int H8SyncDevTime_iRet = -1;
            //BODYCAMDLL_API_YZ.Init_Device("HACH8", ref  H8Init_Device_iRet);
            //if (H6Init_Device_iRet == 1)
            //{
            //    ZFYDLL_API_MC.SyncDevTime(DevicePassword, ref H6SyncDevTime_iRet);
               
            //    if (H6SyncDevTime_iRet == 5)
            //    {
            //        updateMessage(lb_StateInfo, "手动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            //    }
            //    else
            //    {
            //        updateMessage(lb_StateInfo, "手动同步设备时间失败.");
            //    }
            //}
            //if (H8Init_Device_iRet == 1)
            //{
            //    BODYCAMDLL_API_YZ.SyncDevTime(DevicePassword, ref H8SyncDevTime_iRet);

            //    if (H8SyncDevTime_iRet == 5)
            //    {
            //        updateMessage(lb_StateInfo, "手动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            //    }
            //    else
            //    {
            //        updateMessage(lb_StateInfo, "手动同步设备时间失败.");
            //    }
            //}



            //同步时间
            if (SyncDeviceTime(LoginDevice, DevicePassword))
                updateMessage(lb_StateInfo, "手动同步设备时间成功.（" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            else
                updateMessage(lb_StateInfo, "手动设备时间失败.");
        }

        private void btn_ChangePassword_Click(object sender, EventArgs e)
        {
            //string UserPwd = this.tb_NewPassword.Text;
            DevicePassword = tb_Password.Text;
            //int SetUserPassWord_iRet =-1;


          // ZFYDLL_API_MC.SetUserPassWord(UserPwd, DevicePassword, ref SetUserPassWord_iRet);

         //  lb_StateInfo.Items.Add("");
           //lb_StateInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " SetUserPassWord_iRet = " + SetUserPassWord_iRet.ToString());

           //updateMessage(lb_StateInfo, "向执法仪写入信息成功.");

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="logindevice"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool SetDeviceMSDC(DeviceType logindevice, string password)
        {
            int iRet_SetMSDC = -1;
            if (logindevice == DeviceType.EasyStorage)
                BODYCAMDLL_API_YZ.SetMSDC(password, ref iRet_SetMSDC);
            if (logindevice == DeviceType.Cammpro )
                ZFYDLL_API_MC.SetMSDC(password, ref iRet_SetMSDC);
            if (iRet_SetMSDC == 7)
                return true;
            return false;
        }



        private void btn_SetMSDC_Click(object sender, EventArgs e)
        {


            if (SetDeviceMSDC (LoginDevice,DevicePassword ))
            {
                  this.btn_Edit.Enabled = false;
                  btn_ChangePWd.Enabled = false;
                  btnReadDeviceInfo.Enabled = false;
                  btnReadWireless.Enabled = false;
                  btn_Wireles_Edit.Enabled = false;
                  this.btn_SyncDevTime.Enabled = false;
                  //this.tb_NewPassword.Enabled = false;
                  //this.cb_FileType.Enabled = true; 
                  this.btn_FilePathChose.Enabled = true;
                  this.btn_UpdataFile.Enabled = true;
                  this.btn_SetMSDC.Enabled = false;
                  this.btn_4G.Enabled = false;
                  updateMessage(lb_StateInfo, "执法仪已进入U盘模式.");
            }







            // //DestinFolder = string.Empty;
            ////this.btn_Format.Enabled = true;

            //int H6SetMSDC_iRet = -1;
            //ZFYDLL_API_MC.Init_Device(IDCode, ref  H6Init_Device_iRet);

            //int H8SetMSDC_iRet = -1;
            //BODYCAMDLL_API_YZ.Init_Device("HACH8", ref  H8Init_Device_iRet);
            //if (H6Init_Device_iRet == 1)
            //{

            //    ZFYDLL_API_MC.SetMSDC(DevicePassword, ref  H6SetMSDC_iRet);
            //    //MessageBox.Show(SetMSDC_iRet.ToString());
            //    if (H6SetMSDC_iRet == 7)
            //    {
            //        this.btn_Edit.Enabled = false;
            //        //this.btn_ChangePassword.Enabled = false;
            //        this.btn_SyncDevTime.Enabled = false;
            //        //this.tb_NewPassword.Enabled = false;
            //        //this.cb_FileType.Enabled = true; 
            //        this.btn_FilePathChose.Enabled = true;
            //        this.btn_UpdataFile.Enabled = true;
            //        this.btn_SetMSDC.Enabled = false;
            //        this.btn_4G.Enabled = false;
            //        updateMessage(lb_StateInfo, "执法仪已进入U盘模式.");
            //    }
            //    else
            //    {
            //        updateMessage(lb_StateInfo, "执法仪进入U盘模式失败.");
            //    }
              

            //}

            //if (H8Init_Device_iRet == 1)
            //{

            //    BODYCAMDLL_API_YZ.SetMSDC(DevicePassword, ref  H8SetMSDC_iRet);
            //    //MessageBox.Show(SetMSDC_iRet.ToString());
            //    if (H8SetMSDC_iRet == 7)
            //    {
            //        this.btn_Edit.Enabled = false;
            //        //this.btn_ChangePassword.Enabled = false;
            //        this.btn_SyncDevTime.Enabled = false;
            //        //this.tb_NewPassword.Enabled = false;
            //        //this.cb_FileType.Enabled = true; 
            //        this.btn_FilePathChose.Enabled = true;
            //        this.btn_UpdataFile.Enabled = true;
            //        this.btn_SetMSDC.Enabled = false;
                   
            //        updateMessage(lb_StateInfo, "执法仪已进入U盘模式.");
            //    }
            //    else
            //    {
            //        updateMessage(lb_StateInfo, "执法仪进入U盘模式失败.");
            //    }

            //}


        }

        private void btn_FilePathChose_Click(object sender, EventArgs e)
        {
            //pg_Updata.Value = 0;//设置进度栏的当有位置为0
            //tb_FilePath.Clear();//清空文本
            //DestinFolder = string.Empty;
            //str = "";
/*
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
            }

*/



             OpenFileDialog ofg = new OpenFileDialog();
            ofg.Filter = "升级文件(*.Bin)|*.Bin";
            if (ofg.ShowDialog() == DialogResult.OK)//打开文件对话框
            {
                tb_FilePath.Text = ofg.FileName;//获取源文件的路径
            }
        }

        private void btn_UpdataFile_Click(object sender, EventArgs e)
        {

            str = tb_FilePath.Text;//记录源文件的路径
            str = "\\" + str.Substring(str.LastIndexOf('\\') + 1, str.Length - str.LastIndexOf('\\') - 1);//获取源文件的名称
            
            if (str != "")
            {
                //MessageBox.Show(str);
                thdAddFile = new Thread(new ThreadStart(SetAddFile));//创建一个线程
                thdAddFile.Start();//执行当前线程
            }
            else
            {
                updateMessage(lb_StateInfo, "未选择目的文件，当前不能升级！");
            }


        }
        //关闭按钮
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bRestart)
            {
                //ezUSB.RemoveUSBEventWatcher();
                //Environment.Exit(0);
            }
            else
            {
                DialogResult dr = MessageBox.Show("是否确认退出软件,退出点击是(Y),不退出点击否(N)?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    ezUSB.RemoveUSBEventWatcher();
                    Environment.Exit(0);
                }
                else
                    e.Cancel = true;
            }
            

      
        }

        //UI初始化设置
        public void InitUI()
        {
            //按钮
            this.btn_Logon.Enabled = false;
            this.btn_exit.Enabled = false;
            this.btn_Edit.Enabled = false;
            this.btn_OK.Enabled = false;
            this.btn_EcjetSD.Enabled = false;
            this.btn_4G.Enabled = false;
            //this.btn_4G.Enabled = true;
            //this.btn_ChangePassword.Enabled = false;
            this.btn_FilePathChose.Enabled = false;
            //this.btn_Format.Enabled = false;
            this.btn_OK.Enabled = false;
            this.btn_SyncDevTime.Enabled = false;
            this.btn_SetMSDC.Enabled = false;
            //this.btn_UpdataFile.Enabled = false;
            //文本编辑框
            this.tb_Password.Enabled = false;
            this.tb_FilePath.Enabled = false;
            //this.tb_NewPassword.Enabled = false;
            //this.tb_SDCapacity.Enabled = false;
            this.tb_UnitID.Enabled = false;
            this.tb_UnitName.Enabled = false;
            this.tb_UserID.Enabled = false;
            this.tb_UserName.Enabled = false;
            this.tb_DevID.Enabled = false;

            this.pg_Updata.Enabled = false;
            this.btn_UpdataFile.Enabled = false;
        }


        //登陆后UI初始化设置
        public void LogonInitUI()
        {
            //按钮
            this.btn_4G.Enabled = true;
            this.btn_Edit.Enabled = true;
            this.btn_OK.Enabled = true;
            //this.btn_ChangePassword.Enabled = true;
            this.btn_FilePathChose.Enabled = false;
            //this.btn_Format.Enabled = false;
            this.btn_OK.Enabled = true;
            this.btn_SyncDevTime.Enabled = true;
            this.btn_SetMSDC.Enabled = true;
            //this.btn_UpdataFile.Enabled = true;
            //文本编辑框
            this.tb_FilePath.Enabled = true;
            //this.tb_NewPassword.Enabled = true;
            //this.tb_SDCapacity.Enabled = true;
            this.tb_UnitID.Enabled = true;
            this.tb_UnitName.Enabled = true;
            this.tb_UserID.Enabled = true;
            this.tb_UserName.Enabled = true;
            this.tb_DevID.Enabled = true;

            //this.cb_FileType.Enabled = false; 

            this.pg_Updata.Enabled = true;
            //this.cb_FileType.Text = "FAT32";


            //
            this.btn_Logon.Enabled = false;
            this.tb_Password.Enabled = false;
            this.btn_exit.Enabled = true;
            //
            this.tb_DevID.Enabled = false;
            this.tb_UserID.Enabled = false;
            this.tb_UserName.Enabled = false;
            this.tb_UnitID.Enabled = false;
            this.tb_UnitName.Enabled = false;
            this.btnReadDeviceInfo.Enabled = true;

            //
            //不可编辑状态
            lb_WifiName.Enabled = false;
            lb_WifiPassWord.Enabled = false;
            Lb_WifiMode.Enabled = false;
            tb_ServerIP.Enabled = false;
            tb_ServerPort.Enabled = false;
            tb_4GAPN.Enabled = false;
            tb_4GPIN.Enabled = false;

            btn_ChangePWd.Enabled = true;
            grbChangePassword.Enabled = false;
            comboUserID.Enabled = false;
            if (LoginDevice == DeviceType.Cammpro)
                comboServType.SelectedIndex = 0;
            else
            {
                comboServType.Enabled = true;
                comboServType.SelectedIndex = 0;
            }
            btnEditServer.Enabled = true;
            btnReadServer.Enabled = true;
        }


        #endregion

        private void btn_exit_Click(object sender, EventArgs e)
        {


            DialogResult dr = MessageBox.Show("是否确认退出软件,退出点击是(Y),不退出点击否(N)?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                ezUSB.RemoveUSBEventWatcher();
                Environment.Exit(0);
            }

  

                //updateMessage(lb_StateInfo, "恢复初始状态.");
                //InitUI();

                //this.btn_CheckDev.Enabled = true;
                //this.tb_Password.Text = "";
                //this.tb_Resolution.Text = "";
                //this.tb_Battery.Text = "";
                //this.tb_DevID.Text = "";
                //this.tb_UnitID.Text = "";
                //this.tb_UserID.Text = "";
                //this.tb_UnitName.Text = "";
                //this.tb_UserName.Text = "";
                ////this.cb_FileType.Text = "";
                ////this.tb_NewPassword.Text = "";
                //this.tb_FilePath.Text = "";
                //btn_Wireles_Edit.Enabled = false;
                //btnRefreshWifi.Enabled = false;
                //btnReadWireless.Enabled = false;

                //H6Init_Device_iRet = -1; //H6 初始化返回值
                //H8Init_Device_iRet = -1; //H8 初始化返回值
                //    //设置密码
                //pg_Updata.Value = 0;//设置进度栏的当有位置为0
                //tb_FilePath.Clear();//清空文本
                //DestinFolder = string.Empty;
                //str = "";

                //btnReadDeviceInfo.Enabled = false;
                //Lb_WifiMode.SelectedIndex = -1;
                //comboWifiName.SelectedIndex = -1;
                
                ////内容清空
                //lb_WifiName.Text = "";
                //lb_WifiPassWord.Text = "";
                //Lb_WifiMode.Text = "";
                //tb_ServerIP.Text = "";
                //tb_ServerPort.Text = "";
                //DevicePassword = "";
            


        }

        private void btn_EcjetSD_Click(object sender, EventArgs e)
        {
            if (DestinFolder != "")
            {
                //lb_StateInfo.Items.Add(@DestinFolder);
                System.Diagnostics.Process.Start( @DestinFolder);
            }
            else
            {
                updateMessage(lb_StateInfo, "打开磁盘无效.");
            }
        }



        private bool SetDeviceWiFiInfo(DeviceType devicetype, string password, WiFiInfo devicewifiinfo)
        {
            //不可编辑状态
            lb_WifiName.Enabled = false;
            lb_WifiPassWord.Enabled = false;
            Lb_WifiMode.Enabled = false;
            tb_ServerIP.Enabled = false;
            tb_ServerPort.Enabled = false;
            this.btn_Wireless.Enabled = false;
            btn_Wireles_Edit.Text = "编辑";
            tb_4GAPN.Enabled = false;
            tb_4GPIN.Enabled = false;

            if (devicetype == DeviceType.Cammpro )
            {
                try
                {
                    byte[] IP = new byte[50];
                    int iRet_ReadServerIP = -1;
                    //IP = Encoding.Default.GetBytes(this.tb_ServerIP.Text.PadRight(50, '\0').ToArray());
                    IP = Encoding.Default.GetBytes(devicewifiinfo.ServerIP.PadRight(50, '\0').ToArray());
                    ZFYDLL_API_MC.SetServerIP(IP, DevicePassword, ref iRet_ReadServerIP);

                    byte[] Port = new byte[50];
                    int iRet_SetServerPort = -1;
                    Port = Encoding.Default.GetBytes( devicewifiinfo.ServerPort.PadRight(50, '\0').ToArray());
                    ZFYDLL_API_MC.SetServerPort(Port, DevicePassword, ref iRet_SetServerPort);

                    byte[] WifiSSID = new byte[50];
                    int iRet_SetWifiSSID = -1;
                    // WifiSSID = Encoding.Default.GetBytes(this.lb_WifiName.Text.PadRight(50, '\0').ToArray());
                    WifiSSID = Encoding.Default.GetBytes(devicewifiinfo.WiFiSSID.PadRight(50, '\0').ToArray());
                    ZFYDLL_API_MC.SetWifiSSID(WifiSSID, DevicePassword, ref iRet_SetWifiSSID);

                    byte[] WifiPSW = new byte[50];
                    int iRet_SetWifiPSW = -1;
                    WifiPSW = Encoding.Default.GetBytes( devicewifiinfo.WiFiPassword.PadRight(50, '\0').ToArray());
                    ZFYDLL_API_MC.SetWifiPSW(WifiPSW, DevicePassword, ref iRet_SetWifiPSW);

                    byte[] APN = new byte[32];
                    int iRet_SetAPN = -1;
                    APN = Encoding.Default.GetBytes (devicewifiinfo.APN.PadRight(32, '\0').ToArray());
                    ZFYDLL_API_MC.Set4GAPN(APN, password, ref iRet_SetAPN);

                    byte[] PIN = new byte[32];
                    int iRet_SetPIN = -1;
                    PIN = Encoding.Default.GetBytes(devicewifiinfo.PIN.PadRight(32, '\0').ToArray());
                    ZFYDLL_API_MC.Set4GPIN(PIN, password, ref iRet_SetPIN);

                    //设定WiFi模式,0;AP；1;STA
                    int iRet_SetWifiMode = -1;
                    int mode = -1;
                    mode = (int)devicewifiinfo.WiFiMode;
                    ZFYDLL_API_MC.SetWifiMode(mode, DevicePassword, ref iRet_SetWifiMode);

                return true;
                }
                catch (Exception ex)
                {
                    updateMessage(lb_StateInfo, "设置无线信息失败," + ex.Message);
                    return false;
                }
 

            }



            if (devicetype == DeviceType.EasyStorage )
            {



            }







            return false;
        }



        private void btn_Wireless_Click(object sender, EventArgs e)
        {
            ////不可编辑状态
            //lb_WifiName.Enabled = false;
            //lb_WifiPassWord.Enabled = false;
            //Lb_WifiMode.Enabled = false;
            //tb_ServerIP.Enabled = false;
            //tb_ServerPort.Enabled = false;
            //this.btn_Wireless.Enabled = false;


            // DevicePassword = tb_Password.Text;
            // byte[] IP = new byte[50];
            // int iRet_ReadServerIP = -1;
            // IP = Encoding.Default.GetBytes(this.tb_ServerIP.Text.PadRight(50, '\0').ToArray());
            // ZFYDLL_API_MC.SetServerIP(IP, DevicePassword, ref iRet_ReadServerIP);
             
            // byte[] Port = new byte[50];
            // int iRet_SetServerPort =-1;
            // Port = Encoding.Default.GetBytes(this.tb_ServerPort.Text.PadRight(50, '\0').ToArray());
            // ZFYDLL_API_MC.SetServerPort(Port, DevicePassword, ref iRet_SetServerPort);

            // byte[] WifiSSID = new byte[50];
            // int iRet_SetWifiSSID = -1;
            //// WifiSSID = Encoding.Default.GetBytes(this.lb_WifiName.Text.PadRight(50, '\0').ToArray());
            // WifiSSID = Encoding.Default.GetBytes(this.comboWifiName.Text.PadRight(50, '\0').ToArray());
            // ZFYDLL_API_MC.SetWifiSSID(WifiSSID, DevicePassword, ref iRet_SetWifiSSID);

            // byte[] WifiPSW = new byte[50];
            // int iRet_SetWifiPSW = -1;
            // WifiPSW = Encoding.Default.GetBytes(this.lb_WifiPassWord.Text.PadRight(50, '\0').ToArray());
            // ZFYDLL_API_MC.SetWifiPSW(WifiPSW, DevicePassword, ref iRet_SetWifiPSW);


            // //设定WiFi模式,0;AP；1;STA
            // string WiFiMode = this.Lb_WifiMode.Text;
            // int iRet_SetWifiMode = -1;
            // int mode = -1;
            // if (WiFiMode.Contains("AP"))
            // {
            //     mode = 0;
            // }
            // else if (WiFiMode.Contains("STA"))
            // {
            //     mode = 1;
            // }
            // ZFYDLL_API_MC.SetWifiMode(mode, DevicePassword, ref iRet_SetWifiMode);
            // updateMessage(lb_StateInfo, "无线通信参数已设定 ");


            WiFiInfo DeviceWiFiInfo = new WiFiInfo();
            DeviceWiFiInfo.WiFiMode = (WiFiModeType)Lb_WifiMode.SelectedIndex;
            DeviceWiFiInfo.WiFiSSID = comboWifiName.Text.Trim();
            DeviceWiFiInfo.WiFiPassword = lb_WifiPassWord.Text.Trim();
            DeviceWiFiInfo.ServerIP = tb_ServerIP.Text.Trim();
            DeviceWiFiInfo.ServerPort = tb_ServerPort.Text.Trim();
            DeviceWiFiInfo.APN = tb_4GAPN.Text.Trim();
            DeviceWiFiInfo.PIN = tb_4GPIN.Text.Trim();
            if (SetDeviceWiFiInfo(LoginDevice,DevicePassword, DeviceWiFiInfo ))
                updateMessage (lb_StateInfo,"设置无线信息成功.");

        }









        private void btn_4G_Click(object sender, EventArgs e)
        {
            int clickTimes; //按下次数
            //获取按下次数
            object tag = this.btn_4G.Tag;
            if (tag == null)
            {
                clickTimes = 0;
            }
            else
            {
                clickTimes = Convert.ToInt32(tag);
            }
            //增加并记录按下次数
            this.btn_4G.Tag = ++clickTimes;
            if ((clickTimes % 2) == 1)
            {
                //MessageBox.Show("奇数次");
                //执行你的事情1
                //隐藏无线参数框
                txtApnUser.Visible = true;
                //设定状态显示窗体大小
                gb_StatusCommand.Width = 458;
                gb_StatusCommand.Height = 293;
                //设定状态显示窗口位置
                gb_StatusCommand.Location = new Point(453, 274);
                //状态命令大小
                lb_StateInfo.Width = 440;
                lb_StateInfo.Height = 278;

                //不可编辑状态
                lb_WifiName.Enabled = false;
                lb_WifiPassWord.Enabled = false;
                Lb_WifiMode.Enabled = false;
                tb_ServerIP.Enabled = false;
                tb_ServerPort.Enabled = false;
                btn_Wireless.Enabled = false;
                btnRefreshWifi.Enabled = false;
                comboWifiName.Enabled = false;

                //auto refresh wifi 
                wifi.EnumerateAvailableNetwork(comboWifiName, lb_StateInfo);

                try
                {

                    DevicePassword = tb_Password.Text;

                    //读取Wifi SSID
                    Byte[] WifiSSID = new Byte[20]; //新建字节数组
                    int iRet_ReadWifiSSID = -1;
                    ZFYDLL_API_MC.ReadWifiSSID(ref WifiSSID[0], DevicePassword, ref iRet_ReadWifiSSID);
                    lb_WifiName.Text = System.Text.Encoding.Default.GetString(WifiSSID, 0, WifiSSID.Length);    //将字节数组转换为字符串

                    //读取Wifi 密码
                    Byte[] WifiPSW = new Byte[20];
                    int iRet_ReadWifiPSW = -1;
                    ZFYDLL_API_MC.ReadWifiPSW(ref WifiPSW[0], DevicePassword, ref iRet_ReadWifiPSW);
                    lb_WifiPassWord.Text = System.Text.Encoding.Default.GetString(WifiPSW, 0, WifiPSW.Length);    //将字节数组转换为字符串

                    //读取Wifi模式 0:AP;1:STA
                    int mode = -1;
                    int iRet_ReadWifiMode = -1;
                    ZFYDLL_API_MC.ReadWifiMode(ref mode, DevicePassword, ref iRet_ReadWifiMode);

                    //MessageBox.Show(mode.ToString());
                    if (mode == 0)
                    {
                        Lb_WifiMode.Text = "AP（无线接入）";
                    }
                    else if (mode == 1)
                    {
                        Lb_WifiMode.Text = "STA（无线终端）";
                    }


                    //获取服务器IP地址
                    Byte[] IP = new Byte[16];
                    int iRet_ReadServerIP = -1;
                    ZFYDLL_API_MC.ReadServerIP(ref IP[0], DevicePassword, ref iRet_ReadServerIP);
                    tb_ServerIP.Text = System.Text.Encoding.Default.GetString(IP, 0, IP.Length);    //将字节数组转换为字符串

                    //获取服务器端口
                    Byte[] Port = new Byte[6];
                    int iRet_ReadServerPort = -1;
                    ZFYDLL_API_MC.ReadServerPort(ref Port[0], DevicePassword, ref iRet_ReadServerPort);
                    tb_ServerPort.Text = System.Text.Encoding.Default.GetString(Port, 0, Port.Length);    //将字节数组转换为字符串
                    updateMessage(lb_StateInfo, "获取无线通信参数");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }


 
            }
            else
            {
                //MessageBox.Show("偶数次");
                //执行你的事情2
                //MessageBox.Show("偶数次");
                //隐藏无线参数框
                txtApnUser.Visible = false;
                //设定状态显示窗体大小
                gb_StatusCommand.Width = 458;
                gb_StatusCommand.Height = 475;
                //设定状态显示窗口位置
                //gb_StatusCommand.Location = new Point(453, 11);
                gb_StatusCommand.Location = new Point(453, 90);
                //状态命令大小
                lb_StateInfo.Width = 440;
                lb_StateInfo.Height = 450;

                /*
                lb_WifiName.Text = "";
                lb_WifiPassWord.Text = "";
                Lb_WifiMode.Text = "";
                tb_ServerIP.Text = "";
                tb_ServerPort.Text = "";
                */
            }



        }

        private void btn_Wireles_Edit_Click(object sender, EventArgs e)
        {
            if (this.btn_Wireles_Edit.Text == "编辑")
            {
                lb_WifiName.Enabled = true;
                lb_WifiPassWord.Enabled = true;
                Lb_WifiMode.Enabled = true;
                tb_ServerIP.Enabled = true;
                tb_ServerPort.Enabled = true;
                this.btn_Wireles_Edit.Text = "取消";
                comboWifiName.Enabled = true;
                btnRefreshWifi.Enabled = true;
                tb_4GAPN.Enabled = true;
                tb_4GPIN.Enabled = true;
                btn_Wireless.Enabled = true;
            }
            else if (this.btn_Wireles_Edit.Text == "取消")
            {
                lb_WifiName.Enabled = false;
                lb_WifiPassWord.Enabled = false;
                Lb_WifiMode.Enabled = false;
                tb_ServerIP.Enabled = false;
                tb_ServerPort.Enabled = false;
                this.btn_Wireles_Edit.Text = "编辑";
                comboWifiName.Enabled = false;
                btn_Wireless.Enabled = false;
                btnRefreshWifi.Enabled = true;
                tb_4GAPN.Enabled = false;
                tb_4GPIN.Enabled = false;
            }




        }

        private void btnRefreshWifi_Click(object sender, EventArgs e)
        {
            wifi.EnumerateAvailableNetwork(comboWifiName, lb_StateInfo);
        }

        private void tb_Password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                LogIn();
            }
        }

        private void btnReadDeviceInfo_Click(object sender, EventArgs e)
        {
            ///////////////////////////////////////////////////////////////////////////
            //执法仪信息读取返回值
            DeviceInfo DI = new DeviceInfo();
            if (GetDeviceInfo(LoginDevice, DevicePassword, out DI))
            {
                updateMessage(lb_StateInfo, "获取执法仪本机信息成功.");
                this.tb_DevID.Text = DI.cSerial; //System.Text.Encoding.Default.GetString(uuDevice.cSerial);
                this.tb_UserID.Text = DI.userNo; ///System.Text.Encoding.Default.GetString(uuDevice.userNo);
                this.tb_UserName.Text = DI.userName; // System.Text.Encoding.Default.GetString(uuDevice.userName);
                this.tb_UnitID.Text = DI.unitNo;  //System.Text.Encoding.Default.GetString(uuDevice.unitNo);
                this.tb_UnitName.Text = DI.unitName; //System.Text.Encoding.Default.GetString(uuDevice.unitName);        
            }
        }

        private void btnReadWireless_Click(object sender, EventArgs e)
        {
            /////////////////////////////////////////////////////
            //读取执法仪wifi等信息
           // List<string> WiFiList = wifi.EnumerateAvailableNetwork(lb_StateInfo);
            WiFiInfo DeviceWiFiInfo = new WiFiInfo();
            if (ReadDeviceWiFiInfo(LoginDevice, DevicePassword, out DeviceWiFiInfo))
            {

               comboWifiName.Text = DeviceWiFiInfo.WiFiSSID;

                if (DeviceWiFiInfo.WiFiMode == WiFiModeType.AP)
                    Lb_WifiMode.SelectedIndex = 0;
                if (DeviceWiFiInfo.WiFiMode == WiFiModeType.STA)
                    Lb_WifiMode.SelectedIndex = 1;

                tb_ServerIP.Text = DeviceWiFiInfo.ServerIP;
                tb_ServerPort.Text = DeviceWiFiInfo.ServerPort;
                tb_4GAPN.Text = DeviceWiFiInfo.APN;
                tb_4GPIN.Text = DeviceWiFiInfo.PIN;
            }
        }

        private void tb_ServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void tb_ServerIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8 && e.KeyChar != (char)46)
            {
                e.Handled = true;
            }
        }

        private void btn_ChangePWd_Click(object sender, EventArgs e)
        {
            //FrmChangePwd f = new FrmChangePwd();
            //f.ShowDialog();
            grbChangePassword.Enabled = true;
            btn_ChangePWd.Enabled = false;
        }

        private void brnRestart_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否确认重启软件,退出点击是(Y),不退出点击否(N)?", "Restart?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
               bRestart = true;
               Application.Restart();

            }
  
        }

        private void btnChangePwdOK_Click(object sender, EventArgs e)
        {
            if (comboIDType.SelectedIndex == -1)
            {
                updateMessage(lb_StateInfo, "请要修改密码的账号.");
                comboIDType.Focus();
                return;
            }
            if (string.IsNullOrEmpty (txtNewPwd1.Text.Trim ()))
            {
                updateMessage(lb_StateInfo, "密码不能为空,请重新输入.");
                txtNewPwd1.Focus();
                return;
            }
            if (txtNewPwd1.Text.Trim() != txtNewPwd2.Text.Trim())
            {
                updateMessage(lb_StateInfo, "两次输入的新密码不一致,请重新输入");
                txtNewPwd1.SelectAll();
                txtNewPwd1.Focus();
                return;
            }

            if (SetPwd(LoginDevice, comboIDType, DevicePassword, txtNewPwd1.Text.Trim()))
            {
                updateMessage(lb_StateInfo, "修改" + comboIDType.Text + "的密码成功");
                txtNewPwd1.Text = string.Empty;
                txtNewPwd2.Text = string.Empty;
                grbChangePassword.Enabled = false;
                btn_ChangePWd.Enabled = true;
            }
       


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="logtype"></param>
        /// <param name="idtype"></param>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        private bool SetPwd(DeviceType logtype, ComboBox idtype, string oldpassword, string newpassword)
        {
            int iRet_SetPwd = -1;
            if (logtype == DeviceType.Cammpro) 
            {
                byte[] pwd = new byte[6];
                pwd = Encoding.Default.GetBytes(newpassword.PadRight(6, '\0').ToArray());
                if (idtype.SelectedIndex == 0) //admin
                {
                    //ZFYDLL_API_MC.SetAdminPassWord(newpassword, oldpassword, ref iRet_SetPwd);
                    ZFYDLL_API_MC.SetAdminPassWord(newpassword, oldpassword, ref iRet_SetPwd);
                }

                if (idtype.SelectedIndex == 1) //user
                {
                    ZFYDLL_API_MC.SetUserPassWord  (newpassword, oldpassword, ref iRet_SetPwd);
                }
            }
            if (logtype == DeviceType.EasyStorage )
            {
               
            }


            if (iRet_SetPwd == 7)
                return true;

            return false;
        }

        private void comboServType_SelectedIndexChanged(object sender, EventArgs e)
        {

            chkEnable.Enabled = false;
            chkEnable.Checked = false;
            txtUpdateInternal.Enabled = false;
            txtUpdateInternal.Text = string.Empty;
            tb_ServerIP.Enabled = false;
            tb_ServerIP.Text = string.Empty;
            tb_ServerPort.Enabled = false;
            tb_ServerPort.Text = string.Empty;
            txtDeviceID.Enabled = false;
            txtDeviceID.Text = string.Empty;
            txtServerPassword.Enabled = false;
            txtServerPassword.Text = string.Empty;
            txtServerID.Enabled = false;
            txtServerID.Text = string.Empty;
            txtChannelID.Enabled = false;
            txtChannelID.Text = string.Empty;
            txtChannelName.Enabled = false;
            txtChannelName.Text = string.Empty;

            ServerType _ST =  (ServerType)Enum.ToObject (typeof(ServerType),comboServType.SelectedIndex );
            GetServerInfo(LoginDevice, _ST, DevicePassword);

        }




  /// <summary>
  /// 
  /// </summary>
  /// <param name="logindevice"></param>
  /// <param name="sertype"></param>
  /// <param name="password"></param>
  /// <returns></returns>
        private bool GetServerInfo(DeviceType logindevice, ServerType sertype,string password)
        {


            switch (sertype)
            {
                case ServerType.CMSV6:
                    CMCSV6Server cs6 = new CMCSV6Server();
                    if (GetCMSV6(logindevice, password, out cs6))
                    {
                        updateMessage(lb_StateInfo, "获取CMSV6类型服务器信息成功.");
                        UpdateCMSV6Info(cs6, logindevice);
                    }
                    break;
                case ServerType.GB281811:
                    GB28181Server gb2 = new GB28181Server();
                    if (GetGB28181(logindevice, password, out gb2))
                    {
                        updateMessage(lb_StateInfo, "获取GB28181类型服务器信息成功.");
                        UpdateGB28181Info(logindevice, gb2);
                    }

                    break;
                case ServerType.NetCheckServer:
                    break;
                default:
                    break;
            }



            return false;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="logindevice"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool GetCMSV6(DeviceType logindevice, string password,out CMCSV6Server cs6)
        {
            cs6 = new CMCSV6Server();   
            if (logindevice == DeviceType.Cammpro)
            {
                //获取服务器IP地址
                Byte[] IP = new Byte[16];
                int iRet_ReadServerIP = -1;
                ZFYDLL_API_MC.ReadServerIP(ref IP[0], DevicePassword, ref iRet_ReadServerIP);
                cs6.ServerIP  = System.Text.Encoding.Default.GetString(IP, 0, IP.Length);    //将字节数组转换为字符串
                
                //获取服务器端口
                Byte[] Port = new Byte[6];
                int iRet_ReadServerPort = -1;
                ZFYDLL_API_MC.ReadServerPort(ref Port[0], DevicePassword, ref iRet_ReadServerPort);
                cs6.ServerPort  = System.Text.Encoding.Default.GetString(Port, 0, Port.Length);    //将字节数组转换为字符串

                if (iRet_ReadServerIP == 1 && iRet_ReadServerPort == 1)
                    return true;
                else
                    return false;

            }

            if (logindevice == DeviceType.EasyStorage)
            {
                //获取服务器IP地址
                Byte[] IP = new Byte[16];
                //获取服务器端口
                Byte[] Port = new Byte[6];
                int enbale = -1;
                byte[] DevNo = new byte[8];
                int reporttime = -1;
                int result = -1;
                result = BODYCAMDLL_API_YZ.BC_GetCmsv6Cfg(BCHandle, password, out enbale, out IP[0], out Port[0], out DevNo[0], out  reporttime);
                cs6.DevNo = System.Text.Encoding.Default.GetString(DevNo , 0, DevNo.Length);
                cs6.Enable = enbale;
                cs6.ReportTime = reporttime;
                cs6.ServerIP = System.Text.Encoding.Default.GetString(IP, 0, IP.Length);    //将字节数组转换为字符串
                cs6.ServerPort = System.Text.Encoding.Default.GetString(Port, 0, Port.Length);    //将字节数组转换为字符串

                if (result == 1)
                    return true;
                else
                    return false;
                   

            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cs6"></param>
        /// <param name="logindevice"></param>
        private void UpdateCMSV6Info(CMCSV6Server cs6,DeviceType logindevice)
        {
            tb_ServerIP.Text = cs6.ServerIP;
            tb_ServerPort.Text = cs6.ServerPort;
            if (logindevice == DeviceType.EasyStorage )
            {
                txtUpdateInternal.Text = cs6.ReportTime.ToString();
                if (cs6.Enable == 1)
                    chkEnable.Checked = true;
                if (cs6.Enable == 0)
                    chkEnable.Enabled = false;

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="logindevice"></param>
        /// <param name="password"></param>
        /// <param name="gb2"></param>
        /// <returns></returns>
        private bool GetGB28181(DeviceType logindevice, string password, out GB28181Server gb2)
        {
            gb2 = new GB28181Server();

            int enable = -1;
            int result = -1;
            Byte[] IP = new Byte[16];
            Byte[] Port = new Byte[6];
            byte[] Devno = new byte[32];
            byte[] Chnno = new byte[32];
           // byte[] SerID = new byte[32];
            byte[] ChnName = new byte[32];
            byte[] SerName = new byte[32];
            byte[] serPwd = new byte[32];

            if (logindevice == DeviceType.EasyStorage)
            {
                result = BODYCAMDLL_API_YZ.BC_GetGb28181Cfg(BCHandle, password, out enable, out IP[0], out Port[0],
                    out Devno[0], out Chnno[0],out ChnName[0],out SerName[0], out serPwd[0]);
                   
                if (result == 1)
                {
                    gb2.ServerIP = System.Text.Encoding.Default.GetString(IP, 0, IP.Length);
                    gb2.ServerPort = System.Text.Encoding.Default.GetString(Port, 0, Port.Length);
                    gb2.Enable = enable;
                    gb2.DeviceID = System.Text.Encoding.Default.GetString(Devno, 0, Devno.Length);
                    gb2.ChannelID = System.Text.Encoding.Default.GetString(Chnno, 0, Chnno.Length);
                    gb2.ChannelName = System.Text.Encoding.Default.GetString(ChnName, 0, ChnName.Length);
                    gb2.ServerID = System.Text.Encoding.Default.GetString(SerName , 0,SerName.Length );
                    gb2.DevicePassword = System.Text.Encoding.Default.GetString(serPwd, 0, serPwd.Length);
                    return true;
                }
                else
                    return false;
            }



            return false;
        }


        private void UpdateGB28181Info(DeviceType logindevice, GB28181Server gb2)
        {
            if (logindevice == DeviceType.EasyStorage)
            {
                if (gb2.Enable == 1)
                    chkEnable.Checked = true;
                if (gb2.Enable == 0)
                    chkEnable.Checked = false;

                tb_ServerIP.Text = gb2.ServerIP;
                tb_ServerPort.Text =gb2.ServerPort ;
                txtDeviceID.Text =gb2.DeviceID;
                txtChannelID .Text = gb2.ChannelID;
                txtChannelName .Text =  gb2.ChannelName ;//= System.Text.Encoding.Default.GetString(ChnName, 0, ChnName.Length);
                txtServerID .Text = gb2.ServerID ;//= System.Text.Encoding.Default.GetString(SerName, 0, SerName.Length);
               txtServerPassword .Text =   gb2.DevicePassword ;//= System.Text.Encoding.Default.GetString(serPwd, 0, serPwd.Length);
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditServer_Click(object sender, EventArgs e)
        {
            if (this.btnEditServer.Text == "编辑")
            {
                this.btnEditServer.Enabled = true;
                this.btnEditServer.Text = "取消";
                this.btnWriteServer.Enabled = true;
                ServerType _ST = (ServerType)Enum.ToObject(typeof(ServerType), comboServType.SelectedIndex);
                EditServerInfo(LoginDevice, _ST);
            }
            else if (this.btnEditServer.Text == "取消")
            {
                this.btnEditServer.Text = "编辑";
                this.btnWriteServer.Enabled = false;
                //
                chkEnable.Enabled = false;
                txtUpdateInternal.Enabled = false;
                tb_ServerIP.Enabled = false ;
                tb_ServerPort.Enabled = false;
                txtDeviceID.Enabled = false;
                txtServerPassword.Enabled = false;
                txtServerID.Enabled = false;
                txtChannelID.Enabled = false;
                txtChannelName.Enabled = false;

            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="logindevice"></param>
        /// <param name="sertype"></param>
        private void EditServerInfo(DeviceType logindevice, ServerType sertype)
        {

            switch (sertype)
            {
                case ServerType.CMSV6:

                    if (logindevice == DeviceType.Cammpro)
                    {
                        chkEnable.Enabled = false;
                        txtUpdateInternal.Enabled = false;
                        tb_ServerIP.Enabled = true;
                        tb_ServerPort.Enabled = true;
                        txtDeviceID.Enabled = false;
                        txtServerPassword.Enabled = false;
                        txtServerID.Enabled = false;
                        txtChannelID.Enabled = false;
                        txtChannelName.Enabled = false;
                    }
                    if (logindevice == DeviceType.EasyStorage)
                    {
                        chkEnable.Enabled = true;
                        txtUpdateInternal.Enabled = true;
                        tb_ServerIP.Enabled = true;
                        tb_ServerPort.Enabled = true;
                        txtDeviceID.Enabled = false;
                        txtServerPassword.Enabled = false;
                        txtServerID.Enabled = false;
                        txtChannelID.Enabled = false;
                        txtChannelName.Enabled = false;
                    }


                    break;
                case ServerType.GB281811:
                    if (logindevice == DeviceType.EasyStorage)
                    {
                        chkEnable.Enabled = true;
                        txtUpdateInternal.Enabled = false;
                        tb_ServerIP.Enabled = true;
                        tb_ServerPort.Enabled = true;
                        txtDeviceID.Enabled = true;
                        txtServerPassword.Enabled = true;
                        txtServerID.Enabled = true;
                        txtChannelID.Enabled = true;
                        txtChannelName.Enabled = true;
                    }
                    break;
                case ServerType.NetCheckServer:
                    if (logindevice == DeviceType.EasyStorage)
                    {
                        chkEnable.Enabled = true;
                        txtUpdateInternal.Enabled = false;
                        tb_ServerIP.Enabled = true;
                        tb_ServerPort.Enabled = true;
                        txtDeviceID.Enabled = false;
                        txtServerPassword.Enabled = false;
                        txtServerID.Enabled = false;
                        txtChannelID.Enabled = false;
                        txtChannelName.Enabled = false;
                    }

                    break;
                default:
                    break;
            }

        }


    }
}
