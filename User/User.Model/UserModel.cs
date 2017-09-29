using System;

namespace User.Model
{
    [Serializable]
    public class UserShowModel
    {
        public string nickname { get; set; }

        public string headportrait { get; set; }
    }

    [Serializable]
    public class UserRegisterModel
    {
        public long UsersID { get; set; }
        public string LoginName_Mobile { get; set; }
        public string PassWord { get; set; }
        public string UserKey { get; set; }
        public string RegIP { get; set; }
        public string nickname { get; set; }
        public string headerpic { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
