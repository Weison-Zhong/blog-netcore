namespace Blog2022_netcore.Model
{
    public class ApiResult
    {
        public int Code { get; set; } = 200;
        public string Msg { get; set; } = "操作成功";
        public dynamic Data { get; set; } = new { };
    }
}
