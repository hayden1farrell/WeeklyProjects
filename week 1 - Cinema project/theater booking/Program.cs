using System;
using System.Collections.Generic;

namespace Cine_booking
{
    class Program
    { 
        static void Main(string[] args)
        {
            bool run = true;
            while (run == true)
            {
                Console.Clear();
                Console.WriteLine("\n\nwelcome to the cinema!\n\n");
                System.Threading.Thread.Sleep(1000);
                GetPeopleAges();

            }
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
                        Console.WriteLine($"invlaid input must be more than {lowerBound - 1} and less than {upperBound}");
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
        static string[,] DisplayFilms()
        {

            string[,] filmList = new string[5, 4] { { "1", "toy story", "u", "0" }, { "2", "superman", "12", "12" }, { "3", "planes", "u", "0" }, { "4", "1917", "18", "18" }, { "5", "set", "15", "15" } };

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Console.Write($"{filmList[x, y],10}");
                }
                Console.WriteLine("\n---------------------------------------");
            }

            return filmList;
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
        static void GetPeopleAges()
        {
            int numberOfPeople = GetIntInput("How many people are you booking for", 0, 64);
            int[] ages = new int[numberOfPeople];

            for (int x = 0; x < numberOfPeople; x++)
            {
                int ageOfPerson = GetIntInput($"\nWhat is the age of the number {x + 1} person in your booking", 0, 150);
                System.Threading.Thread.Sleep(100);
                ages[x] = ageOfPerson;
            }

            ticketBooking(numberOfPeople, ages);
        }
        static (string, int) FilmSelection(string[,] filmLists, int minAge)
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
                    _ = DisplayFilms();
                }
            }
            return (film, filmNumber);
        }
        static void ticketBooking(int numOfPeople, int[] ages)
        {
            int minAge = SmallestInt(ages);
            string maxFilmRating = MaxAgeRating(minAge);

            System.Threading.Thread.Sleep(500);

            Console.WriteLine($"\n\nYou are booking {numOfPeople} tickets. You can see films up to an age rating of {maxFilmRating}\n");

            string[,] filmList = DisplayFilms();

            (string film, int filmNumber) = FilmSelection(filmList, minAge);

            Console.WriteLine($"You will have {numOfPeople} tickets to see {film}");


            SeatBooking(numOfPeople, film, filmNumber);

            double cost = CalculateCost(numOfPeople, ages);
            System.Threading.Thread.Sleep(500);


            string selectedDate = DateSelection();
            Finsh(selectedDate, cost, numOfPeople, film);
        }

        static void Finsh(string selectedDate, double cost, int numberOfPeople, string film)
        {
            Console.WriteLine("\nYou will now be asked to pay");
            Payment(cost, numberOfPeople);

            System.Threading.Thread.Sleep(500);


            Console.WriteLine("\nYour ticket will be printed bellow");

            System.Threading.Thread.Sleep(300);


            Console.WriteLine($"\n\n------\nCINEMA DELEXUE\nFILM : {film}\nNumber of people : {numberOfPeople}\nCOST : {cost}\nDATE : {selectedDate}\n------");

            Console.WriteLine("\n\nPress any key to finsh");
            Console.ReadKey();
        }
        static void Payment(double cost, int numberOfPeople)
        {
            Console.WriteLine($"\nIt will cost {cost} for {numberOfPeople} number of people");
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
        static string DateSelection()
        {
            DateTime todaysDate = DateTime.Now;
            Console.WriteLine($"\ntodays date is {todaysDate.ToString("dd/MM/yyyy")} please select a date now more than 1 week later");
            string seletedDate = "";

            bool check = false;
            while (check == false)
            {
                int daysInAdvance = GetIntInput("How many days in advance do you wish to book", 0, 7);
                seletedDate = todaysDate.AddDays(daysInAdvance).ToString("dd/MM/yyyy");

                Console.WriteLine($"\nYou have selected {seletedDate} this is {daysInAdvance} days in the future");
                Console.WriteLine("Are you happy with this choice yes or no");
                string correctChoice = Console.ReadLine().ToLower();

                if (correctChoice != "no")
                    check = true;
                else
                    Console.WriteLine("Please select again");

            }

            return seletedDate;
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
            string[,] seats = new string[8, 8];

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    seats[y, x] = "0";
                }
            }

            return seats;
        }
        static Dictionary<string, string[,]> Screen()
        {
            Dictionary<string, string[,]> screens = new Dictionary<string, string[,]>();
            for (int i = 0; i < 5; i++)
            {
                screens.Add(i.ToString(), initlizeScreen());
            }
            return screens;
        }
        static void DisplaySeats(string[,] seats)
        {
            Console.Write("    ");
            for (int i = 1; i < 9; i++)
            {
                Console.Write($"{i}");
            }
            Console.WriteLine("\n");
            for (int x = 0; x < 8; x++)
            {
                Console.Write($"{x + 1} - ");
                for (int y = 0; y < 8; y++)
                {
                    Console.Write(seats[x, y]);
                }
                Console.WriteLine();
            }
        }
        static void SeatBooking(int numberOfPeople, string film, int filmNumber)
        {
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("\nYou will now book your seats");

            Dictionary<string, string[,]> screens = Screen();

            string[,] seats = screens[(filmNumber-1).ToString()];

            DisplaySeats(seats);

            Console.WriteLine("You have now book your seats\n");
        }
    }
}
