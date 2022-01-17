using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using DMS_Synchronization.OracleModels;
using DMS_Synchronization.Models;
using System.Data;

namespace DMS_Synchronization.Mapping
{
    public class MappingProfile : Profile
    {
        public static void Initialize()
        {
            Mapper.Initialize(ctr =>
            {
                ctr.CreateMap<IDataReader, DMS_02_EMP_D_ENT>();
                ctr.CreateMap<DMS_02_EMP_D_ENT, Roshita>()
                        .ForMember(d => d.CardId, m => m.MapFrom(s => s.CARD_ID))
                        .ForMember(d => d.TotalValue, m => m.MapFrom(s =>s.D_VD))
                        .ForMember(d => d.Id, m => m.MapFrom(s => s.D_ID))
                        .ForMember(d => d.Speciality, m => m.MapFrom(s => s.TAKHASOS))
                        .ForMember(d => d.Diagnose1, m => m.MapFrom(s => s.TASHKHES_01))
                        .ForMember(d => d.Diagnose2, m => m.MapFrom(s => s.TASHKHES_02))
                        .ForMember(d => d.diagnose3, m => m.MapFrom(s => s.TASHKHES_03))
                        .ForMember(d => d.Limit, m => m.MapFrom(s => s.INSU_LIMT))
                        .ForMember(d => d.OverInsurance, m => m.MapFrom(s => s.OVER_INSURANCE))
                        .ForMember(d => d.CompanyPayment, m => m.MapFrom(s => s.VALUE_CREDIT))
                        .ForMember(d => d.PersonPayment, m => m.MapFrom(s => s.CARRY))
                        .ForMember(d => d.Cash, m => m.MapFrom(s => s.VALUE_CASH))
                        .ForMember(d => d.Manager, m => m.MapFrom(s => s.MANAGER))
                        .ForMember(d => d.CreatedBy, m => m.MapFrom(s => s.EMP_NAME))
                        .ForMember(d => d.PhoneNumber, m => m.MapFrom(s => s.MOB_NO))
                        //.IgnoreAllSourcePropertiesWithAnInaccessibleSetter().
                        /*.IgnoreAllPropertiesWithAnInaccessibleSetter()*/;
                //.ForMember(dest => dest.CardId, opt => opt.MapFrom(src => src.CARD_ID))
                //.IgnoreAllSourcePropertiesWithAnInaccessibleSetter().
                //IgnoreAllPropertiesWithAnInaccessibleSetter();
                //ctr.CreateMap<Device, asd>()
                //.ForMember(dest => dest.deviceName1, opt => opt.MapFrom(m => m.Device_Name)).
                //IgnoreAllSourcePropertiesWithAnInaccessibleSetter().
                //IgnoreAllPropertiesWithAnInaccessibleSetter();
                //Property_Value, PropertyValueVM DeviceVM
                //ctr.CreateMap<Device_Categories, DeviceCategoryVM>();
                //ctr.CreateMap<Property_Value, PropertyValueVM>();
                //ctr.CreateMap<Device, DeviceVM>()
                //.ForMember(dest => dest.Device_Category, opt => opt.MapFrom(m => Mapper.Map<DeviceCategoryVM>(m.Device_Categories)));

            });
        }
    }
}