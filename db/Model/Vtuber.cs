using System.Drawing;
using System.Reflection.Metadata;

namespace VManager.db.Model
{
    public class Vtuber
    {
        public ulong Id { get; set; }
        public long TelegramId { get; set; }
        public ulong TwitchId { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public bool Kicked { get; set; }
        public ICollection<Dates> Dates { get; set; }
        public ulong LastSubs { get; set; }
        public byte[] Image { get; set; } = new byte[0];
    }
}
