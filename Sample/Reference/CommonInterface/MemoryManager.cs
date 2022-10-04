using System.Collections.Generic;
using System.Linq;

namespace CommonInterface
{
    public class MemoryManager
    {
        List<int> addr;
        List<int> length;

        public List<int> Addr
        {
            get
            {
                if (addr == null)
                {
                    return new List<int>();
                }
                return addr;
            }
        }
        public List<int> Length
        {
            get
            {
                if (length == null)
                {
                    return new List<int>();
                }
                return length;
            }
        }

        public void CreateMap(List<int> startAddr, List<int> addrSize, int maxLenght) // 마지막 cycle 에 대한 저장이 없음.
        {// 주소 2byte
            addr = new List<int>();
            length = new List<int>();

            int indexStartAddr = startAddr[0]; // address
            int index = 0; // length

            for (int i = 0; i < startAddr.Count; i++)
            {
                // 겹치는 구간 파악
                if (index + indexStartAddr > startAddr[i]) // 겹쳐있음
                {
                    int extraLength = (startAddr[i] + addrSize[i]) - (indexStartAddr + index); // 최대 주소의 크기 차이(길이)

                    if (extraLength < 0) // 구간 속에 포함되어있음
                        continue;

                    if (index + extraLength > maxLenght) // 최대 길이 초과
                    {
                        // 시작주소, 길이 저장
                        addr.Add(indexStartAddr);
                        length.Add(index);
                        // 시작주소, 길이 새로 할당
                        index = addrSize[i];
                        indexStartAddr = startAddr[i];
                        continue;
                    }

                    index += extraLength;
                }
                else
                {
                    if ((startAddr[i] + addrSize[i]) - indexStartAddr > maxLenght) // 최대 길이 초과 [ 추가할 주소(0에서 부터) - 현재 시작 주소
                    {
                        // 시작주소, 길이 저장
                        addr.Add(indexStartAddr);
                        length.Add(index);
                        // 시작주소, 길이 새로 할당
                        index = addrSize[i];
                        indexStartAddr = startAddr[i];
                        continue;
                    }

                    index = startAddr[i] - indexStartAddr + addrSize[i];
                }
            }

            if (addr.Count == 0) // 새로운 할당 없이 끝나는 경우
            {
                addr.Add(indexStartAddr);
                length.Add(index);
            }
            // 전체 cycle 이후 마지막 cycle에 대한 새로운 할당이 필요한지 확인
            else if (indexStartAddr != addr.Last()) // 마지막 cycle 이 새로운 할당일 경우
            {
                addr.Add(indexStartAddr);
                length.Add(index);
            }
        }
    }
}
