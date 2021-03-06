﻿using System;
using System.ComponentModel;
using JetBrains.Annotations;
using NLog;
using RailgunNet.Logic;
using Sync;
using TaleWorlds.CampaignSystem;

namespace Coop.Mod.Persistence.World
{
    public class WorldEntityClient : RailEntityClient<WorldState>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [NotNull] private readonly IEnvironmentClient m_Environment;

        public WorldEntityClient(IEnvironmentClient environment)
        {
            m_Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        private void RequestTimeControlChange(object value)
        {
            if (!(value is CampaignTimeControlMode))
            {
                throw new ArgumentException(nameof(value));
            }

            Logger.Trace(
                "[{tick}] Request time control mode '{mode}'.",
                Room.Tick,
                (CampaignTimeControlMode) value);
            Room.RaiseEvent<EventTimeControl>(
                e =>
                {
                    e.RequestedTimeControlMode = (CampaignTimeControlMode) value;
                    e.EntityId = Id;
                });
        }

        protected override void OnAdded()
        {
            m_Environment.TimeControlMode.SetSyncHandler(
                SyncableInstance.Any,
                RequestTimeControlChange);
            State.PropertyChanged += State_PropertyChanged;
        }

        private void State_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(State.TimeControlMode))
            {
                Logger.Trace(
                    "[{tick}] Received time controle mode change to '{mode}'.",
                    Room.Tick,
                    State.TimeControlMode);
                m_Environment.TimeControlMode.SetTyped(
                    m_Environment.GetCurrentCampaign(),
                    State.TimeControlMode);
            }
        }

        protected override void OnRemoved()
        {
            m_Environment.TargetPosition.RemoveSyncHandler(SyncableInstance.Any);
            State.PropertyChanged -= State_PropertyChanged;
        }

        public override string ToString()
        {
            return $"World ({Id}): {State}.";
        }
    }
}
