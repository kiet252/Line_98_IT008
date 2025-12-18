using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98
{
    internal class GameAlgorithm
    {
        //Bảng banh trước khi di chuyển
        private static int[,] PrevBoardColor = new int[9,9];
        //Điểm trước khi di chuyển
        private static int GameScore;

        /// <summary>
        /// Kiểm tra xem có di chuyển được đến ô đã chọn
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        internal static bool CanMoveBall(Point StartPoint, Point EndPoint, ref int[,] BoardColor)
        {
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            bool[,] visited = new bool[9, 9];
            //Duyệt BFS xem có thể đến được đích từ điểm bắt đầu
            Queue<Point> CheckQueue = new Queue<Point>();
            CheckQueue.Enqueue(StartPoint);

            while (CheckQueue.Count > 0)
            {
                Point point = CheckQueue.First();
                CheckQueue.Dequeue();

                //Nếu đã đến được đích thì kết luận có thể đến
                if (point.X == EndPoint.X && point.Y == EndPoint.Y) return true;
                for (int k = 0; k < 4; k++)
                {
                    int New_X = point.X + dx[k];
                    int New_Y = point.Y + dy[k];
                    if (New_X >= 0 && New_Y >= 0 && New_X < 9 && New_Y < 9 && !visited[New_X, New_Y] && BoardColor[New_X, New_Y] <= 0)
                    {
                        visited[New_X, New_Y] = true;
                        Point NextPoint = new Point(New_X, New_Y);
                        CheckQueue.Enqueue(NextPoint);
                    }
                }
            }
            //Không đến được đích
            return false;
        }

        /// <summary>
        /// Tìm đường đi từ start đến end cho animation
        /// </summary>
        internal static List<Point> FindPath(Point start, Point end, ref int[,] BoardColor)
        {
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            bool[,] visited = new bool[9, 9];
            Point[,] parent = new Point[9, 9];
            Queue<Point> queue = new Queue<Point>();

            queue.Enqueue(start);
            visited[start.X, start.Y] = true;
            parent[start.X, start.Y] = new Point(-1, -1);

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();

                if (current.X == end.X && current.Y == end.Y)
                {
                    return ReconstructPath(parent, start, end);
                }

                for (int k = 0; k < 4; k++)
                {
                    int newX = current.X + dx[k];
                    int newY = current.Y + dy[k];

                    if (newX >= 0 && newY >= 0 && newX < 9 && newY < 9 &&
                        !visited[newX, newY] && BoardColor[newX, newY] <= 0)
                    {
                        visited[newX, newY] = true;
                        parent[newX, newY] = current;
                        queue.Enqueue(new Point(newX, newY));
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Xây dựng đường đi cho animation
        /// </summary>
        internal static List<Point> ReconstructPath(Point[,] parent, Point start, Point end)
        {
            List<Point> path = new List<Point>();
            Point current = end;

            while (current.X != -1 && current.Y != -1)
            {
                path.Add(current);
                current = parent[current.X, current.Y];
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Kiểm tra xem có ăn được banh không 
        /// </summary>
        /// <param name="Src"></param>
        /// <returns>
        /// Trả về số banh ăn được
        /// </returns>
        public static int CheckAndRemoveBall(GameCell Src, ref int[,] BoardColor, ref GameCell[,] BoardCells)
        {
            int r = Src.X_Pos;
            int c = Src.Y_Pos;
            int color = BoardColor[r, c];

            int g_Point = 0; // Điểm số là số banh ăn được 

            // Các hướng: ngang, dọc, chéo chính, chéo phụ
            int[] dx = { 0, 1, 1, 1 };
            int[] dy = { 1, 0, 1, -1 };
            bool isRemoveable = false;
            List<GameCell> toRemove = new List<GameCell>();

            for (int dir = 0; dir < 4; ++dir)
            {
                List<GameCell> line = new List<GameCell>();
                line.Add(Src);

                // đi xuôi hướng
                int x = r + dx[dir];
                int y = c + dy[dir];
                while (x >= 0 && x < 9 && y >= 0 && y < 9 && BoardColor[x, y] == color && BoardColor[x, y] > 0)
                {
                    line.Add(BoardCells[x, y]);
                    x += dx[dir];
                    y += dy[dir];
                }

                // đi ngược hướng
                x = r - dx[dir];
                y = c - dy[dir];
                while (x >= 0 && x < 9 && y >= 0 && y < 9 && BoardColor[x, y] == color && BoardColor[x, y] > 0)
                {
                    line.Add(BoardCells[x, y]);
                    x -= dx[dir];
                    y -= dy[dir];
                }

                // Nếu có ≥ 5 quả liên tiếp cùng màu
                if (line.Count >= 5)
                {
                    isRemoveable = true;
                    foreach (GameCell cell in line)
                    {
                        toRemove.Add(cell);
                    }
                }
            }

            // Xóa banh
            if (isRemoveable)
            {
                g_Point += toRemove.Count;
                foreach (GameCell cell in toRemove)
                {
                    int PrevColor = PrevBoardColor[cell.X_Pos, cell.Y_Pos];
                    if (BoardColor[cell.X_Pos, cell.Y_Pos] != Math.Abs(PrevColor) && PrevColor < 0)
                    {
                        BoardColor[cell.X_Pos, cell.Y_Pos] = PrevColor;
                        cell.BallToDefault();
                        cell.ApplyColorToCell(PrevColor);
                    }
                    else
                    {
                        BoardColor[cell.X_Pos, cell.Y_Pos] = 0;
                        cell.RemoveBall();
                    }
                        
                }
                GameSound.PlayDestroySound();
            }

            return g_Point;
        }

        /// <summary>
        /// Lấy bảng màu banh trước khi di chuyển 
        /// </summary>
        /// <param name="BoardColor"></param>
        public static void GetPrevBoard(int[,] BoardColor, int score) {
            for (int i = 0; i < 9; ++i) {
                for (int j = 0; j < 9; ++j) {
                    PrevBoardColor[i, j] = BoardColor[i, j];
                }
            }
            GameScore = score;
        }

        /// <summary>
        /// Quay lại bước đi trước đó
        /// </summary>
        /// <param name="panel"></param>
        public static void Undo(ref MainGamePanel panel) {
            for (int i = 0; i < 9; ++i) {
                for(int j = 0; j < 9; ++j) {
                    panel.setGameBoardColor(i, j, PrevBoardColor[i, j]);
                }
            }

            panel.GameScore = GameScore;
        }
    }
}
