using Marketplace.Interview.Business.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Interview.Business.Shipping
{
    public class NewOptionShipping : ShippingBase
    {
        public IEnumerable<NewOptionCost> NewOptionCosts { get; set; }

        public override string GetDescription(LineItem lineItem, Basket.Basket basket)
        {
            return string.Format("New option shipping to {0}", lineItem.DeliveryRegion);
        }

        public override decimal GetAmount(LineItem lineItem, Basket.Basket basket)
        {
            var result =  (from c in NewOptionCosts
                 where c.DestinationRegion == lineItem.DeliveryRegion
                 select c.Amount).Single();
            return result;
        }
    }
}
