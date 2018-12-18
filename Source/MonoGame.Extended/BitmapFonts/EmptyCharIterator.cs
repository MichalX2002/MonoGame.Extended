
namespace MonoGame.Extended.BitmapFonts
{
    public class EmptyCharIterator : ICharIterator
    {
        public static readonly EmptyCharIterator Instance = new EmptyCharIterator();
        
        public int Length => 0;

        private EmptyCharIterator()
        {
        }
        
        public char GetCharacter16(int index)
        {
            return default;
        }

        public int GetCharacter32(ref int index)
        {
            return 0;
        }

        public void Dispose()
        {
        }
    }
}
