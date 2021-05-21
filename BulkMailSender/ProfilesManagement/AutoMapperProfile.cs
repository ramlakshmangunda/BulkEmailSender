using AutoMapper;
using BulkMailSender.Models;
using BulkMailSender.TableEntities;
using BulkMailSender.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.ProfilesManagement
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TblTdsCertificate, TdsCertificateViewModel>().ReverseMap();
            CreateMap<TblTdsCertificate, CreateTdsViewModel>().ReverseMap();
            CreateMap<TblAllTdsEmail, AllTdsEmailViewModel>().ReverseMap();
            CreateMap<TblAllTdsEmail, EmailUpdateModel>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<TblRestructuring, RestructuringViewModel>()
                .ForMember(d => d.Confirmation,s=>s.MapFrom(m => m.Confirmation == true ? "YES" : "NO"))
                .ForMember(d => d.SendCopy, s => s.MapFrom(m => m.SendCopy == true ? "YES" : "NO"))
                .ForMember(d => d.RespondedDate, s => s.MapFrom(m => m.CreatedOn));
        }
    }
}
