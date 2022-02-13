using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidGetCellContentsTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.GetCellContents("name");
            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNullGetCellContentsTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.GetCellContents(null);

        }

        [TestMethod]
        public void GetCellContentsEmptyTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            String generated = (String) test.GetCellContents("A1");
            String empty = "";

            Assert.AreEqual(empty, generated);
        }

        [TestMethod]
        public void SimpleSetCellContentsTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "25");
            HashSet<String> GeneratedList = new HashSet<string>(test.GetNamesOfAllNonemptyCells());
            HashSet<String> ExpectedList = new HashSet<string>();
            ExpectedList.Add("A1");

            Assert.IsTrue(ExpectedList.SetEquals(GeneratedList));
        }

        [TestMethod]
        public void SetCellContentsTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "25");
            test.SetContentsOfCell("B1", "string");
            test.SetContentsOfCell("C1", "45");
            HashSet<String> GeneratedList = new HashSet<string>(test.GetNamesOfAllNonemptyCells());
            HashSet<String> ExpectedList = new HashSet<string>();
            ExpectedList.Add("A1");
            ExpectedList.Add("B1");
            ExpectedList.Add("C1");
           
                Assert.IsTrue(ExpectedList.SetEquals(GeneratedList));
            

        }

        [TestMethod]
        public void SimpleGetCellContents()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A3", "text");
            Assert.AreEqual("text", test.GetCellContents("A3"));
        }

        [TestMethod]
        public void GetNonEmptyCellsEmptyTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            HashSet<String> list = new HashSet<String>(test.GetNamesOfAllNonemptyCells());
            HashSet<String> emptyList = new HashSet<string>();
            Assert.IsTrue(list.SetEquals(emptyList));
        }

        [TestMethod]
        public void SimpleGetNonEmptyCellsTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("B9", "129");
            test.SetContentsOfCell("Z4", "129");
            test.SetContentsOfCell("A2", "129");
            test.SetContentsOfCell("S1", "129");
            test.SetContentsOfCell("B1", "129");
            HashSet<String> list = new HashSet<String>(test.GetNamesOfAllNonemptyCells());
            HashSet<String> NonEmpty = new HashSet<string>() { "A2", "B1", "B9", "S1", "Z4" };

            Assert.IsTrue(list.SetEquals(NonEmpty));
        }

        [TestMethod]
        public void ChangeTypeOfCellContentTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "string");
            test.GetCellContents("a1");
            Assert.AreEqual("string", test.GetCellContents("a1"));
            test.SetContentsOfCell("a1", "14");
            Assert.AreEqual(14.0, test.GetCellContents("a1"));


        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellTextIsNullTest()
        {

            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("C1", (String)null);
        }

        [TestMethod]
        public void SetCellTextEmptyStringTest()
        {

            AbstractSpreadsheet test = new Spreadsheet();
           HashSet<String> list = new HashSet<string>(test.SetContentsOfCell("C1", ""));
            Assert.IsTrue(list.SetEquals(new HashSet<String>()));
        }

        [TestMethod]
        public void SetCellOverrideTextTest()
        {

            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("C1", "19");
            test.SetContentsOfCell("C1", "test");

            Assert.AreEqual("test", test.GetCellContents("C1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellFomulaNullTest()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("C1", null);

        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SimpleCircularDependencyTest()
        {

            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("C1", "=B1+C1");

        }



        //******************************CELL CLASS TESTS*************************************


        //[TestMethod]
        //public void SimpleGetCellStringTest()
        //{
        //    Cell cell = new Cell("test");
        //    cell.GetCellContents(out object x);
        //    Assert.AreEqual("test", x);
        //}

        //[TestMethod]
        //public void SimpleGetCellDoubleTest()
        //{
        //    Cell cell = new Cell(2.2);
        //    cell.GetCellContents(out object x);
        //    Assert.AreEqual(2.2, x);
        //}

        //[TestMethod]
        //public void SimpleGetCellFormulaTest()
        //{
        //    Formula f1 = new Formula("2+2");
        //    Cell cell = new Cell(f1);
        //    cell.GetCellContents(out object x);
        //    Assert.IsTrue(f1.Equals(x));
        //}

        //[TestMethod]
        //public void SimpleSetCellStringTest()
        //{
        //    Cell cell = new Cell("test");
        //    cell.GetCellContents(out object x);
        //    Assert.AreEqual("test", x);

        //    cell.SetCellContent("change");
        //    cell.GetCellContents(out object z);
        //    Assert.AreEqual("change", z);
        //}

        //[TestMethod]
        //public void SimpleSetCellFomulaTest()
        //{
        //    Cell cell = new Cell("test");
        //    cell.GetCellContents(out object x);
        //    Assert.AreEqual("test", x);

        //    cell.SetCellContent("change");
        //    cell.GetCellContents(out object z);
        //    Assert.AreEqual("change", z);

        //    Formula f1 = new Formula("2+2");
        //    cell = new Cell(f1);
        //    cell.GetCellContents(out object y);
        //    Assert.IsTrue(f1.Equals(y));
        //}

        // EMPTY SPREADSHEETS
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptyGetNull()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptyGetContents()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("1AA");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestGetEmptyContents()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("5")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetInvalidNameDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        public void TestSimpleSetDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNullStringVal()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullStringName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "hello");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestSetGetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNullFormVal()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", null);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullFormName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=2");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "=2");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        public void TestSetGetForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=3");
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(new Formula("3"), f);
            Assert.AreNotEqual(new Formula("2"), f);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(CircularException))]
        public void TestSimpleCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A2", "4");
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }


        [TestCategory("16")]
        [ExpectedException(typeof(CircularException))]
        public void TestComplexCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7", "=A1+A1");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCircular()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17b")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCellsCircular()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2");
                s.SetContentsOfCell("A2", "=A1");
            }
            catch (CircularException e)
            {
                Assert.AreEqual("", s.GetCellContents("A2"));
                Assert.IsTrue(new HashSet<string> { "A1" }.SetEquals(s.GetNamesOfAllNonemptyCells()));
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        public void TestEmptyNames()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        public void TestExplicitEmptySet()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        public void TestSimpleNamesString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestSimpleNamesDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestSimpleNamesFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestMixedNames()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestSetSingletonDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SequenceEqual(new List<string>() { "A1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestSetSingletonString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SequenceEqual(new List<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestSetSingletonFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SequenceEqual(new List<string>() { "C1" }));
        }



        // CHANGING CELLS
        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestChangeFtoD()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A2", "1.5");
            s.SetContentsOfCell("A3", "1.0");
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }



        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestChangeStoF()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestCategory("31")]
        public void TestStress1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            IList<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }

        // Repeated for extra weight

        [TestCategory("32")]
        public void TestStress1a()
        {
            TestStress1();
        }

        [TestCategory("33")]
        public void TestStress1b()
        {
            TestStress1();
        }

        [TestCategory("34")]
        public void TestStress1c()
        {
            TestStress1();
        }


        [TestCategory("35")]
        public void TestStress2()
        {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i + 1))));
            }
        }

        [TestCategory("36")]
        public void TestStress2a()
        {
            TestStress2();
        }

        [TestCategory("37")]
        public void TestStress2b()
        {
            TestStress2();
        }

        [TestCategory("38")]
        public void TestStress2c()
        {
            TestStress2();
        }


        [TestCategory("39")]
        public void TestStress3()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            try
            {
                s.SetContentsOfCell("A150", "=A50");
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }


        [TestCategory("40")]
        public void TestStress3a()
        {
            TestStress3();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestStress3b()
        {
            TestStress3();
        }

        [TestCategory("42")]
        public void TestStress3c()
        {
            TestStress3();
        }


        [TestCategory("43")]
        public void TestStress4()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            LinkedList<string> firstCells = new LinkedList<string>();
            LinkedList<string> lastCells = new LinkedList<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.AddFirst("A1" + i);
                lastCells.AddFirst("A1" + (i + 250));
            }

            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
        }

        [TestCategory("44")]
        public void TestStress4a()
        {
            TestStress4();
        }


        [TestCategory("45")]
        public void TestStress4b()
        {
            TestStress4();
        }


        [TestCategory("46")]
        public void TestStress4c()
        {
            TestStress4();
        }


        [TestCategory("47")]
        public void TestStress5()
        {
            RunRandomizedTest(47, 2519);
        }


        [TestCategory("48")]
        public void TestStress6()
        {
            RunRandomizedTest(48, 2521);
        }


        [TestCategory("49")]
        public void TestStress7()
        {
            RunRandomizedTest(49, 2526);
        }

        [TestCategory("50")]
        public void TestStress8()
        {
            RunRandomizedTest(50, 2521);
        }

        [TestMethod]
        public void SimpleXmlWriterTest()
        {
            try
            {


                using (XmlWriter writer = XmlWriter.Create("save1.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                AbstractSpreadsheet ss = new Spreadsheet("save1.txt", s => true, s => s, "");
                ss.Save("save1.txt");

                ss = new Spreadsheet("save1.txt", s => true, s => s, "");
                ss.GetCellContents("A1");

                Assert.AreEqual("hello", ss.GetCellContents("A1"));
            }
            finally
            {
                File.Delete("save1.txt");
            }
        }

        [TestMethod]
        public void SimpleXmlWReaderMethodTest()
        {
            try
            {


                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "default");

                ss.SetContentsOfCell("A1", "4");
                ss.SetContentsOfCell("B1", "3");

                ss.Save("save.txt");

                ss = new Spreadsheet("save.txt", s => true, s => s, "default");

                Assert.AreEqual(4.0, ss.GetCellContents("A1"));

                String output = ss.GetSavedVersion("save.txt");
                Assert.AreEqual("default", output);
            }

            finally
            {
                File.Delete("save.txt");
            }

        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SimpleXmlReaderExceptionTest()
        {
            try
            {


                using (XmlWriter writer = XmlWriter.Create("save2.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "=A1");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "default");
            }
            finally
            {      
                 File.Delete("save2.txt");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void XmlReaderNameExceptionTest()
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create("save3.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "_A1");
                    writer.WriteElementString("contents", "3");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                AbstractSpreadsheet ss = new Spreadsheet("save3.txt", s => true, s => s, "default");
            }
            finally
            {
                File.Delete("save3.txt");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void XmlReaderFormulaExceptionTest()
        {
            try
            {


                using (XmlWriter writer = XmlWriter.Create("save4.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "default");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "=++");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                AbstractSpreadsheet ss = new Spreadsheet("save4.txt", s => true, s => s, "default");
            }
            finally
            {
                File.Delete("save4.txt");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void XmlReaderVersionExceptionTest()
        {
            try
            {


                using (XmlWriter writer = XmlWriter.Create("save5.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "2");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "default");
            }
            finally
            {
                File.Delete("save5.txt");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void XmlWriterInvalidPathTest()
        {
            try
            {


                using (XmlWriter writer = XmlWriter.Create("save6.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "2");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                AbstractSpreadsheet ss = new Spreadsheet("save6.txt", s => true, s => s, "");

                ss.Save("/some/nonsense/path.xml");
            }
            finally
            {
                File.Delete("save6.txt");
            }
        }


        [TestMethod()]

        public void TestSetCellContentString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "hello");

            Assert.AreEqual(s.GetCellValue("A1"), "hello");
        }

        [TestMethod()]

        public void TestSetCellContentDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "6");

            Assert.AreEqual(s.GetCellValue("A1"), 6.0);
        }

        [TestMethod()]

        public void TestSetCellContent()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", "3");
            s.SetContentsOfCell("A4", "=2");
            s.SetContentsOfCell("A1", "=A5+A4");

            Assert.AreEqual(s.GetCellValue("A1"), 5.0);
        }





        /// <summary>
        /// Sets random contents for a random cell 10000 times
        /// </summary>
        /// <param name="seed">Random seed</param>
        /// <param name="size">The known resulting spreadsheet size, given the seed</param>
        public void RunRandomizedTest(int seed, int size)
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        /// <summary>
        /// Generates a random cell name with a capital letter and number between 1 - 99
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        /// <summary>
        /// Generates a random Formula
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }




    }
}
