namespace Web.Logic.Destruction
{
    using System.Collections.Generic;

    public class AlwaysDestructedCalculator : CalculatorBase
    {
        private static readonly List<DestructionDistanceStep> Steps = new List<DestructionDistanceStep>
            {
                new DestructionDistanceStep(6.5m, 48, 0m),
                new DestructionDistanceStep(15.9m, 48, -0.016m),
                new DestructionDistanceStep(31.1m, 47, -0.017m),
                new DestructionDistanceStep(42.4m, 42, -0.018m),
                new DestructionDistanceStep(53.0m, 33, -0.052m),
                new DestructionDistanceStep(63.7m, 22, -0.055m),
                new DestructionDistanceStep(71.5m, 11, -0.077m),
                new DestructionDistanceStep(83.1m, 5, -0.042m),
                new DestructionDistanceStep(94.7m, 1, 0m),
                new DestructionDistanceStep(100.0m, 1, 0m)
            };

        public AlwaysDestructedCalculator(PersonData person) : base(person, Steps)
        {
        }
    }
}