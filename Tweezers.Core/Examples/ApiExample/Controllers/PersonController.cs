using ApiExample.DataHolders;
using DiscoveryApi.Controllers;

namespace ApiExample.Controllers
{
    public class PersonController : TweezersController<Person>
    {
        public PersonController()
        {
            this.Post(new Person() {Name = "Moshe", CatName = "Shmil"});
            this.Post(new Person() {Name = "Yakov", CatName = "Simon"});
            this.Post(new Person() {Name = "Hatul", CatName = "Adam"});
        }
    }
}
