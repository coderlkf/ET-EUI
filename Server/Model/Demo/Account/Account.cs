namespace ET
{
    /// <summary>
    /// 账户类型
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// 普通账户
        /// </summary>
        General = 0,
        /// <summary>
        /// 禁止登陆的账户
        /// </summary>
        BlackList = 1,
    }
    /// <summary>
    /// 账户实体
    /// </summary>
    public class Account : Entity, IAwake
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName;
        /// <summary>
        /// 账户密码
        /// </summary>
        public string Password;
        /// <summary>
        /// 创建时间（时间戳）
        /// </summary>
        public long CreateTime;
        /// <summary>
        /// 账户类型
        /// </summary>
        public int AccountType;
    }
}
