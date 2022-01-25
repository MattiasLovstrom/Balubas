using System;
using System.Linq;

namespace Balubas
{
    public class Synchronizer : ISynchronizer
    {
        private readonly IRepository _storage1;
        private readonly IRepository _storage2;

        public Synchronizer(IRepository storage1, IRepository storage2)
        {
            _storage1 = storage1;
            _storage2 = storage2;
        }

        public void Synchronize()
        {
            Console.Out.Write("Synchronizing: ");

            Synchronize(_storage1, _storage2);
            Synchronize(_storage2, _storage1);

            Console.Out.WriteLine();
        }

        private void Synchronize(IRepository from, IRepository to)
        {
            if (from == null || !from.Any()) return;

            if (to.Get(from.First().Hash) != null) return;

            var currentToHash = to.FirstOrDefault()?.Hash;
            var toSynchronize = from.TakeWhile(currentFrom => currentFrom.Hash != currentToHash).Reverse();
            foreach (var transaction in toSynchronize)
            {
                to.Add(transaction);
            }
        }
    }
}