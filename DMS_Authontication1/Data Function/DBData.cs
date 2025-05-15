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


        public DataTable getApproval(string crd, string ncrd)
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT    CODE APPROV_NO, COMPANY_ID COMP_ID, CARD_NO CARD_NO, EMP_ENAME NAME, TO_CHAR(RECIV_DATE,'DD-MM-YYYY') RECIV_DATE, TO_CHAR(SEND_DATE,'DD-MM-YYYY') SEND_DATE, 
                                                                  SERVECE_TYP SERVECE_TYP, REPLAY REPLY, VALUE_AFTER APPROV_AMOUNT, MEDICAL_REPLAY MEDICAL_REPLAY, 
                                                                  CREATED_BY CREATED_BY, TO_CHAR(TRUNC(TO_DATE(CREATED_DATE)),'DD-MM-YYYY') CREATED_DATE, TO_CHAR(TO_DATE(CREATED_DATE), 'YYYY-MM-DD') CREATED_DATE1
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     (CARD_NO = :crd OR CARD_NO = :ncrd) AND active = 'Y'", con);



                cmd.Parameters.Clear();

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

        public DataTable getApprovalMatar()
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT    COUNT(CODE)
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE('1 NOV 2024')) AND TRUNC(TO_DATE('31 OCT 2025')) AND COMPANY_ID IN (500118, 500119, 500120, 500121, 500122) AND active = 'Y'", con);



                cmd.Parameters.Clear();
                                
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
        public DataTable getApprovalMatarCount()
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT  case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end typ, COUNT(CODE) cont
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE('1 NOV 2024')) AND TRUNC(TO_DATE('31 OCT 2025'))   
                                                            AND COMPANY_ID IN (500118, 500119, 500120, 500121, 500122) 
                                                            AND active = 'Y'
                                                        GROUP BY case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end", con);



                cmd.Parameters.Clear();

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
        public DataTable getApprovalMatarConsum()
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT  case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end typ, sum(APROVAL_VALUE) gross, sum(VALUE_AFTER) net
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE('1 NOV 2024')) AND TRUNC(TO_DATE('31 OCT 2025'))   
                                                            AND COMPANY_ID IN (500118, 500119, 500120, 500121, 500122) 
                                                            AND active = 'Y'
                                                        GROUP BY case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end", con);



                cmd.Parameters.Clear();

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
        public DataTable getApprovalMatarMonth()
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT    COUNT(CODE), NVL(sum(APROVAL_VALUE), 0) gross, NVL(sum(VALUE_AFTER), 0) net
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE('1 May 2025')) AND TRUNC(TO_DATE('31 May 2025')) AND COMPANY_ID IN (500118, 500119, 500120, 500121, 500122) AND active = 'Y'", con);



                cmd.Parameters.Clear();

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
        public DataTable getApprovalMatarIBNR()
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT    COUNT(CODE), NVL(sum(APROVAL_VALUE), 0) gross, NVL(sum(VALUE_AFTER), 0) net
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     TRUNC(TO_DATE(CREATED_DATE)) > TRUNC(TO_DATE('14 May 2025')) AND COMPANY_ID IN (500118, 500119, 500120, 500121, 500122) AND active = 'Y'", con);



                cmd.Parameters.Clear();

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
        public DataTable getApprovalMatarCountMonth()
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT  case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end typ, COUNT(CODE) cont
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE('1 MAY 2025')) AND TRUNC(TO_DATE('31 MAY 2025'))   
                                                            AND COMPANY_ID IN (500118, 500119, 500120, 500121, 500122) 
                                                            AND active = 'Y'
                                                        GROUP BY case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end", con);



                cmd.Parameters.Clear();

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
        public DataTable getApprovalMatarConsumMonth()
        {
            OracleConnection con = new OracleConnection(connectionStr);
            OracleCommand cmd = new OracleCommand();
            OracleDataAdapter da;
            DataTable dd = new DataTable();
            try
            {

                cmd = new OracleCommand(@"              SELECT  case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end typ, sum(APROVAL_VALUE) gross, sum(VALUE_AFTER) net
                                                        FROM      MEDICAL_APPROVALS 
                                                        WHERE     TRUNC(TO_DATE(CREATED_DATE)) BETWEEN TRUNC(TO_DATE('1 MAY 2025')) AND TRUNC(TO_DATE('31 MAY 2025'))   
                                                            AND COMPANY_ID IN (500118, 500119, 500120, 500121, 500122) 
                                                            AND active = 'Y'
                                                        GROUP BY case when SERVECE_TYP = 'Inpatient' then 'Inpatient' else 'Outpatient' end", con);



                cmd.Parameters.Clear();

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