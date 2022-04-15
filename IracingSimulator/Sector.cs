namespace iRacingSimulator
{
    public class Sector
    {
        public int Number { get; set; }
        public float StartPercentage { get; set; }

        public double EnterSessionTime { get; set; }
        public Laptime SectorTime { get; set; }

        public Sector Copy()
        {
            var s = new Sector();
            s.Number = this.Number;
            s.StartPercentage = this.StartPercentage;
            return s;
        }
    }
}
