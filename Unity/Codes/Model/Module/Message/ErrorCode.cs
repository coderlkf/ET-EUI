namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误

        // 110000以下的错误请看ErrorCore.cs

        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        public const int ERR_NetWorkError = 200002; // 网络错误

        public const int Err_LoginInfoIsNull = 200003; // 登录信息错误

        public const int Err_AccountNameFormError = 200004; // 账号格式错误

        public const int Err_PasswordFormError = 200005; // 密码格式错误

        public const int Err_LoginPasswordError = 200006; // 密码错误

        public const int Err_AccountInBlackListError = 200007; // 账号被封禁

        public const int Err_RequestRepeatedly = 200008; // 请求重复
    }
}