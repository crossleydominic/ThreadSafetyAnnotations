using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;
using Shared.Utilities;

namespace ThreadSafetyAnnotations.Engine.Info
{
    /// <summary>
    /// Represents an ordered hierarchy of locks
    /// </summary>
    public class LockHierarchy : IEnumerable<string>
    {
        #region Private members

        /// <summary>
        /// A set of locks in a hierarchy. Ordered in first-to-last taken order;
        /// </summary>
        private List<string> _firstToLastLockList;

        #endregion

        #region Constructors

        /// <summary>
        /// Only allow creation through static helper methods, not through the constructor directly
        /// </summary>
        private LockHierarchy()
        {
            _firstToLastLockList = new List<string>();
        }

        #endregion

        #region Public methods

        public IEnumerator<string> GetEnumerator()
        {
            return _firstToLastLockList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get { return _firstToLastLockList.Count; }
        }

        public string this[int i] 
        {
            get { return _firstToLastLockList[i]; }
        }

        #endregion

        #region Static helper methods

        /// <summary>
        /// Construct a LockHierarchy from a list of lock names
        /// </summary>
        public static LockHierarchy FromStringList(IEnumerable<string> locks)
        {
            #region Input Validation

            Insist.IsNotNull(locks, "locks");

            #endregion

            LockHierarchy hierarchy = new LockHierarchy();
            hierarchy._firstToLastLockList.AddRange(locks);

            return hierarchy;
        }

        /// <summary>
        /// Compares two lock hierachies to determine if one of them satisfies the requirement 
        /// of the other
        /// </summary>
        public static bool IsSatisfiedBy(LockHierarchy requiredHierarchy, LockHierarchy actualHierarchy)
        {
            int currentIndex = 0;
            foreach (string currentLock in requiredHierarchy)
            {
                bool lockExists = false;

                for (int i = currentIndex; i < actualHierarchy.Count; i++)
                {
                    if (currentLock == actualHierarchy[i])
                    {
                        lockExists = true;
                        currentIndex = i + 1;
                        break;
                    }
                }

                if (!lockExists)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Conflicts(LockHierarchy h1, LockHierarchy h2)
        {

            List<string> firstList = h1._firstToLastLockList;
            List<string> secondList = h2._firstToLastLockList;

            List<string> longer, shorter;

            if (firstList.Count >= secondList.Count)
            {
                longer = firstList;
                shorter = secondList;
            }
            else
            {
                longer = secondList;
                shorter = firstList;
            }

            for (int i = 0; i < shorter.Count; i++)
            {
                string currentShorter = shorter[i];

                int index = longer.IndexOf(currentShorter);
                if (index == -1)
                {
                    continue;
                }

                if (index < i)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
