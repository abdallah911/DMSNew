namespace DMS_Synchronization.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SqlModel : DbContext
    {
        public SqlModel()
            : base("name=SqlModel")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Acception> Acceptions { get; set; }
        public virtual DbSet<AcceptionReason> AcceptionReasons { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Basic_Data> Basic_Data { get; set; }
        public virtual DbSet<C__MigrationHistory1> C__MigrationHistory1 { get; set; }
        public virtual DbSet<CardAcceptionReason> CardAcceptionReasons { get; set; }
        public virtual DbSet<Co_Insurance_01> Co_Insurance_01 { get; set; }
        public virtual DbSet<Comp_Customized_D_D> Comp_Customized_D_D { get; set; }
        //public virtual DbSet<Comp_Customized_D_D_Emp> Comp_Customized_D_D_Emp { get; set; }
        public virtual DbSet<COMP_CUSTOMIZED_D_D_EMP> Comp_Customized_D_D_Emp { get; set; }
        
        public virtual DbSet<Comp_Employees> Comp_Employees { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<Contract_Comp> Contract_Comp { get; set; }
        public virtual DbSet<DailyPermission> DailyPermissions { get; set; }
        public virtual DbSet<Diagnosis> Diagnosis { get; set; }
        public virtual DbSet<Dms_02_Emp_D_Ent> Dms_02_Emp_D_Ent { get; set; }
        public virtual DbSet<DoctorPersonalData> DoctorPersonalDatas { get; set; }
        public virtual DbSet<Doctors_Permissions> Doctors_Permissions { get; set; }
        public virtual DbSet<DoctorWorkPlace> DoctorWorkPlaces { get; set; }
        public virtual DbSet<DosageForm> DosageForms { get; set; }
        public virtual DbSet<Governate> Governates { get; set; }
        public virtual DbSet<LicenseType> LicenseTypes { get; set; }
        public virtual DbSet<Med_Card> Med_Card { get; set; }
        public virtual DbSet<Med_Medicine> Med_Medicine { get; set; }
        public virtual DbSet<MedicineData> MedicineDatas { get; set; }
        public virtual DbSet<MedicineGroup> MedicineGroups { get; set; }
        public virtual DbSet<MedicinesDiagnosi> MedicinesDiagnosis { get; set; }
        public virtual DbSet<MedicineType> MedicineTypes { get; set; }
        public virtual DbSet<MonthlyPermission> MonthlyPermissions { get; set; }
        public virtual DbSet<Name_Lab> Name_Lab { get; set; }
        public virtual DbSet<Name_Lab_Cen> Name_Lab_Cen { get; set; }
        public virtual DbSet<Name_Ray> Name_Ray { get; set; }
        public virtual DbSet<Name_Ray_Cen> Name_Ray_Cen { get; set; }
        public virtual DbSet<Patch> Patchs { get; set; }
        public virtual DbSet<Pr_Bra> Pr_Bra { get; set; }
        public virtual DbSet<PrescriptionRoshitaDignosi> PrescriptionRoshitaDignosis { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Roshita> Roshitas { get; set; }
        public virtual DbSet<RoshitaDetail> RoshitaDetails { get; set; }
        public virtual DbSet<RoshitaPharmcyApproved> RoshitaPharmcyApproveds { get; set; }
        public virtual DbSet<S_Ent_7> S_Ent_7 { get; set; }
        public virtual DbSet<Serv_Lab> Serv_Lab { get; set; }
        public virtual DbSet<Serv_Providers> Serv_Providers { get; set; }
        public virtual DbSet<Serv_Providers1> Serv_Providers1 { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<Specialities1> Specialities1 { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Inv_Sal> Inv_Sal { get; set; }
        public virtual DbSet<Serv_Ray> Serv_Ray { get; set; }
        public virtual DbSet<Swap> Swap { get; set; }
        public virtual DbSet<COMP_CUSTOMIZED_D> COMP_CUSTOMIZED_D { get; set; }
        public virtual DbSet<CompContractClass> CompContractClass { get; set; }
        public virtual DbSet<COMP_CUSTOMIZED_D_EMP> COMP_CUSTOMIZED_D_EMP { get; set; }
        public virtual DbSet<Contract_Data> Contract_Data { get; set; }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acception>()
                .HasMany(e => e.CardAcceptionReasons)
                .WithRequired(e => e.Acception)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AcceptionReason>()
                .HasMany(e => e.CardAcceptionReasons)
                .WithRequired(e => e.AcceptionReason)
                .HasForeignKey(e => e.AcceptionReasonsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers);
            // .WithMany(e => e.AspNetRoles)
            //  .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>();
            //.HasMany(e => e.AspNetUserClaims)
            // .WithRequired(e => e.AspNetUser)
            //.HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>();
                //.HasMany(e => e.AspNetUserLogins)
                //.WithRequired(e => e.AspNetUser)
                //.HasForeignKey(e => e.UserId);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.BS_CODE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.COMP_ID)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.BRANCH_CODE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.AMT)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.ISSUE_QTY)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.DELV_QTY)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.ADJUST_PERIOD)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.MAX_COLLECT_TIME)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.USED_VAL)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.USED_PERIOD)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Basic_Data>()
            //    .Property(e => e.NO_RETURN_MONTH)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.CO_ID)
            //    .HasPrecision(7, 0);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.INSURANCE_DAY)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.INSURANCE_MONTH)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.LAB_DAY)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.RAY_DAY)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.INSURANCE_DAY_LAB)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.INSURANCE_MONTH_LAB)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.COST_MONTHLY_YEAR)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.COST_DALLY_YEAR)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.NO_CLAEM_WEEK)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.NO_CLEEM_YEAR)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.FREE)
            //    .HasPrecision(1, 0);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.NO_CLM_DAY_YYYY)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.MONY_CLM_DAY_YYYY)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.NO_CLM_MON_YYYY)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.MONY_CLM_MON_YYYY)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.MED_DAY)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Co_Insurance_01>()
            //    .Property(e => e.MED_MONTH)
            //    .HasPrecision(18, 6);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.COMP_ID)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.BRANCH_CODE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.C_COMP_ID)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.CONTRACT_NO)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.CEILING_AMT)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.CEILING_PERT)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.NO_DAYS)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.REPEAT_NO)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.REPEAT_PERIOD)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.CARR_AMT)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Comp_Customized_D_D>()
            //    .Property(e => e.DOC_EXP_VALUE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<DailyPermission>()
            //    .HasMany(e => e.Doctors_Permissions)
            //    .WithRequired(e => e.DailyPermission)
            //    .HasForeignKey(e => e.PermissionId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Diagnosis>()
            //    .HasMany(e => e.MedicinesDiagnosis)
            //    .WithRequired(e => e.Diagnosis)
            //    .HasForeignKey(e => e.DiagnoiseId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Governate>()
            //    .HasMany(e => e.Users)
            //    .WithRequired(e => e.Governate)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Med_Card>()
            //    .Property(e => e.CARD_NO)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Card>()
            //    .Property(e => e.NOTES)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Card>()
            //    .Property(e => e.CREATED_BY)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Card>()
            //    .Property(e => e.UPDATE_BY)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Card>()
            //    .Property(e => e.GROUP_NAME)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Card>()
            //    .Property(e => e.TASHKHES_01)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Card>()
            //    .HasOptional(e => e.Med_Card1)
            //    .WithRequired(e => e.Med_Card2);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.MED_CODE)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.CARD_NO)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.MED_TYP)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.DOSE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.NO_OF_UINT)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.TOTAL_AMT)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.MED_DURATION)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.TOT_DUR)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.DOS_DUR)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.EXCESS)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.PACK_SIZE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.PACK_PRICE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.CON_MED)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.UNIT_NO)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.UNIT_PRICE)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.MED_NAME)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.DOSAGE_FORM)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.NOTES)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.ACTIVE)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.CREATED_BY)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.UPDATE_BY)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.ACT_MONTH)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.LFT_MONTH)
            //    .HasPrecision(18, 3);

            //modelBuilder.Entity<Med_Medicine>()
            //    .Property(e => e.MONTH_DATE_STOP)
            //    .IsUnicode(false);

            //modelBuilder.Entity<MedicineData1>()
            //    .Property(e => e.PACK_PRICE)
            //    .HasPrecision(7, 2);

            //modelBuilder.Entity<MedicineData1>()
            //    .Property(e => e.UNIT_PRICE)
            //    .HasPrecision(7, 2);

            //modelBuilder.Entity<MedicineData>()
            //    .HasMany(e => e.MedicinesDiagnosis)
            //    .WithRequired(e => e.MedicineData)
            //    .HasForeignKey(e => e.MedicineId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<MonthlyPermission>()
            //    .HasMany(e => e.Doctors_Permissions)
            //    .WithRequired(e => e.MonthlyPermission)
            //    .HasForeignKey(e => e.PermissionId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Pr_Bra1>()
            //    .Property(e => e.PR_CODE)
            //    .HasPrecision(22, 0);

            //modelBuilder.Entity<Pr_Bra1>()
            //    .Property(e => e.COMP_ID)
            //    .HasPrecision(22, 0);

            //modelBuilder.Entity<Pr_Bra1>()
            //    .Property(e => e.BRANCH_CODE)
            //    .HasPrecision(22, 0);

            //modelBuilder.Entity<Pr_Bra1>()
            //    .Property(e => e.PRV_TYPE)
            //    .HasPrecision(22, 0);

            //modelBuilder.Entity<Pr_Bra1>()
            //    .Property(e => e.AREA_CODE)
            //    .HasPrecision(22, 0);

            //modelBuilder.Entity<Pr_Bra1>()
            //    .Property(e => e.BRA_CODE)
            //    .HasPrecision(22, 0);

            //modelBuilder.Entity<Roshita>()
            //    .HasMany(e => e.RoshitaDetails)
            //    .WithRequired(e => e.Roshita)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Serv_Providers>()
            //    .HasMany(e => e.Users)
            //    .WithRequired(e => e.Serv_Providers)
            //    .HasForeignKey(e => e.USER_CO)
            //    .WillCascadeOnDelete(false);
        }
    }
}
