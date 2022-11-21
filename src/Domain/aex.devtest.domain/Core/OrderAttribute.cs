using System;

namespace aex.devtest.domain.Core
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order_;
        public OrderAttribute(int order = 0)
        {
            order_ = order;
        }

        public int Order { get { return order_; } }
    }
}
