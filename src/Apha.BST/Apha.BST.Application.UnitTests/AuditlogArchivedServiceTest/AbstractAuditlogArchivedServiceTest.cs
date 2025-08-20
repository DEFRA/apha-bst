using Apha.BST.Application.DTOs;
using Apha.BST.Application.Pagination;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.Core.Pagination;
using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.UnitTests.Audit
{
    public abstract class AbstractAuditlogArchivedServiceTest
    {
        protected IAuditlogArchivedRepository MockRepository { get; }
        protected IMapper MockMapper { get; }
        protected AuditlogArchivedService Service { get; }

        protected AbstractAuditlogArchivedServiceTest()
        {
            MockRepository = Substitute.For<IAuditlogArchivedRepository>();
            MockMapper = Substitute.For<IMapper>();
            Service = new AuditlogArchivedService(MockRepository, MockMapper);
        }

        public void SetupMockMapper(QueryParameters filter, PaginationParameters paginationParameters)
        {
            MockMapper.Map<PaginationParameters>(filter).Returns(paginationParameters);
        }

       public void SetupMockRepository(PaginationParameters paginationParameters, string storedProcedure, PagedData<AuditlogArchived> repositoryResult)
        {
            MockRepository.GetArchiveAuditLogsAsync(paginationParameters, storedProcedure).Returns(Task.FromResult(repositoryResult));
        }

        protected virtual void SetupMockMapperForResult(PagedData<AuditlogArchived> repositoryResult, PaginatedResult<AuditLogArchivedDto> expectedResult)
        {
            MockMapper.Map<PaginatedResult<AuditLogArchivedDto>>(Arg.Is<PagedData<AuditlogArchived>>(p =>
                p.Items.Count == repositoryResult.Items.Count &&
                p.TotalCount == repositoryResult.TotalCount))
            .Returns(expectedResult);
        }

        public void SetupMockMapperToThrowException(PagedData<AuditlogArchived> repositoryResult)
        {
            MockMapper.Map<PaginatedResult<AuditLogArchivedDto>>(repositoryResult).Throws(new AutoMapperMappingException("Mapping error"));
        }

       public void SetupMockRepositoryForStoredProcedureNames(List<string> procedureNames)
        {
            MockRepository.GetStoredProcedureNamesAsync().Returns(Task.FromResult(procedureNames));
        }

        public void SetupMockRepositoryForStoredProcedureNamesToThrowException(Exception exception)
        {
            MockRepository.GetStoredProcedureNamesAsync().Throws(exception);
        }
    }

}
