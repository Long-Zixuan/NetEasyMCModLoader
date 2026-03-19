using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        string gamePath_ = "";

        string loaderExePath_ = "";
        string configPath_ = "";

        string[] mods_;

        Thread jarLoadthread_ = null;

        public Form1()
        {
            InitializeComponent();
            
            loaderExePath_ = System.Windows.Forms.Application.StartupPath;

            configPath_ = loaderExePath_ + @"\config.ini";
            if (!File.Exists(configPath_))
            {
                FileStream fs = File.Create(loaderExePath_ + @"\config.ini");
                fs.Close();
            }
            DirectoryInfo modDinfo = new DirectoryInfo(loaderExePath_ + @"\mods");
            if (!modDinfo.Exists)
            {
                modDinfo.Create();
            }
            //label3.Text = "当前游戏文件夹："+ File.ReadAllText(configPath);
            gamePath_ = IniFileHandler.Instance.ReadValue("config", "gamePath", configPath_);
            if (gamePath_ == "" || gamePath_ == null)
            {
                label3.Text = "请先设置游戏文件夹";
            }
            else
            {
                label3.Text = "当前游戏文件夹：" + gamePath_;
            }
            //gamePath = File.ReadAllText(configPath);
            mods_ = Directory.GetFiles(loaderExePath_+@"\mods", "*.jar");
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
                gamePath_ = folderBrowser.SelectedPath + @"\Game\.minecraft\mods";
                label3.Text = "当前游戏文件夹："+gamePath_;
                //File.WriteAllText(configPath, gamePath);
                IniFileHandler.Instance.WriteValue("config", "gamePath", gamePath_, configPath_);
            }
                        
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(gamePath_))
            {
                MessageBox.Show("文件夹不存在");
                return;
            }
            label2.Text = "加载中";
            mods_ = Directory.GetFiles(loaderExePath_ + @"\mods", "*.jar");
            try
            {
                jarLoadthread_.Abort();
            }
            catch
            {
            }
            jarLoadthread_ = new Thread(new ThreadStart(loadJarLogic));
            jarLoadthread_.Start();
        }

        private void loadJarLogic()
        {
            string filePath = gamePath_ + "/isLoaded.jar";
            FileStream fs = File.Create(filePath);
            fs.Close();
            int dotCount = 0;
            while (true)
            {
                if (!File.Exists(filePath))
                {
                    foreach (string mod in mods_)
                    {
                        FileInfo file = new FileInfo(mod);
                        string fileName = Path.GetFileName(mod);
                        if (file.Exists)
                        {
                            //true 覆盖已存在的同名文件，false不覆盖
                            file.CopyTo(gamePath_ + "/" + fileName, true);
                        }
                        else
                        {
                            MessageBox.Show("文件" + mod + "不存在");
                        }
                    }
                    break;
                }
                label2.Text += ".";
                dotCount++;
                if (dotCount > 5) 
                {
                    dotCount = 0;
                    label2.Text = "加载中";
                }
                Thread.Sleep(1000);
            }
            label2.Text = "完成加载";
        }
    }
}
