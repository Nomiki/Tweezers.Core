using ApiExample.DataHolders;
using Tweezers.Api.Controllers;

namespace ApiExample.Controllers
{
    public class ProductController : TweezersController<Product>
    {
        public ProductController()
        {
            Post(new Product() {Id = "1234", Name = "Abibas"});
            Post(new Product() {Id = "2345", Name = "Buma"});
            Post(new Product() {Id = "3456", Name = "Berez"});
        }
    }
}