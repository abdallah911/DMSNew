using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OracleClient;
namespace DMS_Authontication1.Data_Function
{/*196.221.203.129*/
    /*171.0.1.96*/
    public class DBData
    {
         public static string connectionStr = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=196.221.203.129)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369";

       
        //connection
        //OracleConnection con = new OracleConnection(connectionStr);
        //queries
        //public OracleCommand cmd = new OracleCommand();
        //OracleDataAdapter da;
        public DataTable getConsumptionMedOnline(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;

            DataTable dd = new DataTable();
            try
            {
                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_ONLINE WHERE (CARD_NO =:crd OR CARD_NO =:ncrd) and CLAIM_DATE BETWEEN :dat1 AND SYSDATE AND GROUP_NO = 116", con);
               
               //     cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE (CARD_NO =:crd OR CARD_NO =:ncrd) and CLAIM_DATE BETWEEN :dat1 AND SYSDATE AND GROUP_NO != 116", con);


                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                //  cmd.Parameters.Add(":dat2", OracleType.DateTime).Value = dat2;

                da = new OracleDataAdapter(cmd);

                da.Fill(dd);
                con.Dispose();
                con.Close();

                OracleConnection.ClearAllPools();
                return dd;
            }
            catch (Exception ex) { return dd; }
            finally
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Dispose();
                    con.Close();

                    OracleConnection.ClearAllPools();
                }

            }
        }
        public DataTable getConsumptionMedClaim(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;

            DataTable dd = new DataTable();
            try
            {
                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM ME_AUB WHERE (CARD_NO =:crd OR CARD_NO =:ncrd) and CLAIM_DATE BETWEEN :dat1 AND SYSDATE AND GROUP_NO = 116", con);
                


                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                //  cmd.Parameters.Add(":dat2", OracleType.DateTime).Value = dat2;

                da = new OracleDataAdapter(cmd);

                da.Fill(dd);
                con.Dispose();
                con.Close();

                OracleConnection.ClearAllPools();
                return dd;
            }
            catch (Exception ex) { return dd; }
            finally
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Dispose();
                    con.Close();

                    OracleConnection.ClearAllPools();
                }

            }
        }
        public DataTable getConsumptionOther(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;

            DataTable dd = new DataTable();
            try
            {
                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE (CARD_NO =:crd OR CARD_NO =:ncrd) and CLAIM_DATE BETWEEN :dat1 AND SYSDATE AND GROUP_NO != 116", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                //  cmd.Parameters.Add(":dat2", OracleType.DateTime).Value = dat2;

                da = new OracleDataAdapter(cmd);

                da.Fill(dd);
                con.Dispose();
                con.Close();

                OracleConnection.ClearAllPools();
                return dd;
            }
            catch (Exception ex) { return dd; }
            finally
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Dispose();
                    con.Close();

                    OracleConnection.ClearAllPools();
                }

            }
        }
        public DataTable getConsumptionApproval(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;

            DataTable dd = new DataTable();
            try
            {
                cmd = new OracleCommand(@"SELECT NVL(SUM(VALUE_AFTER), 0) FROM MEDICAL_APPROVALS WHERE (CARD_NO = :crd OR CARD_NO = :ncrd) AND TRUNC(TO_DATE(CREATED_DATE)) BETWEEN :dat1 AND TRUNC(TO_DATE(SYSDATE)) AND active = 'Y'", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                //  cmd.Parameters.Add(":dat2", OracleType.DateTime).Value = dat2;

                da = new OracleDataAdapter(cmd);

                da.Fill(dd);
                con.Dispose();
                con.Close();

                OracleConnection.ClearAllPools();
                return dd;
            }
            catch (Exception ex) { return dd; }
            finally
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Dispose();
                    con.Close();

                    OracleConnection.ClearAllPools();
                }

            }
        }
        public DataTable getApproval(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {
                
                cmd = new OracleCommand(@"              select    TO_CHAR(APROV_NO) APPROV_NO, COMP_ID COMP_ID, CARD_NO CARD_NO, PATIENT_NAME NAME,TO_CHAR(DATE_RECIVE,'DD-MM-YYYY') RECIV_DATE,
                                                                  TO_CHAR(DATE_SEND,'DD-MM-YYYY') SEND_DATE, SERV_ENAME SERVECE_TYP, APROV_REPLY  REPLY,
                                                                  APPROV_AMOUNT APPROV_AMOUNT, MED_APP MEDICAL_REPLAY,CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE , TO_DATE(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE1
                                                        FROM      V_APPROVAL LEFT OUTER JOIN IRS_SUPER_GROUP_NEW ON V_APPROVAL.APROV_TYP = IRS_SUPER_GROUP_NEW.IRS_CODE
                                                        WHERE     (CARD_NO = :crd OR CARD_NO = :ncrd) AND TO_DATE(CREATED_DATE) BETWEEN :dat1 AND :dat2-- order by  TO_DATE(created_date,'DD-MM-YYYY') desc
                                                            union all
                                                        select    TO_CHAR(APROV_NO) APPROV_NO, COMP_ID COMP_ID, CARD_NO CARD_NO, PATIENT_NAME NAME,TO_CHAR(DATE_RECIVE,'DD-MM-YYYY') RECIV_DATE,
                                                                                                              TO_CHAR(DATE_SEND,'DD-MM-YYYY') SEND_DATE, SERV_ENAME SERVECE_TYP, APROV_REPLY  REPLY,
                                                                  APPROV_AMOUNT APPROV_AMOUNT, MED_APP MEDICAL_REPLAY,CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE , TO_DATE(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE1
                                                        FROM      IRS_APPROVAL_HIST LEFT OUTER JOIN IRS_SUPER_GROUP_NEW ON IRS_APPROVAL_HIST.APROV_TYP = IRS_SUPER_GROUP_NEW.IRS_CODE
                                                        WHERE     (CARD_NO = :crd OR CARD_NO = :ncrd) AND TO_DATE(CREATED_DATE) BETWEEN :dat1 AND :dat2 --order by  TO_DATE(created_date,'DD-MM-YYYY') desc
                                                            union all
                                                        select    CODE APPROV_NO, COMPANY_ID COMP_ID, CARD_NO CARD_NO, EMP_ENAME NAME, TO_CHAR(RECIV_DATE,'DD-MM-YYYY') RECIV_DATE, TO_CHAR(SEND_DATE,'DD-MM-YYYY') SEND_DATE, 
                                                                  SERVECE_TYP SERVECE_TYP, REPLAY REPLY, VALUE_AFTER APPROV_AMOUNT, MEDICAL_REPLAY MEDICAL_REPLAY, 
                                                                  CREATED_BY CREATED_BY, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE, TO_DATE(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE1
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     (CARD_NO = :crd OR CARD_NO = :ncrd) AND active = 'Y' AND TO_DATE(CREATED_DATE) BETWEEN :dat1 AND :dat2 order by CREATED_DATE1 desc", con);



                cmd.Parameters.Clear();

                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                cmd.Parameters.Add(":dat2", OracleType.DateTime).Value = dat2;
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;

                da = new OracleDataAdapter(cmd);               

                da.Fill(dd);
                con.Dispose();
                con.Close();

                OracleConnection.ClearAllPools();

                return dd;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);                
                return dd;
            }

            finally
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Dispose();
                    con.Close();

                    OracleConnection.ClearAllPools();
                }
            }
        }


    }

}