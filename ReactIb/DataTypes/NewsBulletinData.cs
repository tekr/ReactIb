using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class NewsBulletinData
    {
        public int Id { get; }

        public NewsType MsgType { get; }

        public string Message { get; }

        public string Exchange { get; }

        public NewsBulletinData(int id, NewsType msgType, string message, string exchange)
        {
            Id = id;
            MsgType = msgType;
            Message = message;
            Exchange = exchange;
        }
    }
}