using BankOfMyHouse.API.Endpoints.Accounts.Get;
using BankOfMyHouse.API.Endpoints.Currencies.GetCurrencies;
using BankOfMyHouse.API.Endpoints.PaymentCategories.GetPaymentCategories;
using BankOfMyHouse.API.Endpoints.Transactions.DTOs;
using BankOfMyHouse.API.Endpoints.Users.DTOs;
using BankOfMyHouse.API.Endpoints.Users.Login;
using BankOfMyHouse.API.Endpoints.Users.Register;
using BankOfMyHouse.Domain.BankAccounts;
using BankOfMyHouse.Domain.Iban;
using BankOfMyHouse.Domain.Users;
using Mapster;

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
				.Map(dest => dest.Roles, src => src.UserRoles.Select(r => r.Role.Name).ToList());

			// Additional mappings for other DTOs
			config.NewConfig<LoginRequestDto, User>()
				.Map(dest => dest.Username, src => src.Username);

			//config.NewConfig<IbanCodeDto, IbanCode>()
			//.Map(dest => dest.Value, src => IbanCode.Create(src.Value));

			//config.NewConfig<IbanCode, IbanCodeDto>()
			//	.Map(dest => new IbanCodeDto(dest.Value), src => src.Value);

			config.NewConfig<IbanCodeDto, IbanCode>()
				.ConstructUsing(src => IbanCode.Create(src.Value)); // Uses ConstructUsing instead of MapWith

			config.NewConfig<IbanCode, IbanCodeDto>()
				.ConstructUsing(src => new IbanCodeDto(src.Value)); // 👈 Uses MapWith for constructor

			// PaymentCategory to PaymentCategoryDto mapping
			config.NewConfig<PaymentCategory, PaymentCategoryDto>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Code, src => src.Code)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.IsActive, src => src.IsActive);

			// Currency to CurrencyDto mapping
			config.NewConfig<Currency, CurrencyDto>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Code, src => src.Code)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Symbol, src => src.Symbol);

			// Transaction to TransactionDto mapping
			config.NewConfig<Transaction, TransactionDto>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Amount, src => src.Amount)
				.Map(dest => dest.TransactionCreation, src => src.TransactionCreation)
				.Map(dest => dest.PaymentCategoryCode, src => src.PaymentCategory.Code)
				.Map(dest => dest.PaymentCategoryName, src => src.PaymentCategory.Name)
				.Map(dest => dest.CurrencyCode, src => src.Currency.Code)
				.Map(dest => dest.CurrencySymbol, src => src.Currency.Symbol)
				.Map(dest => dest.Description, src => src.Description)
				.Map(dest => dest.SenderIban, src => src.Sender.Value)
				.Map(dest => dest.ReceiverIban, src => src.Receiver.Value);

			config.NewConfig<BankAccount, BankAccountDto>()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.IBAN, src => src.IBAN.Value)
				.Map(dest => dest.Balance, src => src.Balance)
				.Map(dest => dest.CreatedAt, src => src.CreationDate);
		}
	}
}
