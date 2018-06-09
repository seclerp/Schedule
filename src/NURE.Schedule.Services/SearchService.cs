using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using NURE.Schedule.Common;
using NURE.Schedule.Domain.CistApi.Structure;
using NURE.Schedule.Domain.Entities;
using NURE.Schedule.Domain.Repositories.Interfaces;
using NURE.Schedule.Services.Interfaces;
using NURE.Schedule.Services.Models;
using Group = NURE.Schedule.Domain.CistApi.Structure.Group;

namespace NURE.Schedule.Services
{
  public class SearchService : ISearchService
  {
    private const int SearchItemsRelevanceId = -1;
    
    private readonly IRelevanceService _relevanceService;
    private readonly ISearchItemRepository _searchItemRepository;
    private readonly ICistService _cistService;
    private readonly IMapper _mapper;
    private readonly ILastUpdateRepository _lastUpdateRepository;
    
    public SearchService(
      IRelevanceService relevanceService,
      ISearchItemRepository searchItemRepository, 
      ICistService cistService, 
      IMapper mapper, 
      ILastUpdateRepository lastUpdateRepository)
    {
      _searchItemRepository = searchItemRepository;
      _cistService = cistService;
      _mapper = mapper;
      _lastUpdateRepository = lastUpdateRepository;
      _relevanceService = relevanceService;
    }
    
    public async Task<IEnumerable<SearchResultModel>> SearchAsync(string pattern, bool searchInTeachers, bool searchInGroups)
    {
      pattern = pattern ?? "";
      
      var result = new List<SearchResultModel>();

      if (!await IsSearchItemsRelevantAsync())
      {
        await UpdateSearchItemsAsync();
      }
      
      if (searchInTeachers)
      {       
        var teachersFiltered = await _searchItemRepository.GetAllFilteredAsync(pattern, SearchItemType.Teacher);
        var teachersMapped = _mapper.Map<IEnumerable<SearchItemEntity>, IEnumerable<SearchResultModel>>(teachersFiltered);

        result.AddRange(teachersMapped);
      }
      
      if (searchInGroups)
      {
        var groupsFiltered = await _searchItemRepository.GetAllFilteredAsync(pattern, SearchItemType.Group);;
        var groupsMapped = _mapper.Map<IEnumerable<SearchItemEntity>, IEnumerable<SearchResultModel>>(groupsFiltered);
        
        result.AddRange(groupsMapped);
      }

      return result;
    }

    private async Task UpdateSearchItemsAsync()
    {  
      await _searchItemRepository.RemoveAllAsync();

      var newTeachers = _mapper.Map<IEnumerable<Teacher>, IEnumerable<SearchItemEntity>>(
        await _cistService.GetTeachersAsync()
      );
      
      var newGroups = _mapper.Map<IEnumerable<Group>, IEnumerable<SearchItemEntity>>(
        await _cistService.GetGroupsAsync()
      );

      await _searchItemRepository.AddRangeAsync(newTeachers);
      await _searchItemRepository.AddRangeAsync(newGroups);

      var searchItemRelevanceEntity = new LastUpdateEntity {Id = SearchItemsRelevanceId, DateTime = DateTime.Now};
      var entity = await _lastUpdateRepository.GetAsync(SearchItemsRelevanceId);
      if (!(entity is null))
      {
        await _lastUpdateRepository.UpdateAsync(searchItemRelevanceEntity);
      }
      else
      {
        await _lastUpdateRepository.AddAsync(searchItemRelevanceEntity);
      }
    }
    
    private async Task<bool> IsSearchItemsRelevantAsync()
    {
      return await _relevanceService.IsTimeTableRelevantAsync(SearchItemsRelevanceId);
    }
  }
}