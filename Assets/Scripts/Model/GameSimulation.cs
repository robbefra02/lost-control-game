using System;
using System.Collections.Generic;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>Owns and ticks every model in a level. Views only read its state.</summary>
    public class GameSimulation
    {
        readonly LevelConfig config;
        readonly List<TicketModel> tickets = new List<TicketModel>();
        readonly List<int> arrivalOrder = new List<int>();
        int nextArrival;

        /// <summary>Tickets that have arrived so far, oldest first. They all simulate every tick.</summary>
        public IReadOnlyList<TicketModel> Tickets => tickets;

        /// <summary>Number of tickets in the whole level, including ones that have not arrived yet.</summary>
        public int TotalTickets => config.tickets.Count;

        /// <summary>Seconds since the level started.</summary>
        public float Time { get; private set; }

        public ConnectionModel Connection { get; }

        public GameOutcome Outcome { get; private set; } = GameOutcome.Playing;

        /// <summary>Arrived tickets that are resolved.</summary>
        public int ResolvedCount
        {
            get
            {
                int count = 0;
                foreach (var ticket in tickets) if (ticket.IsResolved) count++;
                return count;
            }
        }

        /// <summary>Users lost: tickets that reached Blackout, resolved or not.</summary>
        public int LostUsers
        {
            get
            {
                int count = 0;
                foreach (var ticket in tickets) if (ticket.IsBlackout) count++;
                return count;
            }
        }

        public int LostUsersToLose => config.lostUsersToLose;

        public event Action<TicketModel> TicketArrived;

        /// <summary>Any ticket gained a damage level, whether or not the player can see it.</summary>
        public event Action<TicketModel> TicketDamaged;

        public event Action<GameOutcome> GameOver;

        public GameSimulation(LevelConfig config)
        {
            this.config = config;
            Connection = new ConnectionModel(config.connectionTime);

            for (int i = 0; i < config.tickets.Count; i++) arrivalOrder.Add(i);
            // Stable sort, so tickets with the same arrival time keep their config order.
            arrivalOrder.Sort((a, b) =>
            {
                int byTime = config.tickets[a].arrivalTime.CompareTo(config.tickets[b].arrivalTime);
                return byTime != 0 ? byTime : a.CompareTo(b);
            });
        }

        public void Connect(TicketModel ticket)
        {
            if (Outcome != GameOutcome.Playing) return;
            Connection.Connect(ticket);
        }

        /// <summary>True when the player is not looking at this ticket: it is not the connected ticket.</summary>
        public bool IsHidden(TicketModel ticket)
        {
            if (Connection.IsConnectedTo(ticket)) return false;
            if (config.liveListWhenDisconnected && Connection.State == ConnectionState.None) return false;
            return true;
        }

        /// <summary>
        /// True when the list may only show this ticket's last-known state: it is open and hidden.
        /// Closed tickets (resolved or lost) never change again, so they are never stale.
        /// </summary>
        public bool IsStale(TicketModel ticket)
        {
            return !ticket.IsClosed && IsHidden(ticket);
        }

        /// <summary>Advances the level. Does nothing once the level is won or lost.</summary>
        public void Tick(float deltaTime)
        {
            if (Outcome != GameOutcome.Playing) return;

            Time += deltaTime;
            foreach (var ticket in tickets) ticket.Tick(deltaTime);
            Connection.Tick(deltaTime);

            while (nextArrival < arrivalOrder.Count && config.tickets[arrivalOrder[nextArrival]].arrivalTime <= Time)
            {
                int id = arrivalOrder[nextArrival++];
                // Each ticket gets its own random stream, so one ticket's dots never change another's.
                var ticket = new TicketModel(id, config.tickets[id], config, new SeededRandom(config.randomSeed + id), Time);
                ticket.DamageIncreased += OnTicketDamaged;
                tickets.Add(ticket);
                TicketArrived?.Invoke(ticket);
            }

            CheckOutcome();
        }

        // Listeners hear about the damage while the session is still open, then a blacked-out ticket kicks the player out.
        void OnTicketDamaged(TicketModel ticket)
        {
            TicketDamaged?.Invoke(ticket);
            if (ticket.IsBlackout) Connection.Kick(ticket);
        }

        // Lost as soon as enough users are lost; otherwise the level ends once every ticket is closed.
        // Losing wins ties: if the last close and the deciding Blackout land on the same tick, the level is lost.
        void CheckOutcome()
        {
            if (LostUsers >= config.lostUsersToLose)
            {
                End(GameOutcome.Lost);
                return;
            }
            if (nextArrival < arrivalOrder.Count) return;
            foreach (var ticket in tickets) if (!ticket.IsClosed) return;
            End(GameOutcome.Won);
        }

        void End(GameOutcome outcome)
        {
            Outcome = outcome;
            GameOver?.Invoke(outcome);
        }
    }
}
