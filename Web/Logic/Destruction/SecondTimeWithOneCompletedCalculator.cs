namespace Web.Logic.Destruction
{
    using System.Collections.Generic;

    public class SecondTimeWithOneCompletedCalculator : CalculatorBase
    {
        private static readonly List<DestructionDistanceStep> Steps = new List<DestructionDistanceStep>
            {
                new DestructionDistanceStep(6.5m, 23, 0m),
                new DestructionDistanceStep(15.9m, 23, -0.016m),
                new DestructionDistanceStep(31.1m, 23, -0.017m),
                new DestructionDistanceStep(42.4m, 19, -0.018m),
                new DestructionDistanceStep(53.0m, 16, -0.052m),
                new DestructionDistanceStep(63.7m, 10, -0.055m),
                new DestructionDistanceStep(71.5m, 4, -0.077m),
                new DestructionDistanceStep(83.1m, 2, -0.042m),
                new DestructionDistanceStep(94.7m, 1, 0m),
                new DestructionDistanceStep(100.0m, 1, 0m)
            };

        public SecondTimeWithOneCompletedCalculator(PersonData person) : base(person, Steps)
        {
        }
    }
}