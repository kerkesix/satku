namespace Web.Logic.Destruction
{
    /// <summary>
    /// Categorizes given person data and creates calculator class that 
    /// is correct for persons category.
    /// </summary>
    public static class CalculatorFactory
    {
        public static IDestructionCalculator Create(PersonData person)
        {
            // Current happening is already included --> 1 attended is first time
            if (person.Attended == 1)
            {
                return new FirstTimeCalculator(person);
            }

            if (person.Attended == person.Destructed)
            {
                return new AlwaysDestructedCalculator(person);
            }

            if (person.Attended == 2)
            {
                return new SecondTimeWithOneCompletedCalculator(person);
            }

            if (person.Attended >= 4 && person.DestructedInPrevious)
            {
                return new ExperiencedButDestructedLastTimeCalculator(person);
            }

            return new ExperiencedCalculator(person);
        }
    }
}