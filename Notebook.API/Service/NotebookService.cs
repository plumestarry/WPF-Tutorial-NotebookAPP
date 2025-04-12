using AutoMapper;
using Notebook.API.Context;
using Notebook.Shared.Dtos;
using Notebook.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Notebook.Shared;
using Notebook.API;
using Notebook.API.Service;

namespace Notebook.API.Service
{
    /// <summary>
    /// 待办事项的实现
    /// </summary>
    public class NotebookService : INotebookService
    {
        private readonly IUnitOfWork work;
        private readonly IMapper Mapper;

        public NotebookService(IUnitOfWork work, IMapper mapper)
        {
            this.work = work;
            Mapper = mapper;
        }

        public async Task<ApiResponse> AddAsync(NotebookDto model)
        {
            try
            {
                var notebook = Mapper.Map<NotebookEntity>(model);
                await work.GetRepository<NotebookEntity>().InsertAsync(notebook);
                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, notebook);
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
                var repository = work.GetRepository<NotebookEntity>();
                var notebook = await repository
                    .GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                repository.Delete(notebook);
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
                var repository = work.GetRepository<NotebookEntity>();
                var notebook = await repository.GetAllAsync(orderBy: source => source.OrderByDescending(t => t.CreateDate));
                var notebooks = await repository.GetPagedListAsync(predicate:
                   x => string.IsNullOrWhiteSpace(parameter.Search) || x.Title.Contains(parameter.Search),
                   pageIndex: parameter.PageIndex,
                   pageSize: parameter.PageSize,
                   orderBy: source => source.OrderByDescending(t => t.CreateDate));
                return new ApiResponse(true, notebooks);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> GetAllAsync(NotebookParameter parameter)
        {
            try
            {
                var repository = work.GetRepository<NotebookEntity>();
                var notebook = await repository.GetAllAsync(orderBy: source => source.OrderByDescending(t => t.CreateDate));
                var notebooks = await repository.GetPagedListAsync(predicate:
                   x => (string.IsNullOrWhiteSpace(parameter.Search) || x.Title.Contains(parameter.Search))
                   && (parameter.Status == null || x.Status.Equals(parameter.Status)),
                   pageIndex: parameter.PageIndex,
                   pageSize: parameter.PageSize,
                   orderBy: source => source.OrderByDescending(t => t.CreateDate));
                return new ApiResponse(true, notebooks);
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
                var repository = work.GetRepository<NotebookEntity>();
                var notebook = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                return new ApiResponse(true, notebook);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> Summary()
        {
            try
            {
                //待办事项结果
                var notebooks = await work.GetRepository<NotebookEntity>().GetAllAsync(
                    orderBy: source => source.OrderByDescending(t => t.CreateDate));

                //备忘录结果
                var memos = await work.GetRepository<Memo>().GetAllAsync(
                    orderBy: source => source.OrderByDescending(t => t.CreateDate));

                SummaryDto summary = new SummaryDto();
                summary.Sum = notebooks.Count(); //汇总待办事项数量
                summary.CompletedCount = notebooks.Where(t => t.Status == 1).Count(); //统计完成数量
                summary.CompletedRatio = (summary.CompletedCount / (double)summary.Sum).ToString("0%"); //统计完成率
                summary.MemoCount = memos.Count();  //汇总备忘录数量
                summary.NotebookList = new ObservableCollection<NotebookDto>(Mapper.Map<List<NotebookDto>>(notebooks.Where(t => t.Status == 0)));
                summary.MemoList = new ObservableCollection<MemoDto>(Mapper.Map<List<MemoDto>>(memos));

                return new ApiResponse(true, summary);
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, "");
            }
        }

        public async Task<ApiResponse> UpdateAsync(NotebookDto model)
        {
            try
            {
                var dbNotebook = Mapper.Map<NotebookEntity>(model);
                var repository = work.GetRepository<NotebookEntity>();
                var notebook = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(dbNotebook.Id));

                notebook.Title = dbNotebook.Title;
                notebook.Content = dbNotebook.Content;
                notebook.Status = dbNotebook.Status;
                notebook.UpdateDate = DateTime.Now;

                repository.Update(notebook);

                if (await work.SaveChangesAsync() > 0)
                    return new ApiResponse(true, notebook);
                return new ApiResponse("更新数据异常！");
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
    }
}
