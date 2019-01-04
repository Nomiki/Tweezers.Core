using ApiExample.DataHolders;
using Tweezers.Api.Controllers;

namespace ApiExample.Controllers
{
    public class PersonController : TweezersController<Person>
    {
        public PersonController()
        {
            this.Post(new Person() {Name = "Moshe", CatName = "Shmil", Gender = Gender.Male});
            this.Post(new Person() {Name = "Yakov", CatName = "Simon", Gender = Gender.Male});
            this.Post(new Person() {Name = "Hatul", CatName = "Adam", Gender = Gender.Unknown});
        }
    }
}
