using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Commands.BookCommandsHandlers;
using LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Services.Queries;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.CommonKernel
{
    public static class LibraryManagementSystemModuleExtensions
    {
        // TODO: AppSettings appSettings 
        //public static IServiceCollection AddDatumModule(this IServiceCollection services, Appsettings )
        public static IServiceCollection AddLibraryManagementSystemModule(this IServiceCollection services )
        {
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddScoped<IRequestHandler<CreateUserCommand, User>, CreateUserCommandHandler>();
            services.AddScoped<IRequestHandler<GetAllUsersQuery, IEnumerable<User>>, GetAllUsersQueryHandler>();
            services.AddScoped<IRequestHandler<GetUserByIdQuery, User>, GetUserQueryHandler>();
            services.AddScoped<IRequestHandler<UpdateUserCommand, User>, UpdateUserCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteUserCommand, User>, DeleteUserCommandHandler>();
            services.AddScoped<IRequestHandler<CreateBookCommand, Book>, CreateBookCommandHandler>();
            services.AddScoped<IRequestHandler<GetAllBooksQuery, IEnumerable<Book>>, GetAllBooksQueryHandler>();
            services.AddScoped<IRequestHandler<GetBookQueryById, Book>, GetBookQueryHandler>();
            services.AddScoped<IRequestHandler<UpdateBookCommand, Book>, UpdateBookCommandHandler>();
            services
                .AddScoped<IRequestHandler<CreateBorrowingCommand, BorrowingRecord>, CreateBorrowingCommandHandler>();
            services
                .AddScoped<IRequestHandler<UpdateBorrowingCommand, BorrowingRecord>, UpdateBorrowingCommandHandler>();
           services
                .AddScoped<IRequestHandler<GetUserPermissionsQuery, List<Permission>>, GetUserPermissionsQueryHandler>();
            services.AddScoped<IRequestHandler<GetUserByEmailQuery, bool>, GetUserByEmailQueryHandler>();
            services.AddScoped<IRequestHandler<ValidateUserCredentialsQuery, User>, ValidateUserCredentialsQueryHandler>();
            services.AddScoped<IRequestHandler<DeleteBookCommand, Book>, DeleteBookCommandHandler>();

            //services.AddDbContext<LMSContext>(opt =>
            //    opt.UseNpgsql(
            //        //builder.Configuration.GetConnectionString("LibraryManagementSystemContext")
            //        ?? throw new InvalidOperationException("Connection string 'LibraryManagementSystemContext' not found.")
            //    )
            //);


            return services;
        }

        //TODO : Common Method for IRequestHandler
        
    }
}
