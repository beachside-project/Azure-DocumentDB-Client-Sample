using System;
using System.Diagnostics;
using System.Dynamic;
using DocumentDbDemo.DocumentDbRepository;
using DocumentDbDemo.Models;
using Newtonsoft.Json;

namespace DocumentDbDemo
{
class Program
{
    static void Main(string[] args)
    {
        var person1 = new Person()
        {
            PersonId = "P1",
            FirstName = "Char",
            LastName = "ming",
            Friends = new[] { 2, 3 }
        };

        var person2 = new Person()
        {
            PersonId = "P2",
            FirstName = "Dan",
            LastName = "Dy",
            Friends = new[] { 2, 3 }
        };

        //insert samples
        //1st connection is very slow.....because instances(database and collection) created
        var timer = Stopwatch.StartNew();
        DemoCollecitonManager.InsertPersonDocument(person1).Wait();
        Console.WriteLine($"1st connection(insert) : {timer.ElapsedMilliseconds}");

        timer.Restart();
        DemoCollecitonManager.InsertPersonDocument(person2).Wait();
        Console.WriteLine($"2nd connection(insert) : {timer.ElapsedMilliseconds}");

        //select sample
        timer.Restart();
        var dandy = DemoCollecitonManager.GetPersonModelsById(person2.PersonId);
        Console.WriteLine($"3rd connection(select) : {timer.ElapsedMilliseconds}");
        Console.WriteLine(JsonConvert.SerializeObject(dandy, Formatting.Indented));

        //delete sample 
        var idArray = new[] {person1.PersonId, person2.PersonId};
        DemoCollecitonManager.DeleteByPersonIdArrayAsync(idArray).Wait();


        Console.ReadKey();

        }
    }
}
