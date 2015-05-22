using Marketplace.Interview.Business.Core;
using System.Linq;

namespace Marketplace.Interview.Business.Basket
{
    public interface IShippingCalculator
    {
        decimal CalculateShipping(Basket basket);
    }

    public class ShippingCalculator : IShippingCalculator
    {
        public decimal CalculateShipping(Basket basket)
        {
            bool check = false;
            foreach (var lineItem in basket.LineItems)
            {
                lineItem.ShippingAmount = lineItem.Shipping.GetAmount(lineItem, basket);
                lineItem.ShippingDescription = lineItem.Shipping.GetDescription(lineItem, basket);
            }
            //check basket alway have more than 1 shipping
            if (basket.LineItems.Count > 1)
            {
                //compare all element from basket
                for (var i = 0; i < basket.LineItems.Count(); i++ )
                {
                    for (var j = 1; j <= basket.LineItems.Count() - 1; j++)
                    {
                        //select from basket where the same Shipping Option and the same Supplier and Region
                        if (
                            basket.LineItems[i].TypeOfShipping.Equals(Constants.NewOption) && 
                            basket.LineItems[i].SupplierId.Equals(basket.LineItems[j].SupplierId)
                            && basket.LineItems[i].DeliveryRegion.Equals(basket.LineItems[j].DeliveryRegion)
                            && basket.LineItems[i].TypeOfShipping.Equals(basket.LineItems[j].TypeOfShipping)
                            && i != j)
                        {
                            check = true;
                        }
                    }
                }
            }
            if(check){
                return basket.LineItems.Sum(li => li.ShippingAmount) - Constants.DeductedNumber;
            }
            return basket.LineItems.Sum(li => li.ShippingAmount);
        }
    }
}