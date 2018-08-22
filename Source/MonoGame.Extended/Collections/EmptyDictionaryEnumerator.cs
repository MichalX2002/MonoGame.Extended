using System.Collections;

namespace MonoGame.Extended.Collections
{
    public class EmptyDictionaryEnumerator : IDictionaryEnumerator
    {
        public static readonly EmptyDictionaryEnumerator Instance;

        public object Key => null;
        public object Value => null;

        public DictionaryEntry Entry => default;
        public object Current => null;

        static EmptyDictionaryEnumerator()
        {
            Instance = new EmptyDictionaryEnumerator();
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }
    }
}
