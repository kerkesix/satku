namespace Web.Logic.Destruction
{
    using System.Collections.Generic;

    public class ExperiencedButDestructedLastTimeCalculator : CalculatorBase
    {
        private static readonly List<DestructionDistanceStep> Steps = new List<DestructionDistanceStep>
            {
                new DestructionDistanceStep(6.5m, 63, 0m),
                new DestructionDistanceStep(15.9m, 63, -0.016m),
                new DestructionDistanceStep(31.1m, 63, -0.017m),
                new DestructionDistanceStep(42.4m, 59, -0.018m),
                new DestructionDistanceStep(53.0m, 52, -0.052m),
                new DestructionDistanceStep(63.7m, 40, -0.055m),
                new DestructionDistanceStep(71.5m, 22, -0.077m),
                new DestructionDistanceStep(83.1m, 10, -0.042m),
                new DestructionDistanceStep(94.7m, 2, 0m),
                new DestructionDistanceStep(100.0m, 1, 0m)
            };

        public ExperiencedButDestructedLastTimeCalculator(PersonData person) : base(person, Steps)
        {
        }
    }
}