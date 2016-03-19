using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackCS.Core.Tools
{
    public class ThreadSafeList<T>
    {
        private List<T> _list = new List<T>();
        private object _sync = new object();
        public void Add(T value)
        {
            lock (_sync)
            {
                _list.Add(value);
            }
        }
        public void Remove(T obj)
        {
            lock (_sync)
            {
                _list.Remove(obj);
            }
        }
        public void Clear()
        {
            lock (_sync)
            {
                _list.Clear();
            }
        }
        public void ForEach(Action<T> method)
        {
            lock (_sync)
            {
                foreach (T obj in _list) method(obj);
            }
        }
        public T FirstOrDefault()
        {
            lock (_sync)
            {
                return _list.FirstOrDefault();
            }
        }
    }
}
