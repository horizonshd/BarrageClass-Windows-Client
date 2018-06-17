using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Quobject.SocketIoClientDotNet.Client;

namespace BarrageClass
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String _account = account.Text.Trim();
            String _password = password.Password.Trim();
            string query = "account=" + _account + "&password=" + _password;
            if (LoginHelper.Login(GlobalVariable._user_server+"/pc/login", query))
            {
                //连接服务器的socket.io
                GlobalVariable._user_account = _account;
                GlobalVariable._user_socket = IO.Socket(GlobalVariable._user_server);
                GlobalVariable._user_socket.Emit("pc_login", GlobalVariable._user_account);

                Window nextWindow = new MainWindow();
                nextWindow.Show();
                this.Close();

            }
            else {
                message.Content = "账号或密码错误！";
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
