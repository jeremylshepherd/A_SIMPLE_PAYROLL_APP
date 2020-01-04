using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace A_SIMPLE_PAYROLL_APP
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff;
            FileReader fr = new FileReader();
            int month = 0, year = 0;

            while (year == 0)
            {
                Console.Write("\nPlease enter the year:" );

                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch(FormatException)
                {
                    Console.WriteLine("Error: Wrong format, please enter an integer.\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            while (month == 0)
            {
                Console.Write("\nPlease enter the month:");

                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("Month be from 1 to 12. Please try again.");
                        month = 0;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error: Wrong format, please enter an integer.\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.WriteLine($"\nEnter the hours worked for {myStaff[i].NameOfStaff}: ");
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }                
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.Read();
        }

        class Staff
        {
            private double hourlyRate;
            private int hWorked;

            public double TotalPay { get; set; }
            public double BasicPay { get; set; }
            public string NameOfStaff { get; set; }
            public int HoursWorked
            {
                get
                {
                    return hWorked;
                }
                set
                {
                    if (value > 0)
                        hWorked = value;
                    else
                        hWorked = 0;
                }
            }

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
                return $"Staff -> Name: {NameOfStaff}\n____________________________\nHourly Rate: {hourlyRate:C}\nHours Worked: {HoursWorked}\nBasic Pay: {BasicPay}\nTotal Pay: {TotalPay}";
            }
        }

        class Manager : Staff
        {
            private const double managerHourlyRate = 50;

            public int Allowance { get; private set; }

            public Manager(string name) : base(name, managerHourlyRate)
            {
                //Nothing further needed
                Console.WriteLine("Manager constructor invoked");
            }

            public override void CalculatePay()
            {
                base.CalculatePay();
                Allowance = 1000;

                if(HoursWorked > 160)
                {
                    TotalPay += Allowance;
                }
            }

            public override string ToString()
            {
                return $"Manager -> Name: {NameOfStaff}\n_________________________\nHourly Rate: {managerHourlyRate}\nHours Worked: {HoursWorked}\nBasic Pay: {BasicPay}\nTotal Pay: {TotalPay}";
            }
        }

        class Admin : Staff
        {
            private const double overtimeRate = 15.5, adminHourlyRate = 30;

            public double Overtime { get; private set; }

            public Admin(string name) : base(name, adminHourlyRate)
            {
                //Nothing further needed
                Console.WriteLine("Admin constructor invoked");
            }

            public override void CalculatePay()
            {
                base.CalculatePay();
                if(HoursWorked > 160)
                {
                    Overtime = overtimeRate * (HoursWorked - 160);
                    TotalPay += Overtime;
                }

            }

            public override string ToString()
            {
                return $"Admin -> Name: {NameOfStaff}\n_________________________\nHourly Rate: {adminHourlyRate}\nHours Worked: {HoursWorked}\nBasic Pay: {BasicPay}\nTotal Pay: {TotalPay}";
            }

        }

        class FileReader
        {
            public List<Staff> ReadFile()
            {
                List<Staff> myStaff = new List<Staff>();
                string[] result = new string[2];
                string path = "staff.txt";
                string[] separator = { ", " };

                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (!sr.EndOfStream)
                        {
                            result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);

                            if(result[1] == "Manager")
                            {
                                myStaff.Add(new Manager(result[0]));
                            }
                            else if(result[1] == "Admin")
                            {
                                myStaff.Add(new Admin(result[0]));
                            }                            
                        }

                        sr.Close();
                    }
                }
                else
                {
                    Console.WriteLine("Error: File does not exist");
                }

                return myStaff;

            }
        }

        class PaySlip
        {
            private int month, year;
            enum MonthsOfYear { JAN = 1, FEB = 2, MAR = 3, APR = 4, MAY = 5, JUN = 6, JUL = 7, AUG = 8, SEP = 9, OCT = 10, NOV = 11, DEC = 12};

            public PaySlip(int payMonth, int payYear)
            {
                month = payMonth;
                year = payYear;
            }

            public void GeneratePaySlip(List<Staff> myStaff)
            {
                string path;

                foreach (Staff f in myStaff)
                {
                    path = $"{f.NameOfStaff}.txt";

                    using(StreamWriter sw = new StreamWriter(path))
                    {
                        sw.WriteLine($"PAYSLIP FOR {(MonthsOfYear)month} {year}");
                        sw.WriteLine("===============================================");
                        sw.WriteLine($"Name of Staff: {f.NameOfStaff}");
                        sw.WriteLine($"Hours Worked: {f.HoursWorked}");
                        sw.WriteLine("\n");
                        sw.WriteLine($"Basic Pay: {f.BasicPay:C}");

                        if(f.GetType() == typeof(Manager))
                            sw.WriteLine($"Allowance: {((Manager)f).Allowance}");
                        else if(f.GetType() == typeof(Admin))
                            sw.WriteLine($"OVertime: {((Admin)f).Overtime}");                      

                        sw.WriteLine("\n");
                        sw.WriteLine("===============================================");
                        sw.WriteLine($"Total Pay: {f.TotalPay:C}");
                        sw.WriteLine("===============================================");
                        sw.Close();
                    }
                }
            }

            public void GenerateSummary(List<Staff> myStaff)
            {
                var result =
                    from staff in myStaff
                    where (staff.HoursWorked < 10)
                    orderby staff.NameOfStaff ascending
                    select new { staff.NameOfStaff, staff.HoursWorked };

                string path = "summary.txt";

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("Staff with less than 10 working hours\n");
                    foreach (var staff in result)                    
                        sw.WriteLine($"Name of Staff: {staff.NameOfStaff}, Hours Worked: {staff.HoursWorked}");

                    sw.Close();
                }                   
            }

            public override string ToString()
            {
                return $"PaySlip -> month: {month}, year = {year}";
            }
        }
    }
}
