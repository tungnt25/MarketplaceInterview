using Marketplace.Interview.Business.Basket;
using Marketplace.Interview.Business.Shipping;
using NUnit.Framework;
using System.Collections.Generic;

namespace Marketplace.Interview.Tests
{
    [TestFixture]
    public class ShippingOptionTests
    {
        [Test]
        public void FlatRateShippingOptionTest()
        {
            var flatRateShippingOption = new FlatRateShipping { FlatRate = 1.5m };
            var shippingAmount = flatRateShippingOption.GetAmount(new LineItem(), new Basket());

            Assert.That(shippingAmount, Is.EqualTo(1.5m), "Flat rate shipping not correct.");
        }

        [Test]
        public void PerRegionShippingOptionTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
                                              {
                                                  PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               }
                                                                       },
                                              };

            var shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.Europe }, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(1.5m));

            shippingAmount = perRegionShippingOption.GetAmount(new LineItem() { DeliveryRegion = RegionShippingCost.Regions.UK }, new Basket());
            Assert.That(shippingAmount, Is.EqualTo(.75m));
        }

        [Test]
        public void BasketShippingTotalTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               }
                                                                       },
            };

            var flatRateShippingOption = new FlatRateShipping { FlatRate = 1.1m };

            var basket = new Basket()
                             {
                                 LineItems = new List<LineItem>
                                                 {
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             Shipping = perRegionShippingOption,
                                                             TypeOfShipping = "PerRegion"
                                                         },
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.Europe,
                                                             Shipping = perRegionShippingOption,
                                                             TypeOfShipping = "PerRegion"
                                                         },
                                                     new LineItem() {
                                                         Shipping = flatRateShippingOption,
                                                         TypeOfShipping = "FlatRate"
                                                     },
                                                 }
                             };

            var calculator = new ShippingCalculator();

            decimal basketShipping = calculator.CalculateShipping(basket);

            Assert.That(basketShipping, Is.EqualTo(3.35m));
        }

        //test case with new option at least one other item in the basket with the same Shipping Option and the same Supplier and Region
        [Test]
        public void BasketShippingWithNewOptionTotalTest()
        {
            var perRegionShippingOption = new PerRegionShipping()
            {
                PerRegionCosts = new[]
                                                                       {
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.UK,
                                                                                   Amount = .75m
                                                                               },
                                                                           new RegionShippingCost()
                                                                               {
                                                                                   DestinationRegion =
                                                                                       RegionShippingCost.Regions.Europe,
                                                                                   Amount = 1.5m
                                                                               }
                                                                       },
            };
            var flatRateShippingOption = new FlatRateShipping { FlatRate = 1.1m };

            var newOptionShipping = new NewOptionShipping
            {
                NewOptionCosts = new[] {
                                new NewOptionCost(){
                                    DestinationRegion = NewOptionCost.Regions.Europe,
                                    Amount = 1.25m
                                   
                                },
                                new NewOptionCost(){
                                    DestinationRegion = NewOptionCost.Regions.UK,
                                    Amount = .75m
                                }
                            },
            };

            var basket = new Basket()
            {
                LineItems = new List<LineItem>
                                                 {
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.UK,
                                                             Shipping = perRegionShippingOption,
                                                             TypeOfShipping = "PerRegion"
                                                         },
                                                     new LineItem()
                                                         {
                                                             DeliveryRegion = RegionShippingCost.Regions.Europe,
                                                             Shipping = perRegionShippingOption,
                                                             TypeOfShipping = "PerRegion"
                                                         },
                                                      new LineItem(){
                                                          DeliveryRegion = NewOptionCost.Regions.UK,
                                                          Shipping = newOptionShipping,
                                                          TypeOfShipping = "NewOption"
                                                      },
                                                      new LineItem(){
                                                          DeliveryRegion = NewOptionCost.Regions.UK,
                                                          Shipping = newOptionShipping,
                                                          TypeOfShipping = "NewOption"
                                                      },
                                                     new LineItem() {
                                                         Shipping = flatRateShippingOption,
                                                         TypeOfShipping = "FlatRate"
                                                     },
                                                 }
            };

            var calculator = new ShippingCalculator();

         decimal basketShipping = calculator.CalculateShipping(basket);
         Assert.That(basketShipping, Is.EqualTo(4.35m));
        }
    }
}