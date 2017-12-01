namespace Web.Logic
{
    public class RandomTestSucceededTexts : RandomTextBase
    {
        private static readonly string[] Texts =
            {
                "Miltä nyt tuntuu?", "Pimeyttä kohti!", "Bujakaa!", "Helpottiko?",
                "Ei armoa kävelijöille.", "Leuka rinnassa kohti uusia pettymyksiä.",
                "Muista, koskaan ei ole liian myöhäistä keskeyttää.",
                "HAL on idolini, minulta puuttuu vain punainen silmä.",
                "Puumaa mä metsästän."
            };

        public RandomTestSucceededTexts()
            : base(Texts)
        {
        }
    }
}