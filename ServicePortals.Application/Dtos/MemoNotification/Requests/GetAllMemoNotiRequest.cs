﻿namespace ServicePortals.Application.Dtos.MemoNotification.Requests
{
    public class GetAllMemoNotiRequest
    {
        public string? CurrentUserCode { get; set; }
        public string? RoleName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
