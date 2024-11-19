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
        public DataTable getClaim(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"          SELECT CLAIM_NO1, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE, TO_NUMBER(BATCH_NO) BATCH_NO, ' ' GROUP_NO, CLAIM_AMOUNT GROSS, CLAIM_NET AMOUNT, ' ' NOTES, 1 F, TO_DATE(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE1
                                                    FROM IRS_CLAIM_REC_H WHERE (card_no = :crd OR card_no = :ncrd) AND claim_date BETWEEN :dat1 AND :dat2 AND PRV_NO != 99999
                                                    union all
                                                    select D_ID, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE, BATSH_NO, GROUP_NO, GROSS_AMOUNT GROSS, CLAIM_AMOUNT AMOUNT, NOTES, 2 F, TO_DATE(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE1
                                                    FROM ONLINE_CONS_01 WHERE (card_no = :crd OR card_no = :ncrd) AND claim_date BETWEEN :dat1 AND :dat2 AND GROUP_NO != 121
                                                    ORDER BY CLAIM_DATE1 DESC", con);



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
        public DataTable getIndemnity(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"       SELECT CLAIM_NO1, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE, CLAIM_AMOUNT, CLAIM_NET, TO_DATE(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE1 
                                                 FROM IRS_CLAIM_REC_H 
                                                 WHERE PRV_NO = 99999 AND (card_no = :crd OR card_no = :ncrd) AND TO_DATE(CLAIM_DATE) BETWEEN :dat1 AND :dat2 
                                                 UNION ALL
                                                 SELECT D_ID, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE, CLAIM_PAID, CLAIM_AMOUNT , TO_DATE(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE1
                                                 FROM ONLINE_CONS_01
                                                 WHERE SERV_CODE = 12101 AND (card_no = :crd OR card_no = :ncrd) AND TO_DATE(CLAIM_DATE) BETWEEN :dat1 AND :dat2 ORDER BY CLAIM_DATE1 DESC", con);



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
        public DataTable getLive(string crd, DateTime dat1, DateTime dat2, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"       SELECT D_ID Claim, TO_CHAR(D_DATE,'DD-MM-YYYY') D_DATE, PR_NAME Provid, PR_BRANCH_NAME Branch, decode(SERV_NAME, 'YES', 'Daily', 'MON', 'Chronic', 'MON_PH', 'Monthly') kind,  
                                                        D_VD Total, CARRY Co_Pay, OVER_INSURANCE  OVER, VALUE_CASH Cash, VALUE_CREDIT CREDIT 
                                                 FROM APP.DMS 
                                                 WHERE (card_id = :crd or card_id = :ncrd) AND TO_DATE(D_DATE) BETWEEN :dat1 AND :dat2 ORDER BY TO_DATE(D_DATE,'DD-MM-YYYY') DESC", con);



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
        public DataTable getData(string crd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                //cmd = new OracleCommand(@" SELECT e.C_COMP_ID, e.CONTRACT_NO, e.CLASS_CODE, e.EMP_ANAME_ST || ' ' || e.EMP_ANAME_SC || ' ' || e.EMP_ANAME_TH NAME, TO_CHAR(e.INS_START_DATE,'DD-MM-YYYY') INS_START_DATE, TO_CHAR(e.INS_END_DATE,'DD-MM-YYYY') INS_END_DATE  
                //                            FROM   DMS_TEST.COMP_EMPLOYEES e                                           
                //                            WHERE  e.CARD_ID = :crd AND TRUNC(TO_DATE(SYSDATE)) BETWEEN TRUNC(TO_DATE(e.INS_START_DATE)) AND TRUNC(TO_DATE(e.INS_END_DATE))", con);

                cmd = new OracleCommand(@" SELECT to_char(e.BIRTH_DATE,'DD-MM-YYYY'), e.C_COMP_ID, e.CLASS_CODE, TO_CHAR(e.INS_START_DATE,'DD-MM-YYYY') INS_START_DATE, TO_CHAR(e.INS_END_DATE,'DD-MM-YYYY') INS_END_DATE, e.TERMINATE_FLAG, e.EMP_ANAME_ST || ' ' || e.EMP_ANAME_SC || ' ' || e.EMP_ANAME_TH NAME, to_char(e.TERMINATE_DATE,'DD-MM-YYYY') TERMINATE_DATE, e.CONTRACT_NO, e.TEL1, e.TEL2, e.EMP_ID,  DECODE (e.GENDER, 1, 'Male', 2, 'Female')  Gender  
                                            FROM   DMS_TEST.COMP_EMPLOYEES e                                           
                                            WHERE  e.CARD_ID = :crd AND TRUNC(TO_DATE(SYSDATE)) BETWEEN TRUNC(TO_DATE(e.INS_START_DATE)) AND TRUNC(TO_DATE(e.INS_END_DATE))", con);


                cmd.Parameters.Clear();

                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;

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