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

namespace BarrageClass
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //load Current config to textbox
            render_width.Text = GlobalVariable._RENDER_WIDTH.ToString();
            render_height.Text = GlobalVariable._RENDER_HEIGHT.ToString();

            barrage_size.Text = GlobalVariable._user_barrage_FontSize.ToString();
            barrage_duration.Text = GlobalVariable._user_barrage_Duration.ToString();
            barrage_R.Text = GlobalVariable._user_barrage_colorR.ToString();
            barrage_G.Text = GlobalVariable._user_barrage_colorG.ToString();
            barrage_B.Text = GlobalVariable._user_barrage_colorB.ToString();
            barrage_shadow.IsChecked = GlobalVariable._user_barrage_EnableShadow;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //  [100,SystemParameters.WorkArea.Width]
                GlobalVariable._RENDER_WIDTH = Convert.ToDouble(render_width.Text);
                GlobalVariable._RENDER_WIDTH = GlobalVariable._RENDER_WIDTH < 100 ? 100 : GlobalVariable._RENDER_WIDTH;
                GlobalVariable._RENDER_WIDTH = GlobalVariable._RENDER_WIDTH > SystemParameters.WorkArea.Width ? SystemParameters.WorkArea.Width : GlobalVariable._RENDER_WIDTH;

                // [100,SystemParameters.WorkArea.Height]
                GlobalVariable._RENDER_HEIGHT = Convert.ToDouble(render_height.Text);
                GlobalVariable._RENDER_HEIGHT = GlobalVariable._RENDER_HEIGHT < 100 ? 100 : GlobalVariable._RENDER_HEIGHT;
                GlobalVariable._RENDER_HEIGHT = GlobalVariable._RENDER_HEIGHT > SystemParameters.WorkArea.Height ? SystemParameters.WorkArea.Height : GlobalVariable._RENDER_HEIGHT;

                // [10,0.7*GlobalVariable._RENDER_HEIGHT]
                GlobalVariable._user_barrage_FontSize = Convert.ToInt32(barrage_size.Text);
                GlobalVariable._user_barrage_FontSize = GlobalVariable._user_barrage_FontSize > Convert.ToInt32(0.7 * GlobalVariable._RENDER_HEIGHT) ? Convert.ToInt32(0.7 * GlobalVariable._RENDER_HEIGHT) : GlobalVariable._user_barrage_FontSize;

                // [1000,20000]
                GlobalVariable._user_barrage_Duration = Convert.ToInt32(barrage_duration.Text);
                GlobalVariable._user_barrage_Duration = GlobalVariable._user_barrage_Duration < 1000 ? 1000 : GlobalVariable._user_barrage_Duration;
                GlobalVariable._user_barrage_Duration = GlobalVariable._user_barrage_Duration > 20000 ? 20000 : GlobalVariable._user_barrage_Duration;

                GlobalVariable._user_barrage_colorR = Convert.ToByte(barrage_R.Text);
                GlobalVariable._user_barrage_colorG = Convert.ToByte(barrage_G.Text);
                GlobalVariable._user_barrage_colorB = Convert.ToByte(barrage_B.Text);

                GlobalVariable._user_barrage_EnableShadow = barrage_shadow.IsChecked.Value;

            }
            catch (Exception)
            {
                MessageBox.Show("存在无效的值, 部分设置将不会生效\r\n\r\nInput value Invalid,Some setting won't change");
            }

            this.Close();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
