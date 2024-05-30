using Commons.Model.Product;

namespace Commons.Model
{
    public class Subscription
    {
        public int Id { get; set; }
        public ProductType Topic { get; set; }
        public string CallbackURL { get; set; }

    }

}
