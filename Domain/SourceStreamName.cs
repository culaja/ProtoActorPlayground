using System.Collections.Generic;
using Framework;

namespace Domain
{
    public sealed class SourceStreamName : ValueObject
    {
        private readonly string _name;

        private SourceStreamName(string name)
        {
            _name = name;
        }
        
        public static SourceStreamName Of(string name) => new SourceStreamName(name);
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _name;
        }

        public override string ToString() => _name;

        public static implicit operator string(SourceStreamName sourceStreamName) => sourceStreamName.ToString();
    }
}