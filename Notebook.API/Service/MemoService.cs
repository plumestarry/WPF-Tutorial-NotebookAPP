﻿using AutoMapper;
using Notebook.API.Context;
using Notebook.Shared.Dtos;
//using Notebook.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.API.Service
{
    /// <summary>
    /// 备忘录的实现
    /// </summary>
    public class MemoService : IMemoService
    {
        private readonly IUnitOfWork work;
        private readonly IMapper Mapper;

        public MemoService(IUnitOfWork work, IMapper mapper)
        {
            this.work = work;
            Mapper = mapper;
        }

        public async Task<ApiResponse> AddAsync(MemoDto model)
        {
            try
            {
                var memo = Mapper.Map<Memo>(model);
                await work.GetRepository<Memo>().InsertAsync(memo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, memo);
                return new ApiResponse("添加数据失败");
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var repository = work.GetRepository<Memo>();
                var memo = await repository
                    .GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                repository.Delete(memo);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, "");
                return new ApiResponse("删除数据失败");
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> GetAllAsync(QueryParameter parameter)
        {
            try
            {
                var repository = work.GetRepository<Memo>();
                var memo = await repository.GetAllAsync(orderBy: source => source.OrderByDescending(t => t.CreateDate));
                var memos = await repository.GetPagedListAsync(predicate:
                   x => string.IsNullOrWhiteSpace(parameter.Search) || x.Title.Contains(parameter.Search),
                   pageIndex: parameter.PageIndex,
                   pageSize: parameter.PageSize,
                   orderBy: source => source.OrderByDescending(t => t.CreateDate));
                return new ApiResponse(true, memos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> GetSingleAsync(int id)
        {
            try
            {
                var repository = work.GetRepository<Memo>();
                var memo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                return new ApiResponse(true, memo);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateAsync(MemoDto model)
        {
            try
            {
                var dbmemo = Mapper.Map<Memo>(model);
                var repository = work.GetRepository<Memo>();
                var memo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(dbmemo.Id));

                memo.Title = dbmemo.Title;
                memo.Content = dbmemo.Content;
                memo.UpdateDate = DateTime.Now;

                repository.Update(memo);

                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, memo);
                return new ApiResponse("更新数据异常！");
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
    }
}
