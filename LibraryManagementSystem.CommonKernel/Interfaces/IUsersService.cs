﻿using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces;

public interface IUsersService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(long id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<List<Permission>> GetUserPermissionsAsync(long userId);
}
