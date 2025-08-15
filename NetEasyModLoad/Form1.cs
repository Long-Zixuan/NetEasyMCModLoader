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
        string gamePath = "";

        string loaderExePath = "";
        string configPath = "";

        string[] mods;
        public Form1()
        {
            InitializeComponent();
            
            loaderExePath = System.Windows.Forms.Application.StartupPath;
            configPath = loaderExePath + @"\config.ini";
            if (!File.Exists(configPath))
            {
                FileStream fs = File.Create(loaderExePath + @"\config.ini");
                fs.Close();
            }
            label3.Text = "当前游戏文件夹："+ File.ReadAllText(configPath);
            gamePath = File.ReadAllText(configPath);
            mods = Directory.GetFiles(loaderExePath+@"\mods", "*.jar");
            //MessageBox.Show(mods[0]);
            //MessageBox.Show(loaderExePath);
        }

        private void choose_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            // folderBrowser.SelectedPath = webpath;
            folderBrowser.Description = "请选择网易我的世界模组文件夹";
            //folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                gamePath = folderBrowser.SelectedPath + @"\Game\.minecraft\mods";
                label3.Text = "当前游戏文件夹："+gamePath;
                File.WriteAllText(configPath, gamePath);
            }
                        
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(gamePath))
            {
                MessageBox.Show("文件夹不存在");
                return;
            }
            label2.Text = "开始加载";
            string filePath = gamePath + "/isLoaded.jar";
            FileStream fs = File.Create(filePath);
            fs.Close();

            while (true)
            {
                if (!File.Exists(filePath))
                {
                    foreach (string mod in mods)
                    {
                        FileInfo file = new FileInfo(mod);
                        string fileName = Path.GetFileName(mod);
                        if (file.Exists)
                        {
                            //true 覆盖已存在的同名文件，false不覆盖
                            file.CopyTo(gamePath+"/"+fileName, true);
                        }
                        else
                        {
                            MessageBox.Show("文件" + mod + "不存在");
                        }
                    }
                    break;
                }
                Thread.Sleep(1000);
            }
            label2.Text = "完成加载";
        }
    }
}
