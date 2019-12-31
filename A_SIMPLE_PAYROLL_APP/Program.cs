using System;

namespace A_SIMPLE_PAYROLL_APP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        class Staff
        {
            private double hourlyRate;
            private int hWorked;

            public double TotalPay { get; set; }
            public double BasicPay { get; set; }
            public string NameOfStaff { get; set; }

            public Staff(string name, double rate)
            {
                NameOfStaff = name;
                hourlyRate = rate;
            }

            public virtual void CalculatePay()
            {
                Console.WriteLine("Calculating Pay ...");
                BasicPay = hWorked * hourlyRate;
                TotalPay = BasicPay;
            }

            public override string ToString()
            {
                return $"Staff Name: {NameOfStaff}\n____________________________\nHourly Rate: {hourlyRate:C}\nHours Worked: {hWorked}\nBasic Pay: {BasicPay}\nTotal Pay: {TotalPay}";
            }
        }

    }
}
