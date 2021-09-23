using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lift
{
    public class Elevator
    {
        public int LiftCapacity { get; private set; }
        public int CurrentFloor { get; private set; } = 1;
        public int CountFloor { get; private set; }
        public List<Person> ListOfPerson { get; private set; } = new();
        public List<int> ListOnButton { get; private set; } = new();

        public Elevator(int countFloor, int liftCapacity)
        {            
            LiftCapacity = liftCapacity;
            CountFloor = countFloor;

            // first init
            if (ListOfPerson.Count == 0)
            {
                CreatePerson();
            }
        }

        public void GoUpstairs()
        {            
            if(CurrentFloor < CountFloor)
            {
                Console.WriteLine("Едем вверх {0}", CurrentFloor);
                CurrentFloor++;
            }
            else
            {
                Console.WriteLine("Это последний этаж");
            }         
        }

        public void GoDownstairs()
        {
            if(CurrentFloor > 1)
            {
                Console.WriteLine("Едем вниз");
                CurrentFloor--;
            }
            else
            {
                Console.WriteLine("Это первый этаж, ниже некуда");
            }
        }        

        public void AddPerson(Person person)
        {
            var totalWeight = ListOfPerson.Sum(x => x.Weight) + person.Weight;

            if(totalWeight < LiftCapacity)
            {
                ListOfPerson.Add(person);
                Console.WriteLine("+ 1 человек в лифте, теперь их {0}", ListOfPerson.Count);
                Console.WriteLine("ему нужен {0} этаж", person.RequiredFloor);
            }
            else
            {
                Console.WriteLine("Превышена грузоподьемность лифта.");
                Console.WriteLine("Максимальная грузоподьемность - {0}", LiftCapacity);
                Console.WriteLine("Суммарный вес людей - {0}", totalWeight);
                Console.WriteLine("Человек не зашел в лифт.");
            }
        }
        
        public void WorkOfElevator()
        {                       
            // сначала лифт развозит всех в одном направлении и по пути  может подбирать людей,
            // которым в тоже направление
            // upd: на менюшки есть 2 кнопки [ вниз и вверх ]
            
            while (CurrentFloor != CountFloor)
            {
                // сортируем список, относительно необх. этажей
                ListOfPerson.Sort(
                    (x,y) => x.RequiredFloor.CompareTo(y.RequiredFloor));
                // 1 или более людей выходят на нужном этаже                                       
                CheckForExit();
                GenerateOnButton();
                TryComeNewPersons();
                GoUpstairs();                    
            }
            CheckForExit();                        
        }

        public void CheckForExit()
        {
            while (CurrentFloor == ListOfPerson.FirstOrDefault()?.RequiredFloor)
            {
                ExitOfPerson();               
            }
        }

        public void CreatePerson(int requiredFloor = 0)
        {
            var person = new Person()
            {
                Weight = new Random().Next(40, 120),
                RequiredFloor = requiredFloor == 0 ? new Random().Next(CurrentFloor+1, CountFloor) : requiredFloor  
                //todo  люди едут только выше (чтобы программа не была бесконечной)
            };
            AddPerson(person);
        }

        public void TryComeNewPersons()
        {
            if (ListOnButton.Count == 0) return;
            var isCreate = false;

            foreach (var item in ListOnButton)
            {
                if(item == CurrentFloor)
                {
                    CreatePerson();
                    isCreate = true;
                }
            }

            if (isCreate)
            {
                ListOnButton.RemoveAll(
                    x => x == CurrentFloor);
            }            
        }

        public void GenerateOnButton()
        {
            var result = new Random().Next(1, 10);

            if(result % 4 == 0)
            {
                ListOnButton.Add(
                    new Random().Next(CurrentFloor+1, CountFloor)
                    );
            }
        }

        public void ExitOfPerson()
        {
            ListOfPerson.Remove(ListOfPerson.First());
            Console.WriteLine("Человек вышел из лифта на {0} этаже, осталось {1}", CurrentFloor, ListOfPerson.Count);
        }
    }
}
