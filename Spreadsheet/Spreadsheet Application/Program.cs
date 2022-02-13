using FormulaEvaluator;
using System;
using static FormulaEvaluator.Evaluator;

namespace Spreadsheet_Application
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(Evaluator.Evaluate("1+2", SimpleVarTest)); //passed simple addition ANS: 3
            Console.WriteLine(Evaluator.Evaluate("2*2", SimpleVarTest)); //PASSED: test simple multiplication ANS: 4
            Console.WriteLine(Evaluator.Evaluate("8/4", SimpleVarTest)); //PASSED: test simple division ANS: 2
            Console.WriteLine(Evaluator.Evaluate("1+A6", SimpleVarTest)); //PASSED: test simple variable ANS: 2
            Console.WriteLine(Evaluator.Evaluate("14-3", SimpleVarTest)); //PASSED: test simple subtraction ANS: 11
            Console.WriteLine(Evaluator.Evaluate("(1+2)+3", SimpleVarTest)); //PASSED: test medium addition ANS: 6
            Console.WriteLine(Evaluator.Evaluate("1+AA66", SimpleVarTest)); //PASSED: test medium variable ANS: 2
                                                                            // Console.WriteLine(Evaluator.Evaluate("1+AA$66", SimpleVarTest)); // PASSED: test thrown exception invalid token
                                                                            //Console.WriteLine(Evaluator.Evaluate("1+AA66&", SimpleVarTest)); // PASSED: test edgecase for invalid token
            Console.WriteLine(Evaluator.Evaluate("( 1 + 2)", SimpleVarTest)); // PASSED: test leading spaces
                                                                              // Console.WriteLine(Evaluator.Evaluate("4/0", SimpleVarTest)); // PASSED: test divide by zero exception
                                                                              //Console.WriteLine(Evaluator.Evaluate("( 1 + 2 3)", SimpleVarTest)); // PASSED: test false token
            Console.WriteLine(Evaluator.Evaluate("0-5", SimpleVarTest)); //PASSED: test negative answer
            Console.WriteLine(Evaluator.Evaluate("( 1 + b5) - 3", SimpleVarTest)); //PASSED: test negative answer with variable.
            Console.WriteLine(Evaluator.Evaluate("(( a4 + b5) - z8 + aa6) * 2", SimpleVarTest)); // PASSED: test multiple variables
            Console.WriteLine(Evaluator.Evaluate("200* (( 50 + 100 - 100 / 5) - (3 + 2 * 2) + 2 ) / ( (((5 * 3))) + 27 / 9 + 5)", SimpleVarTest)); // PASSED

        }

        public static int SimpleVarTest(String s)
        {
            String test = s;
            return 1;
        }

    }
}
