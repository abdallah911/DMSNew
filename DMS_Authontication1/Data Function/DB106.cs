using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Web.Hosting;

namespace DMS_Authontication1.Data_Function
{/*196.221.203.129*/
    /*171.0.1.96*/
    public class DB106
    {
         public static string connectionStr = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST=72.52.116.106)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=15+08+2017";

       
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
        public DataTable getInvoices(Int64 comp1, Int64 comp2, DateTime serv1, DateTime serv2, DateTime reg1, DateTime reg2,
                                       Int64 invoc1, Int64 invoc2, Int64 batch1, Int64 batch2, Int64 prov1, Int64 prov2, Int64 clm1, Int64 clm2)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {
                    cmd = new OracleCommand(@"SELECT t1.COMP_ID, t2.C_ANAME, TO_CHAR(t2.START_DATE,'DD-MM-YYYY') START_DATE, TO_CHAR(t2.END_DATE,'DD-MM-YYYY') END_DATE, t1.COUNT_BATCH, t1.COUNT_CLAIM, t1.GROSS, t1.NET, t1.INVOICE_NO
                                              FROM  (   SELECT  COMP_ID, INVOICE_NO, COUNT(DISTINCT BATCH_NO) COUNT_BATCH, COUNT(DISTINCT CLAIM_NO) COUNT_CLAIM, SUM(CLAIM_SUBMITTED) GROSS, SUM(NET) NET
                                                        FROM    APP.REVIEW_CLAIMS
                                                        WHERE     COMP_ID BETWEEN :comp1 AND :comp2                                                      
                                                              AND TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE(:reg1)) AND TRUNC(TO_DATE(:reg2))
                                                              AND TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:serv1)) AND TRUNC(TO_DATE(:serv2))
                                                              AND NVL(INVOICE_NO, 0) BETWEEN :invoc1 AND :invoc2
                                                              AND NVL(BATCH_NO, 0) BETWEEN :batch1 AND :batch2
                                                              AND NVL(PRV_NO, 0) BETWEEN :prov1 AND :prov2
                                                              AND NVL(CLAIM_NO, 0) BETWEEN :clm1 AND :clm2
                                                        GROUP BY COMP_ID, INVOICE_NO) t1, APP.DIS_COMP t2
                                              WHERE     t1.COMP_ID = t2.C_COMP_ID", con);
            
                cmd.Parameters.Clear();


                cmd.Parameters.Add(":comp1", OracleType.Number).Value = comp1;
                cmd.Parameters.Add(":comp2", OracleType.Number).Value = comp2;
                cmd.Parameters.Add(":serv1", OracleType.DateTime).Value = serv1;
                cmd.Parameters.Add(":serv2", OracleType.DateTime).Value = serv2;
                cmd.Parameters.Add(":reg1", OracleType.DateTime).Value = reg1;
                cmd.Parameters.Add(":reg2", OracleType.DateTime).Value = reg2;               
                cmd.Parameters.Add(":invoc1", OracleType.Number).Value = invoc1;
                cmd.Parameters.Add(":invoc2", OracleType.Number).Value = invoc2;
                cmd.Parameters.Add(":batch1", OracleType.Number).Value = batch1;
                cmd.Parameters.Add(":batch2", OracleType.Number).Value = batch2;

                cmd.Parameters.Add(":prov1", OracleType.Number).Value = prov1;
                cmd.Parameters.Add(":prov2", OracleType.Number).Value = prov2;
                cmd.Parameters.Add(":clm1", OracleType.Number).Value = clm1;
                cmd.Parameters.Add(":clm2", OracleType.Number).Value = clm2;

                da = new OracleDataAdapter(cmd);

                da.Fill(dd);
                con.Dispose();
                con.Close();


                return dd;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);  
                string logFilePath = HostingEnvironment.MapPath("~/Reports/HR/logs.txt");
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(ex.Message.ToString());
                }

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
        public DataTable getBatch(Int64 cmp, Int64 invoc, DateTime serv1, DateTime serv2, DateTime reg1, DateTime reg2,
                                  Int64 batch1, Int64 batch2, Int64 prov1, Int64 prov2, Int64 clm1, Int64 clm2, string provTyp ="")
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {
                cmd = new OracleCommand(@"  SELECT  BATCH_NO, PRV_NO, PRV_NAME, PROVIDER_TYPE, COUNT(DISTINCT CLAIM_NO) COUNT_CLAIM, SUM(CLAIM_SUBMITTED) GROSS, SUM(NET) NET
                                            FROM    APP.REVIEW_CLAIMS
                                            WHERE   COMP_ID = :cmp AND INVOICE_NO = :invoc     
                                                AND TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE(:reg1)) AND TRUNC(TO_DATE(:reg2))
                                                AND TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:serv1)) AND TRUNC(TO_DATE(:serv2))
                                                AND NVL(BATCH_NO, 0) BETWEEN :batch1 AND :batch2
                                                AND NVL(PRV_NO, 0) BETWEEN :prov1 AND :prov2
                                                AND NVL(CLAIM_NO, 0) BETWEEN :clm1 AND :clm2
                                                AND (:provTyp IS NULL OR :provTyp = '' OR PROVIDER_TYPE = :provTyp)
                                            GROUP BY BATCH_NO, PRV_NO, PRV_NAME, PROVIDER_TYPE
                                            ORDER BY PROVIDER_TYPE", con);

                cmd.Parameters.Clear();

                cmd.Parameters.Add(":cmp", OracleType.Number).Value = cmp;
                cmd.Parameters.Add(":invoc", OracleType.Number).Value = invoc;
                cmd.Parameters.Add(":serv1", OracleType.DateTime).Value = serv1;
                cmd.Parameters.Add(":serv2", OracleType.DateTime).Value = serv2;
                cmd.Parameters.Add(":reg1", OracleType.DateTime).Value = reg1;
                cmd.Parameters.Add(":reg2", OracleType.DateTime).Value = reg2;                
                cmd.Parameters.Add(":batch1", OracleType.Number).Value = batch1;
                cmd.Parameters.Add(":batch2", OracleType.Number).Value = batch2;

                cmd.Parameters.Add(":prov1", OracleType.Number).Value = prov1;
                cmd.Parameters.Add(":prov2", OracleType.Number).Value = prov2;
                cmd.Parameters.Add(":clm1", OracleType.Number).Value = clm1;
                cmd.Parameters.Add(":clm2", OracleType.Number).Value = clm2;

                cmd.Parameters.Add(":provTyp", OracleType.VarChar).Value = provTyp;

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
        public DataTable getClaims(DateTime reg1, DateTime reg2, DateTime serv1, DateTime serv2, int cmp, Int64 aprov1, Int64 aprov2, string crd, Int64 invoc1, Int64 invoc2, 
                                   Int64 batch1, Int64 batch2)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {
                if (string.IsNullOrEmpty(crd) == false)
                    cmd = new OracleCommand(@" SELECT  DISTINCT CLAIM_NO, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE,
                                                       CARD_NO, EMP_ANAME EMP_NAME, CLAIM_SUBMITTED GROSS, NET, TAKHASOS DIAGNOSIS, SERV_TYPE
                                                FROM    APP.REVIEW_CLAIMS 
                                                WHERE       COMP_ID = :cmp 
                                                      AND   CARD_NO = :crd         
                                                AND TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE(:reg1)) AND TRUNC(TO_DATE(:reg2))
                                                AND TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:serv1)) AND TRUNC(TO_DATE(:serv2))
                                                      AND CLAIM_NO BETWEEN :aprov1 AND :aprov2
                                                      AND NVL(INVOICE_NO, 0) BETWEEN :invoc1 AND :invoc2
                                                      AND NVL(BATCH_NO, 0) BETWEEN :batch1 AND :batch2
                                                      AND NET IS NOT NULL", con);
                else
                    cmd = new OracleCommand(@" SELECT  DISTINCT CLAIM_NO, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE,
                                                       CARD_NO, EMP_ANAME EMP_NAME, CLAIM_SUBMITTED GROSS, NET, TAKHASOS DIAGNOSIS, SERV_TYPE
                                                FROM    APP.REVIEW_CLAIMS 
                                                WHERE       COMP_ID = :cmp  
                                                      AND TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE(:reg1)) AND TRUNC(TO_DATE(:reg2))
                                                      AND TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:serv1)) AND TRUNC(TO_DATE(:serv2))
                                                      AND CLAIM_NO BETWEEN :aprov1 AND :aprov2
                                                      AND NVL(INVOICE_NO, 0) BETWEEN :invoc1 AND :invoc2
                                                      AND NVL(BATCH_NO, 0) BETWEEN :batch1 AND :batch2
                                                      AND NET IS NOT NULL", con);



                cmd.Parameters.Clear();


                cmd.Parameters.Add(":cmp", OracleType.Number).Value = cmp;
                
                cmd.Parameters.Add(":reg1", OracleType.DateTime).Value = reg1;
                cmd.Parameters.Add(":reg2", OracleType.DateTime).Value = reg2;
                cmd.Parameters.Add(":serv1", OracleType.DateTime).Value = serv1;
                cmd.Parameters.Add(":serv2", OracleType.DateTime).Value = serv2;
                cmd.Parameters.Add(":aprov1", OracleType.Number).Value = aprov1;
                cmd.Parameters.Add(":aprov2", OracleType.Number).Value = aprov2;
                cmd.Parameters.Add(":invoc1", OracleType.Number).Value = invoc1;
                cmd.Parameters.Add(":invoc2", OracleType.Number).Value = invoc2;
                cmd.Parameters.Add(":batch1", OracleType.Number).Value = batch1;
                cmd.Parameters.Add(":batch2", OracleType.Number).Value = batch2;

                if (string.IsNullOrEmpty(crd) == false)
                    cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;


                da = new OracleDataAdapter(cmd);

                da.Fill(dd);
                con.Dispose();
                con.Close();


                return dd;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);  
                string logFilePath = HostingEnvironment.MapPath("~/Reports/HR/logs.txt");
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(ex.Message.ToString());
                }

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
        public DataTable getClaims(int cmp, DateTime serv1, DateTime serv2, DateTime reg1, DateTime reg2, 
                                   Int64 aprov1, Int64 aprov2, string crd, Int64 invoc1, Int64 invoc2, Int64 batch1, Int64 batch2) 
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {
                if(string.IsNullOrEmpty(crd) == false)
                    cmd = new OracleCommand(@" SELECT  DISTINCT CLAIM_NO, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE,
                                                       CARD_NO, EMP_ANAME EMP_NAME, PRV_NAME, PROVIDER_TYPE, TAKHASOS DIAGNOSIS, SERV_TYPE
                                                FROM    APP.REVIEW_CLAIMS 
                                                WHERE       COMP_ID = :cmp 
                                                      AND   CARD_NO = :crd
                                                      AND TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE(:reg1)) AND TRUNC(TO_DATE(:reg2))
                                                      AND TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:serv1)) AND TRUNC(TO_DATE(:serv2))
                                                      AND CLAIM_NO BETWEEN :aprov1 AND :aprov2
                                                      AND NVL(INVOICE_NO, 0) BETWEEN :invoc1 AND :invoc2
                                                      AND NVL(BATCH_NO, 0) BETWEEN :batch1 AND :batch2", con);
                else
                    cmd = new OracleCommand(@" SELECT  DISTINCT CLAIM_NO, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') CREATED_DATE, TO_CHAR(CLAIM_DATE,'DD-MM-YYYY') CLAIM_DATE,
                                                       CARD_NO, EMP_ANAME EMP_NAME, PRV_NAME, PROVIDER_TYPE, TAKHASOS DIAGNOSIS, SERV_TYPE
                                                FROM    APP.REVIEW_CLAIMS 
                                                WHERE       COMP_ID = :cmp 
                                                      AND TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE(:reg1)) AND TRUNC(TO_DATE(:reg2))
                                                      AND TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:serv1)) AND TRUNC(TO_DATE(:serv2))
                                                      AND CLAIM_NO BETWEEN :aprov1 AND :aprov2
                                                      AND NVL(INVOICE_NO, 0) BETWEEN :invoc1 AND :invoc2
                                                      AND NVL(BATCH_NO, 0) BETWEEN :batch1 AND :batch2", con);



                cmd.Parameters.Clear();


                cmd.Parameters.Add(":cmp", OracleType.Number).Value = cmp;

                cmd.Parameters.Add(":serv1", OracleType.DateTime).Value = serv1;
                cmd.Parameters.Add(":serv2", OracleType.DateTime).Value = serv2;
                cmd.Parameters.Add(":reg1", OracleType.DateTime).Value = reg1;
                cmd.Parameters.Add(":reg2", OracleType.DateTime).Value = reg2;
                cmd.Parameters.Add(":aprov1", OracleType.Number).Value = aprov1;
                cmd.Parameters.Add(":aprov2", OracleType.Number).Value = aprov2;
                cmd.Parameters.Add(":invoc1", OracleType.Number).Value = invoc1;
                cmd.Parameters.Add(":invoc2", OracleType.Number).Value = invoc2;
                cmd.Parameters.Add(":batch1", OracleType.Number).Value = batch1;
                cmd.Parameters.Add(":batch2", OracleType.Number).Value = batch2;

                if (string.IsNullOrEmpty(crd) == false)
                    cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;


                da = new OracleDataAdapter(cmd);

                da.Fill(dd);
                con.Dispose();
                con.Close();

                
                return dd;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);  
                string logFilePath = HostingEnvironment.MapPath("~/Reports/HR/logs.txt");
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(ex.Message.ToString());
                }

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
        public DataTable getClaimDetails(Int64 clm)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {
                    cmd = new OracleCommand(@"  SELECT      SERVICES, SERCV_NAME, CLAIM_SUBMITTED, OVER_INSURANCE, DISCOUNT, APPROV_AMOUNT, COPAY_AMT, AFTER_COPAY, 
                                                            LOCAL_AMOUNT, IMPORT_AMOUNT, LOCAL_DISC, IMPORT_DISC, TOTAL_DISCOUNT, NET
                                                FROM    APP.REVIEW_CLAIMS 
                                                WHERE   CLAIM_NO = :clm
                                                ORDER BY NET NULLS LAST", con);
             
                cmd.Parameters.Clear();

                cmd.Parameters.Add(":clm", OracleType.Number).Value = clm;

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