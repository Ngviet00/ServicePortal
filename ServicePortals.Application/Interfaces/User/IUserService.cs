﻿using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;

namespace ServicePortals.Application.Interfaces.User
{
    public interface IUserService
    {
        Task<PersonalInfoResponse?> GetMe(string userCode);
        Task<UserResponse> GetById(Guid id);
        Task<UserResponse?> GetByUserCode(string code);
        Task<object?> GetCustomColumnUserViclockByUserCode(string userCode, string columns);
        Task<Domain.Entities.User?> GetRoleAndPermissionByUser(string userCode);
        UserRolesAndPermissionsResponse FormatRoleAndPermissionByUser(Domain.Entities.User? user);
        Task<UserResponse> Delete(Guid id);
        Task<UserResponse> ForceDelete(Guid id);
        Task<PagedResults<GetAllUserResponse>> GetAll(GetAllUserRequest request);
        Task<bool> UpdateUserRole(UpdateUserRoleRequest request);
        Task<UserResponse> ResetPassword(ResetPasswordRequest request);
        Task<UserResponse> Update(string userCode, UpdatePersonalInfoRequest request);


        //---------------------------------------------
        Task<List<GetEmailByUserCodeAndUserConfigResponse>> GetEmailByUserCodeAndUserConfig(List<string> UserCodes);
    }
}
