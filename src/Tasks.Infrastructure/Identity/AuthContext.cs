using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tasks.Infrastructure.Identity;

public class AuthContext(DbContextOptions<AuthContext> options) : IdentityDbContext(options);