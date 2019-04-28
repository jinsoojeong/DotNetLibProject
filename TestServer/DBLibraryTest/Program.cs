using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleSqlClient;

namespace DBLibraryTest
{
    class Program
    {
        // 레코드 형식의 반환을 위한 QueryRecord 클래스를 상속 받고 
        // 레코드에 입력될 데이터 컬럼을 등록한다.
        class QuestListRecord : QueryRecord
        {
            public QuestListRecord()
            {
                base.AddColumn("QUEST_IDX");
                base.AddColumn("COUNT_SUM");
                base.AddColumn("STEP");
                base.AddColumn("EXPIRATION_TIME");
            }

            // 사용예
            public void View()
            {
                int i = 0;
                foreach (var row in rows_)
                {
                    ++i;

                    Console.WriteLine("---------- {0} row -----------", i);
                    Console.WriteLine("QUEST_IDX : {0}", row.Get("QUEST_IDX") == null ? 0 : Convert.ToInt32(row.Get("QUEST_IDX")));
                    Console.WriteLine("COUNT_SUM : {0}", row.Get("COUNT_SUM") == null ? 0 : Convert.ToInt32(row.Get("COUNT_SUM")));
                    Console.WriteLine("STEP : {0}", row.Get("STEP") == null ? 0 : Convert.ToInt32(row.Get("STEP")));
                    Console.WriteLine("EXPIRATION_TIME : {0}", row.Get("EXPIRATION_TIME") == null ? new DateTime(0) : Convert.ToDateTime(row.Get("EXPIRATION_TIME")));
                }
            }
        }

        static void Main(string[] args)
        {
            //DB 실행 예제
            // db 주소, 계정, 비밀번호, db 카테고리
            CSql.Initialize("192.168.10.19,1433", "sa", "NSDev1234^^", "BS_JJS_GAME");

            QueryParam user_id = new QueryParam(ParamType.INPUT, "@USER_ID", 10086);
            QueryParam output = new QueryParam(ParamType.OUTPUT, "@OUTPUT", 0);
            
            // 레코드 반환이 없는 프로시저 실행
            int result = CSql.ExecSP("USP_BS_GET_LIST_QUEST", user_id, output);
            Console.WriteLine("result : {0}", result);

            // 레코드 반환이 있는 프로시저 실행
            QuestListRecord record = new QuestListRecord();
            result = CSql.ExecSP("USP_BS_GET_LIST_QUEST", record, user_id, output);
            Console.WriteLine("result : {0} {1}", result, record.message_);

            record.View();

            return;
        }
    }
}
