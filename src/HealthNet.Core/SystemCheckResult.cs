namespace HealthNet
{
    public class SystemCheckResult
    {
        public HealthState Health { get; set; }
        public bool IsVital { get; set; }
        public string SystemName { get; set; }
        public string Message { get; set; }
    }
}