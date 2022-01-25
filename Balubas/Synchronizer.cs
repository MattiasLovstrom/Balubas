using System;
using System.Collections.Generic;
using System.Linq;

namespace Balubas
{
    public class Synchronizer : ISynchronizer
    {
        private readonly IEnumerable<IRepository> _repositories;

        public Synchronizer(IEnumerable<IRepository> repositories)
        {
            _repositories = repositories;
        }

        public void Synchronize()
        {
            Console.Out.Write("Synchronizing: ");
            var allInSync = false;

            while (!allInSync)
            {
                allInSync = true;
                foreach (var repository1 in _repositories)
                {
                    foreach (var repository2 in _repositories)
                    {
                        if (repository1 == repository2) continue;
                        allInSync = allInSync & Synchronize(repository1, repository2);
                    }
                }
            }

            Console.Out.WriteLine();
        }

        private bool Synchronize(IRepository from, IRepository to)
        {
            var allInSync = true;

            if (from == null || !from.Any()) return allInSync;

            if (to.Get(from.First().Hash) != null) return allInSync;

            var currentToHash = to.FirstOrDefault()?.Hash;
            var toSynchronize = from.TakeWhile(currentFrom => currentFrom.Hash != currentToHash).Reverse();
            foreach (var transaction in toSynchronize)
            {
                to.Add(transaction);
                allInSync = false;
            }

            return allInSync;
        }
    }
}