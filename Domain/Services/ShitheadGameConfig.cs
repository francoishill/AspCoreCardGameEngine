namespace AspCoreCardGameEngine.Domain.Services
{
    public class ShitheadGameConfig
    {
        public int Joker { get; set; }

        public int Reset { get; set; }
        public int Invisible { get; set; }
        public int Reverse { get; set; }
        public int Burn { get; set; }
        public int Skip { get; set; }

        public static ShitheadGameConfig Default { get; } = new ShitheadGameConfig
        {
            Joker = 14,

            Reset = 2,
            Invisible = 3,
            Reverse = 7,
            Burn = 10,
            Skip = 14,
        };
    }
}