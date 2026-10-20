using System;

namespace LostControl.Model
{
    /// <summary>
    /// The player's single remote session. Connecting takes connectionTime seconds, during which the
    /// ticket cannot be worked on and other clicks are ignored. Once connected, connecting to another
    /// ticket drops the current session at once.
    /// </summary>
    public class ConnectionModel
    {
        readonly float connectionTime;
        float elapsed;

        public ConnectionState State { get; private set; } = ConnectionState.None;

        /// <summary>The ticket being connected to or connected to; null when there is no session.</summary>
        public TicketModel Target { get; private set; }

        /// <summary>The ticket whose session was closed under the player (it blacked out); null after the next connect.</summary>
        public TicketModel KickedFrom { get; private set; }

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

        /// <summary>
        /// Starts connecting to a ticket. Ignored while another connection is still loading,
        /// for the current target and for closed (resolved or lost) tickets.
        /// </summary>
        public void Connect(TicketModel ticket)
        {
            if (ticket == null || ticket == Target || ticket.IsClosed || State == ConnectionState.Connecting) return;

            if (Target != null) Disconnected?.Invoke(Target);
            Target = ticket;
            KickedFrom = null;
            State = ConnectionState.Connecting;
            elapsed = 0f;
            ConnectStarted?.Invoke(ticket);
            if (connectionTime <= 0f) Complete();
        }

        /// <summary>Ends a connected session at once (no wait). Ignored while still connecting or without a session.</summary>
        public void Disconnect()
        {
            if (State != ConnectionState.Connected) return;

            Disconnected?.Invoke(Target);
            KickedFrom = null;
            Target = null;
            State = ConnectionState.None;
            elapsed = 0f;
        }

        /// <summary>Closes the session if it is on this ticket (connecting or connected).</summary>
        public void Kick(TicketModel ticket)
        {
            if (ticket == null || Target != ticket) return;

            Disconnected?.Invoke(ticket);
            KickedFrom = ticket;
            Target = null;
            State = ConnectionState.None;
            elapsed = 0f;
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
