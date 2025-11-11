using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;

        public BranchService(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<IEnumerable<BranchDTO>> GetAllAsync()
        {
            var branches = await _branchRepository.ListAllAsync();
            return branches.Select(MapToDto);
        }

        private static BranchDTO MapToDto(Branch branch)
        {
            return new BranchDTO
            {
                BranchId = branch.BranchId,
                Name = branch.Name,
                Address = branch.Address
            };
        }
    }
}
