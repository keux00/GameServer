﻿using GameServerCore;
using GameServerCore.Packets.Handlers;
using GameServerCore.Packets.PacketDefinitions.Requests;
using System;

namespace LeagueSandbox.GameServer.Packets.PacketHandlers
{
    public class HandleSurrender : PacketHandlerBase<SurrenderRequest>
    {
        private readonly Game _game;
        private readonly IPlayerManager _pm;

        public HandleSurrender(Game game)
        {
            _game = game;
            _pm = game.PlayerManager;
        }

        public override bool HandlePacket(int userId, SurrenderRequest req)
        {
            var c = _pm.GetPeerInfo(userId).Champion;
            _game.Map.HandleSurrender(userId, c, req.VotedYes);
            return true;
        }
    }
}
