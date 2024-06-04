namespace CEC.SAISE.BLL.Dto
{
    public class ValueNamePair
    {
        public ValueNamePair()
        {

        }

        public ValueNamePair(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; set; }

        public string Name { get; set; }
    }
}