﻿using System;
using JetBrains.Annotations;
using NLog;
using RailgunNet.Logic;
using TaleWorlds.CampaignSystem;

namespace Coop.Mod.Persistence.World
{
    public class WorldEntityServer : RailEntityServer<WorldState>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEnvironmentServer m_Environment;

        public WorldEntityServer(IEnvironmentServer environment)
        {
            m_Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        [CanBeNull] public CampaignTimeControlMode? RequestedTimeControlMode { get; set; }

        protected override void UpdateAuthoritative()
        {
            if (RequestedTimeControlMode.HasValue && m_Environment.CanChangeTimeControlMode)
            {
                Logger.Trace("Changing time control to {request}.", RequestedTimeControlMode.Value);
                State.TimeControlMode = RequestedTimeControlMode.Value;
                RequestedTimeControlMode = null;
            }
        }

        public override string ToString()
        {
            return $"World ({Id}): {State}.";
        }
    }
}
