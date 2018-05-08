namespace GameReward
{
    public class GrdCustomResult : GrdResult<object>
    {
        public GrdCustomResult(int error, string message, object json) :
            base(error, message, json)
        {
        }
        public T GetObject<T>() where T : class
        {
            return Json.GetObject<T>(data);
        }
    }
}