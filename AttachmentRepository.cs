using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Malaffi.Storage
{
    public static class AttachmentRepository
    {
        public static readonly string scanAndMoreConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static void InsertAttachment(Attachment attachment)
        {
            using (SqlConnection connection = new SqlConnection(scanAndMoreConnectionString))
            {
                SqlCommand command = new SqlCommand(Constants.SP_InsertAttachment, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                command.Parameters.AddWithValue("@ID", attachment.ID);
                command.Parameters.AddWithValue("@ReferenceID", attachment.ReferenceID);
                command.Parameters.AddWithValue("@AttachmentTypeID", (int)attachment.AttachmentType);
                command.Parameters.AddWithValue("@URL", attachment.URL);
                command.Parameters.AddWithValue("@Name", attachment.Name);
                command.Parameters.AddWithValue("@CreatedBy", attachment.CreatedBy);
                command.Parameters.AddWithValue("@MimeType", attachment.MimeType);
                command.Parameters.AddWithValue("@Size", attachment.Size);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SQL EXCEPTION OCCURRED WHILE TRYING INSERT ATTACHMENT", ex);
                }
            }
        }

        public static Attachment GetAttachmentByReferanceId(AttachmentTypes type, string referenceId)
        {
            Attachment result = null;
            using (SqlConnection connection = new SqlConnection(scanAndMoreConnectionString))
            {
                SqlCommand command = new SqlCommand(Constants.SP_GetAttachmentByReferenceId, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@AttachmentTypeID", (int)type);
                command.Parameters.AddWithValue("@ReferenceId", referenceId);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = ConvertToAttachment(reader);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("SQL EXCEPTION OCCURRED WHILE TRYING TO GET ATTACHMENT BY REFERANCE ID", ex);
                }
            }
            return result;
        }

        public static void DeleteAttachment(Guid attachemntId)
        {
            using (SqlConnection connection = new SqlConnection(scanAndMoreConnectionString))
            {
                SqlCommand command = new SqlCommand(Constants.SP_DeleteAttachment, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                command.Parameters.AddWithValue("@ID", attachemntId);
                command.Parameters.AddWithValue("@ModifiedBy", Thread.CurrentPrincipal.Identity.Name);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SQL EXCEPTION OCCURRED WHILE TRYING TO DELETE ATTACHMENT", ex);
                }
            }
        }

        public static Attachment GetAttachment(Guid attachmentId)
        {
            Attachment result = new Attachment();
            using (SqlConnection connection = new SqlConnection(scanAndMoreConnectionString))
            {
                SqlCommand command = new SqlCommand(Constants.SP_GetAttachmentByID, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ID", attachmentId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = ConvertToAttachment(reader);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("SQL EXCEPTION OCCURRED WHILE TRYING TO GET ATTACHMENT", ex);
                }
            }
            return result;
        }

        public static IList<Attachment> GetAttachmentList(AttachmentTypes type, string referenceId)
        {
            IList<Attachment> result = new List<Attachment>();
            using (SqlConnection connection = new SqlConnection(scanAndMoreConnectionString))
            {
                SqlCommand command = new SqlCommand(Constants.SP_ListAttachments, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@AttachmentTypeID", (int)type);
                command.Parameters.AddWithValue("@ReferenceId", referenceId);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var attachment = ConvertToAttachment(reader);
                        result.Add(attachment);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("SQL EXCEPTION OCCURRED WHILE TRYING TO GET ATTACHMENT LIST", ex);
                }
            }
            return result;
        }

        private static Attachment ConvertToAttachment(IDataReader rdr)
        {
            Attachment attachment = new Attachment();
            if (rdr["ID"] != DBNull.Value)
                attachment.ID = Guid.Parse(rdr["ID"].ToString());
            if (rdr["AttachmentTypeID"] != DBNull.Value)
                attachment.AttachmentType = (AttachmentTypes)Enum.Parse(typeof(AttachmentTypes), rdr["AttachmentTypeID"].ToString());
            if (rdr["CreatedBy"] != DBNull.Value)
                attachment.CreatedBy = rdr["CreatedBy"].ToString();
            if (rdr["CreatedOn"] != DBNull.Value)
                attachment.CreatedOn = DateTime.Parse(rdr["CreatedOn"].ToString());
            if (rdr["IsDeleted"] != DBNull.Value)
                attachment.IsDeleted = bool.Parse(rdr["IsDeleted"].ToString());
            if (rdr["URL"] != DBNull.Value)
                attachment.URL = rdr["URL"].ToString();
            if (rdr["MimeType"] != DBNull.Value)
                attachment.MimeType = rdr["MimeType"].ToString();
            if (rdr["ModifiedBy"] != DBNull.Value)
                attachment.ModifiedBy = rdr["ModifiedBy"].ToString();
            if (rdr["ModifiedOn"] != DBNull.Value)
                attachment.ModifiedOn = DateTime.Parse(rdr["ModifiedOn"].ToString());
            if (rdr["Name"] != DBNull.Value)
                attachment.Name = rdr["Name"].ToString();
            if (rdr["Size"] != DBNull.Value)
                attachment.Size = int.Parse(rdr["Size"].ToString());
            if (rdr["ReferenceID"] != DBNull.Value)
                attachment.ReferenceID = rdr["ReferenceID"].ToString();
            return attachment;
        }

        public static void UpdateAttachmentReferenceId(string oldRefernceId, string newReferenceId)
        {
            using (SqlConnection connection = new SqlConnection(scanAndMoreConnectionString))
            {
                SqlCommand command = new SqlCommand(Constants.SP_UpdateAttachmentReferenceId, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                command.Parameters.AddWithValue("@OldRefernceId", oldRefernceId);
                command.Parameters.AddWithValue("@NewReferenceId", newReferenceId);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SQL EXCEPTION OCCURRED WHILE TRYING TO UPDATE ATTACHMENT REFERENCE ID", ex);
                }
            }
        }
    }
}
