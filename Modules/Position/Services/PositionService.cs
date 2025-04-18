﻿using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Position.DTO;
using ServicePortal.Modules.Position.Interfaces;
using ServicePortal.Modules.Position.Requests;

namespace ServicePortal.Modules.Position.Services
{
    public class PositionService : IPositionService
    {
        private readonly ApplicationDbContext _context;

        public PositionService(ApplicationDbContext context)
        { 
            _context = context;
        }

        public async Task<PositionDTO> Create(PositionDTO dto)
        {
            var exist = await _context.Positions.FirstOrDefaultAsync(e => e.Name == dto.Name);

            if (exist != null)
            {
                throw new ValidationException("Position is exists!");
            }

            _context.Positions.Add(PositionMapper.ToEntity(dto));

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<PagedResults<PositionDTO>> GetAll(GetAllPositionRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Positions.AsQueryable();

            query = query.Where(e => e.PositionLevel != 0);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var positions = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            List<PositionDTO> data = PositionMapper.ToDtoList(positions);
          
            return new PagedResults<PositionDTO>
            {
                Data = data,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<PositionDTO> GetById(int id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            return PositionMapper.ToDto(position);
        }

        public async Task<PositionDTO> Update(int id, PositionDTO dto)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            position.Name = dto.Name;

            position.PositionLevel = dto.PositionLevel;

            _context.Positions.Update(position);

            await _context.SaveChangesAsync();

            return PositionMapper.ToDto(position);
        }

        public async Task<PositionDTO> Delete(int id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            _context.Positions.Remove(position);

            await _context.SaveChangesAsync();

            return PositionMapper.ToDto(position);
        }

        public async Task<PositionDTO> ForceDelete(int id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            _context.Positions.Remove(position);

            await _context.SaveChangesAsync();

            return PositionMapper.ToDto(position);
        }
    }
}
