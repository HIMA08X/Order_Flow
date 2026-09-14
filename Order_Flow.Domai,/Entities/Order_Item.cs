using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.Domain.Entities
{
    public class Order_Item
    {
         public int Id { get; private set; }
         public int OrderId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
         public int Quantity { get;  private set; }
         public decimal UnitPrice { get; private set; }
        public Order_Item(string _productName, int _quantity, decimal _unitPrice)
        {
            if (string.IsNullOrEmpty(_productName))
                throw new ArgumentException("Product name cannot be null or empty", nameof(_productName));

            if (_quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(_quantity));

            if (_unitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than zero", nameof(_unitPrice));

            ProductName = _productName;
            Quantity = _quantity;
            UnitPrice = _unitPrice;
        }
        private Order_Item() { }
    }
}
