using IdentityServer4.Models;
using User.DBUtility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.IRepository;

namespace User.Repository
{
    public class PersistedGrantRepositoryService : IPersistedGrantRepositoryService
    {
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            DbDataReader dr = null;
            try
            {
                List<PersistedGrant> lst = new List<PersistedGrant>();
                string strSql = "select * from identityserver where subjectId=@subjectId";
                MySqlParameter[] parameters = {
                     new MySqlParameter("@subjectId", MySqlDbType.VarChar,1000)};
                parameters[0].Value = subjectId;

                using (dr = await DbHelperMySQL.ExecuteReaderAsync(DBConnection.UsersSystem, strSql, parameters))
                {
                    while (await dr.ReadAsync())
                    {
                        lst.Add(DataRowToModel(dr));
                    }
                }
                return lst;
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

        public async Task<PersistedGrant> GetAsync(string key)
        {
            DbDataReader dr = null;
            try
            {
                string strSql = "select * from identityserver where `key`=@Key";
                MySqlParameter[] parameters = {
                     new MySqlParameter("@Key", MySqlDbType.VarChar,1000)};
                parameters[0].Value = key;
                PersistedGrant entity = new PersistedGrant();
                using (dr = await DbHelperMySQL.ExecuteReaderAsync(DBConnection.UsersSystem, strSql, parameters))
                {
                    while (await dr.ReadAsync())
                    {
                        entity = DataRowToModel(dr);
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

        public async Task<int> RemoveAllAsync(string subjectId, string clientId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete from identityserver ");
                strSql.Append(" where SubjectId=@SubjectId and ClientId=@ClientId");
                MySqlParameter[] parameters = {
                    new MySqlParameter("@SubjectId", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@ClientId", MySqlDbType.VarChar,1000)};

                parameters[0].Value = subjectId;
                parameters[1].Value = clientId;

                return await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql.ToString(), parameters);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<int> RemoveAllAsync(string subjectId, string clientId, string type)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete from identityserver ");
                strSql.Append(" where `Type`=@Type and SubjectId=@SubjectId and ClientId=@ClientId");
                MySqlParameter[] parameters = {
                      new MySqlParameter("@Type", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@SubjectId", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@ClientId", MySqlDbType.VarChar,1000)};
                parameters[0].Value = type;
                parameters[1].Value = subjectId;
                parameters[2].Value = clientId;

                return await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql.ToString(), parameters);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<int> RemoveAsync(string key)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete from identityserver ");
                strSql.Append(" where `Key`=@Key");
                MySqlParameter[] parameters = {
                     new MySqlParameter("@Key", MySqlDbType.VarChar,1000)};
                parameters[0].Value = key;

                return await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql.ToString(), parameters);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<int> SaveAsync(PersistedGrant model)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into identityserver(");
                strSql.Append("`Key`,Type,SubjectId,ClientId,CreationTime,Expiration,Data)");
                strSql.Append(" values (");
                strSql.Append("@Key,@Type,@SubjectId,@ClientId,@CreationTime,@Expiration,@Data)");
                MySqlParameter[] parameters = {
                    new MySqlParameter("@Key", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@Type", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@SubjectId", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@ClientId", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@CreationTime", MySqlDbType.DateTime),
                    new MySqlParameter("@Expiration", MySqlDbType.DateTime),
                    new MySqlParameter("@Data", MySqlDbType.Text)};
                parameters[0].Value = model.Key;
                parameters[1].Value = model.Type;
                parameters[2].Value = model.SubjectId;
                parameters[3].Value = model.ClientId;
                parameters[4].Value = model.CreationTime;
                parameters[5].Value = model.Expiration;
                parameters[6].Value = model.Data;

                return await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql.ToString(), parameters);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<int> UpdateAsync(PersistedGrant model)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update identityserver set ");
                strSql.Append("Type=@Type,");
                strSql.Append("SubjectId=@SubjectId,");
                strSql.Append("ClientId=@ClientId,");
                strSql.Append("CreationTime=@CreationTime,");
                strSql.Append("Expiration=@Expiration,");
                strSql.Append("Data=@Data");
                strSql.Append(" where `Key`=@Key");
                MySqlParameter[] parameters = {
                    new MySqlParameter("@Key", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@Type", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@SubjectId", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@ClientId", MySqlDbType.VarChar,1000),
                    new MySqlParameter("@CreationTime", MySqlDbType.DateTime),
                    new MySqlParameter("@Expiration", MySqlDbType.DateTime),
                    new MySqlParameter("@Data", MySqlDbType.Text)};
                parameters[0].Value = model.Key;
                parameters[1].Value = model.Type;
                parameters[2].Value = model.SubjectId;
                parameters[3].Value = model.ClientId;
                parameters[4].Value = model.CreationTime;
                parameters[5].Value = model.Expiration;
                parameters[6].Value = model.Data;

                return await DbHelperMySQL.ExecuteSqlAsync(DBConnection.UsersSystem, strSql.ToString(), parameters);
            }
            catch (Exception err)
            {
                throw err;
            }
        }


        private PersistedGrant DataRowToModel(DbDataReader row)
        {
            PersistedGrant model = new PersistedGrant();
            if (row != null)
            {
                if (row["Key"] != null)
                {
                    model.Key = row["Key"].ToString();
                }
                if (row["Type"] != null)
                {
                    model.Type = row["Type"].ToString();
                }
                if (row["SubjectId"] != null)
                {
                    model.SubjectId = row["SubjectId"].ToString();
                }
                if (row["ClientId"] != null)
                {
                    model.ClientId = row["ClientId"].ToString();
                }
                if (row["CreationTime"] != null && row["CreationTime"].ToString() != "")
                {
                    model.CreationTime = DateTime.Parse(row["CreationTime"].ToString());
                }
                if (row["Expiration"] != null && row["Expiration"].ToString() != "")
                {
                    model.Expiration = DateTime.Parse(row["Expiration"].ToString());
                }
                if (row["Data"] != null)
                {
                    model.Data = row["Data"].ToString();
                }
            }
            return model;
        }
    }
}
