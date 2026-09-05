using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TfShop.Controllers
{
    public class BaseController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        public BaseController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }
      
        internal SettingDto  GetSetting()
        {
            SettingDto setting = null;
            if (Session["settingPersian"] == null)
            {
                var configuration = new MapperConfiguration(cfg =>
                {

                    cfg.CreateMap<Setting, SettingDto>()
                    .ForMember(dto => dto.fishCardFileName, conf => conf.MapFrom(ol => ol.fishCardattachment.FileName))
                    .ForMember(dto => dto.attachmentFileName, conf => conf.MapFrom(ol => ol.attachment.FileName))
                    .ForMember(dto => dto.attachmentFileNameMag, conf => conf.MapFrom(ol => ol.attachmentLogoMag.FileName))
                    .ForMember(dto => dto.FaviconattachmentFileName, conf => conf.MapFrom(ol => ol.Faviconattachment.FileName))
                    .ForMember(dto => dto.FaviconattachmentFileNameMag, conf => conf.MapFrom(ol => ol.FaviconattachmentMag.FileName))
                    .ForMember(dto => dto.WaterattachmentFileName, conf => conf.MapFrom(ol => ol.Waterattachment.FileName));
                });
                setting = uow.SettingRepository.GetQueryList().AsNoTracking().Include(c => c.attachment).Include(c => c.fishCardattachment).Include(c => c.Faviconattachment).Where(c => c.LanguageId == 1)
                    .ProjectTo<SettingDto>(configuration).FirstOrDefault();
                Session["settingPersian"] = setting;
            }
            else
            {
                setting = Session["settingPersian"] as SettingDto;
            }
            if (setting == null)
            {
                var configuration = new MapperConfiguration(cfg =>
                {

                    cfg.CreateMap<Setting, SettingDto>()
                    .ForMember(dto => dto.fishCardFileName, conf => conf.MapFrom(ol => ol.fishCardattachment.FileName))
                    .ForMember(dto => dto.attachmentFileName, conf => conf.MapFrom(ol => ol.attachment.FileName))
                    .ForMember(dto => dto.attachmentFileNameMag, conf => conf.MapFrom(ol => ol.attachmentLogoMag.FileName))
                    .ForMember(dto => dto.FaviconattachmentFileName, conf => conf.MapFrom(ol => ol.Faviconattachment.FileName))
                    .ForMember(dto => dto.FaviconattachmentFileNameMag, conf => conf.MapFrom(ol => ol.FaviconattachmentMag.FileName))
                    .ForMember(dto => dto.WaterattachmentFileName, conf => conf.MapFrom(ol => ol.Waterattachment.FileName));
                });
                setting = uow.SettingRepository.GetQueryList().AsNoTracking().Include(c => c.fishCardattachment).Include(c => c.attachment).Include(c => c.Faviconattachment).Where(c => c.LanguageId == 1)
                    .ProjectTo<SettingDto>(configuration).FirstOrDefault();
                Session["settingPersian"] = setting;
            }
            return setting;
        }

    }
}