// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<String, HashSet<String>> dependents = new Dictionary<String, HashSet<String>>();
        private Dictionary<String, HashSet<String>> dependees = new Dictionary<String, HashSet<String>>();
        private int SizeCount;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<String, HashSet<String>>();
            dependees = new Dictionary<String, HashSet<String>>();
            SizeCount = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return SizeCount; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {

            get
            {

                if (!dependees.TryGetValue(s, out HashSet<String> DependeeVals))
                {
                    return 0;
                }
                return DependeeVals.Count;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            dependents.TryGetValue(s, out HashSet<String> DependentList);

            if (ReferenceEquals(DependentList, null))
            {
                return false;
            }

            return DependentList.Count > 0;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            dependees.TryGetValue(s, out HashSet<String> DepeeList);

            if (ReferenceEquals(DepeeList, null))
            {
                return false;
            }

            return DepeeList.Count > 0;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!dependents.ContainsKey(s))
            {
                return new HashSet<String>();
            }
            dependents.TryGetValue(s, out HashSet<String> dependentVals);
            return dependentVals;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (!dependees.ContainsKey(s))
            {
                return new HashSet<String>();
            }
            // dependees.TryGetValue(s, out HashSet<String> values);
            return dependees[s]; //values;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            HashSet<String> list = new HashSet<String>();
            if (dependents.TryGetValue(s, out HashSet<String> DependentsList))
            {
                list = DependentsList;
                if (!list.Contains(t))
                {
                    dependents[s].Add(t);
                    SizeCount++;
                }
            }
            else
            {
                list.Add(t);
                dependents.Add(s, list);
                SizeCount++;
            }

            if (dependees.TryGetValue(t, out HashSet<String> DependeesList))
            {
                list = DependeesList;
                if (!list.Contains(s))
                {
                    dependees[t].Add(s);
                }
            }
            else
            {
                list = new HashSet<string>();
                list.Add(s);
                dependees.Add(t, list);
            }

        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependents.TryGetValue(s, out HashSet<String> ValueList))
            {


                if (ValueList.Remove(t))
                {
                    dependents[s].Remove(t);
                    SizeCount--;
                }
                dependees.TryGetValue(t, out HashSet<String> DependeeList);
                if (DependeeList.Remove(s))
                {
                    dependees[t].Remove(s);
                }

            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //in this method size count is relying on dependees.
            if (dependents.TryGetValue(s, out HashSet<String> dependentValues))
            {
                //cycle through dependent values and remove from dependees list
                foreach (String values in dependentValues)
                {
                    if (dependees[values].Contains(s))
                    {
                        dependees[values].Remove(s);
                        SizeCount--;
                    }
                }
                dependents[s].Clear();

                foreach (String values in newDependents)
                {
                    if (!dependents[s].Contains(values))
                    {
                        dependents[s].Add(values);
                        SizeCount++;
                    }
                    //case where dependee does not exist
                    if (dependees.TryGetValue(values, out HashSet<String> test))
                    {
                        dependees[values].Add(s);
                    }
                    else
                    {
                        HashSet<String> newSet = new HashSet<String>();
                        newSet.Add(s);
                        dependees.Add(values, newSet);
                    }
                }
            }
            // case where there is no s currently
            else
            {
                //case where the key is not present
                  dependents.Add(s, new HashSet<string>());
            }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //checks if dependees has valueset at key s
            if (dependees.TryGetValue(s, out HashSet<String> DependeesList))
            {
                //loop removes the depees from the dependents, makes sure both lists are synced
                foreach (String Dependee in DependeesList)
                {
                    if (dependents[Dependee].Contains(s))
                    {
                        dependents[Dependee].Remove(s);
                        SizeCount--;
                    }

                }
                // clears all depees at s
                dependees[s].Clear();
                // manually adds each dp in the list makes sure there are no duplicates
                foreach (String newDepee in newDependees)
                {
                    if (!dependees[s].Contains(newDepee))
                    {
                        dependees[s].Add(newDepee);
                        SizeCount++;
                    }
                    if (dependents.TryGetValue(newDepee, out HashSet<String> test))
                    {
                        dependents[newDepee].Add(s);

                    }
                    else
                    {
                        HashSet<String> newSet = new HashSet<String>();
                        newSet.Add(s);
                        dependents.Add(newDepee, newSet);
                    }
                }

            }
            //case where dependee doesnt exist
            else
            {
                // entire list will be added to count
                SizeCount = SizeCount + newDependees.Count<String>();
                dependees.Add(s, (HashSet<string>)newDependees);

            }
            // cycle through dependee list to add to the dependent list
            foreach (String newDepee in newDependees)
            {
                //case where the key is present
                if (dependents.TryGetValue(newDepee, out HashSet<String> test))
                {
                    dependents[newDepee].Add(s);

                }
                else
                {
                    //case where the key is not present
                    HashSet<String> newSet = new HashSet<String>();
                    newSet.Add(s);
                    dependents.Add(newDepee, newSet);
                }
            }

        }

    }
}
