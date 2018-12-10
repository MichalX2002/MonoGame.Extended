namespace MonoGame.Extended.Particles
{
    public static class FastRandomExtensions
    {
        public static void NextColor(this FastRandom random, out HslColor color, Range<HslColor> range)
        {
            HslColor min = range.Min;
            HslColor max = range.Max;
            float maxH = max.H >= min.H ? max.H : max.H + 360;

            color = new HslColor(
                random.NextSingle(min.H, maxH),
                random.NextSingle(min.S, max.S),
                random.NextSingle(min.L, max.L));
        }
    }
}