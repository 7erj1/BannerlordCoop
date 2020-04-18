﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coop.Network
{
    public enum EDisconnectReason : byte
    {
        ClientLeft,
        ServerIsFull,
        ServerShutDown,
        WrongProtocolVersion,
        Unknown
    }
    public interface INetworkConnection
    {
        int FragmentLength { get; }
        int MaxPackageLength { get; }
        void SendRaw(byte[] raw);        
        void Close(EDisconnectReason eReason);
    }
}