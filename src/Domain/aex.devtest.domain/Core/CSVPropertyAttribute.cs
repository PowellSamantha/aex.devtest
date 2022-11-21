using System;

namespace aex.devtest.domain.Core
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CSVPropertyAttribute : Attribute
    {
        private readonly int _order;
        
        public CSVPropertyAttribute(int order)
        {
            _order = order;
        }

        public int Order { get { return _order; } }
    }
}
