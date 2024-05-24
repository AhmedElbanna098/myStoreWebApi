namespace MyStoreWebAPI.Services
{
    public class OrderHelper
    {
        public static decimal ShippingFee { get; } = 5;

        public static Dictionary<string, string> PaymentMethods { get; } = new()
        {
            {"Cash","Cash on Delivery" },
            {"Paypal","Paypal" },
            {"Credit Card","Credit Card" }
        };

        public static List<string> PaymentStatus { get; } = new()
        {
            "Pending", "Accepted", "Canceled"
        };
        public static List<string> OrderStatus { get; } = new()
        {
            "Created", "Accepted", "Canceled", "Shipped", "Delivered", "Returned"
        };


        /*
         recieve a string of product identifiers separated by '-'

        returns a list of pairs in dictionary
        key -> productid
        value -> product quantity

         */
        public static Dictionary<int,int> GetProductDictionary(string productIdentifiers)
        {
            var productDictionary = new Dictionary<int,int>();
            if (productIdentifiers.Length > 0)
            {
                string[] productIdentifiersArray = productIdentifiers.Split('-');
                foreach (var product in productIdentifiersArray)
                {
                    try
                    {
                        int id = int.Parse(product);
                        if (productDictionary.ContainsKey(id))
                        {
                            productDictionary[id] += 1;
                        }else
                        {
                            productDictionary.Add(id, 1);
                        }
                    }
                    catch (Exception){}
                }
            }
            return productDictionary;
        }
    }
}
