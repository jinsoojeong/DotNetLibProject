using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace NetLibrary.SimpleNet
{
    // 이 클래스는 개별 소켓 I/O 작업에 사용할 수 있도록 SocketAsyncEventArgs 객체로 나누어 할당할 수 있는 단일 대형 버퍼를 만듭니다.
    // 이렇게 하면 버퍼를 쉽게 재사용하고 힙 메모리 조각화를 방지할 수 있다.

    // BufferManager 클래스에서 수행되는 작업은 스레드 세이프 하지 않다.
    internal class BufferManager
    {

        int m_numBytes;                 // 버퍼 풀의 총 사이즈
        byte[] m_buffer;                // 버퍼 매니저가 관리하게될 바이트 배열
        Stack<int> m_freeIndexPool; 
        int m_currentIndex;
        int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        // 버퍼 풀에서 사용될 버퍼 공간을 할당
        public void InitBuffer()
        {
            // 하나의 큰 버퍼를 만들고 이를 각 SocketAsyncEventArg 객체로 나눈다.
            m_buffer = new byte[m_numBytes];
        }
        
        // 버퍼 풀에서 지정된 SocketAsyncEventArgs 객체로 버퍼를 할당
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (m_freeIndexPool.Count > 0)
            {
                // 유휴 index id 를 재활용
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                // 유휴 index 가 없다면 새로 할당, offset
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                    return false;
                
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize; // 다음 버퍼 사이즈 시작지점을 index로 할당
            }
            return true;
        }

        // SocketAsynEventArg 객체로 부터 버퍼를 제거, 제거된 버퍼를 유휴 index 풀에 저장해서 재활용됨
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
