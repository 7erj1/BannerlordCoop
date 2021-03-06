﻿using System.Net;
using LiteNetLib;
using Network.Infrastructure;

namespace Coop.Multiplayer.Network
{
    public static class Extensions
    {
        public static ConnectionBase GetConnection(this NetPeer peer)
        {
            return (ConnectionBase) peer.Tag;
        }

        public static string ToFriendlyString(this IPEndPoint endPoint)
        {
            return $"{endPoint.Address}:{endPoint.Port}";
        }

        public static string ToFriendlyString(this ConnectionRequest request)
        {
            return $"{request.RemoteEndPoint.ToFriendlyString()}";
        }

        public static EDisconnectReason GetReason(this DisconnectInfo info, bool bIsServerSide)
        {
            // If the disconnect was intentional, a reason was attached.
            if (info.AdditionalData.AvailableBytes == 1)
            {
                return (EDisconnectReason) info.AdditionalData.GetByte();
            }

            // Otherwise the disconnect happened for network reasons. Translate.
            switch (info.Reason)
            {
                case DisconnectReason.ConnectionFailed:
                case DisconnectReason.HostUnreachable:
                case DisconnectReason.NetworkUnreachable:
                case DisconnectReason.UnknownHost:
                    return EDisconnectReason.Unreachable;
                case DisconnectReason.ConnectionRejected:
                case DisconnectReason.InvalidProtocol:
                    return EDisconnectReason.ConnectionRejected;
                case DisconnectReason.Timeout:
                case DisconnectReason.Reconnect:
                    return EDisconnectReason.Timeout;
                case DisconnectReason.DisconnectPeerCalled:
                case DisconnectReason.RemoteConnectionClose:
                    return bIsServerSide ?
                        EDisconnectReason.ClientUnreachable :
                        EDisconnectReason.Timeout;
            }

            return EDisconnectReason.Unknown;
        }
    }
}
