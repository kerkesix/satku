namespace Web.Logic
{
    using System.Collections.Generic;

    using Web.Models;

    public interface IAverageCheckpointTimeCalculator
    {
        int Calculate(
            IReadOnlyList<IReadingCalculation> readingsIn, 
            IReadOnlyList<IReadingCalculation> readingsOut);
    }
}
