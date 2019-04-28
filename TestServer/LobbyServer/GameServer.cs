using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using NetLibrary.SimpleNet;

namespace LobbyServer
{
    class GameServer
    {
        object operation_lock;
        Queue<CPacket> user_operations;

        // 로직 스레드
        Thread main_thread;

        private bool shutdown = false;

        // 컨텐츠
        Lobby lobby;

        public GameServer()
        {
            this.operation_lock = new object();
            this.user_operations = new Queue<CPacket>();
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
                lock (this.operation_lock)
                {
                    if (this.user_operations.Count > 0)
                    {
                        packet = this.user_operations.Dequeue();
                    }
                }

                if (packet != null)
                {
                    // 패킷 처리.
                    process_receive(packet);
                }

                // Content Update
                lobby.Update();

                System.Threading.Thread.Sleep(1);
            }
        }
        public void enqueue_packet(CPacket packet)
        {
            lock (this.operation_lock)
            {
                this.user_operations.Enqueue(packet);
            }
        }

        void process_receive(CPacket msg)
        {
            //todo:
            // user msg filter 체크.

            msg.owner.process_user_operation(msg);
        }

        public void Disconnect(User user)
        {
            lobby.Remove(user);
        }

        public bool Connect(User user)
        {
            if (lobby.Add(user) == false)
                return false;

            return true;
        }

        public Lobby GetLobby()
        {
            return lobby;
        }
    }
}
