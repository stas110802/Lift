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
        private Random _random = new();
        private Directions _totalDirection = new();
        private delegate void ElevatorHandler();
        private List<ElevatorHandler> _listOfEvent = new();
        private int _minimalFloor => IsGoUpstair ? CurrentFloor + 1 : 1;
        private int _maximumFloor => IsGoUpstair ? CountFloor : CurrentFloor;        
        private bool IsGoUpstair => _totalDirection == Directions.Up;
        private bool IsGoDownstair => _totalDirection == Directions.Down;

        public Elevator(int countFloor = 50, int liftCapacity = 450)
        {            
            LiftCapacity = liftCapacity;
            CountFloor = countFloor;
            SetRanomDirections();

            if(IsGoDownstair)
            {
                CurrentFloor = CountFloor;
            }                       
        }

        public void GoUpstairs()
        {            
            if(CurrentFloor < CountFloor)
            {
                Console.WriteLine("Едем вверх {0} => {1}", CurrentFloor, ++CurrentFloor);
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
                Console.WriteLine("Едем вниз {0} => {1}", CurrentFloor, --CurrentFloor);
            }
            else
            {
                Console.WriteLine("Первый этаж достигнут");
            }
        }        
        // tryaddperson
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
            SetRanomDirections();
            // first init
            CreatePerson();           
            // инициализация списка ивентов, относительно того,
            // куда движется лифт
            if (IsGoUpstair)
            {
                AddToEventList(SortUp, GoUpstairs);
            }
            else if(IsGoDownstair)
            {
                AddToEventList(SortDown, GoDownstairs);
            }
            
            while (ListOnButton.Count != 0
                || ListOfPerson.Count != 0)
            {
                // едем вверх и сортируем список по возрастанию
                // или едем вниз и сортируем список по убыванию
                foreach (var item in _listOfEvent)
                {
                    item?.Invoke();
                }
                // 1 или более людей выходят на нужном этаже                                       
                CheckForExit();
                // генерация нажатых кнопок
                TryGenerateOnButton();
                // проверка нужен ли лифт на тек. этаже
                TryComeNewPersons();
            }
            CheckForExit();
            _listOfEvent.Clear();
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
            if (_minimalFloor > _maximumFloor) return;
            var person = new Person()
            {
                Weight = _random.Next(40, 120),
                RequiredFloor = requiredFloor == 0 ? _random.Next(_minimalFloor, _maximumFloor) : requiredFloor
            };                     
            AddPerson(person);
        }

        public void TryComeNewPersons()
        {
            if (ListOnButton.Count == 0) return;
            var isCreate = false;//todo убрать IsCreate

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

        public void TryGenerateOnButton()
        {
            var result = _random.Next(1, 15);
            
            if(result == 9)
            {
                if (_minimalFloor == CountFloor ||
                    _minimalFloor == _maximumFloor) 
                {
                    return; 
                }

                ListOnButton.Add(
                     _random.Next(_minimalFloor, _maximumFloor));
            }
        }

        public void ExitOfPerson()
        {
            ListOfPerson.Remove(ListOfPerson.First());
            Console.WriteLine("Человек вышел из лифта на {0} этаже, осталось {1}", CurrentFloor, ListOfPerson.Count);
        }       

        private void SortUp()
        {
            ListOfPerson.Sort((x, y) => x.RequiredFloor.CompareTo(y.RequiredFloor));
        }

        private void SortDown()
        {
            ListOfPerson.Sort((x, y) => y.RequiredFloor.CompareTo(x.RequiredFloor));
        }

        private void AddToEventList(Action first, Action second)
        {
            _listOfEvent.Add(new ElevatorHandler(first));
            _listOfEvent.Add(new ElevatorHandler(second));
        }

        private void SetRanomDirections() 
        {
            // если на 1 этаже, то едем наверх
            if (CurrentFloor == 1)
            {
                _totalDirection = Directions.Up;
            }
            // если на последнем этаже, то едем вниз
            else if (CurrentFloor == CountFloor)
            {
                _totalDirection = Directions.Down;
            }
            // остальное рандом
            else 
            { 
                _totalDirection = (Directions)_random.Next(0, 2);
            }                                  
        } 
    }
}
