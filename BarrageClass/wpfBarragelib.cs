using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace BarrageClass
{
    class wpfBarragelib
    {
        #region 变量
        //part-1
        private Random ra;
        private Canvas system_RenderCanvas;
        private bool system_enableAPCS;
        public delegate void initCompleteHandler();

        //part-2
        private int barrage_Duration;
        private int barrage_FontSize;
        private bool barrage_EnableShadow;
        private byte barrage_colorR;
        private byte barrage_colorG;
        private byte barrage_colorB;

        //part-3
        private int _maxRow;
        private int _barrage_rowHeight;//弹幕实际占用的高度（大于其FontSize）

        //part-4
        private bool[] _rowList;//_rowList[i] =true 表示该行在忙
        private ArrayList idleRows;//空闲的行号
        #endregion

        #region 逻辑处理函数-构造函数流程
        //1.[初始化part-1、part-2]-构造函数
        public wpfBarragelib(Canvas canvas, Random random = null, bool enableAPCS = true, initCompleteHandler final = null, int Duration = 10000, int FontSize = 40, bool Shadow = true, byte ColorR = 255, byte ColorG = 255, byte ColorB = 255)  {
            system_RenderCanvas = canvas;
            if (random == null) { ra = new Random(); }
            else { ra = random; }

            system_enableAPCS = enableAPCS;

            barrage_Duration = Duration;
            barrage_FontSize = FontSize;
            barrage_EnableShadow = Shadow;
            barrage_colorR = ColorR;
            barrage_colorG = ColorG;
            barrage_colorB = ColorB;

            libInit(final);

        }

        //2.[初始化part-3]-通过向canvas中添加一个FontSize大小的弹幕来获取在canvas中显示时的真是尺寸
        private void libInit(initCompleteHandler initCompleted) {
            BarrageUIBlock _testBarrage = new BarrageUIBlock();

            _testBarrage.Text = "Hello,World!";
            _testBarrage.FontFamily = (FontFamily)new FontFamilyConverter().ConvertFromString("Microsoft YaHei");
            _testBarrage.Name = "uni_testforheight";
            _testBarrage.FontSize = barrage_FontSize;
            _testBarrage.FontWeight = FontWeights.Bold;

            _testBarrage.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                calcRow(system_RenderCanvas.Height, _testBarrage.Name, _testBarrage.ActualHeight);
                if (system_enableAPCS)
                {
                    preventBarrageCover(initCompleted);
                }
                else
                {
                    initCompleted();
                }
            };

            system_RenderCanvas.Children.Add(_testBarrage);
            system_RenderCanvas.RegisterName(_testBarrage.Name, _testBarrage);

        }

        //3.[初始化part-3]-辅助函数
        private void calcRow(double _renderHeight, string _testTargetName, double _fontHeight) {
            //canvas中移除该元素
            BarrageUIBlock _testTargetBarrage = system_RenderCanvas.FindName(_testTargetName) as BarrageUIBlock;
            system_RenderCanvas.Children.Remove(_testTargetBarrage);
            system_RenderCanvas.UnregisterName(_testTargetName);

            _maxRow = (int)(_renderHeight / _fontHeight);

            _barrage_rowHeight = (int)_fontHeight;

            Console.WriteLine(_renderHeight);
            Console.WriteLine(_fontHeight);
            Console.WriteLine(_maxRow);
        }

        //4.避免弹幕在canvas中重叠机制
        private void preventBarrageCover(initCompleteHandler initCompleted) {
            _rowList = new bool[_maxRow];
            idleRows = new ArrayList();
            initCompleted();
        }
        #endregion

        #region 业务逻辑函数
        //1.生成弹幕
        public void createBarrage(string _content, int _targetRow, int _rowHeight, int _fontSize,int _duration, byte _R, byte _G, byte _B, bool _enableShadow) {
            BarrageUIBlock _singleBarrage = new BarrageUIBlock();

            _singleBarrage.Text = _content;
            _singleBarrage.FontFamily = (FontFamily)new FontFamilyConverter().ConvertFromString("Microsoft YaHei");
            _singleBarrage.Name = "uni_" + getRandomString(ra.Next(5, 8));
            _singleBarrage.FontSize = _fontSize;
            _singleBarrage.SetValue(Canvas.TopProperty, (double)_targetRow * _rowHeight);
            _singleBarrage.Fill = new SolidColorBrush(Color.FromRgb(_R,_G,_B));
            _singleBarrage.CacheMode = new BitmapCache();
            _singleBarrage.FontWeight = FontWeights.Bold;

            if (_enableShadow == true)
            {
                DropShadowEffect _ef = new DropShadowEffect();

                _ef.RenderingBias = RenderingBias.Performance;
                _ef.Opacity = (double)100;
                _ef.ShadowDepth = (double)0;
                _ef.BlurRadius = (double)11;

                if (_R == 0 && _G == 0 && _B == 0)
                {
                    _ef.Color = Color.FromRgb(255, 255, 255);
                }
                else
                {
                    _ef.Color = Color.FromRgb(0, 0, 0);
                }

                _singleBarrage.Effect = _ef;
            }

            _singleBarrage.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                doAnimation(_singleBarrage.Name, _duration, _targetRow);//添加动画（弹幕移动）
            };
            system_RenderCanvas.Children.Add(_singleBarrage);
            system_RenderCanvas.RegisterName(_singleBarrage.Name, _singleBarrage);

            if (system_enableAPCS)
            {
                lockRow(_targetRow);//置为忙状态
            }

        }

        //2.添加动画（移动）效果
        private void doAnimation(string _targetUniqueName, int _duration, int _row) {
            BarrageUIBlock _targetBarrage = system_RenderCanvas.FindName(_targetUniqueName) as BarrageUIBlock;

            double _barrageWidth = _targetBarrage.ActualWidth;
            DoubleAnimation _doubleAnimation = new DoubleAnimation(system_RenderCanvas.Width,-_barrageWidth,new Duration(TimeSpan.FromMilliseconds(_duration)),FillBehavior.Stop);

            Storyboard _sb = new Storyboard();
            Storyboard.SetTarget(_doubleAnimation, _targetBarrage);
            Storyboard.SetTargetProperty(_doubleAnimation, new PropertyPath("(Canvas.Left)"));

            _sb.Completed += delegate (object sender, EventArgs e) {
                removeOutdateBarrage(_targetUniqueName,_row);
            };

            _sb.Children.Add(_doubleAnimation);
            _sb.Begin();
        }

        //3.移除canvas中已经显示完成的弹幕
        private void removeOutdateBarrage(string _targetUniqueName, int _row) {
            BarrageUIBlock ready2remove = system_RenderCanvas.FindName(_targetUniqueName) as BarrageUIBlock;
            if (ready2remove != null)
            {
                system_RenderCanvas.Children.Remove(ready2remove);
                system_RenderCanvas.UnregisterName(_targetUniqueName);
                ready2remove = null;

                if (system_enableAPCS)
                {
                    unlockRow(_row);
                }
            }
            else {
                Console.WriteLine("移除弹幕失败！");
            }
        }

        //4.解锁非空闲的行号
        private void unlockRow(int _row = -1) {
            if (!system_enableAPCS) {
                throw new InvalidOperationException("APCS is disabled");
            }
            if (_row == -1)
            {//所有行重置为空闲状态
                _rowList = new bool[_maxRow];
            }
            else {
                _rowList[_row] = false;
            }
        }

        //5.锁上发送弹幕（忙）的行
        private void lockRow(int _row)
        {
            if (!system_enableAPCS)
            {
                throw new InvalidOperationException("APCS is disabled");
            }
            _rowList[_row] = true;
        }
        #endregion

        #region 处理函数
        //1.生成弹幕
        public void generateBarage(string text) {
            if (system_enableAPCS)
            {
                createBarrage(
                    text,
                    getAvailableRow(),
                    _barrage_rowHeight,
                    barrage_FontSize,
                    barrage_Duration,
                    barrage_colorR,
                    barrage_colorG,
                    barrage_colorB,
                    barrage_EnableShadow
                    );
            }
            else {
                throw new InvalidOperationException("APCS is disabled");
            }
        }
        //2.获取canvas中空闲的行
        private int getAvailableRow() {
            if (!system_enableAPCS) {

                throw new InvalidOperationException("APCS is disabled");
            }

            idleRows.Clear();

            for (int i = 0; i < _rowList.Length; i++) {
                if (_rowList[i] == false) {
                    idleRows.Add(i);
                }
            }
            if (idleRows.Count == 0)
            {
                Console.WriteLine("Unlock all rows.");
                unlockRow();

                return ra.Next(0, _maxRow );
            }
            else {
                return (int)idleRows[ra.Next(0, idleRows.Count)];
            }
        }
        #endregion

        public int getFinalRowHeight() {
            return _barrage_rowHeight;
        }


        private string getRandomString(int Length) {
            string _strList = "0123456789abcdefghijklmnopqrstuvwxyz";
            string _strRandom = "";
            for (int i = 0; i <= Length; i++) {
                _strRandom += _strList[ra.Next(0, 35)];
            }
            return _strRandom;
        }



    }
}
