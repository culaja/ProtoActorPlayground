using System.Collections.Generic;
using Framework;

namespace Domain
{
    public sealed class StreamPrefix : ValueObject
    {
        private readonly string _name;

        private StreamPrefix(string name)
        {
            _name = name;
        }
        
        public static StreamPrefix Of(string name) => new StreamPrefix(name);
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _name;
        }

        public override string ToString() => _name;

        public static implicit operator string(StreamPrefix streamPrefix) => streamPrefix.ToString();
    }
}