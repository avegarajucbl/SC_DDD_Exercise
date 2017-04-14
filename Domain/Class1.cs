using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Cart
    {
        public void Add(Product product)
        {
            _products.Add(product);
        }

        public void SetQuantity(Product product, uint quantity)
        {
        }

        public Order Checkout(Customer customer, Payment payment)
        {
            if (payment.Amount < DetermineTotalPrice())
            {
                throw new PaymentSizeMattersException();
            }

            return new Order(payment);
        }

        private decimal DetermineTotalPrice()
        {
            return _products.Sum(p => p.Price);
        }

        private readonly List<Product> _products = new List<Product>();
    }

    public class Customer
    {
    }

    public class Order
    {
        public Order(Payment payment)
        {
            Payment = payment;
        }

        public Payment Payment { get; private set; }

        public void DenyPayment()
        {
            Payment.Reject();
        }

        public void VerifyPayment()
        {
            Payment.Verify();
        }
    }

    public class Payment
    {
        public Payment(decimal amount)
        {
            this.Amount = amount;
        }

        public decimal Amount { get; }
        public PaymentState State { get; private set; }

        public void Reject()
        {
            State = PaymentState.Rejected;
        }

        public void Verify()
        {
            State = PaymentState.Verified;
        }
    }

    public class Product
    {
        public decimal Price { get; }

        public Product(decimal price)
        {
            this.Price = price;
        }
    }

    public enum PaymentState
    {
        NotVerified,
        Verified,
        Rejected
    }

    public class PaymentSizeMattersException : Exception
    {
    }
}
