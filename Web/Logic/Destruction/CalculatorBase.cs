namespace Web.Logic.Destruction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class CalculatorBase : IDestructionCalculator
    {
        private readonly List<DestructionDistanceStep> steps;

        protected CalculatorBase(PersonData person, List<DestructionDistanceStep> steps)
        {
            this.Person = person ?? throw new ArgumentNullException(nameof(person));
            this.steps = steps ?? throw new ArgumentNullException(nameof(steps));
        }

        protected PersonData Person { get; private set; }

        /// <summary>
        /// Default implementation of calculation. Override in derived class 
        /// if more complex calculation is desired.
        /// </summary>
        /// <param name="walkingData"></param>
        /// <returns></returns>
        public virtual int Calculate(WalkingData walkingData)
        {
            if (walkingData == null)
            {
                throw new ArgumentNullException(nameof(walkingData));
            }

            var step = this.FindCurrentStep(walkingData.CurrentDistance);
            int result = step.DestructionPropability;

            // Alter propability if attendees speed has increased or decreased 
            // more than normally
            decimal speedChangeFactor = this.CalculateSpeedChangeFactor(
                walkingData.LastAverageSpeedChange, step.NormalSpeedChange);
            result = (int)(result * speedChangeFactor);

            // Never go below 1
            if (result < 1)
            {
                result = 1;
            }

            // Never go over 99
            if (result > 99)
            {
                result = 99;
            }

            return result;
        }

        protected DestructionDistanceStep FindCurrentStep(decimal distance)
        {
            if (distance == 0m)
            {
                return this.steps.First();
            }

            if (distance >= 100m)
            {
                return this.steps.Last();
            }

            var step = this.steps.Last(m => m.Distance <= distance);
            return step;
        }

        /// <summary>
        /// Default implementation of attendees average speed change factor to his overall 
        /// destruction percentage. If this factor is over one, then it increases 
        /// the probability to quit, if below one, then decreases.
        /// </summary>
        /// <param name="lastAverageSpeedChange">Attendees last steps average speed change 
        /// compared to the step before that</param>
        /// <param name="normalSpeedChange">Normal speed change on this same step</param>
        /// <returns>The factor used to change quit percentage</returns>
        protected virtual decimal CalculateSpeedChangeFactor(decimal lastAverageSpeedChange, decimal normalSpeedChange)
        {
            if (lastAverageSpeedChange == normalSpeedChange)
            {
                return 1m;
            }

            decimal change = lastAverageSpeedChange;

            if (normalSpeedChange != 0m)
            {
                // Calculate percentage deviance from normal
                change = (normalSpeedChange - lastAverageSpeedChange) / normalSpeedChange;
            }

            // Take quarter of the change into account as percentage change
            decimal factor = Math.Abs(1 - (change / 4));

            return factor;
        }
    }
}