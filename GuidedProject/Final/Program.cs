/*
LP6 Guided project - Implement debugger and exception handling

Introduction.md

- Scenario: Suppose you are developing an application that will be used to help with the customer check-out process at a thrift store register. You are focussed on a method that calculates the amount of change that should be returned to the customer based on the item cost and the amount of money received from the customer. The 'MakeChange' method must keep track of the money that can be used to make change for the customer, which must include the money in the till and the bills that the customer provides as payment. The 'MakeChange' method must track the number of bills of each denomination, and must check to ensure that the customer has provided enough money to cover the cost of the item. The 'MakeChange' method needs to throw an exception when the customer doesn't offer enough money to cover the cost of the item, and when the till isn't able to make change with the bills that are available. Any exception that's thrown inside 'MakeChange' should be caught and processed by the application that calls it. 

Prepare.md

Coding approach: Develop a console application that can be used to test the 'MakeChange' method using a transaction-based approach. The console application should independently verify the amount of money in the till at the end of each transaction. Testing the logic of the 'MakeChange' method can begin with a small number of transactions defined by an array of item costs. Final testing should simulate a full day of up to 500 randomly generated transactions. The thrift store sells items in the range of $2 to $49.

> [!NOTE]
> To keep calculations as simple as possible so that the focus is on debugging the code logic, this activity assumes that all item costs are whole numbers and the item cost includes any tax or fee. 

- Method workflow: The 'MakeChange' method that you've developed implements the following process:

    - Input parameters: The following parameters are passed into the 'MakeChange' method: 

        - The cost of the item being purchased: 'itemCost' 
        - An integer array containing the number of bills in the till for each denomination: 'cashTill'
        - The payment offered by the customer, where the number of bills for each denomination is specified separately: `paymentTwenties`, `paymentTens`, `paymentFives`, `paymentOnes`

    - Cash available in till: You must know the number of bills of each denomination before making change. You must include the bills offered as payment by the customer.  

    - Change owed to customer: Determine how much change is owed to the customer. This is the amount paid by the customer minus the cost of the item. The amount paid is a calculated value based on the number of each denomination provided by the customer (input parameters). 

    - Underpayment exception: Ensure that the customer has offer a payment that is greater than the item cost. If the customer has not offered sufficient payment, throw an exception and cancel the transaction.

    - Make change: Determine how many bills of each denomination must be returned to the customer as change to complete the transaction. Start with the largest bill denomination that is less than the change owed and work down to the smallest denomination. NOTE: You must ensure that a bill of that denomination is available in the till before adding it to the change returned to the customer.

    - Till exception: If the till doesn't contain the correct bills to produce the exact change, throw and exception and cancel the transaction.


Demonstrate code logic issues
=============================

We have purposely included code logic issues in the 'MakeChange' method code.  that are related to exception handling: 

- One is when the amount paid doesn't cover the cost of the items. 
- The other is when the till doesn't have enough small bills to make change.


It looks simple. Now, without looking at the implementation of the till class, show the test code in program.cs. Once again, stress the simplicity.

Run the simulation with a small set of predefined transactions, like you would in a unit test. It looks great. 

Now, run the simulation with a larger number of random transactions. Point out that the 'MakeChange' method contains code logic and exception handling issues that result in a mismanaged cash till.  


Perform the activity
====================

You completed initial testing of the 'MakeChange' method using a number of predefined transactions and everything looked good. However, the larger simulation representing a full day of transactions has exposed that the 'MakeChange' method contains logic bug that must be identified and managed.

Examine the code in the 'MakeChange' method. Your goal is to "fix" the code so that it still throws exceptions for the two error conditions, but implements recovery from the exceptions in a "safe" way. That means the state of the till cannot change when a transaction can't be completed.

There are a number of ways to implement the change, but all share some common themes discussed in the LP modules.

Your assignment: Use code debugging tools and exception handling techniques to identify logic issues and update the 'MakeChange' method.   


Review the problems
===================

In the starter code, we can see variable state being altered prior detecting issues (throwing exceptions). If you run the starter code using a large number of simulated transactions, it will always fail eventually.

The solution is to manage the till in conjunction with exception handing.

Key point: This is a small simulation, but it demonstrates a common problem in large code bases: Unless unit tests are carefully constructed, exceptions may not always occur in your test environments. It's only under production stress that these appear. Then, data errors happen with live customer data. It gets very expensive and becomes a data recovery issue.


## Post-lab discussion

When Bill delivered this for the first time, we were lucky enough to have different groups each pick one of the different ways to solve the problem. In addition, two groups went down dark alleys and helped reinforce the claim that this is harder than it might first appear. *If you have groups fail, use it as a teaching moment that exceptions don't mean error handling is "easy".*
*/



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
bool useTestData = false;

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
Console.WriteLine($"Expected till value: {registerCheckTillTotal}");
Console.WriteLine();

var valueGenerator = new Random((int)DateTime.Now.Ticks);

int transactions = 10;

if (useTestData)
{
    transactions = testData.Length;
}

while (transactions > 0)
{
    transactions -= 1;
    int itemCost = valueGenerator.Next(2, 50);

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

    try
    {
        // MakeChange manages the transaction and updates the till 
        MakeChange(itemCost, cashTill, paymentTwenties, paymentTens, paymentFives, paymentOnes);

        // Backup Calculation - each transaction adds current "itemCost" to the till
        registerCheckTillTotal += itemCost;
    }
    catch (InvalidOperationException e)
    {
        Console.WriteLine($"Could not make transaction: {e.Message}");
    }

    Console.WriteLine(TillAmountSummary(cashTill));
    Console.WriteLine($"Expected till value: {registerCheckTillTotal}");
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


static void MakeChange(int cost, int[] cashTill, int twenties, int tens = 0, int fives = 0, int ones = 0)
{
    // cashTill[3] += twenties;
    // cashTill[2] += tens;
    // cashTill[1] += fives;
    // cashTill[0] += ones;

    int availableTwenties = cashTill[3] += twenties;
    int availableTens = cashTill[2] += tens;
    int availableFives = cashTill[1] += fives;
    int availableOnes = cashTill[0] += ones;

    int amountPaid = twenties * 20 + tens * 10 + fives * 5 + ones;
    int changeNeeded = amountPaid - cost;

    if (changeNeeded < 0)
        throw new InvalidOperationException("Not enough money provided");

    Console.WriteLine("Cashier Returns:");

    while ((changeNeeded > 19) && (cashTill[3] > 0))
    {
        cashTill[3]--;
        // availableTwenties--;
        changeNeeded -= 20;
        Console.WriteLine("\t A twenty");
    }

    while ((changeNeeded > 9) && (cashTill[2] > 0))
    {
        cashTill[2]--;
        // availableTens--;
        changeNeeded -= 10;
        Console.WriteLine("\t A ten");
    }

    while ((changeNeeded > 4) && (cashTill[1] > 0))
    {
        cashTill[1]--;
        // availableFives--;
        changeNeeded -= 5;
        Console.WriteLine("\t A five");
    }

    while ((changeNeeded > 0) && (cashTill[0] > 0))
    {
        cashTill[0]--;
        // availableOnes--;
        changeNeeded -= 1;
        Console.WriteLine("\t A one");
    }

    if (changeNeeded > 0)
        throw new InvalidOperationException("Can't make change. Do you have anything smaller?");

    cashTill[0] = availableOnes;
    cashTill[1] = availableFives;
    cashTill[2] = availableTens;
    cashTill[3] = availableTwenties;

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