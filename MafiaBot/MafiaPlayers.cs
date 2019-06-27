using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace MafiaBot {
    public class MafiaPlayers : MafiaChannels {
        private const double MafiaPercentage = 1.0 / 6.0; // Rounded up
        private const double DoctorPercentage = 1.0 / 5.0; // Rounded down
        private const double InvestigatorPercentage = 1.0 / 7.0; // Rounded up
        private const double SilencerPercentage = 1.0 / 7.0;
        
        protected readonly List<MafiaPlayer> Players = new List<MafiaPlayer>();
        protected readonly List<MafiaPlayer> Killed = new List<MafiaPlayer>();

        private static readonly MafiaPlayer.Role[] GoodRoles = {
            MafiaPlayer.Role.Citizen,
            MafiaPlayer.Role.Doctor,
            MafiaPlayer.Role.Investigator
        };
        
        private static readonly MafiaPlayer.Role[] MafiaRoles = {
            MafiaPlayer.Role.Mafia
        };

        private static readonly MafiaPlayer.Role[] NeutralRoles = {
            MafiaPlayer.Role.Silencer
        };

        protected static bool IsGood(MafiaPlayer player) {
            return GoodRoles.Contains(player.GetRole());
        }

        protected static bool IsMafia(MafiaPlayer player) {
            return MafiaRoles.Contains(player.GetRole());
        }

        protected static bool IsNotMafia(MafiaPlayer player) {
            return !IsMafia(player);
        }

        protected static bool IsNeutral(MafiaPlayer player) {
            return NeutralRoles.Contains(player.GetRole());
        }
        
        protected async Task Kill(MafiaPlayer player) {
            Players.Remove(player);
            Killed.Add(player);

            await ChannelVisibility(GetGeneral(), Killed, x => false, true);
        }

        private void AssignPool(List<MafiaPlayer> pool, int count, MafiaPlayer.Role role) {
            for (var a = 0; a < count; a++) {
                var player = pool[Utils.Random.Next(pool.Count)];
                player.AssignRole(role);
                pool.Remove(player);
            }
        }

        protected async Task AssignRoles() {
            var mafiaCount = (int)Math.Ceiling(Players.Count * MafiaPercentage);
            var doctorCount = (int)Math.Floor(Players.Count * DoctorPercentage);
            var investigatorCount = (int)Math.Floor(Players.Count * InvestigatorPercentage);
            var silencerCount = (int)Math.Ceiling(Players.Count * SilencerPercentage);
            
            var pool = new List<MafiaPlayer>(Players);

            AssignPool(pool, mafiaCount, MafiaPlayer.Role.Mafia);
            AssignPool(pool, doctorCount, MafiaPlayer.Role.Doctor);
            AssignPool(pool, investigatorCount, MafiaPlayer.Role.Investigator);
            AssignPool(pool, silencerCount, MafiaPlayer.Role.Silencer);
            
            foreach (var player in Players) {
                await player.TellRole();
            }
        }

        public bool HasPlayer(ulong player) {
            return Players.Exists(x => x.GetId() == player);
        }

        protected MafiaPlayers(DiscordSocketClient client, ulong guildId) : base(client, guildId) { }
    }
}