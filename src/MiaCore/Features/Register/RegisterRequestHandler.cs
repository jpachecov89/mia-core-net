using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Infrastructure.Mail;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Models.Enums;
using MiaCore.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MiaCore.Features.Register
{
    internal class RegisterRequestHandler : IRequestHandler<RegisterRequest, MiaUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly MiaCoreOptions _options;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;
        private readonly IMailService _mailService;

        public RegisterRequestHandler(IUserRepository userRepository, IMapper mapper, IOptions<MiaCoreOptions> options, IConfiguration config, IUnitOfWork uow, IMailService mailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _config = config;
            _uow = uow;
            _mailService = mailService;
        }

        public async Task<MiaUserDto> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<MiaUser>(request);
            user.Password = Hashing.GenerateSha256(user.Password);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new BadRequestException(ErrorMessages.EmailAlreadyExists);

            try
            {
                await _uow.BeginTransactionAsync();

                var userSaveRepo = _uow.GetGenericRepository<MiaUser>();
                var requestChangeRepo = _uow.GetGenericRepository<RequestChange>();
                var points = _config.GetSection("CredibilityPoints:StartingPoints").Get<decimal>();
                user.CredibilityPointsChecker = points;
                user.CredibilityPointsCreator = points;
                user.CredibilityPoints = points;

                user.Id = await userSaveRepo.InsertAsync(user);

                if (request.InstitutionRegistration)
                {
                    user.Status = MiaUserStatus.WaitingForValidation;
                    var requestChange = new RequestChange
                    {
                        NewRole = (int)MiaUserRole.Institution,
                        UserId = user.Id,
                        Message = "Institution Registration"
                    };
                    await requestChangeRepo.InsertAsync(requestChange);
                    await userSaveRepo.UpdateAsync(user);
                }
                else
                {
                    await _mailService.SendAsync(user.Email, "Sign-up successful", "successful-normal-registration", user.Language, new { fronturl = _options.FrontUrl });
                }

                await _uow.CommitTransactionAsync();

                var response = _mapper.Map<MiaUserDto>(user);
                return response;
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }
    }
}