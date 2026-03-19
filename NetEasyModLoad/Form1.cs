using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace NetEasyModLoad
{
    public partial class Form1 : Form
    {
        static string gamePath_s = "";

        static string loaderExePath_s = "";
        static string configPath_s = "";

        static string[] mods_s;

        Thread jarLoadthread_ = null;
        LoadJatThread loadJatThread_ = null;

        public Form1()
        {
            InitializeComponent();
            
            loaderExePath_s = System.Windows.Forms.Application.StartupPath;

            configPath_s = loaderExePath_s + @"\config.ini";
            if (!File.Exists(configPath_s))
            {
                FileStream fs = File.Create(loaderExePath_s + @"\config.ini");
                fs.Close();
            }
            DirectoryInfo modDinfo = new DirectoryInfo(loaderExePath_s + @"\mods");
            if (!modDinfo.Exists)
            {
                modDinfo.Create();
            }
            //label3.Text = "当前游戏文件夹："+ File.ReadAllText(configPath);
            gamePath_s = IniFileHandler.Instance.ReadValue("config", "gamePath", configPath_s);
            if (gamePath_s == "" || gamePath_s == null)
            {
                label3.Text = "请先设置游戏文件夹";
            }
            else
            {
                label3.Text = "当前游戏文件夹：" + gamePath_s;
            }
            //gamePath = File.ReadAllText(configPath);
            mods_s = Directory.GetFiles(loaderExePath_s+@"\mods", "*.jar");
            //MessageBox.Show(mods[0]);
            //MessageBox.Show(loaderExePath);
        }

        private void choose_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            // folderBrowser.SelectedPath = webpath;
            folderBrowser.Description = "请选择网易我的世界游戏文件夹";
            //folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                gamePath_s = folderBrowser.SelectedPath + @"\Game\.minecraft\mods";
                label3.Text = "当前游戏文件夹："+gamePath_s;
                //File.WriteAllText(configPath, gamePath);
                IniFileHandler.Instance.WriteValue("config", "gamePath", gamePath_s, configPath_s);
            }
                        
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(gamePath_s))
            {
                MessageBox.Show("文件夹不存在");
                return;
            }
            label2.Text = "加载中";
            mods_s = Directory.GetFiles(loaderExePath_s + @"\mods", "*.jar");
            try
            {
                loadJatThread_.Stop();
                //jarLoadthread_.Abort();
                loadJatThread_ = null;
            }
            catch
            {
            }
            loadJatThread_ = new LoadJatThread(label2);
            jarLoadthread_ = new Thread(new ThreadStart(loadJatThread_.Run));
            jarLoadthread_.Start();
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //jarLoadthread_.Abort();
                loadJatThread_.Stop();
                loadJatThread_ = null;
            }
            catch { }
        }

        private class LoadJatThread
        {
            public LoadJatThread(Label label2)
            {
                this.label2_ = label2;
            }

            private Label label2_;

            private volatile bool running_ = true;
            public bool Running { get { return running_; } }

            public void Stop()
            {
                running_ = false;
            }

            public void Run()
            {
                string filePath = gamePath_s + "/isLoaded.jar";
                FileStream fs = File.Create(filePath);
                fs.Close();
                int dotCount = 0;
                while (running_)
                {
                    if (!File.Exists(filePath))
                    {
                        foreach (string mod in mods_s)
                        {
                            FileInfo file = new FileInfo(mod);
                            string fileName = Path.GetFileName(mod);
                            if (file.Exists)
                            {
                                //true 覆盖已存在的同名文件，false不覆盖
                                file.CopyTo(gamePath_s + "/" + fileName, true);
                            }
                            else
                            {
                                MessageBox.Show("文件" + mod + "不存在");
                            }
                        }
                        break;
                    }
                    try
                    {//防止某些计算机设置了不让主线程以外的线程访问控件
                        label2_.Text += ".";
                        dotCount++;
                        if (dotCount > 5)
                        {
                            dotCount = 0;
                            label2_.Text = "加载中";
                        }
                    }
                    catch { }
                    Thread.Sleep(1000);
                }
                try
                { label2_.Text = "完成加载"; }
                catch { }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            string url = @"https://long-zixuan.github.io/html/lain.html";
            try
            {
                Process.Start(url);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    Console.WriteLine("未检测到已安装的浏览器！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
