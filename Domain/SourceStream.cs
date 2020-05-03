using System.Collections.Generic;
using Framework;

namespace Domain
{
    public sealed class SourceStream : ValueObject
    {
        private readonly string _name;

        private SourceStream(string name)
        {
            _name = name;
        }
        
        public static SourceStream Of(string name) => new SourceStream(name);
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _name;
        }

        public override string ToString() => _name;

        public static implicit operator string(SourceStream sourceStream) => sourceStream.ToString();
    }
}