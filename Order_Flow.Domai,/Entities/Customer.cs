using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.Domain.Entities
{
    public class Customer
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public Customer (string _name)
        {
            if (string.IsNullOrEmpty(_name))
                throw new ArgumentException("Customer name cannot be null or empty", nameof(_name));
            Name = _name;
        }
        private Customer() { }
    }
}
