using ApiExample.DataHolders;
using Tweezers.Api.Controllers;

namespace ApiExample.Controllers
{
    public class EntryPoint : TweezersEntryPoint
    {
        static EntryPoint()
        {
            Title = "Api Example App";
            Description = "Hello! Welcome to the tweezers example app";

            PersonController pc = new PersonController();
            pc.Post(new Person() { Name = "Moshe", CatName = "Shmil", Gender = Gender.Male });
            pc.Post(new Person() { Name = "Yakov", CatName = "Simon", Gender = Gender.Male });
            pc.Post(new Person() { Name = "Hatul", CatName = "Adam", Gender = Gender.Unknown });

            ShoeBrandController prc = new ShoeBrandController();
            prc.Post(new ShoeBrand() { Id = "1234", Name = "Abibas" });
            prc.Post(new ShoeBrand() { Id = "2345", Name = "Buma" });
            prc.Post(new ShoeBrand() { Id = "3456", Name = "Berez" });
        }
    }
}
