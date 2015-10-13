namespace WaveInfo.Extensions
{
    public static class SizeExtensions
    {
        public static uint Normalize(this uint value)
        {
            if (value%2 == 0)
            {
                return value;
            }

            return value + 1;
        }


        public static ulong Normalize(this ulong value)
        {
            if (value%2 == 0)
            {
                return value;
            }

            return value + 1;
        }
    }
}