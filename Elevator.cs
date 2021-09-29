using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lift
{
    // todo minimalFloor && maxflooor сделать полями и добавить метод постоянного отслеживания 
    public class Elevator
    {
        public int LiftCapacity { get; private set; }
        public int CurrentFloor { get; private set; } = 1;
        public int CountFloor { get; private set; }
        public List<Person> ListOfPerson { get; private set; } = new();
        public List<int> ListOnButton { get; private set; } = new();

        private Random _random = new();
        private Directions _totalDirection = new();

        private delegate void ElevatorHandler();
        private event ElevatorHandler SortEvent;

        private void SetRanomDirections() => _totalDirection = (Directions)_random.Next(0, 2);

        public Elevator(int countFloor, int liftCapacity)
        {            
            LiftCapacity = liftCapacity;
            CountFloor = countFloor;
            SetRanomDirections();

            if(_totalDirection == Directions.Down)
            {
                CurrentFloor = CountFloor;
            }
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
                Console.WriteLine("Последний этаж достигнут");
            }          
        }

        public void GoDownstairs()
        {
            if(CurrentFloor > 1)
            {
                Console.WriteLine("Едем вниз {0}", CurrentFloor);
                CurrentFloor--;
            }
            else
            {
                Console.WriteLine("Первый этаж достигнут");
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
        // события sortevent & DirectEvent (goupst,godownst) & method CheckCoord(minflr,maxflr)
        public void WorkOfElevator()
        {
            Func<Elevator, bool> predicate = x => x.CurrentFloor == ListOfPerson.FirstOrDefault()?.RequiredFloor;

            if (_totalDirection == Directions.Up)
            {
                SortEvent = new ElevatorHandler(SortUp);                
            }
            else if(_totalDirection == Directions.Down)
            {
                SortEvent = new ElevatorHandler(SortDown);
                
            }

            while (ListOnButton.Count != 0
                || ListOfPerson.Count != 0)
            {
                // сортируем список, относительно необх. этажей
                SortEvent?.Invoke();
                
                // 1 или более людей выходят на нужном этаже                                       
                CheckForExit(predicate);

                TryGenerateOnButton();
                TryComeNewPersons();
                if (_totalDirection == Directions.Up) GoUpstairs();
                else GoDownstairs();
            }
            CheckForExit(predicate);                        
        }

        // Func event
        public void CheckForExit(Func<Elevator ,bool> predicate)
        {
            while (predicate(this))
            {
                ExitOfPerson();               
            }
        }

        public void CreatePerson(int requiredFloor = 0)
        {
            var isUp = _totalDirection == Directions.Up;

            var minimalFloor = isUp ? CurrentFloor + 1 : 1;
            var maximumFloor = isUp ? CountFloor : CurrentFloor;

            var person = new Person()
            {
                Weight = _random.Next(40, 120),
                RequiredFloor = requiredFloor == 0 ? _random.Next(minimalFloor, maximumFloor) : requiredFloor
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
        // добавить 2 параметра int
        public void TryGenerateOnButton()
        {
            var result = _random.Next(1, 15);
            
            if(result == 9)
            {
                var isUp = _totalDirection == Directions.Up;

                var minimalFloor = isUp ? CurrentFloor + 1 : 1;
                var maximumFloor = isUp ? CountFloor : CurrentFloor;

                if (minimalFloor == CountFloor) 
                {
                    return; 
                }

                ListOnButton.Add(
                    _random.Next(minimalFloor, maximumFloor)
                    );
            }
        }
        // Func e -
        public void ExitOfPerson()
        {
            ListOfPerson.Remove(ListOfPerson.First());
            Console.WriteLine("Человек вышел из лифта на {0} этаже, осталось {1}", CurrentFloor, ListOfPerson.Count);
        }

        private void SortUp()
        {
            // (x, y) => x.RequiredFloor.CompareTo(y.RequiredFloor)
            ListOfPerson.Sort((x, y) => x.RequiredFloor.CompareTo(y.RequiredFloor));
        }
        private void SortDown()
        {
            // (x, y) => x.RequiredFloor.CompareTo(y.RequiredFloor)
            ListOfPerson.Sort((x, y) => y.RequiredFloor.CompareTo(x.RequiredFloor));
        }
    }
}
