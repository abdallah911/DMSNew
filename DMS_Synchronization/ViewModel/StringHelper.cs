
namespace DMS_Synchronization.ViewModels
{
    internal class StringHelper
    {
        public static string GetQyertDMS_02_EMP_D_ENT = "select * from SH_01.DMS_02_EMP_D_ENT WHERE D_DATE>='28 MAY 2018'";
        public static string GetTableNameDMS_02_EMP_D_ENT = "SH_01.DMS_02_EMP_D_ENT";

        // public static string GetQyertCOMP_CUSTOMIZED_D_D = "select  * from DMS_TEST.COMP_CUSTOMIZED_D_D where rownum <20000  ";



        #region Select All For Insert
        public static string GetQyertBASIC_DATA = "select * from (select m.*, rownum r from  DMS_TEST.BASIC_DATA m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1}";
        public static string GetTableNameBASIC_DATA = "DMS_TEST.BASIC_DATA";

        public static string GetQyertCOMP_CUSTOMIZED_D_D = "select * from (select m.*, rownum r from  DMS_TEST.COMP_CUSTOMIZED_D_D m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CUSTOMIZED_D_D = "DMS_TEST.COMP_CUSTOMIZED_D_D";

        public static string GetQyertCOMP_CUSTOMIZED_D = "select * from (select m.*, rownum r from  DMS_TEST.COMP_CUSTOMIZED_D m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CUSTOMIZED_D = "DMS_TEST.COMP_CUSTOMIZED_D";

        public static string GetQyertCOMP_CUSTOMIZED_D_D_EMP = "select * from (select m.*, rownum r from  DMS_TEST.COMP_CUSTOMIZED_D_D_EMP m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CUSTOMIZED_D_D_EMP = "DMS_TEST.COMP_CUSTOMIZED_D_D_EMP";

        public static string GetQyertCOMP_CUSTOMIZED_D_EMP = "select * from (select m.*, rownum r from  DMS_TEST.COMP_CUSTOMIZED_D_EMP m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CUSTOMIZED_D_EMP = "DMS_TEST.COMP_CUSTOMIZED_D_EMP";

        public static string GetQyertCOMP_EMPLOYEES = "select * from (select m.*, rownum r from  DMS_TEST.COMP_EMPLOYEES m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_EMPLOYEES = "DMS_TEST.COMP_EMPLOYEES";

        public static string GetQyertCOMP_CONTRACT_CLASS = "select * from (select m.*, rownum r from  DMS_TEST.COMP_CONTRACT_CLASS m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CONTRACT_CLASS = "DMS_TEST.COMP_CONTRACT_CLASS";

        public static string GetQyertCONTRACT_COMP = "select * from (select m.*, rownum r from  DMS_TEST.CONTRACT_COMP m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCONTRACT_COMP = "DMS_TEST.CONTRACT_COMP";

        public static string GetQyertCONTRACT_DATA = "select * from (select m.*, rownum r from  DMS_TEST.CONTRACT_DATA m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCONTRACT_DATA = "DMS_TEST.CONTRACT_DATA";

        public static string GetQyertDIAGNOSES = "select * from (select m.*, rownum r from  DMS_TEST.DIAGNOSES m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameDIAGNOSES = "DMS_TEST.DIAGNOSES";

        public static string GetQyertMEDICINE_DATA = "select * from (select m.*, rownum r from  SH_01.MEDICINE_DATA m WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA' ) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameMEDICINE_DATA = "SH_01.MEDICINE_DATA";

        public static string GetQyertPR_BRA = "select * from (select m.*, rownum r from  DMS_TEST.PR_BRA m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePR_BRA = "DMS_TEST.PR_BRA";

        public static string GetQyertSERV_PROVIDERS = "select * from (select m.*, rownum r from  DMS_TEST.SERV_PROVIDERS m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameSERV_PROVIDERS = "DMS_TEST.SERV_PROVIDERS";

        public static string GetQyertSERV_PROVIDERS_NEW = "select * from (select m.*, rownum r from  APP.SERV_PROVIDERS_NEW m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameSERV_PROVIDERS_NEW = "APP.SERV_PROVIDERS_NEW";

        public static string GetQyertSERVICES = "select * from (select m.*, rownum r from  APP.SERVICES m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameSERVICES = "APP.SERVICES";

        public static string GetQyertSER_PROV_DISC = "select * from (select m.*, rownum r from  SH_01.SER_PROV_DISC m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameSER_PROV_DISC = "SH_01.SER_PROV_DISC";
        public static string GetQyertCO_INSURANCE_01 = "select * from (select m.*, rownum r from  SH_01.CO_INSURANCE_01 m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCO_INSURANCE_01 = "SH_01.CO_INSURANCE_01";

        public static string GetQyertDMS_02_EMP_D_ENT_MAN = "select * from (select m.*, rownum r from  SH_01.DMS_02_EMP_D_ENT_MAN m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameDMS_02_EMP_D_ENT_MAN = "SH_01.DMS_02_EMP_D_ENT_MAN";

        public static string GetQyertCOMP_CUSTOMIZED_D_D_MED = "select * from (select m.*, rownum r from  APP.COMP_CUSTOMIZED_D_D_MED m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CUSTOMIZED_D_D_MED = "APP.COMP_CUSTOMIZED_D_D_MED";
        
        public static string GetQyertREMAIN_CONSUMATION = "select * from (select m.*, rownum r from  APP.REMAIN_CONSUMATION m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameREMAIN_CONSUMATION = "APP.REMAIN_CONSUMATION";
        
        public static string GetQyertCONSUMPTION_POOL = "select * from (select m.*, rownum r from  APP.CONSUMPTION_POOL m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCONSUMPTION_POOL = "APP.CONSUMPTION_POOL";
        
        public static string GetQyertCLOSE_EMP_DATA = "select * from (select m.*, rownum r from  DMS_TEST.CLOSE_EMP_DATA m WHERE (IS_SYNC=0 OR IS_SYNC IS NULL) AND TRANS_TYP='L') WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCLOSE_EMP_DATA = "DMS_TEST.CLOSE_EMP_DATA";

        public static string GetQyertCOMP_CUSTOMIZED_D_D_MED_EMP = "select * from (select m.*, rownum r from  APP.COMP_CUSTOMIZED_D_D_MED_EMP m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CUSTOMIZED_D_D_MED_EMP = "APP.COMP_CUSTOMIZED_D_D_MED_EMP";

        //public static string GetQyertPOLL_AMOUNT = "select * from (select m.*, rownum r from  APP.POLL_AMOUNT m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        //public static string GetTableNamePOLL_AMOUNT = "APP.POLL_AMOUNT";
        public static string GetQyertPOLL_DATA = "select * from (select m.*, rownum r from  APP.POLL_DATA m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_DATA = "APP.POLL_DATA";

        public static string GetQyertPOLL_DATA_SERVICE = "select * from (select m.*, rownum r from  APP.POLL_DATA_SERVICE m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_DATA_SERVICE = "APP.POLL_DATA_SERVICE";

        public static string GetQyertPOLL_PERCENT = "select * from (select m.*, rownum r from  APP.POLL_PERCENT m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_PERCENT = "APP.POLL_PERCENT";

        public static string GetQyertPOLL_PERCENT_CARD = "select * from (select m.*, rownum r from  APP.POLL_PERCENT_CARD m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_PERCENT_CARD = "APP.POLL_PERCENT_CARD";

        public static string GetQyertPOLL_AMOUNT = "select * from (select m.*, rownum r from  APP.POLL_AMOUNT m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_AMOUNT = "APP.POLL_AMOUNT";

        public static string GetQyertPOLL_AMOUNT_CARD = "select * from (select m.*, rownum r from  APP.POLL_AMOUNT_CARD m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_AMOUNT_CARD = "APP.POLL_AMOUNT_CARD";

        public static string GetQyertPOLL_DATA_CHRONIC = "select * from (select m.*, rownum r from  APP.POLL_DATA_CHRONIC m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_DATA_CHRONIC = "APP.POLL_DATA_CHRONIC";

        public static string GetQyertPOLL_DATA_DIAG = "select * from (select m.*, rownum r from  APP.POLL_DATA_DIAG m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_DATA_DIAG = "APP.POLL_DATA_DIAG";

        public static string GetQyertPOLL_DATA_EXCEPTIONS = "select * from (select m.*, rownum r from  APP.POLL_DATA_EXCEPTIONS m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_DATA_EXCEPTIONS = "APP.POLL_DATA_EXCEPTIONS";

        public static string GetQyertAPPROVAL_BAD = "select * from (select m.*, rownum r from  APP.APPROVAL_BAD m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameAPPROVAL_BAD = "APP.APPROVAL_BAD";

        public static string GetQyertPOLL_DATA_PREX = "select * from (select m.*, rownum r from  APP.POLL_DATA_PREX m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNamePOLL_DATA_PREX = "APP.POLL_DATA_PREX";

        public static string GetQyertCOMP_CONTRACT_CLASS_EMP = "select * from (select m.*, rownum r from  dms_test.COMP_CONTRACT_CLASS_EMP m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameCOMP_CONTRACT_CLASS_EMP = "dms_test.COMP_CONTRACT_CLASS_EMP";

        #endregion

        #region Select For Update 

        public static string GetUpdateQyertBASIC_DATA = "SELECT * FROM DMS_TEST.BASIC_DATA WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1 ";

        public static string GetUpdateQyertCOMP_CUSTOMIZED_D_D = "SELECT * FROM DMS_TEST.COMP_CUSTOMIZED_D_D WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCOMP_CUSTOMIZED_D = "SELECT * FROM DMS_TEST.COMP_CUSTOMIZED_D WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCOMP_CUSTOMIZED_D_D_EMP = "SELECT * FROM DMS_TEST.COMP_CUSTOMIZED_D_D_EMP WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCOMP_CUSTOMIZED_D_EMP = "SELECT * FROM DMS_TEST.COMP_CUSTOMIZED_D_EMP WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCOMP_EMPLOYEES = "SELECT * FROM DMS_TEST.COMP_EMPLOYEES WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCOMP_CONTRACT_CLASS = "SELECT * FROM DMS_TEST.COMP_CONTRACT_CLASS WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCONTRACT_COMP = "SELECT * FROM DMS_TEST.CONTRACT_COMP WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCO_INSURANCE_01 = "SELECT * FROM SH_01.CO_INSURANCE_01 WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertDMS_02_EMP_D_ENT_MAN = "SELECT * FROM SH_01.DMS_02_EMP_D_ENT_MAN WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";
        public static string GetUpdateQyertCOMP_CUSTOMIZED_D_D_MED = "SELECT * FROM APP.COMP_CUSTOMIZED_D_D_MED WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";
        public static string GetUpdateQyertREMAIN_CONSUMATION = "SELECT * FROM APP.REMAIN_CONSUMATION WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";
        public static string GetUpdateQyertCOMP_CUSTOMIZED_D_D_MED_EMP = "SELECT * FROM APP.COMP_CUSTOMIZED_D_D_MED_EMP WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCONTRACT_DATA = "SELECT * FROM DMS_TEST.CONTRACT_DATA WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertDIAGNOSES = "SELECT * FROM DMS_TEST.DIAGNOSES WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertMEDICINE_DATA = "SELECT * FROM SH_01.MEDICINE_DATA WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertPR_BRA = "SELECT * FROM DMS_TEST.PR_BRA WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertSERV_PROVIDERS = "SELECT * FROM DMS_TEST.SERV_PROVIDERS WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertCOMP_CONTRACT_CLASS_EMP = "SELECT * FROM DMS_TEST.COMP_CONTRACT_CLASS_EMP WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertPOLL_DATA_PREX = "SELECT * FROM APP.POLL_DATA_PREX WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertAPPROVAL_BAD = "SELECT * FROM APP.APPROVAL_BAD WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertPOLL_DATA_EXCEPTIONS = "SELECT * FROM APP.POLL_DATA_EXCEPTIONS WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertPOLL_DATA_DIAG = "SELECT * FROM APP.POLL_DATA_DIAG WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertPOLL_DATA_CHRONIC = "SELECT * FROM APP.POLL_DATA_CHRONIC WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        public static string GetUpdateQyertPOLL_AMOUNT_CARD = "SELECT * FROM APP.POLL_AMOUNT_CARD WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1";

        #endregion

        #region Push Query

        public static string GetQyertRoshita = "select * from (select m.*, rownum r from  Roshita m WHERE IS_SYNC=0 OR IS_SYNC IS NULL) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameRoshita = "Roshita";


        public static string GetQyertMedicineGroup = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY GroupId DESC) AS RowNum, * FROM MedicineGroup" +
                 " WHERE (IsSync=0 OR IsSync IS NULL )) " +
                 "AS m WHERE RowNum > {0} AND RowNum<= {1}";
        public static string GetTableNameMedicineGroup = "MedicineGroup";



        public static string GetQyertMedicineData = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Id DESC) AS RowNum, * FROM MedicineData" +
                 " WHERE (IsSync=0 OR IsSync IS NULL )) " +
                 "AS m WHERE RowNum > {0} AND RowNum<= {1}";
        public static string GetTableNameMedicineData = "MedicineData";


        #endregion






        //public static string GetQyertMED_MEDICINE = "select MED_CODE, CARD_NO, RDATE, ROUND(MED_TYP,3), ROUND(DOSE,3), ROUND(NO_OF_UINT,3), ROUND(TOTAL_AMT, 3), ROUND(MED_DURATION,3), ROUND(TOT_DUR,3)," +
        //                    " ROUND(DOS_DUR,3), ROUND(EXCESS,3), ROUND(PACK_SIZE,3), ROUND(PACK_PRICE,3), CON_MED, ROUND(UNIT_NO,3),ROUND(UNIT_PRICE, 3), MED_NAME, DOSAGE_FORM, " +
        //                   " NOTES, ACTIVE, CREATED_BY, CREATED_DATE, UPDATE_BY, UPDATE_DATE, ROUND(ACT_MONTH,3), ROUND(LFT_MONTH,3) from SH_01.MED_MEDICINE ";

        public static string GetQyertMED_MEDICINE = "select * from (select m.*, rownum r from SH_01.MED_MEDICINE m ) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameMED_MEDICINE = "SH_01.MED_MEDICINE";


        public static string GetQyertMED_CARD = "select * from (select m.*, rownum r from SH_01.MED_CARD m) WHERE r > {0} and r<= {1} ";
        public static string GetTableNameMED_CARD = "SH_01.MED_CARD";


        //public static string GetQyertPR_BRA = "select * from DMS_TEST.PR_BRA WHERE IS_SYNC=0";
        //public static string GetTableNamePR_BRA = "DMS_TEST.PR_BRA";
        //public static string GetQyertSERV_PROVIDERS = "select * from DMS_TEST.SERV_PROVIDERS WHERE IS_SYNC=0";
        //public static string GetTableNameSERV_PROVIDERS = "DMS_TEST.SERV_PROVIDERS";

        //public static string GetQyertCO_INSURANCE_01 = "select * from SH_01.CO_INSURANCE_01";
        //public static string GetTableNameCO_INSURANCE_01 = "SH_01.CO_INSURANCE_01";
        public static string GetQyertNAME_LAB = "select * from SH_01.NAME_LAB";
        public static string GetTableNameNAME_LAB = "SH_01.NAME_LAB";
        public static string GetQyertNAME_LAB_CEN = "select * from SH_01.NAME_LAB_CEN";
        public static string GetTableNameNAME_LAB_CEN = "SH_01.NAME_LAB_CEN";
        public static string GetQyertNAME_RAY = "select * from SH_01.NAME_RAY";
        public static string GetTableNameNAME_RAY = "SH_01.NAME_RAY";
        public static string GetQyertNAME_RAY_CEN = "select * from SH_01.NAME_RAY_CEN";
        public static string GetTableNameNAME_RAY_CEN = "SH_01.NAME_RAY_CEN";
        public static string GetQyertSERV_LAB = "select * from SH_01.SERV_LAB";
        public static string GetTableNameSERV_LAB = "SH_01.SERV_LAB";
        public static string GetQyertSERV_RAY = "select * from SH_01.SERV_RAY";
        public static string GetTableNameSERV_RAY = "SH_01.SERV_RAY";
        public static string GetQyertUSERS = "select * from SH_01.USERS";
        public static string GetTableNameUSERS = "SH_01.USERS";

        public static string GetQyertMEDICINEGROUP = "select * from SH_01.MEDICINEGROUP";
        public static string GetTableNameMEDICINEGROUP = "SH_01.MEDICINEGROUP";
        public static string GetQyertINV_SAL = "select * from SH_01.INV_SAL";
        public static string GetTableNameINV_SAL = "SH_01.INV_SAL";

        //public static string GetQyertS_ENT_2 = "select * from SH_01.S_ENT_2";
        //public static string GetTableNameS_ENT_2 = "SH_01.S_ENT_2";
        public static string GetQyertS_ENT_7 = "select * from SH_01.S_ENT_7";
        public static string GetTableNameS_ENT_7 = "SH_01.S_ENT_7";


        //public static string GetQyertComp_Employees = "select * from DMS_TEST.Comp_Employees WHERE IS_SYNC=0";
        //public static string GetTableNameComp_Employees = "DMS_TEST.Comp_Employees";
        //public static string GetQyertINV_SAL = "select * from SH_01.INV_SAL";
        //public static string GetTableNameINV_SAL = "SH_01.INV_SAL";
        //public static string GetQyertS_ENT_2 = "select * from SH_01.S_ENT_2";
        //public static string GetTableNameS_ENT_2 = "SH_01.S_ENT_2";
        //public static string GetQyertPROVIDERS = "select * from SH_01.PROVIDERS";
        //public static string GetTableNamePROVIDERS = "SH_01.PROVIDERS";


        //employee_coding
        public static string GetQyertEMPLOYEES_CODING = "select * from EMPLOYEES_CODING";
        public static string GetTableNameEMPLOYEES_CODING = "EMPLOYEES_CODING";
        public static string GetQyertEMP_LoansTransactions = "select * from EMP_LoansTransactions";
        public static string GetTableNameEMP_LoansTransactions = "EMP_LoansTransactions";

    }
}

