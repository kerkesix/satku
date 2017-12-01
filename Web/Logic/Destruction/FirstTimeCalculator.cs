namespace Web.Logic.Destruction
{
    using System.Collections.Generic;

    public class FirstTimeCalculator : CalculatorBase
    {
        private static readonly List<DestructionDistanceStep> Steps = new List<DestructionDistanceStep>
            {
                new DestructionDistanceStep(6.5m, 59, 0m),
                new DestructionDistanceStep(15.9m, 59, -0.016m),
                new DestructionDistanceStep(31.1m, 59, -0.017m),
                new DestructionDistanceStep(42.4m, 56, -0.018m),
                new DestructionDistanceStep(53.0m, 46, -0.052m),
                new DestructionDistanceStep(63.7m, 35, -0.055m),
                new DestructionDistanceStep(71.5m, 16, -0.077m),
                new DestructionDistanceStep(83.1m, 8, -0.042m),
                new DestructionDistanceStep(94.7m, 1, 0m),
                new DestructionDistanceStep(100.0m, 1, 0m)
            };

        public FirstTimeCalculator(PersonData person) : base(person, Steps)
        {
        }
    }
}