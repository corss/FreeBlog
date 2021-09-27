using AutoMapper;
using FreeBlog.Common;
using FreeBlog.Model.Models;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Model.ViewModels;


namespace FreeBlog.IService.AutoMapper
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// 对象转换配置
        /// </summary>
        public MappingProfile()
        {
            CreateMap<UserInfo, UserViewModel>()
              .ForMember(a => a.username, a => a.MapFrom(s => s.userName));
            CreateMap<UserViewModel, UserInfo>()
                .ForMember(a => a.userName, a => a.MapFrom(s => s.username));

           // 任务
            CreateMap<Article, ArticleViewModel>()
              //.ForMember(d => d.ImgUrl, o => o.MapFrom(s => Utility.GetImgUrl(s.ImgUrl)))
              //.ForMember(d => d.FileUrl, o => o.MapFrom(s => Utility.GetImgUrl(s.FileUrl)))
              .ForMember(d => d.AddDate, o => o.MapFrom(s => Utility.GetDateFormat(s.AddDate)));

            // 任务
            CreateMap<Policeproject, PoliceProModel>()
              //.ForMember(d => d.ImgUrl, o => o.MapFrom(s => Utility.GetImgUrl(s.ImgUrl)))
              //.ForMember(d => d.FileUrl, o => o.MapFrom(s => Utility.GetImgUrl(s.FileUrl)))
              .ForMember(d => d.AddDate, o => o.MapFrom(s => Utility.GetDateFormat(s.AddDate)));
        }
    }
}
