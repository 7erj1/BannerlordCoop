﻿using System;
using System.Collections.Generic;
using Coop.Common;
using Coop.Game.Patch;
using Coop.Multiplayer;
using Coop.Multiplayer.Network;
using Coop.Sync;
using JetBrains.Annotations;
using RailgunNet.Connection.Client;

namespace Coop.Game.Persistence
{
    public class PersistenceClient : IUpdateable
    {
        private readonly IEnvironmentClient m_Environment;
        private readonly RailClient m_RailClient;
        private readonly RailClientRoom m_Room;

        public PersistenceClient(IEnvironmentClient environment)
        {
            m_Environment = environment;
            m_RailClient = new RailClient(Registry.Client(environment));
            CampaignMapMovement.s_Environment = m_Environment;
            TimeControl.s_Environment = m_Environment;
            m_Room = m_RailClient.StartRoom();
        }

        public void Update(TimeSpan frameTime)
        {
            List<object> toRemove = new List<object>();
            foreach (KeyValuePair<Field, Dictionary<object, BufferedData>> fieldBuffer in
                FieldWatcher.BufferedChanges)
            {
                foreach (KeyValuePair<object, BufferedData> instanceBuffer in fieldBuffer.Value)
                {
                    if (CheckShouldRemove(
                        fieldBuffer.Key,
                        instanceBuffer.Key,
                        instanceBuffer.Value))
                    {
                        toRemove.Add(instanceBuffer.Key);
                    }
                    else if (!instanceBuffer.Value.Sent)
                    {
                        Field field = fieldBuffer.Key;
                        field.SyncHandler?.Invoke(instanceBuffer.Value.ToSend);
                        instanceBuffer.Value.Sent = true;
                    }
                }

                toRemove.ForEach(o => fieldBuffer.Value.Remove(o));
                toRemove.Clear();
            }

            m_RailClient.Update();
        }

        public void SetConnection([CanBeNull] ConnectionClient connection)
        {
            m_RailClient.SetPeer((RailNetPeerWrapper) connection?.GameStatePersistence);
        }

        private bool CheckShouldRemove(Field field, object instance, BufferedData buffer)
        {
            if (buffer.Sent && Equals(buffer.ToSend, buffer.Actual))
            {
                return true;
            }

            object currentValue = field.Get(instance);
            if (Equals(currentValue, buffer.Actual))
            {
                return false;
            }

            if (buffer.Sent)
            {
                return true;
            }

            buffer.Actual = currentValue;
            return false;
        }
    }
}
