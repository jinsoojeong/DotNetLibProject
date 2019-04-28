using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using NetLibrary.SimpleNet;

namespace LobbyServer
{
    class Program
    {
        private static ConcurrentQueue<string> log_queue = new ConcurrentQueue<string>();
        private static bool shutdown;

        public static GameServer game_server = new GameServer();

        static void Main(string[] args)
        {
            // 서버 기본 설정
            LogReport("Server Initialize Start!");

            shutdown = false;
            CPacketBufferManager.initialize(2000);
            CNetworkService service = new CNetworkService();

            // 세션 연결 콜백 메소드 설정
            service.session_created_callback += OnSessionCreate;

            // 초기화
            service.initialize();
            game_server.Start();

            LogReport("Server Initialize Succeed!");

            service.listen("0.0.0.0", 7979, 100);
            LogReport("Server Listen Client!");

            Thread log_thread = new Thread(new ThreadStart(OnLoging));
            log_thread.Start();

            while (true)
            {
                string input = Console.ReadLine();
                //Console.Write(".");

                if (input == "exit")
                {
                    shutdown = true;
                    break;
                }

                System.Threading.Thread.Sleep(1000);
            }

            LogReport("Server End!");

            game_server.Stop();
            service.Shutdown();

            if (log_thread != null)
                log_thread.Join();
        }

        static public void LogReport(String log, params object[] args)
        {
            log_queue.Enqueue(String.Format(log, args));
        }

        static void OnLoging()
        {
            String log_text;
            while (!shutdown)
            {                
                if (!log_queue.TryDequeue(out log_text))
                    continue;

                Console.WriteLine(DateTime.Now.ToString() + "  " + log_text);
                System.Threading.Thread.Sleep(1);
            }
        }

        private static void OnSessionCreate(CUserToken token)
        {
            User user = new User();
            user.Initialize(token); // DB 작업 이후에 User를 초기화

            if (game_server.RegistWaitUser(user) == false)
            {
                LogReport("Connection Create User Failed!");
                user.Disconnect();
            }

            LogReport("New Connection Create User!");
            LogReport("User Enter Channel!");
        }

        public static void remove_user(User user)
        {
            game_server.Disconnect(user);
        }
    }
}
