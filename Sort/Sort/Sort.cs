using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Sort
{
    class Sort
    {
        private static int CondCount { get; set; }                                                                      // Condition counter (1 or 2)
        private static readonly string CurrDir = Directory.GetParent(Directory.GetCurrentDirectory()).
            Parent.Parent.FullName;                                                                                     // Path to the project(current) directory
        private static bool Descend { get; set; }                                                                       // Descending sort mode
        private static bool Number { get; set; }                                                                        // Input type is integer


        static void Main(string[] args)
        {
            Program();
        }


        public static void Program()
        {
            // Guide for user
            Console.WriteLine("PROCEDURE:" +
                "\n*) Type '-a' for ASC sort or '-d' for DESC sort (default '-a')" +
                "\n1) Type '-s' for String or '-i' for Integer" +
                "\n2) Enter the name of the output file" +
                "\n3) Enter the name(s) of input file(s)" +
                "\n\nEXAMPLE:" +
                "\n-i -a out.txt in.txt [for Integers by ASC]" +
                "\n-s out.txt in1.txt in2.txt in3.txt [for Strings by ASC]" +
                "\n-d -s out.txt in1.txt in2.txt [for Strings by DESC]\n");


            string input = Regex.Replace(Console.ReadLine(), @"\s+", " ");                                              // Replace multiple spaces with one space
            string[] tmp = input.Split(' ');
            List<string> parameters = new List<string>(tmp);


            // Check for correct input form
            if (!Conditions(parameters))
            {
                Console.WriteLine("Incorrect input!\n");
            }


            // Check for type input
            if (!InpuType(parameters))
            {
                Console.WriteLine("Parameters are absent!");
            }


            // Check for conditions
            Circs(parameters);


            // Output file
            string outFile = parameters[CondCount];


            // Input file(s) sort and extract it's content to the output file
            if (PreSort(outFile, parameters))
            {
                Console.WriteLine("File(s) content sorted");
            }


            // Sorting inside output file
            if(MainSort(outFile, parameters))
            {
                Console.WriteLine("Output file sorted");
            }
        }


        private static bool Conditions(List<string> param)
        {
            try
            {
                if (param[0].Contains('-') && (param[1].Contains('-')))
                {
                    CondCount = 2;
                    for (int i = 0; i < 2; i++)
                    {
                        if (param[i].Substring(0, 1).Equals("-") && param[i].Length == 2)
                        {
                            // Check two conditions
                            continue;
                        }
                        // If condition is in incorrect form
                        return false;
                    }
                    return true;
                }
                else if (param[0].Contains('-'))
                {
                    CondCount = 1;
                    if (param[0].Substring(0, 1).Equals("-") && param[0].Length == 2)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Condition Error: " + ex.Message);
            }


            return false;
        }


        private static void Circs(List<string> param)
        {
            try
            {
                for (int i = 0; i < CondCount; i++)
                {
                    switch (param[i].Substring(1, 1))
                    {
                        case "a":
                            Descend = false;
                            break;

                        case "d":
                            Descend = true;
                            break;

                        case "s":
                            Number = false;
                            break;

                        case "i":
                            Number = true;
                            break;

                        default:
                            Console.WriteLine("Incorrect condition '" + param[i] + "'!");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Circs Error: " + ex.Message);
            }
        }


        private static bool InpuType(List<string> param)
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    if (param[i].Equals("-s") || param[i].Equals("-i"))
                    {
                        return true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Type Error: " + ex.Message);
            }

            return false;
        }


        private static bool PreSort(string outputFile, List<string> param)
        {
            try
            {
                // Clear output file
                File.WriteAllText(@CurrDir + "\\Resources\\" + outputFile, string.Empty);

                // Condition Count + 1 because we pass the name of the output file
                for (int i = CondCount + 1; i < param.Count; i++)
                {
                    string path = @CurrDir + "\\Resources\\" + param[i];                                                // Path to the resource directory
                    string[] lines = File.ReadAllLines(path);

                    // Unsorted lists
                    ArrayList numeric = new ArrayList();
                    ArrayList symbols = new ArrayList();

   
                    foreach(var line in lines)
                    {
                        // If line contains numbers
                        if (int.TryParse(line, out int ires))
                        {
                            numeric.Add(Int32.Parse(line));
                        }
                        // If line contains chars
                        else if (char.TryParse(line, out char cres))
                        {
                            symbols.Add(line.ToString());
                        }
                        // If line is string
                        else
                        {
                            symbols.Add(line);
                        }
                    }


                    // Sorted lists
                    ArrayList sortnum = new ArrayList();
                    ArrayList sortsym = new ArrayList();

                    sortnum = IntMergeSort(numeric);
                    sortsym = StrMergeSort(symbols);


                    // Clear file(s) content
                    File.WriteAllText(path, string.Empty);


                    // Rewrite file(s) content with sorting
                    using (var sw = new StreamWriter(path, true))
                    {
                        if (sortnum.Count > 0)
                        {
                            foreach (var tmp in sortnum)
                            {
                                sw.WriteLine(tmp.ToString());
                            }
                        }

                        if (sortsym.Count > 0)
                        {
                            foreach (var tmp in sortsym)
                            {
                                sw.WriteLine(tmp);
                            }
                        }
                    }


                    //Write result to output file
                    using (var sw = new StreamWriter(@CurrDir + "\\Resources\\" + outputFile, true))
                    {
                        if (Number)
                        {
                            foreach (var tmp in sortnum)
                            {
                                sw.WriteLine(tmp.ToString());
                            }
                        }
                        else
                        {
                            foreach (var tmp in sortsym)
                            {
                                sw.WriteLine(tmp);
                            }
                        }
                    }


                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pre-Sort File Error: " + ex.Message);
            }

            return false;
        }


        private static bool MainSort(string outputFile, List<string> param)
        {
            try
            {
                string[] lines = File.ReadAllLines(@CurrDir + "\\Resources\\" + outputFile);
                ArrayList unsorted = new ArrayList();
                ArrayList sorted = new ArrayList();


                // Clear output file
                File.WriteAllText(@CurrDir + "\\Resources\\" + outputFile, string.Empty);


                using (var sw = new StreamWriter(@CurrDir + "\\Resources\\" + outputFile, true))
                {
                    // If sort is by numbers
                    if (Number)
                    {
                        foreach (var line in lines)
                        {
                            unsorted.Add(Int32.Parse(line));
                        }
                        sorted = IntMergeSort(unsorted);
                        foreach (var tmp in sorted)
                        {
                            sw.WriteLine(tmp.ToString());
                        }
                    }
                    // If sort is by string
                    else
                    {
                        foreach (var line in lines)
                        {
                            unsorted.Add(line);
                        }
                        sorted = StrMergeSort(unsorted);
                        foreach (var tmp in sorted)
                        {
                            sw.WriteLine(tmp);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main Sorting Error: " + ex.Message);
            }

            return false;
        }


        private static ArrayList IntMergeSort(ArrayList param)
        {
            try
            {
                // If list contain only 1 element
                if(param.Count <= 1)
                {
                    return param;
                }

                ArrayList left = new ArrayList();
                ArrayList right = new ArrayList();

                int middle = param.Count / 2;

                // Divide list on left side
                for(int i = 0; i < middle; i++)
                {
                    left.Add(param[i]);
                }

                // Divide list on right side
                for(int i = middle; i < param.Count; i++)
                {
                    right.Add(param[i]);
                }

                // Continue dividing
                left = IntMergeSort(left);
                right = IntMergeSort(right);

                // Merge lists
                return IntMerge(left, right, Descend);

            }
            catch(Exception ex)
            {
                Console.WriteLine("Merge Sort Error: " + ex.Message);
            }

            return null;
        }


        private static ArrayList IntMerge(ArrayList left, ArrayList right, bool rightSort)
        {
            try
            {
                ArrayList result = new ArrayList();

                do
                {
                    // Asc sort
                    if (!rightSort)
                    {
                        if (left.Count > 0 && right.Count > 0)
                        {
                            if ((int)left[0] <= (int)right[0])
                            {
                                result.Add(left[0]);
                                left.Remove(left[0]);
                            }
                            else
                            {
                                result.Add(right[0]);
                                right.Remove(right[0]);
                            }
                        }
                        else if (left.Count > 0)
                        {
                            result.Add(left[0]);
                            left.Remove(left[0]);
                        }
                        else if (right.Count > 0)
                        {
                            result.Add(right[0]);
                            right.Remove(right[0]);
                        }
                    }
                    // Desc sort
                    else
                    {
                        if (left.Count > 0 && right.Count > 0)
                        {
                            if ((int)left[0] <= (int)right[0])
                            {
                                result.Add(right[0]);
                                right.Remove(right[0]);
                            }
                            else
                            {
                                result.Add(left[0]);
                                left.Remove(left[0]);
                            }
                        }
                        else if (left.Count > 0)
                        {
                            result.Add(left[0]);
                            left.Remove(left[0]);
                        }
                        else if (right.Count > 0)
                        {
                            result.Add(right[0]);
                            right.Remove(right[0]);
                        }
                    }
                    
                }
                while (left.Count > 0 || right.Count > 0);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Merge Error: " + ex.Message);
            }

            return null;
        }


        private static ArrayList StrMergeSort(ArrayList param)
        {
            try
            {
                if (param.Count <= 1)
                {
                    return param;
                }

                ArrayList left = new ArrayList();
                ArrayList right = new ArrayList();

                int middle = param.Count / 2;

                for(int i = 0; i < middle; i++)
                {
                    left.Add(param[i]);
                }

                for(int i = middle; i < param.Count; i++)
                {
                    right.Add(param[i]);
                }

                left = StrMergeSort(left);
                right = StrMergeSort(right);

                return StrMerge(left, right, Descend);
            }
            catch(Exception ex)
            {
                Console.WriteLine("String Merge Sort Error: " + ex.Message);
            }

            return null;
        } 


        private static ArrayList StrMerge(ArrayList left, ArrayList right, bool rightSort)
        {
            try
            {
                ArrayList result = new ArrayList();

                do
                {
                    // Asc sort
                    if (!rightSort)
                    {
                        if (left.Count > 0 && right.Count > 0)
                        {
                            if (left[0].ToString().CompareTo(right[0].ToString()) < 0)
                            {
                                result.Add(left[0]);
                                left.Remove(left[0]);
                            }
                            else
                            {
                                result.Add(right[0]);
                                right.Remove(right[0]);
                            }
                        }
                        else if (left.Count > 0)
                        {
                            result.Add(left[0]);
                            left.Remove(left[0]);
                        }
                        else if (right.Count > 0)
                        {
                            result.Add(right[0]);
                            right.Remove(right[0]);
                        }
                    }
                    // Desc sort
                    else
                    {
                        if (left.Count > 0 && right.Count > 0)
                        {
                            if (left[0].ToString().CompareTo(right[0].ToString()) < 0)
                            {
                                result.Add(right[0]);
                                right.Remove(right[0]);
                            }
                            else
                            {
                                result.Add(left[0]);
                                left.Remove(left[0]);
                            }
                        }
                        else if (left.Count > 0)
                        {
                            result.Add(left[0]);
                            left.Remove(left[0]);
                        }
                        else if (right.Count > 0)
                        {
                            result.Add(right[0]);
                            right.Remove(right[0]);
                        }
                    }
                    
                }
                while (left.Count > 0 || right.Count > 0);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("String Merge Error: " + ex.Message);
            }

            return null;
        }
    }
}
