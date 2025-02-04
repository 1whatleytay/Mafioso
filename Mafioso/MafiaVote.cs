using Discord.WebSocket;

namespace Mafioso {
    public struct MafiaVote {
        public readonly ulong Voter;
        public readonly ulong Channel;
        public readonly int Vote;
        
        private MafiaVote(ulong voter, ulong channel, int vote) {
            Voter = voter;
            Channel = channel;
            Vote = vote;
        }

        public MafiaVote(SocketMessage message, int vote) : this(message.Author.Id, message.Channel.Id, vote) { }
    }
}