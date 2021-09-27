using FreeBlog.Model.Models;
using FreeBlog.Model.Models.Log;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Model.Models.Project;
using FreeSql;

namespace FreeBlog.Model
{
    public class TemplateContext : DbContext
    {
        DbSet<UserInfo> UserInfo { get; set; }
        DbSet<LoginLog> LoginLog { get; set; }
        DbSet<Log> Log { get; set; }
        DbSet<ExceptionLog> ExceptionLog { get; set; }
        DbSet<Menu> Menu { get; set; }
        DbSet<Role> Role { get; set; }
        DbSet<ArticleMenu> ArticleMenu { get; set; }
        DbSet<ArticleMenu_Article> ArticleMenu_Article { get; set; }
        DbSet<Department> Department { get; set; }
        DbSet<StandingBook> StandingBook { get; set; }
        DbSet<Role_value> Role_value { get; set; }
        DbSet<PlanCommentInfo> PlanCommentInfo { get; set; }
        DbSet<ProjectPlanInfo> ProjectPlanInfo { get; set; }
        DbSet<ProjectUserInfo> ProjectUserInfo { get; set; }
        DbSet<PoliceInfo> Police { get; set; }
        DbSet<PoliceForce> PoliceForce { get; set; }

        DbSet<PoliceGrade> PoliceGrade { get; set; }
        DbSet<Policeproject> Policeproject { get; set; }
        DbSet<PoliceprojectMenu> PoliceprojectMenu { get; set; }
        DbSet<PoliceprojectMenu_Policeproject> PoliceprojectMenu_Policeproject { get; set; }
        DbSet<Module> Module { get; set; }
    }
}
