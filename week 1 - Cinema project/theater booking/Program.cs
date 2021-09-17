using System;
using System.Collections.Generic;

namespace Cine_booking
{
    class Program
    {
        static void Main(string[] args)
        {
            bool run = true;
            Dictionary<string, string[,]> screens = Screen();

            DateTime todaysDate = DateTime.Now.Date;


            while (run == true)
            {
                Console.Clear();
                Console.WriteLine("\n\nwelcome to the cinema!\n\n");
                System.Threading.Thread.Sleep(1000);
                screens = GetPeopleAges(screens);

                if (todaysDate < DateTime.Now.Date)
                { 
                    screens = NewDayResets(screens);
                    todaysDate = DateTime.Now.Date;
                } 
            }
        }

        static Dictionary<string, string[,]> NewDayResets(Dictionary<string, string[,]> screens)
        {
            Console.WriteLine("reseting cinema data");

            Dictionary<string, string[,]> seatData = new Dictionary<string, string[,]>();

            for (int daysInAdvance = 0; daysInAdvance < 8; daysInAdvance++)
            {
                for (int filmNumber = 1; filmNumber < 6; filmNumber++)
                {
                    if (daysInAdvance != 0)
                    {
                        string[,] seats = screens[$"{daysInAdvance},{filmNumber}"];
                        string dayAndScreenNum = $"{daysInAdvance - 1},{filmNumber}";
                        seatData.Add(dayAndScreenNum, seats);
                    }
                    else
                    {
                        string dayAndScreenNum = $"7,{filmNumber}";
                        seatData.Add(dayAndScreenNum, initlizeScreen());
                    }
                }
            }
            return seatData;
        }

        static int GetIntInput(string msg, int lowerBound, int upperBound)
        {
            int _int = 0;
            bool valid = false;

            while (valid == false)
            {
                try
                {
                    Console.WriteLine(msg);
                    _int = int.Parse(Console.ReadLine());

                    if (_int >= lowerBound && _int <= upperBound)
                        valid = true;
                    else
                        Console.WriteLine($"invlaid input must be more than {lowerBound} and less than {upperBound}");
                }
                catch
                {
                    Console.WriteLine("You input was not an int");
                }
            }

            return _int;
        }
        static int SmallestInt(int[] arr)
        {
            int smallest = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < smallest)
                {
                    smallest = arr[i];
                }
            }
            return smallest;
        }
        static string[,] DisplayFilms(Dictionary<string, string[,]> screens)
        {

            string[,] filmList = new string[5, 4] { { "1", "toy story", "u", "0" }, { "2", "superman", "12", "12" }, { "3", "planes", "u", "0" }, { "4", "1917", "18", "18" }, { "5", "set", "15", "15" } };

            Console.WriteLine("\n-------------------------------------------------------------");
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Console.Write($"{filmList[x, y],10}");
                }
                // pain to come
                DisplayTicketsSold(screens, x);

            }
            Console.WriteLine("\n-------------------------------------------------------------");

            return filmList;
        }
        static void DisplayTicketsSold(Dictionary<string, string[,]> screens, int filmNumber)
        {
            int ticketsSold = 0;

            // loop through only the certain cinema screen
            for (int daysInAdvance = 0; daysInAdvance < 8; daysInAdvance++)
            {
                string[,] seats = screens[$"{daysInAdvance},{filmNumber + 1}"];

                foreach (string seat in seats)
                {
                    if (seat == "X")
                    {
                        ticketsSold++;
                    }
                }
            }

            Console.WriteLine($"\tTotal tickets sold : {ticketsSold}");
        }
        static string MaxAgeRating(int minAge)
        {
            string maxAgeRating = "18";

            if (minAge < 12)
                maxAgeRating = "U";
            else if (minAge < 15)
                maxAgeRating = "12";
            else if (minAge < 18)
                maxAgeRating = "15";

            return maxAgeRating;
        }
        static Dictionary<string, string[,]> GetPeopleAges(Dictionary<string, string[,]> screens)
        {
            int numberOfPeople = GetIntInput("How many people are you booking for", 0, 45);
            int[] ages = new int[numberOfPeople];

            for (int x = 0; x < numberOfPeople; x++)
            {
                int ageOfPerson = GetIntInput($"\nWhat is the age of the number {x + 1} person in your booking", 0, 150);
                System.Threading.Thread.Sleep(100);
                ages[x] = ageOfPerson;
            }

            Dictionary<string, string[,]> updatedScreen = ticketBooking(numberOfPeople, ages, screens);

            return updatedScreen;
        }
        static (string, int) FilmSelection(string[,] filmLists, int minAge, Dictionary<string, string[,]> screens)
        {
            string film = "";
            bool valid = false;
            int filmNumber = 0;

            while (valid == false)
            {
                Console.WriteLine("\n");
                filmNumber = GetIntInput($"\nChoose the film number in which you would like to see", 0, 5);
                film = filmLists[filmNumber - 1, 1];
                Console.WriteLine($"You have choosen to see film number {filmNumber} this is film {film} is this correct yes or no");
                string doubleCheck = Console.ReadLine().ToLower();

                if (doubleCheck != "no")
                {
                    if (minAge >= int.Parse(filmLists[filmNumber - 1, 3]))
                    {
                        Console.WriteLine("Eveyone in your group is old enough for the film\n");
                        valid = true;
                    }
                    else
                        Console.WriteLine("One or more members of your group are too young to see the selected film please select a differant film");
                }
                else
                {
                    Console.WriteLine("You can choose a new film ");
                    _ = DisplayFilms(screens);
                }
            }
            return (film, filmNumber);
        }
        static Dictionary<string, string[,]> ticketBooking(int numOfPeople, int[] ages, Dictionary<string, string[,]> screens)
        {
            int minAge = SmallestInt(ages);
            string maxFilmRating = MaxAgeRating(minAge);

            System.Threading.Thread.Sleep(500);

            Console.WriteLine($"\n\nYou are booking {numOfPeople} tickets. You can see films up to an age rating of {maxFilmRating}\n");

            string[,] filmList = DisplayFilms(screens);

            (string film, int filmNumber) = FilmSelection(filmList, minAge, screens);

            Console.WriteLine($"You will have {numOfPeople} tickets to see {film}");

            (string selectedDate, int daysInAdvance) = DateSelection(screens, numOfPeople, filmNumber);
            Dictionary<string, string[,]> updatedScreens = SeatBooking(numOfPeople, film, filmNumber, daysInAdvance, screens);

            double cost = CalculateCost(numOfPeople, ages);
            System.Threading.Thread.Sleep(500);

            Finsh(selectedDate, cost, numOfPeople, film);

            return updatedScreens;
        }

        static void Finsh(string selectedDate, double cost, int numberOfPeople, string film)
        {
            Console.WriteLine("\nYou will now be asked to pay");
            Payment(cost, numberOfPeople);

            System.Threading.Thread.Sleep(500);


            Console.WriteLine("\nYour ticket will be printed bellow");

            System.Threading.Thread.Sleep(300);


            Console.WriteLine($"\n\n------\nCINEMA DELEXUE\nFILM : {film}\nNumber of people : {numberOfPeople}\nCOST : £{cost}\nDATE : {selectedDate}\n------");

            Console.WriteLine("\n\nPress any key to finsh");
            Console.ReadKey();
        }
        static void Payment(double cost, int numberOfPeople)
        {
            Console.WriteLine($"\nIt will cost £{cost} for {numberOfPeople} number of people");
            double payment = 0.00;

            while (payment < cost)
            {
                try
                {
                    Console.WriteLine($"\nEnter the amount you wish to pay change will be given if needed current payment £{payment}");
                    payment += double.Parse(Console.ReadLine());

                    if (payment < cost)
                        Console.WriteLine("To little you must give more money");
                }
                catch
                {
                    Console.WriteLine("Must be a number");
                }
            }

            Console.WriteLine($"You have payed your change is £{payment - cost}");
        }
        static (string, int) DateSelection(Dictionary<string, string[,]> screens, int numberOfPeople, int filmNumber)
        {
            DateTime todaysDate = DateTime.Now;
            Console.WriteLine($"\ntodays date is {todaysDate.ToString("dd/MM/yyyy")} please select a date now more than 1 week later");
            string seletedDate = "";
            int daysInAdvance = 0;

            bool check = false;
            while (check == false)
            {
                daysInAdvance = GetIntInput("How many days in advance do you wish to book", 0, 7);
                seletedDate = todaysDate.AddDays(daysInAdvance).ToString("dd/MM/yyyy");

                Console.WriteLine($"\nYou have selected {seletedDate} this is {daysInAdvance} days in the future");
                Console.WriteLine("Are you happy with this choice yes or no");
                string correctChoice = Console.ReadLine().ToLower();

                bool enoughSpace = checkIfEnoughSpace(screens, numberOfPeople, daysInAdvance, filmNumber);

                if (correctChoice != "no" && enoughSpace == true)
                    check = true;
                else
                    Console.WriteLine("Please select again");

            }

            return (seletedDate, daysInAdvance);
        }

        static bool checkIfEnoughSpace(Dictionary<string, string[,]> screens, int numberOfPeople, int daysInAdvance, int filmNumber)
        {
            bool enoughSpace = true;
            int emptySeats = 0;

            string[,] seats = screens[$"{daysInAdvance},{filmNumber}"];

            for (int i = 0; i < 9; i++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (seats[x, i] != "X")
                        emptySeats += 1;
                }
            }

            if (numberOfPeople > emptySeats)
                enoughSpace = false;

            return enoughSpace;
        }

        static double CalculateCost(int numberOfPeople, int[] ages)
        {
            int minors = 0;
            double price = 7.00;
            int adults = 0;

            foreach (int age in ages)
            {
                if (age >= 18)
                    adults += 1;
                else
                    minors += 1;
            }

            double cost = Math.Round((double)((adults * price) + (double)(minors * price / 2)), 2);

            Console.WriteLine($"\nThe cost of the tickets is £{cost} this is for {adults} adults {minors} minors");

            Console.WriteLine("Payment will be taken at the end\n");

            return cost;
        }

        static string[,] initlizeScreen()
        {
            string[,] seats = new string[5, 9];

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    seats[y, x] = "0";
                }
            }

            return seats;
        }
        static Dictionary<string, string[,]> Screen()
        {
            Dictionary<string,string[,]> screens = new Dictionary<string, string[,]>();
            for (int day = 0; day < 8; day++)
            {
                for (int screenNumber = 1; screenNumber < 6; screenNumber++)
                {
                    string dayAndScreenNum = $"{day},{screenNumber}";

                    screens.Add(dayAndScreenNum, initlizeScreen());
                }
            }
            return screens;
        }
        static void DisplaySeats(string[,] seats)
        {
            Console.Write("  ");
            for (int i = 1; i < 10; i++)
            {
                Console.Write($"{i}");
            }
            Console.WriteLine();
            for (int x = 0; x < 5; x++)
            {
                Console.Write($"{x + 1} ");
                for (int y = 0; y < 9; y++)
                {
                    Console.Write(seats[x, y]);
                }
                Console.WriteLine();
            }
        }
        static Dictionary<string, string[,]> SeatBooking(int numberOfPeople, string film, int filmNumber, int daysInAdvance, Dictionary<string, string[,]> screens)
        {
            System.Threading.Thread.Sleep(500);

            int xCorrdinate = 0;
            int yCorrdinate = 0;
            Console.WriteLine("\nYou will now book your seats");

            string[,] seats = screens[$"{daysInAdvance},{filmNumber}"];

            for (int bookedseats = 0; bookedseats < numberOfPeople; bookedseats++)
            {
                bool valid = false;
                while(valid == false)
                {
                    DisplaySeats(seats);
                    xCorrdinate = GetIntInput($"\nplease select the x value of your wanted seat for person {bookedseats + 1}", 1, 9);
                    yCorrdinate = GetIntInput($"\nplease select the y value of your wanted seat for person {bookedseats + 1}", 1, 5);

                    Console.WriteLine($"for person {bookedseats + 1} you have selected seat {xCorrdinate}{yCorrdinate}");

                    if (seats[yCorrdinate - 1, xCorrdinate - 1] != "X")
                        valid = true;
                    else
                        Console.WriteLine("the seat is already taken please choose one labeled with 0\n");
                    System.Threading.Thread.Sleep(500);
                }
                Console.WriteLine($"For person {bookedseats + 1} you have book seat {xCorrdinate}{yCorrdinate}");
                seats[yCorrdinate - 1, xCorrdinate -1] = "X";
            }

            screens[$"{daysInAdvance},{filmNumber}"] = seats;

            DisplaySeats(screens[$"{daysInAdvance},{filmNumber}"]);

            Console.WriteLine("You have now book your seats\n");
            System.Threading.Thread.Sleep(500);

            return screens;
        }
    }
}
