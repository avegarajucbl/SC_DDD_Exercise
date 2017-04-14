using System;
using Domain;
using FluentAssertions;
using Xunit;

namespace Tests.Unit
{
    public class CheckoutTests
    {
        /// <summary>
        /// Given a cart has products
        /// And the payment amount equals the total sales price of the products
        /// When the customer checks out
        /// Then an order is created
        /// And payment is pending verification
        /// </summary>
        [Fact]
        public void Checkout()
        {
            var cart = new Cart();

            var product1 = new Product(price: 8.5m);
            var product2 = new Product(price: 15.99m);
            
            cart.Add(product1);
            cart.Add(product2);

            var customer = new Customer();
            var payment = new Payment(amount: product1.Price + product2.Price);
            var order = cart.Checkout(customer, payment);

            order.Payment.State.Should().Be(PaymentState.NotVerified);
        }

        /// <summary>
        /// Given a cart has products
        /// And the payment amount does not equal the total sales price of the products
        /// When the customer checks out
        /// Then an exception is thrown in its face
        /// And there is much gnashing of teeth
        /// </summary>
        [Fact]
        public void CheckoutWithDisappointingAmount()
        {
            var cart = new Cart();

            var product1 = new Product(price: 8.5m);
            var product2 = new Product(price: 15.99m);

            cart.Add(product1);
            cart.Add(product2);

            var customer = new Customer();
            var payment = new Payment(amount: product1.Price + product2.Price - 1m);

            cart.Invoking(obj => obj.Checkout(customer, payment))
                .ShouldThrow<PaymentSizeMattersException>();
        }

        /// <summary>
        /// Given an order exists
        /// And payment is pending
        /// When the payment provider denies the payment
        /// Then the payment is rejected
        /// </summary>
        [Fact]
        public void PaymentDenied()
        {
            var order = new Order(new Payment(amount:20m));

            order.DenyPayment();

            order.Payment.State.Should().Be(PaymentState.Rejected);
        }

        /// <summary>
        /// Given an order exists
        /// And payment is pending
        /// When the payment provider confirms the payment
        /// Then the payment is verified
        /// </summary>
        [Fact]
        public void PaymentConfirmed()
        {
            var order = new Order(new Payment(amount: 20m));

            order.VerifyPayment();

            order.Payment.State.Should().Be(PaymentState.Verified);
        }
    }
}