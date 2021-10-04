using System;
using System.IO;

namespace vendingmachine
{
    struct item
    {
        public string name;
        public double price;
        public string position;
        public int numberLeft;
    }

    class Program
    {
        static void Main(string[] args)
        {
            int numberOfItems = 12;
            item[] items = FillVendingMachine(numberOfItems);
            bool run = true;

            while (run == true)
            {
                Welcome(items); 
                string itemCode = GetproductCode(numberOfItems, items);
                ChargeForItem(int.Parse(itemCode) - 1, items);
                items = UpdateStock(items, int.Parse(itemCode) - 1);
                Console.Clear();
            }
        }

        static void Admin(item[] items){
            Console.WriteLine("ADMIN PANNEL");

            Console.WriteLine("edit delete or create item (E/D/C)");
            string option = Console.ReadLine();

            if (option == "E")
                items = EditItem(items);

            if (option == "D")
                items = DeleteItem(items);
                
            if (option == "C")
                items = CreateItem(items);
        // needs perma option which writes to file

        // logs all login attempts

        // has a password == "Adm1n"

        //Alows the user to change the price name of an item

        // when adding a new item it auto does the product number 

        //double checks to make sure the admin is correct in the price and item they added

        // closes the program
        }
        
        static items[] EditItem(item[] items){
            DisplayItems(items);
            Console.WriteLine("Enter the item code for which you wish to edit");
            int itemCode = int.Parse(Console.ReadLine());
            Console.WriteLine("Do you want to edit the name price or stock");
            string option = Console.ReadLine();

            Console.WriteLine("Enter the new value you wish to input");
            string newValue = Console.ReadLine();

            if (option == "name")
                items[itemCode - 1].name = newValue;
            if(option == "price")
                items[itemCode - 1].price = newValue;
            if(option == "stock")
                items[itemCode - 1].stock = newValue;

            return items
        }

        static item[] UpdateStock(item[] items, int itemCode){
            int stock = items[itemCode].numberLeft;
            items[itemCode].numberLeft = stock - 1;

            return items;
        }

        static void ChargeForItem(int itemCode, item[] items){
            double cost = items[itemCode].price;

            Console.WriteLine($"You have selected to buy : {items[itemCode].name}");
            Console.WriteLine($"This item costs : £{cost}");

            double amountInputed = Payment(cost);
        }

        static double Payment(double itemCost){
            double userPayment = 0.0;
            int[] paymentOptions = new int[] {10, 20, 50, 100, 200};

            Console.WriteLine("this vending machine only accepts 10p 20p 50p £1 and £2 coins");

            while (userPayment < itemCost)
            {
                double coin = GetDoubleInput("Please enter the amount you wish to pay", 0, 200);
                bool valid = linearSeach(paymentOptions, coin);

                if(valid == true){
                    Console.WriteLine("You hvae entered a valid coin");
                    Console.WriteLine($"You inputed a {coin} coin");
                    userPayment += coin / 100;
                }
                else
                    Console.WriteLine("please Enter Valid coin");

                Console.WriteLine($"You have inputed : £{userPayment}\n");
                Console.WriteLine("More money is needed to complete the transcation");
            }

            Console.WriteLine($"Your change is : £{userPayment - itemCost} Do you want this change Y/N");
            string output = Console.ReadLine();
            if(output != "n")
                Console.WriteLine($"Here is your change : {userPayment - itemCost}");

            return userPayment;
        }

        static bool linearSeach(int[] paymentTypes, double coin){
            bool valid = false;

            for (int i=0; i < paymentTypes.Length; i++){
                if(paymentTypes[i] == coin)
                    valid = true;
            }

            return valid;
        }

        static double GetDoubleInput(string msg, int lowerBound, int upperBound)
        {
            double userIntput = 0;                                                             // instiltise the vairble userIntput an assigns it to 0
            bool valid = false;                                                             // instiltise the bool valid to be the while loop variable 

            while (valid == false)                                                          // runs for ever unitill conditions are met
            {
                try
                {
                    Console.WriteLine(msg);                                                 // out puts the argument msg which will ask the user to input an int
                    userIntput = double.Parse(Console.ReadLine());                             // gets the user input as a string and parses it to an int

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

        static string GetproductCode(int amountOfItems, item[] items){
            bool found = false;
            string code = "";
            string productCode = "";
            while (found == false)
            {
                Console.WriteLine("Enter a product code");

                productCode = Console.ReadLine();

                if(productCode == "1337")
                    Admin(items);
                    //MIGHT NEED TO FORCE RESTART

                for (int i = 1; i < amountOfItems + 1; i++)
                {
                    if(i >= 10)
                        code = i.ToString();
                    else
                        code = "0" + i.ToString();
                    if(productCode == code){
                        Console.WriteLine("Your item has been found Lets check if we have any in stock");
                        int stock = items[int.Parse(productCode) - 1].numberLeft;
                        if(stock == 0)
                            Console.WriteLine($"we are sorry to inform you we are out of {items[int.Parse(productCode) - 1].name} please select a different item\n");
                        else
                            found = true;
                    }
                }
                if(found == false){
                    Console.WriteLine("Invalid product code\n");
                }    
            }



            return productCode;
        }





        //GETTING THE VENDING MACHINE TO DISPLAY
        static item[] FillVendingMachine(int amountOfItems){
            item[] items = new item[amountOfItems];

            for(int i =0; i < amountOfItems; i++){
                string[] itemStats = GetItems(i);
                items[i].name = itemStats[0];
                items[i].price = double.Parse(itemStats[1]);
                items[i].position = itemStats[2];
                items[i].numberLeft = int.Parse(itemStats[3]);

            }

            return items;
        }
        static string[] GetItems(int index){
            string directoary = System.IO.Directory.GetCurrentDirectory() + "/items.txt";
            string[] items = System.IO.File.ReadAllLines(directoary);

            return items[index].Split(",");

        }

        static void Welcome(item[] items){
            Console.WriteLine($"\n\n\nVending Machine\t current day {DateTime.Now.ToString("d/m/yyyy")}");
            Console.WriteLine("this vending machine only accepts 10p 20p 50p £1 and £2 coins");

            Console.Write("\n\n--------------");
            DisplayItems(items);
            Console.Write("--------------\n\n\n");

        }
        static void DisplayItems(item[] items){
            Console.WriteLine("Items are as shown bellow");

            Console.Write("NAME\t\t\t");Console.Write($"PRICE\t\t\t");Console.Write($"CODE\t\t\t");Console.Write($"AMOUNT\n");

            foreach(var item in items){
                Console.Write($"{item.name,15}");
                Console.Write($"{item.price, 15}");
                Console.Write($"{item.position, 15}");
                Console.Write($"{item.numberLeft, 15}\n");

            }
        }
    }
}
