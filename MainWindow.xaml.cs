using StarLight_Core.Authentication;
using StarLight_Core.Enum;
using StarLight_Core.Launch;
using StarLight_Core.Models.Authentication;
using StarLight_Core.Models.Launch;
using StarLight_Core.Utilities;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Diagnostics;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Identity.Client;
using Microsoft.Win32;

namespace LanMay_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GerGameVer();
            GetJavas();

            LoginMode.SelectedIndex = 1;

            //string directoryPath = "background"; // 初始化创建的文件夹名称
            //CheckAndCreateDirectory(directoryPath);
        }

        void GerGameVer()
        {
            GameVersion.DisplayMemberPath = "Id";
            GameVersion.SelectedValuePath = "Id";
            GameVersion.ItemsSource = GameCoreUtil.GetGameCores();
        }
        async Task GetJavas()
        {
            
            JavaPath.DisplayMemberPath = "JavaPath";
            JavaPath.SelectedValuePath = "JavaPath";
            JavaPath.ItemsSource = JavaUtil.GetJavas();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            BaseAccount account;
            
            if(LoginMode.SelectedIndex == 0)
            {
                var auth = new MicrosoftAuthentication("e1e383f9-59d9-4aa2-bf5e-73fe83b15ba0");
                var code = await auth.RetrieveDeviceCodeInfo();
                Clipboard.Clear();
                Clipboard.SetText(code.UserCode);
                MessageBox.Show(code.UserCode);
                Process.Start(new ProcessStartInfo(code.VerificationUri)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
                var token = await auth.GetTokenResponse(code);
                account = await auth.MicrosoftAuthAsync(token, x =>
                {
                    Progress.Text = x;
                });
            }
            else
            {
                account = new OfflineAuthentication(PlayerName.Text).OfflineAuth();
            }
            
            // Get windows Memroy to setting Maxmemory
            var getWinAPI = new GetWindowsApi();
            int systemMemory = (int)((int)initMemory.Value * getWinAPI.GetWindowsAvailableMBytes() * 0.01);
           // systemMemory = System.Convert.ToInt32(systemMemory);
            Console.WriteLine("System total Memory Size: ", getWinAPI.GetWindowsAvailableMBytes() );


            LaunchConfig args = new() // 配置启动参数
            {
                Account = new()
                {
                    BaseAccount = account // 账户
                },
                GameCoreConfig = new()
                {
                    Root = ".minecraft", // 游戏根目录(可以是绝对的也可以是相对的,自动判断)
                    Version = GameVersion.Text, // 启动的版本
                    IsVersionIsolation = true, //版本隔离
                },

                JavaConfig = new()
                {
                    JavaPath = JavaPath.Text, // Java 路径(绝对路径)
                    MaxMemory = (int)systemMemory,
                    MinMemory = 1024*1
                }
            };
            var launch = new MinecraftLauncher(args); // 实例化启动器
            var la = await launch.LaunchAsync(ReportProgress); // 启动
            //kh
            // 日志输出
            la.ErrorReceived += (output) => Console.WriteLine($"{output}");
            la.OutputReceived += (output) => Console.WriteLine($"{output}");

            if (la.Status == Status.Succeeded)
            {
                MessageBox.Show("启动成功");
                // 启动成功执行操作
            }
            else
            {
                MessageBox.Show("启动失败：" + la.Exception);
            }
        }

        private void ReportProgress(ProgressReport report)
        {
            Progress.Text = report.Description + " " + report.Percentage + "%";
        }

        private void showMemory_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            
            var getWinAPI = new GetWindowsApi();
            int systemMemory = (int)((int)initMemory.Value * getWinAPI.GetWindowsAvailableMBytes() * 0.01);
            showMemory.Content = "使用运行内存百分比：" + (int)initMemory.Value + "%" + " " + systemMemory + "MB";
        }

        //打开版本文件夹
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
                // 获取当前应用程序的目录
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // 构建要打开的文件夹路径
                string folderPath = Path.Combine(currentDirectory, ".minecraft\\versions\\"); // 替换为您想要打开的子文件夹名称

                // 使用Process.Start打开文件夹
                try
                {
                    ProcessStartInfo OpenFiles =  new ProcessStartInfo("explorer.exe", $"/select,\"{folderPath}");
                    OpenFiles.UseShellExecute = true;
                    Process.Start(OpenFiles);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法打开文件夹: {ex.Message}");
                }
        }


        //设置背景
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Images (*.png)|*.png|JPEG Images (*.jpg)|*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                backgroud.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName));
            }
            
        }
        
            private void CheckAndCreateDirectory(string directoryPath)
            {
                string currentDirectory = System.IO.Directory.GetCurrentDirectory(); // 获取当前应用程序目录
                string localPath = Path.Combine(currentDirectory, directoryPath); // 组合成完整路径

                if (!Directory.Exists(localPath)) // 检查文件夹是否存在
                {
                    Directory.CreateDirectory(localPath); // 如果不存在，则创建文件夹
                }
            }

        //private void CustomizeBackground_Click(object sender, RoutedEventArgs e)
        //{
        //    // 创建一个OpenFileDialog对象
        //    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
        //    dlg.DefaultExt = ".jpg";
        //    dlg.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png";

        //    // 显示对话框并检查用户是否选择了文件
        //    if (dlg.ShowDialog() == true)
        //    {
        //        // 创建一个ImageBrush对象
        //        ImageBrush brush = new ImageBrush();
        //        brush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlg.FileName));

        //        // 将背景设置为整个窗口
        //        this.Background = brush;
        //    }
        //}


        //登录
        private void LoginMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(LoginMode.SelectedIndex == 0)
            {
                PlayerNameText.Visibility = Visibility.Collapsed;
                PlayerName.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlayerNameText.Visibility = Visibility.Visible;
                PlayerName.Visibility = Visibility.Visible;
            }
        }
    }
}