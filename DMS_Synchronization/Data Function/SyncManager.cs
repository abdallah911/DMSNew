//using DMS_Synchronization.BackModels;
using DMS_Synchronization.Exceptions;
using DMS_Synchronization.Models;
using DMS_Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using DMS_Synchronization.OracleModels;
using DMS_Synchronization.BaseEntity;
using DMS_Synchronization.ViewModel;
using OracleConnection = Oracle.ManagedDataAccess.Client.OracleConnection;
using OracleDataAdapter = Oracle.ManagedDataAccess.Client.OracleDataAdapter;
using OracleCommand = Oracle.ManagedDataAccess.Client.OracleCommand;
using OracleDataReader = Oracle.ManagedDataAccess.Client.OracleDataReader;
using Microsoft.Ajax.Utilities;

namespace DMS_Synchronization
{
    public class SyncManager
    {
        #region variables
        //static Config configuration;
        public static bool LogEnabled = false;
        internal static long LogOrder = 0;
        private static string DateToSync = "";
        internal static long GetLogOrder()
        {
            return LogOrder++;
        }
        static SyncResult _result = new SyncResult();
        static ConnectionStringsSettings _connectionSettings = new ConnectionStringsSettings();
        #endregion

        /// <summary>
        /// Sync
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="token"></param>
        /// <param name="config"></param>
        /// <returns>SyncResult</returns>


        public static SyncResult Sync(string Operation)
        {

            #region initialization
            _connectionSettings = new ConnectionStringsSettings();
            _result = new SyncResult();
            LogOrder = 0;

            #endregion

            #region connection
            ConnectionStringsSettings connectionSettings = new ConnectionStringsSettings();
            //connectionSettings.SQlConnection = "data source=sql5041.site4now.net;persist security info=True;user id=DB_A45413_DMSERP_admin;password=gouda2003;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=600";
            //connectionSettings.SQlConnection = "data source=171.0.1.93;Database=DB_A45413_DMSERP1;persist security info=True;user id=sa;password=gouda2003;MultipleActiveResultSets=True;App=EntityFramework";
            //connectionSettings.SQlConnection = "data source=72.52.116.106;Database=DB_A45413_DMSERP;persist security info=True;user id=sa;password=gouda2003;MultipleActiveResultSets=True;App=EntityFramework";

            connectionSettings.SQlConnection = "data source=72.52.116.106;Database=DB_A45413_DMSERP;persist security info=True;user id=dms_abdallah; password =Aya1995@DmS;MultipleActiveResultSets=True;App=EntityFramework";
            //connectionSettings.SQlConnection = "data source=171.0.1.93;Database=DB_A45413_DMSERP;persist security info=True;user id=sa; password =123;MultipleActiveResultSets=True;App=EntityFramework";
            //connectionSettings.SQlConnection = "data source=72.52.116.106;Database=DMSERP_develop;persist security info=True;user id=sa;password=gouda2003;MultipleActiveResultSets=True;App=EntityFramework";
            //connectionSettings.SQlConnection = "data source=.\\;Database=TEST;persist security info=True;user id=sa;password=123;MultipleActiveResultSets=True;App=EntityFramework";
            connectionSettings.OrcaleConnection = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 72.52.116.106)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = dms_test; Password = ***";
            connectionSettings.OrcaleConnectionApp = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 72.52.116.106)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = APP; Password = 12369";
            connectionSettings.OrcaleConnectionApp129 = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 196.221.203.129)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = APP; Password = 12369";
            connectionSettings.OrcaleConnectionSH = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 196.221.203.129)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = SH_01; Password = ***";
            connectionSettings.OrcaleConnectionTRN_SQL = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 196.221.203.129)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = TRN_SQL; Password = ***";
            //connectionSettings.OrcaleConnectionTRN_SQL = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 72.52.116.106)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = APP; Password = 12369";
            //connectionSettings.OrcaleConnectionSH65 = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 62.210.148.165)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = SH_01; Password = ***";
            connectionSettings.OrcaleConnectionSH65 = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = 72.52.116.106)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDECATED)(SERVICE_NAME = ora11g))); User Id = SH_01; Password = ***";

            if (connectionSettings.SQlConnection == null)
            {
                throw new ConnectionStringNullException("SQlConnection");
            }
            if (connectionSettings.OrcaleConnection == null)
            {
                throw new ConnectionStringNullException("OrcaleConnection");
            }
            _connectionSettings = connectionSettings;
            #endregion

            #region push
            // Sync from Sql db to backoffice Orcale 

            if (Operation.ToLower() == "push")
            {
                PushRoshita();
                PushRoshitaDetails();
                PushLabDetails();
                PushRayDetails();
                UpdatePushRoshita();
                UpdatePushRoshitaDetails();
                UpdatePushLabDetails();
                UpdatePushRayDetails();

                #region Push MedCard and MedMedicine

                //PushMedCard();
                //PushMedMedicine();
                //UpdateMedCard();
                //UpdateMedMedicine();

                #endregion

                //_currenctConnectionString = connectionSettings.SQlConnection;


                //var whereStatement = $"(IsSync = 0 or IsSync is null) and IsDeleted = 0";

                #region old
                // users
                //SyncUsers();

                //var allSEUsers = GetTable<Models.User>("select * from Users");
                //_result.Logs.Add(new ViewModels.Log
                //{
                //    Order = GetLogOrder(),
                //    Action = SyncAction.Get.ToString(),
                //    Database = GetDatabaseName(),
                //    Server = GetServerName(),
                //    Table = "Users",
                //    Note = "All",
                //    AffectedRows = allSEUsers.Count
                //});
                //var allBOUsers = GetTable<BackModels.Users>("select * from Users", _connectionSettings.OrcaleConnection);
                //_result.Logs.Add(new ViewModels.Log
                //{
                //    Order = GetLogOrder(),
                //    Action = SyncAction.Get.ToString(),
                //    Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                //    Server = GetServerName(_connectionSettings.OrcaleConnection),
                //    Table = "Users",
                //    Note = "All",
                //    AffectedRows = allBOUsers.Count
                //});

                ////customers
                //var customersRespose = SyncTableBack<Models.CUSTOMERS_CODING, BackModels.CUSTOMERS_CODING>(
                //    "CUSTOMERS_CODING", whereStatement, "Id", "Code", "CUSTOMERS_CODING", customer => new BackModels.CUSTOMERS_CODING
                //    {
                //        CUSTOMER_ID = customer.Id,
                //        CUSTOMER_CODE = customer.Code,
                //        CUSTOMER_NAME = customer.Name,
                //        CUSTOMER_ADDRESS = customer.Address,
                //        CUSTOMER_PHONE = customer.Phome,
                //        CUSTOMER_MOBILE = customer.Mobile,
                //        CUSTOMER_Mobile2 = "",
                //        CUSTOMER_TYPE = customer.CUSTOMER_TYPE.ToString(),
                //        CUSTOMER_PHONE2 = "",
                //        CUSTOMER_PHONE3 = "",
                //        CUSTOMER_ADDRESS2 = customer.Address2,
                //        CUSTOMER_Mofoed1 = customer.Mofoed1,
                //        CUSTOMER_Mofoed2 = customer.Mofoed2,
                //        CUSTOMER_Value = customer.Value,
                //        CUSTOMER_Remarks = customer.Remarks,
                //        CUSTOMER_Account = customer.Account,
                //        CUSTOMER_STOP = customer.Stop,
                //        CURRENCY_TYPE = customer.CurrencyType,
                //        Account_TYPE = "1",
                //        Discount = "0",
                //        CUSTOMER_NAME_EN = customer.EnName,
                //        RollCode = customer.RollCode,
                //        PatientPercent = customer.PatientPercent,
                //        PatientParentPercent = customer.PatientParentPercent,
                //        PatientWifePercent = customer.PatientWifePercent,
                //        IsDeleted = false,
                //        IsDefault = customer.IsDefault,
                //        UserCode = customer.UserCode,
                //        EnterTime = customer.EnterDate.ToShortTimeString(),
                //        EnterDate = customer.EnterDate.ToString(configuration.DateFormat),
                //        IsOpened = customer.IsOpened,
                //        IsPosted = customer.IsPosted,
                //        CUSTOMERS_CATEGORIES_Code = customer.CategoryCode,
                //        EName = customer.EnName,
                //        Customer_Group_ID = customer.CustomerGroupId,
                //        IsLimit = customer.IsLimit,
                //        LimitType = customer.LimitType,
                //        TaxType = customer.TaxType,
                //        TermsCondition = customer.TermsCondition,
                //        IFN = customer.IFN,
                //        ICE = customer.ICE,
                //        EMP_ID = customer.EmpId,
                //        SOPaymentToInvoice = customer.SOPaymentToInvoice,
                //        CashOnly = customer.CashOnly,
                //        IsTerms = customer.IsTerms,
                //        Terms = customer.Terms,
                //        DiscountQuantity = customer.DiscountQuantity,
                //        Latitude = customer.Latitude != null ? customer.Latitude.ToString() : null,
                //        Longitude = customer.Longitude != null ? customer.Longitude.ToString() : null,
                //        GoogleAddress = customer.GoogleAddress,
                //    });

                //var allSECustomers = GetTable<Models.CUSTOMERS_CODING>("select * from CUSTOMERS_CODING");
                //_result.Logs.Add(new ViewModels.Log
                //{
                //    Order = GetLogOrder(),
                //    Action = SyncAction.Get.ToString(),
                //    Database = GetDatabaseName(),
                //    Server = GetServerName(),
                //    Table = "CUSTOMERS_CODING",
                //    Note = "All",
                //    AffectedRows = allSECustomers.Count
                //});
                //var allBOCustomers = GetTable<BackModels.CUSTOMERS_CODING>("select * from CUSTOMERS_CODING", _connectionSettings.OrcaleConnection);
                //_result.Logs.Add(new ViewModels.Log
                //{
                //    Order = GetLogOrder(),
                //    Action = SyncAction.Get.ToString(),
                //    Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                //    Server = GetServerName(_connectionSettings.OrcaleConnection),
                //    Table = "CUSTOMERS_CODING",
                //    Note = "All",
                //    AffectedRows = allBOCustomers.Count
                //});

                //whereStatement = $"(IsSync = 0 or IsSync is null) and IsDeleted = 0";

                //// SELLING_MAIN
                //var salesInvoicesResponse = SyncTableBack<REX_SalesInvoiceMain, BackModels.SELLING_MAIN>(
                //        "REX_SalesInvoiceMain", whereStatement, "Id", "SalesInvoiceReference", "SELLING_MAIN", (salesInvoice => new BackModels.SELLING_MAIN
                //        {
                //            REPORT_ID = 0,
                //            REPORT_NUMBER = salesInvoice.SalesInvoiceReference,
                //            TOTAL_MONEY_PAID = 0,
                //            TOTAL_MONEY_REMAIN = salesInvoice.TotalInvoiceValue,
                //            SELLING_TYPE = salesInvoice.SalesInvoiceType.ToString(),
                //            SELLING_DATE = salesInvoice.SalesInvoiceDate.HasValue ? salesInvoice.SalesInvoiceDate.Value.ToString(configuration.DateFormat) : null,
                //            TOTAL_TAX = salesInvoice.VATaxValue,
                //            TOTAL_DISCOUNT = salesInvoice.TotalDiscountValue,
                //            PRICE_TYPE = salesInvoice.PriceType.HasValue ? salesInvoice.PriceType.Value.ToString() : null,
                //            CUSTOMER_TYPE = "0",
                //            NUMBER = GetCustomer(salesInvoice.CustomerId, allSECustomers).Name,
                //            SALLER_ID = GetUserId(salesInvoice.UserId),
                //            CUSTOMER_ID = GetCustomer(salesInvoice.CustomerId, allSECustomers).Synced_ID,
                //            BRANCH_ID = salesInvoice.BranchId,
                //            SELLING_STORE_ID = salesInvoice.StoreId,
                //            KhaznaID = 0,
                //            ATSTaxValue = salesInvoice.ATSTaxValue,
                //            PostTaxValue = salesInvoice.PostTaxValue,
                //            TotalInvoiceValue = salesInvoice.TotalInvoiceValue,
                //            DueDate = salesInvoice.SalesInvoiceDueDate.HasValue ? salesInvoice.SalesInvoiceDueDate.Value.ToString(configuration.DateFormat) : null,
                //            Remark = salesInvoice.SalesInvoiceRemarks,
                //            FrieghtBy = (short?)salesInvoice.FrieghtBy,
                //            applyTax = true,
                //            applyFrieght = false,
                //        //  SalesOrderNumber = null, //Todo if SalesOrderId != null=> get order refrance from ID else Null
                //        SalesOrderNumber = salesInvoice.SalesOrderId != 0 ? salesInvoice.SalesInvoiceReference : null,
                //            UseOrder = salesInvoice.SalesOrderId != 0 ? true : false,

                //            ContactPerson = salesInvoice.ContactPerson,
                //            ContactMobile = salesInvoice.ContactMobile,
                //            ShipPerson = salesInvoice.ShipPerson,
                //            ShipMobile = salesInvoice.ShipMobile,
                //            ConvertFactor = salesInvoice.ConvertFactor,
                //            Currency = salesInvoice.Currency,

                //            IsImportant = salesInvoice.IsImportant,
                //            ImportantUser = (short?)salesInvoice.ImportantUserId,
                //            ImportantDate = salesInvoice.ImportantDate.HasValue ? salesInvoice.ImportantDate.Value.ToString(configuration.DateFormat) : null,
                //            IsReviewed = salesInvoice.IsReviewed,
                //            ReviewUser = (short?)salesInvoice.ReviewUserId,
                //            ReviewDate = salesInvoice.ReviewDate.HasValue ? salesInvoice.ReviewDate.Value.ToString(configuration.DateFormat) : null,

                //            SyncDate = DateTime.Now.ToString(configuration.DateFormat),

                //            LeadgerBook_CODE = 0,
                //            applyDiscount = true,
                //            TransactionName = salesInvoice.TransactionName,
                //            REGPatientAccountCode = null,
                //            SyncInvoice = null,
                //            applyMultiStors = false,
                //            applyPurchasing = false,
                //            applySerialNumber = false,
                //            AddBySerial = false,
                //            VATaxPercent = salesInvoice.VATaxPercent,
                //            VATaxValue = salesInvoice.VATaxValue,
                //            DailyType = salesInvoice.DailyType,
                //            DailyNum = salesInvoice.DailyNum,
                //            MHT = salesInvoice.MHT
                //        }),
                //        true, "REPORT_NUMBER");

                //if (salesInvoicesResponse.SEData.Any())
                //{
                //    whereStatement = $"SalesInvoiceId in ({GetJoinedString(salesInvoicesResponse.SEData.Select(e => e.Id).ToList())})";

                //    // SELLING_DETAILS
                //    SyncTableBack<REX_SalesInvoiceDetails, BackModels.SELLING_DETAILS>("REX_SalesInvoiceDetails", whereStatement, null, null, "SELLING_DETAILS", salesInvoiceDetails => new BackModels.SELLING_DETAILS
                //    {

                //        EXPIREDATE = "",
                //        SerialNo = "",
                //        CostCenter = "",

                //        ITEM_Balance = salesInvoiceDetails.ItemBalance,
                //        Remarks = salesInvoiceDetails.Remarks,
                //        ITEM_Credit_VALUE = salesInvoiceDetails.ItemCreditValue,
                //        CRMGroup_ID = (short?)salesInvoiceDetails.CRMGroupID,
                //        CRMLabs_ID = (short?)salesInvoiceDetails.CRMLabsID,


                //    });
                #endregion
                #region old2
                //}


                //    // CHECKS_COLLECTIONS
                //    var x = SyncTableBack<REX_Check, BackModels.CHECKS_COLLECTIONS>("REX_Check", whereStatement, "Id", "CheckReference", "CHECKS_COLLECTIONS", check => new BackModels.CHECKS_COLLECTIONS
                //    {
                //        ID = 0,
                //        ACCOUNT_ID = check.AccountId,
                //        CostCenter = "",

                //        SyncDate = DateTime.Now.ToString(configuration.DateFormat),

                //        SyncInvoice = null,
                //    });



                //    //Vacation
                //    whereStatement = $"(IsSync = 0 or IsSync is null)and CurrentStatus=1";
                //    SyncTableBack<LSS_Vacations, BackModels.EMP_Vacations>("LSS_Vacations", whereStatement, "Id", "SKU", "EMP_Vacations", LSS_Vacation => new BackModels.EMP_Vacations
                //    {
                //        //ID = 0,
                //        EMP_Vacation_Id = 0,
                //        SKU = LSS_Vacation.SKU,
                //        EMP_ID = GetEmpId(LSS_Vacation.EmployeeId),
                //        DateFrom = LSS_Vacation.DateFrom.ToString("dd/MM/yyyy"),
                //        SyncDate = DateTime.Now.ToString(configuration.DateFormat),

                //    });
                #endregion
            }
            #endregion

            #region MedCard and MedMedicine

            if (Operation.ToLower() == "med2" || Operation.ToLower() == "both")
            {


                _currenctConnectionString = connectionSettings.SQlConnection;
                SyncToSqlTableSH<Med_Card>(StringHelper.GetQyertMED_CARD, StringHelper.GetTableNameMED_CARD);
                SyncToSqlTableSH<Med_Medicine>(StringHelper.GetQyertMED_MEDICINE, StringHelper.GetTableNameMED_MEDICINE);

                UpdateToSqlTableSH<Med_Card>(StringHelper.GetTableNameMED_CARD);
                UpdateToSqlTableSH<Med_Medicine>(StringHelper.GetTableNameMED_MEDICINE);

            }

            #endregion


            #region Push MedCard and MedMedicine

            if (Operation.ToLower() == "pushmed" || Operation.ToLower() == "both")
            {

                PushMedCard();
                PushMedMedicine();
                //UpdateMedCard();
                //UpdateMedMedicine();


            }

            #endregion


            #region Sh_01

            if (Operation.ToLower() == "sh" || Operation.ToLower() == "both")
            {
                //PullUsers();
                //PullRoles();
                //PullMan();
                PullRoshita();
                PullRoshitaDetails();
                UpdatePullRoshita();

            }

            #endregion


            #region Pull DMS
            //From backoffice Orcale To Sql
            if (Operation.ToLower() == "pull" || Operation.ToLower() == "both")
            {

                _currenctConnectionString = connectionSettings.SQlConnection;

                SyncToSqlTable<CONSUMPTION_POOL>(StringHelper.GetQyertCONSUMPTION_POOL, StringHelper.GetTableNameCONSUMPTION_POOL);
                SyncToSqlTable<REMAIN_CONSUMATION>(StringHelper.GetQyertREMAIN_CONSUMATION, StringHelper.GetTableNameREMAIN_CONSUMATION);
                CLOSE_EMP_DATASyncToSqlTable();
                SyncToSqlTable<COMP_CUSTOMIZED_D_D_MED>(StringHelper.GetQyertCOMP_CUSTOMIZED_D_D_MED, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D_MED);
                SyncToSqlTable<COMP_CUSTOMIZED_D_D_MED_EMP>(StringHelper.GetQyertCOMP_CUSTOMIZED_D_D_MED_EMP, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D_MED_EMP);

                PushRoshitaDiagnosisAdmin();
                //PullUsers();
                //PullRoles();

                SyncToSqlTable<APPROVAL_BAD>(StringHelper.GetQyertAPPROVAL_BAD, StringHelper.GetTableNameAPPROVAL_BAD);
                SyncToSqlTable<DMS_02_EMP_D_ENT_MAN>(StringHelper.GetQyertDMS_02_EMP_D_ENT_MAN, StringHelper.GetTableNameDMS_02_EMP_D_ENT_MAN);

                SyncToSqlTable<CompContractClassEmp>(StringHelper.GetQyertCOMP_CONTRACT_CLASS_EMP, StringHelper.GetTableNameCOMP_CONTRACT_CLASS_EMP);

                SyncToSqlTable<PollDataPreX>(StringHelper.GetQyertPOLL_DATA_PREX, StringHelper.GetTableNamePOLL_DATA_PREX);
                SyncToSqlTable<PollDataExcepitions>(StringHelper.GetQyertPOLL_DATA_EXCEPTIONS, StringHelper.GetTableNamePOLL_DATA_EXCEPTIONS);
                SyncToSqlTable<PollDataDiag>(StringHelper.GetQyertPOLL_DATA_DIAG, StringHelper.GetTableNamePOLL_DATA_DIAG);
                SyncToSqlTable<PollDataChronic>(StringHelper.GetQyertPOLL_DATA_CHRONIC, StringHelper.GetTableNamePOLL_DATA_CHRONIC);
                SyncToSqlTable<PollAmountCard>(StringHelper.GetQyertPOLL_AMOUNT_CARD, StringHelper.GetTableNamePOLL_AMOUNT_CARD);
                SyncToSqlTable<PollAmount>(StringHelper.GetQyertPOLL_AMOUNT, StringHelper.GetTableNamePOLL_AMOUNT);
                SyncToSqlTable<PollPercentCard>(StringHelper.GetQyertPOLL_PERCENT_CARD, StringHelper.GetTableNamePOLL_PERCENT_CARD);
                SyncToSqlTable<PollPercent>(StringHelper.GetQyertPOLL_PERCENT, StringHelper.GetTableNamePOLL_PERCENT);
                SyncToSqlTable<PollDataService>(StringHelper.GetQyertPOLL_DATA_SERVICE, StringHelper.GetTableNamePOLL_DATA_SERVICE);
                SyncToSqlTable<PollData>(StringHelper.GetQyertPOLL_DATA, StringHelper.GetTableNamePOLL_DATA);


                PushMedicineData();
                UpdatePushMedicineData();
                PushMedicineGroup();
                #region DMS_Test
                //PushSqlTables<MEDICINE_DATA>(StringHelper.GetQyertMedicineData, StringHelper.GetTableNameMedicineData);
                //PushSqlTables<MedicineGroup>(StringHelper.GetQyertMedicineGroup, StringHelper.GetTableNameMedicineGroup);



                SyncToSqlTable<Co_Insurance_01>(StringHelper.GetQyertCO_INSURANCE_01, StringHelper.GetTableNameCO_INSURANCE_01);
                SyncToSqlTable<SER_PROV_DISC>(StringHelper.GetQyertSER_PROV_DISC, StringHelper.GetTableNameSER_PROV_DISC);
                SyncToSqlTable<SERVICES>(StringHelper.GetQyertSERVICES, StringHelper.GetTableNameSERVICES);
                SyncToSqlTable<SERV_PROVIDERS_NEW>(StringHelper.GetQyertSERV_PROVIDERS_NEW, StringHelper.GetTableNameSERV_PROVIDERS_NEW);

                SyncToSqlTable<Comp_Employees>(StringHelper.GetQyertCOMP_EMPLOYEES, StringHelper.GetTableNameCOMP_EMPLOYEES);
                SyncToSqlTable<Contract_Comp>(StringHelper.GetQyertCONTRACT_COMP, StringHelper.GetTableNameCONTRACT_COMP);
                SyncToSqlTable<Contract_Data>(StringHelper.GetQyertCONTRACT_DATA, StringHelper.GetTableNameCONTRACT_DATA);
                SyncToSqlTable<Pr_Bra>(StringHelper.GetQyertPR_BRA, StringHelper.GetTableNamePR_BRA);
                //SyncToSqlTable<Diagnosis>(StringHelper.GetQyertDIAGNOSES, StringHelper.GetTableNameDIAGNOSES);
                SyncToSqlTable<CompContractClass>(StringHelper.GetQyertCOMP_CONTRACT_CLASS, StringHelper.GetTableNameCOMP_CONTRACT_CLASS);
                SyncToSqlTable<Serv_Providers>(StringHelper.GetQyertSERV_PROVIDERS, StringHelper.GetTableNameSERV_PROVIDERS);
                SyncToSqlTable<Basic_Data>(StringHelper.GetQyertBASIC_DATA, StringHelper.GetTableNameBASIC_DATA);
                SyncToSqlTable<MedicineData>(StringHelper.GetQyertMEDICINE_DATA, StringHelper.GetTableNameMEDICINE_DATA);
                SyncToSqlTable<Comp_Customized_D_D>(StringHelper.GetQyertCOMP_CUSTOMIZED_D_D, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D);
                SyncToSqlTable<COMP_CUSTOMIZED_D>(StringHelper.GetQyertCOMP_CUSTOMIZED_D, StringHelper.GetTableNameCOMP_CUSTOMIZED_D);
                SyncToSqlTable<COMP_CUSTOMIZED_D_D_EMP>(StringHelper.GetQyertCOMP_CUSTOMIZED_D_D_EMP, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D_EMP);
                SyncToSqlTable<COMP_CUSTOMIZED_D_EMP>(StringHelper.GetQyertCOMP_CUSTOMIZED_D_EMP, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_EMP);
                #endregion 


                #region Update DMS TEST

                UpdateSqlTable<REMAIN_CONSUMATION>(StringHelper.GetUpdateQyertREMAIN_CONSUMATION, StringHelper.GetTableNameREMAIN_CONSUMATION, "RemainConsumption");
                UpdateSqlTable<COMP_CUSTOMIZED_D_D_MED>(StringHelper.GetUpdateQyertCOMP_CUSTOMIZED_D_D_MED, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D_MED, "COMP_CUSTOMIZED_D_D_MED");
                UpdateSqlTable<COMP_CUSTOMIZED_D_D_MED_EMP>(StringHelper.GetUpdateQyertCOMP_CUSTOMIZED_D_D_MED_EMP, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D_MED_EMP, "COMP_CUSTOMIZED_D_D_MED_EMP");
                UpdateSqlTable<DMS_02_EMP_D_ENT_MAN>(StringHelper.GetUpdateQyertDMS_02_EMP_D_ENT_MAN, StringHelper.GetTableNameDMS_02_EMP_D_ENT_MAN, "DMS_02_EMP_D_ENT_MAN");
                UpdateSqlTable<CompContractClassEmp>(StringHelper.GetUpdateQyertCOMP_CONTRACT_CLASS_EMP, StringHelper.GetTableNameCOMP_CONTRACT_CLASS_EMP, "CompContractClassEmp");
                UpdateSqlTable<APPROVAL_BAD>(StringHelper.GetUpdateQyertAPPROVAL_BAD, StringHelper.GetTableNameAPPROVAL_BAD, "APPROVAL_BAD");
                UpdateSqlTable<PollDataPreX>(StringHelper.GetUpdateQyertPOLL_DATA_PREX, StringHelper.GetTableNamePOLL_DATA_PREX, "PollDataPreX");
                UpdateSqlTable<PollDataExcepitions>(StringHelper.GetUpdateQyertPOLL_DATA_EXCEPTIONS, StringHelper.GetTableNamePOLL_DATA_EXCEPTIONS, "PollDataExcepitions");
                UpdateSqlTable<PollDataDiag>(StringHelper.GetUpdateQyertPOLL_DATA_DIAG, StringHelper.GetTableNamePOLL_DATA_DIAG, "PollDataDiag");
                UpdateSqlTable<PollDataChronic>(StringHelper.GetUpdateQyertPOLL_DATA_CHRONIC, StringHelper.GetTableNamePOLL_DATA_CHRONIC, "PollDataChronic");
                UpdateSqlTable<PollAmountCard>(StringHelper.GetQyertPOLL_AMOUNT_CARD, StringHelper.GetTableNamePOLL_AMOUNT_CARD, "PollAmountCard");


                UpdateSqlTable<Co_Insurance_01>(StringHelper.GetUpdateQyertCO_INSURANCE_01, StringHelper.GetTableNameCO_INSURANCE_01, "Co_Insurance_01");
                UpdateSqlTable<Contract_Comp>(StringHelper.GetUpdateQyertCONTRACT_COMP, StringHelper.GetTableNameCONTRACT_COMP, "Contract_Comp");
                UpdateSqlTable<Contract_Data>(StringHelper.GetUpdateQyertCONTRACT_DATA, StringHelper.GetTableNameCONTRACT_DATA, "Contract_Data");
                UpdateSqlTable<Pr_Bra>(StringHelper.GetUpdateQyertBASIC_DATA, StringHelper.GetTableNameBASIC_DATA, "Basic_Data");
                //UpdateSqlTable<Diagnosis>(StringHelper.GetUpdateQyertPR_BRA, StringHelper.GetTableNamePR_BRA, "Pr_Bra");
                UpdateSqlTable<CompContractClass>(StringHelper.GetUpdateQyertCOMP_CONTRACT_CLASS, StringHelper.GetTableNameCOMP_CONTRACT_CLASS, "CompContractClass");
                UpdateSqlTable<Serv_Providers>(StringHelper.GetUpdateQyertSERV_PROVIDERS, StringHelper.GetTableNameSERV_PROVIDERS, "Serv_Providers1");
                UpdateSqlTable<Basic_Data>(StringHelper.GetUpdateQyertBASIC_DATA, StringHelper.GetTableNameBASIC_DATA, "Basic_Data");
                UpdateSqlTable<MedicineData>(StringHelper.GetUpdateQyertMEDICINE_DATA, StringHelper.GetTableNameMEDICINE_DATA, "MedicineData");
                UpdateSqlTable<Comp_Customized_D_D>(StringHelper.GetUpdateQyertCOMP_CUSTOMIZED_D_D, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D, "Comp_Customized_D_D");
                UpdateSqlTable<COMP_CUSTOMIZED_D>(StringHelper.GetUpdateQyertCOMP_CUSTOMIZED_D, StringHelper.GetTableNameCOMP_CUSTOMIZED_D, "COMP_CUSTOMIZED_D");
                UpdateSqlTable<Comp_Employees>(StringHelper.GetUpdateQyertCOMP_EMPLOYEES, StringHelper.GetTableNameCOMP_EMPLOYEES, "Comp_Employees");
                UpdateSqlTable<COMP_CUSTOMIZED_D_D_EMP>(StringHelper.GetUpdateQyertCOMP_CUSTOMIZED_D_D_EMP, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_D_EMP, "COMP_CUSTOMIZED_D_D_EMP");
                UpdateSqlTable<COMP_CUSTOMIZED_D_EMP>(StringHelper.GetUpdateQyertCOMP_CUSTOMIZED_D_EMP, StringHelper.GetTableNameCOMP_CUSTOMIZED_D_EMP, "COMP_CUSTOMIZED_D_EMP");

                #endregion

            }

            #endregion

            _result.Succeeded = _result.Errors.Count == 0;
            _result.SucceededWithErrors = _result.Errors.Count > 0;
            return _result;
        }

        #region core functions
        private static string _currenctConnectionString = null;
        private static int? _batchSize = null;

        public static void SetBatchSize(int? batchSize)
        {
            _batchSize = batchSize;
        }

        private static DataTable GetDataTable(string query, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            if (connectionString == null)
            {
                throw new Exception("connectionString is null and you didn't pass connection as a parameter! You must set connectionString");
            }

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 120;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new ConnectionStringInvalidException();
            }

            return dt;
        }

        private static DataTable GetOracleDataTable(string query, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            if (connectionString == null)
            {
                throw new Exception("connectionString is null and you didn't pass connection as a parameter! You must set connectionString");
            }

            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        //cmd.CommandTimeout = 120;
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }

            return dt;
        }


        private static int ExecuteNonQueryCommand(string query, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            if (connectionString == null)
            {
                throw new Exception("connectionString is null and you didn't pass connection as a parameter! You must set connectionString");
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private static int ExecuteOracleNonQueryCommand(string query, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            if (connectionString == null)
            {
                throw new Exception("connectionString is null and you didn't pass connection as a parameter! You must set connectionString");
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private static List<T> GetFirstColumnValues<T>(DataTable datatable)
        {
            return (from DataRow dataRow in datatable.Rows
                    select (T)dataRow[0]).ToList();
        }


        #region Varibles
        public static List<int> MaxIDs = new List<int>();
        public static List<int> MaxOrderIDs = new List<int>();
        public static List<int> MaxInvoiceIDs = new List<int>();
        public static List<int> MaxReturnIDs = new List<int>();
        public static List<int> MaxQuotationIDs = new List<int>();
        public static List<int> MaxReceiptIDs = new List<int>();
        public static List<int> MaxCheckIDs = new List<int>();
        public static List<int> MaxCreditMemoIDs = new List<int>();
        public static List<int> MaxSandLinkIDs = new List<int>();
        #endregion

        private static DataTable ToDataTable<T>(List<T> items, DataTable dataTable)
        {
            try
            {
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (T item in items)
                {

                    var row = dataTable.NewRow();
                    foreach (var prop in Props)
                    {
                        // get the right order of the column
                        //inserting property values to datatable rows

                        var value = prop.GetValue(item, null);
                        row[dataTable.Columns[prop.Name]] = value == null ? DBNull.Value : value;


                    }
                    dataTable.Rows.Add(row);


                }

            }
            catch (Exception e)
            {
                var esw = e.InnerException;

            }

            //put a breakpoint here and check datatable
            return dataTable;


        }

        private static DataTable GetDataTableSchemaFromTable(string tableName, DataTable dataTable = null, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            if (dataTable == null)
            {
                dataTable = new DataTable();
            }

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = sqlConn.CreateCommand())
                    {
                        command.CommandText = String.Format("SELECT * FROM {0} where 1 = 0", tableName);
                        command.CommandType = CommandType.Text;
                        sqlConn.Open();

                        SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly);

                        dataTable.Load(reader);
                        sqlConn.Close();
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            return dataTable;
        }


        private static void AddNewEntities<T>(List<T> list, string tableName, bool keepIdentity = false, string connectionString = null)
        {
            try
            {
                connectionString = connectionString == null ? _currenctConnectionString : connectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable dataTable = new DataTable();

                    GetDataTableSchemaFromTable(tableName, dataTable, connectionString);
                    try
                    {
                        ToDataTable(list, dataTable);
                    }
                    catch (Exception ex)
                    {
                        var message = ex.Message;
                    }

                    var options = GetDefaultSyncOptions();
                    if (keepIdentity)
                    {
                        options = GetSyncOptionsWithKeepIdentity();
                    }
                    using (var sqlBulk = new SqlBulkCopy(connectionString, options))
                    {
                        if (_batchSize.HasValue)
                        {
                            sqlBulk.BatchSize = _batchSize.Value;
                        }
                        sqlBulk.DestinationTableName = tableName;
                        sqlBulk.WriteToServer(dataTable);


                    }
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        private static SqlBulkCopyOptions GetDefaultSyncOptions()
        {
            return SqlBulkCopyOptions.FireTriggers |
                            SqlBulkCopyOptions.CheckConstraints |
                            SqlBulkCopyOptions.KeepNulls;
        }

        private static SqlBulkCopyOptions GetSyncOptionsWithKeepIdentity()
        {
            return SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.FireTriggers |
                            SqlBulkCopyOptions.CheckConstraints |
                            SqlBulkCopyOptions.KeepNulls;
        }

        private static bool CheckIfColumnExists(string tableName, string columnName, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;
            var query = $"SELECT * FROM sys.columns WHERE [name] = N'{columnName}' AND [object_id] = OBJECT_ID(N'{tableName}')";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var count = cmd.ExecuteScalar();
                    con.Close();
                    return count != null;
                }
            }
        }

        private static void AddNewColumn(string tableName, string columnName, string dataType, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;
            var query = $"alter table [{tableName}] add [{columnName}] {dataType}";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        private static int DeleteTable(string tableName, string connectionString = null)
        {
            var query = "delete from " + tableName;
            return ExecuteNonQueryCommand(query, connectionString);
        }

        private static List<T> GetTable<T>(string query, string connectionString = null)
        {
            var datatable = GetDataTable(query, connectionString);
            var list = AutoMapper.Mapper.DynamicMap<IDataReader,
                List<T>>(datatable.CreateDataReader());
            return list;
        }
        private static List<T> GetOracleTable<T>(string query, string connectionString = null)
        {
            var datatable = GetOracleDataTable(query, connectionString);
            try
            {
                var list = AutoMapper.Mapper.DynamicMap<IDataReader,
                    List<T>>(datatable.CreateDataReader());
                return list;

            }
            catch (Exception e)
            {
                var es = e;
                return null;
            }
        }
        #endregion

        #region functions
        private static void SyncTable<T>(string query, string tableName)
        {
            try
            {

                var data = GetTable<T>(query, _connectionSettings.OrcaleConnection);
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                    Server = GetServerName(_connectionSettings.OrcaleConnection),
                    Table = tableName,
                    Note = "All",
                    AffectedRows = data.Count
                });
                if (tableName == "Users")
                    tableName = "Users_ERP";
                try
                {
                    var count = DeleteTable(tableName);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Delete.ToString(),
                        Database = GetDatabaseName(),
                        Server = GetServerName(),
                        Table = tableName,
                        Note = "All",
                        AffectedRows = count
                    });
                }
                catch (Exception ex)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Delete.ToString(),
                        Database = GetDatabaseName(),
                        Server = GetServerName(),
                        Table = tableName,
                        Exception = ex,
                    });
                }

                try
                {
                    AddNewEntities(data, tableName, true);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(),
                        Server = GetServerName(),
                        Table = tableName,
                        AffectedRows = data.Count
                    });
                }
                catch (Exception ex)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(),
                        Server = GetServerName(),
                        Table = tableName,
                        Exception = ex,
                    });
                }
            }
            catch (Exception e)
            {
                _result.Errors.Add(new Error()
                {
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                    Server = GetServerName(_connectionSettings.OrcaleConnection),
                    Table = tableName,
                    Exception = e,
                });
            }
        }
        private static void UpdateSqlTable<T>(string query, string tableName, string SqlTable)
        {

            string conn = "";
            if (tableName == "SH_01.MEDICINE_DATA" || tableName == "SH_01.CO_INSURANCE_01"
                || tableName == "SH_01.SER_PROV_DISC" || tableName == "SH_01.DMS_02_EMP_D_ENT_MAN")
            {
                conn = _connectionSettings.OrcaleConnectionSH65;
            }
            else if (tableName == "APP.SERV_PROVIDERS_NEW" || tableName == "APP.POLL_DATA_PREX" || tableName == "APP.POLL_DATA_EXCEPTIONS"
                || tableName == "APP.POLL_DATA_DIAG" || tableName == "APP.POLL_DATA_CHRONIC" || tableName == "APP.POLL_AMOUNT_CARD"
                || tableName == "APP.COMP_CUSTOMIZED_D_D_MED" || tableName == "APP.COMP_CUSTOMIZED_D_D_MED_EMP" || tableName == "APP.REMAIN_CONSUMATION")
            {
                conn = _connectionSettings.OrcaleConnectionApp;
            }
            else if (tableName == "APP.APPROVAL_BAD")
            {
                conn = _connectionSettings.OrcaleConnectionApp129;
            }
            else
            {
                conn = _connectionSettings.OrcaleConnection;
            }

            tableName = tableName.Split('.')[1];
            try
            {

                #region Test
                var ORACLEdatatable = GetOracleDataTable(query, conn);
                DataTable SQLdataTable = new DataTable();
                GetDataTableSchemaFromTable(SqlTable, SQLdataTable, _connectionSettings.SQlConnection);

                if (ORACLEdatatable.Rows.Count != 0)
                {
                    int iteration = 3;
                    if (SqlTable == "COMP_CUSTOMIZED_D" || SqlTable == "COMP_CUSTOMIZED_D_EMP")
                    {
                        iteration = 6;
                    }
                    else if (SqlTable == "Comp_Customized_D_D" || SqlTable == "COMP_CUSTOMIZED_D_D_EMP")
                    {
                        iteration = 7;
                    }
                    else if (SqlTable == "Co_Insurance_01" /*|| SqlTable== "CompContractClassEmp"*/)
                    {
                        iteration = 4;
                    }
                    for (int i = 0; i < ORACLEdatatable.Rows.Count; i++)
                    {
                        string UpdateQuery = "UPDATE " + SqlTable + " SET ";
                        for (int j = 0; j < ORACLEdatatable.Columns.Count - iteration; j++)
                        {
                            UpdateQuery = UpdateQuery + SQLdataTable.Columns[j + 1].ColumnName + (string.IsNullOrEmpty(ORACLEdatatable.Rows[i][j].ToString()) ? "=NULL," : ("=N'" + ORACLEdatatable.Rows[i][j]) + "', ");

                        }
                        UpdateQuery = UpdateQuery + "IsSync =1 , SyncDate='" + DateTime.Now + "', SyncBy='Admin'";
                        switch (SqlTable)
                        {
                            #region DMS
                            case "Basic_Data":
                                UpdateQuery = UpdateQuery + " WHERE BS_CODE=" + ORACLEdatatable.Rows[i][0] +
                                       " AND SOURCE_MOD='" + ORACLEdatatable.Rows[i][1] + "' AND COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                       " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][3];
                                break;
                            case "Comp_Customized_D_D":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] + " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][3] + " AND SERV_CODE='" + ORACLEdatatable.Rows[i][4] +
                                    "' AND D_SERV_CODE='" + ORACLEdatatable.Rows[i][5] + "' AND CLASS_CODE='" + ORACLEdatatable.Rows[i][6] +
                                    "' AND SER_SERV='" + ORACLEdatatable.Rows[i][7] + "'";
                                break;
                            case "Contract_Comp":
                                UpdateQuery = UpdateQuery + " WHERE C_COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                       " AND COMP_ID=" + ORACLEdatatable.Rows[i][1] + " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][2];
                                break;
                            case "Contract_Data":
                                UpdateQuery = UpdateQuery + " WHERE C_COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][1] + " AND COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][3];
                                break;
                            case "Pr_Bra":
                                UpdateQuery = UpdateQuery + " WHERE PR_CODE=" + ORACLEdatatable.Rows[i][0] +
                                    " AND COMP_ID=" + ORACLEdatatable.Rows[i][1] + " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][2] +
                                    " AND PRV_TYPE=" + ORACLEdatatable.Rows[i][3];
                                break;
                            case "Diagnosis":
                                UpdateQuery = UpdateQuery + " WHERE DIAG_CODE='" + ORACLEdatatable.Rows[i][0] +
                                    "' AND COMP_ID=" + ORACLEdatatable.Rows[i][1] + " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][2];
                                break;

                            case "CompContractClass":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] + " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][3] + " AND CLASS_CODE='" + ORACLEdatatable.Rows[i][4] + "'";
                                break;
                            case "Serv_Providers1":
                                UpdateQuery = UpdateQuery + " WHERE PR_CODE=" + ORACLEdatatable.Rows[i][0] +
                                    " AND COMP_ID=" + ORACLEdatatable.Rows[i][1] + " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][2] +
                                    " AND PRV_TYPE=" + ORACLEdatatable.Rows[i][3];
                                break;

                            case "MedicineData":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] + " AND M_CODE='" + ORACLEdatatable.Rows[i][2] +
                                    "'";
                                break;

                            case "COMP_CUSTOMIZED_D":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] + " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][3] + " AND SERV_CODE='" + ORACLEdatatable.Rows[i][4] +
                                    "' AND CLASS_CODE='" + ORACLEdatatable.Rows[i][5] + "' AND D_SERV_CODE='" + ORACLEdatatable.Rows[i][6] +
                                    "'";
                                break;

                            case "Comp_Employees":
                                UpdateQuery = UpdateQuery + " WHERE CARD_ID='" + ORACLEdatatable.Rows[i][0] +
                                    "' AND COMP_ID=" + ORACLEdatatable.Rows[i][1] + " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][2] +
                                    " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][3] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][4] +
                                    " AND CLASS_CODE='" + ORACLEdatatable.Rows[i][5] + "'";
                                break;
                            case "COMP_CUSTOMIZED_D_D_MED":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] +
                                    " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][3] +
                                    " AND SERV_CODE='" + ORACLEdatatable.Rows[i][4] +
                                    "' AND D_SERV_CODE='" + ORACLEdatatable.Rows[i][5] +
                                    "' AND CLASS_CODE='" + ORACLEdatatable.Rows[i][6] +
                                    "' AND SER_SERV='" + ORACLEdatatable.Rows[i][7] + "'";
                                break;
                            case "COMP_CUSTOMIZED_D_D_MED_EMP":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] +
                                    " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][3] +
                                    " AND SERV_CODE='" + ORACLEdatatable.Rows[i][4] +
                                    "' AND D_SERV_CODE='" + ORACLEdatatable.Rows[i][5] +
                                    "' AND CLASS_CODE='" + ORACLEdatatable.Rows[i][6] +
                                    "' AND SER_SERV='" + ORACLEdatatable.Rows[i][7] +
                                    "' AND CARD_ID='" + ORACLEdatatable.Rows[i][8] + "'";
                                break;
                            case "COMP_CUSTOMIZED_D_D_EMP":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] +
                                    " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][3] +
                                    " AND SERV_CODE='" + ORACLEdatatable.Rows[i][4] +
                                    "' AND D_SERV_CODE='" + ORACLEdatatable.Rows[i][5] +
                                    "' AND CLASS_CODE='" + ORACLEdatatable.Rows[i][6] +
                                    "' AND SER_SERV='" + ORACLEdatatable.Rows[i][7] +
                                    "' AND CARD_ID='" + ORACLEdatatable.Rows[i][8] + "'";
                                break;
                            case "COMP_CUSTOMIZED_D_EMP":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND BRANCH_CODE=" + ORACLEdatatable.Rows[i][1] +
                                    " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] +
                                    " AND CONTRACT_NO=" + ORACLEdatatable.Rows[i][3] +
                                    " AND SERV_CODE='" + ORACLEdatatable.Rows[i][4] +
                                    "' AND CLASS_CODE ='" + ORACLEdatatable.Rows[i][5] +
                                    "' AND D_SERV_CODE ='" + ORACLEdatatable.Rows[i][6] +
                                    "' AND CARD_ID='" + ORACLEdatatable.Rows[i][7] + "'";
                                break;
                            case "Co_Insurance_01":
                                UpdateQuery = UpdateQuery + " WHERE CO_ID=" + ORACLEdatatable.Rows[i][0] +
                                    " AND LIVEL='" + ORACLEdatatable.Rows[i][1] + "'";
                                break;
                            case "Ser_Prov_Disc":
                                UpdateQuery = UpdateQuery + " WHERE PROV_ID=" + ORACLEdatatable.Rows[i][0];
                                break;
                            case "DMS_02_EMP_D_ENT_MAN":
                                UpdateQuery = UpdateQuery + " WHERE CARD_ID='" + ORACLEdatatable.Rows[i][1] + "' AND " +
                                    " RE_SEQ=" + ORACLEdatatable.Rows[i][21] + " AND ID_STOP=" + ORACLEdatatable.Rows[i][38];
                                break;
                            case "CompContractClassEmp":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0] + " AND BRANCH_CODE=" +
                                     ORACLEdatatable.Rows[i][1] + " AND C_COMP_ID=" + ORACLEdatatable.Rows[i][2] + " AND CONTRACT_NO="
                                     + ORACLEdatatable.Rows[i][3] + " AND CLASS_CODE='" + ORACLEdatatable.Rows[i][4] + "' AND " +
                                    " CARD_ID='" + ORACLEdatatable.Rows[i][5] + "'";
                                break;
                            case "RemainConsumption":
                                UpdateQuery = UpdateQuery + " WHERE CARD_ID='" + ORACLEdatatable.Rows[i][0] + "'";
                                break;
                            #endregion
                            case "PollDataPreX":
                                UpdateQuery = UpdateQuery + " WHERE POLL_CODE=" + ORACLEdatatable.Rows[i][0] + " AND PREX_CODE=" +
                                     ORACLEdatatable.Rows[i][1] + " AND ACTIVE='" + ORACLEdatatable.Rows[i][2] + "'";
                                break;

                            case "PollDataExcepitions":
                                UpdateQuery = UpdateQuery + " WHERE POLL_CODE=" + ORACLEdatatable.Rows[i][0] + " AND EXCEPTIONS_CODE=" +
                                     ORACLEdatatable.Rows[i][1] + " AND ACTIVE='" + ORACLEdatatable.Rows[i][2] + "'";
                                break;

                            case "PollDataDiag":
                                UpdateQuery = UpdateQuery + " WHERE POLL_CODE=" + ORACLEdatatable.Rows[i][0] + " AND DIA_CODE=" +
                                     ORACLEdatatable.Rows[i][1] + " AND ACTIVE='" + ORACLEdatatable.Rows[i][2] + "'";
                                break;


                            case "PollDataChronic":
                                UpdateQuery = UpdateQuery + " WHERE POLL_CODE=" + ORACLEdatatable.Rows[i][0] + " AND DIA_CODE=" +
                                     ORACLEdatatable.Rows[i][1] + " AND ACTIVE='" + ORACLEdatatable.Rows[i][2] + "'";
                                break;

                            case "PollAmountCard":
                                UpdateQuery = UpdateQuery + " WHERE POLL_CODE=" + ORACLEdatatable.Rows[i][0] + " AND CARD_ID='" +
                                     ORACLEdatatable.Rows[i][1] + "' AND TYPE_AMOUNT=" + ORACLEdatatable.Rows[i][2];
                                break;

                            case "APPROVAL_BAD":
                                UpdateQuery = UpdateQuery + " WHERE COMP_ID=" + ORACLEdatatable.Rows[i][0];
                                //+ " AND CARD_NO='" +
                                // ORACLEdatatable.Rows[i][1] + "' AND CLASS_CODE='" + ORACLEdatatable.Rows[i][2]
                                // + "' AND FLAG='"+ ORACLEdatatable.Rows[i][4]+ "' AND CODE="+ ORACLEdatatable.Rows[i][7];
                                break;

                            default:
                                break;
                        }
                        ExecuteSQLUpdateQuery(UpdateQuery, _connectionSettings.SQlConnection);
                        UpdateQuery = "";
                    }

                    if (tableName == "COMP_CUSTOMIZED_D_D_EMP")
                    {
                        string queryDDEmp = " update Comp_Customized_D_D_Emp set CEILING_AMT = null WHERE CEILING_AMT = 0";
                        ExecuteSQLUpdateQuery(queryDDEmp, _connectionSettings.SQlConnection);
                    }
                    if (tableName == "COMP_CUSTOMIZED_D_EMP")
                    {
                        string queryDEMP = "update COMP_CUSTOMIZED_D_EMP set CEILING_AMT = null WHERE CEILING_AMT = 0";
                        ExecuteSQLUpdateQuery(queryDEMP, _connectionSettings.SQlConnection);
                    }
                    if (tableName == "COMP_CUSTOMIZED_D_D")
                    {
                        string queryDD = "update Comp_Customized_D_D set CEILING_AMT = null WHERE CEILING_AMT = 0";
                        ExecuteSQLUpdateQuery(queryDD, _connectionSettings.SQlConnection);
                    }
                    if (tableName == "COMP_CUSTOMIZED_D")
                    {
                        string queryD = "update Comp_Customized_D set CEILING_AMT = null WHERE CEILING_AMT = 0";
                        ExecuteSQLUpdateQuery(queryD, _connectionSettings.SQlConnection);
                    }
                    if (tableName == "MEDICINE_DATA")
                    {
                        string queryD = " UPDATE MedicineData SET diagnoisegender = 3, iscovered = 1, DiagnoiseAge = 'All'";
                        ExecuteSQLUpdateQuery(queryD, _connectionSettings.SQlConnection);
                        string queryDa = " UPDATE MedicineData SET ACTIVE = 'Y' WHERE ACTIVE IS NULL";
                        ExecuteSQLUpdateQuery(queryDa, _connectionSettings.SQlConnection);
                    }

                    string OracleQuery = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=1 and SYNC_BY = 'UPDATE'";
                    ExecuteOracleQuery(OracleQuery, conn);

                }
                #endregion

            }
            catch (Exception e)
            {
                _result.Errors.Add(new Error()
                {
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                    Server = GetServerName(_connectionSettings.OrcaleConnection),
                    Table = tableName,
                    Exception = e,
                });
            }
        }
        private static void SyncToSqlTable<T>(string query, string tableName)
        {
            string conn = "";
            if (tableName == "SH_01.MEDICINE_DATA")
            {
                conn = _connectionSettings.OrcaleConnectionSH65;
                string OracleQuery2 = "UPDATE SH_01.MEDICINE_DATA SET UNIT_PRICE = ROUND(UNIT_PRICE,3),PACK_PRICE=ROUND(PACK_PRICE,3),PACK_SIZE = ROUND(PACK_SIZE,3) WHERE IS_SYNC=0 OR SYNC_BY='UPDATE'";
                ExecuteOracleQuery(OracleQuery2, _connectionSettings.OrcaleConnectionSH65);
            }
            else if (tableName == "SH_01.SER_PROV_DISC" || tableName == "SH_01.CO_INSURANCE_01" || tableName == "SH_01.DMS_02_EMP_D_ENT_MAN")
            {
                conn = _connectionSettings.OrcaleConnectionSH65;
            }
            else if (tableName == "APP.SERV_PROVIDERS_NEW" || tableName == "APP.SERVICES" || tableName == "APP.POLL_DATA"
                || tableName == "APP.POLL_DATA_SERVICE" || tableName == "APP.POLL_PERCENT"
                || tableName == "APP.POLL_PERCENT_CARD" || tableName == "APP.POLL_AMOUNT" || tableName == "APP.POLL_AMOUNT_CARD"
                || tableName == "APP.POLL_DATA_CHRONIC" || tableName == "APP.POLL_DATA_DIAG" || tableName == "APP.POLL_DATA_EXCEPTIONS"
                || tableName == "APP.POLL_DATA_PREX" || tableName == "APP.COMP_CUSTOMIZED_D_D_MED_EMP"
                || tableName == "APP.COMP_CUSTOMIZED_D_D_MED" || tableName == "APP.REMAIN_CONSUMATION"|| tableName == "APP.CONSUMPTION_POOL")
            {
                conn = _connectionSettings.OrcaleConnectionApp;
            }
            else if (tableName == "APP.APPROVAL_BAD" || tableName == "APP.SER_PROV_DISC_SQL")
            {
                conn = _connectionSettings.OrcaleConnectionApp129;
            }
            else
            {
                conn = _connectionSettings.OrcaleConnection;
            }
            double count = GetCount(tableName, conn);
            var maxiteration = Math.Ceiling(count / 1000);
            tableName = tableName.Split('.')[1];
            for (int i = 0; i < maxiteration; i = i)
            {
                try
                {
                    var data = GetOracleTable<T>(string.Format(query, (i * 1000), ((++i) * 1000)), conn);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(conn),
                        Server = GetServerName(conn),
                        Table = tableName,
                        Note = "All",
                        AffectedRows = data.Count
                    });

                    try
                    {
                        var SyncData = FullSyncFieldsSQL<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                        var CastData = SyncData.Cast<T>().ToList();
                        string SqlTableName;

                        switch (tableName)
                        {
                            case "COMP_CONTRACT_CLASS":
                                SqlTableName = "CompContractClass";
                                break;
                            case "DIAGNOSES":
                                SqlTableName = "Diagnosis";
                                break;
                            case "MEDICINE_DATA":
                                SqlTableName = "MedicineData";
                                break;
                            case "SER_PROV_DISC":
                                SqlTableName = "Ser_Prov_Disc";
                                break;
                            case "CO_INSURANCE_01":
                                SqlTableName = "Co_Insurance_01";
                                break;

                            case "SERV_PROVIDERS":
                                SqlTableName = "Serv_Providers1";
                                break;

                            case "POLL_DATA":
                                SqlTableName = "PollData";
                                break;
                            case "POLL_DATA_SERVICE":
                                SqlTableName = "PollDataService";
                                break;
                            case "POLL_PERCENT":
                                SqlTableName = "PollPercent";
                                break;
                            case "POLL_PERCENT_CARD":
                                SqlTableName = "PollPercentCard";
                                break;
                            case "POLL_AMOUNT":
                                SqlTableName = "PollAmount";
                                break;
                            case "POLL_AMOUNT_CARD":
                                SqlTableName = "PollAmountCard";
                                break;
                            case "POLL_DATA_CHRONIC":
                                SqlTableName = "PollDataChronic";
                                break;
                            case "POLL_DATA_DIAG":
                                SqlTableName = "PollDataDiag";
                                break;
                            case "POLL_DATA_EXCEPTIONS":
                                SqlTableName = "PollDataExcepitions";
                                break;
                            case "POLL_DATA_PREX":
                                SqlTableName = "PollDataPreX";
                                break;
                            case "COMP_CONTRACT_CLASS_EMP":
                                SqlTableName = "CompContractClassEmp";
                                break;

                            case "SER_PROV_DISC_SQL":
                                SqlTableName = "Ser_Prov_Disc";
                                break;
                            case "REMAIN_CONSUMATION":
                                SqlTableName = "RemainConsumption";
                                break;
                            //case "DMS_02_EMP_D_ENT_MAN":
                            //    SqlTableName = "DMS_02_EMP_D_ENT_MAN3";
                            //    break;

                            default:
                                SqlTableName = tableName;
                                break;
                        }
                        // هنا بيضيف يا ايه اول ما يقف هنا تمسحي الجدول اللي فال SQL  وتدوسي Continue
                        AddNewEntities(CastData, SqlTableName, false, _connectionSettings.SQlConnection);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = tableName,
                            AffectedRows = data.Count
                        });


                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }
                catch (Exception e)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(conn),
                        Server = GetServerName(conn),
                        Table = tableName,
                        Exception = e,
                    });
                }
            }

            if (tableName == "COMP_CUSTOMIZED_D_D_EMP")
            {
                string queryDDEmp = " update Comp_Customized_D_D_Emp set CEILING_AMT = null WHERE CEILING_AMT = 0";
                ExecuteSQLUpdateQuery(queryDDEmp, _connectionSettings.SQlConnection);
            }
            if (tableName == "COMP_CUSTOMIZED_D_EMP")
            {
                string queryDEMP = "update COMP_CUSTOMIZED_D_EMP set CEILING_AMT = null WHERE CEILING_AMT = 0";
                ExecuteSQLUpdateQuery(queryDEMP, _connectionSettings.SQlConnection);
            }
            if (tableName == "COMP_CUSTOMIZED_D_D")
            {
                string queryDD = "update Comp_Customized_D_D set CEILING_AMT = null WHERE CEILING_AMT = 0";
                ExecuteSQLUpdateQuery(queryDD, _connectionSettings.SQlConnection);
            }
            if (tableName == "COMP_CUSTOMIZED_D")
            {
                string queryD = "update Comp_Customized_D set CEILING_AMT = null WHERE CEILING_AMT = 0";
                ExecuteSQLUpdateQuery(queryD, _connectionSettings.SQlConnection);
            }
            if (tableName == "MEDICINE_DATA")
            {
                string queryD = " UPDATE MedicineData SET diagnoisegender = 3, iscovered = 1, DiagnoiseAge = 'All'";
                ExecuteSQLUpdateQuery(queryD, _connectionSettings.SQlConnection);
                string queryDa = " UPDATE MedicineData SET ACTIVE = 'Y' WHERE ACTIVE IS NULL";
                ExecuteSQLUpdateQuery(queryDa, _connectionSettings.SQlConnection);
            }

            string OracleQuery = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
            ExecuteOracleQuery(OracleQuery, conn);
        }
        private static void SyncToSqlTableSH<T>(string query, string tableName)
        {
            tableName = tableName.Split('.')[1];
            //if (DateTime.Now.Day == 21)
            //{
            if (tableName == "MED_CARD")
            {

                List<Roshita> roshitas = new List<Roshita>();
                double count = double.Parse(GetOracleDataTable("select COUNT(*) from MED_CARD WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA' ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
                var maxiteration = Math.Ceiling(count / 10000);
                query = "SELECT * FROM(select m.*,rownum r from SH_01.MED_CARD m "
                           + " WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA' )WHERE r > {0} and r<= {1}";



                #region test
                for (int i = 0; i < maxiteration; i = i)
                {
                    try
                    {

                        var data = GetOracleTable<T>(string.Format(query, (i * 10000), ((++i) * 10000)), _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Get.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                            Table = tableName,
                            Note = "All",
                            AffectedRows = data.Count
                        });
                        try
                        {
                            var SyncData = FullSyncFieldsSQL<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                            var CastData = SyncData.Cast<T>().ToList();
                            AddNewEntities(CastData, tableName);

                            #region rostita and details
                            if (tableName == "MED_CARD")
                            {
                                var med_card = CastData.Cast<Med_Card>().ToList();
                                foreach (var item in med_card)
                                {
                                    roshitas.Add(new Roshita
                                    {
                                        CardId = item.CARD_NO,
                                        Speciality = "Empty",
                                        Diagnose1 = item.TASHKHES_01,
                                        RoshetaType = "11602",
                                        Limit = 0,
                                        CompanyPayment = 0,
                                        OverInsurance = 0,
                                        TotalValue = 0,
                                        CompanyPercent = 0,
                                        PersonPayment = 0,
                                        Cash = 0,
                                        Manager = "Doctor_Chronic",
                                        CreatedBy = item.CREATED_BY == null ? "Admin" : item.CREATED_BY,
                                        CreatedDate = item.CREATED_DATE,
                                        IsSync = true,
                                        SyncBy = "Admin",
                                        SyncDate = DateTime.Now,
                                        Diagnose2 = "Empty",
                                        diagnose3 = "Empty",
                                        PhoneNumber = "Empty",
                                        Oracle_Id = 0

                                    });
                                }
                                AddNewEntities(roshitas, "Roshita");
                                roshitas.Clear();
                            }
                            #endregion
                            _result.Logs.Add(new ViewModels.Log
                            {
                                Order = GetLogOrder(),
                                Action = SyncAction.Insert.ToString(),
                                Database = GetDatabaseName(),
                                Server = GetServerName(),
                                Table = tableName,
                                AffectedRows = data.Count
                            });
                            var ob = CastData.Cast<Med_Card>().ToList();
                            if (ob.Count == 1)
                            {
                                string OracleQuery = "UPDATE MED_CARD SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA' " +
                                                     " AND CARD_NO ='" + ob[0].CARD_NO + "'";
                                ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);

                            }
                            else if (ob.Count > 1)
                            {
                                string OracleQuery = "UPDATE MED_CARD SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA' " +
                                                     " AND CARD_NO IN('" + ob[0].CARD_NO + "','";
                                for (int j = 1; j < ob.Count - 1; j++)
                                {
                                    OracleQuery += ob[j].CARD_NO + "','";
                                }
                                OracleQuery += ob[ob.Count - 1].CARD_NO + "')";
                                ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                            }

                        }
                        catch (Exception ex)
                        {
                            _result.Errors.Add(new Error()
                            {
                                Action = SyncAction.Insert.ToString(),
                                Database = GetDatabaseName(),
                                Server = GetServerName(),
                                Table = tableName,
                                Exception = ex,
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Get.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                            Table = tableName,
                            Exception = e,
                        });
                    }
                }
                //string OracleQuery = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
                //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                #endregion
            }

            else if (tableName == "MED_MEDICINE")
            {
                SysMedMedicineWithDetails();
                //string OracleQuery = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
                //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);

            }
            //string OracleQuery1 = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
            //ExecuteOracleQuery(OracleQuery1, _connectionSettings.OrcaleConnectionSH65);
            //}

        }

        private static void SysMedMedicineWithDetails()
        {

            string OracleQuery = "UPDATE SH_01.MED_MEDICINE SET DOSE = ROUND(DOSE,3),DOS_DUR=ROUND(DOS_DUR,3),EXCESS = ROUND(EXCESS,3),PACK_PRICE=ROUND(PACK_PRICE,3),UNIT_PRICE=ROUND(UNIT_PRICE,3),TOTAL_AMT=ROUND(TOTAL_AMT,3) WHERE IS_SYNC=0 OR IS_SYNC IS NULL OR SYNC_BY='UPDATE'";
            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);

            List<RoshitaIDs> swap = new List<RoshitaIDs>();
            List<RoshitaDetail> roshita_detail = new List<RoshitaDetail>();
            string SelectIds = "SELECT Id , CardId  FROM Roshita WHERE Manager='Doctor_Chronic' AND Oracle_Id=0";

            var IDs = GetSqlDataTable(SelectIds, _connectionSettings.SQlConnection);

            swap = IDs.AsEnumerable().Select(row => new RoshitaIDs
            {
                Id = row.IsNull("Id") ? 0 : long.Parse(row.Field<long>("Id").ToString()),
                CardId = row.IsNull("CardId") ? "Empty" : row.Field<string>("CardId").ToString()
            }).ToList();


            //double count = double.Parse(GetOracleDataTable("select COUNT(*) from SH_01.MED_MEDICINE WHERE  ACTIVE = 'Y' AND CREATED_DATE >='" + dateMed + "' AND (MONTH_DATE_STOP IS NULL OR MONTH_DATE_STOP>=SYSDATE)", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
            //var maxiteration = Math.Ceiling(count / 1000);
            //string query = "SELECT * FROM(select MED_CODE ,MED_NAME,DOSE,DOS_DUR,NO_OF_UINT,TOTAL_AMT,ACTIVE,MONTH_DATE_STOP,CARD_NO,rownum r from SH_01.MED_MEDICINE "
            //               + " WHERE ACTIVE = 'Y' AND CREATED_DATE >='" + dateMed + "' AND(MONTH_DATE_STOP IS NULL OR MONTH_DATE_STOP >= SYSDATE))WHERE r > {0} and r<= {1}";


            double count = double.Parse(GetOracleDataTable("select COUNT(*) from SH_01.MED_MEDICINE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA'", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);
            string query = "SELECT * FROM(select m.*,rownum r from SH_01.MED_MEDICINE m"
                           + " WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA' )WHERE r > {0} and r<= {1}";


            for (int i = 0; i < maxiteration; i = i)
            {
                try
                {

                    var data = GetOracleTable<Med_Medicine>(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "Med_Medicine",
                        Note = "All",
                        AffectedRows = data.Count
                    });
                    var SyncData = FullSyncFieldsSQL<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                    var med_medicien = SyncData.Cast<Med_Medicine>().ToList();
                    AddNewEntities(med_medicien, "Med_Medicine");
                    //var med_medicien = data.Cast<Med_Medicine>().ToList();
                    roshita_detail.Clear();
                    DateTime datenow = DateTime.Now;
                    foreach (var item in med_medicien)
                    {
                        if (item.ACTIVE == "Y" && (item.MONTH_DATE_STOP >= new DateTime(datenow.Year, datenow.Month, datenow.Day) || item.MONTH_DATE_STOP == null))
                        {
                            roshita_detail.Add(new RoshitaDetail
                            {
                                MedicienCode = item.MED_CODE,
                                MedicienName = item.MED_NAME,
                                Dose = (int)item.DOSE,
                                Duration = (int)item.MED_DURATION,
                                TotalDuration = 0,
                                TotalUnits = (int)item.NO_OF_UINT,
                                Amount = (double)item.TOTAL_AMT,
                                IsDealed = false,
                                PaymentGroup = "YES",
                                RoshitaID = GetRoshitaId(item.CARD_NO, swap),
                                IsSync = true,
                                SyncDate = DateTime.Now,
                                SyncBy = "Admin"

                            });
                        }
                        else
                        {
                            roshita_detail.Add(new RoshitaDetail
                            {
                                MedicienCode = item.MED_CODE,
                                MedicienName = item.MED_NAME,
                                Dose = (int)item.DOSE,
                                Duration = (int)item.DOS_DUR,
                                TotalDuration = 0,
                                TotalUnits = (int)item.NO_OF_UINT,
                                Amount = (double)item.TOTAL_AMT,
                                IsDealed = true,
                                PaymentGroup = "YES",
                                RoshitaID = GetRoshitaId(item.CARD_NO, swap),
                                IsSync = true,
                                SyncDate = DateTime.Now,
                                SyncBy = "Admin"

                            });
                        }
                    }
                    var RemoveRelation = "ALTER TABLE RoshitaDetails DROP CONSTRAINT  FK_RoshitaDetails_Roshita; ";
                    var AddRelation = "ALTER TABLE RoshitaDetails WITH NOCHECK ADD CONSTRAINT FK_RoshitaDetails_Roshita FOREIGN KEY(RoshitaID) REFERENCES Roshita(Id); ";
                    SetOrRemoveRelation(RemoveRelation, _connectionSettings.SQlConnection);
                    AddNewEntities(roshita_detail, "RoshitaDetails");
                    SetOrRemoveRelation(AddRelation, _connectionSettings.SQlConnection);
                    roshita_detail.Clear();

                    if (med_medicien.Count == 1)
                    {
                        string OracleQuery2 = "UPDATE MED_MEDICINE SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA'" +
                                             " AND CARD_NO ='" + med_medicien[0].CARD_NO + "' AND MED_CODE='" +
                                             med_medicien[0].MED_CODE + "'";
                        ExecuteOracleQuery(OracleQuery2, _connectionSettings.OrcaleConnectionSH65);

                    }
                    else if (med_medicien.Count > 1)
                    {
                        string OracleQuery2 = "UPDATE MED_MEDICINE SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA' " +
                                             " AND CARD_NO IN('" + med_medicien[0].CARD_NO + "','";
                        for (int j = 1; j < med_medicien.Count - 1; j++)
                        {
                            OracleQuery2 += med_medicien[j].CARD_NO + "','";
                        }
                        OracleQuery2 += med_medicien[med_medicien.Count - 1].CARD_NO + "')";
                        ExecuteOracleQuery(OracleQuery2, _connectionSettings.OrcaleConnectionSH65);
                    }

                }
                catch (Exception e)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "Med_Medicine" + i,
                        Exception = e,
                    });
                }
            }

        }

        private static void UpdateToSqlTableSH<T>(string tableName)
        {
            string query = "";
            tableName = tableName.Split('.')[1];

            if (tableName == "MED_CARD")
            {

                List<Roshita> roshitas = new List<Roshita>();
                double count = double.Parse(GetOracleDataTable("select COUNT(*) from MED_CARD WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1 ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
                var maxiteration = Math.Ceiling(count / 1000);
                query = "SELECT * FROM(select m.*,rownum r from SH_01.MED_CARD m "
                           + " WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1 )WHERE r > {0} and r<= {1}";



                #region test
                for (int i = 0; i < maxiteration; i = i)
                {
                    try
                    {

                        var data = GetOracleTable<T>(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Get.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                            Table = tableName,
                            Note = "All",
                            AffectedRows = data.Count
                        });
                        try
                        {
                            var SyncData = FullSyncFieldsSQL<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                            var CastData = SyncData.Cast<Med_Card>().ToList();
                            foreach (var item in CastData)
                            {
                                string queryUpdate = "UPDATE MED_CARD SET CARD_NO ='" + item.CARD_NO +
                                    "' ,PROVIDER_CODE= " + item.PROVIDER_CODE + ",C_COMP_ID=" + item.C_COMP_ID +
                                    ",NOTES='" + item.NOTES + "',UPDATE_BY='" + item.UPDATE_BY + "',UPDATE_DATE='" +
                                    item.UPDATE_DATE + "',MONTH_START_DATE='" + item.MONTH_START_DATE + "',MONTH_END_DATE='" +
                                    item.MONTH_END_DATE + "',GROUP_ID=" + item.GROUP_ID + ",GROUP_NAME=N'" + item.GROUP_NAME +
                                    "',LOOK_01=" + item.LOOK_01 + ",SEQ=" + item.SEQ + ",PROVIDER_CODE_OLD=" + item.PROVIDER_CODE_OLD +
                                    ",TASHKHES_01=N'" + item.TASHKHES_01 + "',NO_PAY=" + item.NO_PAY + ",NO_OVER=" + item.NO_OVER + ",ST_DAY=" +
                                    item.ST_DAY + ",SyncDate='" + DateTime.Now + "' WHERE CARD_NO='" + item.CARD_NO + "'";

                                ExecuteSQLUpdateQuery(queryUpdate, _connectionSettings.SQlConnection);
                                queryUpdate = "";
                            }



                            #region rostita and details

                            #endregion
                            _result.Logs.Add(new ViewModels.Log
                            {
                                Order = GetLogOrder(),
                                Action = SyncAction.Update.ToString(),
                                Database = GetDatabaseName(),
                                Server = GetServerName(),
                                Table = tableName,
                                AffectedRows = data.Count
                            });

                            if (CastData.Count == 1)
                            {
                                string updateOracleQuery = "UPDATE MED_CARD SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=1 AND SYNC_BY = 'UPDATE' " +
                                                     " AND CARD_NO ='" + CastData[0].CARD_NO + "'";
                                ExecuteOracleQuery(updateOracleQuery, _connectionSettings.OrcaleConnectionSH65);

                            }
                            else if (CastData.Count > 1)
                            {
                                string updateOracleQuery = "UPDATE MED_CARD SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=1 AND SYNC_BY = 'UPDATE' " +
                                                     " AND CARD_NO IN('" + CastData[0].CARD_NO + "','";
                                for (int j = 1; j < CastData.Count - 1; j++)
                                {
                                    updateOracleQuery += CastData[j].CARD_NO + "','";
                                }
                                updateOracleQuery += CastData[CastData.Count - 1].CARD_NO + "')";
                                ExecuteOracleQuery(updateOracleQuery, _connectionSettings.OrcaleConnectionSH65);
                            }
                        }
                        catch (Exception ex)
                        {
                            _result.Errors.Add(new Error()
                            {
                                Action = SyncAction.Insert.ToString(),
                                Database = GetDatabaseName(),
                                Server = GetServerName(),
                                Table = tableName,
                                Exception = ex,
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Get.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                            Table = tableName,
                            Exception = e,
                        });
                    }
                }
                //string OracleQuery = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
                //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                #endregion
            }

            else if (tableName == "MED_MEDICINE")
            {
                SysUpdateMedMedicineWithDetails();
                //string OracleQuery = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
                //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);

            }
            //string OracleQuery = "UPDATE " + tableName + " SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin'  WHERE IS_SYNC=1 AND SYNC_BY = 'UPDATE' ";
            //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
            //}

        }

        private static void SysUpdateMedMedicineWithDetails()
        {

            List<RoshitaIDs> swap = new List<RoshitaIDs>();
            List<RoshitaDetail> roshita_detail = new List<RoshitaDetail>();
            string SelectIds = "SELECT Id , CardId  FROM Roshita WHERE Manager='Doctor_Chronic' AND Oracle_Id=0";

            var IDs = GetSqlDataTable(SelectIds, _connectionSettings.SQlConnection);

            swap = IDs.AsEnumerable().Select(row => new RoshitaIDs
            {
                Id = row.IsNull("Id") ? 0 : long.Parse(row.Field<long>("Id").ToString()),
                CardId = row.IsNull("CardId") ? "Empty" : row.Field<string>("CardId").ToString()
            }).ToList();

            double count = double.Parse(GetOracleDataTable("select COUNT(*) from SH_01.MED_MEDICINE WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);
            string query = "SELECT * FROM(select m.*,rownum r from SH_01.MED_MEDICINE m"
                           + " WHERE SYNC_BY = 'UPDATE' AND IS_SYNC=1 )WHERE r > {0} and r<= {1}";


            for (int i = 0; i < maxiteration; i = i)
            {
                try
                {

                    var data = GetOracleTable<Med_Medicine>(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "Med_Medicine",
                        Note = "All",
                        AffectedRows = data.Count
                    });
                    var SyncData = FullSyncFieldsSQL<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                    var med_medicien = SyncData.Cast<Med_Medicine>().ToList();


                    foreach (var item in med_medicien)
                    {
                        string queryUpdate = "UPDATE Med_Medicine SET MED_CODE=N'" + item.MED_CODE + "', CARD_NO =N'" + item.CARD_NO +
                            "' ,RDATE=N'" + item.RDATE + "',MED_TYP=" + item.MED_TYP +
                            ",DOSE=" + item.DOSE + ",NO_OF_UINT=" + item.NO_OF_UINT + ",TOTAL_AMT=" +
                            item.TOTAL_AMT + ",MED_DURATION=" + item.MED_DURATION + ",TOT_DUR=N'" +
                            item.TOT_DUR + "',DOS_DUR=" + item.DOS_DUR + ",EXCESS=" + item.EXCESS +
                            ",PACK_SIZE=" + item.PACK_SIZE + ",PACK_PRICE=" + item.PACK_PRICE + ",CON_MED=N'" + item.CON_MED +
                            "',UNIT_NO=" + item.UNIT_NO + ",UNIT_PRICE=" + item.UNIT_PRICE + ",MED_NAME=N'" + item.MED_NAME +
                            "',DOSAGE_FORM=N'" + item.DOSAGE_FORM + "',NOTES=N'" + item.NOTES + "',ACTIVE=N'" + item.ACTIVE +
                            "',UPDATE_BY='" + item.UPDATE_BY + "',UPDATE_DATE='" + item.UPDATE_DATE + "',ACT_MONTH=N'"
                            + item.ACT_MONTH + "',LFT_MONTH=" + item.LFT_MONTH +
                            ",MONTH_DATE_STOP=" + (item.MONTH_DATE_STOP == null ? "null" : "N'" + item.MONTH_DATE_STOP + "'") + ",SyncDate='" + DateTime.Now + "' ,IsSync=1,SyncBy='Admin' WHERE CARD_NO='"
                            + item.CARD_NO + "' AND MED_CODE='" + item.MED_CODE + "'";

                        ExecuteSQLUpdateQuery(queryUpdate, _connectionSettings.SQlConnection);
                        queryUpdate = "";
                    }


                    //AddNewEntities(med_medicien, "Med_Medicine");
                    //var med_medicien = data.Cast<Med_Medicine>().ToList();
                    roshita_detail.Clear();
                    DateTime datenow = DateTime.Now;
                    bool x = false;
                    foreach (var item in med_medicien)
                    {


                        if (item.ACTIVE == "Y")
                        {
                            x = true;
                        }
                        else
                        {
                            x = false;
                        }
                        roshita_detail.Add(new RoshitaDetail
                        {
                            MedicienCode = item.MED_CODE,
                            MedicienName = item.MED_NAME,
                            Dose = (int)item.DOSE,
                            Duration = (int)item.DOS_DUR,
                            TotalDuration = 0,
                            TotalUnits = (int)item.NO_OF_UINT,
                            Amount = (double)item.TOTAL_AMT,
                            IsDealed = x,
                            PaymentGroup = "YES",
                            RoshitaID = GetRoshitaId(item.CARD_NO, swap),
                            IsSync = true,
                            SyncDate = DateTime.Now,
                            SyncBy = "Admin"

                        });
                    }
                    var RemoveRelation = "ALTER TABLE RoshitaDetails DROP CONSTRAINT  FK_RoshitaDetails_Roshita; ";
                    var AddRelation = "ALTER TABLE RoshitaDetails WITH NOCHECK ADD CONSTRAINT FK_RoshitaDetails_Roshita FOREIGN KEY(RoshitaID) REFERENCES Roshita(Id); ";
                    SetOrRemoveRelation(RemoveRelation, _connectionSettings.SQlConnection);

                    foreach (var item in roshita_detail)
                    {
                        if (item.IsDealed)
                        {
                            item.IsDealed = false;
                            string queryUpdate = "UPDATE RoshitaDetails SET MedicienCode='" + item.MedicienCode +
                                "',MedicienName='" + item.MedicienName + "', Dose=" + item.Dose + ", Duration =" + item.Duration +
                                " ,TotalDuration=" + item.TotalDuration + ",TotalUnits=" + item.TotalUnits +
                                ",Amount=" + item.Amount + ",PaymentGroup='" + item.PaymentGroup + "',SyncDate='" + DateTime.Now + "', IsSync = 1, SyncBy = 'Admin' " +
                                " WHERE MedicienCode='" + item.MedicienCode + "' AND RoshitaID=" + item.RoshitaID;

                            int resault = ExecuteSQLUpdateQuery2(queryUpdate, _connectionSettings.SQlConnection);
                            if (resault == 0)
                            {
                                List<RoshitaDetail> lis = new List<RoshitaDetail>();
                                lis.Add(item);
                                AddNewEntities(lis, "RoshitaDetails");
                            }
                            queryUpdate = "";
                        }
                        else
                        {
                            item.IsDealed = true;
                            string queryUpdate = "UPDATE RoshitaDetails SET MedicienCode='" + item.MedicienCode +
                                "',MedicienName='" + item.MedicienName + "', Dose=" + item.Dose + ", Duration =" + item.Duration +
                                " ,TotalDuration=" + item.TotalDuration + ",TotalUnits=" + item.TotalUnits +
                                " ,IsDealed =1,Amount=" + item.Amount + ",PaymentGroup='" + item.PaymentGroup + "',SyncDate='" + DateTime.Now + "', IsSync = 1, SyncBy = 'Admin' " +
                                " WHERE MedicienCode='" + item.MedicienCode + "' AND RoshitaID=" + item.RoshitaID;

                            int resault = ExecuteSQLUpdateQuery2(queryUpdate, _connectionSettings.SQlConnection);
                            //string queryDelete = "DELETE RoshitaDetails  WHERE MedicienCode='" + item.MedicienCode + "' AND RoshitaID=" + item.RoshitaID;

                            //ExecuteSQLUpdateQuery(queryDelete, _connectionSettings.SQlConnection);
                            //queryDelete = "";
                        }
                    }


                    //AddNewEntities(roshita_detail, "RoshitaDetails");
                    SetOrRemoveRelation(AddRelation, _connectionSettings.SQlConnection);
                    roshita_detail.Clear();

                    if (med_medicien.Count == 1)
                    {
                        string OracleQuery2 = "UPDATE MED_MEDICINE SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=1 AND SYNC_BY = 'UPDATE' " +
                                             " AND CARD_NO ='" + med_medicien[0].CARD_NO + "' AND MED_CODE='" +
                                             med_medicien[0].MED_CODE + "'";
                        ExecuteOracleQuery(OracleQuery2, _connectionSettings.OrcaleConnectionSH65);

                    }
                    else if (med_medicien.Count > 1)
                    {
                        string OracleQuery2 = "UPDATE MED_MEDICINE SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=1 AND SYNC_BY = 'UPDATE' " +
                                             " AND CARD_NO IN('" + med_medicien[0].CARD_NO + "','";
                        for (int j = 1; j < med_medicien.Count - 1; j++)
                        {
                            OracleQuery2 += med_medicien[j].CARD_NO + "','";
                        }
                        OracleQuery2 += med_medicien[med_medicien.Count - 1].CARD_NO + "')";
                        ExecuteOracleQuery(OracleQuery2, _connectionSettings.OrcaleConnectionSH65);
                    }
                }
                catch (Exception e)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "Med_Medicine" + i,
                        Exception = e,
                    });
                }
            }

        }

        private static long GetRoshitaId(string CardId, List<RoshitaIDs> swap)
        {
            var result = swap.Where(o => o.CardId == CardId).FirstOrDefault();
            return (result == null ? 0 : result.Id);
            //string Query = "SELECT Id  FROM Roshita WHERE Manager='Doctor_Chronic' AND Oracle_Id=0";
            //var data = GetSqlDataTable(Query, _connectionSettings.SQlConnection);
            //return int.Parse(data.Rows[0][0].ToString());

        }

        private static double GetCount(string TableName, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;
            string Query = "SELECT COUNT(*) FROM " + TableName + " WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
            var data = GetOracleDataTable(Query, connectionString);
            return data.Rows.Count > 0 ? double.Parse(data.Rows[0][0].ToString()) : 0;

        }
        private static List<T> FullSyncFieldsSQL<T>(List<T> data) where T : BaseEntityDB
        {
            foreach (var item in data)
            {
                item.IsSync = true;
                item.SyncBy = "Admin";
                item.SyncDate = DateTime.Now;
            }
            return data;
        }

        private static List<T> FullSyncFieldsORA<T>(List<T> data) where T : BaseEntityDB
        {
            foreach (var item in data)
            {
                item.IsSync = true;
                item.SyncBy = "SQL";
                item.SyncDate = DateTime.Now;
            }
            return data;
        }


        /// <summary>
        /// returns list of inserted data 
        /// </summary>
        /// <typeparam name="Tse">SE Table Class</typeparam>
        /// <typeparam name="Tback">Back Office Table class</typeparam>
        /// <param name="tableName">SE Table Name that we get the data from</param>
        /// <param name="whereStatement">where statement of get query</param>
        /// <param name="idColumnNameForSync">Id column name in SE Table</param>
        /// <param name="backTableName">Back Office Table Name that we insert the data into it</param>
        /// <param name="handling">The Mapping between The two Classes SE and Back Office</param>
        /// <param name="fetchInsertedData">Whether we need to fetch the inserted data after the inserting operation or not</param>
        /// <param name="whereColumnName">if the previous parameter is true, we must specify the column's name of the column that has the unique value in both tables</param>
        /// <returns>SyncTableBackResponse</returns>
        private static SyncTableBackResponse<Tse, Tback> SyncTableBack<Tse, Tback>(string tableName, string whereStatement, string idColumnNameForSync, string refColumnNameForSync, string backTableName,
            Func<Tse, Tback> handling, bool fetchInsertedData = false, string whereColumnName = "")
        {

            try
            {
                //select frontoffice  Data
                string seQuery = $"select * from {tableName} ";
                if (!string.IsNullOrEmpty(whereStatement))
                {
                    seQuery += " where " + whereStatement;
                }
                var data = GetTable<Tse>(seQuery);
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,

                    AffectedRows = data.Count
                });
                if (data.Count == 0)
                {
                    return new SyncTableBackResponse<Tse, Tback>();
                }
                var newData = data.Select(handling).ToList();
                try
                {
                    //add customer accout
                    if (tableName.ToLower() == "customers_coding")
                    {
                        var propertyInfo = typeof(Tse).GetProperty(idColumnNameForSync, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var ids = data.Select(e => propertyInfo.GetValue(e, null).ToString()).ToList();

                        seQuery = $"select * from {tableName} ";
                        if (!string.IsNullOrEmpty(whereStatement))
                        {
                            seQuery += " where " + whereStatement;
                        }
                        data = GetTable<Tse>(seQuery);
                        newData = data.Select(handling).ToList();
                    }
                    //add new data

                    SqlConnection connection = new SqlConnection(_connectionSettings.OrcaleConnection);
                    connection.Open();


                    AddNewEntities(newData, backTableName, false, _connectionSettings.OrcaleConnection);

                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = backTableName,
                        AffectedRows = newData.Count
                    });
                    // set synced and add entries accounts
                    if (idColumnNameForSync != null)
                    {
                        var propertyInfo = typeof(Tse).GetProperty(idColumnNameForSync, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var ids = data.Select(e => propertyInfo.GetValue(e, null).ToString()).ToList();
                        var propertyInfoRefs = typeof(Tse).GetProperty(idColumnNameForSync, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var refs = data.Select(e => propertyInfo.GetValue(e, null).ToString()).ToList();

                        if (refColumnNameForSync != null)
                        {
                            propertyInfoRefs = typeof(Tse).GetProperty(refColumnNameForSync, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            refs = data.Select(e => propertyInfoRefs.GetValue(e, null).ToString()).ToList();
                        }
                        try
                        {
                            UpdateToSynced(tableName, idColumnNameForSync, ids, refColumnNameForSync, refs, null);
                            _result.Logs.Add(new ViewModels.Log
                            {
                                Order = GetLogOrder(),
                                Action = SyncAction.MarkAsSynced.ToString(),
                                Database = GetDatabaseName(),
                                Server = GetServerName(),
                                Table = tableName,
                                AffectedRows = ids.Count
                            });
                        }
                        catch (Exception ex)
                        {
                            _result.Errors.Add(new Error()
                            {
                                Action = SyncAction.MarkAsSynced.ToString(),
                                Database = GetDatabaseName(),
                                Server = GetServerName(),
                                Table = tableName,
                                Exception = ex,
                            });
                        }
                    }


                    //new  data for details
                    if (idColumnNameForSync != null)
                    {
                        var propertyInfo = typeof(Tse).GetProperty(idColumnNameForSync, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var ids = data.Select(e => propertyInfo.GetValue(e, null).ToString()).ToList();
                        seQuery = $"select * from {tableName} ";
                        if (!string.IsNullOrEmpty(whereStatement))
                        {
                            seQuery += " where id in(";
                            for (int item = 0; item < ids.Count; item++)
                            {
                                if (item == ids.Count - 1)
                                    seQuery += "'" + ids[item] + "'";
                                else
                                    seQuery += "'" + ids[item] + "',";
                            }

                            seQuery += ")";
                        }
                        data = GetTable<Tse>(seQuery);
                    }


                    // fetch inserted data
                    var insertedDataList = new List<Tback>();
                    if (fetchInsertedData)
                    {
                        var propertyInfo = typeof(Tback).GetProperty(whereColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var ids = newData.Select(e => propertyInfo.GetValue(e, null).ToString()).ToList();

                        var result = GetJoinedString(ids);

                        var tbQuery = $"select * from {backTableName} where {whereColumnName} in (" + result + ")";
                        insertedDataList = GetTable<Tback>(tbQuery, _connectionSettings.OrcaleConnection);
                    }

                    return new SyncTableBackResponse<Tse, Tback>
                    {
                        BackOfficeData = insertedDataList,
                        SEData = data
                    };
                }
                catch (Exception ex)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = tableName,
                        Exception = ex,
                    });
                    return new SyncTableBackResponse<Tse, Tback>();
                }
            }
            catch (Exception e)

            {
                _result.Errors.Add(new Error()
                {
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Table = tableName,
                    Exception = e,
                });

                return new SyncTableBackResponse<Tse, Tback>();
            }
        }


        private static int UpdateToSynced<T>(string tableName, string idColumnNameForSync, List<T> ids,
            string refColumnNameForSync, List<T> refs, string connectionString = null)
        {
            int result = 0;
            for (int item = 0; item < ids.Count; item++)
            {
                var query = "";
                if (tableName.ToLower() == "users")
                {
                    query = $"update {tableName} set IsSync = 1, SyncDate = '{DateTime.Now.ToShortDateString()}',Synced_ID='{MaxIDs[item]}',SyncBy='Admin' where {idColumnNameForSync} = {ids[item]}";
                }
                //else if (tableName.ToLower() == "rex_salesinvoicemain")
                //{
                //    //updated is sync  
                //    var getsynced_ID = GetTable<BackModels.SELLING_MAIN>("select * from SELLING_MAIN Where REPORT_NUMBER='" + refs[item].ToString() + "'", _connectionSettings.OrcaleConnection);
                //    query = $"update {tableName} set IsSync = 1, SyncDate = '{DateTime.Now.ToShortDateString()}',synced_ID='{ getsynced_ID[0].REPORT_ID}' where {idColumnNameForSync} = {ids[item]}";
                //}
                //else if (tableName.ToLower() == "rex_check")
                //{
                //    var getsynced_ID = GetTable<BackModels.CHECKS_COLLECTIONS>("select * from CHECKS_COLLECTIONS Where CHECK_NUMBER='" + refs[item].ToString() + "'", _connectionSettings.OrcaleConnection);
                //    query = $"update {tableName} set IsSync = 1, SyncDate = '{DateTime.Now.ToShortDateString()}',synced_ID='{ getsynced_ID[0].ID }' where {idColumnNameForSync} = {ids[item]}";

                //}

                //else if (tableName.ToLower() == "customers_coding")
                //{
                //    var getsynced_ID = GetTable<BackModels.CUSTOMERS_CODING>("select MAX(CUSTOMER_ID) CUSTOMER_ID from CUSTOMERS_CODING Where CUSTOMER_CODE='" + refs[item].ToString() + "'", _connectionSettings.OrcaleConnection);
                //    query = $"update {tableName} set IsSync = 1, SyncDate = '{DateTime.Now.ToShortDateString()}',synced_ID='{ getsynced_ID[0].CUSTOMER_ID }' where {idColumnNameForSync} = {ids[item]}";
                //}
                //else if (tableName.ToLower() == "lss_vacations")
                //{
                //    var getsynced_ID = GetTable<BackModels.EMP_Vacations>("select * from EMP_Vacations Where SKU='" + refs[item].ToString() + "'", _connectionSettings.OrcaleConnection);
                //    query = $"update {tableName} set IsSync = 1, SyncDate = '{DateTime.Now.ToString("yyyy-MM-dd")}',syncId='{ getsynced_ID[0].EMP_Vacation_Id }' where {idColumnNameForSync} = '{ids[item]}'";
                //}
                else
                {
                    query = $"update {tableName} set IsSync = 1, SyncDate = '{DateTime.Now.ToString("yyyy-MM-dd")}',SyncBy='Admin' where {idColumnNameForSync} = {ids[item]}";

                }
                //using (SqlConnection connection = new SqlConnection(_currenctConnectionString))
                //{
                //    connection.Open();
                //    SqlCommand command = connection.CreateCommand();
                //    SqlTransaction Updatetransaction = connection.BeginTransaction();
                //    command.Connection = connection;
                //    command.Transaction = Updatetransaction;
                //}
                try
                {
                    result = ExecuteNonQueryCommand(query, _currenctConnectionString);


                }
                catch (Exception ex)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.MarkAsSynced.ToString(),
                        Database = GetDatabaseName(),
                        Server = GetServerName(),
                        Table = tableName,
                        Exception = ex,
                    });
                    // Updatetransaction.Rollback();
                }
            }
            return result;
        }

        private static string GetJoinedString<T>(List<T> ids)
        {
            List<string> values = new List<string>();
            foreach (var id in ids)
            {
                values.Add($"'{id.ToString()}'");
            }
            var result = string.Join(", ", values.ToArray());
            return result;
        }

        private static string GetDatabaseName(string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            SqlConnectionStringBuilder builder =
                new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }

        private static string GetServerName(string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            SqlConnectionStringBuilder builder =
                new SqlConnectionStringBuilder(connectionString);
            return builder.DataSource;
        }

        private static void PullRoshitaDetails()
        {
            //string LastDate = "SELECT MAX(SyncDate) FROM RoshitaDetails";
            //try
            //{


            //    DateTime date = DateTime.Parse(GetSqlDataTable(LastDate, _connectionSettings.SQlConnection).Rows[0][0].ToString());
            //    DateToSync = string.Format("{0:dd-MMM-yyyy}", date);
            //}
            //catch (Exception ex)
            //{
            //    DateToSync = "28-May-2018";
            //}

            var RemoveRelation = "ALTER TABLE [DB_A45413_DMSERP].[dbo].[RoshitaDetails] DROP CONSTRAINT  FK_RoshitaDetails_Roshita; ";
            var AddRelation = "ALTER TABLE RoshitaDetails WITH NOCHECK ADD CONSTRAINT FK_RoshitaDetails_Roshita FOREIGN KEY(RoshitaID) REFERENCES Roshita(Id); ";

            SetOrRemoveRelation(RemoveRelation, _connectionSettings.SQlConnection);
            #region RoshitaDetails
            //string q = "select * from (select m.*, rownum r from SH_01.INV_SAL m where INV_DATE>='09-03-2020') WHERE r > 0 and r<= 100";
            //var query = "select * from (select m.*, rownum r from SH_01.INV_SAL m where INV_DATE>='" + DateToSync + "') WHERE r > {0} and r<= {1} ";
            //var queryLab = "select * from (select m.*, rownum r from SH_01.INV_SAL_LAB m where INV_DATE>='" + DateToSync + "') WHERE r > {0} and r<= {1} ";
            //var queryRay = "select * from (select m.*, rownum r from SH_01.INV_SAL_RAY m where INV_DATE>='" + DateToSync + "') WHERE r > {0} and r<= {1} ";

            //double count = double.Parse(GetOracleDataTable("select COUNT(*) from SH_01.INV_SAL WHERE INV_DATE>='" + DateToSync + "'", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());

            var query = "select * from (select m.*, rownum r from SH_01.INV_SAL m WHERE(IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9)"
                        + " AND SYNC_BY = 'ORA'  ) WHERE r > {0} AND r<= {1}";
            var queryLab = "select * from (select m.*, rownum r from SH_01.INV_SAL_LAB m where (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY = 'ORA') WHERE r > {0} and r<= {1} ";
            var queryRay = "select * from (select m.*, rownum r from SH_01.INV_SAL_RAY m where (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY = 'ORA') WHERE r > {0} and r<= {1} ";

            double countINV_SAL = double.Parse(GetOracleDataTable("select COUNT(*) from SH_01.INV_SAL WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA' ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
            double countLAB = double.Parse(GetOracleDataTable("select COUNT(*) from SH_01.INV_SAL_LAB WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY = 'ORA' ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
            double countRAY = double.Parse(GetOracleDataTable("select COUNT(*) from SH_01.INV_SAL_RAY WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY = 'ORA' ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());
            double count = Math.Max(countINV_SAL, countLAB);
            count = Math.Max(count, countRAY);

            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = ++i)
            {
                List<INV_SAL> data = new List<INV_SAL>();
                List<INV_SAL> iNV_SALs = new List<INV_SAL>();
                iNV_SALs.AddRange(Inv_Sal_Medicine(i, query));
                data.AddRange(Inv_Sal_Medicine(i, query));
                data.AddRange(Inv_Sal_Lab(i, queryLab));
                data.AddRange(Inv_Sal_Ray(i, queryRay));


                List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();

                string DeleteQuery = "DELETE FROM Swap ;";
                SetOrRemoveRelation(DeleteQuery, _connectionSettings.SQlConnection);
                List<Swap> swaps = new List<Swap>();
                swaps.AddRange(FillSwapObject(data));
                swaps.OrderBy(o => o.Id);
                try
                {
                    //add new data to swap IDs
                    AddNewEntities(swaps, "Swap", false, _connectionSettings.SQlConnection);

                }
                catch (Exception ex)
                {

                }
                string JoinQuery = "select r.Id as RoshitaID ,d.Id as Oracle_Id  from Swap d left join Roshita r on  d.Id = r.Oracle_Id  ORDER BY(r.Id); ";

                var IDs = GetSqlDataTable(JoinQuery, _connectionSettings.SQlConnection);

                List<JoinSwap> joinSwap = IDs.AsEnumerable().Select(row => new JoinSwap
                {
                    RoshitaID = row.IsNull("RoshitaID") ? 0 : long.Parse(row.Field<long>("RoshitaID").ToString()),
                    Oracle_Id = row.IsNull("Oracle_Id") ? 0 : long.Parse(row.Field<long>("Oracle_Id").ToString())
                }).ToList();


                string tableName = "INV_SAL and INV_SAL_LAB and INV_SAL_RAY";

                roshitaDetail.AddRange(FillRoshitaDetailsObject(data, joinSwap));
                swaps.Clear();
                joinSwap.Clear();


                try
                {
                    //add new data
                    AddNewEntities(roshitaDetail, "RoshitaDetails", false, _connectionSettings.SQlConnection);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = tableName,
                        AffectedRows = data.Count
                    });
                    if (iNV_SALs.Count == 1)
                    {
                        string OracleQuery = "UPDATE INV_SAL SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA' " +
                            " AND INV_ID IN = " + iNV_SALs[0].INV_ID;
                        ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);

                    }
                    else if (iNV_SALs.Count > 1)
                    {
                        string OracleQuery = "UPDATE INV_SAL SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA' " +
                            " AND INV_ID IN ( " + iNV_SALs[0].INV_ID + ",";
                        for (int j = 1; j < iNV_SALs.Count - 1; j++)
                        {
                            OracleQuery = OracleQuery + iNV_SALs[j].INV_ID + ",";
                        }
                        OracleQuery = OracleQuery + iNV_SALs[iNV_SALs.Count - 1].INV_ID + ")";
                        ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                    }
                }
                catch (Exception ex)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = tableName,
                        Exception = ex,
                    });
                }
            }
            SetOrRemoveRelation(AddRelation, _connectionSettings.SQlConnection);
            //string OracleQuery = "UPDATE INV_SAL SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA' ";
            //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
            string OracleQuery2 = "UPDATE INV_SAL_LAB SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA'  ";
            ExecuteOracleQuery(OracleQuery2, _connectionSettings.OrcaleConnectionSH65);
            string OracleQuery3 = "UPDATE INV_SAL_RAY SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA'  ";
            ExecuteOracleQuery(OracleQuery3, _connectionSettings.OrcaleConnectionSH65);

            #endregion
        }

        private static void PullRoshita()
        {

            #region Code
            ////string LastDate = "SELECT MAX(SyncDate) FROM Roshita WHERE Manager!='Doctor_Chronic'";
            ////try
            ////{


            ////    DateTime date = DateTime.Parse(GetSqlDataTable(LastDate, _connectionSettings.SQlConnection).Rows[0][0].ToString());
            ////    DateToSync = string.Format("{0:dd-MMM-yyyy}", date);
            ////}
            ////catch (Exception ex)
            ////{
            ////    DateToSync = "28-May-2018";
            ////}
            ////var query = "select * from (select m.*, rownum r from SH_01.DMS_02_EMP_D_ENT m where D_DATE>='" + DateToSync + "') WHERE r > {0} and r<= {1} ";

            ////double count = double.Parse(GetOracleDataTable("select  COUNT(*) from DMS_02_EMP_D_ENT WHERE D_DATE>='" + DateToSync + "'", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());

            //var query2 = "select * from (select m.EMP_NAME,m.D_ID, rownum r from SH_01.DMS_02_EMP_D_ENT3 m  ) WHERE r > {0} and r<= {1} ";
            //var maxiteration2 = 5;
            //for (int i = 0; i < maxiteration2; i = i)
            //{
            //    List<DMS_02_EMP_D_ENT> data = new List<DMS_02_EMP_D_ENT>();
            //    var data2 = GetOracleDataTable(string.Format(query2, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);
            //    try
            //    {

            //        data = data2.AsEnumerable().Select(row => new DMS_02_EMP_D_ENT
            //        {
            //            D_ID = row.IsNull("D_ID") ? 0 : long.Parse(row.Field<decimal>("D_ID").ToString()),
            //            EMP_NAME = row.IsNull("EMP_NAME") ? "Empty" : row.Field<string>("EMP_NAME")

            //        }).ToList();
            //    }
            //    catch (Exception ex)
            //    {
            //        var message = ex.Message;
            //    }

            //    //var data = GetOracleTable<DMS_02_EMP_D_ENT>("select * from SH_01.DMS_02_EMP_D_ENT WHERE D_DATE>='04 FEB 2020' and D_ID= 50220201368045", _connectionSettings.OrcaleConnection);

            //    //string tableName = "DMS_02_EMP_D_ENT";
            //    if (data.Count != 0)
            //    {
            //        //double CompanyPercent;
            //        //List<Roshita> roshitas = new List<Roshita>();
            //        //Roshita rsh = new Roshita();
            //        foreach (var item in data)
            //        {
            //            var user = GetOracleDataTable(@"select USER_ID ,USER_CO,USER_N from USERS_2 where USER_NAME='" + item.EMP_NAME.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
            //            if (user.Rows.Count > 0)
            //            {
            //                var EMP_ID = user.Rows[0][0].ToString() != string.Empty ? long.Parse(user.Rows[0][0].ToString()) : 0;
            //                var EMP_ID_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
            //                var EMP_SUB = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : "";
            //                int x = ExecuteOracleNonQueryCommand("update  DMS_02_EMP_D_ENT3 set "
            //                        + "EMP_ID =" + EMP_ID
            //                        + ",EMP_ID_ID=" + EMP_ID_ID
            //                        + ",EMP_SUB='" + EMP_SUB + "' WHERE D_ID=" + item.D_ID, _connectionSettings.OrcaleConnectionSH65);
            //                //var user = GetOracleDataTable("select USER_ID ,USER_CO,USER_N,USER_NAME from USERS_2 where USER_NAME='" + createdBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
            //                //if (user.Rows.Count > 0)
            //                //{
            //                //    var ID_PHARM = EMP_ID;
            //                //    iNV_SALOracle.PH_ID = EMP_ID_ID;
            //                //    iNV_SALOracle.PH_NAME = EMP_SUB;
            //                //    iNV_SALOracle.NAME_PHARM =item.EMP_NAME;
            //                //    iNV_SALOracle.ID_EX = EMP_ID.ToString();
            //                //    iNV_SALOracle.NAME_EX = item.EMP_NAME;

            //                //}
            //                int x2 = ExecuteOracleNonQueryCommand("update  INV_SAL3 set "
            //                        + "ID_PHARM =" + EMP_ID
            //                        + ",PH_ID=" + EMP_ID_ID
            //                        + ",PH_NAME='" + EMP_SUB + "',NAME_PHARM='" + item.EMP_NAME + "',ID_EX='" + EMP_ID.ToString() +
            //                        "',NAME_EX='" + item.EMP_NAME + "' WHERE INV_ID=" + item.D_ID, _connectionSettings.OrcaleConnectionSH65);

            //            }
            //        }
            //    }
            //}
            #endregion

            var query = "select * from (select m.*, rownum r from SH_01.DMS_02_EMP_D_ENT m WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC = 9) AND SYNC_BY='ORA') WHERE r > {0} and r<= {1}";

            double count = double.Parse(GetOracleDataTable("select  COUNT(*) from DMS_02_EMP_D_ENT WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA' ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());


            var maxiteration = Math.Ceiling(count / 1000);
            for (int i = 0; i < maxiteration; i = i)
            {
                List<DMS_02_EMP_D_ENT> data = new List<DMS_02_EMP_D_ENT>();
                var data2 = GetOracleDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);
                try
                {

                    data = data2.AsEnumerable().Select(row => new DMS_02_EMP_D_ENT
                    {
                        CARD_ID = row.IsNull("CARD_ID") ? "Now" : row.Field<string>("CARD_ID"),
                        D_VD = row.IsNull("D_VD") ? 0 : double.Parse(row.Field<decimal>("D_VD").ToString()),
                        D_ID = row.IsNull("D_ID") ? 0 : long.Parse(row.Field<decimal>("D_ID").ToString()),
                        TAKHASOS = row.IsNull("TAKHASOS") ? "Empty" : row.Field<string>("TAKHASOS"),
                        TASHKHES_01 = row.IsNull("TASHKHES_01") ? "Empty" : row.Field<string>("TASHKHES_01"),
                        TASHKHES_02 = row.IsNull("TASHKHES_02") ? "Empty" : row.Field<string>("TASHKHES_02"),
                        TASHKHES_03 = row.IsNull("TASHKHES_03") ? "Empty" : row.Field<string>("TASHKHES_03"),
                        INSU_LIMT = row.IsNull("INSU_LIMT") ? 0 : int.Parse(row.Field<decimal>("INSU_LIMT").ToString()),
                        OVER_INSURANCE = row.IsNull("OVER_INSURANCE") ? 0 : double.Parse(row.Field<decimal>("OVER_INSURANCE").ToString()),
                        VALUE_CREDIT = row.IsNull("VALUE_CREDIT") ? 0 : double.Parse(row.Field<decimal>("VALUE_CREDIT").ToString()),
                        CARRY = row.IsNull("CARRY") ? 0 : double.Parse(row.Field<decimal>("CARRY").ToString()),
                        VALUE_CASH = row.IsNull("VALUE_CASH") ? 0 : double.Parse(row.Field<decimal>("VALUE_CASH").ToString()),
                        MANAGER = row.IsNull("MANAGER") ? "Empty" : row.Field<string>("MANAGER"),
                        EMP_NAME = row.IsNull("EMP_NAME") ? "Empty" : row.Field<string>("EMP_NAME"),
                        MOB_NO = row.IsNull("MOB_NO") ? "Now" : row.Field<string>("MOB_NO"),
                        D_DATE = row.IsNull("D_DATE") ? DateTime.Now : (DateTime)row.Field<DateTime>("D_DATE"),

                    }).ToList();
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                }

                //var data = GetOracleTable<DMS_02_EMP_D_ENT>("select * from SH_01.DMS_02_EMP_D_ENT WHERE D_DATE>='04 FEB 2020' and D_ID= 50220201368045", _connectionSettings.OrcaleConnection);

                string tableName = "DMS_02_EMP_D_ENT";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    double CompanyPercent;
                    List<Roshita> roshitas = new List<Roshita>();
                    Roshita rsh = new Roshita();
                    foreach (var item in data)
                    {
                        rsh = FillRoshitaObject(item);
                        switch (item.MANAGER)
                        {
                            case "YES":
                                rsh.Manager = "Daily";
                                rsh.RoshetaType = "11601";
                                break;
                            case "MON":
                                rsh.Manager = "Pharmacy_Chronic";
                                rsh.RoshetaType = "11602";
                                break;
                            case "STOP_L":
                                rsh.Manager = "Lab_Stop";
                                rsh.RoshetaType = "11206";
                                break;
                            case "MON_PH":
                                rsh.Manager = "Monthly";
                                rsh.RoshetaType = "11603";
                                break;
                            case "STOP_PH":
                                rsh.Manager = "Daily_Stop";
                                rsh.RoshetaType = "11601";
                                break;
                            case "STOP_R":
                                rsh.Manager = "Ray_Stop";
                                rsh.RoshetaType = "11204";
                                break;
                            case "STOP_PH_MON":
                                rsh.Manager = "Monthly_Stop";
                                rsh.RoshetaType = "";
                                break;
                            case "LAB":
                                rsh.Manager = "Lab";
                                rsh.RoshetaType = "11206";
                                break;
                            case "RAY":
                                rsh.Manager = "Ray";
                                rsh.RoshetaType = "11204";
                                break;
                            case "DOC":
                                rsh.Manager = "Doctor_Daily";
                                rsh.RoshetaType = "11601";
                                break;
                            case "DOC_MON":
                                rsh.Manager = "Doctor_Chronic";
                                rsh.RoshetaType = "11602";
                                break;
                            case "STOP_DOC":
                                rsh.Manager = "Doctor_Daily_Stop";
                                rsh.RoshetaType = "11601";
                                break;
                            default:
                                rsh.Manager = item.MANAGER;
                                rsh.RoshetaType = "Empty";
                                break;
                        }
                        if (rsh.TotalValue == 0)
                        {
                            CompanyPercent = 0;
                            rsh.CompanyPercent = Double.IsNaN(CompanyPercent) ? 0 : CompanyPercent;
                            roshitas.Add(rsh);
                        }
                        else
                        {
                            CompanyPercent = ((rsh.TotalValue - rsh.PersonPayment) / rsh.TotalValue) * 100;
                            rsh.CompanyPercent = Double.IsNaN(CompanyPercent) ? 0 : CompanyPercent;
                            roshitas.Add(rsh);
                        }

                    }


                    try
                    {
                        //add new data
                        AddNewEntities(roshitas, "Roshita", false, _connectionSettings.SQlConnection);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                            Table = tableName,
                            AffectedRows = roshitas.Count
                        });
                        if (roshitas.Count == 1)
                        {
                            string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA'" +
                                " AND D_ID IN= " + roshitas[0].Oracle_Id;
                            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                        }
                        else if (roshitas.Count > 1)
                        {
                            string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA'" +
                                " AND D_ID IN( " + roshitas[0].Oracle_Id + ",";

                            for (int j = 1; j < roshitas.Count - 1; j++)
                            {
                                OracleQuery += roshitas[j].Oracle_Id + ",";
                            }
                            OracleQuery += roshitas[roshitas.Count - 1].Oracle_Id + ")";
                            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                        }
                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }
            }

            //string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) AND SYNC_BY='ORA' ";
            //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
        }

        private static void PullMan()
        {
            var query = "SELECT * FROM (SELECT m.*, ROWNUM r FROM SH_01.DMS_02_EMP_D_ENT_MAN m  INNER JOIN DMS_02_EMP_D_ENT e ON m.D_ID=e.D_ID WHERE (m.IS_SYNC = 0 OR m.IS_SYNC IS NULL OR m.IS_SYNC = 9) AND e.EMP_NAME='منوال') WHERE r >{0} AND r<= {1} ;";

            double count = double.Parse(GetOracleDataTable("select  COUNT(*) FROM SH_01.DMS_02_EMP_D_ENT_MAN m  INNER JOIN DMS_02_EMP_D_ENT e ON m.D_ID=e.D_ID WHERE (m.IS_SYNC = 0 OR m.IS_SYNC IS NULL OR m.IS_SYNC = 9) AND e.EMP_NAME='منوال' ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());


            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = i)
            {
                try
                {
                    var data = GetOracleTable<DMS_02_EMP_D_ENT_MAN>(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "DMS_02_EMP_D_ENT_MAN",
                        Note = "All",
                        AffectedRows = data.Count
                    });

                    try
                    {
                        var SyncData = FullSyncFieldsSQL<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                        var CastData = SyncData.Cast<DMS_02_EMP_D_ENT_MAN>().ToList();
                        foreach (var item in CastData)
                        {
                            switch (item.MANAGER)
                            {
                                case "YES":
                                case "MON":
                                case "MON_PH":
                                    if (item.CARD_ID.Split('-')[0].Contains("500"))
                                    {
                                        item.Gross = item.MAN_IMP + item.MAN_LOC;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        double? discountlocal, discountImp;
                                        if ((item.PERCENT_MONY_M + item.OVER_INSURANCE) > item.MAN_LOC)
                                        {
                                            discountlocal = 0;
                                            discountImp = (item.P_2 / 100) * (item.MAN_IMP + (item.MAN_LOC - item.PERCENT_MONY_M - item.OVER_INSURANCE));
                                        }
                                        else
                                        {
                                            discountlocal = ((item.P_3 / 100) * (item.MAN_LOC - item.PERCENT_MONY_M - item.OVER_INSURANCE));
                                            discountImp = (item.P_2 / 100) * item.MAN_IMP;
                                        }
                                        item.TotalDiscount = discountlocal + discountImp;
                                        item.Net = item.Gross - item.TotalDiscount - item.OVER_INSURANCE - item.PERCENT_MONY_M;
                                    }
                                    else
                                    {
                                        item.Gross = item.MAN_IMP + item.MAN_LOC;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        item.TotalDiscount = 0;
                                        item.Net = item.Gross - item.PERCENT_MONY_M - item.OVER_INSURANCE;
                                    }
                                    break;
                                case "LAB":
                                case "RAY":
                                    if (item.CARD_ID.Split('-')[0].Contains("500"))
                                    {
                                        item.Gross = item.TOT_DISC_EXP;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        if (item.DISC_TYPE_LOC == 9 || item.DISC_TYPE_LOC == 15)
                                        {
                                            item.Net = item.MAN_TOT;
                                            item.TotalDiscount = item.Gross - item.Net - item.PERCENT_MONY_M; ;
                                        }
                                        else
                                        {
                                            item.Net = item.Gross - item.OVER_INSURANCE - item.PERCENT_MONY_M;
                                            item.TotalDiscount = item.Gross - item.Net;
                                        }

                                    }
                                    else
                                    {
                                        item.Gross = item.TOT_DISC_EXP * 1.5;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        item.TotalDiscount = item.TOT_DISC_EXP * 0.15;
                                        item.Net = item.Gross - item.PERCENT_MONY_M - item.OVER_INSURANCE - item.TotalDiscount;


                                    }
                                    break;

                            }

                            switch (item.MANAGER)
                            {
                                case "YES":
                                    item.MANAGER = "Daily_Manual";

                                    break;
                                case "MON":
                                    item.MANAGER = "Pharmacy_Chronic_Manual";
                                    break;
                                case "MON_PH":
                                    item.MANAGER = "Monthly_Manual";
                                    break;
                                case "LAB":
                                    item.MANAGER = "Lab_Manual";
                                    break;
                                case "RAY":
                                    item.MANAGER = "Ray_Manual";
                                    break;
                                default:
                                    item.MANAGER = item.MANAGER + "_Manual";
                                    break;
                            }


                        }

                        AddNewEntities(CastData, "DMS_02_EMP_D_ENT_MAN", false, _connectionSettings.SQlConnection);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = "DMS_02_EMP_D_ENT_MAN",
                            AffectedRows = data.Count
                        });
                        if (CastData.Count == 1)
                        {
                            string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT_MAN SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) " +
                                " AND D_ID IN= " + CastData[0].D_ID;
                            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                        }
                        else if (CastData.Count > 1)
                        {
                            string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT_MAN SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) " +
                                " AND D_ID IN( " + CastData[0].D_ID + ",";

                            for (int j = 1; j < CastData.Count - 1; j++)
                            {
                                OracleQuery += CastData[j].D_ID + ",";
                            }
                            OracleQuery += CastData[CastData.Count - 1].D_ID + ")";
                            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = "DMS_02_EMP_D_ENT_MAN",
                            Exception = ex,
                        });
                    }
                }
                catch (Exception e)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "DMS_02_EMP_D_ENT_MAN",
                        Exception = e,
                    });
                }
            }

            var query2 = "SELECT * FROM (SELECT m.*, ROWNUM r FROM SH_01.DMS_02_EMP_D_ENT_MAN m   WHERE (m.IS_SYNC = 0 OR m.IS_SYNC IS NULL OR m.IS_SYNC = 9) ) WHERE r >{0} AND r<= {1} ;";

            double count2 = double.Parse(GetOracleDataTable("select  COUNT(*) FROM SH_01.DMS_02_EMP_D_ENT_MAN m  WHERE (m.IS_SYNC = 0 OR m.IS_SYNC IS NULL OR m.IS_SYNC = 9) ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());


            var maxiteration2 = Math.Ceiling(count2 / 1000);

            for (int i = 0; i < maxiteration2; i = i)
            {
                try
                {
                    var data = GetOracleTable<DMS_02_EMP_D_ENT_MAN>(string.Format(query2, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "DMS_02_EMP_D_ENT_MAN",
                        Note = "All",
                        AffectedRows = data.Count
                    });

                    try
                    {
                        var SyncData = FullSyncFieldsSQL<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                        var CastData = SyncData.Cast<DMS_02_EMP_D_ENT_MAN>().ToList();
                        foreach (var item in CastData)
                        {
                            switch (item.MANAGER)
                            {
                                case "YES":
                                case "MON":
                                case "MON_PH":
                                    if (item.CARD_ID.Split('-')[0].Contains("500"))
                                    {
                                        item.Gross = item.MAN_IMP + item.MAN_LOC;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        double? discountlocal, discountImp;
                                        if ((item.PERCENT_MONY_M + item.OVER_INSURANCE) > item.MAN_LOC)
                                        {
                                            discountlocal = 0;
                                            discountImp = (item.P_2 / 100) * (item.MAN_IMP + (item.MAN_LOC - item.PERCENT_MONY_M - item.OVER_INSURANCE));
                                        }
                                        else
                                        {
                                            discountlocal = ((item.P_3 / 100) * (item.MAN_LOC - item.PERCENT_MONY_M - item.OVER_INSURANCE));
                                            discountImp = (item.P_2 / 100) * item.MAN_IMP;
                                        }
                                        item.TotalDiscount = discountlocal + discountImp;
                                        item.Net = item.Gross - item.TotalDiscount - item.OVER_INSURANCE - item.PERCENT_MONY_M;
                                    }
                                    else
                                    {
                                        item.Gross = item.MAN_IMP + item.MAN_LOC;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        item.TotalDiscount = 0;
                                        item.Net = item.Gross - item.PERCENT_MONY_M - item.OVER_INSURANCE;
                                    }
                                    break;
                                case "LAB":
                                case "RAY":
                                    if (item.CARD_ID.Split('-')[0].Contains("500"))
                                    {
                                        item.Gross = item.TOT_DISC_EXP;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        if (item.DISC_TYPE_LOC == 9 || item.DISC_TYPE_LOC == 15)
                                        {
                                            item.Net = item.MAN_TOT;
                                            item.TotalDiscount = item.Gross - item.Net - item.PERCENT_MONY_M; ;
                                        }
                                        else
                                        {
                                            item.Net = item.Gross - item.OVER_INSURANCE - item.PERCENT_MONY_M;
                                            item.TotalDiscount = item.Gross - item.Net;
                                        }

                                    }
                                    else
                                    {
                                        item.Gross = item.TOT_DISC_EXP * 1.5;
                                        item.PersonPayment = item.PERCENT_MONY_M;
                                        item.OverInsurance = item.OVER_INSURANCE;
                                        item.TotalDiscount = item.TOT_DISC_EXP * 0.15;
                                        item.Net = item.Gross - item.PERCENT_MONY_M - item.OVER_INSURANCE - item.TotalDiscount;


                                    }
                                    break;

                            }

                            switch (item.MANAGER)
                            {
                                case "YES":
                                    item.MANAGER = "Daily_Manual";

                                    break;
                                case "MON":
                                    item.MANAGER = "Pharmacy_Chronic_Manual";
                                    break;
                                case "MON_PH":
                                    item.MANAGER = "Monthly_Manual";
                                    break;
                                case "LAB":
                                    item.MANAGER = "Lab_Manual";
                                    break;
                                case "RAY":
                                    item.MANAGER = "Ray_Manual";
                                    break;
                                default:
                                    item.MANAGER = item.MANAGER + "_Manual";
                                    break;
                            }


                        }

                        AddNewEntities(CastData, "DMS_02_EMP_D_ENT_MAN", false, _connectionSettings.SQlConnection);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = "DMS_02_EMP_D_ENT_MAN",
                            AffectedRows = data.Count
                        });
                        if (CastData.Count == 1)
                        {
                            string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT_MAN SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) " +
                                " AND D_ID IN= " + CastData[0].D_ID;
                            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                        }
                        else if (CastData.Count > 1)
                        {
                            string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT_MAN SET IS_SYNC = 1,SYNC_DATE=SYSDATE WHERE (IS_SYNC = 0 OR IS_SYNC IS NULL OR IS_SYNC=9) " +
                                " AND D_ID IN( " + CastData[0].D_ID + ",";

                            for (int j = 1; j < CastData.Count - 1; j++)
                            {
                                OracleQuery += CastData[j].D_ID + ",";
                            }
                            OracleQuery += CastData[CastData.Count - 1].D_ID + ")";
                            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = "DMS_02_EMP_D_ENT_MAN",
                            Exception = ex,
                        });
                    }
                }
                catch (Exception e)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnectionSH65),
                        Server = GetServerName(_connectionSettings.OrcaleConnectionSH65),
                        Table = "DMS_02_EMP_D_ENT_MAN",
                        Exception = e,
                    });
                }
            }


            //string OracleQuery = "UPDATE  SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
            //ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
        }

        private static void UpdatePullRoshita()
        {
            try
            {
                var query = "select * from (select m.*, rownum r from SH_01.DMS_02_EMP_D_ENT m WHERE IS_SYNC = 9 AND SYNC_BY='UPDATE') WHERE r > {0} and r<= {1}";

                double count = double.Parse(GetOracleDataTable("select  COUNT(*) from DMS_02_EMP_D_ENT WHERE  IS_SYNC = 9 AND SYNC_BY='UPDATE' ", _connectionSettings.OrcaleConnectionSH65).Rows[0][0].ToString());

                var maxiteration = Math.Ceiling(count / 1000);
                string manager = "";
                double CompanyPercent;
                double CompanyPercentsql;
                for (int i = 0; i < maxiteration; i = i)
                {
                    List<DMS_02_EMP_D_ENT> data = new List<DMS_02_EMP_D_ENT>();
                    var data2 = GetOracleDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);

                    try
                    {

                        data = data2.AsEnumerable().Select(row => new DMS_02_EMP_D_ENT
                        {
                            CARD_ID = row.IsNull("CARD_ID") ? "Now" : row.Field<string>("CARD_ID"),
                            D_VD = row.IsNull("D_VD") ? 0 : double.Parse(row.Field<decimal>("D_VD").ToString()),
                            D_ID = row.IsNull("D_ID") ? 0 : long.Parse(row.Field<decimal>("D_ID").ToString()),
                            TAKHASOS = row.IsNull("TAKHASOS") ? "Empty" : row.Field<string>("TAKHASOS"),
                            TASHKHES_01 = row.IsNull("TASHKHES_01") ? "Empty" : row.Field<string>("TASHKHES_01"),
                            TASHKHES_02 = row.IsNull("TASHKHES_02") ? "Empty" : row.Field<string>("TASHKHES_02"),
                            TASHKHES_03 = row.IsNull("TASHKHES_03") ? "Empty" : row.Field<string>("TASHKHES_03"),
                            INSU_LIMT = row.IsNull("INSU_LIMT") ? 0 : int.Parse(row.Field<decimal>("INSU_LIMT").ToString()),
                            OVER_INSURANCE = row.IsNull("OVER_INSURANCE") ? 0 : double.Parse(row.Field<decimal>("OVER_INSURANCE").ToString()),
                            VALUE_CREDIT = row.IsNull("VALUE_CREDIT") ? 0 : double.Parse(row.Field<decimal>("VALUE_CREDIT").ToString()),
                            CARRY = row.IsNull("CARRY") ? 0 : double.Parse(row.Field<decimal>("CARRY").ToString()),
                            VALUE_CASH = row.IsNull("VALUE_CASH") ? 0 : double.Parse(row.Field<decimal>("VALUE_CASH").ToString()),
                            MANAGER = row.IsNull("MANAGER") ? "Empty" : row.Field<string>("MANAGER"),
                            EMP_NAME = row.IsNull("EMP_NAME") ? "Empty" : row.Field<string>("EMP_NAME"),
                            MOB_NO = row.IsNull("MOB_NO") ? "Now" : row.Field<string>("MOB_NO"),
                            D_DATE = row.IsNull("D_DATE") ? DateTime.Now : (DateTime)row.Field<DateTime>("D_DATE"),

                        }).ToList();
                    }
                    catch (Exception ex)
                    {
                        var message = ex.Message;
                    }

                    if (data.Count != 0)
                    {
                        foreach (var item in data)
                        {
                            switch (item.MANAGER)
                            {
                                case "YES":
                                    manager = "Daily";
                                    break;
                                case "MON":
                                    manager = "Pharmacy_Chronic";
                                    break;
                                case "STOP_L":
                                    manager = "Lab_Stop";
                                    break;
                                case "MON_PH":
                                    manager = "Monthly";
                                    break;
                                case "STOP_PH":
                                    manager = "Daily_Stop";
                                    break;
                                case "STOP_R":
                                    manager = "Ray_Stop";
                                    break;
                                case "STOP_PH_MON":
                                    manager = "Monthly_Stop";
                                    break;
                                case "LAB":
                                    manager = "Lab";
                                    break;
                                case "RAY":
                                    manager = "Ray";
                                    break;
                                case "DOC":
                                    manager = "Doctor_Daily";
                                    break;
                                case "DOC_MON":
                                    manager = "Doctor_Chronic";
                                    break;
                                case "STOP_DOC":
                                    manager = "Doctor_Daily_Stop";
                                    break;
                                default:
                                    manager = item.MANAGER;
                                    break;
                            }

                            if (item.D_VD == 0)
                            {
                                CompanyPercent = 0;
                                CompanyPercentsql = Double.IsNaN(CompanyPercent) ? 0 : CompanyPercent;
                                CompanyPercentsql = Math.Round(CompanyPercentsql, 5);

                            }
                            else
                            {
                                CompanyPercent = ((item.D_VD - item.CARRY) / item.D_VD) * 100;
                                CompanyPercentsql = Double.IsNaN(CompanyPercent) ? 0 : CompanyPercent;
                                CompanyPercentsql = Math.Round(CompanyPercentsql, 5);

                            }
                            string UpdateRoshitaDetailsQuery = " UPDATE Roshita SET " +

                            " TotalValue =" + item.D_VD +
                            " , Limit = " + item.INSU_LIMT +
                            " , OverInsurance =" + item.OVER_INSURANCE +
                            " , CompanyPayment =" + item.VALUE_CREDIT +
                            " , PersonPayment =" + item.CARRY +
                            " , Cash = " + item.VALUE_CASH +
                            " , Manager ='" + manager +
                            "' , CompanyPercent =" + CompanyPercentsql +
                            " WHERE Oracle_Id='" + item.D_ID + "' AND CardId='" + item.CARD_ID + "'";
                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);

                            var sqlQuery = " SELECT Id FROM Roshita WHERE Oracle_Id='" + item.D_ID + "' AND CardId='" + item.CARD_ID + "'";

                            long id = long.Parse(GetSqlDataTable(sqlQuery, _connectionSettings.SQlConnection).Rows[0][0].ToString());
                            SetOrRemoveRelation("DELETE FROM RoshitaDetails WHERE RoshitaId=" + id, _connectionSettings.SQlConnection);

                            var OraDetails = " SELECT * FROM INV_SAL WHERE INV_ID= " + item.D_ID + " AND CARD_ID='" + item.CARD_ID + "'";
                            var OraDetailsRay = " SELECT * FROM INV_SAL_RAY WHERE INV_ID= " + item.D_ID + " AND CARD_ID='" + item.CARD_ID + "'";
                            var OraDetailsLab = " SELECT * FROM INV_SAL_LAB WHERE INV_ID= " + item.D_ID + " AND CARD_ID='" + item.CARD_ID + "'";
                            List<INV_SAL> dataDetails = new List<INV_SAL>();

                            var data3 = GetOracleDataTable(OraDetails, _connectionSettings.OrcaleConnectionSH65);
                            var dataRay = GetOracleDataTable(OraDetailsRay, _connectionSettings.OrcaleConnectionSH65);
                            var dataLab = GetOracleDataTable(OraDetailsLab, _connectionSettings.OrcaleConnectionSH65);

                            try
                            {
                                if (data3.Rows.Count > 0)
                                {
                                    dataDetails = data3.AsEnumerable().Select(row => new INV_SAL
                                    {
                                        INVT_NO = row.IsNull("INVT_NO") ? 0 : long.Parse(row.Field<decimal>("INVT_NO").ToString()),
                                        INVT_NAM = row.IsNull("INVT_NAM") ? "Now" : row.Field<string>("INVT_NAM"),
                                        DOSE = row.IsNull("DOSE") ? 0 : int.Parse(row.Field<int>("DOSE").ToString()),
                                        DURATION = row.IsNull("DURATION") ? 0 : int.Parse(row.Field<int>("DURATION").ToString()),
                                        T_DURATION = row.IsNull("T_DURATION") ? 0 : int.Parse(row.Field<int>("T_DURATION").ToString()),
                                        COUNT = row.IsNull("COUNT") ? 0 : int.Parse(row.Field<int>("COUNT").ToString()),
                                        KIND_EX = row.IsNull("KIND_EX") ? false : true,
                                        INV_ID = row.IsNull("INV_ID") ? 0 : long.Parse(row.Field<decimal>("INV_ID").ToString()),
                                        GRUOP_TYPE = row.IsNull("GRUOP_TYPE") ? "YES" : row.Field<string>("GRUOP_TYPE"),
                                        AMOUNT = row.IsNull("AMOUNT") ? 0 : double.Parse(row.Field<decimal>("AMOUNT").ToString()),

                                    }).ToList();
                                }
                                else if (dataRay.Rows.Count > 0)
                                {
                                    dataDetails = dataRay.AsEnumerable().Select(row => new INV_SAL
                                    {
                                        INVT_NO = row.IsNull("INVT_NO") ? 0 : long.Parse(row.Field<decimal>("INVT_NO").ToString()),
                                        INVT_NAM = row.IsNull("INVT_NAM") ? "Now" : row.Field<string>("INVT_NAM"),
                                        DOSE = row.IsNull("DOSE") ? 0 : int.Parse(row.Field<int>("DOSE").ToString()),
                                        DURATION = row.IsNull("DURATION") ? 0 : int.Parse(row.Field<int>("DURATION").ToString()),
                                        T_DURATION = row.IsNull("T_DURATION") ? 0 : int.Parse(row.Field<int>("T_DURATION").ToString()),
                                        COUNT = row.IsNull("COUNT") ? 0 : int.Parse(row.Field<int>("COUNT").ToString()),
                                        KIND_EX = row.IsNull("KIND_EX") ? false : true,
                                        INV_ID = row.IsNull("INV_ID") ? 0 : long.Parse(row.Field<decimal>("INV_ID").ToString()),
                                        GRUOP_TYPE = row.IsNull("GRUOP_TYPE") ? "YES" : row.Field<string>("GRUOP_TYPE"),
                                        AMOUNT = row.IsNull("AMOUNT") ? 0 : double.Parse(row.Field<decimal>("AMOUNT").ToString()),

                                    }).ToList();
                                }
                                else if (dataLab.Rows.Count > 0)
                                {
                                    dataDetails = dataLab.AsEnumerable().Select(row => new INV_SAL
                                    {
                                        INVT_NO = row.IsNull("INVT_NO") ? 0 : long.Parse(row.Field<decimal>("INVT_NO").ToString()),
                                        INVT_NAM = row.IsNull("INVT_NAM") ? "Now" : row.Field<string>("INVT_NAM"),
                                        DOSE = row.IsNull("DOSE") ? 0 : int.Parse(row.Field<int>("DOSE").ToString()),
                                        DURATION = row.IsNull("DURATION") ? 0 : int.Parse(row.Field<int>("DURATION").ToString()),
                                        T_DURATION = row.IsNull("T_DURATION") ? 0 : int.Parse(row.Field<int>("T_DURATION").ToString()),
                                        COUNT = row.IsNull("COUNT") ? 0 : int.Parse(row.Field<int>("COUNT").ToString()),
                                        KIND_EX = row.IsNull("KIND_EX") ? false : true,
                                        INV_ID = row.IsNull("INV_ID") ? 0 : long.Parse(row.Field<decimal>("INV_ID").ToString()),
                                        GRUOP_TYPE = row.IsNull("GRUOP_TYPE") ? "YES" : row.Field<string>("GRUOP_TYPE"),
                                        AMOUNT = row.IsNull("AMOUNT") ? 0 : double.Parse(row.Field<decimal>("AMOUNT").ToString()),

                                    }).ToList();
                                }

                            }
                            catch (Exception ex)
                            {
                                var message = ex.Message;
                            }
                            if (dataDetails.Count > 0)
                            {
                                foreach (var itemDetails in dataDetails)
                                {
                                    var sqlQueryInsert = " INSERT RoshitaDetails(MedicienCode,MedicienName,Dose,Duration,TotalDuration" +
                                        ",TotalUnits,Amount,IsDealed,RoshitaID,PaymentGroup,IsSync,SyncDate,SyncBy) VALUES (" +
                                        "N'" + itemDetails.INVT_NO + "',N'" + itemDetails.INVT_NAM + "'," + itemDetails.DOSE +
                                        "," + itemDetails.DURATION + "," + itemDetails.T_DURATION + "," + itemDetails.COUNT + "," + itemDetails.AMOUNT +
                                        ",'" + itemDetails.KIND_EX + "'," + id + ",N'" + itemDetails.GRUOP_TYPE + "',0,'" + DateTime.Now + "','Admin'"
                                        + ")";
                                    SetOrRemoveRelation(sqlQueryInsert, _connectionSettings.SQlConnection);
                                }
                            }

                        }
                    }
                }

                //var UpdatetableOra = " UPDATE DMS_02_EMP_D_ENT SET IS_SYNC=1,SYNC_BY='ORA',SYNC_DATE=SYSDATE";
                string OracleQuery = "UPDATE  SH_01.DMS_02_EMP_D_ENT SET IS_SYNC=1,SYNC_BY='ORA',SYNC_DATE=SYSDATE WHERE IS_SYNC = 9  AND SYNC_BY='UPDATE' ";
                ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        private static void PullUsers()
        {
            var data = GetOracleTable<Models.User>("select * from USERS_2 WHERE IS_SYNC=0 OR IS_SYNC IS NULL ", _connectionSettings.OrcaleConnectionSH65);

            string tableName = "user";
            _result.Logs.Add(new ViewModels.Log
            {
                Order = GetLogOrder(),
                Action = SyncAction.Get.ToString(),
                Database = GetDatabaseName(),
                Server = GetServerName(),
                Note = "Not Synced",
                Table = tableName,
                AffectedRows = data.Count
            });
            if (data.Count != 0)
            {
                // return new SyncTableBackResponse<Models.EMPLOYEES_CODING, BackModels.EMPLOYEES_CODING>();
                var newData = data.Select(e => new Models.AspNetUser
                {
                    Id = GetGuid().ToString(),
                    UserName = e.USER_NAME,
                    PasswordHash = "ABoAFNTZ8Nw7CXxjJltZyC0cXXIsIBvsVI/mYTgZmxktwJ7/aVDYZK+P/luboc7wQg==",//GetPassword(e.USER_PWD),
                    SecurityStamp = "5628b334-82e8-4f19-9cc3-4cdb63b6dbd8",
                    Address = e.ADDRS,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    PhoneNumber = e.TEL,
                    Provider = e.USER_CO.ToString(),
                    Type = e.KIND_NO.ToString()
                }).ToList();
                try
                {
                    //add new data
                    AddNewEntities(newData, "AspNetUsers", false, _connectionSettings.SQlConnection);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = tableName,
                        AffectedRows = newData.Count
                    });

                }
                catch (Exception ex)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = tableName,
                        Exception = ex,
                    });
                }
                //update is sync
                //string query = $"update {tableName} set IsSync = 1, SyncDate = '{DateTime.Now.ToShortDateString()}' where lower(EMP_ID) in ({ GetJoinedString(codesForUpdate)})";
                //ExecuteNonQueryCommand(query, _connectionSettings.OrcaleConnection);
            }

        }
        private static void PullRoles()
        {
            var data = GetOracleTable<Models.User>("select * from USERS_2 Where KIND_NO=1 AND  (IS_SYNC=0 OR IS_SYNC IS NULL)", _connectionSettings.OrcaleConnectionSH65);

            string tableName = "users";
            _result.Logs.Add(new ViewModels.Log
            {
                Order = GetLogOrder(),
                Action = SyncAction.Get.ToString(),
                Database = GetDatabaseName(),
                Server = GetServerName(),
                Note = "Not Synced",
                Table = tableName,
                AffectedRows = data.Count
            });
            if (data.Count != 0)
            {
                var newData1 = data.Select(e => new Models.AspNetUserRoles//pharmacy
                {

                    UserId = GetUserId(e.USER_NAME),
                    RoleId = "9579fdc0-6b53-430c-9a40-cbfcd45b3b22",

                }).ToList();

                data = GetOracleTable<Models.User>("select * from USERS_2 Where KIND_NO=2 AND  (IS_SYNC=0 OR IS_SYNC IS NULL)", _connectionSettings.OrcaleConnectionSH65);
                var newData2 = data.Select(e => new Models.AspNetUserRoles//Admin 71654125
                {
                    UserId = GetUserId(e.USER_NAME),
                    RoleId = "d01032fc-c0e8-44ca-a262-b04f4ccfe628",

                }).ToList();

                data = GetOracleTable<Models.User>("select * from USERS_2 Where KIND_NO=3 AND  (IS_SYNC=0 OR IS_SYNC IS NULL)", _connectionSettings.OrcaleConnectionSH65);
                var newData3 = data.Select(e => new Models.AspNetUserRoles//lab
                {

                    UserId = GetUserId(e.USER_NAME),
                    RoleId = "51065c78-a84f-4bd4-b14b-82f53374e508",

                }).ToList();
                data = GetOracleTable<Models.User>("select * from USERS_2 Where KIND_NO=4 AND  (IS_SYNC=0 OR IS_SYNC IS NULL)", _connectionSettings.OrcaleConnectionSH65);
                var newData4 = data.Select(e => new Models.AspNetUserRoles//ray
                {
                    UserId = GetUserId(e.USER_NAME),
                    RoleId = "51a546e8-7cf1-44b2-91b5-819029093d58",

                }).ToList();



                //add new
                try
                {
                    //add new data
                    AddNewEntities(newData1, "AspNetUserRoles", false, _connectionSettings.SQlConnection);
                    AddNewEntities(newData2, "AspNetUserRoles", false, _connectionSettings.SQlConnection);
                    AddNewEntities(newData3, "AspNetUserRoles", false, _connectionSettings.SQlConnection);
                    AddNewEntities(newData4, "AspNetUserRoles", false, _connectionSettings.SQlConnection);

                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = tableName,
                        AffectedRows = newData1.Count
                    });


                }
                catch (Exception ex)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Insert.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = tableName,
                        Exception = ex,
                    });
                }
                string OracleQuery = "UPDATE USERS_2 SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin'  WHERE IS_SYNC=0 OR IS_SYNC IS NULL ";
                ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);

            }
        }

        private static string GetUserId(string UserName)
        {

            List<AspNetUser> user = GetTable<AspNetUser>("select * from AspNetUsers where UserName=N'" + UserName + "' COLLATE SQL_Latin1_General_CP1_CS_AS", _connectionSettings.SQlConnection);
            return user[0].Id;

        }
        private static string GetPassword(string UserPassword)
        {
            string Password = "";
            return Password;

        }

        private static Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        private static DataTable GetSqlDataTable(string query, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            if (connectionString == null)
            {
                throw new Exception("connectionString is null and you didn't pass connection as a parameter! You must set connectionString");
            }

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = 120;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException;
            }

            return dt;
        }

        private static List<Swap> FillSwapObject(List<INV_SAL> INV_SAL)
        {
            List<Swap> swap = new List<Swap>();
            foreach (var item in INV_SAL)
            {
                swap.Add(new Swap
                {
                    Id = item.INV_ID
                });
            }
            return swap;

        }

        private static Roshita FillRoshitaObject(DMS_02_EMP_D_ENT obj)
        {
            string name = "Empty";
            if (obj.EMP_NAME != null)
            {
                name = obj.EMP_NAME;
            }
            return new Roshita
            {
                CardId = obj.CARD_ID,
                TotalValue = obj.D_VD,
                Speciality = obj.TAKHASOS,
                Diagnose1 = obj.TASHKHES_01,
                Diagnose2 = obj.TASHKHES_02,
                diagnose3 = obj.TASHKHES_03,
                Limit = obj.INSU_LIMT,
                OverInsurance = obj.OVER_INSURANCE,
                CompanyPayment = obj.VALUE_CREDIT,
                PersonPayment = obj.CARRY,
                Cash = obj.VALUE_CASH,
                Manager = obj.MANAGER,
                CreatedBy = name,
                PhoneNumber = obj.MOB_NO,
                CreatedDate = obj.D_DATE,
                IsSync = true,
                SyncDate = DateTime.Now,
                SyncBy = "Admin",
                Oracle_Id = obj.D_ID

            };
        }

        private static List<RoshitaDetail> FillRoshitaDetailsObject(List<INV_SAL> INV_SAL, List<JoinSwap> joinSwap)
        {
            List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();
            for (var item = 0; item < INV_SAL.Count; ++item)
            {
                roshitaDetail.Add(new RoshitaDetail
                {
                    MedicienCode = INV_SAL[item].INVT_NO.ToString(),
                    MedicienName = INV_SAL[item].INVT_NAM,
                    Dose = INV_SAL[item].DOSE,
                    Duration = INV_SAL[item].DURATION,
                    TotalDuration = INV_SAL[item].T_DURATION,
                    TotalUnits = INV_SAL[item].COUNT,
                    IsDealed = INV_SAL[item].KIND_EX,
                    RoshitaID = GetSqlRoshitaID(INV_SAL[item].INV_ID, joinSwap),
                    PaymentGroup = INV_SAL[item].GRUOP_TYPE,
                    Amount = INV_SAL[item].AMOUNT,
                    IsSync = true,
                    SyncDate = DateTime.Now,
                    SyncBy = "Admin"
                });
            }
            return roshitaDetail;
        }
        private static long GetSqlRoshitaID(long INV_ID, List<JoinSwap> joinSwap)
        {
            var result1 = joinSwap.Where(o => o.Oracle_Id == INV_ID).FirstOrDefault();
            long result = 0;
            if (result1 != null)
            {
                result = result1.RoshitaID;
            }
            return (result == 0) ? 0 : result;
        }
        private static void SetOrRemoveRelation(string Query, string connectionString = null)
        {

            connectionString = connectionString == null ? _currenctConnectionString : connectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(Query, connection);
                try
                {
                    connection.Open();
                    command.ExecuteReader();
                    connection.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }

        private static void ExecuteSQLUpdateQuery(string Query, string connectionString = null)
        {

            connectionString = connectionString == null ? _currenctConnectionString : connectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(Query, connection);
                try
                {
                    connection.Open();
                    var x = command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }


        private static int ExecuteSQLUpdateQuery2(string Query, string connectionString = null)
        {

            connectionString = connectionString == null ? _currenctConnectionString : connectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(Query, connection);
                try
                {
                    connection.Open();
                    var x = command.ExecuteNonQuery();
                    connection.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }


        private static void ExecuteOracleQuery(string Query, string connectionString = null)
        {

            connectionString = connectionString == null ? _currenctConnectionString : connectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand command = new OracleCommand(Query, connection);
                try
                {
                    connection.Open();
                    command.CommandText = Query;
                    int x = command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }

        private static List<INV_SAL> Inv_Sal_Medicine(int j, string query)
        {
            List<INV_SAL> data = new List<INV_SAL>();

            var data2 = GetOracleDataTable(string.Format(query, (j * 1000), ((++j) * 1000)), _connectionSettings.OrcaleConnectionSH65);

            try
            {

                data = data2.AsEnumerable().Select(row => new INV_SAL
                {
                    INVT_NO = row.IsNull("INVT_NO") ? 0 : long.Parse(row.Field<decimal>("INVT_NO").ToString()),
                    INVT_NAM = row.IsNull("INVT_NAM") ? "Now" : row.Field<string>("INVT_NAM"),
                    DOSE = row.IsNull("DOSE") ? 0 : int.Parse(row.Field<int>("DOSE").ToString()),
                    DURATION = row.IsNull("DURATION") ? 0 : int.Parse(row.Field<int>("DURATION").ToString()),
                    T_DURATION = row.IsNull("T_DURATION") ? 0 : int.Parse(row.Field<int>("T_DURATION").ToString()),
                    COUNT = row.IsNull("COUNT") ? 0 : int.Parse(row.Field<int>("COUNT").ToString()),
                    KIND_EX = row.IsNull("KIND_EX") ? false : true,
                    INV_ID = row.IsNull("INV_ID") ? 0 : long.Parse(row.Field<decimal>("INV_ID").ToString()),
                    GRUOP_TYPE = row.IsNull("GRUOP_TYPE") ? "YES" : row.Field<string>("GRUOP_TYPE"),
                    AMOUNT = row.IsNull("AMOUNT") ? 0 : double.Parse(row.Field<decimal>("AMOUNT").ToString()),

                }).ToList();

            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            string tableName = "INV_SAL";
            _result.Logs.Add(new ViewModels.Log
            {
                Order = GetLogOrder(),
                Action = SyncAction.Get.ToString(),
                Database = GetDatabaseName(),
                Server = GetServerName(),
                Note = "Not Synced",
                Table = tableName,
                AffectedRows = data.Count
            });
            return data;
        }

        private static List<INV_SAL> Inv_Sal_Lab(int j, string query)
        {
            List<INV_SAL> data = new List<INV_SAL>();

            var data2 = GetOracleDataTable(string.Format(query, (j * 1000), ((++j) * 1000)), _connectionSettings.OrcaleConnectionSH65);

            try
            {

                data = data2.AsEnumerable().Select(row => new INV_SAL
                {
                    INVT_NO = row.IsNull("INVT_NO") ? 0 : long.Parse(row.Field<decimal>("INVT_NO").ToString()),
                    INVT_NAM = row.IsNull("INVT_NAM") ? "Now" : row.Field<string>("INVT_NAM"),
                    DOSE = row.IsNull("DOSE") ? 0 : int.Parse(row.Field<int>("DOSE").ToString()),
                    DURATION = row.IsNull("DURATION") ? 0 : int.Parse(row.Field<int>("DURATION").ToString()),
                    T_DURATION = row.IsNull("T_DURATION") ? 0 : int.Parse(row.Field<int>("T_DURATION").ToString()),
                    COUNT = row.IsNull("COUNT") ? 0 : int.Parse(row.Field<int>("COUNT").ToString()),
                    KIND_EX = row.IsNull("KIND_EX") ? false : true,
                    INV_ID = row.IsNull("INV_ID") ? 0 : long.Parse(row.Field<decimal>("INV_ID").ToString()),
                    GRUOP_TYPE = row.IsNull("GRUOP_TYPE") ? "YES" : row.Field<string>("GRUOP_TYPE"),
                    AMOUNT = row.IsNull("AMOUNT") ? 0 : double.Parse(row.Field<decimal>("AMOUNT").ToString()),

                }).ToList();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            string tableName = "INV_SAL_LAB";
            _result.Logs.Add(new ViewModels.Log
            {
                Order = GetLogOrder(),
                Action = SyncAction.Get.ToString(),
                Database = GetDatabaseName(),
                Server = GetServerName(),
                Note = "Not Synced",
                Table = tableName,
                AffectedRows = data.Count
            });
            return data;
        }

        private static List<INV_SAL> Inv_Sal_Ray(int j, string query)
        {

            List<INV_SAL> data = new List<INV_SAL>();


            var data2 = GetOracleDataTable(string.Format(query, (j * 1000), ((++j) * 1000)), _connectionSettings.OrcaleConnectionSH65);

            try
            {

                data = data2.AsEnumerable().Select(row => new INV_SAL
                {
                    INVT_NO = row.IsNull("INVT_NO") ? 0 : long.Parse(row.Field<decimal>("INVT_NO").ToString()),
                    INVT_NAM = row.IsNull("INVT_NAM") ? "Now" : row.Field<string>("INVT_NAM"),
                    DOSE = row.IsNull("DOSE") ? 0 : int.Parse(row.Field<int>("DOSE").ToString()),
                    DURATION = row.IsNull("DURATION") ? 0 : int.Parse(row.Field<int>("DURATION").ToString()),
                    T_DURATION = row.IsNull("T_DURATION") ? 0 : int.Parse(row.Field<int>("T_DURATION").ToString()),
                    COUNT = row.IsNull("COUNT") ? 0 : int.Parse(row.Field<int>("COUNT").ToString()),
                    KIND_EX = row.IsNull("KIND_EX") ? false : true,
                    INV_ID = row.IsNull("INV_ID") ? 0 : long.Parse(row.Field<decimal>("INV_ID").ToString()),
                    GRUOP_TYPE = row.IsNull("GRUOP_TYPE") ? "YES" : row.Field<string>("GRUOP_TYPE"),
                    AMOUNT = row.IsNull("AMOUNT") ? 0 : double.Parse(row.Field<decimal>("AMOUNT").ToString()),

                }).ToList();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            string tableName = "INV_SAL_RAY";
            _result.Logs.Add(new ViewModels.Log
            {
                Order = GetLogOrder(),
                Action = SyncAction.Get.ToString(),
                Database = GetDatabaseName(),
                Server = GetServerName(),
                Note = "Not Synced",
                Table = tableName,
                AffectedRows = data.Count
            });
            return data;
        }


        private static void PushSqlTables<T>(string query, string tableName)
        {
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  " + tableName +
                " WHERE (IsSync=0 OR IsSync IS NULL) ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            //tableName = tableName.Split('.')[1];
            for (int i = 0; i < maxiteration; i = i)
            {
                try
                {
                    var data = GetTable<T>(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.SQlConnection),
                        Server = GetServerName(_connectionSettings.SQlConnection),
                        Table = tableName,
                        Note = "All",
                        AffectedRows = data.Count
                    });

                    try
                    {
                        var SyncData = FullSyncFieldsORA<BaseEntityDB>(data.Cast<BaseEntityDB>().ToList());
                        var CastData = data.Cast<T>().ToList();
                        string SqlTableName;

                        switch (tableName)
                        {
                            case "MedicineData":
                                SqlTableName = "MEDICINE_DATA";
                                break;
                            case "MedicineGroup":
                                SqlTableName = "MEDICINE_GRUOP";
                                break;
                            default:
                                SqlTableName = tableName;
                                break;
                        }

                        AddNewEntities(CastData, SqlTableName, false, _connectionSettings.SQlConnection);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = tableName,
                            AffectedRows = data.Count
                        });
                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }
                catch (Exception e)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.SQlConnection),
                        Server = GetServerName(_connectionSettings.SQlConnection),
                        Table = tableName,
                        Exception = e,
                    });
                }
            }

            string UpdateMedCardQuery = "UPDATE " + tableName + " SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE()" +
               " WHERE IsSync=0 OR IsSync IS NULL ";
            _ = ExecuteNonQueryCommand(UpdateMedCardQuery, _connectionSettings.SQlConnection);

            //string UpdateMedCardQuery = "UPDATE Med_Card SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE() " +
            //   " WHERE IsSync=0 OR IsSync IS NULL ";
            //var resultmessage = ExecuteNonQueryCommand(UpdateMedCardQuery, _connectionSettings.SQlConnection);


        }

        private static void PushMedicineData()
        {
            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Id DESC) AS RowNum, * FROM MedicineData" +
                 " WHERE (IsSync=0 OR IsSync IS NULL )) " +
                 "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            //AND CAST(createddate AS date) = CAST('"+dateTime+"' AS date)
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  MedicineData " +
                " WHERE (IsSync=0 OR IsSync IS NULL) ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = i)
            {

                List<MEDICINE_DATA> data = new List<MEDICINE_DATA>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new MEDICINE_DATA
                    {
                        BRANCH_CODE = row.IsNull("BRANCH_CODE") ? (int?)null : row.Field<int>("BRANCH_CODE"),
                        COMP_ID = row.IsNull("COMP_ID") ? (int?)null : row.Field<int>("COMP_ID"),
                        UNIT_NO = row.IsNull("UNIT_NO") ? (int?)null : row.Field<int>("UNIT_NO"),
                        M_CODE = row.IsNull("M_CODE") ? null : row.Field<string>("M_CODE"),
                        LIC_TYPE = row.IsNull("LIC_TYPE") ? null : row.Field<string>("LIC_TYPE"),
                        MED_GROUP = row.IsNull("MED_GROUP") ? null : row.Field<string>("MED_GROUP"),
                        TRADE_NAME = row.IsNull("TRADE_NAME") ? null : row.Field<string>("TRADE_NAME"),
                        DOSAGE_FORM = row.IsNull("DOSAGE_FORM") ? null : row.Field<string>("DOSAGE_FORM"),
                        M_TYPE = row.IsNull("M_TYPE") ? null : row.Field<string>("M_TYPE"),
                        CON_MED = row.IsNull("CON_MED") ? null : row.Field<string>("CON_MED"),
                        ACTIVE = row.IsNull("ACTIVE") ? null : row.Field<string>("ACTIVE"),
                        GRN_CODE = row.IsNull("GRN_CODE") ? null : row.Field<string>("GRN_CODE"),
                        PACK_SIZE = row.IsNull("PACK_SIZE") ? (double?)null : row.Field<double>("PACK_SIZE"),
                        PACK_PRICE = row.IsNull("PACK_PRICE") ? (double?)null : row.Field<double>("PACK_PRICE"),
                        UNIT_PRICE = row.IsNull("UNIT_PRICE") ? (double?)null : row.Field<double>("UNIT_PRICE"),
                        CREATED_BY = row.IsNull("CREATED_BY") ? null : row.Field<string>("CREATED_BY"),
                        CREATED_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        UPDATE_BY = row.IsNull("UPDATE_BY") ? null : row.Field<string>("UPDATE_BY"),
                        UPDATE_DATE = row.IsNull("UPDATE_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("UPDATE_DATE"),
                        REG_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        STOP_ID = 0,
                        GRUOP_ID = 0,
                        IS_SYNC = 1,
                        SYNC_BY = "SQL",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "MEDICINE_DATA";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    try
                    {
                        AddNewEntitiesSql2(data, "MEDICINE_DATA", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "MEDICINE_DATA",
                            AffectedRows = data.Count
                        });

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }

            }

            string UpdateMedCardQuery = "UPDATE MedicineData SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE() " +
               " WHERE IsSync=0 OR IsSync IS NULL ";
            var resultmessage = ExecuteNonQueryCommand(UpdateMedCardQuery, _connectionSettings.SQlConnection);

        }

        private static void UpdatePushMedicineData()
        {
            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Id DESC) AS RowNum, * FROM MedicineData" +
                 " WHERE SyncBy='Updated') " +
                 "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            //AND CAST(createddate AS date) = CAST('"+dateTime+"' AS date)
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  MedicineData " +
                " WHERE SyncBy='Updated' ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = i)
            {

                List<MEDICINE_DATA> data = new List<MEDICINE_DATA>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new MEDICINE_DATA
                    {
                        BRANCH_CODE = row.IsNull("BRANCH_CODE") ? (int?)null : row.Field<int>("BRANCH_CODE"),
                        COMP_ID = row.IsNull("COMP_ID") ? (int?)null : row.Field<int>("COMP_ID"),
                        UNIT_NO = row.IsNull("UNIT_NO") ? (int?)null : row.Field<int>("UNIT_NO"),
                        M_CODE = row.IsNull("M_CODE") ? null : row.Field<string>("M_CODE"),
                        LIC_TYPE = row.IsNull("LIC_TYPE") ? null : row.Field<string>("LIC_TYPE"),
                        MED_GROUP = row.IsNull("MED_GROUP") ? null : row.Field<string>("MED_GROUP"),
                        TRADE_NAME = row.IsNull("TRADE_NAME") ? null : row.Field<string>("TRADE_NAME"),
                        DOSAGE_FORM = row.IsNull("DOSAGE_FORM") ? null : row.Field<string>("DOSAGE_FORM"),
                        M_TYPE = row.IsNull("M_TYPE") ? null : row.Field<string>("M_TYPE"),
                        CON_MED = row.IsNull("CON_MED") ? null : row.Field<string>("CON_MED"),
                        ACTIVE = row.IsNull("ACTIVE") ? null : row.Field<string>("ACTIVE"),
                        GRN_CODE = row.IsNull("GRN_CODE") ? null : row.Field<string>("GRN_CODE"),
                        PACK_SIZE = row.IsNull("PACK_SIZE") ? (double?)null : row.Field<double>("PACK_SIZE"),
                        PACK_PRICE = row.IsNull("PACK_PRICE") ? (double?)null : row.Field<double>("PACK_PRICE"),
                        UNIT_PRICE = row.IsNull("UNIT_PRICE") ? (double?)null : row.Field<double>("UNIT_PRICE"),
                        CREATED_BY = row.IsNull("CREATED_BY") ? null : row.Field<string>("CREATED_BY"),
                        CREATED_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        UPDATE_BY = row.IsNull("UPDATE_BY") ? null : row.Field<string>("UPDATE_BY"),
                        UPDATE_DATE = row.IsNull("UPDATE_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("UPDATE_DATE"),
                        REG_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        STOP_ID = 0,
                        GRUOP_ID = 0,
                        IS_SYNC = 9,
                        SYNC_BY = "UPDATE",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "MEDICINE_DATA";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    try
                    {
                        AddNewEntitiesSql2(data, "MEDICINE_DATA", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "MEDICINE_DATA",
                            AffectedRows = data.Count
                        });

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }

            }

            string UpdateMedCardQuery = "UPDATE MedicineData SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE() " +
               " WHERE IsSync=0 OR IsSync IS NULL ";
            var resultmessage = ExecuteNonQueryCommand(UpdateMedCardQuery, _connectionSettings.SQlConnection);

        }

        private static void PushMedicineGroup()
        {
            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY GroupId DESC) AS RowNum, * FROM MedicineGroup" +
                 " WHERE (IsSync=0 OR IsSync IS NULL )) " +
                 "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            //AND CAST(createddate AS date) = CAST('"+dateTime+"' AS date)
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  MedicineGroup " +
                " WHERE (IsSync=0 OR IsSync IS NULL) ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = i)
            {

                List<MEDICINE_GRUOP> data = new List<MEDICINE_GRUOP>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new MEDICINE_GRUOP
                    {
                        GRUOP_ID = row.IsNull("GroupId") ? int.Parse(null) : int.Parse(row.Field<string>("GroupId")),
                        GRUOP_NAME = row.IsNull("GroupName") ? null : row.Field<string>("GroupName"),
                        GRUOP_TYPE = row.IsNull("GroupType") ? null : row.Field<string>("GroupType"),
                        IS_SYNC = 1,
                        SYNC_BY = "SQL",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "MEDICINE_GRUOP";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    try
                    {
                        AddNewEntitiesSql2(data, "MEDICINE_GRUOP", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "MEDICINE_GRUOP",
                            AffectedRows = data.Count
                        });

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }

            }

            string UpdateMedCardQuery = "UPDATE MedicineGroup SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE() " +
               " WHERE IsSync=0 OR IsSync IS NULL ";
            var resultmessage = ExecuteNonQueryCommand(UpdateMedCardQuery, _connectionSettings.SQlConnection);


        }


        private static void PushRoshitaDiagnosisAdmin()
        {
            var query = @"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY r.CreatedDate DESC) AS RowNum, r.*,ro.Oracle_Id 
                            FROM RoshitaDiagnosisAdmin r
                            join Roshita ro on ro.Id=r.RoshitaId 
                            WHERE (r.IsSync=0 OR r.IsSync IS NULL ) OR 
                            ( r.SyncBy = 'Updated' AND r.IsSync=1 ) )
                            AS m WHERE RowNum > 0 AND RowNum<= 1000";
            //var query = @"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CreatedDate DESC) AS RowNum, * FROM RoshitaDiagnosisAdmin" +
            //     " WHERE (IsSync=0 OR IsSync IS NULL ) OR ( SyncBy = 'Updated' AND IsSync=1 ) ) " +
            //     "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  RoshitaDiagnosisAdmin " +
                " WHERE (IsSync=0 OR IsSync IS NULL) OR ( SyncBy = 'Updated' AND IsSync=1 )  ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = i)
            {

                List<ADMINDIAGNOSIS> data = new List<ADMINDIAGNOSIS>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new ADMINDIAGNOSIS
                    {
                        ORACLEID = row.IsNull("Oracle_Id") ? null : (string)row.Field<long>("Oracle_Id").ToString(),
                        SPECIALIST = row.IsNull("SpecialistName") ? null : row.Field<string>("SpecialistName"),
                        DIAGNOSIS = row.IsNull("DiagnoiseName") ? null : row.Field<string>("DiagnoiseName"),
                        CREATEDBY = row.IsNull("CreatedBy") ? null : row.Field<string>("CreatedBy"),
                        CREATEDDATE = row.IsNull("CreatedDate") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CreatedDate"),
                        UPDATEDBY = row.IsNull("UpdatedBy") ? null : row.Field<string>("UpdatedBy"),
                        UPDATEDDATE = row.IsNull("UpdatedDate") ? (DateTime?)null : (DateTime)row.Field<DateTime>("UpdatedDate"),
                        IS_SYNC = 1,
                        SYNC_BY = "SQL",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "RoshitaDiagnosisAdmin";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    try
                    {
                        AddNewEntitiesSql2(data, "ADMINDIAGNOSIS", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Table = "ADMINDIAGNOSIS",
                            AffectedRows = data.Count
                        });

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }

            }

            string UpdateMedCardQuery = "UPDATE RoshitaDiagnosisAdmin SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE() " +
               " WHERE IsSync=0 OR IsSync IS NULL ";
            var resultmessage = ExecuteNonQueryCommand(UpdateMedCardQuery, _connectionSettings.SQlConnection);
            string UpdateMedCardQuery2 = "UPDATE RoshitaDiagnosisAdmin SET IsSync=1,SyncBy='SQLTESTUpdate',SyncDate=GETDATE() " +
               " WHERE  SyncBy = 'Updated' AND IsSync=1  ";
            var resultmessage2 = ExecuteNonQueryCommand(UpdateMedCardQuery2, _connectionSettings.SQlConnection);

        }

        private static void PushMedCard()
        {
            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CREATED_DATE DESC) AS RowNum, * FROM Med_Card" +
                 " WHERE (IsSync=0 OR IsSync IS NULL ) OR ( SyncBy = 'Updated' AND IsSync=1 ) ) " +
                 "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            //AND CAST(createddate AS date) = CAST('"+dateTime+"' AS date)
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  Med_Card " +
                " WHERE (IsSync=0 OR IsSync IS NULL) OR ( SyncBy = 'Updated' AND IsSync=1 )  ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = i)
            {

                List<MED_CARD> data = new List<MED_CARD>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new MED_CARD
                    {
                        CARD_NO = row.IsNull("CARD_NO") ? null : row.Field<string>("CARD_NO"),
                        PROVIDER_CODE = row.IsNull("PROVIDER_CODE") ? int.Parse(null) : row.Field<int>("PROVIDER_CODE"),
                        C_COMP_ID = row.IsNull("C_COMP_ID") ? int.Parse(null) : row.Field<int>("C_COMP_ID"),
                        NOTES = row.IsNull("NOTES") ? null : row.Field<string>("NOTES"),
                        CREATED_BY = row.IsNull("CREATED_BY") ? null : row.Field<string>("CREATED_BY"),
                        CREATED_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        UPDATE_BY = row.IsNull("UPDATE_BY") ? null : row.Field<string>("UPDATE_BY"),
                        UPDATE_DATE = row.IsNull("UPDATE_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("UPDATE_DATE"),
                        MONTH_START_DATE = row.IsNull("MONTH_START_DATE") ? (DateTime)row.Field<DateTime>("CREATED_DATE") : (DateTime)row.Field<DateTime>("MONTH_START_DATE"),
                        MONTH_END_DATE = row.IsNull("MONTH_END_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("MONTH_END_DATE"),
                        GROUP_ID = row.IsNull("GROUP_ID") ? int.Parse(null) : row.Field<int>("GROUP_ID"),
                        GROUP_NAME = row.IsNull("GROUP_NAME") ? null : row.Field<string>("GROUP_NAME"),
                        LOOK_01 = row.IsNull("LOOK_01") ? (int?)null : row.Field<int>("LOOK_01"),
                        SEQ = row.IsNull("SEQ") ? (int?)null : row.Field<int>("SEQ"),
                        PROVIDER_CODE_OLD = row.IsNull("PROVIDER_CODE_OLD") ? (int?)null : row.Field<int>("PROVIDER_CODE_OLD"),
                        TASHKHES_01 = row.IsNull("TASHKHES_01") ? null : row.Field<string>("TASHKHES_01"),
                        NO_PAY = row.IsNull("NO_PAY") ? int.Parse(null) : row.Field<int>("NO_PAY"),
                        NO_OVER = row.IsNull("NO_OVER") ? int.Parse(null) : row.Field<int>("NO_OVER"),
                        ST_DAY = row.IsNull("ST_DAY") ? int.Parse(null) : row.Field<int>("ST_DAY"),
                        REG_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        IS_SYNC = 1,
                        SYNC_BY = "SQL",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "MED_CARD";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    try
                    {
                        AddNewEntitiesSql2(data, "MED_CARD", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Table = "MED_CARD",
                            AffectedRows = data.Count
                        });

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }

            }

            string UpdateMedCardQuery = "UPDATE Med_Card SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE() " +
               " WHERE IsSync=0 OR IsSync IS NULL ";
            var resultmessage = ExecuteNonQueryCommand(UpdateMedCardQuery, _connectionSettings.SQlConnection);
            string UpdateMedCardQuery2 = "UPDATE Med_Card SET IsSync=1,SyncBy='SQLTESTUpdate',SyncDate=GETDATE() " +
               " WHERE  SyncBy = 'Updated' AND IsSync=1  ";
            var resultmessage2 = ExecuteNonQueryCommand(UpdateMedCardQuery2, _connectionSettings.SQlConnection);

        }

        private static void PushMedMedicine()
        {
            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CREATED_DATE DESC) AS RowNum, * FROM Med_Medicine" +
                 " WHERE (IsSync=0 OR IsSync IS NULL ) OR ( SyncBy = 'Updated' AND IsSync=1 ) ) " +
                 " AS m WHERE RowNum > {0} AND RowNum<= {1}";
            //AND CAST(createddate AS date) = CAST('"+dateTime+"' AS date)
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  Med_Medicine " +
                " WHERE (IsSync=0 OR IsSync IS NULL) OR ( SyncBy = 'Updated' AND IsSync=1 )  ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            for (int i = 0; i < maxiteration; i = i)
            {

                List<MED_MEDICINE> data = new List<MED_MEDICINE>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new MED_MEDICINE
                    {
                        CARD_NO = row.IsNull("CARD_NO") ? null : row.Field<string>("CARD_NO"),
                        MED_CODE = row.IsNull("MED_CODE") ? null : row.Field<string>("MED_CODE"),
                        MED_TYP = row.IsNull("MED_TYP") ? int.Parse(null) : row.Field<int>("MED_TYP"),
                        DOSE = row.IsNull("DOSE") ? int.Parse(null) : row.Field<int>("DOSE"),
                        NO_OF_UINT = row.IsNull("NO_OF_UINT") ? int.Parse(null) : row.Field<int>("NO_OF_UINT"),
                        MED_DURATION = row.IsNull("MED_DURATION") ? int.Parse(null) : row.Field<int>("MED_DURATION"),
                        DOS_DUR = row.IsNull("DOS_DUR") ? int.Parse(null) : row.Field<int>("DOS_DUR"),
                        PACK_SIZE = row.IsNull("PACK_SIZE") ? int.Parse(null) : row.Field<int>("PACK_SIZE"),
                        UNIT_NO = row.IsNull("UNIT_NO") ? int.Parse(null) : row.Field<int>("UNIT_NO"),
                        LFT_MONTH = row.IsNull("LFT_MONTH") ? (int?)null : row.Field<int>("LFT_MONTH"),
                        TOTAL_AMT = row.IsNull("TOTAL_AMT") ? double.Parse(null) : row.Field<double>("TOTAL_AMT"),
                        EXCESS = row.IsNull("EXCESS") ? (double?)null : row.Field<double>("EXCESS"),
                        PACK_PRICE = row.IsNull("PACK_PRICE") ? double.Parse(null) : row.Field<double>("PACK_PRICE"),
                        UNIT_PRICE = row.IsNull("UNIT_PRICE") ? double.Parse(null) : row.Field<double>("UNIT_PRICE"),
                        NOTES = row.IsNull("NOTES") ? null : row.Field<string>("NOTES"),
                        CON_MED = row.IsNull("CON_MED") ? null : row.Field<string>("CON_MED"),
                        MED_NAME = row.IsNull("MED_NAME") ? null : row.Field<string>("MED_NAME"),
                        DOSAGE_FORM = row.IsNull("DOSAGE_FORM") ? null : row.Field<string>("DOSAGE_FORM"),
                        ACTIVE = row.IsNull("ACTIVE") ? null : row.Field<string>("ACTIVE"),
                        ACT_MONTH = row.IsNull("ACT_MONTH") ? null : row.Field<string>("ACT_MONTH"),
                        TOT_DUR = row.IsNull("TOT_DUR") ? (int?)null : int.Parse(row.Field<string>("TOT_DUR")),
                        CREATED_BY = row.IsNull("CREATED_BY") ? null : row.Field<string>("CREATED_BY"),
                        RDATE = row.IsNull("RDATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("RDATE"),
                        CREATED_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        UPDATE_BY = row.IsNull("UPDATE_BY") ? null : row.Field<string>("UPDATE_BY"),
                        UPDATE_DATE = row.IsNull("UPDATE_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("UPDATE_DATE"),
                        MONTH_DATE_STOP = row.IsNull("MONTH_DATE_STOP") ? (DateTime?)null : (DateTime)row.Field<DateTime>("MONTH_DATE_STOP"),
                        REG_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        IS_SYNC = 1,
                        SYNC_BY = "SQL",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "MED_MEDICINE";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    try
                    {
                        AddNewEntitiesSql2(data, "MED_MEDICINE", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Table = "MEDICINE2",
                            AffectedRows = data.Count
                        });

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Server = GetServerName(_connectionSettings.OrcaleConnectionTRN_SQL),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }

            }

            string UpdateMedMedicineQuery = "UPDATE Med_Medicine SET IsSync=1,SyncBy='SQLTEST',SyncDate=GETDATE() " +
               " WHERE IsSync=0 OR IsSync IS NULL ";
            var resultmessage = ExecuteNonQueryCommand(UpdateMedMedicineQuery, _connectionSettings.SQlConnection);

            string UpdateMedMedicineQuery2 = "UPDATE Med_Medicine SET IsSync=1,SyncBy='SQLTESTUpdate',SyncDate=GETDATE() " +
               " WHERE  SyncBy = 'Updated' AND IsSync=1  ";
            var resultmessage2 = ExecuteNonQueryCommand(UpdateMedMedicineQuery2, _connectionSettings.SQlConnection);


        }


        private static void UpdateMedCard()
        {
            var query = "SELECT * FROM(select m.*,rownum r from Med_Card m "
                           + " WHERE SYNC_BY = 'Update' AND IS_SYNC=1 )WHERE r > {0} and r<= {1}";
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from Med_Card WHERE  SyncBy = 'Update' AND IsSync=1  ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);
            for (int i = 0; i < maxiteration; i = i)
            {

                List<MED_CARD> data = new List<MED_CARD>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new MED_CARD
                    {
                        CARD_NO = row.IsNull("CARD_NO") ? null : row.Field<string>("CARD_NO"),
                        PROVIDER_CODE = row.IsNull("PROVIDER_CODE") ? int.Parse(null) : row.Field<int>("PROVIDER_CODE"),
                        C_COMP_ID = row.IsNull("C_COMP_ID") ? int.Parse(null) : row.Field<int>("C_COMP_ID"),
                        NOTES = row.IsNull("NOTES") ? null : row.Field<string>("NOTES"),
                        CREATED_BY = row.IsNull("CREATED_BY") ? null : row.Field<string>("CREATED_BY"),
                        CREATED_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        UPDATE_BY = row.IsNull("UPDATE_BY") ? null : row.Field<string>("UPDATE_BY"),
                        UPDATE_DATE = row.IsNull("UPDATE_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("UPDATE_DATE"),
                        MONTH_START_DATE = row.IsNull("MONTH_START_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("MONTH_START_DATE"),
                        MONTH_END_DATE = row.IsNull("MONTH_END_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("MONTH_END_DATE"),
                        GROUP_ID = row.IsNull("GROUP_ID") ? int.Parse(null) : row.Field<int>("GROUP_ID"),
                        GROUP_NAME = row.IsNull("GROUP_NAME") ? null : row.Field<string>("GROUP_NAME"),
                        LOOK_01 = row.IsNull("LOOK_01") ? (int?)null : row.Field<int>("LOOK_01"),
                        SEQ = row.IsNull("SEQ") ? (int?)null : row.Field<int>("SEQ"),
                        PROVIDER_CODE_OLD = row.IsNull("PROVIDER_CODE_OLD") ? (int?)null : row.Field<int>("PROVIDER_CODE_OLD"),
                        TASHKHES_01 = row.IsNull("TASHKHES_01") ? null : row.Field<string>("TASHKHES_01"),
                        NO_PAY = row.IsNull("NO_PAY") ? int.Parse(null) : row.Field<int>("NO_PAY"),
                        NO_OVER = row.IsNull("NO_OVER") ? int.Parse(null) : row.Field<int>("NO_OVER"),
                        ST_DAY = row.IsNull("ST_DAY") ? int.Parse(null) : row.Field<int>("ST_DAY"),
                        REG_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        IS_SYNC = 1,
                        SYNC_BY = "SQL",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "MED_CARD";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    foreach (var item in data)
                    {
                        string queryUpdate = "UPDATE MED_CARD SET CARD_NO ='" + item.CARD_NO +
                            "' ,PROVIDER_CODE= " + item.PROVIDER_CODE + ",C_COMP_ID=" + item.C_COMP_ID +
                            ",NOTES='" + item.NOTES + "',UPDATE_BY='" + item.UPDATE_BY + "',UPDATE_DATE='" +
                            item.UPDATE_DATE + "',MONTH_START_DATE='" + item.MONTH_START_DATE + "',MONTH_END_DATE='" +
                            item.MONTH_END_DATE + "',GROUP_ID=" + item.GROUP_ID + ",GROUP_NAME=N'" + item.GROUP_NAME +
                            "',LOOK_01=" + item.LOOK_01 + ",SEQ=" + item.SEQ + ",PROVIDER_CODE_OLD=" + item.PROVIDER_CODE_OLD +
                            ",TASHKHES_01=N'" + item.TASHKHES_01 + "',NO_PAY=" + item.NO_PAY + ",NO_OVER=" + item.NO_OVER + ",ST_DAY=" +
                            item.ST_DAY + ",SyncDate='" + DateTime.Now + "' WHERE CARD_NO='" + item.CARD_NO + "'";

                        ExecuteOracleQuery(queryUpdate, _connectionSettings.OrcaleConnectionSH65);
                        //ExecuteSQLUpdateQuery(queryUpdate, _connectionSettings.SQlConnection);
                        queryUpdate = "";
                    }
                }
            }
            string SQLQuery = "UPDATE Med_Card SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'SQL'  WHERE IS_SYNC=1 AND SYNC_BY = 'UPDATE' ";
            ExecuteSQLUpdateQuery(SQLQuery, _connectionSettings.SQlConnection);

        }

        private static void UpdateMedMedicine()
        {
            var query = "SELECT * FROM(select m.*,rownum r from Med_Medicine m "
                           + " WHERE SYNC_BY = 'Update' AND IS_SYNC=1 )WHERE r > {0} and r<= {1}";
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from Med_Medicine WHERE  SyncBy = 'Update' AND IsSync=1  ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);
            for (int i = 0; i < maxiteration; i = i)
            {

                List<MED_MEDICINE> data = new List<MED_MEDICINE>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {
                    data = data2.AsEnumerable().Select(row => new MED_MEDICINE
                    {
                        CARD_NO = row.IsNull("CARD_NO") ? null : row.Field<string>("CARD_NO"),
                        MED_CODE = row.IsNull("MED_CODE") ? null : row.Field<string>("MED_CODE"),
                        MED_TYP = row.IsNull("MED_TYP") ? int.Parse(null) : row.Field<int>("MED_TYP"),
                        DOSE = row.IsNull("DOSE") ? int.Parse(null) : row.Field<int>("DOSE"),
                        NO_OF_UINT = row.IsNull("NO_OF_UINT") ? int.Parse(null) : row.Field<int>("NO_OF_UINT"),
                        MED_DURATION = row.IsNull("MED_DURATION") ? int.Parse(null) : row.Field<int>("MED_DURATION"),
                        DOS_DUR = row.IsNull("DOS_DUR") ? int.Parse(null) : row.Field<int>("DOS_DUR"),
                        PACK_SIZE = row.IsNull("PACK_SIZE") ? int.Parse(null) : row.Field<int>("PACK_SIZE"),
                        UNIT_NO = row.IsNull("UNIT_NO") ? int.Parse(null) : row.Field<int>("UNIT_NO"),
                        LFT_MONTH = row.IsNull("LFT_MONTH") ? (int?)null : row.Field<int>("LFT_MONTH"),
                        TOTAL_AMT = row.IsNull("TOTAL_AMT") ? double.Parse(null) : row.Field<double>("TOTAL_AMT"),
                        EXCESS = row.IsNull("EXCESS") ? (double?)null : row.Field<double>("EXCESS"),
                        PACK_PRICE = row.IsNull("PACK_PRICE") ? double.Parse(null) : row.Field<double>("PACK_PRICE"),
                        UNIT_PRICE = row.IsNull("UNIT_PRICE") ? double.Parse(null) : row.Field<double>("UNIT_PRICE"),
                        NOTES = row.IsNull("NOTES") ? null : row.Field<string>("NOTES"),
                        CON_MED = row.IsNull("CON_MED") ? null : row.Field<string>("CON_MED"),
                        MED_NAME = row.IsNull("MED_NAME") ? null : row.Field<string>("MED_NAME"),
                        DOSAGE_FORM = row.IsNull("DOSAGE_FORM") ? null : row.Field<string>("DOSAGE_FORM"),
                        ACTIVE = row.IsNull("ACTIVE") ? null : row.Field<string>("ACTIVE"),
                        ACT_MONTH = row.IsNull("ACT_MONTH") ? null : row.Field<string>("ACT_MONTH"),
                        TOT_DUR = row.IsNull("TOT_DUR") ? (int?)null : int.Parse(row.Field<string>("TOT_DUR")),
                        CREATED_BY = row.IsNull("CREATED_BY") ? null : row.Field<string>("CREATED_BY"),
                        RDATE = row.IsNull("RDATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("RDATE"),
                        CREATED_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        UPDATE_BY = row.IsNull("UPDATE_BY") ? null : row.Field<string>("UPDATE_BY"),
                        UPDATE_DATE = row.IsNull("UPDATE_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("UPDATE_DATE"),
                        MONTH_DATE_STOP = row.IsNull("MONTH_DATE_STOP") ? (DateTime?)null : (DateTime)row.Field<DateTime>("MONTH_DATE_STOP"),
                        REG_DATE = row.IsNull("CREATED_DATE") ? (DateTime?)null : (DateTime)row.Field<DateTime>("CREATED_DATE"),
                        IS_SYNC = 1,
                        SYNC_BY = "SQL",
                        SYNC_DATE = DateTime.Now

                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                string tableName = "MED_MEDICINE";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {
                    foreach (var item in data)
                    {
                        string queryUpdate = "UPDATE MED_MEDICINE SET MED_CODE='" + item.MED_CODE + "', CARD_NO ='" + item.CARD_NO +
                            "' ,RDATE='" + item.RDATE + "',MED_TYP=" + item.MED_TYP +
                            ",DOSE=" + item.DOSE + ",NO_OF_UINT=" + item.NO_OF_UINT + ",TOTAL_AMT=" +
                            item.TOTAL_AMT + ",MED_DURATION=" + item.MED_DURATION + ",TOT_DUR='" +
                            item.TOT_DUR + "',DOS_DUR=" + item.DOS_DUR + ",EXCESS=" + item.EXCESS +
                            ",PACK_SIZE=" + item.PACK_SIZE + ",PACK_PRICE=" + item.PACK_PRICE + ",CON_MED='" + item.CON_MED +
                            "',UNIT_NO=" + item.UNIT_NO + ",UNIT_PRICE=" + item.UNIT_PRICE + ",MED_NAME='" + item.MED_NAME +
                            "',DOSAGE_FORM='" + item.DOSAGE_FORM + "',NOTES='" + item.NOTES + "',ACTIVE='" + item.ACTIVE +
                            "',UPDATE_BY='" + item.UPDATE_BY + "',UPDATE_DATE='" + item.UPDATE_DATE + "',ACT_MONTH='"
                            + item.ACT_MONTH + "',LFT_MONTH=" + item.LFT_MONTH +
                            ",MONTH_DATE_STOP='" + item.MONTH_DATE_STOP + "',SyncDate='" + DateTime.Now + "' ,IsSync=1,SyncBy='Admin' WHERE CARD_NO='"
                            + item.CARD_NO + "' AND MED_CODE='" + item.MED_CODE + "'";

                        ExecuteOracleQuery(queryUpdate, _connectionSettings.OrcaleConnectionSH65);
                        queryUpdate = "";
                    }

                }

            }
            string SQLQuery = "UPDATE Med_Medicine SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'SQL'  WHERE IS_SYNC=1 AND SYNC_BY = 'UPDATE' ";
            ExecuteSQLUpdateQuery(SQLQuery, _connectionSettings.SQlConnection);
        }

        private static void PushRoshita()
        {

            #region Code
            //var sqlQuery = "SELECT * FROM(SELECT ROW_NUMBER() OVER(ORDER BY CREATED_DATE DESC)" +
            //               " AS RowNum, *FROM Comp_Employees) " +
            //               " AS m WHERE RowNum > {0} AND RowNum<= {1}";
            //var query3  = "select * from SH_01.INV_SAL3  WHERE DOSAGE IS NULL";

            //double count2 = 40;
            //var maxiteration2 = Math.Ceiling(count2 / 1000);

            //for (int i = 0; i < maxiteration2; i = i)
            //{
            //    List<INV_SAL> data3 = new List<INV_SAL>();

            //    var data4 = GetOracleDataTable(string.Format(query3, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnectionSH65);

            //    try
            //    {

            //        data3 = data4.AsEnumerable().Select(row => new INV_SAL
            //        {
            //            INVT_NO = row.IsNull("INVT_NO") ? 0 : long.Parse(row.Field<decimal>("INVT_NO").ToString()),
            //            INV_ID = row.IsNull("INV_ID") ? 0 : long.Parse(row.Field<decimal>("INV_ID").ToString()),

            //        }).ToList();

            //    }
            //    catch (Exception ex)
            //    {
            //        var e = ex.Message;
            //    }

            //    foreach (var item in data3)
            //    {
            //        var medicineData = GetOracleDataTable(@"SELECT LIC_TYPE,GRUOP_ID,DOSAGE_FORM,PACK_PRICE,UNIT_PRICE,UNIT_NO,PACK_SIZE,MED_GROUP " +
            //                                    " FROM MEDICINE_DATA WHERE M_CODE='" + item.INVT_NO + "'", _connectionSettings.OrcaleConnectionSH65);
            //        if (medicineData.Rows.Count > 0)
            //        {
            //            var updatequery = "UPDATE INV_SAL3 SET DOSAGE='" + medicineData.Rows[0][2].ToString() + "' ,LIC_TYPE='" + medicineData.Rows[0][0].ToString() +
            //                "' ,MED_GROUP=" + int.Parse(medicineData.Rows[0][1].ToString()) + " , PACK_PRICE=" + double.Parse(medicineData.Rows[0][3].ToString())
            //                + " ,PRICE_UNIT=" + double.Parse(medicineData.Rows[0][4].ToString()) + " ,UNIT=" + double.Parse(medicineData.Rows[0][6].ToString())
            //                + " ,SIZE_UNIT=" + double.Parse(medicineData.Rows[0][6].ToString())+ " WHERE INVT_NO="+item.INVT_NO +
            //                " AND INV_ID="+item.INV_ID;
            //            ExecuteOracleQuery(updatequery, _connectionSettings.OrcaleConnectionSH65);
            //            //iNV_SALOracle.LIC_TYPE = medicineData.Rows[0][0].ToString() != string.Empty ? medicineData.Rows[0][0].ToString() : null;
            //            //iNV_SALOracle.MED_GROUP = medicineData.Rows[0][1].ToString() != string.Empty ? int.Parse(medicineData.Rows[0][1].ToString()) : 0;
            //            //iNV_SALOracle.DOSAGE = medicineData.Rows[0][2].ToString() != string.Empty ? medicineData.Rows[0][2].ToString() : null;
            //            //iNV_SALOracle.PACK_PRICE = medicineData.Rows[0][3].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][3].ToString()) : 0;
            //            //iNV_SALOracle.PRICE_UNIT = medicineData.Rows[0][4].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][4].ToString()) : 0;
            //            //iNV_SALOracle.UNIT = medicineData.Rows[0][5].ToString() != string.Empty ? int.Parse(medicineData.Rows[0][5].ToString()) : 0;
            //            //iNV_SALOracle.SIZE_UNIT = medicineData.Rows[0][6].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][6].ToString()) : 0;

            //            //if (manager != null && manager != "Doctor_Chronic")
            //            //{
            //            //    var MedGroup = GetOracleDataTable("select GRUOP_TYPE from MEDICINE_GRUOP where GRUOP_ID=" + long.Parse(medicineData.Rows[0][7].ToString()), _connectionSettings.OrcaleConnectionSH65);
            //            //    iNV_SALOracle.GRUOP_TYPE = MedGroup.Rows[0][0].ToString() != string.Empty ? MedGroup.Rows[0][0].ToString() : null;
            //            //    iNV_SALOracle.MED_GROUP = medicineData.Rows[0][7].ToString() != string.Empty ? long.Parse(medicineData.Rows[0][7].ToString()) : 0;
            //            //}
            //            //else
            //            //{
            //            //    iNV_SALOracle.GRUOP_TYPE = null;
            //            //    iNV_SALOracle.MED_GROUP = null;
            //            //}

            //        }
            //    }
            //}
            #endregion

            #region Update Employee

            //var quer = " SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CARD_ID DESC) " +
            //             " AS RowNum, * FROM CardsArabic)AS m WHERE RowNum > {0} AND RowNum<= {1}";

            //for (int i = 0; i < 8; i = i)
            //{
            //    List<string> car = new List<string>();
            //    var data2 = GetSqlDataTable(string.Format(quer, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
            //    car = data2.AsEnumerable().Select(row =>
            //    row.IsNull("CARD_ID") ? "Now" : row.Field<string>("CARD_ID")).ToList();

            //    if(car.Count>0)
            //    {
            //        var Selecquery = " SELECT DISTINCT CARD_ID, EMP_ANAME,EMP_ENAME,EMP_ANAME_ST,EMP_ANAME_SC," +
            //            "EMP_ANAME_TH,EMP_ANAME_FR,OLD_CLASS_CODE FROM COMP_EMPLOYEES WHERE CARD_ID IN('" + car[0];
            //        for (int j =1; j < car.Count; j++)
            //        {
            //            Selecquery = Selecquery + "','" + car[j];
            //        }
            //        Selecquery = Selecquery + "')" ;
            //        var data3 = GetOracleDataTable(Selecquery, _connectionSettings.OrcaleConnection);
            //        var da= data3.AsEnumerable().Select(row => new
            //        {
            //            CARD_ID=row.IsNull("CARD_ID") ? null : row.Field<string>("CARD_ID"),
            //            EMP_ANAME =row.IsNull("EMP_ANAME") ? null : row.Field<string>("EMP_ANAME"),
            //            EMP_ENAME=row.IsNull("EMP_ENAME") ? null : row.Field<string>("EMP_ENAME"),
            //            EMP_ANAME_ST=row.IsNull("EMP_ANAME_ST") ? null : row.Field<string>("EMP_ANAME_ST"),
            //            EMP_ANAME_SC=row.IsNull("EMP_ANAME_SC") ? null : row.Field<string>("EMP_ANAME_SC"),
            //            EMP_ANAME_TH=row.IsNull("EMP_ANAME_TH") ? null : row.Field<string>("EMP_ANAME_TH"),
            //            EMP_ANAME_FR=row.IsNull("EMP_ANAME_FR") ? null : row.Field<string>("EMP_ANAME_FR"),
            //            OLD_CLASS_CODE=row.IsNull("OLD_CLASS_CODE") ? null : row.Field<string>("OLD_CLASS_CODE"),

            //        }).ToList();
            //        if(da.Count>0)
            //        {
            //            foreach (var item in da)
            //            {
            //                var updaquery = "UPDATE Comp_Employees SET EMP_ANAME=N'" + item.EMP_ANAME + "',"
            //                    + "EMP_ENAME=N'" + item.EMP_ENAME + "',EMP_ANAME_ST=N'" + item.EMP_ANAME_ST + "'," +
            //                    "EMP_ANAME_SC=N'" + item.EMP_ANAME_SC + "',EMP_ANAME_TH=N'" + item.EMP_ANAME_TH + "'," +
            //                    "EMP_ANAME_FR=N'" + item.EMP_ANAME_FR + "',OLD_CLASS_CODE=N'" + item.OLD_CLASS_CODE + "'" +
            //                    " WHERE CARD_ID='" + item.CARD_ID+"'";
            //                int resault = ExecuteSQLUpdateQuery2(updaquery, _connectionSettings.SQlConnection);
            //            }
            //        }
            //    }
            //}


            #endregion

            //var query1 = "select * from (select m.*, rownum r from Roshita WHERE IsSync=0 OR IsSync IS NULL) WHERE r > {0} and r<= {1} ";

            DateTime da = DateTime.Parse(GetSqlDataTable("SELECT CAST(GETDATE() AS date)", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            int hour = int.Parse(GetSqlDataTable("SELECT DATEPART(HOUR,GETDATE())", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            DateTime dateTime = new DateTime(da.Year, da.Month, da.Day, hour, 00, 00);


            var QueryUpdate = "update roshita SET oracle_id=CONCAT(2,REPLACE(convert(varchar, CreatedDate,3),'/',''), Roshita.Id)" +
                         "WHERE(isSync IS NULL OR issync = 0)  AND CreatedDate<='" + dateTime + "' AND Manager !='Doctor_Chronic'";
            ExecuteSQLUpdateQuery(QueryUpdate, _connectionSettings.SQlConnection);

            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CreatedDate DESC) AS RowNum, * FROM Roshita" +
                  " WHERE (IsSync=0 OR IsSync IS NULL ) " +
                  " AND CreatedDate <= '" + dateTime + "' AND Manager !='Doctor_Chronic' )" +
                  "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            //AND CAST(createddate AS date) = CAST('"+dateTime+"' AS date)
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  Roshita " +
                " WHERE (IsSync=0 OR IsSync IS NULL)  AND Manager !='Doctor_Chronic' " +
                " AND CreatedDate <='" + dateTime + "'", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);
            //long sequenc = 1689;
            //long.Parse(GetSqlDataTable(@"SELECT ISNULL(MAX(ID),0) FROM SequenceNumber", _connectionSettings.SQlConnection).Rows[0][0].ToString()) + 1;
            long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(D_SEQ),0)   FROM DMS_02_EMP_D_ENT ORDER BY D_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
            if (DateTime.Now.Day == 2)
            {
                sequenc = 1;
            }
            for (int i = 0; i < maxiteration; i = i)
            {

                List<Roshita> data = new List<Roshita>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {

                    data = data2.AsEnumerable().Select(row => new Roshita
                    {
                        Id = row.Field<long>("Id"),
                        CardId = row.IsNull("CardId") ? "Now" : row.Field<string>("CardId"),
                        Speciality = row.IsNull("Speciality") ? "Now" : row.Field<string>("Speciality"),
                        Diagnose1 = row.IsNull("Diagnose1") ? "Now" : row.Field<string>("Diagnose1"),
                        Diagnose2 = row.IsNull("Diagnose2") ? "Now" : row.Field<string>("Diagnose2"),
                        diagnose3 = row.IsNull("diagnose3") ? "Now" : row.Field<string>("diagnose3"),
                        RoshetaType = row.IsNull("RoshetaType") ? "Now" : row.Field<string>("RoshetaType"),
                        Limit = row.IsNull("Limit") ? 0 : row.Field<int>("Limit"),
                        CompanyPercent = row.IsNull("CompanyPercent") ? 0 : Convert.ToDouble(row.Field<int>("CompanyPercent")),
                        OverInsurance = row.IsNull("OverInsurance") ? 0 : row.Field<double>("OverInsurance"),
                        TotalValue = row.IsNull("TotalValue") ? 0 : row.Field<double>("TotalValue"),
                        CompanyPayment = row.IsNull("CompanyPayment") ? 0 : row.Field<double>("CompanyPayment"),
                        PersonPayment = row.IsNull("PersonPayment") ? 0 : row.Field<double>("PersonPayment"),
                        Cash = row.IsNull("Cash") ? 0 : row.Field<double>("Cash"),
                        Manager = row.IsNull("Manager") ? "Now" : row.Field<string>("Manager"),
                        CreatedBy = row.IsNull("CreatedBy") ? "Now" : row.Field<string>("CreatedBy"),
                        CreatedDate = row.IsNull("CreatedDate") ? DateTime.Now : (DateTime)row.Field<DateTime>("CreatedDate"),
                        PatchId = row.IsNull("PatchId") ? 0 : row.Field<int>("PatchId"),
                        PhoneNumber = row.IsNull("PhoneNumber") ? "Now" : row.Field<string>("PhoneNumber"),
                        Oracle_Id = row.IsNull("Oracle_Id") ? 0 : row.Field<long>("Oracle_Id"),
                        ClaimNumber = row.IsNull("ClaimNumber") ? 0 : row.Field<double>("ClaimNumber"),
                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }

                //string tableName = "DMS_02_EMP_D_ENT";
                string tableName = "Roshita";
                _result.Logs.Add(new ViewModels.Log
                {
                    Order = GetLogOrder(),
                    Action = SyncAction.Get.ToString(),
                    Database = GetDatabaseName(),
                    Server = GetServerName(),
                    Note = "Not Synced",
                    Table = tableName,
                    AffectedRows = data.Count
                });
                if (data.Count != 0)
                {

                    List<DMS_02_EMP_D_ENT3> roshitas = new List<DMS_02_EMP_D_ENT3>();
                    DMS_02_EMP_D_ENT3 rsh = new DMS_02_EMP_D_ENT3();
                    foreach (var item in data)
                    {
                        try
                        {
                            //string d_id = "2" + string.Format("{0:dd MM yyyy}", item.CreatedDate) + sequenc.ToString();
                            rsh = FillOracleRoshitaObject(item);
                            var empName = GetOracleDataTable(@"select* from comp_employees where card_id='" + item.CardId.ToUpper() + "' order by contract_no desc", _connectionSettings.OrcaleConnection);
                            //string result = string.Concat(d_id.Where(c => !char.IsWhiteSpace(c)));
                            rsh.D_SEQ = sequenc;
                            //rsh.D_ID = long.Parse(result);
                            rsh.INVOICE = sequenc.ToString();
                            rsh.C_COMP_ID = empName.Rows[0][3].ToString() != string.Empty ? Convert.ToInt32(empName.Rows[0][3].ToString()) : 0;
                            rsh.CONTRACT_NO = empName.Rows[0][4].ToString() != string.Empty ? Convert.ToInt32(empName.Rows[0][4].ToString()) : 0;
                            rsh.CLASS_CODE = empName.Rows[0][5].ToString() != string.Empty ? empName.Rows[0][5].ToString() : "";
                            DateTime insStartdate = DateTime.Parse(empName.Rows[0][25].ToString());
                            DateTime insEnddate = DateTime.Parse(empName.Rows[0][26].ToString());
                            rsh.INS_START_DATE = insStartdate.Date;
                            rsh.INS_END_DATE = insEnddate.Date;
                            rsh.D_EXP = empName.Rows[0][44].ToString() != string.Empty ? empName.Rows[0][44].ToString() : "";
                            rsh.D_EXP_2 = empName.Rows[0][45].ToString() != string.Empty ? empName.Rows[0][45].ToString() : "";
                            rsh.D_EXP_3 = empName.Rows[0][46].ToString() != string.Empty ? empName.Rows[0][46].ToString() : "";
                            rsh.D_EXP_4 = empName.Rows[0][47].ToString() != string.Empty ? empName.Rows[0][47].ToString() : "";
                            rsh.CUST_E_NAME = empName.Rows[0][49].ToString() != string.Empty ? empName.Rows[0][49].ToString() : "";
                            rsh.CUST_E_NAME_2 = empName.Rows[0][50].ToString() != string.Empty ? empName.Rows[0][50].ToString() : "";
                            rsh.CUST_E_NAME_3 = empName.Rows[0][51].ToString() != string.Empty ? empName.Rows[0][51].ToString() : "";
                            rsh.CUST_E_NAME_4 = String.Format("{0:dd/MM/yy hh:mm}", item.CreatedDate); /*item.CreatedDate.ToString();*/

                            string RoshitaAcception = "SELECT * FROM RoshitaAcceptions " +
                                "  WHERE RoshitaId=" + item.Id + ";";
                            var RoshitaAcc = GetSqlDataTable(RoshitaAcception, _connectionSettings.SQlConnection);

                            if (RoshitaAcc.Rows.Count > 0)
                            {
                                rsh.MASS_NO = int.Parse(RoshitaAcc.Rows[0][1].ToString());

                            }

                            string QueryLocal = "SELECT SUM(RoshitaDetails.Amount) FROM RoshitaDetails " +
                                "INNER JOIN  MedicineData  ON MedicineData.M_CODE=RoshitaDetails.MedicienCode " +
                                " AND MedicineData.LIC_TYPE='LOCAL' WHERE RoshitaID=" + item.Id + " AND IsDealed=1 AND " +
                                " PaymentGroup!='Pending' AND  PaymentGroup!='Rejected';";
                            string QueryUniversal = "SELECT SUM(RoshitaDetails.Amount) FROM RoshitaDetails " +
                                "INNER JOIN  MedicineData  ON MedicineData.M_CODE=RoshitaDetails.MedicienCode " +
                                " AND MedicineData.LIC_TYPE='IMPORT' WHERE RoshitaID=" + item.Id + " AND IsDealed=1 AND " +
                                " PaymentGroup!='Pending' AND  PaymentGroup!='Rejected' ;";
                            var TotalLocal = GetSqlDataTable(QueryLocal, _connectionSettings.SQlConnection);

                            var TotalImport = GetSqlDataTable(QueryUniversal, _connectionSettings.SQlConnection);

                            if (!TotalLocal.Rows[0][0].ToString().IsNullOrWhiteSpace())
                            {
                                rsh.LOC_AMOUNT = double.Parse(TotalLocal.Rows[0][0].ToString());
                            }
                            if (!TotalImport.Rows[0][0].ToString().IsNullOrWhiteSpace())
                            {
                                rsh.IMP_AMOUNT = double.Parse(TotalImport.Rows[0][0].ToString());
                            }
                            try
                            {
                                rsh.MGR_ID = int.Parse(GetSqlDataTable("SELECT COUNT(*) FROM RoshitaDetails WHERE RoshitaID=" + item.Id
                                    + " AND IsDealed = 1 AND  PaymentGroup!='Pending' AND  PaymentGroup!='Rejected' ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
                            }
                            catch (Exception ex)
                            {
                                var itemexception = item;
                            }
                            rsh.C_COMP_NAME = GetOracleDataTable(@"SELECT C_ANAME FROM CONTRACT_COMP WHERE C_COMP_ID='" + rsh.C_COMP_ID + "'", _connectionSettings.OrcaleConnection).Rows[0][0].ToString();

                            //var insurance = GetSqlDataTable(@"select INSURANCE_DAY,INSURANCE_MONTH  from CO_INSURANCE_01 where CO_ID=" + rsh.C_COMP_ID + " AND LIVEL='" + rsh.CLASS_CODE + "'", _connectionSettings.SQlConnection);
                            var insuranceMedEmp = GetSqlDataTable(@"select DAY_AMT,MON_AMT  from COMP_CUSTOMIZED_D_D_MED_EMP where CARD_ID='"
                                + rsh.CARD_ID + "' AND CLASS_CODE='" + rsh.CLASS_CODE + "' AND CONTRACT_NO=" + rsh.CONTRACT_NO
                                + " AND SER_SERV='" + item.RoshetaType + "' C_COMP_ID=" + rsh.C_COMP_ID, _connectionSettings.SQlConnection);
                            if (insuranceMedEmp.Rows.Count > 0)
                            {
                                rsh.INSU_LIMT = insuranceMedEmp.Rows[0][0].ToString() != string.Empty ? int.Parse(insuranceMedEmp.Rows[0][0].ToString()) : 0;

                                rsh.INSU_LIMT_M = insuranceMedEmp.Rows[0][1].ToString() != string.Empty ? int.Parse(insuranceMedEmp.Rows[0][1].ToString()) : 0;
                                if (rsh.INSU_LIMT_M == 0)
                                {
                                    rsh.NON_COVERED = rsh.OVER_INSURANCE;
                                }
                                else
                                {
                                    rsh.NON_COVERED = 0;
                                }
                            }
                            else
                            {
                                var insuranceMed = GetSqlDataTable(@"select DAY_AMT,MON_AMT  from COMP_CUSTOMIZED_D_D_MED where C_COMP_ID='"
                                + rsh.C_COMP_ID + "' AND CLASS_CODE='" + rsh.CLASS_CODE + "' AND CONTRACT_NO=" + rsh.CONTRACT_NO
                                + " AND SER_SERV='" + item.RoshetaType + "' ", _connectionSettings.SQlConnection);
                                if (insuranceMed.Rows.Count > 0)
                                {
                                    rsh.INSU_LIMT = insuranceMed.Rows[0][0].ToString() != string.Empty ? int.Parse(insuranceMed.Rows[0][0].ToString()) : 0;

                                    rsh.INSU_LIMT_M = insuranceMed.Rows[0][1].ToString() != string.Empty ? int.Parse(insuranceMed.Rows[0][1].ToString()) : 0;
                                    if (rsh.INSU_LIMT_M == 0)
                                    {
                                        rsh.NON_COVERED = rsh.OVER_INSURANCE;
                                    }
                                    else
                                    {
                                        rsh.NON_COVERED = 0;
                                    }
                                }
                            }

                            //rsh.INSU_LIMT = insurance.Rows[0][0].ToString() != string.Empty ? int.Parse(insurance.Rows[0][0].ToString()) : 0;
                            //if (insurance.Rows.Count > 0)
                            //{
                            //    rsh.INSU_LIMT_M = insurance.Rows[0][1].ToString() != string.Empty ? int.Parse(insurance.Rows[0][1].ToString()) : 0;
                            //    if (rsh.INSU_LIMT_M == 0)
                            //    {
                            //        rsh.NON_COVERED = rsh.OVER_INSURANCE;
                            //    }
                            //    else
                            //    {
                            //        rsh.NON_COVERED = 0;
                            //    }
                            //}
                            string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                " WHERE UserName='" + item.CreatedBy + "';";
                            var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                            if (UserId.Rows.Count > 0)
                            {
                                var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                               " WHERE PR_CODE=" + providerId + ";";
                                var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                if (UName.Rows.Count > 0)
                                {
                                    rsh.EMP_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    rsh.EMP_ID_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    rsh.EMP_SUB = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                }
                                else
                                {
                                    string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                          " WHERE UserCode='" + providerId + "';";
                                    var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                    if (MapName.Rows.Count > 0)
                                    {
                                        rsh.EMP_ID = providerId;
                                        rsh.EMP_ID_ID = providerId;
                                        rsh.EMP_SUB = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : "";
                                    }
                                }
                            }

                            //var user = GetOracleDataTable(@"select USER_ID ,USER_CO,USER_N from USERS_2 where USER_NAME='" + item.CreatedBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                            //if (user.Rows.Count > 0)
                            //{
                            //    rsh.EMP_ID = user.Rows[0][0].ToString() != string.Empty ? long.Parse(user.Rows[0][0].ToString()) : 0;
                            //    rsh.EMP_ID_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                            //    rsh.EMP_SUB = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : "";
                            //}
                            var SER_PROV_DISC = GetOracleDataTable(@"select DEV_LOC_DIS,DEV_IMP_DIS  from SER_PROV_DISC where PROV_ID=" + rsh.EMP_ID_ID, _connectionSettings.OrcaleConnectionSH65);
                            if (SER_PROV_DISC.Rows.Count > 0)
                            {
                                rsh.DEV_LOC = SER_PROV_DISC.Rows[0][0].ToString() != string.Empty ? double.Parse(SER_PROV_DISC.Rows[0][0].ToString()) : 0;
                                rsh.DEV_IMP = SER_PROV_DISC.Rows[0][1].ToString() != string.Empty ? double.Parse(SER_PROV_DISC.Rows[0][1].ToString()) : 0;
                            }

                            rsh.EMP_NAME = item.CreatedBy;
                            rsh.IS_SYNC = 2;
                            rsh.SYNC_BY = "SQL";
                            rsh.SYNC_DATE = DateTime.Now;
                            item.Oracle_Id = rsh.D_ID;
                            //string updatesqlTable = "UPDATE Roshita set Oracle_Id=" + item.Oracle_Id + " WHERE Id=" + item.Id;

                            // ,IsSync = 1,SyncDate = '" + DateTime.Now + " , SyncBy='SQL'

                            //var resultmessage = ExecuteNonQueryCommand(updatesqlTable, _connectionSettings.SQlConnection);
                            // Lab_Stop,Ray_Stop,Lab,Ray
                            switch (item.Manager)
                            {
                                case "Daily":
                                    rsh.MANAGER = "YES";
                                    string QueryAcceptions = "SELECT c.AcceptionReasonsId FROM CardAcceptionReasons c " +
                                                 " JOIN RoshitaAcceptions r ON  c.AcceptionId=r.AcceptionId " +
                                               " AND r.RoshitaId=" + item.Id;
                                    var Acceptions = GetSqlDataTable(QueryAcceptions, _connectionSettings.SQlConnection);
                                    if (Acceptions.Rows.Count > 0)
                                    {
                                        for (int a = 0; a < Acceptions.Rows.Count; a++)
                                        {
                                            //var re = Acceptions.Rows[a][0].ToString();
                                            int val = Acceptions.Rows[a][0].ToString() != string.Empty ? int.Parse(Acceptions.Rows[a][0].ToString()) : 0;
                                            switch (val)
                                            {
                                                case 1:
                                                    rsh.TN1 = 1;
                                                    break;
                                                case 2:
                                                    rsh.TN2 = 1;
                                                    break;
                                                case 7:
                                                    rsh.TN3 = 1;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case "Pharmacy_Chronic":
                                    rsh.MANAGER = "MON";
                                    string QueryMedCard = "SELECT NO_PAY,NO_OVER FROM Med_Card " +
                                               " WHERE CARD_NO='" + item.CardId + "';";
                                    var medcard = GetSqlDataTable(QueryMedCard, _connectionSettings.SQlConnection);
                                    if (medcard.Rows.Count > 0)
                                    {
                                        rsh.TN1 = medcard.Rows[0][0].ToString() != string.Empty ? int.Parse(medcard.Rows[0][0].ToString()) : 0;
                                        rsh.TN2 = medcard.Rows[0][1].ToString() != string.Empty ? int.Parse(medcard.Rows[0][1].ToString()) : 0;
                                    }
                                    if (rsh.TN1 == 0 || rsh.TN2 == 0)
                                    {
                                        string QueryAcceptions3 = "SELECT c.AcceptionReasonsId FROM CardAcceptionReasons c " +
                                                 " JOIN RoshitaAcceptions r ON  c.AcceptionId=r.AcceptionId " +
                                               " AND r.RoshitaId=" + item.Id;
                                        var Acceptions3 = GetSqlDataTable(QueryAcceptions3, _connectionSettings.SQlConnection);
                                        if (Acceptions3.Rows.Count > 0)
                                        {
                                            for (int a = 0; a < Acceptions3.Rows.Count; a++)
                                            {
                                                int val = Acceptions3.Rows[a][0].ToString() != string.Empty ? int.Parse(Acceptions3.Rows[a][0].ToString()) : 0;
                                                switch (val)
                                                {
                                                    case 1:
                                                        rsh.TN1 = 1;
                                                        break;
                                                    case 2:
                                                        rsh.TN2 = 1;
                                                        break;
                                                    case 7:
                                                        rsh.TN3 = 1;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                case "Lab_Stop":
                                    rsh.MANAGER = "STOP_L";
                                    break;
                                case "Monthly":
                                    rsh.MANAGER = "MON_PH";

                                    string QueryAcceptions2 = "SELECT c.AcceptionReasonsId FROM CardAcceptionReasons c " +
                                                 " JOIN RoshitaAcceptions r ON  c.AcceptionId=r.AcceptionId " +
                                               " AND r.RoshitaId=" + item.Id;
                                    var Acceptions2 = GetSqlDataTable(QueryAcceptions2, _connectionSettings.SQlConnection);
                                    if (Acceptions2.Rows.Count > 0)
                                    {
                                        for (int a = 0; a < Acceptions2.Rows.Count; a++)
                                        {
                                            int val = Acceptions2.Rows[a][0].ToString() != string.Empty ? int.Parse(Acceptions2.Rows[a][0].ToString()) : 0;
                                            switch (val)
                                            {
                                                case 1:
                                                    rsh.TN1 = 1;
                                                    break;
                                                case 2:
                                                    rsh.TN2 = 1;
                                                    break;
                                                case 7:
                                                    rsh.TN3 = 1;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case "Daily_Stop":
                                    rsh.MANAGER = "STOP_PH";
                                    break;
                                case "Ray_Stop":
                                    rsh.MANAGER = "STOP_R";
                                    break;
                                case "Monthly_Stop ":
                                    rsh.MANAGER = "STOP_PH_MON";
                                    break;
                                case "Lab":
                                    rsh.MANAGER = "LAB";
                                    break;
                                case "Ray":
                                    rsh.MANAGER = "RAY";
                                    break;
                                case "Doctor_Daily":
                                    rsh.MANAGER = "DOC";
                                    break;
                                case "Doctor_Chronic":
                                    rsh.MANAGER = "DOC_MON";
                                    break;
                                case "Doctor_Daily_Stop":
                                    rsh.MANAGER = "STOP_DOC";
                                    break;
                                case "Pharmacy_Doctor":
                                    rsh.MANAGER = "YES";
                                    break;
                                case "Stop-ED":
                                    rsh.MANAGER = "STOP_ED";
                                    break;
                                default:
                                    rsh.MANAGER = item.Manager;
                                    break;
                            }

                            string QueryTASHKHES = "SELECT DiagnoiseName FROM PrescriptionRoshitaDignosis " +
                                " WHERE RositaId=" + item.Id + ";";
                            var TASHKHES = GetSqlDataTable(QueryTASHKHES, _connectionSettings.SQlConnection);
                            if (TASHKHES.Rows.Count > 0)
                            {
                                switch (TASHKHES.Rows.Count)
                                {
                                    case 0:
                                        rsh.TASHKHES_01 = null;
                                        rsh.TASHKHES_02 = null;
                                        rsh.TASHKHES_03 = null;
                                        break;
                                    case 1:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        break;
                                    case 2:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        break;
                                    case 3:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        rsh.TASHKHES_03 = TASHKHES.Rows[2][0].ToString();
                                        break;
                                    default:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        rsh.TASHKHES_03 = TASHKHES.Rows[2][0].ToString();
                                        break;
                                }
                            }

                            roshitas.Add(rsh);
                            sequenc++;
                        }
                        catch (Exception ex)
                        {
                            int w = roshitas.Count();
                            var itemexception = item;
                            string sex = ex.Message;
                        }

                    }


                    try
                    {
                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(roshitas, "DMS_02_EMP_D_ENT", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = tableName,
                            AffectedRows = roshitas.Count
                        });

                        string SQLQuery = "UPDATE  Roshita SET IsSync=1,SyncBy='TEST',SyncDate=GETDATE() " +
                            " WHERE(IsSync = 0 OR IsSync IS NULL)  AND Oracle_Id IN( '" + roshitas[0].D_ID + "','";

                        for (int j = 1; j < roshitas.Count - 1; j++)
                        {
                            SQLQuery += roshitas[j].D_ID + "','";
                        }
                        SQLQuery += roshitas[roshitas.Count - 1].D_ID + "')";
                        var resultmessage2 = ExecuteNonQueryCommand(SQLQuery, _connectionSettings.SQlConnection);

                        //string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='TEST',SyncDate=GETDATE() " +
                        //         " WHERE (IsSync=0 OR IsSync IS NULL) " +
                        //         "  AND CreatedDate<='" + dateTime + "' AND Oracle_Id IN( '" + roshitas[0].D_ID + "','";
                        //for (int j = 1; j < roshitas.Count - 1; j++)
                        //{
                        //    UpdateRoshitaQuery += roshitas[j].D_ID + "','";
                        //}
                        //UpdateRoshitaQuery += roshitas[roshitas.Count - 1].D_ID + "')";

                        //var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);


                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }

            }

            //string deleteSequence = "DELETE SequenceNumber";
            //ExecuteSQLUpdateQuery(deleteSequence, _connectionSettings.SQlConnection);
            //string UpdateQuery = "INSERT INTO SequenceNumber VALUES(" + sequenc + ")";
            //ExecuteSQLUpdateQuery(UpdateQuery, _connectionSettings.SQlConnection);
        }

        private static DMS_02_EMP_D_ENT3 FillOracleRoshitaObject(Roshita obj)
        {
            string name = "Empty";
            int compId = int.Parse(obj.CardId.Split('-')[0]);
            if (obj.CreatedBy != null)
            {
                name = obj.CreatedBy;
            }
            try
            {
                var x = string.Format("{0:dd MMM yy}", obj.CreatedDate);
                DateTime da = obj.CreatedDate.Value;
                return new DMS_02_EMP_D_ENT3
                {
                    D_ID = obj.Oracle_Id,
                    D_DATE = da.Date,
                    D_VD = obj.TotalValue,
                    DD_NUM = 1,
                    DC_NUM = 4700001,
                    PERCENT_MONY = 100 - (int)obj.CompanyPercent,
                    CARD_ID = obj.CardId,
                    CUST_E_NAME_4 = obj.CreatedDate.ToString(),
                    D_TAX = 0,
                    MANAGER = obj.Manager,
                    DATE_INVOICE = da.Date,
                    //EMP_ID_ID =,
                    //EMP_SUB =,
                    //EMP_ID =,
                    //EMP_NAME = name,
                    //MGR_ID =,
                    C_COMP_ID = compId,
                    REGUL_1 = "",
                    //CLASS_CODE =,
                    //INS_START_DATE =,
                    //INS_END_DATE =,
                    //TAFK_01 =,
                    //C_COMP_NAME =,
                    INSU_LIMT = obj.Limit,
                    //INSU_LIMT_M =,
                    CARRY = obj.PersonPayment,
                    NON_COVERED = 0,
                    VALUE_CREDIT = obj.CompanyPayment,
                    OVER_INSURANCE = obj.OverInsurance,
                    VALUE_CASH = obj.Cash,
                    DATE_ROSHTA = da.Date,
                    TAKHASOS = obj.Speciality == "Empty" ? null : obj.Speciality,
                    OVER_KIND = 0,
                    D_TIME = string.Format("{0:hh mm tt}", obj.CreatedDate),
                    OVER_KIND_01 = 0,
                    DALY_SEQ = 0,
                    REGS_NO = 0,
                    DOC_ID = 0,
                    TYPE_DOC = 0,
                    TYPE_VISIT = 0,
                    APPROV_CLAIM = 0,
                    APPROV_PAY = 0,
                    BATSH_AMT = 0,
                    BATSH_NO = 0,
                    REVIEW_ID = 0,
                    REVIEW_NAME = "0",
                    KIND_KIND = 0,
                    HOLD_CLAM = 0,
                    RECIT = "0",
                    BATSH_REP = 0,
                    MASS_NO = 0,
                    GROUP_ID = 0,
                    MONY_ACT = 0,
                    PER_LOC = 0,
                    PER_IMP = 0,
                    LOC_AMOUNT = 0,
                    IMP_AMOUNT = 0,
                    KIND_REP = 0,
                    TN1 = 0,
                    TN2 = 0,
                    TN3 = 0,
                    DEV_LOC = 0,
                    DEV_IMP = 0,
                    APRROV_COUN = 0,
                    P_1 = 0,
                    P_2 = 0,
                    P_3 = 0,
                    P_4 = 0,
                    P_5 = 0,
                    P_6 = 0,
                    P_7 = 0,
                    P_8 = 0,
                    P_9 = 0,
                    P_10 = 0,
                    P_11 = 0,
                    P_12 = 0,
                    P_13 = 0,
                    P_14 = 0,
                    P_15 = 0,
                    P_16 = 0,
                    P_17 = 0,
                    P_18 = 0,
                    P_19 = 0,
                    SER_NO = 0,
                    CONTRACT_NO = 0,
                    MANUAL_NO = 0,
                    FREE_PERCENT = 0,
                    FREE_OVER = 0,
                    CLAM_FIN_LOC_CO = 0,
                    LOC_DISC = 0,
                    CLAM_FIN_IMP_CO = 0,
                    IMP_DISC = 0,
                    OVR = 0,
                    MONY_RECIT = 0,
                    MOB_NO = obj.PhoneNumber == "Empty" ? null : obj.PhoneNumber,
                    REG_DATE = da.Date,
                    NATIONAL_ID = obj.Diagnose2 == null ? null : obj.Diagnose2,
                    NO_EX = obj.ClaimNumber == null ? "0" : obj.ClaimNumber.ToString()
                };
            }
            catch (Exception ex)
            {
                return new DMS_02_EMP_D_ENT3();
            }

        }

        private static void PushRoshitaDetails()
        {
            try
            {
                //DateTime dat = DateTime.Parse(GetSqlDataTable("SELECT CAST(GETDATE() AS date)", _connectionSettings.SQlConnection).Rows[0][0].ToString());
                //int hour = int.Parse(GetSqlDataTable("SELECT DATEPART(HOUR,GETDATE())", _connectionSettings.SQlConnection).Rows[0][0].ToString());
                //DateTime dateTime = new DateTime(dat.Year, dat.Month, dat.Day, hour, 00, 00);


                //var query1 = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY (select null)) AS RowNum, " +
                //     "* FROM RoshitaDetails WHERE roshitaid in (SELECT id FROM roshita  WHERE (IsSync=0 OR IsSync IS NULL) " +
                //     " AND CreatedDate<='" + dateTime + "'))" +
                //     "AS m WHERE RowNum > {0} AND RowNum<= {1}";


                var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY (select null)) AS RowNum, " +
                    "* FROM RoshitaDetails WHERE IsDealed=1 AND roshitaid in (SELECT id FROM roshita  WHERE IsSync=1 AND SyncBy='TEST' " +
                    " AND Manager NOT IN('Lab_Stop','Ray_Stop','Lab','Ray','Doctor_Chronic') ))" +
                    "AS m WHERE RowNum > {0} AND RowNum<= {1}";




                #region RoshitaDetails

                //double count = double.Parse(GetSqlDataTable("SELECT Count(*) FROM Roshitadetails WHERE roshitaid in (SELECT id FROM Roshita  WHERE (IsSync=0 OR IsSync IS NULL) " +
                //    " AND  CreatedDate<='" + dateTime + "')", _connectionSettings.SQlConnection).Rows[0][0].ToString());
                double count = double.Parse(GetSqlDataTable("SELECT Count(*) FROM Roshitadetails WHERE roshitaid in (SELECT id FROM Roshita  WHERE IsSync=1 AND SyncBy='TEST'" +
                    " AND Manager NOT IN('Lab_Stop','Ray_Stop','Lab','Ray','Doctor_Chronic') )", _connectionSettings.SQlConnection).Rows[0][0].ToString());

                var maxiteration = Math.Ceiling(count / 100000);
                long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(INVT_SEQ),0)   FROM INV_SAL ORDER BY INV_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
                if (DateTime.Now.Day == 2)
                {
                    sequenc = 1;
                }
                for (int i = 0; i < maxiteration; i = i)
                {
                    List<INV_SALOracle> data = new List<INV_SALOracle>();

                    INV_SALOracle iNV_SALOracle = new INV_SALOracle();

                    //data.AddRange(Inv_Sal_Medicine(i, query));
                    //data.AddRange(Inv_Sal_Lab(i, queryLab));
                    //data.AddRange(Inv_Sal_Ray(i, queryRay));


                    List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();
                    var data2 = GetSqlDataTable(string.Format(query, (i * 100000), ((++i) * 100000)), _connectionSettings.SQlConnection);

                    //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                    try
                    {

                        roshitaDetail = data2.AsEnumerable().Select(row => new RoshitaDetail
                        {
                            Id = row.Field<long>("Id"),
                            MedicienCode = row.IsNull("MedicienCode") ? "" : row.Field<string>("MedicienCode"),
                            MedicienName = row.IsNull("MedicienName") ? "" : row.Field<string>("MedicienName"),
                            Dose = row.IsNull("Dose") ? 0 : row.Field<int>("Dose"),
                            Duration = row.IsNull("Duration") ? 0 : row.Field<int>("Duration"),
                            TotalDuration = row.IsNull("TotalDuration") ? 0 : row.Field<int>("TotalDuration"),
                            TotalUnits = row.IsNull("TotalUnits") ? 0 : row.Field<int>("TotalUnits"),
                            Amount = row.IsNull("Amount") ? 0 : row.Field<double>("Amount"),
                            RoshitaID = row.IsNull("RoshitaID") ? 0 : row.Field<long>("RoshitaID"),
                            PaymentGroup = row.IsNull("PaymentGroup") ? null : row.Field<string>("PaymentGroup")
                        }).ToList();
                    }

                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        var itemexception = new RoshitaDetail();
                        foreach (var item in roshitaDetail)
                        {
                            try
                            {
                                string createdBy = "";

                                iNV_SALOracle = FillOracleRoshitaDetailObject(item);

                                iNV_SALOracle.INVT_SEQ = sequenc;
                                string QueryRoshita = "SELECT CardId,CreatedDate,Oracle_Id,CreatedBy,Manager  FROM Roshita WHERE Id= " + item.RoshitaID;
                                var RoshitaData = GetSqlDataTable(QueryRoshita, _connectionSettings.SQlConnection);
                                string manager = null;
                                if (RoshitaData.Rows.Count > 0)
                                {
                                    DateTime da = DateTime.Parse(RoshitaData.Rows[0][1].ToString());
                                    iNV_SALOracle.CARD_ID = RoshitaData.Rows[0][0].ToString();
                                    iNV_SALOracle.INV_DATE = new DateTime(da.Year, da.Month, da.Day);
                                    iNV_SALOracle.DATE_CHANGE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.DATE_DURATION = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.REG_DATE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.INV_ID = long.Parse(RoshitaData.Rows[0][2].ToString());
                                    createdBy = RoshitaData.Rows[0][3].ToString();
                                    manager = RoshitaData.Rows[0][4].ToString();
                                }

                                if (item.PaymentGroup == "Cash")
                                {
                                    iNV_SALOracle.CASH_TOT = iNV_SALOracle.AMOUNT;
                                }
                                else
                                {
                                    iNV_SALOracle.CASH_TOT = 0;
                                }
                                if (createdBy != null)
                                {

                                    string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                                    " WHERE UserName='" + createdBy + "';";
                                    var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    if (UserId.Rows.Count > 0)
                                    {
                                        var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                        string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                                       " WHERE PR_CODE=" + providerId + ";";
                                        var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                        if (UName.Rows.Count > 0)
                                        {
                                            iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_NAME = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                            iNV_SALOracle.NAME_PHARM = createdBy;
                                            iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                            iNV_SALOracle.NAME_EX = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : null;
                                        }
                                        else
                                        {
                                            string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                                  " WHERE UserCode='" + providerId + "';";
                                            var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                            if (MapName.Rows.Count > 0)
                                            {
                                                iNV_SALOracle.ID_PHARM = providerId;
                                                iNV_SALOracle.PH_ID = providerId;
                                                iNV_SALOracle.PH_NAME = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                                iNV_SALOracle.NAME_PHARM = createdBy;
                                                iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                                iNV_SALOracle.NAME_EX = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                            }
                                        }
                                    }


                                    //string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                    //                   " WHERE UserName='" + createdBy + "';";
                                    //var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    //if (UserId.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = createdBy;
                                    //    iNV_SALOracle.NAME_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? UserId.Rows[0][0].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;
                                    //}

                                    //var user = GetOracleDataTable("select USER_ID ,USER_CO,USER_N,USER_NAME from USERS_2 where USER_NAME='" + createdBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                                    //if (user.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = user.Rows[0][0].ToString() != string.Empty ? double.Parse(user.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : null;
                                    //    iNV_SALOracle.NAME_PHARM = user.Rows[0][3].ToString() != string.Empty ? user.Rows[0][3].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;

                                    //}
                                }
                                var medicineData = GetSqlDataTable(@"SELECT LIC_TYPE,DOSAGE_FORM,PACK_PRICE,UNIT_PRICE,UNIT_NO,PACK_SIZE,MED_GROUP " +
                                                     " FROM MedicineData WHERE M_CODE='" + item.MedicienCode + "'", _connectionSettings.SQlConnection);
                                if (medicineData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.LIC_TYPE = medicineData.Rows[0][0].ToString() != string.Empty ? medicineData.Rows[0][0].ToString() : null;
                                    iNV_SALOracle.MED_GROUP = 0;
                                    iNV_SALOracle.DOSAGE = medicineData.Rows[0][1].ToString() != string.Empty ? medicineData.Rows[0][1].ToString() : null;
                                    iNV_SALOracle.PACK_PRICE = medicineData.Rows[0][2].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][2].ToString()) : 0;
                                    iNV_SALOracle.PRICE_UNIT = medicineData.Rows[0][3].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][3].ToString()) : 0;
                                    iNV_SALOracle.UNIT = medicineData.Rows[0][4].ToString() != string.Empty ? int.Parse(medicineData.Rows[0][4].ToString()) : 0;
                                    iNV_SALOracle.SIZE_UNIT = medicineData.Rows[0][5].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][5].ToString()) : 0;

                                    if (manager != null && manager != "Doctor_Chronic")
                                    {
                                        var MedGroup = GetSqlDataTable("select GroupType from MedicineGroup where GroupId=" + long.Parse(medicineData.Rows[0][6].ToString()), _connectionSettings.SQlConnection);
                                        iNV_SALOracle.GRUOP_TYPE = MedGroup.Rows[0][0].ToString() != string.Empty ? MedGroup.Rows[0][0].ToString() : null;
                                        iNV_SALOracle.MED_GROUP = medicineData.Rows[0][6].ToString() != string.Empty ? long.Parse(medicineData.Rows[0][6].ToString()) : 0;
                                    }
                                    else
                                    {
                                        iNV_SALOracle.GRUOP_TYPE = null;
                                        iNV_SALOracle.MED_GROUP = null;
                                    }

                                }
                                if (iNV_SALOracle.LIC_TYPE == "IMPORT")
                                {
                                    iNV_SALOracle.AMOUNT_IMP = iNV_SALOracle.AMOUNT;
                                }
                                else
                                {
                                    iNV_SALOracle.AMOUNT_LOC = iNV_SALOracle.AMOUNT;
                                }

                                var entData = GetOracleDataTable(@"SELECT PERCENT_MONY,CLASS_CODE " +
                                                    " FROM DMS_02_EMP_D_ENT WHERE D_ID=" + iNV_SALOracle.INV_ID, _connectionSettings.OrcaleConnectionTRN_SQL);
                                if (entData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.P_CENT = entData.Rows[0][0].ToString() != string.Empty ? int.Parse(entData.Rows[0][0].ToString()) : 0;
                                    iNV_SALOracle.CLASS_CODE = entData.Rows[0][1].ToString() != string.Empty ? entData.Rows[0][1].ToString() : null;
                                }
                                switch (item.PaymentGroup.ToLower())
                                {
                                    case "cash":
                                        iNV_SALOracle.CASH_ITEM = "Cash";
                                        break;
                                    case "rejected":
                                        iNV_SALOracle.CASH_ITEM = "Cancel";
                                        break;
                                    case "accepted":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        break;
                                    case "yes":
                                        iNV_SALOracle.CASH_ITEM = "YES";
                                        break;
                                    case "approval":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        break;
                                    case "pending":
                                        iNV_SALOracle.CASH_ITEM = "PENDING";
                                        break;
                                    default:
                                        iNV_SALOracle.CASH_ITEM = "MON";
                                        break;
                                }
                                if (manager != null && manager == "Pharmacy_Chronic")
                                {
                                    iNV_SALOracle.CASH_ITEM = "MON";
                                }
                                sequenc++;
                                data.Add(iNV_SALOracle);
                            }
                            catch (Exception ex)
                            {
                                int w = data.Count();
                                itemexception = item;
                                string sex = ex.Message;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        string sex = ex.Message;
                    }

                    try
                    {
                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(data, "INV_SAL", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL",
                            AffectedRows = data.Count
                        });

                        if (data.Count == 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TEST'  AND Oracle_Id = '" + data[0].INV_ID + "' )";


                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TEST' AND Oracle_Id IN= '" + data[0].INV_ID + "'";

                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);

                        }
                        else if (data.Count > 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TEST'  AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaDetailsQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaDetailsQuery += data[data.Count - 1].INV_ID + "'))";
                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TEST' AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaQuery += data[data.Count - 1].INV_ID + "')";
                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL",
                            Exception = ex,
                        });
                    }


                }


                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException;
            }

        }


        private static void UpdatePushRoshita()
        {
            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CreatedDate DESC) AS RowNum, * FROM Roshita" +
                  " WHERE IsSync=1 AND SyncBy='update' AND Manager !='Doctor_Chronic')" +
                  "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  Roshita " +
               " WHERE IsSync=1 AND SyncBy='update' AND Manager !='Doctor_Chronic' ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);

            long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(D_SEQ),0)   FROM DMS_02_EMP_D_ENT ORDER BY D_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
            if (DateTime.Now.Day == 2)
            {
                sequenc = 1;
            }

            for (int i = 0; i < maxiteration; i = i)
            {

                List<Roshita> data = new List<Roshita>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {

                    data = data2.AsEnumerable().Select(row => new Roshita
                    {
                        Id = row.Field<long>("Id"),
                        CardId = row.IsNull("CardId") ? "Now" : row.Field<string>("CardId"),
                        Speciality = row.IsNull("Speciality") ? "Now" : row.Field<string>("Speciality"),
                        Diagnose1 = row.IsNull("Diagnose1") ? "Now" : row.Field<string>("Diagnose1"),
                        Diagnose2 = row.IsNull("Diagnose2") ? "Now" : row.Field<string>("Diagnose2"),
                        diagnose3 = row.IsNull("diagnose3") ? "Now" : row.Field<string>("diagnose3"),
                        RoshetaType = row.IsNull("RoshetaType") ? "Now" : row.Field<string>("RoshetaType"),
                        Limit = row.IsNull("Limit") ? 0 : row.Field<int>("Limit"),
                        CompanyPercent = row.IsNull("CompanyPercent") ? 0 : Convert.ToDouble(row.Field<int>("CompanyPercent")),
                        OverInsurance = row.IsNull("OverInsurance") ? 0 : row.Field<double>("OverInsurance"),
                        TotalValue = row.IsNull("TotalValue") ? 0 : row.Field<double>("TotalValue"),
                        CompanyPayment = row.IsNull("CompanyPayment") ? 0 : row.Field<double>("CompanyPayment"),
                        PersonPayment = row.IsNull("PersonPayment") ? 0 : row.Field<double>("PersonPayment"),
                        Cash = row.IsNull("Cash") ? 0 : row.Field<double>("Cash"),
                        Manager = row.IsNull("Manager") ? "Now" : row.Field<string>("Manager"),
                        CreatedBy = row.IsNull("CreatedBy") ? "Now" : row.Field<string>("CreatedBy"),
                        CreatedDate = row.IsNull("CreatedDate") ? DateTime.Now : (DateTime)row.Field<DateTime>("CreatedDate"),
                        PatchId = row.IsNull("PatchId") ? 0 : row.Field<int>("PatchId"),
                        PhoneNumber = row.IsNull("PhoneNumber") ? "Now" : row.Field<string>("PhoneNumber"),
                        Oracle_Id = row.IsNull("Oracle_Id") ? 0 : row.Field<long>("Oracle_Id"),
                        ClaimNumber = row.IsNull("ClaimNumber") ? 0 : row.Field<double>("ClaimNumber"),
                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }
                string tableName = "Update Ent";
                if (data.Count != 0)
                {
                    List<DMS_02_EMP_D_ENT3> roshitas = new List<DMS_02_EMP_D_ENT3>();
                    DMS_02_EMP_D_ENT3 rsh = new DMS_02_EMP_D_ENT3();
                    foreach (var item in data)
                    {
                        try
                        {
                            //string d_id = "2" + string.Format("{0:dd MM yyyy}", item.CreatedDate) + sequenc.ToString();
                            rsh = FillOracleRoshitaObject(item);
                            var empName = GetOracleDataTable(@"select* from comp_employees where card_id='" + item.CardId.ToUpper() + "' order by contract_no desc", _connectionSettings.OrcaleConnection);
                            //string result = string.Concat(d_id.Where(c => !char.IsWhiteSpace(c)));
                            rsh.D_SEQ = sequenc;
                            //rsh.D_ID = long.Parse(result);
                            rsh.INVOICE = sequenc.ToString();
                            rsh.C_COMP_ID = empName.Rows[0][3].ToString() != string.Empty ? Convert.ToInt32(empName.Rows[0][3].ToString()) : 0;
                            rsh.CONTRACT_NO = empName.Rows[0][4].ToString() != string.Empty ? Convert.ToInt32(empName.Rows[0][4].ToString()) : 0;
                            rsh.CLASS_CODE = empName.Rows[0][5].ToString() != string.Empty ? empName.Rows[0][5].ToString() : "";
                            DateTime insStartdate = DateTime.Parse(empName.Rows[0][25].ToString());
                            DateTime insEnddate = DateTime.Parse(empName.Rows[0][26].ToString());
                            rsh.INS_START_DATE = insStartdate.Date;
                            rsh.INS_END_DATE = insEnddate.Date;
                            rsh.D_EXP = empName.Rows[0][44].ToString() != string.Empty ? empName.Rows[0][44].ToString() : "";
                            rsh.D_EXP_2 = empName.Rows[0][45].ToString() != string.Empty ? empName.Rows[0][45].ToString() : "";
                            rsh.D_EXP_3 = empName.Rows[0][46].ToString() != string.Empty ? empName.Rows[0][46].ToString() : "";
                            rsh.D_EXP_4 = empName.Rows[0][47].ToString() != string.Empty ? empName.Rows[0][47].ToString() : "";
                            rsh.CUST_E_NAME = empName.Rows[0][49].ToString() != string.Empty ? empName.Rows[0][49].ToString() : "";
                            rsh.CUST_E_NAME_2 = empName.Rows[0][50].ToString() != string.Empty ? empName.Rows[0][50].ToString() : "";
                            rsh.CUST_E_NAME_3 = empName.Rows[0][51].ToString() != string.Empty ? empName.Rows[0][51].ToString() : "";
                            rsh.CUST_E_NAME_4 = String.Format("{0:dd/MM/yy hh:mm}", item.CreatedDate); /*item.CreatedDate.ToString();*/

                            string RoshitaAcception = "SELECT * FROM RoshitaAcceptions " +
                                "  WHERE RoshitaId=" + item.Id + ";";
                            var RoshitaAcc = GetSqlDataTable(RoshitaAcception, _connectionSettings.SQlConnection);

                            if (RoshitaAcc.Rows.Count > 0)
                            {
                                rsh.MASS_NO = int.Parse(RoshitaAcc.Rows[0][1].ToString());

                            }

                            string QueryLocal = "SELECT SUM(RoshitaDetails.Amount) FROM RoshitaDetails " +
                                "INNER JOIN  MedicineData  ON MedicineData.M_CODE=RoshitaDetails.MedicienCode " +
                                " AND MedicineData.LIC_TYPE='LOCAL' WHERE RoshitaID=" + item.Id + " AND IsDealed=1 AND " +
                                " PaymentGroup!='Pending' AND  PaymentGroup!='Rejected';";
                            string QueryUniversal = "SELECT SUM(RoshitaDetails.Amount) FROM RoshitaDetails " +
                                "INNER JOIN  MedicineData  ON MedicineData.M_CODE=RoshitaDetails.MedicienCode " +
                                " AND MedicineData.LIC_TYPE='IMPORT' WHERE RoshitaID=" + item.Id + " AND IsDealed=1 AND " +
                                " PaymentGroup!='Pending' AND  PaymentGroup!='Rejected' ;";
                            var TotalLocal = GetSqlDataTable(QueryLocal, _connectionSettings.SQlConnection);

                            var TotalImport = GetSqlDataTable(QueryUniversal, _connectionSettings.SQlConnection);

                            if (!TotalLocal.Rows[0][0].ToString().IsNullOrWhiteSpace())
                            {
                                rsh.LOC_AMOUNT = double.Parse(TotalLocal.Rows[0][0].ToString());
                            }
                            if (!TotalImport.Rows[0][0].ToString().IsNullOrWhiteSpace())
                            {
                                rsh.IMP_AMOUNT = double.Parse(TotalImport.Rows[0][0].ToString());
                            }
                            try
                            {
                                rsh.MGR_ID = int.Parse(GetSqlDataTable("SELECT COUNT(*) FROM RoshitaDetails WHERE RoshitaID=" + item.Id
                                    + " AND IsDealed = 1 AND  PaymentGroup!='Pending' AND  PaymentGroup!='Rejected' ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
                            }
                            catch (Exception ex)
                            {
                                var itemexception = item;
                            }
                            rsh.C_COMP_NAME = GetOracleDataTable(@"SELECT C_ANAME FROM CONTRACT_COMP WHERE C_COMP_ID='" + rsh.C_COMP_ID + "'", _connectionSettings.OrcaleConnection).Rows[0][0].ToString();

                            //var insurance = GetSqlDataTable(@"select INSURANCE_DAY,INSURANCE_MONTH  from CO_INSURANCE_01 where CO_ID=" + rsh.C_COMP_ID + " AND LIVEL='" + rsh.CLASS_CODE + "'", _connectionSettings.SQlConnection);
                            var insuranceMedEmp = GetSqlDataTable(@"select DAY_AMT,MON_AMT  from COMP_CUSTOMIZED_D_D_MED_EMP where CARD_ID='"
                                + rsh.CARD_ID + "' AND CLASS_CODE='" + rsh.CLASS_CODE + "' AND CONTRACT_NO=" + rsh.CONTRACT_NO
                                + " SER_SERV='" + item.RoshetaType + "' C_COMP_ID=" + rsh.C_COMP_ID, _connectionSettings.SQlConnection);
                            if (insuranceMedEmp.Rows.Count > 0)
                            {
                                rsh.INSU_LIMT = insuranceMedEmp.Rows[0][0].ToString() != string.Empty ? int.Parse(insuranceMedEmp.Rows[0][0].ToString()) : 0;

                                rsh.INSU_LIMT_M = insuranceMedEmp.Rows[0][1].ToString() != string.Empty ? int.Parse(insuranceMedEmp.Rows[0][1].ToString()) : 0;
                                if (rsh.INSU_LIMT_M == 0)
                                {
                                    rsh.NON_COVERED = rsh.OVER_INSURANCE;
                                }
                                else
                                {
                                    rsh.NON_COVERED = 0;
                                }
                            }
                            else
                            {
                                var insuranceMed = GetSqlDataTable(@"select DAY_AMT,MON_AMT  from COMP_CUSTOMIZED_D_D_MED where C_COMP_ID='"
                                + rsh.C_COMP_ID + "' AND CLASS_CODE='" + rsh.CLASS_CODE + "' AND CONTRACT_NO=" + rsh.CONTRACT_NO
                                + " SER_SERV='" + item.RoshetaType + "' ", _connectionSettings.SQlConnection);
                                if (insuranceMed.Rows.Count > 0)
                                {
                                    rsh.INSU_LIMT = insuranceMed.Rows[0][0].ToString() != string.Empty ? int.Parse(insuranceMed.Rows[0][0].ToString()) : 0;

                                    rsh.INSU_LIMT_M = insuranceMed.Rows[0][1].ToString() != string.Empty ? int.Parse(insuranceMed.Rows[0][1].ToString()) : 0;
                                    if (rsh.INSU_LIMT_M == 0)
                                    {
                                        rsh.NON_COVERED = rsh.OVER_INSURANCE;
                                    }
                                    else
                                    {
                                        rsh.NON_COVERED = 0;
                                    }
                                }
                            }


                            //string QueryLocal = "SELECT SUM(RoshitaDetails.Amount) FROM RoshitaDetails " +
                            //    "INNER JOIN  MedicineData  ON MedicineData.M_CODE=RoshitaDetails.MedicienCode " +
                            //    " AND MedicineData.LIC_TYPE='LOCAL' WHERE RoshitaID=" + item.Id + ";";
                            //string QueryUniversal = "SELECT SUM(RoshitaDetails.Amount) FROM RoshitaDetails " +
                            //    "INNER JOIN  MedicineData  ON MedicineData.M_CODE=RoshitaDetails.MedicienCode " +
                            //    " AND MedicineData.LIC_TYPE='IMPORT' WHERE RoshitaID=" + item.Id + ";";
                            //var TotalLocal = GetSqlDataTable(QueryLocal, _connectionSettings.SQlConnection);

                            //var TotalImport = GetSqlDataTable(QueryUniversal, _connectionSettings.SQlConnection);

                            //if (!TotalLocal.Rows[0][0].ToString().IsNullOrWhiteSpace())
                            //{
                            //    rsh.LOC_AMOUNT = double.Parse(TotalLocal.Rows[0][0].ToString());
                            //}
                            //if (!TotalImport.Rows[0][0].ToString().IsNullOrWhiteSpace())
                            //{
                            //    rsh.IMP_AMOUNT = double.Parse(TotalImport.Rows[0][0].ToString());
                            //}
                            //try
                            //{
                            //    rsh.MGR_ID = int.Parse(GetSqlDataTable("SELECT COUNT(*) FROM RoshitaDetails WHERE RoshitaID=" + item.Id, _connectionSettings.SQlConnection).Rows[0][0].ToString());
                            //}
                            //catch (Exception ex)
                            //{
                            //    var itemexception = item;
                            //}
                            //rsh.C_COMP_NAME = GetOracleDataTable(@"SELECT C_ANAME FROM CONTRACT_COMP WHERE C_COMP_ID='" + rsh.C_COMP_ID + "'", _connectionSettings.OrcaleConnection).Rows[0][0].ToString();

                            //var insurance = GetOracleDataTable(@"select INSURANCE_DAY,INSURANCE_MONTH  from CO_INSURANCE_01 where CO_ID=" + rsh.C_COMP_ID + " AND LIVEL='" + rsh.CLASS_CODE + "'", _connectionSettings.OrcaleConnectionSH65);
                            //rsh.INSU_LIMT = insurance.Rows[0][0].ToString() != string.Empty ? int.Parse(insurance.Rows[0][0].ToString()) : 0;
                            //if (insurance.Rows.Count > 0)
                            //{
                            //    rsh.INSU_LIMT_M = insurance.Rows[0][1].ToString() != string.Empty ? int.Parse(insurance.Rows[0][1].ToString()) : 0;
                            //    if (rsh.INSU_LIMT_M == 0)
                            //    {
                            //        rsh.NON_COVERED = rsh.OVER_INSURANCE;
                            //    }
                            //    else
                            //    {
                            //        rsh.NON_COVERED = 0;
                            //    }
                            //}

                            string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                " WHERE UserName='" + item.CreatedBy + "';";
                            var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                            if (UserId.Rows.Count > 0)
                            {
                                var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                               " WHERE PR_CODE=" + providerId + ";";
                                var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                if (UName.Rows.Count > 0)
                                {
                                    rsh.EMP_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    rsh.EMP_ID_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    rsh.EMP_SUB = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                }
                                else
                                {
                                    string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                          " WHERE UserCode='" + providerId + "';";
                                    var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                    if (MapName.Rows.Count > 0)
                                    {
                                        rsh.EMP_ID = providerId;
                                        rsh.EMP_ID_ID = providerId;
                                        rsh.EMP_SUB = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : "";
                                    }
                                }
                            }

                            //string QueryUser = "SELECT Provider FROM AspNetUsers " +
                            //    " WHERE UserName='" + item.CreatedBy + "';";
                            //var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                            //if (UserId.Rows.Count > 0)
                            //{
                            //    rsh.EMP_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                            //    rsh.EMP_ID_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                            //    rsh.EMP_SUB = item.CreatedBy;
                            //}


                            //var user = GetOracleDataTable(@"select USER_ID ,USER_CO,USER_N from USERS_2 where USER_NAME='" + item.CreatedBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                            //if (user.Rows.Count > 0)
                            //{
                            //    rsh.EMP_ID = user.Rows[0][0].ToString() != string.Empty ? long.Parse(user.Rows[0][0].ToString()) : 0;
                            //    rsh.EMP_ID_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                            //    rsh.EMP_SUB = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : "";
                            //}


                            var SER_PROV_DISC = GetOracleDataTable(@"select DEV_LOC_DIS,DEV_IMP_DIS  from SER_PROV_DISC where PROV_ID=" + rsh.EMP_ID_ID, _connectionSettings.OrcaleConnectionSH65);
                            if (SER_PROV_DISC.Rows.Count > 0)
                            {
                                rsh.DEV_LOC = SER_PROV_DISC.Rows[0][0].ToString() != string.Empty ? double.Parse(SER_PROV_DISC.Rows[0][0].ToString()) : 0;
                                rsh.DEV_IMP = SER_PROV_DISC.Rows[0][1].ToString() != string.Empty ? double.Parse(SER_PROV_DISC.Rows[0][1].ToString()) : 0;
                            }

                            rsh.EMP_NAME = item.CreatedBy;
                            rsh.IS_SYNC = 9;
                            rsh.SYNC_BY = "UPDATE";
                            rsh.SYNC_DATE = DateTime.Now;
                            item.Oracle_Id = rsh.D_ID;
                            //string updatesqlTable = "UPDATE Roshita set Oracle_Id=" + item.Oracle_Id + " WHERE Id=" + item.Id;

                            // ,IsSync = 1,SyncDate = '" + DateTime.Now + " , SyncBy='SQL'

                            //var resultmessage = ExecuteNonQueryCommand(updatesqlTable, _connectionSettings.SQlConnection);
                            // Lab_Stop,Ray_Stop,Lab,Ray
                            switch (item.Manager)
                            {
                                case "Daily":
                                    rsh.MANAGER = "YES";
                                    string QueryAcceptions = "SELECT c.AcceptionReasonsId FROM CardAcceptionReasons c " +
                                                 " JOIN RoshitaAcceptions r ON  c.AcceptionId=r.AcceptionId " +
                                               " AND r.RoshitaId=" + item.Id;
                                    var Acceptions = GetSqlDataTable(QueryAcceptions, _connectionSettings.SQlConnection);
                                    if (Acceptions.Rows.Count > 0)
                                    {
                                        for (int a = 0; a < Acceptions.Rows.Count; a++)
                                        {
                                            //var re = Acceptions.Rows[a][0].ToString();
                                            int val = Acceptions.Rows[a][0].ToString() != string.Empty ? int.Parse(Acceptions.Rows[a][0].ToString()) : 0;
                                            switch (val)
                                            {
                                                case 1:
                                                    rsh.TN1 = 1;
                                                    break;
                                                case 2:
                                                    rsh.TN2 = 1;
                                                    break;
                                                case 7:
                                                    rsh.TN3 = 1;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case "Pharmacy_Chronic":
                                    rsh.MANAGER = "MON";
                                    string QueryMedCard = "SELECT NO_PAY,NO_OVER FROM Med_Card " +
                                               " WHERE CARD_NO='" + item.CardId + "';";
                                    var medcard = GetSqlDataTable(QueryMedCard, _connectionSettings.SQlConnection);
                                    if (medcard.Rows.Count > 0)
                                    {
                                        rsh.TN1 = medcard.Rows[0][0].ToString() != string.Empty ? int.Parse(medcard.Rows[0][0].ToString()) : 0;
                                        rsh.TN2 = medcard.Rows[0][1].ToString() != string.Empty ? int.Parse(medcard.Rows[0][1].ToString()) : 0;
                                    }
                                    if (rsh.TN1 == 0 || rsh.TN2 == 0)
                                    {
                                        string QueryAcceptions3 = "SELECT c.AcceptionReasonsId FROM CardAcceptionReasons c " +
                                                 " JOIN RoshitaAcceptions r ON  c.AcceptionId=r.AcceptionId " +
                                               " AND r.RoshitaId=" + item.Id;
                                        var Acceptions3 = GetSqlDataTable(QueryAcceptions3, _connectionSettings.SQlConnection);
                                        if (Acceptions3.Rows.Count > 0)
                                        {
                                            for (int a = 0; a < Acceptions3.Rows.Count; a++)
                                            {
                                                int val = Acceptions3.Rows[a][0].ToString() != string.Empty ? int.Parse(Acceptions3.Rows[a][0].ToString()) : 0;
                                                switch (val)
                                                {
                                                    case 1:
                                                        rsh.TN1 = 1;
                                                        break;
                                                    case 2:
                                                        rsh.TN2 = 1;
                                                        break;
                                                    case 7:
                                                        rsh.TN3 = 1;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                case "Lab_Stop":
                                    rsh.MANAGER = "STOP_L";
                                    break;
                                case "Monthly":
                                    rsh.MANAGER = "MON_PH";

                                    string QueryAcceptions2 = "SELECT c.AcceptionReasonsId FROM CardAcceptionReasons c " +
                                                 " JOIN RoshitaAcceptions r ON  c.AcceptionId=r.AcceptionId " +
                                               " AND r.RoshitaId=" + item.Id;
                                    var Acceptions2 = GetSqlDataTable(QueryAcceptions2, _connectionSettings.SQlConnection);
                                    if (Acceptions2.Rows.Count > 0)
                                    {
                                        for (int a = 0; a < Acceptions2.Rows.Count; a++)
                                        {
                                            int val = Acceptions2.Rows[a][0].ToString() != string.Empty ? int.Parse(Acceptions2.Rows[a][0].ToString()) : 0;
                                            switch (val)
                                            {
                                                case 1:
                                                    rsh.TN1 = 1;
                                                    break;
                                                case 2:
                                                    rsh.TN2 = 1;
                                                    break;
                                                case 7:
                                                    rsh.TN3 = 1;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case "Daily_Stop":
                                    rsh.MANAGER = "STOP_PH";
                                    break;
                                case "Ray_Stop":
                                    rsh.MANAGER = "STOP_R";
                                    break;
                                case "Monthly_Stop ":
                                    rsh.MANAGER = "STOP_PH_MON";
                                    break;
                                case "Lab":
                                    rsh.MANAGER = "LAB";
                                    break;
                                case "Ray":
                                    rsh.MANAGER = "RAY";
                                    break;
                                case "Doctor_Daily":
                                    rsh.MANAGER = "DOC";
                                    break;
                                case "Doctor_Chronic":
                                    rsh.MANAGER = "DOC_MON";
                                    break;
                                case "Doctor_Daily_Stop":
                                    rsh.MANAGER = "STOP_DOC";
                                    break;
                                case "Pharmacy_Doctor":
                                    rsh.MANAGER = "YES";
                                    break;
                                case "Stop-ED":
                                    rsh.MANAGER = "STOP_ED";
                                    break;
                                default:
                                    rsh.MANAGER = item.Manager;
                                    break;
                            }

                            string QueryTASHKHES = "SELECT DiagnoiseName FROM PrescriptionRoshitaDignosis " +
                                " WHERE RositaId=" + item.Id + ";";
                            var TASHKHES = GetSqlDataTable(QueryTASHKHES, _connectionSettings.SQlConnection);
                            if (TASHKHES.Rows.Count > 0)
                            {
                                switch (TASHKHES.Rows.Count)
                                {
                                    case 0:
                                        rsh.TASHKHES_01 = null;
                                        rsh.TASHKHES_02 = null;
                                        rsh.TASHKHES_03 = null;
                                        break;
                                    case 1:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        break;
                                    case 2:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        break;
                                    case 3:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        rsh.TASHKHES_03 = TASHKHES.Rows[2][0].ToString();
                                        break;
                                    default:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        rsh.TASHKHES_03 = TASHKHES.Rows[2][0].ToString();
                                        break;
                                }
                            }

                            roshitas.Add(rsh);
                            sequenc++;
                        }
                        catch (Exception ex)
                        {
                            int w = roshitas.Count();
                            var itemexception = item;
                            string sex = ex.Message;
                        }

                    }


                    try
                    {
                        string ORAQuery2 = "DELETE INV_SAL" +
                            " WHERE INV_ID IN( '" + roshitas[0].D_ID + "','";

                        for (int j = 1; j < roshitas.Count - 1; j++)
                        {
                            ORAQuery2 += roshitas[j].D_ID + "','";
                        }
                        ORAQuery2 += roshitas[roshitas.Count - 1].D_ID + "')";
                        ExecuteOracleQuery(ORAQuery2, _connectionSettings.OrcaleConnectionTRN_SQL);


                        string ORAQuery = "DELETE DMS_02_EMP_D_ENT" +
                            " WHERE D_ID IN( '" + roshitas[0].D_ID + "','";

                        for (int j = 1; j < roshitas.Count - 1; j++)
                        {
                            ORAQuery += roshitas[j].D_ID + "','";
                        }
                        ORAQuery += roshitas[roshitas.Count - 1].D_ID + "')";
                        ExecuteOracleQuery(ORAQuery, _connectionSettings.OrcaleConnectionTRN_SQL);

                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(roshitas, "DMS_02_EMP_D_ENT", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = tableName,
                            AffectedRows = roshitas.Count
                        });

                        string SQLQuery = "UPDATE  Roshita SET IsSync=1,SyncBy='TESTUPDATE',SyncDate=GETDATE() " +
                            " WHERE Oracle_Id IN( '" + roshitas[0].D_ID + "','";

                        for (int j = 1; j < roshitas.Count - 1; j++)
                        {
                            SQLQuery += roshitas[j].D_ID + "','";
                        }
                        SQLQuery += roshitas[roshitas.Count - 1].D_ID + "')";
                        var resultmessage2 = ExecuteNonQueryCommand(SQLQuery, _connectionSettings.SQlConnection);


                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }
            }
        }

        private static void UpdatePushRoshitaDetails()
        {
            try
            {

                var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY (select null)) AS RowNum, " +
                    "* FROM RoshitaDetails WHERE roshitaid in (SELECT id FROM roshita  WHERE IsSync=1 AND SyncBy='TESTUPDATE' " +
                    " AND Manager NOT IN('Lab_Stop','Ray_Stop','Lab','Ray','Doctor_Chronic') ))" +
                    "AS m WHERE RowNum > {0} AND RowNum<= {1}";




                #region RoshitaDetails
                double count = double.Parse(GetSqlDataTable("SELECT Count(*) FROM Roshitadetails WHERE roshitaid in (SELECT id FROM Roshita  WHERE IsSync=1 AND SyncBy='TESTUPDATE'" +
                                    " AND Manager NOT IN('Lab_Stop','Ray_Stop','Lab','Ray','Doctor_Chronic') )", _connectionSettings.SQlConnection).Rows[0][0].ToString());

                var maxiteration = Math.Ceiling(count / 1000);
                long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(INVT_SEQ),0)   FROM INV_SAL ORDER BY INV_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
                if (DateTime.Now.Day == 2)
                {
                    sequenc = 1;
                }
                for (int i = 0; i < maxiteration; i = i)
                {
                    List<INV_SALOracle> data = new List<INV_SALOracle>();

                    INV_SALOracle iNV_SALOracle = new INV_SALOracle();

                    //data.AddRange(Inv_Sal_Medicine(i, query));
                    //data.AddRange(Inv_Sal_Lab(i, queryLab));
                    //data.AddRange(Inv_Sal_Ray(i, queryRay));


                    List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();
                    var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);

                    //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                    try
                    {

                        roshitaDetail = data2.AsEnumerable().Select(row => new RoshitaDetail
                        {
                            Id = row.Field<long>("Id"),
                            MedicienCode = row.IsNull("MedicienCode") ? "" : row.Field<string>("MedicienCode"),
                            MedicienName = row.IsNull("MedicienName") ? "" : row.Field<string>("MedicienName"),
                            Dose = row.IsNull("Dose") ? 0 : row.Field<int>("Dose"),
                            Duration = row.IsNull("Duration") ? 0 : row.Field<int>("Duration"),
                            TotalDuration = row.IsNull("TotalDuration") ? 0 : row.Field<int>("TotalDuration"),
                            TotalUnits = row.IsNull("TotalUnits") ? 0 : row.Field<int>("TotalUnits"),
                            Amount = row.IsNull("Amount") ? 0 : row.Field<double>("Amount"),
                            RoshitaID = row.IsNull("RoshitaID") ? 0 : row.Field<long>("RoshitaID"),
                            PaymentGroup = row.IsNull("PaymentGroup") ? null : row.Field<string>("PaymentGroup")
                        }).ToList();
                    }

                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        var itemexception = new RoshitaDetail();
                        foreach (var item in roshitaDetail)
                        {
                            try
                            {
                                string createdBy = "";

                                iNV_SALOracle = FillOracleRoshitaDetailObject(item);

                                iNV_SALOracle.INVT_SEQ = sequenc;
                                string QueryRoshita = "SELECT CardId,CreatedDate,Oracle_Id,CreatedBy,Manager  FROM Roshita WHERE Id= " + item.RoshitaID;
                                var RoshitaData = GetSqlDataTable(QueryRoshita, _connectionSettings.SQlConnection);
                                string manager = null;
                                if (RoshitaData.Rows.Count > 0)
                                {
                                    DateTime da = DateTime.Parse(RoshitaData.Rows[0][1].ToString());
                                    iNV_SALOracle.CARD_ID = RoshitaData.Rows[0][0].ToString();
                                    iNV_SALOracle.INV_DATE = new DateTime(da.Year, da.Month, da.Day);
                                    iNV_SALOracle.DATE_CHANGE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.DATE_DURATION = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.REG_DATE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.INV_ID = long.Parse(RoshitaData.Rows[0][2].ToString());
                                    createdBy = RoshitaData.Rows[0][3].ToString();
                                    manager = RoshitaData.Rows[0][4].ToString();
                                }

                                if (item.PaymentGroup == "Cash")
                                {
                                    iNV_SALOracle.CASH_TOT = iNV_SALOracle.AMOUNT;
                                }
                                else
                                {
                                    iNV_SALOracle.CASH_TOT = 0;
                                }
                                if (createdBy != null)
                                {
                                    string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                                    " WHERE UserName='" + createdBy + "';";
                                    var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    if (UserId.Rows.Count > 0)
                                    {
                                        var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                        string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                                       " WHERE PR_CODE=" + providerId + ";";
                                        var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                        if (UName.Rows.Count > 0)
                                        {
                                            iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_NAME = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                            iNV_SALOracle.NAME_PHARM = createdBy;
                                            iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                            iNV_SALOracle.NAME_EX = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : null;
                                        }
                                        else
                                        {
                                            string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                                  " WHERE UserCode='" + providerId + "';";
                                            var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                            if (MapName.Rows.Count > 0)
                                            {
                                                iNV_SALOracle.ID_PHARM = providerId;
                                                iNV_SALOracle.PH_ID = providerId;
                                                iNV_SALOracle.PH_NAME = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                                iNV_SALOracle.NAME_PHARM = createdBy;
                                                iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                                iNV_SALOracle.NAME_EX = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                            }
                                        }
                                    }
                                    //string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                    //                   " WHERE UserName='" + createdBy + "';";
                                    //var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    //if (UserId.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = createdBy;
                                    //    iNV_SALOracle.NAME_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? UserId.Rows[0][0].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;
                                    //}

                                    //var user = GetOracleDataTable("select USER_ID ,USER_CO,USER_N,USER_NAME from USERS_2 where USER_NAME='" + createdBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                                    //if (user.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = user.Rows[0][0].ToString() != string.Empty ? double.Parse(user.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : null;
                                    //    iNV_SALOracle.NAME_PHARM = user.Rows[0][3].ToString() != string.Empty ? user.Rows[0][3].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;

                                    //}
                                }

                                var medicineData = GetSqlDataTable(@"SELECT LIC_TYPE,DOSAGE_FORM,PACK_PRICE,UNIT_PRICE,UNIT_NO,PACK_SIZE,MED_GROUP " +
                                                     " FROM MedicineData WHERE M_CODE='" + item.MedicienCode + "'", _connectionSettings.SQlConnection);
                                if (medicineData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.LIC_TYPE = medicineData.Rows[0][0].ToString() != string.Empty ? medicineData.Rows[0][0].ToString() : null;
                                    iNV_SALOracle.MED_GROUP = 0;
                                    iNV_SALOracle.DOSAGE = medicineData.Rows[0][1].ToString() != string.Empty ? medicineData.Rows[0][1].ToString() : null;
                                    iNV_SALOracle.PACK_PRICE = medicineData.Rows[0][2].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][2].ToString()) : 0;
                                    iNV_SALOracle.PRICE_UNIT = medicineData.Rows[0][3].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][3].ToString()) : 0;
                                    iNV_SALOracle.UNIT = medicineData.Rows[0][4].ToString() != string.Empty ? int.Parse(medicineData.Rows[0][4].ToString()) : 0;
                                    iNV_SALOracle.SIZE_UNIT = medicineData.Rows[0][5].ToString() != string.Empty ? double.Parse(medicineData.Rows[0][5].ToString()) : 0;

                                    if (manager != null && manager != "Doctor_Chronic")
                                    {
                                        var MedGroup = GetSqlDataTable("select GroupType from MedicineGroup where GroupId=" + long.Parse(medicineData.Rows[0][6].ToString()), _connectionSettings.SQlConnection);
                                        iNV_SALOracle.GRUOP_TYPE = MedGroup.Rows[0][0].ToString() != string.Empty ? MedGroup.Rows[0][0].ToString() : null;
                                        iNV_SALOracle.MED_GROUP = medicineData.Rows[0][6].ToString() != string.Empty ? long.Parse(medicineData.Rows[0][6].ToString()) : 0;
                                    }
                                    else
                                    {
                                        iNV_SALOracle.GRUOP_TYPE = null;
                                        iNV_SALOracle.MED_GROUP = null;
                                    }

                                }
                                if (iNV_SALOracle.LIC_TYPE == "IMPORT")
                                {
                                    iNV_SALOracle.AMOUNT_IMP = iNV_SALOracle.AMOUNT;
                                }
                                else
                                {
                                    iNV_SALOracle.AMOUNT_LOC = iNV_SALOracle.AMOUNT;
                                }

                                var entData = GetOracleDataTable(@"SELECT PERCENT_MONY,CLASS_CODE " +
                                                    " FROM DMS_02_EMP_D_ENT WHERE D_ID=" + iNV_SALOracle.INV_ID, _connectionSettings.OrcaleConnectionTRN_SQL);
                                if (entData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.P_CENT = entData.Rows[0][0].ToString() != string.Empty ? int.Parse(entData.Rows[0][0].ToString()) : 0;
                                    iNV_SALOracle.CLASS_CODE = entData.Rows[0][1].ToString() != string.Empty ? entData.Rows[0][1].ToString() : null;
                                }
                                switch (item.PaymentGroup.ToLower())
                                {
                                    case "cash":
                                        iNV_SALOracle.CASH_ITEM = "Cash";
                                        break;
                                    case "rejected":
                                        iNV_SALOracle.CASH_ITEM = "Cancel";
                                        break;
                                    case "accepted":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        break;
                                    case "yes":
                                        iNV_SALOracle.CASH_ITEM = "YES";
                                        break;
                                    case "approval":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        break;
                                    case "pending":
                                        iNV_SALOracle.CASH_ITEM = "PENDING";
                                        break;
                                    default:
                                        iNV_SALOracle.CASH_ITEM = "MON";
                                        break;
                                }
                                if (manager != null && manager == "Pharmacy_Chronic")
                                {
                                    iNV_SALOracle.CASH_ITEM = "MON";
                                }
                                sequenc++;
                                iNV_SALOracle.IS_SYNC = 9;
                                iNV_SALOracle.SYNC_BY = "UPDATE";
                                iNV_SALOracle.SYNC_DATE = DateTime.Now;
                                data.Add(iNV_SALOracle);
                            }
                            catch (Exception ex)
                            {
                                int w = data.Count();
                                itemexception = item;
                                string sex = ex.Message;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        string sex = ex.Message;
                    }

                    try
                    {
                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(data, "INV_SAL", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "Update INV_SAL",
                            AffectedRows = data.Count
                        });

                        if (data.Count == 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TESTUPDATE'  AND Oracle_Id = '" + data[0].INV_ID + "' )";


                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TESTUPDATE' AND Oracle_Id = '" + data[0].INV_ID + "'";

                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);

                        }
                        else if (data.Count > 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TESTUPDATE'  AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaDetailsQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaDetailsQuery += data[data.Count - 1].INV_ID + "'))";
                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TESTUPDATE' AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaQuery += data[data.Count - 1].INV_ID + "')";
                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL",
                            Exception = ex,
                        });
                    }


                }


                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException;
            }
        }


        private static void UpdatePushRoshita2()
        {
            var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CreatedDate DESC) AS RowNum, * FROM Roshita" +
                  " WHERE IsSync=1 AND SyncBy='update' )" +
                  "AS m WHERE RowNum > {0} AND RowNum<= {1}";
            double count = double.Parse(GetSqlDataTable("select COUNT(*) from  Roshita " +
               " WHERE IsSync=1 AND SyncBy='update' ", _connectionSettings.SQlConnection).Rows[0][0].ToString());
            var maxiteration = Math.Ceiling(count / 1000);
            for (int i = 0; i < maxiteration; i = i)
            {

                List<Roshita> data = new List<Roshita>();
                var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                try
                {

                    data = data2.AsEnumerable().Select(row => new Roshita
                    {
                        Id = row.Field<long>("Id"),
                        CardId = row.IsNull("CardId") ? "Now" : row.Field<string>("CardId"),
                        Speciality = row.IsNull("Speciality") ? "Now" : row.Field<string>("Speciality"),
                        Diagnose1 = row.IsNull("Diagnose1") ? "Now" : row.Field<string>("Diagnose1"),
                        Diagnose2 = row.IsNull("Diagnose2") ? "Now" : row.Field<string>("Diagnose2"),
                        diagnose3 = row.IsNull("diagnose3") ? "Now" : row.Field<string>("diagnose3"),
                        RoshetaType = row.IsNull("RoshetaType") ? "Now" : row.Field<string>("RoshetaType"),
                        Limit = row.IsNull("Limit") ? 0 : row.Field<int>("Limit"),
                        CompanyPercent = row.IsNull("CompanyPercent") ? 0 : Convert.ToDouble(row.Field<int>("CompanyPercent")),
                        OverInsurance = row.IsNull("OverInsurance") ? 0 : row.Field<double>("OverInsurance"),
                        TotalValue = row.IsNull("TotalValue") ? 0 : row.Field<double>("TotalValue"),
                        CompanyPayment = row.IsNull("CompanyPayment") ? 0 : row.Field<double>("CompanyPayment"),
                        PersonPayment = row.IsNull("PersonPayment") ? 0 : row.Field<double>("PersonPayment"),
                        Cash = row.IsNull("Cash") ? 0 : row.Field<double>("Cash"),
                        Manager = row.IsNull("Manager") ? "Now" : row.Field<string>("Manager"),
                        CreatedBy = row.IsNull("CreatedBy") ? "Now" : row.Field<string>("CreatedBy"),
                        CreatedDate = row.IsNull("CreatedDate") ? DateTime.Now : (DateTime)row.Field<DateTime>("CreatedDate"),
                        PatchId = row.IsNull("PatchId") ? 0 : row.Field<int>("PatchId"),
                        PhoneNumber = row.IsNull("PhoneNumber") ? "Now" : row.Field<string>("PhoneNumber"),
                        Oracle_Id = row.IsNull("Oracle_Id") ? 0 : row.Field<long>("Oracle_Id"),
                        ClaimNumber = row.IsNull("ClaimNumber") ? 0 : row.Field<double>("ClaimNumber"),
                    }).ToList();
                }

                catch (Exception ex)
                {
                    var e = ex.Message;
                }
                if (data.Count != 0)
                {

                    //List<DMS_02_EMP_D_ENT3> roshitas = new List<DMS_02_EMP_D_ENT3>();
                    DMS_02_EMP_D_ENT3 rsh = new DMS_02_EMP_D_ENT3();
                    string manager = "";
                    double percent;
                    foreach (var item in data)
                    {
                        try
                        {

                            switch (item.Manager)
                            {
                                case "Daily":
                                    rsh.MANAGER = "YES";
                                    break;
                                case "Pharmacy_Chronic":
                                    rsh.MANAGER = "MON";
                                    break;
                                case "Lab_Stop":
                                    rsh.MANAGER = "STOP_L";
                                    break;
                                case "Monthly":
                                    rsh.MANAGER = "MON_PH";
                                    break;
                                case "Daily_Stop":
                                    rsh.MANAGER = "STOP_PH";
                                    break;
                                case "Ray_Stop":
                                    rsh.MANAGER = "STOP_R";
                                    break;
                                case "Monthly_Stop ":
                                    rsh.MANAGER = "STOP_PH_MON";
                                    break;
                                case "Lab":
                                    rsh.MANAGER = "LAB";
                                    break;
                                case "Ray":
                                    rsh.MANAGER = "RAY";
                                    break;
                                case "Doctor_Daily":
                                    rsh.MANAGER = "DOC";
                                    break;
                                case "Doctor_Chronic":
                                    rsh.MANAGER = "DOC_MON";
                                    break;
                                case "Doctor_Daily_Stop":
                                    rsh.MANAGER = "STOP_DOC";
                                    break;
                                default:
                                    rsh.MANAGER = item.Manager;
                                    break;
                            }

                            string QueryTASHKHES = "SELECT DiagnoiseName FROM PrescriptionRoshitaDignosis " +
                                " WHERE RositaId=" + item.Id + ";";
                            var TASHKHES = GetSqlDataTable(QueryTASHKHES, _connectionSettings.SQlConnection);
                            if (TASHKHES.Rows.Count > 0)
                            {
                                switch (TASHKHES.Rows.Count)
                                {
                                    case 0:
                                        rsh.TASHKHES_01 = null;
                                        rsh.TASHKHES_02 = null;
                                        rsh.TASHKHES_03 = null;
                                        break;
                                    case 1:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        break;
                                    case 2:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        break;
                                    case 3:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        rsh.TASHKHES_03 = TASHKHES.Rows[2][0].ToString();
                                        break;
                                    default:
                                        rsh.TASHKHES_01 = TASHKHES.Rows[0][0].ToString();
                                        rsh.TASHKHES_02 = TASHKHES.Rows[1][0].ToString();
                                        rsh.TASHKHES_03 = TASHKHES.Rows[2][0].ToString();
                                        break;
                                }
                            }


                            percent = 100 - (int)item.CompanyPercent;
                            string OracleQuery = "UPDATE DMS_02_EMP_D_ENT SET D_VD=" + item.TotalValue +
                                " , PERCENT_MONY=" + percent + " , MANAGER='" + manager + "' , INSU_LIMT=" + item.Limit +
                                " , CARRY=" + item.PersonPayment + " ,OVER_INSURANCE=" + item.OverInsurance + " ,VALUE_CASH=" +
                                item.Cash + " , VALUE_CREDIT=" + item.CompanyPayment + " , SYNC_DATE=SYSDATE WHERE D_ID=" + item.Oracle_Id +
                                " AND CARD_ID='" + item.CardId + "'";
                            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnectionSH65);

                        }
                        catch (Exception ex)
                        {
                            var e = ex.Message;
                        }
                    }
                }
            }
        }

        private static INV_SALOracle FillOracleRoshitaDetailObject(RoshitaDetail obj)
        {
            try
            {
                return new INV_SALOracle
                {
                    MANAGER = 1,
                    CASH_ITEM = "YES",
                    CANCEL_ITEM = "0",
                    TOT_PEND = 0,
                    TOT_TOT_PEND = 0,
                    KIND_EX = 1,
                    REPET_KIND = 0,
                    REPET_MONTH = 0,
                    APP_TOT = 0,
                    DISCOUNT_CLAIM = 0,
                    PAY_CLAIM = 0,
                    MAN_CLM = 0,
                    LOC_PER = 0,
                    IMP_PER = 0,
                    CONS = 0,
                    IS_SYNC = 2,
                    SYNC_BY = "SQL",
                    SYNC_DATE = DateTime.Now,
                    PART_NO = obj.MedicienCode,
                    INVT_NO = long.Parse(obj.MedicienCode),
                    INVT_NAM = obj.MedicienName,
                    DOSE = obj.Dose,
                    DURATION = obj.Duration,
                    T_DURATION = obj.TotalDuration,
                    COUNT = obj.TotalUnits,
                    AMOUNT = obj.Amount,
                    GRUOP_TYPE = obj.PaymentGroup,
                    YES_TOT = obj.Amount,

                };
            }
            catch (Exception ex)
            {
                return new INV_SALOracle();
            }
        }

        private static INV_RayOracle FillOracleRayDetailObject(RoshitaDetail obj)
        {
            try
            {
                return new INV_RayOracle
                {
                    MANAGER = 1,

                    TOT_PEND = 0,
                    TOT_TOT_PEND = 0,
                    KIND_EX = 1,
                    REPET_KIND = 0,
                    REPET_MONTH = 0,

                    DISCOUNT_CLAIM = 0,
                    PAY_CLAIM = 0,
                    MAN_CLM = 0,
                    LOC_PER = 0,
                    IMP_PER = 0,
                    CONS = 0,
                    IS_SYNC = 2,
                    SYNC_BY = "SQL",
                    SYNC_DATE = DateTime.Now,
                    PART_NO = obj.MedicienCode,
                    DMS_CODE = long.Parse(obj.MedicienCode),
                    INVT_NO = long.Parse(obj.MedicienCode),
                    INVT_NAM = obj.MedicienName,
                    DOSE = obj.Dose,
                    DURATION = obj.Duration,
                    T_DURATION = obj.TotalDuration,
                    COUNT = obj.TotalUnits,
                    AMOUNT = obj.Amount,



                };
            }
            catch (Exception ex)
            {
                return new INV_RayOracle();
            }
        }

        #region SQL Functions

        private static void AddNewEntitiesSql2<T>(List<T> list, string tableName, bool keepIdentity = false, string connectionString = null)
        {
            try
            {
                connectionString = connectionString == null ? _currenctConnectionString : connectionString;
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(connectionString))
                {

                    connection.Open();

                    DataTable dataTable = new DataTable();

                    GetDataTableSchemaFromTableOracle(tableName, dataTable, connectionString);
                    try
                    {
                        ToDataTable(list, dataTable);
                    }
                    catch (Exception ex)
                    {
                        var message = ex.Message;
                    }

                    var options = GetDefaultSyncOptions();
                    if (keepIdentity)
                    {
                        options = GetSyncOptionsWithKeepIdentity();
                    }
                    ////using (var sqlBulk = new SqlBulkCopy(connectionString, options))
                    ////{
                    ////    if (_batchSize.HasValue)
                    ////    {
                    ////        sqlBulk.BatchSize = _batchSize.Value;
                    ////    }
                    ////    sqlBulk.DestinationTableName = tableName;
                    ////    sqlBulk.WriteToServer(dataTable);


                    //}
                    using (var bulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(connection, Oracle.DataAccess.Client.OracleBulkCopyOptions.Default))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.WriteToServer(dataTable);
                    }
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }


        private static DataTable GetDataTableSchemaFromTableOracle(string tableName, DataTable dataTable = null, string connectionString = null)
        {
            connectionString = connectionString == null ? _currenctConnectionString : connectionString;

            if (dataTable == null)
            {
                dataTable = new DataTable();
            }

            using (OracleConnection oracleConn = new OracleConnection(connectionString))
            {
                using (OracleCommand command = oracleConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM {0} where 1 = 0", tableName);
                    command.CommandType = CommandType.Text;
                    oracleConn.Open();

                    OracleDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly);

                    dataTable.Load(reader);
                    oracleConn.Close();
                }
            }
            return dataTable;
        }

        #endregion

        #endregion

        private static void PushRayDetails()
        {
            try
            {
                var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY (select null)) AS RowNum, " +
                    "* FROM RoshitaDetails WHERE roshitaid in (SELECT id FROM roshita  WHERE IsSync=1 AND SyncBy='TEST' " +
                    " AND Manager  IN('Ray_Stop','Ray') ))" +
                    "AS m WHERE RowNum > {0} AND RowNum<= {1}";

                #region RoshitaDetails

                double count = double.Parse(GetSqlDataTable("SELECT Count(*) FROM Roshitadetails WHERE roshitaid in (SELECT id FROM Roshita  WHERE IsSync=1 AND SyncBy='TEST'" +
                    " AND Manager IN('Ray_Stop','Ray') )", _connectionSettings.SQlConnection).Rows[0][0].ToString());

                var maxiteration = Math.Ceiling(count / 10000);
                long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(INVT_SEQ),0)   FROM INV_SAL_RAY ORDER BY INV_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
                if (DateTime.Now.Day == 2)
                {
                    sequenc = 1;
                }
                for (int i = 0; i < maxiteration; i = i)
                {
                    List<INV_RayOracle> data = new List<INV_RayOracle>();

                    INV_RayOracle iNV_SALOracle = new INV_RayOracle();

                    List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();
                    var data2 = GetSqlDataTable(string.Format(query, (i * 10000), ((++i) * 10000)), _connectionSettings.SQlConnection);
                    try
                    {

                        roshitaDetail = data2.AsEnumerable().Select(row => new RoshitaDetail
                        {
                            Id = row.Field<long>("Id"),
                            MedicienCode = row.IsNull("MedicienCode") ? "" : row.Field<string>("MedicienCode"),
                            MedicienName = row.IsNull("MedicienName") ? "" : row.Field<string>("MedicienName"),
                            Dose = row.IsNull("Dose") ? 0 : row.Field<int>("Dose"),
                            Duration = row.IsNull("Duration") ? 0 : row.Field<int>("Duration"),
                            TotalDuration = row.IsNull("TotalDuration") ? 0 : row.Field<int>("TotalDuration"),
                            TotalUnits = row.IsNull("TotalUnits") ? 0 : row.Field<int>("TotalUnits"),
                            Amount = row.IsNull("Amount") ? 0 : row.Field<double>("Amount"),
                            RoshitaID = row.IsNull("RoshitaID") ? 0 : row.Field<long>("RoshitaID"),
                            PaymentGroup = row.IsNull("PaymentGroup") ? null : row.Field<string>("PaymentGroup")
                        }).ToList();
                    }

                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        var itemexception = new RoshitaDetail();
                        foreach (var item in roshitaDetail)
                        {
                            try
                            {
                                string createdBy = "";

                                iNV_SALOracle = FillOracleRayDetailObject(item);

                                iNV_SALOracle.INVT_SEQ = sequenc;
                                string QueryRoshita = "SELECT CardId,CreatedDate,Oracle_Id,CreatedBy,Manager  FROM Roshita WHERE Id= " + item.RoshitaID;
                                var RoshitaData = GetSqlDataTable(QueryRoshita, _connectionSettings.SQlConnection);
                                string manager = null;
                                if (RoshitaData.Rows.Count > 0)
                                {
                                    DateTime da = DateTime.Parse(RoshitaData.Rows[0][1].ToString());
                                    iNV_SALOracle.CARD_ID = RoshitaData.Rows[0][0].ToString();
                                    iNV_SALOracle.INV_DATE = new DateTime(da.Year, da.Month, da.Day);
                                    iNV_SALOracle.DATE_CHANGE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.DATE_DURATION = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.REG_DATE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.INV_ID = long.Parse(RoshitaData.Rows[0][2].ToString());
                                    createdBy = RoshitaData.Rows[0][3].ToString();
                                    manager = RoshitaData.Rows[0][4].ToString();
                                }


                                switch (item.PaymentGroup)
                                {
                                    case "Approved":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.APP_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    case "Pending":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Accepted":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "approved";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Rejected":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "Rejected";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CANCEL_ITEM = "3";
                                        break;
                                    case "Cash":
                                        iNV_SALOracle.CASH_ITEM = "Cash";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CASH_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    default:
                                        iNV_SALOracle.CASH_ITEM = "YES";
                                        iNV_SALOracle.GRUOP_TYPE = "YES";
                                        iNV_SALOracle.YES_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                }

                                if (createdBy != null)
                                {

                                    string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                                    " WHERE UserName='" + createdBy + "';";
                                    var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    if (UserId.Rows.Count > 0)
                                    {
                                        var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                        string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                                       " WHERE PR_CODE=" + providerId + ";";
                                        var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                        if (UName.Rows.Count > 0)
                                        {
                                            iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_NAME = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                            iNV_SALOracle.NAME_PHARM = createdBy;
                                            iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                            iNV_SALOracle.NAME_EX = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : null;
                                        }
                                        else
                                        {
                                            string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                                  " WHERE UserCode='" + providerId + "';";
                                            var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                            if (MapName.Rows.Count > 0)
                                            {
                                                iNV_SALOracle.ID_PHARM = providerId;
                                                iNV_SALOracle.PH_ID = providerId;
                                                iNV_SALOracle.PH_NAME = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                                iNV_SALOracle.NAME_PHARM = createdBy;
                                                iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                                iNV_SALOracle.NAME_EX = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                            }
                                        }
                                    }

                                    //string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                    //                   " WHERE UserName='" + createdBy + "';";
                                    //var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    //if (UserId.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = createdBy;
                                    //    iNV_SALOracle.NAME_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? UserId.Rows[0][0].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;
                                    //}

                                    //var user = GetOracleDataTable("select USER_ID ,USER_CO,USER_N,USER_NAME from USERS_2 where USER_NAME='" + createdBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                                    //if (user.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = user.Rows[0][0].ToString() != string.Empty ? double.Parse(user.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : null;
                                    //    iNV_SALOracle.NAME_PHARM = user.Rows[0][3].ToString() != string.Empty ? user.Rows[0][3].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;

                                    //}
                                }

                                var group = GetSqlDataTable("SELECT s.GRUOP_ID FROM Serv_Ray s " +
                                            " Join AspNetUsers u on u.Provider = s.LAB_CODE " +
                                        " WHERE u.UserName = '" + createdBy + "' AND s.SERV_CODE ="
                                        + item.MedicienCode, _connectionSettings.SQlConnection);
                                if (group.Rows.Count > 0)
                                {
                                    iNV_SALOracle.MED_GROUP = group.Rows[0][0].ToString() != string.Empty ? long.Parse(group.Rows[0][0].ToString()) : 0;
                                }

                                var entData = GetOracleDataTable(@"SELECT PERCENT_MONY,CLASS_CODE " +
                                                    " FROM DMS_02_EMP_D_ENT WHERE D_ID=" + iNV_SALOracle.INV_ID, _connectionSettings.OrcaleConnectionTRN_SQL);
                                if (entData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.P_CENT = entData.Rows[0][0].ToString() != string.Empty ? int.Parse(entData.Rows[0][0].ToString()) : 0;
                                    iNV_SALOracle.CLASS_CODE = entData.Rows[0][1].ToString() != string.Empty ? entData.Rows[0][1].ToString() : null;
                                }
                                sequenc++;
                                data.Add(iNV_SALOracle);
                            }
                            catch (Exception ex)
                            {
                                int w = data.Count();
                                itemexception = item;
                                string sex = ex.Message;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        string sex = ex.Message;
                    }

                    try
                    {
                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(data, "INV_SAL_RAY", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL_RAY3",
                            AffectedRows = data.Count
                        });

                        if (data.Count == 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TEST'  AND Oracle_Id = '" + data[0].INV_ID + "' )";


                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TEST' AND Oracle_Id IN= '" + data[0].INV_ID + "'";

                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);

                        }
                        else if (data.Count > 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TEST'  AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaDetailsQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaDetailsQuery += data[data.Count - 1].INV_ID + "'))";
                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TEST' AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaQuery += data[data.Count - 1].INV_ID + "')";
                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL_RAY3",
                            Exception = ex,
                        });
                    }


                }


                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException;
            }

        }

        private static void PushLabDetails()
        {
            try
            {
                var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY (select null)) AS RowNum, " +
                    "* FROM RoshitaDetails WHERE roshitaid in (SELECT id FROM roshita  WHERE IsSync=1 AND SyncBy='TEST' " +
                    " AND Manager  IN('Lab_Stop','Lab') ))" +
                    "AS m WHERE RowNum > {0} AND RowNum<= {1}";




                #region RoshitaDetails

                double count = double.Parse(GetSqlDataTable("SELECT Count(*) FROM Roshitadetails WHERE roshitaid in (SELECT id FROM Roshita  WHERE IsSync=1 AND SyncBy='TEST'" +
                    " AND Manager IN('Lab_Stop','Lab') )", _connectionSettings.SQlConnection).Rows[0][0].ToString());

                var maxiteration = Math.Ceiling(count / 10000);
                long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(INVT_SEQ),0)   FROM INV_SAL_RAY ORDER BY INV_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
                if (DateTime.Now.Day == 2)
                {
                    sequenc = 1;
                }
                for (int i = 0; i < maxiteration; i = i)
                {
                    List<INV_RayOracle> data = new List<INV_RayOracle>();

                    INV_RayOracle iNV_SALOracle = new INV_RayOracle();


                    //data.AddRange(Inv_Sal_Medicine(i, query));
                    //data.AddRange(Inv_Sal_Lab(i, queryLab));
                    //data.AddRange(Inv_Sal_Ray(i, queryRay));


                    List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();
                    var data2 = GetSqlDataTable(string.Format(query, (i * 10000), ((++i) * 10000)), _connectionSettings.SQlConnection);

                    //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                    try
                    {

                        roshitaDetail = data2.AsEnumerable().Select(row => new RoshitaDetail
                        {
                            Id = row.Field<long>("Id"),
                            MedicienCode = row.IsNull("MedicienCode") ? "" : row.Field<string>("MedicienCode"),
                            MedicienName = row.IsNull("MedicienName") ? "" : row.Field<string>("MedicienName"),
                            Dose = row.IsNull("Dose") ? 0 : row.Field<int>("Dose"),
                            Duration = row.IsNull("Duration") ? 0 : row.Field<int>("Duration"),
                            TotalDuration = row.IsNull("TotalDuration") ? 0 : row.Field<int>("TotalDuration"),
                            TotalUnits = row.IsNull("TotalUnits") ? 0 : row.Field<int>("TotalUnits"),
                            Amount = row.IsNull("Amount") ? 0 : row.Field<double>("Amount"),
                            RoshitaID = row.IsNull("RoshitaID") ? 0 : row.Field<long>("RoshitaID"),
                            PaymentGroup = row.IsNull("PaymentGroup") ? null : row.Field<string>("PaymentGroup")
                        }).ToList();
                    }

                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        var itemexception = new RoshitaDetail();
                        foreach (var item in roshitaDetail)
                        {
                            try
                            {
                                string createdBy = "";

                                iNV_SALOracle = FillOracleRayDetailObject(item);

                                iNV_SALOracle.INVT_SEQ = sequenc;
                                string QueryRoshita = "SELECT CardId,CreatedDate,Oracle_Id,CreatedBy,Manager  FROM Roshita WHERE Id= " + item.RoshitaID;
                                var RoshitaData = GetSqlDataTable(QueryRoshita, _connectionSettings.SQlConnection);
                                string manager = null;
                                if (RoshitaData.Rows.Count > 0)
                                {
                                    DateTime da = DateTime.Parse(RoshitaData.Rows[0][1].ToString());
                                    iNV_SALOracle.CARD_ID = RoshitaData.Rows[0][0].ToString();
                                    iNV_SALOracle.INV_DATE = new DateTime(da.Year, da.Month, da.Day);
                                    iNV_SALOracle.DATE_CHANGE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.DATE_DURATION = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.REG_DATE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.INV_ID = long.Parse(RoshitaData.Rows[0][2].ToString());
                                    createdBy = RoshitaData.Rows[0][3].ToString();
                                    manager = RoshitaData.Rows[0][4].ToString();
                                }


                                switch (item.PaymentGroup)
                                {
                                    case "Approved":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.APP_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    case "Pending":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Accepted":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "approved";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Rejected":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "Rejected";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CANCEL_ITEM = "3";
                                        break;
                                    case "Cash":
                                        iNV_SALOracle.CASH_ITEM = "Cash";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CASH_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    default:
                                        iNV_SALOracle.CASH_ITEM = "YES";
                                        iNV_SALOracle.GRUOP_TYPE = "YES";
                                        iNV_SALOracle.YES_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                }

                                if (createdBy != null)
                                {
                                    string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                                    " WHERE UserName='" + createdBy + "';";
                                    var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    if (UserId.Rows.Count > 0)
                                    {
                                        var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                        string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                                       " WHERE PR_CODE=" + providerId + ";";
                                        var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                        if (UName.Rows.Count > 0)
                                        {
                                            iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_NAME = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                            iNV_SALOracle.NAME_PHARM = createdBy;
                                            iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                            iNV_SALOracle.NAME_EX = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : null;
                                        }
                                        else
                                        {
                                            string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                                  " WHERE UserCode='" + providerId + "';";
                                            var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                            if (MapName.Rows.Count > 0)
                                            {
                                                iNV_SALOracle.ID_PHARM = providerId;
                                                iNV_SALOracle.PH_ID = providerId;
                                                iNV_SALOracle.PH_NAME = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                                iNV_SALOracle.NAME_PHARM = createdBy;
                                                iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                                iNV_SALOracle.NAME_EX = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                            }
                                        }
                                    }
                                    //string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                    //                   " WHERE UserName='" + createdBy + "';";
                                    //var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    //if (UserId.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = createdBy;
                                    //    iNV_SALOracle.NAME_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? UserId.Rows[0][0].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;
                                    //}

                                    //var user = GetOracleDataTable("select USER_ID ,USER_CO,USER_N,USER_NAME from USERS_2 where USER_NAME='" + createdBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                                    //if (user.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = user.Rows[0][0].ToString() != string.Empty ? double.Parse(user.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : null;
                                    //    iNV_SALOracle.NAME_PHARM = user.Rows[0][3].ToString() != string.Empty ? user.Rows[0][3].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;

                                    //}
                                }

                                var group = GetSqlDataTable("SELECT s.GRUOP_ID FROM Serv_Lab s " +
                                            " Join AspNetUsers u on u.Provider = s.LAB_CODE " +
                                        " WHERE u.UserName = '" + createdBy + "' AND s.SERV_CODE ="
                                        + item.MedicienCode, _connectionSettings.SQlConnection);
                                if (group.Rows.Count > 0)
                                {
                                    iNV_SALOracle.MED_GROUP = group.Rows[0][0].ToString() != string.Empty ? long.Parse(group.Rows[0][0].ToString()) : 0;
                                }

                                var entData = GetOracleDataTable(@"SELECT PERCENT_MONY,CLASS_CODE " +
                                                    " FROM DMS_02_EMP_D_ENT WHERE D_ID=" + iNV_SALOracle.INV_ID, _connectionSettings.OrcaleConnectionTRN_SQL);
                                if (entData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.P_CENT = entData.Rows[0][0].ToString() != string.Empty ? int.Parse(entData.Rows[0][0].ToString()) : 0;
                                    iNV_SALOracle.CLASS_CODE = entData.Rows[0][1].ToString() != string.Empty ? entData.Rows[0][1].ToString() : null;
                                }

                                sequenc++;
                                data.Add(iNV_SALOracle);
                            }
                            catch (Exception ex)
                            {
                                int w = data.Count();
                                itemexception = item;
                                string sex = ex.Message;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        string sex = ex.Message;
                    }

                    try
                    {
                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(data, "INV_SAL_LAB", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL_LAB3",
                            AffectedRows = data.Count
                        });

                        if (data.Count == 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TEST'  AND Oracle_Id = '" + data[0].INV_ID + "' )";


                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TEST' AND Oracle_Id IN= '" + data[0].INV_ID + "'";

                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);

                        }
                        else if (data.Count > 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TEST'  AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaDetailsQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaDetailsQuery += data[data.Count - 1].INV_ID + "'))";
                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TEST' AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaQuery += data[data.Count - 1].INV_ID + "')";
                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL",
                            Exception = ex,
                        });
                    }


                }


                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException;
            }

        }


        private static void UpdatePushRayDetails()
        {
            try
            {
                var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY (select null)) AS RowNum, " +
                    "* FROM RoshitaDetails WHERE roshitaid in (SELECT id FROM roshita  WHERE IsSync=1  AND SyncBy='TESTUPDATE' " +
                    " AND Manager  IN('Ray_Stop','Ray') ))" +
                    "AS m WHERE RowNum > {0} AND RowNum<= {1}";

                #region RoshitaDetails

                double count = double.Parse(GetSqlDataTable("SELECT Count(*) FROM Roshitadetails WHERE roshitaid in (SELECT id FROM Roshita  WHERE IsSync=1 AND SyncBy='TESTUPDATE' " +
                    " AND Manager IN('Ray_Stop','Ray') )", _connectionSettings.SQlConnection).Rows[0][0].ToString());

                var maxiteration = Math.Ceiling(count / 1000);
                long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(INVT_SEQ),0)   FROM INV_SAL_RAY ORDER BY INV_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
                if (DateTime.Now.Day == 2)
                {
                    sequenc = 1;
                }
                for (int i = 0; i < maxiteration; i = i)
                {
                    List<INV_RayOracle> data = new List<INV_RayOracle>();

                    INV_RayOracle iNV_SALOracle = new INV_RayOracle();

                    List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();
                    var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);
                    try
                    {

                        roshitaDetail = data2.AsEnumerable().Select(row => new RoshitaDetail
                        {
                            Id = row.Field<long>("Id"),
                            MedicienCode = row.IsNull("MedicienCode") ? "" : row.Field<string>("MedicienCode"),
                            MedicienName = row.IsNull("MedicienName") ? "" : row.Field<string>("MedicienName"),
                            Dose = row.IsNull("Dose") ? 0 : row.Field<int>("Dose"),
                            Duration = row.IsNull("Duration") ? 0 : row.Field<int>("Duration"),
                            TotalDuration = row.IsNull("TotalDuration") ? 0 : row.Field<int>("TotalDuration"),
                            TotalUnits = row.IsNull("TotalUnits") ? 0 : row.Field<int>("TotalUnits"),
                            Amount = row.IsNull("Amount") ? 0 : row.Field<double>("Amount"),
                            RoshitaID = row.IsNull("RoshitaID") ? 0 : row.Field<long>("RoshitaID"),
                            PaymentGroup = row.IsNull("PaymentGroup") ? null : row.Field<string>("PaymentGroup")
                        }).ToList();
                    }

                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        var itemexception = new RoshitaDetail();
                        foreach (var item in roshitaDetail)
                        {
                            try
                            {
                                string createdBy = "";

                                iNV_SALOracle = FillOracleRayDetailObject(item);

                                iNV_SALOracle.INVT_SEQ = sequenc;
                                string QueryRoshita = "SELECT CardId,CreatedDate,Oracle_Id,CreatedBy,Manager  FROM Roshita WHERE Id= " + item.RoshitaID;
                                var RoshitaData = GetSqlDataTable(QueryRoshita, _connectionSettings.SQlConnection);
                                string manager = null;
                                if (RoshitaData.Rows.Count > 0)
                                {
                                    DateTime da = DateTime.Parse(RoshitaData.Rows[0][1].ToString());
                                    iNV_SALOracle.CARD_ID = RoshitaData.Rows[0][0].ToString();
                                    iNV_SALOracle.INV_DATE = new DateTime(da.Year, da.Month, da.Day);
                                    iNV_SALOracle.DATE_CHANGE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.DATE_DURATION = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.REG_DATE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.INV_ID = long.Parse(RoshitaData.Rows[0][2].ToString());
                                    createdBy = RoshitaData.Rows[0][3].ToString();
                                    manager = RoshitaData.Rows[0][4].ToString();
                                }


                                switch (item.PaymentGroup)
                                {
                                    case "Approved":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.APP_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    case "Pending":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Accepted":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "approved";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Rejected":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "Rejected";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CANCEL_ITEM = "3";
                                        break;
                                    case "Cash":
                                        iNV_SALOracle.CASH_ITEM = "Cash";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CASH_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    default:
                                        iNV_SALOracle.CASH_ITEM = "YES";
                                        iNV_SALOracle.GRUOP_TYPE = "YES";
                                        iNV_SALOracle.YES_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                }

                                if (createdBy != null)
                                {
                                    string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                                    " WHERE UserName='" + createdBy + "';";
                                    var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    if (UserId.Rows.Count > 0)
                                    {
                                        var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                        string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                                       " WHERE PR_CODE=" + providerId + ";";
                                        var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                        if (UName.Rows.Count > 0)
                                        {
                                            iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_NAME = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                            iNV_SALOracle.NAME_PHARM = createdBy;
                                            iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                            iNV_SALOracle.NAME_EX = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : null;
                                        }
                                        else
                                        {
                                            string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                                  " WHERE UserCode='" + providerId + "';";
                                            var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                            if (MapName.Rows.Count > 0)
                                            {
                                                iNV_SALOracle.ID_PHARM = providerId;
                                                iNV_SALOracle.PH_ID = providerId;
                                                iNV_SALOracle.PH_NAME = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                                iNV_SALOracle.NAME_PHARM = createdBy;
                                                iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                                iNV_SALOracle.NAME_EX = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                            }
                                        }
                                    }
                                    //string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                    //                   " WHERE UserName='" + createdBy + "';";
                                    //var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    //if (UserId.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = createdBy;
                                    //    iNV_SALOracle.NAME_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? UserId.Rows[0][0].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;
                                    //}



                                    //var user = GetOracleDataTable("select USER_ID ,USER_CO,USER_N,USER_NAME from USERS_2 where USER_NAME='" + createdBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                                    //if (user.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = user.Rows[0][0].ToString() != string.Empty ? double.Parse(user.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : null;
                                    //    iNV_SALOracle.NAME_PHARM = user.Rows[0][3].ToString() != string.Empty ? user.Rows[0][3].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;

                                    //}
                                }

                                var group = GetSqlDataTable("SELECT s.GRUOP_ID FROM Serv_Ray s " +
                                            " Join AspNetUsers u on u.Provider = s.LAB_CODE " +
                                        " WHERE u.UserName = '" + createdBy + "' AND s.SERV_CODE ="
                                        + item.MedicienCode, _connectionSettings.SQlConnection);
                                if (group.Rows.Count > 0)
                                {
                                    iNV_SALOracle.MED_GROUP = group.Rows[0][0].ToString() != string.Empty ? long.Parse(group.Rows[0][0].ToString()) : 0;
                                }

                                var entData = GetOracleDataTable(@"SELECT PERCENT_MONY,CLASS_CODE " +
                                                    " FROM DMS_02_EMP_D_ENT3 WHERE D_ID=" + iNV_SALOracle.INV_ID, _connectionSettings.OrcaleConnectionSH65);
                                if (entData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.P_CENT = entData.Rows[0][0].ToString() != string.Empty ? int.Parse(entData.Rows[0][0].ToString()) : 0;
                                    iNV_SALOracle.CLASS_CODE = entData.Rows[0][1].ToString() != string.Empty ? entData.Rows[0][1].ToString() : null;
                                }
                                sequenc++;
                                iNV_SALOracle.IS_SYNC = 9;
                                iNV_SALOracle.SYNC_BY = "UPDATE";
                                iNV_SALOracle.SYNC_DATE = DateTime.Now;
                                data.Add(iNV_SALOracle);
                            }
                            catch (Exception ex)
                            {
                                int w = data.Count();
                                itemexception = item;
                                string sex = ex.Message;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        string sex = ex.Message;
                    }

                    try
                    {
                        string ORAQuery2 = "DELETE INV_SAL_RAY" +
                            " WHERE INV_ID IN( '" + data[0].INV_ID + "','";

                        for (int j = 1; j < data.Count - 1; j++)
                        {
                            ORAQuery2 += data[j].INV_ID + "','";
                        }
                        ORAQuery2 += data[data.Count - 1].INV_ID + "')";
                        ExecuteOracleQuery(ORAQuery2, _connectionSettings.OrcaleConnectionTRN_SQL);


                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(data, "INV_SAL_RAY", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "Update INV_SAL_RAY3",
                            AffectedRows = data.Count
                        });

                        if (data.Count == 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TESTUPDATE'  AND Oracle_Id = '" + data[0].INV_ID + "' )";


                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TESTUPDATE' AND Oracle_Id = '" + data[0].INV_ID + "'";

                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);

                        }
                        else if (data.Count > 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TESTUPDATE'  AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaDetailsQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaDetailsQuery += data[data.Count - 1].INV_ID + "'))";
                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TESTUPDATE' AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaQuery += data[data.Count - 1].INV_ID + "')";
                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL_RAY3",
                            Exception = ex,
                        });
                    }


                }


                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException;
            }

        }

        private static void UpdatePushLabDetails()
        {
            try
            {
                var query = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY (select null)) AS RowNum, " +
                    "* FROM RoshitaDetails WHERE roshitaid in (SELECT id FROM roshita  WHERE IsSync=1 AND SyncBy='TESTUPDATE' " +
                    " AND Manager  IN('Lab_Stop','Lab') ))" +
                    "AS m WHERE RowNum > {0} AND RowNum<= {1}";




                #region RoshitaDetails

                double count = double.Parse(GetSqlDataTable("SELECT Count(*) FROM Roshitadetails WHERE roshitaid in (SELECT id FROM Roshita  WHERE IsSync=1 AND SyncBy='TESTUPDATE' " +
                    " AND Manager IN('Lab_Stop','Lab') )", _connectionSettings.SQlConnection).Rows[0][0].ToString());

                var maxiteration = Math.Ceiling(count / 1000);
                long sequenc = long.Parse(GetOracleDataTable(@"SELECT NVL(MAX(INVT_SEQ),0)   FROM INV_SAL_RAY ORDER BY INV_DATE DESC ", _connectionSettings.OrcaleConnectionTRN_SQL).Rows[0][0].ToString()) + 1;
                if (DateTime.Now.Day == 2)
                {
                    sequenc = 1;
                }
                for (int i = 0; i < maxiteration; i = i)
                {
                    List<INV_RayOracle> data = new List<INV_RayOracle>();

                    INV_RayOracle iNV_SALOracle = new INV_RayOracle();

                    List<RoshitaDetail> roshitaDetail = new List<RoshitaDetail>();
                    var data2 = GetSqlDataTable(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.SQlConnection);

                    //var data2 = GetSqlDataTable(string.Format(query), _connectionSettings.SQlConnection);
                    try
                    {

                        roshitaDetail = data2.AsEnumerable().Select(row => new RoshitaDetail
                        {
                            Id = row.Field<long>("Id"),
                            MedicienCode = row.IsNull("MedicienCode") ? "" : row.Field<string>("MedicienCode"),
                            MedicienName = row.IsNull("MedicienName") ? "" : row.Field<string>("MedicienName"),
                            Dose = row.IsNull("Dose") ? 0 : row.Field<int>("Dose"),
                            Duration = row.IsNull("Duration") ? 0 : row.Field<int>("Duration"),
                            TotalDuration = row.IsNull("TotalDuration") ? 0 : row.Field<int>("TotalDuration"),
                            TotalUnits = row.IsNull("TotalUnits") ? 0 : row.Field<int>("TotalUnits"),
                            Amount = row.IsNull("Amount") ? 0 : row.Field<double>("Amount"),
                            RoshitaID = row.IsNull("RoshitaID") ? 0 : row.Field<long>("RoshitaID"),
                            PaymentGroup = row.IsNull("PaymentGroup") ? null : row.Field<string>("PaymentGroup")
                        }).ToList();
                    }

                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        var itemexception = new RoshitaDetail();
                        foreach (var item in roshitaDetail)
                        {
                            try
                            {
                                string createdBy = "";

                                iNV_SALOracle = FillOracleRayDetailObject(item);

                                iNV_SALOracle.INVT_SEQ = sequenc;
                                string QueryRoshita = "SELECT CardId,CreatedDate,Oracle_Id,CreatedBy,Manager  FROM Roshita WHERE Id= " + item.RoshitaID;
                                var RoshitaData = GetSqlDataTable(QueryRoshita, _connectionSettings.SQlConnection);
                                string manager = null;
                                if (RoshitaData.Rows.Count > 0)
                                {
                                    DateTime da = DateTime.Parse(RoshitaData.Rows[0][1].ToString());
                                    iNV_SALOracle.CARD_ID = RoshitaData.Rows[0][0].ToString();
                                    iNV_SALOracle.INV_DATE = new DateTime(da.Year, da.Month, da.Day);
                                    iNV_SALOracle.DATE_CHANGE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.DATE_DURATION = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.REG_DATE = iNV_SALOracle.INV_DATE;
                                    iNV_SALOracle.INV_ID = long.Parse(RoshitaData.Rows[0][2].ToString());
                                    createdBy = RoshitaData.Rows[0][3].ToString();
                                    manager = RoshitaData.Rows[0][4].ToString();
                                }


                                switch (item.PaymentGroup)
                                {
                                    case "Approved":
                                        iNV_SALOracle.CASH_ITEM = "APPROV";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.APP_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    case "Pending":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Accepted":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "approved";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        break;
                                    case "Rejected":
                                        iNV_SALOracle.CASH_ITEM = "Pending";
                                        iNV_SALOracle.COVERED_ITEM = "Rejected";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CANCEL_ITEM = "3";
                                        break;
                                    case "Cash":
                                        iNV_SALOracle.CASH_ITEM = "Cash";
                                        iNV_SALOracle.GRUOP_TYPE = "NO";
                                        iNV_SALOracle.CASH_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                    default:
                                        iNV_SALOracle.CASH_ITEM = "YES";
                                        iNV_SALOracle.GRUOP_TYPE = "YES";
                                        iNV_SALOracle.YES_TOT = iNV_SALOracle.AMOUNT;
                                        break;
                                }

                                if (createdBy != null)
                                {
                                    string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                                    " WHERE UserName='" + createdBy + "';";
                                    var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    if (UserId.Rows.Count > 0)
                                    {
                                        var providerId = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;

                                        string QueryServProvider = "SELECT PR_ANAME FROM Serv_Providers1 " +
                                       " WHERE PR_CODE=" + providerId + ";";
                                        var UName = GetSqlDataTable(QueryServProvider, _connectionSettings.SQlConnection);
                                        if (UName.Rows.Count > 0)
                                        {
                                            iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                            iNV_SALOracle.PH_NAME = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : "";
                                            iNV_SALOracle.NAME_PHARM = createdBy;
                                            iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                            iNV_SALOracle.NAME_EX = UName.Rows[0][0].ToString() != string.Empty ? UName.Rows[0][0].ToString() : null;
                                        }
                                        else
                                        {
                                            string QueryMapping = "SELECT UserName FROM MappingTable " +
                                                                  " WHERE UserCode='" + providerId + "';";
                                            var MapName = GetSqlDataTable(QueryMapping, _connectionSettings.SQlConnection);
                                            if (MapName.Rows.Count > 0)
                                            {
                                                iNV_SALOracle.ID_PHARM = providerId;
                                                iNV_SALOracle.PH_ID = providerId;
                                                iNV_SALOracle.PH_NAME = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                                iNV_SALOracle.NAME_PHARM = createdBy;
                                                iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                                iNV_SALOracle.NAME_EX = MapName.Rows[0][0].ToString() != string.Empty ? MapName.Rows[0][0].ToString() : null;
                                            }
                                        }
                                    }
                                    //string QueryUser = "SELECT Provider FROM AspNetUsers " +
                                    //                   " WHERE UserName='" + createdBy + "';";
                                    //var UserId = GetSqlDataTable(QueryUser, _connectionSettings.SQlConnection);
                                    //if (UserId.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? double.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = UserId.Rows[0][0].ToString() != string.Empty ? long.Parse(UserId.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = createdBy;
                                    //    iNV_SALOracle.NAME_PHARM = UserId.Rows[0][0].ToString() != string.Empty ? UserId.Rows[0][0].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;
                                    //}

                                    //var user = GetOracleDataTable("select USER_ID ,USER_CO,USER_N,USER_NAME from USERS_2 where USER_NAME='" + createdBy.ToUpper() + "' ", _connectionSettings.OrcaleConnectionSH65);
                                    //if (user.Rows.Count > 0)
                                    //{
                                    //    iNV_SALOracle.ID_PHARM = user.Rows[0][0].ToString() != string.Empty ? double.Parse(user.Rows[0][0].ToString()) : 0;
                                    //    iNV_SALOracle.PH_ID = user.Rows[0][1].ToString() != string.Empty ? long.Parse(user.Rows[0][1].ToString()) : 0;
                                    //    iNV_SALOracle.PH_NAME = user.Rows[0][2].ToString() != string.Empty ? user.Rows[0][2].ToString() : null;
                                    //    iNV_SALOracle.NAME_PHARM = user.Rows[0][3].ToString() != string.Empty ? user.Rows[0][3].ToString() : null;
                                    //    iNV_SALOracle.ID_EX = iNV_SALOracle.ID_PHARM.ToString();
                                    //    iNV_SALOracle.NAME_EX = iNV_SALOracle.NAME_PHARM;

                                    //}
                                }

                                var group = GetSqlDataTable("SELECT s.GRUOP_ID FROM Serv_Lab s " +
                                            " Join AspNetUsers u on u.Provider = s.LAB_CODE " +
                                        " WHERE u.UserName = '" + createdBy + "' AND s.SERV_CODE ="
                                        + item.MedicienCode, _connectionSettings.SQlConnection);
                                if (group.Rows.Count > 0)
                                {
                                    iNV_SALOracle.MED_GROUP = group.Rows[0][0].ToString() != string.Empty ? long.Parse(group.Rows[0][0].ToString()) : 0;
                                }

                                var entData = GetOracleDataTable(@"SELECT PERCENT_MONY,CLASS_CODE " +
                                                    " FROM DMS_02_EMP_D_ENT3 WHERE D_ID=" + iNV_SALOracle.INV_ID, _connectionSettings.OrcaleConnectionSH65);
                                if (entData.Rows.Count > 0)
                                {
                                    iNV_SALOracle.P_CENT = entData.Rows[0][0].ToString() != string.Empty ? int.Parse(entData.Rows[0][0].ToString()) : 0;
                                    iNV_SALOracle.CLASS_CODE = entData.Rows[0][1].ToString() != string.Empty ? entData.Rows[0][1].ToString() : null;
                                }

                                sequenc++;
                                iNV_SALOracle.IS_SYNC = 9;
                                iNV_SALOracle.SYNC_BY = "UPDATE";
                                iNV_SALOracle.SYNC_DATE = DateTime.Now;
                                data.Add(iNV_SALOracle);
                            }
                            catch (Exception ex)
                            {
                                int w = data.Count();
                                itemexception = item;
                                string sex = ex.Message;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        string sex = ex.Message;
                    }

                    try
                    {
                        string ORAQuery2 = "DELETE INV_SAL_LAB" +
                           " WHERE INV_ID IN( '" + data[0].INV_ID + "','";

                        for (int j = 1; j < data.Count - 1; j++)
                        {
                            ORAQuery2 += data[j].INV_ID + "','";
                        }
                        ORAQuery2 += data[data.Count - 1].INV_ID + "')";
                        ExecuteOracleQuery(ORAQuery2, _connectionSettings.OrcaleConnectionTRN_SQL);


                        //add new data AddNewEntitiesSql2
                        AddNewEntitiesSql2(data, "INV_SAL_LAB", false, _connectionSettings.OrcaleConnectionTRN_SQL);
                        //AddNewEntitiesSql(roshitas, "DMS_02_EMP_D_ENT2", false, _connectionSettings.OrcaleConnectionSH65);
                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "Update INV_SAL_LAB3",
                            AffectedRows = data.Count
                        });

                        if (data.Count == 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TESTUPDATE'  AND Oracle_Id = '" + data[0].INV_ID + "' )";


                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TESTUPDATE' AND Oracle_Id = '" + data[0].INV_ID + "'";

                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);

                        }
                        else if (data.Count > 1)
                        {
                            string UpdateRoshitaDetailsQuery = " UPDATE RoshitaDetails SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                     " WHERE roshitaid in (SELECT id FROM roshita " +
                                     " WHERE IsSync=1 AND SyncBy='TESTUPDATE'  AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaDetailsQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaDetailsQuery += data[data.Count - 1].INV_ID + "'))";
                            var resultmessage2 = ExecuteNonQueryCommand(UpdateRoshitaDetailsQuery, _connectionSettings.SQlConnection);


                            string UpdateRoshitaQuery = "UPDATE Roshita SET IsSync=1,SyncBy='SQL',SyncDate=GETDATE() " +
                                " WHERE IsSync=1 AND SyncBy='TESTUPDATE' AND Oracle_Id IN( '" + data[0].INV_ID + "','";

                            for (int j = 1; j < data.Count - 1; j++)
                            {
                                UpdateRoshitaQuery += data[j].INV_ID + "','";
                            }
                            UpdateRoshitaQuery += data[data.Count - 1].INV_ID + "')";
                            var resultmessage = ExecuteNonQueryCommand(UpdateRoshitaQuery, _connectionSettings.SQlConnection);
                        }

                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                            Server = GetServerName(_connectionSettings.OrcaleConnection),
                            Table = "INV_SAL_LAB3",
                            Exception = ex,
                        });
                    }


                }


                #endregion
            }
            catch (Exception ex)
            {
                var message = ex.InnerException;
            }

        }

        private static void CLOSE_EMP_DATASyncToSqlTable()
        {
            var query = "select * from (select m.*, rownum r from  DMS_TEST.CLOSE_EMP_DATA m WHERE (IS_SYNC=0 OR IS_SYNC IS NULL) AND TRANS_TYP='L') WHERE r > {0} and r<= {1} ";

            double count = double.Parse(GetOracleDataTable("select  COUNT(*) from DMS_TEST.CLOSE_EMP_DATA m WHERE (IS_SYNC=0 OR IS_SYNC IS NULL) AND TRANS_TYP='L' ", _connectionSettings.OrcaleConnection).Rows[0][0].ToString());

            //double count = GetCount(tableName, _connectionSettings.OrcaleConnection);
            var maxiteration = Math.Ceiling(count / 1000);
            var tableName = "CLOSE_EMP_DATA";
            List<Med_Card> MedCard = new List<Med_Card>();
            List<Med_Medicine> medMedicines = new List<Med_Medicine>();
            List<Roshita> roshitas = new List<Roshita>();
            List<RoshitaDetail> roshitasDetail = new List<RoshitaDetail>();
            for (int i = 0; i < maxiteration; i = i)
            {
                try
                {
                    var data = GetOracleTable<CLOSE_EMP_DATA>(string.Format(query, (i * 1000), ((++i) * 1000)), _connectionSettings.OrcaleConnection);
                    _result.Logs.Add(new ViewModels.Log
                    {
                        Order = GetLogOrder(),
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = tableName,
                        Note = "All",
                        AffectedRows = data.Count
                    });

                    try
                    {
                        foreach (var item in data)
                        {
                            string Querymedcard = "SELECT * FROM Med_Card " +
                                                    " WHERE CARD_NO='" + item.CARD_ID + "';";
                            var cardmed = GetSqlDataTable(Querymedcard, _connectionSettings.SQlConnection);
                            if (cardmed.Rows.Count > 0)
                            {
                                MedCard.Add(new Med_Card
                                {
                                    CARD_NO = item.N_CARD,
                                    PROVIDER_CODE = cardmed.Rows[0][2].ToString() != string.Empty ? long.Parse(cardmed.Rows[0][2].ToString()) : 0,
                                    C_COMP_ID = cardmed.Rows[0][3].ToString() != string.Empty ? long.Parse(cardmed.Rows[0][3].ToString()) : 0,
                                    NOTES = cardmed.Rows[0][4].ToString() != string.Empty ? cardmed.Rows[0][4].ToString() : null,
                                    CREATED_BY = cardmed.Rows[0][5].ToString() != string.Empty ? cardmed.Rows[0][5].ToString() : null,
                                    CREATED_DATE = cardmed.Rows[0][6].ToString() != string.Empty ? DateTime.Parse(cardmed.Rows[0][6].ToString()) : (DateTime?)null,
                                    UPDATE_BY = cardmed.Rows[0][7].ToString() != string.Empty ? cardmed.Rows[0][7].ToString() : null,
                                    UPDATE_DATE = cardmed.Rows[0][8].ToString() != string.Empty ? DateTime.Parse(cardmed.Rows[0][8].ToString()) : (DateTime?)null,
                                    MONTH_START_DATE = cardmed.Rows[0][9].ToString() != string.Empty ? DateTime.Parse(cardmed.Rows[0][9].ToString()) : (DateTime?)null,
                                    MONTH_END_DATE = cardmed.Rows[0][10].ToString() != string.Empty ? DateTime.Parse(cardmed.Rows[0][10].ToString()) : (DateTime?)null,
                                    GROUP_ID = cardmed.Rows[0][11].ToString() != string.Empty ? long.Parse(cardmed.Rows[0][11].ToString()) : 0,
                                    GROUP_NAME = cardmed.Rows[0][12].ToString() != string.Empty ? cardmed.Rows[0][12].ToString() : null,
                                    LOOK_01 = cardmed.Rows[0][13].ToString() != string.Empty ? Int16.Parse(cardmed.Rows[0][13].ToString()) : Int16.Parse(null),
                                    SEQ = cardmed.Rows[0][14].ToString() != string.Empty ? decimal.Parse(cardmed.Rows[0][14].ToString()) : 0,
                                    PROVIDER_CODE_OLD = cardmed.Rows[0][15].ToString() != string.Empty ? long.Parse(cardmed.Rows[0][15].ToString()) : 0,
                                    TASHKHES_01 = cardmed.Rows[0][16].ToString() != string.Empty ? cardmed.Rows[0][16].ToString() : null,
                                    NO_PAY = cardmed.Rows[0][17].ToString() != string.Empty ? byte.Parse(cardmed.Rows[0][17].ToString()) : byte.Parse(null),
                                    NO_OVER = cardmed.Rows[0][18].ToString() != string.Empty ? byte.Parse(cardmed.Rows[0][18].ToString()) : byte.Parse(null),
                                    ST_DAY = cardmed.Rows[0][19].ToString() != string.Empty ? Int16.Parse(cardmed.Rows[0][19].ToString()) : Int16.Parse(null),
                                    PhoneNumber = cardmed.Rows[0][20].ToString() != string.Empty ? cardmed.Rows[0][20].ToString() : null,
                                    NationalId = cardmed.Rows[0][21].ToString() != string.Empty ? cardmed.Rows[0][21].ToString() : null,
                                    IsSync = false,
                                    SyncDate = DateTime.Now,
                                    SyncBy = "Aya",
                                    ExceptionType = cardmed.Rows[0][25].ToString() != string.Empty ? cardmed.Rows[0][25].ToString() : null,

                                });
                                roshitas.Add(new Roshita
                                {
                                    Oracle_Id = 0,
                                    CardId = item.N_CARD,
                                    Speciality = "Empty",
                                    Diagnose1 = cardmed.Rows[0][16].ToString() != string.Empty ? cardmed.Rows[0][16].ToString() : null,
                                    RoshetaType = "11602",
                                    Limit = 0,
                                    CompanyPayment = 0,
                                    OverInsurance = 0,
                                    TotalValue = 0,
                                    CompanyPercent = 0,
                                    PersonPayment = 0,
                                    Cash = 0,
                                    Manager = "Doctor_Chronic",
                                    CreatedBy = cardmed.Rows[0][5].ToString() != string.Empty ? cardmed.Rows[0][5].ToString() : "Admin",
                                    CreatedDate = cardmed.Rows[0][6].ToString() != string.Empty ? DateTime.Parse(cardmed.Rows[0][6].ToString()) : (DateTime?)null,
                                    IsSync = false,
                                    SyncBy = "Aya",
                                    SyncDate = DateTime.Now,
                                    Diagnose2 = "Empty",
                                    diagnose3 = "Empty",
                                    PhoneNumber = "Empty",

                                });
                            }

                            string Querymedmedicine = "SELECT * FROM Med_Medicine " +
                                                    " WHERE CARD_NO='" + item.CARD_ID + "';";
                            var cardmedicne = GetSqlDataTable(Querymedmedicine, _connectionSettings.SQlConnection);
                            if (cardmedicne.Rows.Count > 0)
                            {
                                for (int k = 0; k < cardmedicne.Rows.Count; k++)
                                {
                                    medMedicines.Add(new Med_Medicine
                                    {
                                        CARD_NO = item.N_CARD,
                                        MED_CODE = cardmedicne.Rows[k][1].ToString() != string.Empty ? cardmedicne.Rows[k][1].ToString() : null,
                                        RDATE = cardmedicne.Rows[k][3].ToString() != string.Empty ? DateTime.Parse(cardmedicne.Rows[k][3].ToString()) : (DateTime?)null,
                                        MED_TYP = cardmedicne.Rows[k][4].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][4].ToString()) : (decimal?)null,
                                        DOSE = cardmedicne.Rows[k][5].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][5].ToString()) : (decimal?)null,
                                        NO_OF_UINT = cardmedicne.Rows[k][6].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][6].ToString()) : (decimal?)null,
                                        TOTAL_AMT = cardmedicne.Rows[k][7].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][7].ToString()) : (decimal?)null,
                                        MED_DURATION = cardmedicne.Rows[k][8].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][8].ToString()) : (decimal?)null,
                                        TOT_DUR = cardmedicne.Rows[k][9].ToString() != string.Empty ? cardmedicne.Rows[k][9].ToString() : null,
                                        DOS_DUR = cardmedicne.Rows[k][10].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][10].ToString()) : (decimal?)null,
                                        EXCESS = cardmedicne.Rows[k][11].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][11].ToString()) : (decimal?)null,
                                        PACK_SIZE = cardmedicne.Rows[k][12].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][12].ToString()) : (decimal?)null,
                                        PACK_PRICE = cardmedicne.Rows[k][13].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][13].ToString()) : (decimal?)null,
                                        CON_MED = cardmedicne.Rows[k][14].ToString() != string.Empty ? cardmedicne.Rows[k][14].ToString() : null,
                                        UNIT_NO = cardmedicne.Rows[k][15].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][15].ToString()) : (decimal?)null,
                                        UNIT_PRICE = cardmedicne.Rows[k][16].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][16].ToString()) : (decimal?)null,
                                        MED_NAME = cardmedicne.Rows[k][17].ToString() != string.Empty ? cardmedicne.Rows[k][17].ToString() : null,
                                        DOSAGE_FORM = cardmedicne.Rows[k][18].ToString() != string.Empty ? cardmedicne.Rows[k][18].ToString() : null,
                                        NOTES = cardmedicne.Rows[k][19].ToString() != string.Empty ? cardmedicne.Rows[k][19].ToString() : null,
                                        ACTIVE = cardmedicne.Rows[k][20].ToString() != string.Empty ? cardmedicne.Rows[k][20].ToString() : null,
                                        CREATED_BY = cardmedicne.Rows[k][21].ToString() != string.Empty ? cardmedicne.Rows[k][21].ToString() : null,
                                        CREATED_DATE = cardmedicne.Rows[k][22].ToString() != string.Empty ? DateTime.Parse(cardmedicne.Rows[k][22].ToString()) : (DateTime?)null,
                                        UPDATE_BY = cardmedicne.Rows[k][23].ToString() != string.Empty ? cardmedicne.Rows[k][23].ToString() : null,
                                        UPDATE_DATE = cardmedicne.Rows[k][24].ToString() != string.Empty ? DateTime.Parse(cardmedicne.Rows[k][24].ToString()) : (DateTime?)null,
                                        ACT_MONTH = cardmedicne.Rows[k][25].ToString() != string.Empty ? cardmedicne.Rows[k][25].ToString() : null,
                                        LFT_MONTH = cardmedicne.Rows[k][26].ToString() != string.Empty ? decimal.Parse(cardmedicne.Rows[k][26].ToString()) : (decimal?)null,
                                        MONTH_DATE_STOP = cardmedicne.Rows[k][27].ToString() != string.Empty ? DateTime.Parse(cardmedicne.Rows[k][27].ToString()) : (DateTime?)null,
                                        IsSync = false,
                                        SyncDate = DateTime.Now,
                                        SyncBy = "Aya",

                                    });
                                }

                            }
                        }
                        AddNewEntities(MedCard, "Med_Card", false, _connectionSettings.SQlConnection);
                        AddNewEntities(medMedicines, "Med_Medicine", false, _connectionSettings.SQlConnection);
                        AddNewEntities(roshitas, "Roshita", false, _connectionSettings.SQlConnection);

                        foreach (var item in data)
                        {
                            string QueryRoshitaDetails = "SELECT * FROM RoshitaDetails " +
                                                    " WHERE RoshitaID IN( SELECT Id FROM Roshita WHERE CardId=" +
                                                    "'" + item.CARD_ID + "' AND Manager='Doctor_Chronic')";
                            long roshitaId;
                            var rosh = GetSqlDataTable("SELECT Id FROM Roshita WHERE CardId='" + item.N_CARD + "' AND Manager = 'Doctor_Chronic'"
                                , _connectionSettings.SQlConnection);
                            if (rosh.Rows.Count > 0)
                            {
                                roshitaId = long.Parse(rosh.Rows[0][0].ToString());

                                var RoshitaDetail = GetSqlDataTable(QueryRoshitaDetails, _connectionSettings.SQlConnection);
                                if (RoshitaDetail.Rows.Count > 0)
                                {
                                    for (int a = 0; a < RoshitaDetail.Rows.Count; a++)
                                    {
                                        roshitasDetail.Add(new RoshitaDetail
                                        {
                                            MedicienCode = RoshitaDetail.Rows[a][1].ToString() != string.Empty ? RoshitaDetail.Rows[a][1].ToString() : null,
                                            MedicienName = RoshitaDetail.Rows[a][2].ToString() != string.Empty ? RoshitaDetail.Rows[a][2].ToString() : null,
                                            Dose = RoshitaDetail.Rows[a][3].ToString() != string.Empty ? int.Parse(RoshitaDetail.Rows[a][3].ToString()) : int.Parse(null),
                                            Duration = RoshitaDetail.Rows[a][4].ToString() != string.Empty ? int.Parse(RoshitaDetail.Rows[a][4].ToString()) : int.Parse(null),
                                            TotalDuration = RoshitaDetail.Rows[a][5].ToString() != string.Empty ? int.Parse(RoshitaDetail.Rows[a][5].ToString()) : int.Parse(null),
                                            TotalUnits = RoshitaDetail.Rows[a][6].ToString() != string.Empty ? int.Parse(RoshitaDetail.Rows[a][6].ToString()) : int.Parse(null),
                                            Amount = RoshitaDetail.Rows[a][7].ToString() != string.Empty ? double.Parse(RoshitaDetail.Rows[a][7].ToString()) : (double?)null,
                                            IsDealed = RoshitaDetail.Rows[a][8].ToString() != string.Empty ? bool.Parse(RoshitaDetail.Rows[a][8].ToString()) : bool.Parse(null),
                                            PaymentGroup = RoshitaDetail.Rows[a][10].ToString() != string.Empty ? RoshitaDetail.Rows[a][10].ToString() : null,
                                            RoshitaID = roshitaId,
                                            IsSync = false,
                                            SyncDate = DateTime.Now,
                                            SyncBy = "Aya",

                                        });
                                    }
                                }
                            }
                        }

                        AddNewEntities(roshitasDetail, "RoshitaDetails", false, _connectionSettings.SQlConnection);

                        _result.Logs.Add(new ViewModels.Log
                        {
                            Order = GetLogOrder(),
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = tableName,
                            AffectedRows = data.Count
                        });


                    }
                    catch (Exception ex)
                    {
                        _result.Errors.Add(new Error()
                        {
                            Action = SyncAction.Insert.ToString(),
                            Database = GetDatabaseName(),
                            Server = GetServerName(),
                            Table = tableName,
                            Exception = ex,
                        });
                    }
                }
                catch (Exception e)
                {
                    _result.Errors.Add(new Error()
                    {
                        Action = SyncAction.Get.ToString(),
                        Database = GetDatabaseName(_connectionSettings.OrcaleConnection),
                        Server = GetServerName(_connectionSettings.OrcaleConnection),
                        Table = tableName,
                        Exception = e,
                    });
                }
            }


            string OracleQuery = "UPDATE DMS_TEST.CLOSE_EMP_DATA SET IS_SYNC = 1,SYNC_DATE=SYSDATE,SYNC_BY = 'Admin' WHERE IS_SYNC=0 OR IS_SYNC IS NULL";
            ExecuteOracleQuery(OracleQuery, _connectionSettings.OrcaleConnection);


        }

    }
}