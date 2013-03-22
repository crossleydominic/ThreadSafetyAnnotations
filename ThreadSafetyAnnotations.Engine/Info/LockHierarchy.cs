using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;

namespace ThreadSafetyAnnotations.Engine.Info
{
    public class LockHierarchy : IEnumerable<string>
    {
        private List<string> _generalToSpecificLockList;

        private LockHierarchy()
        {
            _generalToSpecificLockList = new List<string>();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _generalToSpecificLockList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get { return _generalToSpecificLockList.Count; }
        }

        public string this[int i] 
        {
            get { return _generalToSpecificLockList[i]; }
        }

        public static LockHierarchy FromStringList(List<string> locks)
        {
            LockHierarchy hierarchy = new LockHierarchy();
            hierarchy._generalToSpecificLockList.AddRange(locks);

            return hierarchy;
        }

        public static LockHierarchy FromIdentifierName(IdentifierNameSyntax identifier)
        {
            LockHierarchy hierarchy = new LockHierarchy();

            //Go up the syntax tree looking at locks and record them in 
            //SPECIFIC TO GENERAL order
            List<string> specificToGeneralLockList = new List<string>();
            SyntaxNode currentNode = identifier;
            do
            {
                currentNode = currentNode.Ancestors().OfType<LockStatementSyntax>().FirstOrDefault();

                if (currentNode != null)
                {
                    string lockName = currentNode.DescendantNodes()
                        .OfType<IdentifierNameSyntax>()
                        .First().Identifier.ValueText;

                    specificToGeneralLockList.Add(lockName);
                }

            } while (currentNode != null);

            specificToGeneralLockList.Reverse();

            hierarchy._generalToSpecificLockList.AddRange(specificToGeneralLockList);

            return hierarchy;
        }

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
    }
}
