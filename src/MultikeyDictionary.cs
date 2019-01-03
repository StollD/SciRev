using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SciRev
{
    public class MultikeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        private class Enumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> _backend;
            
            public Boolean MoveNext()
            {
                return _backend.MoveNext();
            }

            public void Reset()
            {
                _backend.Reset();
            }

            public Object Current
            {
                get { return _backend.Current; }
            }

            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(Key, Value); }
            }

            public Object Key
            {
                get { return _backend.Current.Key; }
            }

            public Object Value
            {
                get { return _backend.Current.Value; }
            }

            public Enumerator(IEnumerator<KeyValuePair<TKey, TValue>> backend)
            {
                _backend = backend;
            }
        }
        
        private readonly List<KeyValuePair<TKey, TValue>> _backend;

        public MultikeyDictionary()
        {
            _backend = new List<KeyValuePair<TKey, TValue>>(); 
        }

        public Boolean Contains(Object key)
        {
            return ContainsKey((TKey) key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(GetEnumerator());
        }

        public void Remove(Object key)
        {
            Remove((TKey) key);
        }

        public Boolean IsFixedSize
        {
            get { return (_backend as IList).IsFixedSize; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _backend.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _backend.Add(item);
        }

        public void Add(Object key, Object value)
        {
            Add((TKey)key, (TValue)value);
        }

        public void Clear()
        {
            _backend.Clear();
        }

        public Boolean Contains(KeyValuePair<TKey, TValue> item)
        {
            return _backend.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex)
        {
            _backend.CopyTo(array, arrayIndex);
        }

        public Boolean Remove(KeyValuePair<TKey, TValue> item)
        {
            return _backend.Remove(item);
        }

        public void CopyTo(Array array, Int32 index)
        {
            (_backend as IList).CopyTo(array, index);
        }

        public Int32 Count
        {
            get { return _backend.Count; }
        }

        public Boolean IsSynchronized
        {
            get { return (_backend as IList).IsSynchronized; }
        }

        public Object SyncRoot
        {
            get { return (_backend as IList).SyncRoot; }
        }

        public Boolean IsReadOnly
        {
            get { return false; }
        }

        public Object this[Object key]
        {
            get { return this[(TKey)key]; }
            set { this[(TKey) key] = (TValue) value; }
        }

        public void Add(TKey key, TValue value)
        {
            _backend.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public Boolean ContainsKey(TKey key)
        {
            return _backend.Any(k => k.Key.Equals(key));
        }

        public Boolean Remove(TKey key)
        {
            return _backend.RemoveAll(k => k.Key.Equals(key)) > 0;
        }

        public Boolean TryGetValue(TKey key, out TValue value)
        {
            if (_backend.Any(k => k.Key.Equals(key)))
            {
                value = _backend.Find(k => k.Key.Equals(key)).Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get { return _backend.Find(k => k.Key.Equals(key)).Value; }
            set { _backend.Add(new KeyValuePair<TKey, TValue>(key, value)); }
        }

        public ICollection<TKey> Keys
        {
            get { return _backend.Select(k => k.Key).ToList(); }
        }

        ICollection IDictionary.Values
        {
            get { return _backend.Select(k => k.Value).ToList(); }
        }

        ICollection IDictionary.Keys
        {
            get { return _backend.Select(k => k.Key).ToList(); }
        }

        public ICollection<TValue> Values
        {
            get { return _backend.Select(k => k.Value).ToList(); }
        }
    }
}