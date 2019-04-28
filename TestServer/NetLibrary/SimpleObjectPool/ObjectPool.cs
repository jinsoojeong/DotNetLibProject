using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLibrary.SimpleObjectPool
{
    public interface IObject
    {
        void Reset();
    }

    public class CObjectPool<T> where T : IObject, new()
    {
        private Queue<T> ObjectList = new Queue<T>();
        private int extend_pool_size = 50;

        public CObjectPool(int defualt_pool_size = 0)
        {
            if (defualt_pool_size == 0)
                defualt_pool_size = extend_pool_size;

            for (int i = 0 ; i < defualt_pool_size; ++i)
            {
                ObjectList.Enqueue(new T());
            }
        }

        private void ExtendPool()
        {
            for (int i = 0; i < extend_pool_size; ++i)
            {
                ObjectList.Enqueue(new T());
            }
        }

        public T Alloc()
        {
            lock (ObjectList)
            {
                if (ObjectList.Count() == 0)
                    ExtendPool();

                T instance = ObjectList.Dequeue();
                return instance;
            }
        }

        public void Free(T instance)
        {
            instance.Reset();

            lock (ObjectList)
            {
                ObjectList.Enqueue(instance);
            }
        }
    }
}
