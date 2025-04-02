using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Text.Json;

namespace JobPortalApp.Repository.Companies.Concretes;

public class CompanyReviewRepositoryWithCache : ICompanyReviewRepository
{
    private readonly ICompanyReviewRepository _innerRepository;
    private readonly IDistributedCache _distributedCache;
    private const string CacheKeyPrefix = "company_review_";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
    };

    public CompanyReviewRepositoryWithCache(ICompanyReviewRepository innerRepository, IDistributedCache distributedCache)
    {
        _innerRepository = innerRepository;
        _distributedCache = distributedCache;
    }

    public async Task<List<CompanyReview>> GetAllAsync()
    {
        var cacheKey = $"{CacheKeyPrefix}all_reviews";

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            var cachedReviews = JsonSerializer.Deserialize<List<CompanyReview>>(cachedData, _jsonSerializerOptions);
            return cachedReviews!;
        }

        var reviews = await _innerRepository.GetAllAsync();

        await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(reviews, _jsonSerializerOptions), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return reviews;
    }

    public async Task<CompanyReview?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<CompanyReview>(cachedData, _jsonSerializerOptions);
        }

        var review = await _innerRepository.GetByIdAsync(id);

        if (review != null)
        {
            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(review, _jsonSerializerOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        return review;
    }

    public IQueryable<CompanyReview> Where(Expression<Func<CompanyReview, bool>>? predicate = null)
    {
        return _innerRepository.Where(predicate);
    }

    public async Task AddAsync(CompanyReview entity)
    {
        await _innerRepository.AddAsync(entity);
        await _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        await _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_reviews");
    }

    public void Update(CompanyReview entity)
    {
        _innerRepository.Update(entity);
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_reviews");
    }

    public void Delete(CompanyReview entity)
    {
        _innerRepository.Delete(entity);
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_reviews");
    }
}
