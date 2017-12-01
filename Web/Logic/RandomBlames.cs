namespace Web.Logic
{
    public class RandomBlames : RandomTextBase
    {
        private static readonly string[] Blames =
            {
                "Teit virheen. Puusilmä.", "Ei onnistunut. Yritä nyt edes.",
                "Aiaiaiaiai mikä moka.", "Hullu!", "Mokasit!",
                "Sinä suoralapainen torspo. Ei se noin mene"
            };

        public RandomBlames() : base(Blames)
        {
        }
    }
}