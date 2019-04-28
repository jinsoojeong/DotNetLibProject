using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace NetLibrary.SimpleNet
{
    // 재사용 가능한 SocketAsyncEventArgs 객체의 컬렉션을 나타낸다.
	class SocketAsyncEventArgsPool
	{
		Stack<SocketAsyncEventArgs> m_pool;

        // 객체 풀의 사이즈를 초기화
        // SocketAsyncEventArgs 객체의 최대 보유 개수를 지정
		public SocketAsyncEventArgsPool(int capacity)
		{
			m_pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

        // 새로운 SocketAsyncEventArgs 객체를 추가
		public void Push(SocketAsyncEventArgs item)
		{
			if (item == null)
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");

			lock (m_pool)
			{
				m_pool.Push(item);
			}
		}

        // SocketAsynEventArgs 객체를 풀에서 제거, 리턴
		public SocketAsyncEventArgs Pop()
		{
			lock (m_pool)
			{
				return m_pool.Pop();
			}
		}

		// 풀에 보유한 객체의 수
		public int Count
		{
			get { return m_pool.Count; }
		}
	}
}
