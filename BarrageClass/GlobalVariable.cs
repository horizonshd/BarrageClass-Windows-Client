using Quobject.SocketIoClientDotNet.Client;
namespace BarrageClass
{
    class GlobalVariable
    {
        //Render - User Settings
        public static double _RENDER_WIDTH;
        public static double _RENDER_HEIGHT;

        //Danmaku - User Settings
        public static int _user_barrage_Duration;
        public static int _user_barrage_FontSize;
        public static bool _user_barrage_EnableShadow;

        public static byte _user_barrage_colorR;
        public static byte _user_barrage_colorG;
        public static byte _user_barrage_colorB;

        //Communication - User Settings
        public static bool _user_audit;
        //public static string _user_server = "http://10.63.15.130:3000";
        public static string _user_server = "http://120.79.10.123:3000";//aliyun
        public static string _user_account;
        public static Socket _user_socket;
    }
}
