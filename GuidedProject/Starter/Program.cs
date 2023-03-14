/*
This application manages transactions at a store check-out line. The
check-out line has a cash register, and the register has a cash till
that is prepared with a number of bills each morning. The till includes
bills of four denominations: $1, $5, $10, and $20. The till is used
to provide the customer with change during the transaction. The item 
cost is a randomly generated number between 2 and 49. The customer 
offers payment based on an algorithm that determines a number of bills
in each denomination. 

Each day, the cash till is loaded at the start of the day. As transactions
occur, the cash till is managed in a method named MakeChange (customer 
payments go in and the change returned to the customer comes out). A 
separate "safety check" calculation that's used to verify the amount of
money in the till is performed in the "main program". This safety check
is used to ensure that logic in the MakeChange method is working as 
expected.
*/

string? readResult = null;
bool useTestData = true;

Console.Clear();

int[] cashTill = new int[] { 0, 0, 0, 0 };
int registerCheckTillTotal = 0;

// registerDailyStartingCash: $1 x 50, $5 x 20, $10 x 10, $20 x 5 => ($350 total)
int[,] registerDailyStartingCash = new int[,] { { 1, 50 }, { 5, 20 }, { 10, 10 }, { 20, 5 } };

int[] testData = new int[] { 6, 10, 17, 20, 31, 36, 40, 41 };
int testCounter = 0;

LoadTillEachMorning(registerDailyStartingCash, cashTill);

registerCheckTillTotal = registerDailyStartingCash[0, 0] * registerDailyStartingCash[0, 1] + registerDailyStartingCash[1, 0] * registerDailyStartingCash[1, 1] + registerDailyStartingCash[2, 0] * registerDailyStartingCash[2, 1] + registerDailyStartingCash[3, 0] * registerDailyStartingCash[3, 1];

// display the number of bills of each denomination currently in the till
LogTillStatus(cashTill);

// display a message showing the amount of cash in the till
Console.WriteLine(TillAmountSummary(cashTill));

// display the expected registerDailyStartingCash total
Console.WriteLine($"Expected till value: {registerCheckTillTotal}\n\r");

var valueGenerator = new Random((int)DateTime.Now.Ticks);

int transactions = 10;

if (useTestData)
{
    transactions = testData.Length;
}

while (transactions > 0)
{
    transactions -= 1;
    int itemCost = valueGenerator.Next(2, 20);

    if (useTestData)
    {
        itemCost = testData[testCounter];
        testCounter += 1;
    }

    int paymentOnes = itemCost % 2;                 // value is 1 when itemCost is odd, value is 0 when itemCost is even
    int paymentFives = (itemCost % 10 > 7) ? 1 : 0; // value is 1 when itemCost ends with 8 or 9, otherwise value is 0
    int paymentTens = (itemCost % 20 > 13) ? 1 : 0; // value is 1 when 13 < itemCost < 20 OR 33 < itemCost < 40, otherwise value is 0
    int paymentTwenties = (itemCost < 20) ? 1 : 2;  // value is 1 when itemCost < 20, otherwise value is 2

    // display messages describing the current transaction
    Console.WriteLine($"Customer is making a ${itemCost} purchase");
    Console.WriteLine($"\t Using {paymentTwenties} twenty dollar bills");
    Console.WriteLine($"\t Using {paymentTens} ten dollar bills");
    Console.WriteLine($"\t Using {paymentFives} five dollar bills");
    Console.WriteLine($"\t Using {paymentOnes} one dollar bills");

    // MakeChange manages the transaction and updates the till 
    string transactionMessage = MakeChange(itemCost, cashTill, paymentTwenties, paymentTens, paymentFives, paymentOnes);

    // Backup Calculation - each transaction adds current "itemCost" to the till
    if (transactionMessage == "transaction succeeded")
    {
        Console.WriteLine($"Transaction successfully completed.");
        registerCheckTillTotal += itemCost;
    }
    else
    {
        Console.WriteLine($"Transaction unsuccessful: {transactionMessage}");
    }

    Console.WriteLine(TillAmountSummary(cashTill));
    Console.WriteLine($"Expected till value: {registerCheckTillTotal}\n\r");
    Console.WriteLine();
}

Console.WriteLine("Press the Enter key to exit");
do
{
    readResult = Console.ReadLine();

} while (readResult == null);


static void LoadTillEachMorning(int[,] registerDailyStartingCash, int[] cashTill)
{
    cashTill[0] = registerDailyStartingCash[0, 1];
    cashTill[1] = registerDailyStartingCash[1, 1];
    cashTill[2] = registerDailyStartingCash[2, 1];
    cashTill[3] = registerDailyStartingCash[3, 1];
}


static string MakeChange(int cost, int[] cashTill, int twenties, int tens = 0, int fives = 0, int ones = 0)
{
    string transactionMessage = "";

    cashTill[3] += twenties;
    cashTill[2] += tens;
    cashTill[1] += fives;
    cashTill[0] += ones;

    int amountPaid = twenties * 20 + tens * 10 + fives * 5 + ones;
    int changeNeeded = amountPaid - cost;

    if (changeNeeded < 0)
        transactionMessage = "Not enough money provided.";

    Console.WriteLine("Cashier Returns:");

    while ((changeNeeded > 19) && (cashTill[3] > 0))
    {
        cashTill[3]--;
        changeNeeded -= 20;
        Console.WriteLine("\t A twenty");
    }

    while ((changeNeeded > 9) && (cashTill[2] > 0))
    {
        cashTill[2]--;
        changeNeeded -= 10;
        Console.WriteLine("\t A ten");
    }

    while ((changeNeeded > 4) && (cashTill[1] > 0))
    {
        cashTill[2]--;
        changeNeeded -= 5;
        Console.WriteLine("\t A five");
    }

    while ((changeNeeded > 0) && (cashTill[0] > 0))
    {
        cashTill[0]--;
        changeNeeded--;
        Console.WriteLine("\t A one");
    }

    if (changeNeeded > 0)
        transactionMessage = "Can't make change. Do you have anything smaller?";

    if (transactionMessage == "")
        transactionMessage = "transaction succeeded";

    return transactionMessage;
}

static void LogTillStatus(int[] cashTill)
{
    Console.WriteLine("The till currently has:");
    Console.WriteLine($"{cashTill[3] * 20} in twenties");
    Console.WriteLine($"{cashTill[2] * 10} in tens");
    Console.WriteLine($"{cashTill[1] * 5} in fives");
    Console.WriteLine($"{cashTill[0]} in ones");
    Console.WriteLine();
}

static string TillAmountSummary(int[] cashTill)
{
    return $"The till has {cashTill[3] * 20 + cashTill[2] * 10 + cashTill[1] * 5 + cashTill[0]} dollars";

}

/* 

========================================
ORIGINAL STARTER PROJECT (Visual Studio)
========================================

using System;

namespace ExceptionsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var theBank = new Till(50, 20, 10, 5);

            var expectedTotal = 50 * 1 + 20 * 5 + 10 * 10 + 5 * 20;

            theBank.LogTillStatus();
            Console.WriteLine(theBank);
            Console.WriteLine($"Expected till value: {expectedTotal}");

            int transactions = 15;
            var valueGenerator = new Random((int)DateTime.Now.Ticks);

            while (transactions-- > 0)
            {
                int itemCost = valueGenerator.Next(2, 50);

                int paymentOnes = itemCost % 2;
                int paymentFives = (itemCost % 10 > 7) ? 1 : 0;
                int paymentTens = (itemCost % 20 > 13) ? 1 : 0;
                int paymentTwenties = (itemCost < 20) ? 1 : 2;

                try
                {
                    Console.WriteLine($"Customer making a £{itemCost} purchase");
                    Console.WriteLine($"\t Using {paymentTwenties} twenties");
                    Console.WriteLine($"\t Using {paymentTens} tenners");
                    Console.WriteLine($"\t Using {paymentFives} fivers");
                    Console.WriteLine($"\t Using {paymentOnes} one-pound coins");

                    theBank.MakeChange(itemCost, paymentTwenties, paymentTens, paymentFives, paymentOnes);

                    expectedTotal += itemCost;
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"Could not make transaction: {e.Message}");
                }

                Console.WriteLine(theBank);
                Console.WriteLine($"Expected till value: {expectedTotal}");
                Console.WriteLine();
            }
        }
    }



    public class Till
    {
        private int numOneDollarBills;
        private int numFiveDollarBills;
        private int numTenDollarBills;
        private int numTwentyDollarBills;

        public Till(int ones, int fives, int tens = 0, int twenties = 0) =>
            (numOneDollarBills, numFiveDollarBills, numTenDollarBills, numTwentyDollarBills) =
            (ones, fives, tens, twenties);

        public void MakeChange(int cost, int twenties, int tens = 0, int fives = 0, int ones = 0)
        {
            numTwentyDollarBills += twenties;
            numTenDollarBills += tens;
            numFiveDollarBills += fives;
            numOneDollarBills += ones;

            int amountPaid = twenties * 20 + tens * 10 + fives * 5 + ones;
            int changeNeeded = amountPaid - cost;

            if (changeNeeded < 0)
                throw new InvalidOperationException("Not enough money provided");

            Console.WriteLine("Cashier Returns:");

            while ((changeNeeded > 19) && (numTwentyDollarBills > 0))
            {
                numTwentyDollarBills--;
                changeNeeded -= 20;
                Console.WriteLine("\t A twenty");
            }

            while ((changeNeeded > 9) && (numTenDollarBills > 0))
            {
                numTenDollarBills--;
                changeNeeded -= 10;
                Console.WriteLine("\t A tenner");
            }

            while ((changeNeeded > 4) && (numFiveDollarBills > 0))
            {
                numFiveDollarBills--;
                changeNeeded -= 5;
                Console.WriteLine("\t A fiver");
            }

            while ((changeNeeded > 0) && (numOneDollarBills > 0))
            {
                numOneDollarBills--;
                changeNeeded--;
                Console.WriteLine("\t A one");
            }

            if (changeNeeded > 0)
                throw new InvalidOperationException("Can't make change. Do you have anything smaller?");
        }

        public void LogTillStatus()
        {
            Console.WriteLine("The till currently has:");
            Console.WriteLine($"{numTwentyDollarBills * 20} in twenties");
            Console.WriteLine($"{numTenDollarBills * 10} in tens");
            Console.WriteLine($"{numFiveDollarBills * 5} in fives");
            Console.WriteLine($"{numOneDollarBills} in ones");
            Console.WriteLine();
        }

        public override string ToString() =>
            $"The till has {numTwentyDollarBills * 20 + numTenDollarBills * 10 + numFiveDollarBills * 5 + numOneDollarBills} dollars";
    }
}


=========================================
ORIGINAL FINISHED PROJECT (Visual Studio)
=========================================


using System;

namespace ExceptionsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var theBank = new Till(50, 20, 10, 5);

            var expectedTotal = 50 * 1 + 20 * 5 + 10 * 10 + 5 * 20;

            theBank.LogTillStatus();
            Console.WriteLine(theBank);
            Console.WriteLine($"Expected till value: {expectedTotal}");

            int transactions = 500;
            var valueGenerator = new Random((int)DateTime.Now.Ticks);

            while (transactions-- > 0)
            {
                int itemCost = valueGenerator.Next(2, 50);

                int paymentOnes = itemCost % 2;
                int paymentFives = (itemCost % 10 > 7) ? 1 : 0;
                int paymentTens = (itemCost % 20 > 13) ? 1 : 0;
                int paymentTwenties = (itemCost < 20) ? 1 : 2;

                try
                {
                    Console.WriteLine($"Customer making a £{itemCost} purchase");
                    Console.WriteLine($"\t Using {paymentTwenties} twenties");
                    Console.WriteLine($"\t Using {paymentTens} tenners");
                    Console.WriteLine($"\t Using {paymentFives} fivers");
                    Console.WriteLine($"\t Using {paymentOnes} one-pound coins");

                    theBank.MakeChange(itemCost, paymentTwenties, paymentTens, paymentFives, paymentOnes);

                    expectedTotal += itemCost;
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"Could not make transaction: {e.Message}");
                }

                Console.WriteLine(theBank);
                Console.WriteLine($"Expected till value: {expectedTotal}");
                Console.WriteLine();
            }
        }
    }



    public class Till
    {
        private int OneDollarBills;
        private int FiveDollarBills;
        private int TenDollarBills;
        private int TwentyDollarBills;

        public Till(int ones, int fives, int tens = 0, int twenties = 0) =>
            (OneDollarBills, FiveDollarBills, TenDollarBills, TwentyDollarBills) = (ones, fives, tens, twenties);

        public void MakeChange(int cost, int twenties, int tens = 0, int fives = 0, int ones = 0)
        {
            var twentyDollarBillsInHand = TwentyDollarBills + twenties;
            var tenDollarBillsInHand = TenDollarBills + tens;
            var fiveDollarBillsInHand = FiveDollarBills + fives;
            var onesInHand = OneDollarBills + ones;

            int amountPaid = twenties * 20 + tens * 10 + fives * 5 + ones;
            int changeNeeded = amountPaid - cost;

            if (changeNeeded < 0)
                throw new InvalidOperationException("Not enough money provided");

            Console.WriteLine("Cashier Returns:");

            while ((changeNeeded > 19) && (twentyDollarBillsInHand > 0))
            {
                twentyDollarBillsInHand--;
                changeNeeded -= 20;
                Console.WriteLine("\t A twenty");
            }

            while ((changeNeeded > 9) && (tenDollarBillsInHand > 0))
            {
                tenDollarBillsInHand--;
                changeNeeded -= 10;
                Console.WriteLine("\t A tenner");
            }

            while ((changeNeeded > 4) && (fiveDollarBillsInHand > 0))
            {
                fiveDollarBillsInHand--;
                changeNeeded -= 5;
                Console.WriteLine("\t A fiver");
            }

            while ((changeNeeded > 0) && (onesInHand > 0))
            {
                onesInHand--;
                changeNeeded--;
                Console.WriteLine("\t A one");
            }

            if (changeNeeded > 0)
                throw new InvalidOperationException("Can't make change. Do you have anything smaller?");

            TwentyDollarBills = twentyDollarBillsInHand;
            TenDollarBills = tenDollarBillsInHand;
            FiveDollarBills = fiveDollarBillsInHand;
            OneDollarBills = onesInHand;
        }

        public void LogTillStatus()
        {
            Console.WriteLine("The till currently has:");
            Console.WriteLine($"{TwentyDollarBills * 20} in twenties");
            Console.WriteLine($"{TenDollarBills * 10} in tens");
            Console.WriteLine($"{FiveDollarBills * 5} in fives");
            Console.WriteLine($"{OneDollarBills} in ones");
            Console.WriteLine();
        }

        public override string ToString() =>
            $"The till has {TwentyDollarBills * 20 + TenDollarBills * 10 + FiveDollarBills * 5 + OneDollarBills} dollars";
    }
}

*/