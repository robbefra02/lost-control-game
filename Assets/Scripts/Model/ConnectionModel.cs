using System;

namespace LostControl.Model
{
    /// <summary>
    /// The player's single remote session. Connecting takes connectionTime seconds, during which the
    /// ticket cannot be worked on. Connecting to another ticket drops the current session at once.
    /// </summary>
    public class ConnectionModel
    {
        readonly float connectionTime;
        float elapsed;

        public ConnectionState State { get; private set; } = ConnectionState.None;

        /// <summary>The ticket being connected to or connected to; null when there is no session.</summary>
        public TicketModel Target { get; private set; }

        /// <summary>0-1 progress of the current connection attempt; 1 once connected.</summary>
        public float Progress => State == ConnectionState.Connected ? 1f
            : State == ConnectionState.Connecting && connectionTime > 0f ? Math.Min(elapsed / connectionTime, 1f)
            : 0f;

        public event Action<TicketModel> ConnectStarted;
        public event Action<TicketModel> ConnectDone;
        public event Action<TicketModel> Disconnected;

        public ConnectionModel(float connectionTime)
        {
            this.connectionTime = connectionTime;
        }

        public bool IsConnectedTo(TicketModel ticket)
        {
            return State == ConnectionState.Connected && Target == ticket;
        }

        /// <summary>Starts connecting to a ticket. Ignored for the current target and for resolved tickets.</summary>
        public void Connect(TicketModel ticket)
        {
            if (ticket == null || ticket == Target || ticket.IsResolved) return;

            if (Target != null) Disconnected?.Invoke(Target);
            Target = ticket;
            State = ConnectionState.Connecting;
            elapsed = 0f;
            ConnectStarted?.Invoke(ticket);
            if (connectionTime <= 0f) Complete();
        }

        public void Tick(float deltaTime)
        {
            if (State != ConnectionState.Connecting) return;
            elapsed += deltaTime;
            if (elapsed >= connectionTime) Complete();
        }

        void Complete()
        {
            State = ConnectionState.Connected;
            ConnectDone?.Invoke(Target);
        }
    }
}
