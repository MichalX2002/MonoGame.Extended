using System.Collections;

namespace MonoGame.Extended.Collections
{
    public struct EmptyDictionaryEnumerator : IDictionaryEnumerator
    {
        public object Key => null;
        public object Value => null;

        public DictionaryEntry Entry => default;
        public object Current => null;

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }
    }
}
