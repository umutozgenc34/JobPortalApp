using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Text.Json;

namespace JobPortalApp.Repository.Companies.Concretes;

public class CompanyRepositoryWithCache : ICompanyRepository
{
    private readonly ICompanyRepository _innerRepository;
    private readonly IDistributedCache _distributedCache;
    private const string CacheKeyPrefix = "company_";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
    };

    public CompanyRepositoryWithCache(ICompanyRepository innerRepository, IDistributedCache distributedCache)
    {
        _innerRepository = innerRepository;
        _distributedCache = distributedCache;
    }

    public async Task<List<Company>> GetAllAsync()
    {
        var cacheKey = $"{CacheKeyPrefix}all_companies";

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            var cachedCompanies = JsonSerializer.Deserialize<List<Company>>(cachedData, _jsonSerializerOptions);
            return cachedCompanies!;
        }

        var companies = await _innerRepository.GetAllAsync();

        await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(companies, _jsonSerializerOptions), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return companies;
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<Company>(cachedData, _jsonSerializerOptions);
        }

        var company = await _innerRepository.GetByIdAsync(id);

        if (company != null)
        {
            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(company, _jsonSerializerOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        return company;
    }

    public IQueryable<Company> Where(Expression<Func<Company, bool>>? predicate = null)
    {
        return _innerRepository.Where(predicate);
    }

    public async Task AddAsync(Company entity)
    {
        await _innerRepository.AddAsync(entity);
        await _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        await _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_companies");
    }

    public void Update(Company entity)
    {
        _innerRepository.Update(entity);
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_companies");
    }

    public void Delete(Company entity)
    {
        _innerRepository.Delete(entity);
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_companies");
    }

    public IQueryable<Company?> GetCompanyWithJobPostings()
    {
        return _innerRepository.GetCompanyWithJobPostings();
    }

    public async Task<Company?> GetCompanyWithJobPostingsAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}_with_jobs";
        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<Company>(cachedData, _jsonSerializerOptions);
        }

        var company = await _innerRepository.GetCompanyWithJobPostingsAsync(id);
        if (company != null)
        {
            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(company, _jsonSerializerOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
        }
        return company;
    }
}