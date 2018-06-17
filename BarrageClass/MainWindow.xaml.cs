using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;

namespace BarrageClass
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量
        wpfBarragelib lib;
        Random ra = new Random();
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            loadDefaultConfig();

            barrageClassInit();

            GlobalVariable._user_socket.On("pc-message", (data) =>
            {
                msgHandler((string)data);
            });
            GlobalVariable._user_socket.On("forceoffline",()=> {
                this.Close();
            });


        }
        #region 逻辑处理函数

        //1.加载默认的配置到全局变量
        private void loadDefaultConfig() {
            GlobalVariable._RENDER_WIDTH = SystemParameters.WorkArea.Width / 2;
            GlobalVariable._RENDER_HEIGHT = SystemParameters.WorkArea.Height / 2;

            GlobalVariable._user_barrage_FontSize = 40;
            GlobalVariable._user_barrage_Duration = 10000;
            GlobalVariable._user_barrage_EnableShadow = false;

            GlobalVariable._user_barrage_colorR = 255;
            GlobalVariable._user_barrage_colorG = 255;
            GlobalVariable._user_barrage_colorB = 255;

            GlobalVariable._user_audit = false;
        }

        //2.初始化主窗口控件的属性
        private void barrageClassInit() {
            setSize(GlobalVariable._RENDER_WIDTH, GlobalVariable._RENDER_HEIGHT);

            lib = new wpfBarragelib(
                barrageRender,
                ra,
                true,
                InitCompleted,
                GlobalVariable._user_barrage_Duration,
                GlobalVariable._user_barrage_FontSize,
                GlobalVariable._user_barrage_EnableShadow,
                GlobalVariable._user_barrage_colorR,
                GlobalVariable._user_barrage_colorG,
                GlobalVariable._user_barrage_colorB
                );
        }

        //3.设置尺寸
        private void setSize(double width, double height) {
            //窗口尺寸
            renderWindow.Width = width;
            renderWindow.Height = height;

            //画布尺寸
            barrageRender.Width = width;
            barrageRender.Height = height;

            //边框尺寸
            visualBorder.Width = width;
            visualBorder.Height = height;
        }

        //4.lib初始化结束时调用的一个委托
        private void InitCompleted() {
            lib.createBarrage("相关配置加载完毕", 0, lib.getFinalRowHeight(), GlobalVariable._user_barrage_FontSize, GlobalVariable._user_barrage_Duration, GlobalVariable._user_barrage_colorR, GlobalVariable._user_barrage_colorG, GlobalVariable._user_barrage_colorB, GlobalVariable._user_barrage_EnableShadow);
        }

        //5.发送弹幕（用于测试）
        public void sendBarrage(string msg) {
            lib.generateBarage(msg);
        }
        #endregion



        #region 事件
        //1.光标进入主窗口
        private void renderWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            visualBorder.BorderBrush = Brushes.SkyBlue;
            visualBorder.BorderThickness = new Thickness(2);
        }

        //2.光标离开主窗口
        private void renderWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            visualBorder.BorderBrush = null;
            visualBorder.BorderThickness = new Thickness(0);
        }

        //3.窗口内点击鼠标左键
        private void renderWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //4.快捷键
        private void renderWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)) && e.KeyboardDevice.IsKeyDown(Key.S))
                {//【shift+S】:打开设置窗口
                    Window win = new Settings();
                    win.ShowDialog();

                    barrageClassInit();
                }
                else if ((e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)) && e.KeyboardDevice.IsKeyDown(Key.T))
                {//【shift+T】:发送测试弹幕
                    Thread test = new Thread(()=> {
                        for (int i = 0; i < 50; i++) {
                            Thread.Sleep(500);
                            this.Dispatcher.Invoke(new Action(()=> {
                                sendBarrage(getRandomString(20));
                            }));
                        }
                    });

                    test.IsBackground = true;
                    test.Start();
                }
                else if ((e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)) && e.KeyboardDevice.IsKeyDown(Key.C))
                {//【shift+C】:退出程序
                    //GlobalVariable._user_socket.Disconnect();
                    this.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        //5.主窗口关闭事件
        private void renderWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("closing.....");
            GlobalVariable._user_socket.Off("send-message");
            GlobalVariable._user_socket.Disconnect();
            Console.WriteLine("after Disconnect().....");
        }
        #endregion

        #region Helper
        private string getRandomString(int _Length) {
            string _strList = "0123456789abcdefghijklmnopqrstuvwxyz";
            string _strRandom = "";
            for (int i = 0; i <= _Length; i++) {
                _strRandom += _strList[ra.Next(0, 35)];
            }
            return _strRandom;
        }

        private void msgHandler(string msg) {
            if (msg == String.Empty) {
                return;
            }
            else//直接在桌面显示
            {
                this.Dispatcher.Invoke(new Action(() => sendBarrage(msg)));
            }
        }

        #endregion

    }
}
