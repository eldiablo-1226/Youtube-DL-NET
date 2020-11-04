using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Youtube_DL.Helper
{
    internal class Query : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable, IReadOnlyDictionary<string, string>, IReadOnlyCollection<KeyValuePair<string, string>>
    {
        private int count;
        private readonly string baseUri;
        private KeyValuePair<string, string>[] pairs;

        public Query(string uri)
        {
            int length1 = uri.IndexOf('?');
            if (length1 == -1)
            {
                if (uri.IndexOf('&') == -1)
                {
                    this.baseUri = uri;
                    return;
                }
                this.baseUri = (string)null;
            }
            else
            {
                this.baseUri = uri.Substring(0, length1);
                uri = uri.Substring(length1 + 1);
            }
            string[] strArray1 = uri.Split('&');
            string[] strArray2 = EmptyArray<string>.Value;
            string[] strArray3 = EmptyArray<string>.Value;
            this.pairs = new KeyValuePair<string, string>[strArray1.Length];
            for (int index = 0; index < strArray1.Length; ++index)
            {
                string str1 = strArray1[index];
                int length2 = str1.IndexOf('=');
                string key = str1.Substring(0, length2);
                string str2 = length2 < str1.Length ? str1.Substring(length2 + 1) : string.Empty;
                this.pairs[index] = new KeyValuePair<string, string>(key, str2);
            }
            this.count = strArray1.Length;
        }

        public string this[string key]
        {
            get
            {
                for (int index = 0; index < this.count; ++index)
                {
                    KeyValuePair<string, string> pair = this.pairs[index];
                    if (pair.Key == key)
                        return pair.Value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                for (int index = 0; index < this.count; ++index)
                {
                    if (this.pairs[index].Key == key)
                    {
                        this.pairs[index] = new KeyValuePair<string, string>(key, value);
                        return;
                    }
                }
                throw new KeyNotFoundException();
            }
        }

        public string BaseUri => this.baseUri;

        public int Count => this.count;

        public bool IsReadOnly => false;

        public Query.KeyCollection Keys => new Query.KeyCollection(this);

        ICollection<string> IDictionary<string, string>.Keys => (ICollection<string>)this.Keys;

        public KeyValuePair<string, string>[] Pairs => this.pairs;

        public Query.ValueCollection Values => new Query.ValueCollection(this);

        ICollection<string> IDictionary<string, string>.Values => (ICollection<string>)this.Values;

        IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => (IEnumerable<string>)this.Keys;

        IEnumerable<string> IReadOnlyDictionary<string, string>.Values => (IEnumerable<string>)this.Values;

        void ICollection<KeyValuePair<string, string>>.Add(
          KeyValuePair<string, string> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(string key, string value)
        {
            this.EnsureCapacity(this.count + 1);
            this.pairs[this.count++] = new KeyValuePair<string, string>(key, value);
        }

        public void Clear()
        {
            if (this.count == 0)
                return;
            Array.Clear((Array)this.pairs, 0, this.count);
            this.count = 0;
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(
          KeyValuePair<string, string> item)
        {
            for (int index = 0; index < this.count; ++index)
            {
                KeyValuePair<string, string> pair = this.pairs[index];
                if (item.Key == pair.Key && item.Value == pair.Value)
                    return true;
            }
            return false;
        }

        public bool ContainsKey(string key)
        {
            for (int index = 0; index < this.count; ++index)
            {
                if (key == this.pairs[index].Key)
                    return true;
            }
            return false;
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(
          KeyValuePair<string, string>[] array,
          int arrayIndex)
        {
            Array.Copy((Array)this.pairs, 0, (Array)array, arrayIndex, this.count);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            for (int i = 0; i < this.count; ++i)
                yield return this.pairs[i];
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(
          KeyValuePair<string, string> item)
        {
            return this.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            for (int destinationIndex = 0; destinationIndex < this.count; ++destinationIndex)
            {
                if (this.pairs[destinationIndex].Key == key)
                {
                    if (destinationIndex != this.count--)
                        Array.Copy((Array)this.pairs, destinationIndex + 1, (Array)this.pairs, destinationIndex, this.count - destinationIndex);
                    this.pairs[this.count] = new KeyValuePair<string, string>();
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(string key)
        {
            for (int index = 0; index < this.count; ++index)
            {
                KeyValuePair<string, string> pair = this.pairs[index];
                if (key == pair.Key)
                {
                    return true;
                }
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        public override string ToString()
        {
            if (this.count == 0)
                return this.baseUri;
            StringBuilder stringBuilder = new StringBuilder();
            if (this.baseUri != null)
                stringBuilder.Append(this.baseUri).Append('?');
            KeyValuePair<string, string> pair = this.pairs[0];
            stringBuilder.Append(pair.Key).Append('=').Append(pair.Value);
            for (int index = 1; index < this.count; ++index)
            {
                pair = this.pairs[index];
                stringBuilder.Append('&').Append(pair.Key).Append('=').Append(pair.Value);
            }
            return stringBuilder.ToString();
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity <= this.pairs.Length)
                return;
            capacity = Math.Max(capacity, this.pairs.Length * 2);
            Array.Resize<KeyValuePair<string, string>>(ref this.pairs, capacity);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            throw new NotImplementedException();
        }

        public class KeyCollection : ICollection<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>
        {
            private readonly Query query;

            public KeyCollection(Query query) => this.query = query;

            public int Count => this.query.Count;

            public bool IsReadOnly => true;

            public void Add(string item) => throw new NotSupportedException();

            public void Clear() => throw new NotSupportedException();

            public bool Contains(string item)
            {
                for (int index = 0; index < this.query.Count; ++index)
                {
                    KeyValuePair<string, string> pair = this.query.Pairs[index];
                    if (item == pair.Key)
                        return true;
                }
                return false;
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                for (int index = 0; index < this.query.Count; ++index)
                    array[arrayIndex++] = this.query.Pairs[index].Key;
            }

            public IEnumerator<string> GetEnumerator()
            {
                for (int i = 0; i < this.query.Count; ++i)
                    yield return this.query.Pairs[i].Key;
            }

            public bool Remove(string item) => throw new NotSupportedException();

            IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();
        }

        public class ValueCollection : ICollection<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>
        {
            private readonly Query query;

            public ValueCollection(Query query) => this.query = query;

            public int Count => this.query.Count;

            public bool IsReadOnly => true;

            public void Add(string item) => throw new NotSupportedException();

            public void Clear() => throw new NotSupportedException();

            public bool Contains(string item)
            {
                for (int index = 0; index < this.query.Count; ++index)
                {
                    KeyValuePair<string, string> pair = this.query.Pairs[index];
                    if (item == pair.Value)
                        return true;
                }
                return false;
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                for (int index = 0; index < this.query.Count; ++index)
                    array[arrayIndex++] = this.query.Pairs[index].Value;
            }

            public IEnumerator<string> GetEnumerator()
            {
                for (int i = 0; i < this.query.Count; ++i)
                    yield return this.query.Pairs[i].Value;
            }

            public bool Remove(string item) => throw new NotSupportedException();

            IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();
        }
    }
}