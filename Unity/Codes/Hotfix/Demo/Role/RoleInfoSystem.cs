namespace ET
{
    public static class RoleInfoSystem
    {
        public static void FromMessage(this RoleInfo self,RoleInfoProto roleInfoProto)
        {
            self.Id = roleInfoProto.Id;
            self.Name = roleInfoProto.Name;
            self.AccountId = roleInfoProto.AccountId;
            self.ServerId = roleInfoProto.ServerId;
            self.LastLoginTime = roleInfoProto.LastLoginTime;
            self.CreateTime = roleInfoProto.CreateTime;
            self.State = roleInfoProto.State;
        }

        public static RoleInfoProto ToMessage(this RoleInfo self)
        {
            return new RoleInfoProto
            {
                Id = self.Id,
                Name = self.Name,
                AccountId = self.AccountId,
                ServerId = self.ServerId,
                LastLoginTime = self.LastLoginTime,
                CreateTime = self.CreateTime,
                State = self.State
            };
        }
    }
}
