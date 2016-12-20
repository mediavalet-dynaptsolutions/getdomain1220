using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvapi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace Mvapi.DataAccessLayer
{
    public class TrackingDataAcessLayer
    {

        //public static  void SavetrackingData(TrackingDataModel trackingDataModel)
        //{

        //    MediaValetTracking mediaValetTracking = new MediaValetTracking();
        //    using (var ctx = new MediaValetTracking())
        //    {


        //        ctx.trackingDataModel.Add(trackingDataModel);
        //        ctx.SaveChanges();
        //    }
        //  //  mediaValetTracking.trackingDataModel.Add(trackingDataModel);
        //    // call SaveChanges method to save student into database

        //}

        public static void SaveTrackingData(string partitionid, string AssetId, string Username, string OrgUnitId, string Application, string Events,string AssetName)
        {
            try {
                // DateTimeOffset TimeStamp,

                string connStr = ConfigurationManager.ConnectionStrings["MediaValetTracking"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd = new SqlCommand("sp_mediavalettrackingdatainsert", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AssetId", SqlDbType.VarChar).Value = AssetId;
                    cmd.Parameters.AddWithValue("@Username", SqlDbType.VarChar).Value = Username;
                  // cmd.Parameters.AddWithValue("@TimeStamp", SqlDbType.DateTimeOffset).Value = timestapms;
                    cmd.Parameters.AddWithValue("@OrgUnitId", SqlDbType.VarChar).Value = OrgUnitId;
                    cmd.Parameters.AddWithValue("@Application", SqlDbType.VarChar).Value = Application;
                    cmd.Parameters.AddWithValue("@Events", SqlDbType.VarChar).Value = Events;
                    cmd.Parameters.AddWithValue("@AssetName", SqlDbType.VarChar).Value = AssetName;

                    cmd.ExecuteNonQuery();

                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }

                }
            }catch(Exception ex)
            {

            }
        }

      
    }

}