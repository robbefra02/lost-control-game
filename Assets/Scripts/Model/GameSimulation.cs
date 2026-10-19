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

        public event Action<TicketModel> TicketArrived;

        public GameSimulation(LevelConfig config)
        {
            this.config = config;
            for (int i = 0; i < config.tickets.Count; i++) arrivalOrder.Add(i);
            // Stable sort, so tickets with the same arrival time keep their config order.
            arrivalOrder.Sort((a, b) =>
            {
                int byTime = config.tickets[a].arrivalTime.CompareTo(config.tickets[b].arrivalTime);
                return byTime != 0 ? byTime : a.CompareTo(b);
            });
        }

        public void Tick(float deltaTime)
        {
            Time += deltaTime;
            foreach (var ticket in tickets) ticket.Tick(deltaTime);

            while (nextArrival < arrivalOrder.Count && config.tickets[arrivalOrder[nextArrival]].arrivalTime <= Time)
            {
                int id = arrivalOrder[nextArrival++];
                // Each ticket gets its own random stream, so one ticket's dots never change another's.
                var ticket = new TicketModel(id, config.tickets[id], config, new SeededRandom(config.randomSeed + id), Time);
                tickets.Add(ticket);
                TicketArrived?.Invoke(ticket);
            }
        }
    }
}
