using FEnet_TCP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_FEnet
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\MappingListFile.csv"; // FilePath

            FEnet fenet = new FEnet();

            fenet.setTagInfo(path);

            var lstTagInfo = fenet.lstTagInfo;

            Dictionary<string, Dictionary<string, object>> readAllData;

            // 연결
            if (fenet.connect())
            {
                // 전체 스캔 (스캔 여부 허용된 태그만)
                var readAllDataresult = fenet.readAllData();
                if (readAllDataresult.Item1)
                {
                    readAllData = readAllDataresult.Item2; // 데이터그룹 < 태그 명 , 데이터 >
                    
                    // To-Do
                    // ..

                }

                // 읽기 요청
                Dictionary<string, Dictionary<string, object>> readsDic = new(); // 데이터를 읽기 위한 객체
                var groupName = lstTagInfo.First().equipmentCode;
                readsDic[groupName] = new();

                var onceGroup = lstTagInfo.Where(x=> x.equipmentCode == groupName).ToList();
                foreach (var groupItem in onceGroup)
                {
                    readsDic[groupName][groupItem.name] = null;
                }

                var readsResult = fenet.reads(readsDic);
                if (readsResult)
                {
                    // dicData
                    // To-Do
                    // ..
                }

                // 쓰기 요청
                Dictionary<string, Dictionary<string, object>> writesDic = new(); // 변경하기 위한 객체
                writesDic[groupName] = new();

                int index = 0;
                foreach (var groupItem in onceGroup)
                {
                    writesDic[groupName][groupItem.name] = index++;
                }

                var writesResult = fenet.writes(writesDic);
                if (writesResult)
                {
                    Console.WriteLine($"Write --> result = {writesResult}");

                    // To-Do
                    // ..

                }

                // 연결 종료
                fenet.disConnect();
            }
            

            while (!Console.KeyAvailable)
            {
            }
        }
    }
}
