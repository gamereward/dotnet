namespace GameReward
{
    public class GrdResultBase
    {
        public GrdResultBase()
        {

        }
        public GrdResultBase(int error, string message)
        {
            this.error = error;
            this.message = message;
        }
        public int error;
        public string message;
    }
}