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

using System.Net;
using System.Collections.Concurrent;
using System.Windows.Threading;

using NetLibrary.SimpleNet;
using MsgProtocol;

namespace TestClient
{
    public enum GAME_STATE
    {
        WAIT,
        MATCHING,
        INITIALIZE,
        PLAYING,
        GAME_OVER,
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<IPeer> game_servers = new List<IPeer>();
        static ConcurrentQueue<string> log_queue = new ConcurrentQueue<string>();
        static Button[ , ] position_button = new Button[5, 5];

        Button selected_button = null;
        List<Button> move_able_buttons = new List<Button>();

        static GAME_STATE game_state = GAME_STATE.WAIT;
        static TEAM game_team = TEAM.TEAM_MAX;
        static TEAM turn_team = TEAM.TEAM_MAX;

        public MainWindow()
        {
            InitializeComponent();

            position_button[1, 1] = POSITION_1_1;
            position_button[1, 2] = POSITION_1_2;
            position_button[1, 3] = POSITION_1_3;
            position_button[1, 4] = POSITION_1_4;
            position_button[2, 1] = POSITION_2_1;
            position_button[2, 2] = POSITION_2_2;
            position_button[2, 3] = POSITION_2_3;
            position_button[2, 4] = POSITION_2_4;
            position_button[3, 1] = POSITION_3_1;
            position_button[3, 2] = POSITION_3_2;
            position_button[3, 3] = POSITION_3_3;
            position_button[3, 4] = POSITION_3_4;
            position_button[4, 1] = POSITION_4_1;
            position_button[4, 2] = POSITION_4_2;
            position_button[4, 3] = POSITION_4_3;
            position_button[4, 4] = POSITION_4_4;
        }

        public static void LogReport(string log, params object[] args)
        {
            log_queue.Enqueue("[ LOG ] " + string.Format(log, args));
        }

        public static void ChatReport(string log, params object[] args)
        {
            log_queue.Enqueue("[ CHAT ] " + string.Format(log, args));
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            string log_text;
            while (log_queue.TryDequeue(out log_text))
            {
                log_view.Text += log_text + "\n";
            }

            switch (game_state)
            {
                case GAME_STATE.INITIALIZE:
                    GameInit();
                    break;
                case GAME_STATE.MATCHING:
                    break;
                case GAME_STATE.PLAYING:
                    UpdatePiece();
                    break;
            }
        }

        // 폼 종료 이벤트
        protected override void OnClosed(EventArgs e)
        {
            foreach (IPeer peer in game_servers)
            {
                ((CRemoteServerPeer)peer).token.disconnect();
            }

            base.OnClosed(e);
        }

        // 접속 성공시 호출될 콜백 메소드
        static void on_connected_gameserver(CUserToken server_token)
        {
            lock (game_servers)
            {
                IPeer server = new CRemoteServerPeer(server_token);
                game_servers.Add(server);
                LogReport("Server Connected!");
                LogReport("Chat Server Connected!");
            }
        }

        public static void ChangeState(GAME_STATE state)
        {
            game_state = state;
        }

        public static void SetTeam(TEAM team)
        {
            game_team = team;
        }

        public static void ChangeTrun(TEAM team)
        {
            turn_team = team;
        }

        public static void GameInit()
        {
            LogReport("Game Start ... team {0}", game_team.ToString());

            //버튼 초기화
            PieceManager.Initialize(game_team);
            for (int i = 1; i < 5; ++i)
            {
                for (int j = 1; j < 5; ++j)
                {
                    Piece piece = PieceManager.GetPosition(i, j);
                    if (piece != null)
                    {
                        position_button[i, j].Content = piece.name_;
                        position_button[i, j].Background = (piece.team_ == TEAM.A) ? Brushes.MediumVioletRed : Brushes.CornflowerBlue;
                    }
                    else
                        position_button[i, j].Background = Brushes.Gray;
                }
            }

            game_state = GAME_STATE.PLAYING;
            turn_team = TEAM.A;
        }

        public static void UpdatePiece()
        {
            for (int i = 1; i < 5; ++i)
            {
                for (int j = 1; j < 5; ++j)
                {
                    Piece piece = PieceManager.GetPosition(i, j);
                    if (piece != null)
                    {
                        position_button[i, j].Content = piece.name_;
                        position_button[i, j].Background = (piece.team_ == TEAM.A) ? Brushes.MediumVioletRed : Brushes.CornflowerBlue;
                    }
                    else
                    {
                        position_button[i, j].Content = "";
                        position_button[i, j].Background = Brushes.Gray;
                    }
                }
            }
        }

        public static void PieceMove(Position start, Position dest, bool move_able_check = true)
        {
            Piece selected_piece = PieceManager.GetPosition(start.x, start.y);
            Piece target_piece = PieceManager.GetPosition(dest.x, dest.y);

            if (move_able_check && !selected_piece.MoveAbleCheck(dest.x, dest.y))
                return;

            if (!selected_piece.Move(dest.x, dest.y))
                return;

            if (target_piece != null && target_piece.name_ == "EmperorPenguin")
            {
                MessageBox.Show(string.Format("Game Over Win : {0} King Maker : {1}", ((selected_piece.team_ == TEAM.A) ? "TEAM A" : "TEAM B"), selected_piece.name_));
                game_state = GAME_STATE.GAME_OVER;
            }
        }

        private void Connect_Btn_Click(object sender, RoutedEventArgs e)
        {
            CPacketBufferManager.initialize(2000);
            // CNetworkService객체는 메시지의 비동기 송,수신 처리를 수행한다.
            // 메시지 송,수신은 서버, 클라이언트 모두 동일한 로직으로 처리될 수 있으므로
            // CNetworkService객체를 생성하여 Connector객체에 넘겨준다.
            CNetworkService service = new CNetworkService();

            // endpoint정보를 갖고있는 Connector생성. 만들어둔 NetworkService객체를 넣어준다.
            CConnector connector = new CConnector(service);
            // 접속 성공시 호출될 콜백 매소드 지정.
            connector.connected_callback += on_connected_gameserver;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            connector.connect(endpoint);

            DispatcherTimer log_timer = new DispatcherTimer(); // 객체 생성
            log_timer.Interval = TimeSpan.FromMilliseconds(0.01); // 시간 간격 설정
            log_timer.Tick += new EventHandler(OnUpdate); // 이벤트 추가
            log_timer.Start();
        }

        private void Chat_Send_Click(object sender, RoutedEventArgs e)
        {
            if (Chat_Text_Box.Text.Count() == 0)
                return;

            string chat_txt = Chat_Text_Box.Text;
            Chat_Text_Box.Clear();

            CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
            msg.push(chat_txt);
            game_servers[0].send(msg);
        }

        private void Match_Btn_Click(object sender, RoutedEventArgs e)
        {
            LogReport("Match Start ...");

            CPacket msg = CPacket.create((short)PROTOCOL.ENTRY_MATCHING_REQ);
            game_servers[0].send(msg);
        }

        private void POSITION_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (game_state != GAME_STATE.PLAYING || turn_team != game_team)
                return;

            Button button = sender as Button;
            string[] split = button.Name.Split('_');
            int target_x = Convert.ToInt32(split[1]);
            int target_y = Convert.ToInt32(split[2]);
            Piece target_piece = PieceManager.GetPosition(target_x, target_y);

            if (selected_button != null)
            {
                if (move_able_buttons.Count() > 0)
                {
                    foreach (var botton in move_able_buttons)
                    {
                        botton.BorderThickness = new Thickness(1);
                        button.BorderBrush = Brushes.Gray;
                    }

                    move_able_buttons.Clear();
                }

                if (selected_button == button)
                {
                    selected_button = null;
                    return;
                }
                else
                {
                    string[] selected_split = selected_button.Name.Split('_');
                    int selected_x = Convert.ToInt32(selected_split[1]);
                    int selected_y = Convert.ToInt32(selected_split[2]);
                    selected_button = null;

                    Piece selected_piece = PieceManager.GetPosition(selected_x, selected_y);

                    if (!selected_piece.MoveAbleCheck(target_x, target_x))
                        return;

                    CPacket msg = CPacket.create((short)PROTOCOL.PIECE_MOVE_REQ);
                    msg.push(selected_x);
                    msg.push(selected_y);
                    msg.push(target_x);
                    msg.push(target_x);
                    game_servers[0].send(msg);
                }
            }
            else
            {
                if (target_piece == null)
                    return;

                List<Position> move_able_positions = target_piece.MovePossiblePositions();
                foreach (var position in move_able_positions)
                {
                    Button move_able = position_button[position.x, position.y];
                    move_able.BorderThickness = new Thickness(5);
                    move_able.BorderBrush = Brushes.Yellow;
                    move_able_buttons.Add(move_able);
                }

                selected_button = button;
            }
        }
    }
}
