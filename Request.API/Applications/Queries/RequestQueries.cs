﻿using API.Exceptions;
using AutoMapper;
using Dapper;
using Request.Domain.Entities.Requests;
using Request.Domain.Entities.Users;
using Request.Infrastructure.Data;
using System.Net;

namespace Request.API.Applications.Queries
{
    public class RequestQueries : IRequestQueries
    {
        private readonly ApplicationDbContext _dbContext;
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<RequestQueries> _logger;
        private readonly IMapper _mapper;

        public RequestQueries(ApplicationDbContext dbContext,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            ILogger<RequestQueries> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<List<LeaveRequestResponse>> GetLeaveRequestByRequestorId()
        {
            using var connection = _dbContext.GetConnection();

            string query = $@"SELECT *
                    FROM dbo.LeaveRequests as r
                    LEFT JOIN dbo.Statuses    AS s
                    ON (
                        r.StatusId  = s.Id
                    )
					LEFT JOIN dbo.Users	AS u
                    ON (
                        r.ApproverId	= u.Id
                    )
					INNER JOIN dbo.Stages	AS st
                    ON (
                        r.Id		= st.LeaveRequestId
                    )
                    WHERE r.RequestorId = @requestorId AND r.IsDelete = 0";
            var requestDictionary = new Dictionary<Guid, LeaveRequest>();
            var results = await connection.QueryAsync<LeaveRequest, Status, User, Stage, LeaveRequest>(query,
                (request, status, user, stage) =>
                {
                    LeaveRequest requestEntry;

                    if (!requestDictionary.TryGetValue(request.Id, out requestEntry))
                    {
                        requestEntry = request;
                        requestEntry.Stages = new List<Stage>();
                        requestDictionary.Add(requestEntry.Id, requestEntry);
                        requestEntry.Status = status;
                        requestEntry.Approver = user;
                    }

                    requestEntry.Stages.Add(stage);
                    return requestEntry;
                }, new { requestorId = GetCurrentUserId() });
            return MapperLeaveRequests(results.ToList());
        }

        public async Task<List<LeaveRequestResponse>> GetLeaveRequestByApproverId()
        {
            using var connection = _dbContext.GetConnection();

            string query = $@"SELECT *
                    FROM dbo.LeaveRequests as r
                    LEFT JOIN dbo.Statuses    AS s
                    ON (
                        r.StatusId  = s.Id
                    )
					LEFT JOIN dbo.Users	AS u
                    ON (
                        r.ApproverId	= u.Id
                    )
					INNER JOIN dbo.Stages	AS st
                    ON (
                        r.Id		= st.LeaveRequestId
                    )
                    WHERE r.ApproverId = @approverId AND r.IsDelete = 0";
            var requestDictionary = new Dictionary<Guid, LeaveRequest>();
            var results = await connection.QueryAsync<LeaveRequest, Status, User, Stage, LeaveRequest>(query,
                (request, status, user, stage) =>
                {
                    LeaveRequest requestEntry;

                    if (!requestDictionary.TryGetValue(request.Id, out requestEntry))
                    {
                        requestEntry = request;
                        requestEntry.Stages = new List<Stage>();
                        requestDictionary.Add(requestEntry.Id, requestEntry);
                        requestEntry.Status = status;
                        requestEntry.Approver = user;
                    }

                    requestEntry.Stages.Add(stage);
                    return requestEntry;
                }, new { approverId = GetCurrentUserId() });
            return MapperLeaveRequests(results.Where(c => c.Stages.Count(c => c.Name == StageEnum.Finish) == 0).ToList());
        }

        public List<LeaveRequestResponse> MapperLeaveRequests(List<LeaveRequest> results)
        {
            List<LeaveRequestResponse> leaveRequests = new List<LeaveRequestResponse>();
            foreach (var item in results)
            {
                LeaveRequestResponse leaveRequestReponse = new LeaveRequestResponse();
                leaveRequestReponse.Id = item.Id;
                leaveRequestReponse.StatusId = item.StatusId;
                leaveRequestReponse.StatusName = item.Status.Name;
                leaveRequestReponse.ApproverId = item.Approver.Id;
                leaveRequestReponse.ApproverName = item.Approver.UserName;
                leaveRequestReponse.Name = item.Name;
                leaveRequestReponse.StageName = item.Stages.Last().Name;
                leaveRequestReponse.CreatedAt = item.CreatedAt;
                leaveRequestReponse.UpdatedAt = item.UpdatedAt;
                leaveRequests.Add(leaveRequestReponse);
            }
            return leaveRequests;
        }

        public async Task<LeaveRequestDetail> GetLeaveRequest(Guid id)
        {
            using var connection = _dbContext.GetConnection();

            string query = $@"SELECT *
                    FROM dbo.LeaveRequests as r
                    LEFT JOIN dbo.Statuses    AS s
                    ON (
                        r.StatusId  = s.Id
                    )
					LEFT JOIN dbo.Users	AS u
                    ON (
                        r.ApproverId	= u.Id
                    )
                    LEFT JOIN dbo.Users	AS ur
                    ON (
                        r.RequestorId	= ur.Id
                    )
					INNER JOIN dbo.Stages	AS st
                    ON (
                        r.Id		= st.LeaveRequestId
                    )
                    WHERE r.Id = @id
                    AND r.IsDelete = 0
                    AND (r.RequestorId = @userId OR r.ApproverId = @userId)";
            var requestDictionary = new Dictionary<Guid, LeaveRequest>();
            var results = await connection.QueryAsync<LeaveRequest, Status, User, User, Stage, LeaveRequest>(query,
                (request, status, approver, requestor, stage) =>
                {
                    LeaveRequest requestEntry;

                    if (!requestDictionary.TryGetValue(request.Id, out requestEntry))
                    {
                        requestEntry = request;
                        requestEntry.Stages = new List<Stage>();
                        requestDictionary.Add(requestEntry.Id, requestEntry);
                        requestEntry.Status = status;
                        requestEntry.Approver = approver;
                        requestEntry.Requestor = requestor;
                    }

                    requestEntry.Stages.Add(stage);
                    return requestEntry;
                }, new
                {
                    id = id,
                    userId = GetCurrentUserId()
                });
            var leaveRequests = results.ToList();
            if (leaveRequests.Count() == 0)
            {
                _logger.LogError("Not found Leave request", nameof(RequestQueries));
                throw new KeyNotFoundException();
            }
            return MapperLeaveRequest(leaveRequests[0]);
        }

        public LeaveRequestDetail MapperLeaveRequest(LeaveRequest result)
        {
            LeaveRequestDetail leaveRequestDetail = new LeaveRequestDetail();
            leaveRequestDetail.Id = result.Id;
            leaveRequestDetail.StatusId = result.StatusId;
            leaveRequestDetail.StatusName = result.Status.Name;
            leaveRequestDetail.ApproverId = result.Approver.Id;
            leaveRequestDetail.ApproverName = result.Approver.UserName;
            leaveRequestDetail.Name = result.Name;
            leaveRequestDetail.Stages = _mapper.Map<List<Stage>, List<StageResponse>>(result.Stages.ToList());
            leaveRequestDetail.CreatedAt = result.CreatedAt;
            leaveRequestDetail.UpdatedAt = result.UpdatedAt;
            leaveRequestDetail.Message = result.Message;
            leaveRequestDetail.RequestorId = result.RequestorId;
            leaveRequestDetail.RequestorName = result.Requestor.UserName;
            return leaveRequestDetail;
        }

        public Guid GetCurrentUserId()
        {
            var result = Guid.TryParse(_contextAccessor.HttpContext.User.Claims.First(i => i.Type == "id").Value, out var userId);
            return result ? userId : throw new HttpResponseException(HttpStatusCode.Unauthorized, "Something went wrong");
        }
    }
}