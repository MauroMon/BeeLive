namespace BeeLive.Hive.TransferModels
{
    public class HiveDto
    {
        public int Id { get; set; }
        public decimal Decibel { get; set; }
        public HiveStatus Status { get; set; }
        public string FormattedDecibel
        {
            get { return $"{Decibel.ToString("0.####")} db"; }
        }
    }
}