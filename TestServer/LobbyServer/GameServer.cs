using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

using System.Threading;
using NetLibrary.SimpleNet;

namespace LobbyServer
{
    class GameServer
    {
        ConcurrentQueue<CPacket> user_operations;
        ConcurrentDictionary<int, User> login_wait_users;

        // 로직 스레드
        Thread main_thread;

        private bool shutdown = false;

        // 컨텐츠
        Lobby lobby;

        public GameServer()
        {
            this.user_operations = new ConcurrentQueue<CPacket>();
            this.login_wait_users = new ConcurrentDictionary<int, User>();
            this.lobby = new Lobby();
        }

        public bool Start()
        {
            shutdown = false;

            this.main_thread = new Thread(gameloop);
            this.main_thread.Start();

            return true;
        }

        public void Stop()
        {
            shutdown = true;
            this.main_thread.Join();
        }

        void gameloop()
        {
            while (!shutdown)
            {
                CPacket packet = null;
                while (user_operations.TryDequeue(out packet))
                {
                    if (packet == null)
                        break;

                    MsgHandle(packet);
                }

                User user = null;
                while (login_wait_users.TryGetValue(out user))
                {
                    if (user == null)
                        break;

                    if (lobby.Add(user) == false)
                        user.Disconnect();
                }

                // Content Update
                lobby.Update();

                System.Threading.Thread.Sleep(1);
            }
        }
        public void PushPacket(CPacket packet)
        {
            this.user_operations.Enqueue(packet);
        }

        void MsgHandle(CPacket msg)
        {
            //todo:
            // user msg filter 체크.

            msg.owner.process_user_operation(msg);
        }

        public void Disconnect(User user)
        {
            if (login_wait_users.ContainsKey(user.GetTokkenID()))
            {
                login_wait_users.Remove(user.GetTokkenID(), out user);
            }

            lobby.Remove(user);
        }

        public bool RegistWaitUser(User user)
        {
            if (login_wait_users.TryAdd(user.GetTokkenID(), user) == false)
                return false;

            return true;
        }

        public Lobby GetLobby()
        {
            return lobby;
        }
    }
}
