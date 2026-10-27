using System;
using System.Collections.Generic;
using LostControl.Config;
using UnityEngine;

namespace LostControl.Model
{
    /// <summary>
    /// The Maze: steer a dot with the joystick from the start cell (bottom left) to the exit cell (top right).
    /// Each round's maze is generated from the minigame seed. Touching a wall bounces the dot back a short way along
    /// the path it came; when nobody steers it (joystick released, or the player is on another PC) for driftDelay
    /// seconds, it slides back that way too. Positions are in cells: (0, 0) is the bottom-left corner of the maze.
    /// The meter fills on a clock.
    /// </summary>
    public class MazeModel : MinigameModel
    {
        // Breadcrumbs closer together than this are not recorded; drifting back follows them in reverse.
        const float CrumbSpacing = 0.05f;

        readonly MazeSettings settings;
        readonly List<Vector2> trail = new List<Vector2>();
        bool[,] eastWalls = new bool[0, 0];
        bool[,] northWalls = new bool[0, 0];
        Vector2 moveDirection;
        float bounceLeft;
        float idleTime;

        public override MinigameType Type => MinigameType.Maze;
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public Vector2Int Exit => new Vector2Int(Columns - 1, Rows - 1);
        public float DotRadius => settings.dotRadius;
        public Vector2 DotPosition { get; private set; }

        /// <summary>The dot touched a wall and bounces back.</summary>
        public event Action WallHit;

        public MazeModel(MazeSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        /// <summary>The joystick this frame: direction and how far it is pushed (length 0-1); zero when released.</summary>
        public void SetMove(Vector2 direction)
        {
            moveDirection = direction;
        }

        /// <summary>Wall along the bottom edge of cell (x, y), for x 0..Columns-1 and y 0..Rows (y = Rows is the top border).</summary>
        public bool HorizontalWall(int x, int y)
        {
            return y == 0 || y == Rows || northWalls[x, y - 1];
        }

        /// <summary>Wall along the left edge of cell (x, y), for x 0..Columns (x = Columns is the right border) and y 0..Rows-1.</summary>
        public bool VerticalWall(int x, int y)
        {
            return x == 0 || x == Columns || eastWalls[x - 1, y];
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            AddPressure(deltaTime / settings.meterFillTime);

            if (bounceLeft > 0f)
            {
                float step = Math.Min(bounceLeft, settings.bounceSpeed * deltaTime);
                // At the start of the path there is nothing left to bounce back along.
                bounceLeft = Retrace(step) < step ? 0f : bounceLeft - step;
                return;
            }

            var direction = connected ? moveDirection : Vector2.zero;
            if (direction != Vector2.zero)
            {
                idleTime = 0f;
                MoveForward(Vector2.ClampMagnitude(direction, 1f), deltaTime);
                return;
            }

            idleTime += deltaTime;
            if (idleTime >= settings.driftDelay) Retrace(settings.driftSpeed * deltaTime);
        }

        protected override void Generate(SeededRandom random, int round)
        {
            var size = settings.SizeFor(round);
            Columns = size.x;
            Rows = size.y;
            eastWalls = new bool[Columns, Rows];
            northWalls = new bool[Columns, Rows];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    eastWalls[x, y] = northWalls[x, y] = true;
            CarvePassages(random);
            ResetDot();
        }

        // Depth-first carving from the start cell: every cell is reachable and there is exactly one way to the exit.
        void CarvePassages(SeededRandom random)
        {
            var visited = new bool[Columns, Rows];
            var stack = new Stack<Vector2Int>();
            var options = new List<Vector2Int>(4);
            visited[0, 0] = true;
            stack.Push(Vector2Int.zero);
            while (stack.Count > 0)
            {
                var cell = stack.Peek();
                options.Clear();
                if (cell.x > 0 && !visited[cell.x - 1, cell.y]) options.Add(Vector2Int.left);
                if (cell.x < Columns - 1 && !visited[cell.x + 1, cell.y]) options.Add(Vector2Int.right);
                if (cell.y > 0 && !visited[cell.x, cell.y - 1]) options.Add(Vector2Int.down);
                if (cell.y < Rows - 1 && !visited[cell.x, cell.y + 1]) options.Add(Vector2Int.up);
                if (options.Count == 0)
                {
                    stack.Pop();
                    continue;
                }

                var step = options[random.RangeInclusive(0, options.Count - 1)];
                var next = cell + step;
                if (step == Vector2Int.right) eastWalls[cell.x, cell.y] = false;
                else if (step == Vector2Int.left) eastWalls[next.x, next.y] = false;
                else if (step == Vector2Int.up) northWalls[cell.x, cell.y] = false;
                else northWalls[next.x, next.y] = false;
                visited[next.x, next.y] = true;
                stack.Push(next);
            }
        }

        void MoveForward(Vector2 direction, float deltaTime)
        {
            var next = DotPosition + direction * settings.dotSpeed * deltaTime;
            if (TouchesWall(next))
            {
                bounceLeft = settings.bounceDistance;
                WallHit?.Invoke();
                return;
            }

            DotPosition = next;
            if ((DotPosition - trail[trail.Count - 1]).sqrMagnitude >= CrumbSpacing * CrumbSpacing) trail.Add(DotPosition);
            if (Mathf.FloorToInt(DotPosition.x) == Exit.x && Mathf.FloorToInt(DotPosition.y) == Exit.y) CompleteRound();
        }

        // Moves the dot back along the recorded path (drift and bounce); the path never touched a wall, so this never
        // does either. Returns the distance actually moved, which is less than asked once the dot is back at the start.
        float Retrace(float distance)
        {
            float budget = distance;
            while (budget > 0f)
            {
                var target = trail[trail.Count - 1];
                float gap = Vector2.Distance(DotPosition, target);
                if (gap > budget)
                {
                    DotPosition = Vector2.MoveTowards(DotPosition, target, budget);
                    return distance;
                }
                DotPosition = target;
                budget -= gap;
                if (trail.Count == 1) return distance - budget;
                trail.RemoveAt(trail.Count - 1);
            }
            return distance;
        }

        void ResetDot()
        {
            DotPosition = new Vector2(0.5f, 0.5f);
            trail.Clear();
            trail.Add(DotPosition);
            bounceLeft = 0f;
            idleTime = 0f;
        }

        bool TouchesWall(Vector2 position)
        {
            float r = settings.dotRadius;
            if (position.x < r || position.y < r || position.x > Columns - r || position.y > Rows - r) return true;

            int x = Mathf.Clamp(Mathf.FloorToInt(position.x), 0, Columns - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(position.y), 0, Rows - 1);
            float fx = position.x - x;
            float fy = position.y - y;
            if (fx < r && VerticalWall(x, y)) return true;
            if (fx > 1f - r && VerticalWall(x + 1, y)) return true;
            if (fy < r && HorizontalWall(x, y)) return true;
            if (fy > 1f - r && HorizontalWall(x, y + 1)) return true;

            // Wall ends meet at the cell's corners.
            for (int cornerX = 0; cornerX <= 1; cornerX++)
                for (int cornerY = 0; cornerY <= 1; cornerY++)
                    if (new Vector2(fx - cornerX, fy - cornerY).sqrMagnitude < r * r && CornerHasWall(x + cornerX, y + cornerY))
                        return true;
            return false;
        }

        bool CornerHasWall(int vertexX, int vertexY)
        {
            return (vertexX > 0 && HorizontalWall(vertexX - 1, vertexY))
                || (vertexX < Columns && HorizontalWall(vertexX, vertexY))
                || (vertexY > 0 && VerticalWall(vertexX, vertexY - 1))
                || (vertexY < Rows && VerticalWall(vertexX, vertexY));
        }
    }
}
