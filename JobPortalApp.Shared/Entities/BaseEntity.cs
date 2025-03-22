﻿namespace JobPortalApp.Shared.Entities;

public class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
}
