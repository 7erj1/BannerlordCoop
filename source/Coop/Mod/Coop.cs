﻿namespace Coop.Mod
{
    public static class Coop
    {
        public static bool IsServer => CoopServer.Instance.Current != null;
        public static bool IsClient => CoopClient.Instance.Connected;
        public static bool IsServerSimulation => IsServer && !IsClient;

        /// <summary>
        ///     The arbiter is the game instance with authority over all clients.
        /// </summary>
        public static bool IsArbiter =>
            IsServer && IsClient; // The server currently runs in the hosts game session.
    }
}
