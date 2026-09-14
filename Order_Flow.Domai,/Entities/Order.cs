using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public string Status { get; set; } = "Pending";
        
        public List<Order_Item> Items { get; set; } = new List<Order_Item>();

        public Order(int customerId)
        {
            if (customerId <= 0) 
                throw new ArgumentException("Customer ID cannot be Zero or negative", nameof(customerId));
            CustomerId = customerId;
        }
        private Order () { }

        public decimal Total =>  Items.Sum(item => item.Quantity * item.UnitPrice);
        public void AddItem(Order_Item item)
        {
            Items.Add(item);
        }
        public void Complet()
        {
            Status = "Completed";
        }
    }

}
