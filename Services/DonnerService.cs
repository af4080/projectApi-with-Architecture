using projectApiAngular.Models;
using projectApiAngular.Repositories;
using System.Data;
using static projectApiAngular.DTO.DonnerDto;

namespace projectApiAngular.Services
{
    public class DonnerService : IDonnerService
    {
        private readonly IDonnerRepository _donnerRepository;
        private readonly ILogger<DonnerService> _logger;
        public DonnerService(IDonnerRepository donnerRepository, ILogger<DonnerService> logger)
        {
            _donnerRepository = donnerRepository;
            _logger = logger;
        }

        //get
        public async Task<IEnumerable<ReadDonnerDto>> GetAllDonners()
        {
            var donners = await _donnerRepository.GetAllDonners();
            if (donners == null) return Enumerable.Empty<ReadDonnerDto>();
            var dtos = donners.Select(d => new ReadDonnerDto { Name = d.Name, Email = d.Email, Id = d.Id, Phone = d.Phone });
            _logger.LogInformation("Retrieved {Count} donners from the repository.", dtos.Count());
            return dtos;
        }

        //get by id
        public async Task<ReadDonnerDto?> GetDonnerById(int id)
        {
            var d = await _donnerRepository.GetDonnerById(id);
            if (d == null) return null;
            _logger.LogInformation("Retrieved donner with ID {Id} from the repository.", id);
            return new ReadDonnerDto { Id = d.Id, Name = d.Name, Email = d.Email, Phone = d.Phone };
        }

        //get by name
        public async Task<ReadDonnerDto?> GetDonnerByName(string name)
        {
            var d = await _donnerRepository.GetDonnerByName(name);
            if (d == null) return null;
            _logger.LogInformation("Retrieved donner with Name {Name} from the repository.", name);
            return new ReadDonnerDto { Id = d.Id, Name = d.Name, Email = d.Email, Phone = d.Phone };
        }

        //get by email
        public async Task<ReadDonnerDto?> GetDonnerByEmail(string email)
        {
            var d = await _donnerRepository.GetDonnerByEmail(email);
            if (d == null) return null;
            _logger.LogInformation("Retrieved donner with Email {Email} from the repository.", email);
            return new ReadDonnerDto { Id = d.Id, Name = d.Name, Email = d.Email, Phone = d.Phone };
        }

        //get by gift id
        public async Task<ReadDonnerDto?> GetDonnerByGiftId(int giftId)
        {
            var d = await _donnerRepository.GetDonnerByGiftId(giftId);
            if (d == null) return null;
            _logger.LogInformation("Retrieved donner for Gift ID {GiftId} from the repository.", giftId);
            return new ReadDonnerDto { Id = d.Id, Name = d.Name, Email = d.Email, Phone = d.Phone };
        }

        //add donner
        public async Task<ReadDonnerDto> AddDonner(CreateDonnerDto dto)
        {
            _logger.LogInformation("Attempting to add a new donner with Email {Email}.", dto.Email);
            var entity = new Donner
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Gifts = new List<Gift>()
            };

            var created = await _donnerRepository.AddDonner(entity);
            _logger.LogInformation("Added new donner with ID {Id} to the repository.", created.Id);
            return new ReadDonnerDto { Id = created.Id, Name = created.Name, Email = created.Email, Phone = created.Phone };
        }
        //update donner
        public async Task<ReadDonnerDto?> UpdateDonner(int id, UpdateDonnerDto dto)
        {
            var existing= await _donnerRepository.GetDonnerById(id);
            _logger.LogInformation("Attempting to update donner with ID {Id}.", id);
            if (existing == null) return null;
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existingEmail = await _donnerRepository.GetDonnerByEmail(dto.Email);
                if (existingEmail != null && existingEmail.Id != id)
                    throw new InvalidOperationException(
                        $"A donner with the email '{dto.Email}' already exists.");
            }


            existing.Name = dto.Name ?? existing.Name;
            existing.Email = dto.Email ?? existing.Email;
            existing.Phone = dto.Phone ?? existing.Phone;

            var updated = await _donnerRepository.UpdateDonner( existing);
            if (updated == null) return null;
            _logger.LogInformation("Updated donner with ID {Id} in the repository.", id);
            return new ReadDonnerDto { Id = updated.Id, Name = updated.Name, Email = updated.Email, Phone = updated.Phone };
        }
        //delete donner
        public async Task<ReadDonnerDto?> DeleteDonner(int id)
        {
            _logger.LogWarning("Attempting to delete donner with ID {Id}.", id);
            var deleted = await _donnerRepository.DeleteDonner(id);
            if (deleted == null) return null;
            _logger.LogWarning("Deleted donner with ID {Id} from the repository.", id);
            return new ReadDonnerDto { Id = deleted.Id, Name = deleted.Name, Email = deleted.Email, Phone = deleted.Phone };
        }
    }
}
