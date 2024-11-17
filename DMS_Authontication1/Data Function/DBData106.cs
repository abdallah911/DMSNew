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
    /*72.52.116.106*/
public class DBData106
{
     public static string connectionStr = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                        (HOST=196.221.203.129)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)
                                        (SERVICE_NAME=ora11g)));User Id=app;Password=12369";


    //connection
    //OracleConnection con = new OracleConnection(connectionStr);
    //queries
    //public OracleCommand cmd = new OracleCommand();
    //OracleDataAdapter da;
    public DataTable getConsumptionMedOnline(string crd, DateTime dat1, DateTime dat2, string ncrd, string fmly)
    {
        OracleConnection con = new OracleConnection(connectionStr);
        OracleCommand cmd = new OracleCommand();
        OracleDataAdapter da;

        DataTable dd = new DataTable();
        try
        {
            if (fmly == "Y")
            {
                DBApproval db = new DBApproval();
                DataTable dtinf = new DataTable();

                dtinf = db.RunReader(@"SELECT C_COMP_ID, CONTRACT_NO, CLASS_CODE FROM DMS_TEST.COMP_EMPLOYEES WHERE NVL(TERMINATE_FLAG, 'N') = 'N' AND CARD_ID = '" + crd + "' ORDER BY INS_END_DATE DESC");

                Int32 cmp = Convert.ToInt32(dtinf.Rows[0]["C_COMP_ID"].ToString());
                int contr = Convert.ToInt16(dtinf.Rows[0]["CONTRACT_NO"].ToString());
                string cls = dtinf.Rows[0]["CLASS_CODE"].ToString();
                string cod = getempcode(crd);


                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE COMP_ID = :cmp AND GET_CARD_DETAILS (CARD_NO, 'CODE') = :crd and TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:dat1)) AND TRUNC(TO_DATE(SYSDATE)) AND TYPE = 'Online'
                                                    AND SERV_CODE NOT IN (
                                                    SELECT SER_SERV FROM DMS_TEST.COMP_CUSTOMIZED_D_D 
                                                    WHERE NVL(POLL_CONSUMPTION, 'N') = 'Y' AND C_COMP_ID = :cmp AND CONTRACT_NO = :contr AND CLASS_CODE = :cls)", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = cod;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                cmd.Parameters.Add(":cmp", OracleType.Number).Value = cmp;
                cmd.Parameters.Add(":contr", OracleType.Number).Value = contr;
                cmd.Parameters.Add(":cls", OracleType.VarChar).Value = cls;
            }
            else
            {
                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE (CARD_NO =:crd OR CARD_NO =:ncrd) and TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:dat1)) AND TRUNC(TO_DATE(SYSDATE)) AND TYPE = 'Online'", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
            }

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
    private string getempcode(string crd)
    {
        int on = crd.LastIndexOf('-');
        int to = crd.LastIndexOf('-', on - 1);
        string ss = crd.Substring(to + 1, on - to - 1);

        return ss;
    }
    public DataTable getConsumptionMedClaim(string crd, DateTime dat1, DateTime dat2, string ncrd, string fmly)
    {
        OracleConnection con = new OracleConnection(connectionStr);
        OracleCommand cmd = new OracleCommand();
        OracleDataAdapter da;

        DataTable dd = new DataTable();
        try
        {
            if (fmly == "Y")
            {

                DBApproval db = new DBApproval();
                DataTable dtinf = new DataTable();

                dtinf = db.RunReader(@"SELECT C_COMP_ID, CONTRACT_NO, CLASS_CODE FROM DMS_TEST.COMP_EMPLOYEES WHERE NVL(TERMINATE_FLAG, 'N') = 'N' AND CARD_ID = '" + crd + "' ORDER BY INS_END_DATE DESC");

                Int32 cmp = Convert.ToInt32(dtinf.Rows[0]["C_COMP_ID"].ToString());
                int contr = Convert.ToInt16(dtinf.Rows[0]["CONTRACT_NO"].ToString());
                string cls = dtinf.Rows[0]["CLASS_CODE"].ToString();
                string cod = getempcode(crd);



                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE COMP_ID = :cmp AND GET_CARD_DETAILS (CARD_NO, 'CODE') = :crd and TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:dat1)) AND TRUNC(TO_DATE(SYSDATE)) AND GROUP_NO = 116 AND TYPE = 'Claim'
                                                    AND SERV_CODE NOT IN (
                                                    SELECT SER_SERV FROM DMS_TEST.COMP_CUSTOMIZED_D_D 
                                                    WHERE NVL(POLL_CONSUMPTION, 'N') = 'Y' AND C_COMP_ID = :cmp AND CONTRACT_NO = :contr AND CLASS_CODE = :cls)", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = cod;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                cmd.Parameters.Add(":cmp", OracleType.Number).Value = cmp;
                cmd.Parameters.Add(":contr", OracleType.Number).Value = contr;
                cmd.Parameters.Add(":cls", OracleType.VarChar).Value = cls;
            }
            else
            {

                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE (CARD_NO =:crd OR CARD_NO =:ncrd) and TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:dat1)) AND TRUNC(TO_DATE(SYSDATE)) AND GROUP_NO = 116 AND TYPE = 'Claim'", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
            }

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
    public DataTable getConsumptionOther(string crd, DateTime dat1, DateTime dat2, string ncrd, string fmly)
    {
        OracleConnection con = new OracleConnection(connectionStr);
        OracleCommand cmd = new OracleCommand();
        OracleDataAdapter da;

        DataTable dd = new DataTable();
        try
        {
            if (fmly == "Y")
            {

                DBApproval db = new DBApproval();
                DataTable dtinf = new DataTable();

                dtinf = db.RunReader(@"SELECT C_COMP_ID, CONTRACT_NO, CLASS_CODE FROM DMS_TEST.COMP_EMPLOYEES WHERE NVL(TERMINATE_FLAG, 'N') = 'N' AND CARD_ID = '" + crd + "' ORDER BY INS_END_DATE DESC");

                Int32 cmp = Convert.ToInt32(dtinf.Rows[0]["C_COMP_ID"].ToString());
                int contr = Convert.ToInt16(dtinf.Rows[0]["CONTRACT_NO"].ToString());
                string cls = dtinf.Rows[0]["CLASS_CODE"].ToString();
                string cod = getempcode(crd);




                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE COMP_ID = :cmp AND GET_CARD_DETAILS (CARD_NO, 'CODE') = :crd and TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:dat1)) AND TRUNC(TO_DATE(SYSDATE)) AND GROUP_NO != 116 AND TYPE = 'Claim'
                                                    AND SERV_CODE NOT IN (
                                                    SELECT SER_SERV FROM DMS_TEST.COMP_CUSTOMIZED_D_D 
                                                    WHERE NVL(POLL_CONSUMPTION, 'N') = 'Y' AND C_COMP_ID = :cmp AND CONTRACT_NO = :contr AND CLASS_CODE = :cls)", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = cod;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
                cmd.Parameters.Add(":cmp", OracleType.Number).Value = cmp;
                cmd.Parameters.Add(":contr", OracleType.Number).Value = contr;
                cmd.Parameters.Add(":cls", OracleType.VarChar).Value = cls;
            }
            else
            {

                cmd = new OracleCommand(@" SELECT NVL(SUM(CLAIM_NET),0) FROM CONSM_APPROVAL WHERE (CARD_NO =:crd OR CARD_NO =:ncrd) and TRUNC(TO_DATE(CLAIM_DATE)) BETWEEN TRUNC(TO_DATE(:dat1)) AND TRUNC(TO_DATE(SYSDATE)) AND GROUP_NO != 116 AND TYPE = 'Claim'", con);

                cmd.Parameters.Clear();
                cmd.Parameters.Add(":crd", OracleType.VarChar).Value = crd;
                cmd.Parameters.Add(":ncrd", OracleType.VarChar).Value = ncrd;
                cmd.Parameters.Add(":dat1", OracleType.DateTime).Value = dat1;
            }

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


}

}