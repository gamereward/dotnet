namespace GameReward
{
    public class GrdResult<T> : GrdResultBase
    {
        public GrdResult()
        {
        }
        public GrdResult(int error, string message, T data) :
            base(error, message)
        {
            this.data = data;
        }
        public T data;
    }
}