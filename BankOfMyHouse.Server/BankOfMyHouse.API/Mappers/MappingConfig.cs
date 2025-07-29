using BankOfMyHouse.API.Endpoints.Users.DTOs;
using BankOfMyHouse.API.Endpoints.Users.Login;
using BankOfMyHouse.API.Endpoints.Users.Register;
using BankOfMyHouse.Domain.Users;
using Mapster;
using Microsoft.AspNetCore.Identity.Data;

namespace BankOfMyHouse.API.Mappers
{
	public class MappingConfig : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			// Configure RegisterUserRequest to User mapping
			config.NewConfig<RegisterUserRequestDto, User>()
				.Map(dest => dest.Username, src => src.Username)
				.Map(dest => dest.Email, src => src.Email)
				.Map(dest => dest.FirstName, src => src.FirstName)
				.Map(dest => dest.LastName, src => src.LastName)
				.Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
				.Map(dest => dest.IsActive, src => true)
				.Map(dest => dest.EmailConfirmed, src => false)
				.Map(dest => dest.UserRoles, src => new List<UserRole>())
				.Map(dest => dest.Roles, src => new List<Role>())
				.Ignore(dest => dest.PasswordHash)
				.Ignore(dest => dest.Id) // Auto-generated
				.Ignore(dest => dest.LastLoginAt); // Will be set on first login

			// User to UserResponse mapping
			config.NewConfig<User, UserDto>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Username, src => src.Username)
				.Map(dest => dest.Email, src => src.Email)
				.Map(dest => dest.CreatedAt, src => src.CreatedAt)
				.Map(dest => dest.LastLoginAt, src => src.LastLoginAt)
				.Map(dest => dest.IsActive, src => src.IsActive)
				.Map(dest => dest.Roles, src => src.Roles.Select(r => r.Name).ToList());

			// Additional mappings for other DTOs
			config.NewConfig<LoginRequestDto, User>()
				.Map(dest => dest.Username, src => src.Username);
		}
	}
}
