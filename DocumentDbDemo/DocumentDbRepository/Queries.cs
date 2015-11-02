
namespace DocumentDbDemo.DocumentDbRepository
{
    static class Queries
    {
        public static string GetQueryForDeleteByPersonIdArray(string[] personIdArray)
        {
            var whereInString = GetWhereInString(personIdArray);
            return $"select * from c where c.personId in ({whereInString})";
        }

        static string GetWhereInString(string[] items) => $"'{string.Join("','", items)}'";
    }
}
