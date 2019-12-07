using System.Collections.Generic;
using Framework;

namespace Domain
{
    public sealed class StreamName : ValueObject
    {
        private readonly string _name;

        private StreamName(string name)
        {
            _name = name;
        }
        
        public static StreamName Of(string name) => new StreamName(name);
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _name;
        }

        public override string ToString() => _name;

        public static implicit operator string(StreamName streamName) => streamName.ToString();
    }
}