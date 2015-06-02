namespace HealthNet
{
    class TestHealthNetConfiguration : HealthNetConfiguration
    {
        private readonly string overridePath;

        public TestHealthNetConfiguration(string overridePath = null)
        {
            this.overridePath = overridePath;
        }

        public override string Path
        {
            get { return overridePath ?? base.Path; }
        }
    }
}