using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{ 
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, Cell> cells;
        private DependencyGraph dg;
        private String versionName;


        // PARAGRAPHS 2 and 3 modified for PS5.
        /// <summary>
        /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
        /// spreadsheet consists of an infinite number of named cells.
        /// 
        /// A string is a cell name if and only if it consists of one or more letters,
        /// followed by one or more digits AND it satisfies the predicate IsValid.
        /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
        /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
        /// regardless of IsValid.
        /// 
        /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
        /// must be normalized with the Normalize method before it is used by or saved in 
        /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
        /// the Formula "x3+a5" should be converted to "X3+A5" before use.
        /// 
        /// A spreadsheet contains a cell corresponding to every possible cell name.  
        /// In addition to a name, each cell has a contents and a value.  The distinction is
        /// important.
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
        /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
        /// 
        /// In a new spreadsheet, the contents of every cell is the empty string.
        ///  
        /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
        /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
        /// in the grid.)
        /// 
        /// If a cell's contents is a string, its value is that string.
        /// 
        /// If a cell's contents is a double, its value is that double.
        /// 
        /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
        /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
        /// of course, can depend on the values of variables.  The value of a variable is the 
        /// value of the spreadsheet cell it names (if that cell's value is a double) or 
        /// is undefined (otherwise).
        /// 
        /// Spreadsheets are never allowed to contain a combination of Formulas that establish
        /// a circular dependency.  A circular dependency exists when a cell depends on itself.
        /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
        /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
        /// dependency.
        /// </summary>
        /// 
        public Spreadsheet()
    : this(s => true, s => s, "default")
        {

        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, String version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            Changed = true;
            versionName = version;

        }

        public Spreadsheet(String path, Func<string, bool> isValid, Func<string, string> normalize, String version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            versionName = version;
            Changed = true;

            try
            {
                GetSavedVersion(path);
            }
            catch(Exception e)
            {
                throw new SpreadsheetReadWriteException("issue reading file " + e.Message);
                
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }


        protected bool NameValidator(string name)
        {
            Regex rx = new Regex("^([A-Z]+|[a-z])+[0-9]+$");

            if (name is null || !(rx.IsMatch(name)) || !(IsValid(name)) || !(IsValid(Normalize(name))))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {

            if(!NameValidator(name))
            {
                throw new InvalidNameException();
            }

            if(cells.TryGetValue(name, out Cell value)) 
            {
                value.GetCellContents(out object ValueType);
                return ValueType;
            }
            else
            {
                //might need to delete
                Cell temp = new Cell("", LookupFunc);
                temp.GetCellContents(out object s);
                return s;
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (!NameValidator(name))
            {
                throw new InvalidNameException();
            }

            cells.TryGetValue(name, out Cell value);

            return value.getCellValue();

        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            HashSet<String> populatedCells = new HashSet<string>();
            if (cells.Count > 0)
            {
                foreach (String keys in cells.Keys)
                {
                    populatedCells.Add(keys);
                }
                return populatedCells;
            }
            //call on empty spreadsheet prevents exception
            return populatedCells;
        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    String saveVersion = "";
                    String nameElement = "";
                    String contentElement = "";
                    //check
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    saveVersion = reader.GetAttribute("version");
                                    break;

                                case "name":
                                    reader.Read();
                                    nameElement = reader.Value;
                                    break;

                                case "contents":

                                    if (reader.IsEmptyElement)
                                    {
                                        throw new SpreadsheetReadWriteException("invalid content");
                                    }

                                    reader.Read();

                                    contentElement = reader.Value;
                                    break;

                                default:
                                    break;

                            }
                            if (!(saveVersion.Equals(versionName)))
                            {
                                throw new SpreadsheetReadWriteException("incorrect version");
                            }


                            if (!(nameElement == "") && !(contentElement == ""))
                            {
                                if (!NameValidator(nameElement))
                                {
                                    throw new SpreadsheetReadWriteException("Name is invalid");
                                }

                                try
                                {
                                    SetContentsOfCell(nameElement, contentElement);
                                    nameElement = "";
                                    contentElement = "";
                                }
                                catch
                                {
                                    throw new SpreadsheetReadWriteException("Invalid Formula or Circular Depenency");
                                }

                            }
                        }
                    }
                    return saveVersion;
                }


        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            //TODO:
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try
            {

                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    // first versioning
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", versionName);
                    //while loop that goes through every nonempty cell
                    foreach (string cellName in cells.Keys)
                    {
                        //cell label
                        writer.WriteStartElement("cell");
                        //cell name
                        writer.WriteElementString("name", cellName);

                        cells.TryGetValue(cellName, out Cell value);
                        value.GetCellContents(out object s);
                        String content = s.ToString();

                        //contents
                        writer.WriteElementString("contents", content);

                        //end of cell tag
                        writer.WriteEndElement();
                    }

                    //end versioning tag
                    writer.WriteEndElement();


                    writer.WriteEndDocument();
                }
                Changed = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Invalid FilePath");
            }
        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// The contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {

            if (cells.TryGetValue(name, out Cell value))
            {
                value.GetCellContents(out object x);
                if (x is Formula)
                {
                    Formula f1 = (Formula)x;
                    List<String> oldDependees = f1.GetEquationTokens();
                    foreach(string s in oldDependees)
                    {
                        dg.RemoveDependency(s, name);
                    }
                }
                value.SetCellContent(number);
                
            }
            else
            {
                 //case where new cell hasn't been added to the dictionary
                 Cell c1 = new Cell(number, LookupFunc);
                 cells.Add(name, c1);
            }
            
            
            List<String> dgList = new List<string>(GetCellsToRecalculate(name));

            return dgList;

        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// The contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {

            // exception checked in setcontentsofcell


            if(text == "")
            {
                return new List<String> (dg.GetDependees(name));
            }

            if (cells.TryGetValue(name, out Cell value))
            {
                value.SetCellContent(text);
            }
            else
            {
                //case where new cell hasn't been added to the dictionary
                Cell c1 = new Cell(text, LookupFunc);
                cells.Add(name, c1);
            }

            List<String> dgList = new List<string>(GetCellsToRecalculate(name));

            return dgList;
        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// If changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula. The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // exception checked in setcontentsofcell

            //surround try catch
            //circular dependency check here
            List<string> oldDependees = new List<string>(dg.GetDependees(name));
            List<String> retList;
            try
            {
                HashSet<String> variables = new HashSet<string>();
                variables = new HashSet<string>(formula.GetVariables());
                dg.ReplaceDependees(name, variables);
                retList = new List<String>(GetCellsToRecalculate(name));
            }
            
            catch
            {
                // revert back to before if failed.
                dg.ReplaceDependees(name, oldDependees);
                throw new CircularException();
            }

            if (cells.TryGetValue(name, out Cell value))
            {
                value.SetCellContent(formula);

            }
            else
            {
                //case where new cell hasn't been added to the dictionary
                Cell c1 = new Cell(formula, LookupFunc);
                cells.Add(name, c1);
            }


            return retList;

        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {

            if (!NameValidator(name))
            {
                throw new InvalidNameException();
            }

            if (content is null)
            {
                throw new ArgumentNullException();
            }


            Changed = true;
            if (Double.TryParse(content, out double val))
            {
                return SetCellContents(name, val);
            }
            else if (content.StartsWith("="))
            {
              
                    // evaluate does not expect leading = token, must remove once
                    // validating its a string
                    String s = content.TrimStart('=');
                    Formula f1 = new Formula(s, Normalize, IsValid);

                    return SetCellContents(name, f1);
                
            }
            else
            {
                return SetCellContents(name, content);
            }
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if(dg.GetDependents(name) != null)
            {
               return new HashSet<String>(dg.GetDependents(name));
            }
            return new HashSet<String>();
        }

        public double LookupFunc(string name)
        {
            // go to cell variable and parse the value
            try
            {
                if (cells.TryGetValue(name, out Cell value) && value.getCellValue() is Double)
                {
                    return (double)value.getCellValue();
                }
            }
            catch
            {

                throw new ArgumentException("invalid Variable");

            }
            return 0.0;


        }

    }




    public class Cell
    {
        private String inputString;
        private Formula cellFormula;
        private double val;
        private Func<String, double> cellLookup;
        private bool isString;
        private bool isFormula;
        private bool isDouble;
        public Cell(object val, Func<String, double> lookup)
        {
            cellLookup = lookup;
            SetCellContent(val);
        }
        /// <summary>
        /// This method retrieves the data stored in the cell.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool GetCellContents(out object x)
        {
            x = null;
            if(inputString == null)
            {
                return false;
            }
            if (isString)
            {
                x = inputString;
            }
            else if (isFormula)
            {
                x = cellFormula;
            }
            else if(isDouble)
            {
                x = Double.Parse(inputString);
            }

            return true;
        }
        /// <summary>
        /// This method sets data into the cell
        /// </summary>
        /// <param name="input"></param>
        public void SetCellContent(object input)
        {
            inputString = "";
            cellFormula = null;
            //these are to help with the getcellcontent ensuring the correct object is returned
            isDouble = false;
            isString = false;
            isFormula = false;

            if (input is Formula)
            {
                inputString = input.ToString();
                cellFormula = (Formula)input;
                isFormula = true;
                setCellVal();

            }
            else if (input is String)
            {
                inputString = (String)input;
                isString = true;
            }
            else if (input is Double)
            {
                inputString = input.ToString();
                isDouble = true;
                setCellVal();

            }
        }
        //set cell value
        //helper method that automatically generates cell value
        private void setCellVal()
        {
            if (isFormula)
            {
                val = (double)cellFormula.Evaluate(cellLookup);
            }
            if (isDouble)
            {
                val = Double.Parse(inputString);
            }

        }

        //get cell value
        //retrieves cell value if cell contains formula, formula evaluates to double
        //if cell contains double, double is returned, if cell is string, string is returned
        public object getCellValue()
        {
            if (isString)
            {
                return inputString;
            }
            else
            {
                
                return val;
            }

        }
    }

}
