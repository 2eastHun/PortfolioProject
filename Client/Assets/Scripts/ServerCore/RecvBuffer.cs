using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        //*[r][][][]*/ [w][][][][] == 4 DataSize
        public int DataSize { get { return _writePos - _readPos; } } // 데이터가 얼마나 쌓여있는지 확인

        //[r][][][] /*[w][][][][]*/ == 5 FreeSize
        public int FreeSize { get { return _buffer.Count -_writePos; } } //남아있는 데이터 공간

        //*[r][][][]*/
        public ArraySegment<byte> ReadSegment //현재까지 받은 데이터의 유효 범위를 나타내는 함수
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); } 
        }

        //*[w][][][][]*/
        public ArraySegment<byte> WriteSegment //데이터를 받을 때 유효 범위를 확인하는 함수
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        // 버퍼를 그냥 놔두면 버퍼가 고갈되니 초기화하는 작업
        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0) //클라에서 보내준 모든 데이터를 처리한 상태 //[][][][][rw][][][][]
            {
                //남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋 //[rw][][][][][][][][]
                _readPos = _writePos = 0;
            }
            else //[][]/*[r][]*/[w][][][][]       ->
            {
                //남은 찌끄레기가 있으면 시작 위치로 복사 //[r][][w][][][][][][]
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfByte) // // 클라에서 데이터를 보낸 후 서버가 받았을 때 리드 커서를 데이터 크기만큼 이동
        {
            if (numOfByte > DataSize)
                return false;

            _readPos += numOfByte;

            return true;
        }

        public bool OnWrite(int numOfByte) // 클라에서 데이터를 보낸 후 서버가 받았을 때 라이트 커서를 데이터 크기만큼 이동
        {
            if (numOfByte > FreeSize)
                return false;

            _writePos += numOfByte;
            return true;
        }
    }
}
