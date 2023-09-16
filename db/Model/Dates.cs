namespace VManager.db.Model
{
    public class Dates
    {
        public ulong Id { get; set; }
        public string Date_Description { get; set; }
        public DateTime Time { get; set; }
        public ulong VtuberId { get; set; }
        public Vtuber Vtuber { get; set; }
        public bool Important { get; set; }
    }
}
