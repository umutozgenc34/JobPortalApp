﻿using JobPortalApp.Shared.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApp.Repository.Interceptors;

public class AuditDbContextInterceptor : SaveChangesInterceptor
{
    private static readonly Dictionary<EntityState, Action<DbContext, IAuditEntity>> Behaviors = new()
     {
         { EntityState.Added, AddBehavior },
         { EntityState.Modified, ModifiedBehavior }
     };

    private static void AddBehavior(DbContext context, IAuditEntity auditEntity)
    {
        auditEntity.CreatedAt = DateTime.UtcNow;
        context.Entry(auditEntity).Property(x => x.UpdatedAt).IsModified = false;
    }

    private static void ModifiedBehavior(DbContext context, IAuditEntity auditEntity)
    {
        context.Entry(auditEntity).Property(x => x.CreatedAt).IsModified = false;
        auditEntity.UpdatedAt = DateTime.UtcNow;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entityEntry in eventData.Context!.ChangeTracker.Entries().ToList())
        {
            if (entityEntry.Entity is not IAuditEntity auditEntity) continue;


            if (entityEntry.State is not (EntityState.Added or EntityState.Modified)) continue;


            Behaviors[entityEntry.State](eventData.Context, auditEntity);


        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
