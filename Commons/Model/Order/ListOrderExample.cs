using Swashbuckle.AspNetCore.Filters;

namespace Commons.Model.Order
{
    public class ListOrderExample : IExamplesProvider<List<Order>>
    {
        public List<Order> GetExamples() {

            return new List<Order>(){
                new Order { 
                    Id = 10, 
                    ProductId = 1001, 
                    ProductName = "OLED 57 inch TV\"", 
                    ProductType = Product.ProductType.TV, 
                    Quantity = 1 
                },
                new Order { 
                    Id = 25, 
                    ProductId = 2001, 
                    ProductName = "Smartphone M1302", 
                    ProductType = Product.ProductType.Smartphone, 
                    Quantity = 2 
                },
                // this is a bad order, without a product location in DB
                new Order {
                    Id = 26,
                    ProductId = 90, //this product is not in DB
                    ProductName = "Canned food",
                    ProductType = Product.ProductType.TV,
                    Quantity = 90
                },
                new Order {
                    Id = 101,
                    ProductId = 2002,
                    ProductName = "Tablet model 8",
                    ProductType = Product.ProductType.Tablet,
                    Quantity = 90
                }
            };
        }

    }
}
