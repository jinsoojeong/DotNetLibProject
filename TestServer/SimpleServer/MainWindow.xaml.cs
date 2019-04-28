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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.Concurrent;
using System.Windows.Threading;
using NetLibrary.SimpleNet;
using NetLibrary.SimpleMatchMaking;

namespace SimpleServer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        static ConcurrentQueue<string> log_queue = new ConcurrentQueue<string>();
        CNetworkService service;

        DispatcherTimer log_timer;

        public MainWindow()
        {
            InitializeComponent();

            // 서버 기본 설정
            LogReport("Server Initialize Start!");

            CPacketBufferManager.initialize(2000);
            service = new CNetworkService();

            // 세션 연결 콜백 메소드 설정
            service.session_created_callback += OnSessionCreate;

            // 초기화
            service.initialize();
            LogReport("Server Initialize Succeed!");

            service.listen("0.0.0.0", 7979, 100);
            LogReport("Server Listen Client!");

            log_timer = new DispatcherTimer(); // 객체 생성
            log_timer.Interval = TimeSpan.FromMilliseconds(0.01); // 시간 간격 설정
            log_timer.Tick += new EventHandler(OnUpdate); // 이벤트 추가
            log_timer.Start();
        }

        public static void LogReport(String log, params object[] args)
        {
            log_queue.Enqueue(String.Format(log, args));
        }

        void OnUpdate(object sender, EventArgs e)
        {
            String log_text;
            while (log_queue.TryDequeue(out log_text))
            {
                log_view.Text += log_text + "\n";
            }
        }

        private void OnSessionCreate(CUserToken token)
        {
            User user = new User(token);
            LogReport("New Connection Create User!");

            if (!Channel.Add(user))
            {
                token.disconnect();
                LogReport("Channel Enter Failed !");
            }

            LogReport("User Enter Channel!");
        }

        public static void remove_user(User user)
        {
            Channel.Remove(user);
        }

        protected override void OnClosed(EventArgs e)
        {
            service.Shutdown();

            if (log_timer != null)
                log_timer.Stop();

            base.OnClosed(e);
        }
    }
}
