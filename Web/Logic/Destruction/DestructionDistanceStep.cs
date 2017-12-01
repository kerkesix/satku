namespace Web.Logic.Destruction
{
    public struct DestructionDistanceStep
    {
        public DestructionDistanceStep(decimal distance, int destructionPropability, decimal normalSpeedChange)
            : this()
        {
            this.Distance = distance;
            this.DestructionPropability = destructionPropability;
            this.NormalSpeedChange = normalSpeedChange;
        }

        public decimal Distance { get; private set; }

        public int DestructionPropability { get; private set; }

        public decimal NormalSpeedChange { get; private set; }
    }
}