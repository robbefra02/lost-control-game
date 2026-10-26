using System;
using System.Collections.Generic;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// Packet filter (D53): packets travel along one cable into the PC, each carrying a port number. Every round a rule
    /// blocks one or more ports. Clicking a packet blocks it: a bad one counts towards the round, a good one adds
    /// pressure. A bad packet that reaches the PC adds pressure; the stream keeps flowing while the player is away.
    /// The round ends once enough bad packets are blocked. Only leaks and mistakes fill the meter.
    /// </summary>
    public class PacketFilterModel : MinigameModel
    {
        public struct Packet
        {
            public int id;
            public int port;
            /// <summary>0 where the packet enters the cable, 1 at the PC.</summary>
            public float progress;
        }

        readonly PacketFilterSettings settings;
        readonly List<Packet> packets = new List<Packet>();
        readonly List<int> blockedPorts = new List<int>();
        readonly List<int> allowedPorts = new List<int>();
        SeededRandom random;
        float spawnTimer;
        int nextId;

        public override MinigameType Type => MinigameType.PacketFilter;
        public IReadOnlyList<Packet> Packets => packets;
        public IReadOnlyList<int> BlockedPorts => blockedPorts;

        /// <summary>Bad packets blocked this round.</summary>
        public int BlockedCount { get; private set; }
        public int BlocksNeeded => settings.blocksPerRound;

        /// <summary>A bad packet reached the PC. Passes its port.</summary>
        public event Action<int> Leaked;

        /// <summary>The player blocked a good packet. Passes its port.</summary>
        public event Action<int> WrongBlock;

        public PacketFilterModel(PacketFilterSettings settings, MinigameSeed seed, int ticketId) : base(seed, ticketId)
        {
            this.settings = settings;
        }

        public bool IsBlocked(int port)
        {
            return blockedPorts.Contains(port);
        }

        public void ClickPacket(int id)
        {
            if (!IsRoundOpen) return;
            int index = packets.FindIndex(p => p.id == id);
            if (index < 0) return;

            int port = packets[index].port;
            packets.RemoveAt(index);
            if (IsBlocked(port))
            {
                BlockedCount++;
                if (BlockedCount >= settings.blocksPerRound) CompleteRound();
                return;
            }
            AddPressure(settings.wrongBlockPressure);
            WrongBlock?.Invoke(port);
        }

        protected override void OnTick(float deltaTime, bool connected)
        {
            spawnTimer += deltaTime;
            while (spawnTimer >= settings.spawnInterval)
            {
                spawnTimer -= settings.spawnInterval;
                Spawn();
            }

            float step = deltaTime / settings.travelTime;
            for (int i = packets.Count - 1; i >= 0; i--)
            {
                var packet = packets[i];
                packet.progress += step;
                if (packet.progress < 1f)
                {
                    packets[i] = packet;
                    continue;
                }
                packets.RemoveAt(i);
                if (!IsBlocked(packet.port)) continue;
                AddPressure(settings.leakPressure);
                Leaked?.Invoke(packet.port);
            }
        }

        // A new rule from this round's stream and an empty cable; the first packet enters right away.
        protected override void Generate(SeededRandom random, int round)
        {
            this.random = random;
            packets.Clear();
            BlockedCount = 0;

            allowedPorts.Clear();
            allowedPorts.AddRange(settings.ports);
            for (int i = allowedPorts.Count - 1; i > 0; i--)
            {
                int j = random.RangeInclusive(0, i);
                (allowedPorts[i], allowedPorts[j]) = (allowedPorts[j], allowedPorts[i]);
            }
            blockedPorts.Clear();
            int count = Math.Min(settings.BlockedPortCount(round), allowedPorts.Count);
            blockedPorts.AddRange(allowedPorts.GetRange(0, count));
            allowedPorts.RemoveRange(0, count);
            blockedPorts.Sort();

            spawnTimer = settings.spawnInterval;
        }

        void Spawn()
        {
            bool bad = allowedPorts.Count == 0 || random.Range(0f, 1f) < settings.badChance;
            var source = bad ? blockedPorts : allowedPorts;
            packets.Add(new Packet { id = nextId++, port = source[random.RangeInclusive(0, source.Count - 1)], progress = 0f });
        }
    }
}
