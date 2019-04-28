using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    using MoveAblePosition = List<Position>;

    public class Position
    {
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }
        public int y { get; set; }
    }

    public enum PieceState
    {
        NONE,
        SELECTED,
        MOVE,
        FEEDON
    }

    public enum TEAM
    {
        A,
        B,
        TEAM_MAX
    }

    public abstract class Piece
    {
        public Piece(TEAM team, string name, Position init_position)
        {
            this.team_ = team;
            this.name_ = name;
            this.position_ = init_position;

            move_able_positions_ = new MoveAblePosition();
        }

        public PieceState GetState()
        {
            return state_;
        }

        public void SetState(PieceState state)
        {
            state_ = state;
        }

        public bool MoveAbleCheck(int x, int y)
        {
            MoveAblePosition move_possible_position = MovePossiblePositions();

            foreach (var position in move_possible_position)
            {
                if (position.x == x && position.y == y)
                    return true;
            }

            return false;
        }

        public bool Move(int x, int y)
        {
            Piece piece = PieceManager.GetPosition(x, y);
            if (piece != null)
            {
                if (piece.team_ == team_)
                    return false;
                else
                {
                    if (!piece.Die())
                        return false;
                }
            }

            if (!PieceManager.Delete(position_.x, position_.y))
                return false;

            if (!PieceManager.Move(this, x, y))
                return false;

            position_.x = x;
            position_.y = y;

            return true;
        }

        public bool Die()
        {
            if (!PieceManager.Delete(position_.x, position_.y))
                return false;

            return true;
        }

        public MoveAblePosition MovePossiblePositions()
        {
            MoveAblePosition list = new MoveAblePosition();
            foreach (var position in move_able_positions_)
            {
                if ((position.x + position_.x > 4) || (position.x + position_.x < 1))
                    continue;

                if ((position.y + position_.y > 4) || (position.y + position_.y < 1))
                    continue;

                list.Add(new Position((position.x + position_.x), (position.y + position_.y)));
            }

            return list;
        }

        public PieceState state_;
        public string name_ { get; protected set; }
        public TEAM team_ { get; protected set; }
        protected MoveAblePosition move_able_positions_ { get; set; }
        protected Position position_ { get; set; }
    }

    class Chick : Piece
    {
        public Chick(TEAM team, Position position) : base(team, "Chick", position)
        {
            base.move_able_positions_.Add(new Position(-1, 0));
            //base.move_able_positions_.Add(new Position(1 * ((team == TEAM.A) ? 1 : -1), 0));
        }
    }

    class Eagle : Piece
    {
        public Eagle(TEAM team, Position position) : base(team, "Eagle", position)
        {
            base.move_able_positions_.Add(new Position(0, 1));
            base.move_able_positions_.Add(new Position(0, -1));
            base.move_able_positions_.Add(new Position(1, 0));
            base.move_able_positions_.Add(new Position(-1, 0));
        }
    }

    class Hawk : Piece
    {
        public Hawk(TEAM team, Position position) : base(team, "Hawk", position)
        {
            base.move_able_positions_.Add(new Position(1, 1));
            base.move_able_positions_.Add(new Position(1, -1));
            base.move_able_positions_.Add(new Position(-1, 1));
            base.move_able_positions_.Add(new Position(-1, -1));
        }
    }

    class Penguin : Piece
    {
        public Penguin(TEAM team, Position position) : base (team, "Penguin", position)
        {
            base.move_able_positions_.Add(new Position(0, 1));
            base.move_able_positions_.Add(new Position(0, -1));
            base.move_able_positions_.Add(new Position(1, 0));
            base.move_able_positions_.Add(new Position(-1, 0));

            base.move_able_positions_.Add(new Position(-1, 1));
            base.move_able_positions_.Add(new Position(-1, - 1));
            //base.move_able_positions_.Add(new Position(1 * ((team == TEAM.A) ? 1 : -1), 1));
            //base.move_able_positions_.Add(new Position(1 * ((team == TEAM.A) ? 1 : -1), - 1));
        }
    }

    class EmperorPenguin : Piece
    {
        public EmperorPenguin(TEAM team, Position position) : base(team, "EmperorPenguin", position)
        {
            base.move_able_positions_.Add(new Position(0, 1));
            base.move_able_positions_.Add(new Position(0, -1));
            base.move_able_positions_.Add(new Position(1, 0));
            base.move_able_positions_.Add(new Position(-1, 0));

            base.move_able_positions_.Add(new Position(1, 1));
            base.move_able_positions_.Add(new Position(1, -1));
            base.move_able_positions_.Add(new Position(-1, 1));
            base.move_able_positions_.Add(new Position(-1, -1));
        }
    }

    class PieceManager
    {
        static Piece[,] position_piece = new Piece[5, 5];

        static public void Initialize(TEAM team)
        {
            position_piece[1, 1] = new Hawk((team == TEAM.A) ? TEAM.B : TEAM.A, new Position(1, 1));
            position_piece[1, 2] = new Penguin((team == TEAM.A) ? TEAM.B : TEAM.A, new Position(1, 2));
            position_piece[1, 3] = new EmperorPenguin((team == TEAM.A) ? TEAM.B : TEAM.A, new Position(1, 3));
            position_piece[1, 4] = new Eagle((team == TEAM.A) ? TEAM.B : TEAM.A, new Position(1, 4));

            position_piece[2, 1] = null;
            position_piece[2, 2] = new Chick((team == TEAM.A) ? TEAM.B : TEAM.A, new Position(2, 2));
            position_piece[2, 3] = new Chick((team == TEAM.A) ? TEAM.B : TEAM.A, new Position(2, 3));
            position_piece[2, 4] = null;

            position_piece[3, 1] = null;
            position_piece[3, 2] = new Chick(team, new Position(3, 2));
            position_piece[3, 3] = new Chick(team, new Position(3, 3));
            position_piece[3, 4] = null;

            position_piece[4, 1] = new Eagle(team, new Position(4, 1));
            position_piece[4, 2] = new EmperorPenguin(team, new Position(4, 2));
            position_piece[4, 3] = new Penguin(team, new Position(4, 3));
            position_piece[4, 4] = new Hawk(team, new Position(4, 4));
        }

        static public Piece GetPosition(int x, int y)
        {
            Piece piece = position_piece[x, y];

            if (piece == null)
                return null;

            return piece;
        }

        static public bool Delete(int x, int y)
        {
            Piece piece = position_piece[x, y];

            if (piece == null)
                return false;

            position_piece[x, y] = null;

            return true;
        }

        static public bool Move(Piece piece, int target_x, int target_y)
        {
            Piece target_piece = position_piece[target_x, target_y];

            if (target_piece != null)
                return false;

            position_piece[target_x, target_y] = piece;

            return true;
        }
    }
}
