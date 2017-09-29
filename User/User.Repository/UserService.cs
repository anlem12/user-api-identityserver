using User.IRepository;
using System;
using User.Model;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;
using User.DBUtility;

namespace User.Repository
{
    public class UserService : IUserService
    {
        public async Task<long> Login(string user, string pwd)
        {
            try
            {
                string strSql = "SELECT usersid FROM users where loginname_mobile=@u and password=@p";
                MySqlParameter[] para =
                 {
                new MySqlParameter("@u",MySqlDbType.VarChar,30),
                new MySqlParameter("@p",MySqlDbType.VarChar,50)
            };
                para[0].Value = user;
                para[1].Value = pwd;
                object obj = await DbHelperMySQL.GetSingleAsync(DBConnection.UsersSystem, strSql, para);
                if (obj == null)
                {
                    return 0;
                }
                return Convert.ToInt64(obj);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<bool> CheckMobileIsExist(string mobile)
        {
            try
            {
                string strSql = "select count(0) from users where LoginName_Mobile=@u";
                MySqlParameter[] para =
                {
                    new MySqlParameter("@u",MySqlDbType.VarChar,15)
                };
                para[0].Value = mobile;
                object obj = await DbHelperMySQL.GetSingleAsync(DBConnection.UsersSystem, strSql, para);
                if (obj == null) return false;
                if (Convert.ToInt32(obj) > 0) return true;
                return false;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<bool> ChangeHeaderPic(long userid, string headerImage)
        {
            try
            {
                string strSql = "update users set headerpic=@img where usersid=@u";
                MySqlParameter[] para =
                {
                    new MySqlParameter("@u",MySqlDbType.Int64),
                    new MySqlParameter("@img",MySqlDbType.VarChar,150)
                };
                para[0].Value = userid;
                para[1].Value = headerImage;
                int irow = await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql, para);
                if (irow > 0) return true;
                return false;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<bool> ChangeNickName(long userid, string nick_name)
        {
            try
            {
                string strSql = "update users set nickname=@n where usersid=@u";
                MySqlParameter[] para =
                {
                    new MySqlParameter("@u",MySqlDbType.Int64),
                    new MySqlParameter("@n",MySqlDbType.VarChar,30)
                };
                para[0].Value = userid;
                para[1].Value = nick_name;
                int irow = await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql, para);
                if (irow > 0) return true;
                return false;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<bool> ChangePwd(long userid, string new_pwd)
        {
            try
            {
                string strSql = "update users set password=@p where usersid=@u";
                MySqlParameter[] para =
                {
                    new MySqlParameter("@u",MySqlDbType.Int64),
                    new MySqlParameter("@p",MySqlDbType.VarChar,45)
                };
                para[0].Value = userid;
                para[1].Value = new_pwd;
                int irow = await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql, para);
                if (irow > 0) return true;
                return false;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<bool> ChangePwdByMoible(string mobile, string pwd)
        {
            try
            {
                string strSql = "update users set password=@p where LoginName_Mobile=@u";
                MySqlParameter[] para =
                {
                    new MySqlParameter("@u",MySqlDbType.VarChar,15),
                    new MySqlParameter("@p",MySqlDbType.VarChar,45)
                };
                para[0].Value = mobile;
                para[1].Value = pwd;
                int irow = await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql, para);
                if (irow > 0) return true;
                return false;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<bool> ValidateOldPwd(long userid, string pwd)
        {
            try
            {
                string strSql = "select count(0) from users where usersid=@u and password=@p";
                MySqlParameter[] para =
                {
                    new MySqlParameter("@u",MySqlDbType.Int64),
                    new MySqlParameter("@p",MySqlDbType.VarChar,45)
                };
                para[0].Value = userid;
                para[1].Value = pwd;
                object obj = await DbHelperMySQL.GetSingleAsync(DBConnection.UsersSystem, strSql, para);
                if (obj == null) return false;
                if (Convert.ToInt32(obj) > 0) return true;
                return false;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<bool> RegisterUser(UserRegisterModel model)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into Users(");
                strSql.Append("UsersID,LoginName_Mobile,PassWord,UserKey,RegIP,nickname,headerpic,CreateDate)");
                strSql.Append(" values (");
                strSql.Append("@UsersID,@LoginName_Mobile,@PassWord,@UserKey,@RegIP,@nickname,@headerpic,@CreateDate)");
                MySqlParameter[] parameters = {
                    new MySqlParameter("@UsersID", MySqlDbType.Int64),
                    new MySqlParameter("@LoginName_Mobile", MySqlDbType.VarChar,12),
                    new MySqlParameter("@PassWord", MySqlDbType.VarChar,50),
                    new MySqlParameter("@UserKey", MySqlDbType.VarChar,50),
                    new MySqlParameter("@RegIP", MySqlDbType.VarChar,50),
                    new MySqlParameter("@nickname", MySqlDbType.VarChar,30),
                    new MySqlParameter("@headerpic", MySqlDbType.VarChar,200),
                    new MySqlParameter("@CreateDate", MySqlDbType.DateTime)};
                parameters[0].Value = model.UsersID;
                parameters[1].Value = model.LoginName_Mobile;
                parameters[2].Value = model.PassWord;
                parameters[3].Value = model.UserKey;
                parameters[4].Value = model.RegIP;
                parameters[5].Value = model.nickname;
                parameters[6].Value = model.headerpic;
                parameters[7].Value = model.CreateDate;
                return await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql.ToString(), parameters)>0?true:false;
            }
            catch (Exception err)
            {

                throw err;
            }

        }

        public async Task<long> GetUserIdRandom()
        {
            DbDataReader dr = null;
            try
            {
                long userID = 0;
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from UsersRandom where IsActive=0 order by rand() limit 0,1");
                using (dr = await DbHelperMySQL.ExecuteReaderAsync(DBConnection.UsersSystem, strSql.ToString(), null))
                {
                    if (await dr.ReadAsync())
                    {
                        userID = dr.GetInt64(0);
                    }
                }
                return userID;
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Dispose();
                }
            }
        }

        public async Task<bool> ChangeUserIdRandom(long userid)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update UsersRandom set ");
                strSql.Append("IsActive=@IsActive");
                strSql.Append(" where UsersRandomID=@UsersRandomID ");
                MySqlParameter[] parameters = {
                    new MySqlParameter("@IsActive", MySqlDbType.Bit),
                    new MySqlParameter("@UsersRandomID", MySqlDbType.Int64)};
                parameters[0].Value = true;
                parameters[1].Value = userid;

                return await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql.ToString(), parameters) > 0 ? true : false;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<UserShowModel> GetUserModel(long userid)
        {
            DbDataReader dr = null;
            UserShowModel entity = null;
            try
            {
                string strSql = "select nickname,headerpic from users where usersid=@userid";
                MySqlParameter[] para =
                {
                    new MySqlParameter("@userid",MySqlDbType.Int64)
                };
                para[0].Value = userid;
                using (dr = await DbHelperMySQL.ExecuteReaderAsync(DBConnection.UsersSystem, strSql, para))
                {
                    while (await dr.ReadAsync())
                    {
                        entity = new UserShowModel();
                        entity.nickname = dr[0].ToString();
                        entity.headportrait = dr[1].ToString();
                    }
                }
                return entity;
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                if (dr != null || dr.IsClosed)
                {
                    dr.Dispose();
                }
            }
        }


    }
}
