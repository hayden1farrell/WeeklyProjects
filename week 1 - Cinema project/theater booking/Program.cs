using System;
using System.Collections.Generic;

namespace Cine_booking
{
    class Program
    {
        static void Main()
        {
            bool run = true;                                                    // loop variable , Should never be false
            Dictionary<string, string[,]> screens = Screen();                   // Creates a dictionary with a key of a string and a value of a string array

            DateTime todaysDate = DateTime.Now.Date;                            // Gets todays date


            while (run == true)
            {
                Console.Clear();                                                // Clears the console when a new user gets to the screen 
                Console.WriteLine("\n\nwelcome to the cinema!\n\n");            // Intro message

                //Begins calling the functions
                (int numberOfPeople, int[] ages) = GetPeopleAges();             //Call the function to get the users ages

                int minAge = SmallestInt(ages);                                 // gets the lowest age out of the group
                string maxFilmRating = MaxAgeRating(minAge);                    // gets the max film rating the group can wacth eg. 12

                Console.WriteLine($"\n\nYou are booking {numberOfPeople} tickets. You can see films up to an age rating of {maxFilmRating}\n");

                string[,] filmList = DisplayFilms(screens);                     // Displays the list of films

                (string film, int filmNumber) = FilmSelection(filmList, minAge);// gets the users film selection and validates the choice

                Console.WriteLine($"You will have {numberOfPeople} tickets to see {film}");

                (string selectedDate, int daysInAdvance) = DateSelection(screens, numberOfPeople, filmNumber);  // gets the date the users wants to see the film on and how many days in advance
                Dictionary<string, string[,]> updatedScreens = SeatBooking(numberOfPeople, film, filmNumber, daysInAdvance, screens);   //gets the user to choose their seats

                screens = updatedScreens;                                       // updates the screen dictionry to include the seats choosen by the user

                double cost = CalculateCost(numberOfPeople, ages);              // gets the cost of the event for the user

                Finsh(selectedDate, cost, numberOfPeople, film);                // ends the booking and prints the ticket

                if (todaysDate < DateTime.Now.Date)                             // sees if a new day has happend
                { 
                    screens = NewDayResets(screens);                            // calls the reset function to remove yesterdays screens and seat data and create new data for 7 day in advance bookings
                    todaysDate = DateTime.Now.Date;                             // makes the stored day equal to the actual day
                } 
            }
        }

        static Dictionary<string, string[,]> NewDayResets(Dictionary<string, string[,]> screens)
        {
            /*
             * Runs once per day at midnight when a new day begins
             * removes yesterdays cinema data eg seats data
             * adds new seats and dictionary values for 7 days in advance at the time of the function being called
             */

            Console.WriteLine("reseting cinema data");

            Dictionary<string, string[,]> seatData = new Dictionary<string, string[,]>();   // Creates a new dictionary to store all the seat data and film data

            for (int daysInAdvance = 0; daysInAdvance < 8; daysInAdvance++)                 //loops for the 8 days need eg today to 1 week in advance
            {
                for (int filmNumber = 1; filmNumber < 6; filmNumber++)                      // loops through the films begin at 1 because the films are displayed as 1 - 5
                {
                    if (daysInAdvance != 0)                                                 // makes sure the day is not yesterday to prevent old data being reused
                    {
                        string[,] seats = screens[$"{daysInAdvance},{filmNumber}"];         // sets the 2d string array to be the the same seat data from the day before. copys the data
                        string dayAndScreenNum = $"{daysInAdvance - 1},{filmNumber}";       // gets the key of the array which is the days - 1 to account for the new a day and the film number
                        seatData.Add(dayAndScreenNum, seats);                               // adds all the values to the dictionary with the key as the daysinadvance - 1 and the value as the 2d array of seats
                    }
                    else
                    {                                                                       // if the days in advance is 0 eg it it yesterday at the time of function call then
                        string dayAndScreenNum = $"7,{filmNumber}";                         // the key will be set to 7 days with the filmnumber 
                        seatData.Add(dayAndScreenNum, initlizeScreen());                    // creates a new dictionary value with the specifed key and a value of a empty 2d string array of seats
                    }
                }
            }
            return seatData;                                                                // returns the newly created cinema screen data
        }

        static int GetIntInput(string msg, int lowerBound, int upperBound)
        {
            int userIntput = 0;                                                             // instiltise the vairble userIntput an assigns it to 0
            bool valid = false;                                                             // instiltise the bool valid to be the while loop variable 

            while (valid == false)                                                          // runs for ever unitill conditions are met
            {
                try
                {
                    Console.WriteLine(msg);                                                 // out puts the argument msg which will ask the user to input an int
                    userIntput = int.Parse(Console.ReadLine());                             // gets the user input as a string and parses it to an int

                    if (userIntput >= lowerBound && userIntput <= upperBound)               // checks weather the users int is within the correct upper and lower bounds
                        valid = true;                                                       // the int is valid so valid is set to true to end the loop
                    else
                        Console.WriteLine($"invlaid input must be more than {lowerBound} and less than {upperBound}");  // outputs the message to tell the users the bounds needed for the input
                }
                catch
                {
                    Console.WriteLine("You input was not an int");                          // printed when the input is not an int
                }
            }

            return userIntput;                                                              // returns the int that the user inputed
        }
        static int SmallestInt(int[] arr)       
        {
            int smallest = arr[0];                   // gets the first value of the array
            for (int i = 1; i < arr.Length; i++)     // sets i as the loop variable and loops for the length of the array
            {
                if (arr[i] < smallest)              // sees if the current index of the array is less then the smallest
                {
                    smallest = arr[i];              // assigns the varible arr[i] to smallest
                }
            }
            return smallest;                        // returns the smallest found number
        }
        static string[,] DisplayFilms(Dictionary<string, string[,]> screens)
        {

            string[,] filmList = new string[5, 4] { { "1", "toy story", "u", "0" }, { "2", "superman", "12", "12" }, { "3", "planes", "u", "0" }, { "4", "1917", "18", "18" }, { "5", "set", "15", "15" } };
            // an array of all the films their number and age rating is created

            Console.WriteLine("\n-------------------------------------------------------------");
            for (int x = 0; x < 5; x++)                         // loops for 5 times
            {
                for (int y = 0; y < 3; y++)                     // loops for 3 times
                {
                    Console.Write($"{filmList[x, y],10}");      // writes the index of filmlist with the current variable and adds a spacing of 10
                }

                DisplayTicketsSold(screens, x);                 // calls a procdure to display the number of tickets sold

            }
            Console.WriteLine("\n-------------------------------------------------------------");

            return filmList;                                    // returns the list of films
        }
        static void DisplayTicketsSold(Dictionary<string, string[,]> screens, int filmNumber)
        {
            int ticketsSold = 0;                                                        //creates and instisles the variable ticketsSold
                
            // loop through only the certain cinema screen
            for (int daysInAdvance = 0; daysInAdvance < 8; daysInAdvance++)             // loops 8 times for the 8 possible days
            {
                string[,] seats = screens[$"{daysInAdvance},{filmNumber + 1}"];         // gets the seats for the current date and the film

                foreach (string seat in seats)                                          // loops through the seat array
                {
                    if (seat == "X")                                                    // sees if the current seat value  is X
                    {
                        ticketsSold++;                                                  // adds one to the tickets sold
                    }
                }
            }

            Console.WriteLine($"\tTotal tickets sold : {ticketsSold}");                 // displays the amount of tickets sold for the whole week for a certain film
        }
        static string MaxAgeRating(int minAge)
        {
            string maxAgeRating = "18";             // creates the maxAgeRating variable which is the max film rating the users group can see

            if (minAge < 12)                        // sees if the youngest member of the group is yonger than 12
                maxAgeRating = "U";
            else if (minAge < 15)                   // sees if the youngest member of the group is yonger than 15
                maxAgeRating = "12";               
            else if (minAge < 18)                   //sees if the youngest member of the group is yonger than 18
                maxAgeRating = "15";

            return maxAgeRating;                    // returns the string is the max film rating the users group can see
        }
        static (int, int[]) GetPeopleAges()
        {
            int numberOfPeople = GetIntInput("How many people are you booking for", 0, 45);             
            // gets the int amount of people the user is booking for making sure it is greater than 0 and no more than 45

            int[] ages = new int[numberOfPeople];           // creates an int array with the length of the number of people

            for (int x = 0; x < numberOfPeople; x++)        // loops through every single person
            {
                int ageOfPerson = GetIntInput($"\nWhat is the age of the number {x + 1} person in your booking", 0, 150);   
                // gets the age of the user making sure they are older than 0 and no more than 150 years old

                ages[x] = ageOfPerson;  //  sets the current index of ages to be the age of person
            }


            return (numberOfPeople, ages);  // return the intger of how many people and the int array of their ages
        }
        static (string, int) FilmSelection(string[,] filmLists, int minAge)
        {
            string film = "";           // Creates a string with the value ""
            bool valid = false;         // creates the bool valid to be used in the loop
            int filmNumber = 0;         // creates the int filmnunber as 0 to be used inside an outside the loop

            while (valid == false)      // loops while the selection is invalid
            {
                Console.WriteLine("\n");
                filmNumber = GetIntInput($"\nChoose the film number in which you would like to see", 0, 5);     // gets the user to input the film number they want between 1 and 5
                film = filmLists[filmNumber - 1, 1];    // sets the film to be the inputed choice - 1  to account for array starting at 0 not 1 and with the second bit being the index the names are stored
                Console.WriteLine($"You have choosen to see film number {filmNumber} this is film {film} is this correct yes or no");
                string doubleCheck = Console.ReadLine().ToLower();  // gets the user to say they are sure and makes it all lower case

                if (doubleCheck != "no")
                {
                    if (minAge >= int.Parse(filmLists[filmNumber - 1, 3]))  // checks if everyone is old enough by checking the 4th value of the filmlist array
                    {
                        Console.WriteLine("Eveyone in your group is old enough for the film\n");

                        // need to check if any screen has enough space for the group or possible softlock

                        valid = true;       //sets valid to be trute to end the loop
                    }
                    else
                        Console.WriteLine("One or more members of your group are too young to see the selected film please select a differant film");
                }
                else
                {
                    Console.WriteLine("You can choose a new film ");
                }
            }
            return (film, filmNumber);
        }
        static void Finsh(string selectedDate, double cost, int numberOfPeople, string film)
        {
            Console.WriteLine("\nYou will now be asked to pay");
            Payment(cost, numberOfPeople);



            Console.WriteLine("\nYour ticket will be printed bellow");



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
            {
                enoughSpace = false;
                Console.WriteLine("sorry this screen has too few seats remaining so please choose a differant day");
            }

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
                }
                Console.WriteLine($"For person {bookedseats + 1} you have book seat {xCorrdinate}{yCorrdinate}");
                seats[yCorrdinate - 1, xCorrdinate -1] = "X";
            }

            screens[$"{daysInAdvance},{filmNumber}"] = seats;

            DisplaySeats(screens[$"{daysInAdvance},{filmNumber}"]);

            Console.WriteLine("You have now book your seats\n");

            return screens;
        }
    }
}
