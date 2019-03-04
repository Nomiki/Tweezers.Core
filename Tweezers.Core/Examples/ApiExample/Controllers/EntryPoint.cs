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
            pc.Post(new Person() { Name = "Moshe", Gender = Gender.Male });
            pc.Post(new Person() { Name = "Yakov", Gender = Gender.Male });
            pc.Post(new Person() { Name = "Miki", Gender = Gender.Unknown });

            LaptopController lc = new LaptopController();
            lc.Post(new Laptop() {BrandName = "Asus", Name = "ZenBook z6", Price = 6000.00, CpuModel = "i7 8750u", Memory = "16GB", Disk = "512GB SSD"});
            lc.Post(new Laptop() {BrandName = "Lenovo", Name = "Yoga 3", Price = 5560.00, CpuModel = "i7 7600u", Memory = "8GB", Disk = "256GB SSD"});
            lc.Post(new Laptop() {BrandName = "Dell", Name = "Latitude 5480", Price = 7600.00, CpuModel = "i7 7820HQ", Memory = "16GB", Disk = "512GB SSD"});

            SmartPhoneController sc = new SmartPhoneController();
            sc.Post(new SmartPhone() { BrandName = "Samsung", Name = "Galaxy s10", Price = 4000.00, CpuModel = "Snapdragon 840", Memory = "6GB", Disk = "64GB" });
            sc.Post(new SmartPhone() { BrandName = "Google", Name = "Pixel 3", Price = 3400.00, CpuModel = "Snapdragon 835", Memory = "4GB", Disk = "128GB" });
            sc.Post(new SmartPhone() { BrandName = "Apple", Name = "iPhone XS", Price = 5600.00, CpuModel = "ARM A12", Memory = "4GB", Disk = "512GB" });

            PeripheralController prc = new PeripheralController();
            prc.Post(new Peripheral() {BrandName = "Corsair", Name = "K70", PeripheralType = PeripheralType.Keyboard, Price = 360});
            prc.Post(new Peripheral() {BrandName = "Razer", Name = "Kraken 7.1", PeripheralType = PeripheralType.Headphones, Price = 360});
            prc.Post(new Peripheral() {BrandName = "Microsoft", Name = "XBox Controller", PeripheralType = PeripheralType.Keyboard, Price = 400});
            prc.Post(new Peripheral() {BrandName = "Logitech", Name = "Marathon Mouse", PeripheralType = PeripheralType.Mouse, Price = 160});
        }
    }
}
