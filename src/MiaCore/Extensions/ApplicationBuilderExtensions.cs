using MiaCore.Features.AssignCategoryToUser;
using MiaCore.Features.CreateGetAccount;
using MiaCore.Features.CreateUser;
using MiaCore.Features.CurrentUserPlan;
using MiaCore.Features.FetchEntityById;
using MiaCore.Features.FetchMiaBillingInfo;
using MiaCore.Features.FetchProfile;
using MiaCore.Features.Firebase;
using MiaCore.Features.GenerictList;
using MiaCore.Features.GetDashboardStats;
using MiaCore.Features.Login;
using MiaCore.Features.NewsList;
using MiaCore.Features.RecoveryPassword;
using MiaCore.Features.Register;
using MiaCore.Features.RemoveEntityById;
using MiaCore.Features.SaveBillingInfo;
using MiaCore.Features.SaveCategory;
using MiaCore.Features.SaveNews;
using MiaCore.Features.SearchNewsByLocation;
using MiaCore.Features.TransferList;
using MiaCore.Features.UpdateProfile;
using MiaCore.Models;
using MiaCore.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MiaCore.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UserMiaAuthentication(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPostRequest<LoginRequest>("mia-auth/login", true);
                endpoints.MapPostRequest<RegisterRequest>("mia-auth/register", true);
                endpoints.MapGetRequest<FetchProfileRequest>("mia-auth/me");
                endpoints.MapPostRequest<SaveUserRequest>("mia-auth/user/save", roles: Roles.Admin);
                endpoints.MapPostRequest<UpdateProfileRequest>("mia-auth/update-profile");
                endpoints.MapPostRequest<FirebaseAuthenticationRequest>("mia-auth/login-with-firebase");
                endpoints.MapPostRequest<FirebaseAuthenticationRequest>("mia-auth/login-with-google");
                endpoints.MapPostRequest<FirebaseAuthenticationRequest>("mia-auth/login-with-facebook");
                endpoints.MapPostRequest<RecoveryPasswordRequest>("mia-auth/recovery");
                endpoints.MapPostRequest<ChangePasswordRecoveryRequest>("mia-auth/change-password-recovery");
                endpoints.MapPostRequest<GenerictListRequest<MiaUser>>("mia-auth/user/list", roles: Roles.Admin);
                endpoints.MapPostRequest<GenerictListRequest<MiaCurrency>>("mia-currency/list", true);
                endpoints.MapPostRequest<GenerictListRequest<MiaCategory>>("mia-category/list", true);
                endpoints.MapPostRequest<NewsListRequest>("news/list", true);
                endpoints.MapPostRequest<SaveCategoryRequest>("mia-category/save", roles: Roles.Admin);
                endpoints.MapPostRequest<AssignCategoryToUserRequest>("mia-category/assign-to-user");
                endpoints.MapPostRequest<GenerictListRequest<MiaUserPlan>>("mia-auth/user/plan/list", roles: Roles.Admin);
                endpoints.MapGetRequest<CurrentUserPlanRequest>("mia-auth/user/plan/current/{id}", roles: Roles.Admin);
                endpoints.MapGetRequest<GetDashboardStatsRequest>("dashboard/stats", roles: Roles.Admin);
                endpoints.MapGetRequest<CreateGetAccountRequest>("account/me");
                endpoints.MapGetRequest<TransferListRequest>("transfer/list");
                endpoints.MapDeleteRequest<RemoveEntityByIdRequest<MiaUser>>("mia-auth/user/remove/{id}", roles: Roles.Admin);
                endpoints.MapPostRequest<SaveNewsRequest>("news/save");
                endpoints.MapPostRequest<SearchNewsByLocationRequest>("news/search-by-location");
                endpoints.MapGetRequest<FetchEntityByIdRequest<MiaUser>>("mia-auth/user/fetch/{id}");
                endpoints.MapPostRequest<SaveBillingInfoRequest>("mia-billing/info/save", roles: Roles.Admin);
                endpoints.MapGetRequest<FetchMiaBillingInfoRequest>("mia-billing/info/fetch/{id}", roles: Roles.Admin);
            });

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            return app;
        }
    }
}