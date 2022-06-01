namespace Blog2022_netcore.Entities
{
    //Demo和Icon多对多中间表
    public class DemoIcon
    {
        public Guid DemoId { get; set; }
        public Guid IconId { get; set; }
        public Demo Demo { get; set; }
        public Icon Icon { get; set; }
    }
}
