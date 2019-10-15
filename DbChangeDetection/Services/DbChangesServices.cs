using DbChangeDetection.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbChangeDetection.Services
{
    public class DbChangesServices : IDbChangesServices
    {
        public DbChangesServices()
        {
        }

        public async Task<int> GetCurentDbChangeId()
        {
            string query = "SELECT CHANGE_TRACKING_CURRENT_VERSION()";
            int previousChangeId = 0;
            var str = Environment.GetEnvironmentVariable("SPT_DEV_CONNECTION");
            using (SqlConnection connection = new SqlConnection(str))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        // Check is the reader has any rows at all before starting to read.
                        if (reader.HasRows)
                        {
                            // Read advances to the next row.
                            await reader.ReadAsync();
                            previousChangeId = int.Parse(reader[0].ToString());

                        }
                    }
                }
            }

            return previousChangeId;
        }

        public async Task<string> GetChangeDiaryNetData(int PreviousChangeId)
        {
            var dataList = new List<Dictionary<ChangeTableType, object>>();

            var str = Environment.GetEnvironmentVariable("SPT_DEV_CONNECTION");
            using (SqlConnection connection = new SqlConnection(str))
            {
                await connection.OpenAsync();
                var queryList = GetQueryList(PreviousChangeId);

                foreach (var item in queryList)
                {
                    var queryTableResults = new List<Dictionary<string, object>>();

                    using (SqlCommand cmd = new SqlCommand(item.Item2, connection))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            // Check is the reader has any rows at all before starting to read.
                            if (reader.HasRows)
                            {
                                // Read advances to the next row.
                                while (await reader.ReadAsync())
                                {
                                    var fieldValues = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        fieldValues.Add(reader.GetName(i), reader[i]);
                                    }
                                    queryTableResults.Add(fieldValues);
                                }
                            }
                        }
                    }

                    var tableResult = new Dictionary<ChangeTableType, object>();
                    tableResult.Add(item.Item1, queryTableResults);

                    dataList.Add(tableResult);
                }
            }

            return JsonConvert.SerializeObject(dataList);
        }


        private List<Tuple<ChangeTableType, string>> GetQueryList(int PreviousChangeId)
        {
            List<Tuple<ChangeTableType, string>> list = new List<Tuple<ChangeTableType, string>>();

            string diaryNetQuery = "SELECT dbo.DPSDiaryNet.* FROM dbo.DPSDiaryNet INNER JOIN " +
            "CHANGETABLE(CHANGES DPSDiaryNet, " + PreviousChangeId + ") AS CT_DIR " +
            "ON dbo.DPSDiaryNet.U_ID = CT_DIR.U_ID";

            var diary = Tuple.Create(ChangeTableType.DPSDiaryNet, diaryNetQuery);
            list.Add(diary);

            string mattersQuery = "SELECT dbo.Matters.* FROM dbo.Matters INNER JOIN " +
            "CHANGETABLE(CHANGES Matters, " + PreviousChangeId + ") AS CT_DIR " +
            "ON dbo.Matters.MAT_Counter = CT_DIR.MAT_Counter";

            var matter = Tuple.Create(ChangeTableType.Matter, mattersQuery);
            list.Add(matter);


            return list;

        }




    }
}
