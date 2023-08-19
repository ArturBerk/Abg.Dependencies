using System;
using System.Collections;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    public struct TypeCollection
    {
        private Type type;
        private List<Type> types;

        public TypeCollection(Type type) : this()
        {
            this.type = type;
        }

        public void Add(Type type)
        {
            if (types == null)
            {
                types = new List<Type> { this.type };
            }

            if (types.Contains(type)) return;
            types.Add(type);
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<Type>
        {
            private TypeCollection collection;
            private int index;

            public Enumerator(TypeCollection collection) : this()
            {
                this.collection = collection;
                Reset();
            }

            public void Dispose()
            {
                Reset();
            }

            public bool MoveNext()
            {
                ++index;
                if (collection.types != null) return index < collection.types.Count;
                return index == 0;
            }

            public void Reset()
            {
                index = -1;
            }

            public Type Current
            {
                get
                {
                    if (collection.types != null) return collection.types[index];
                    if (index == 0) return collection.type;
                    throw new IndexOutOfRangeException();
                }
            }

            object IEnumerator.Current => Current;
        }
        
    }
}