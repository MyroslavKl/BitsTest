using System.Formats.Asn1;
using System.Globalization;
using System;
using BitsMVCProject.Data;
using BitsMVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BitsMVCProject.Services
{
    public class ContactService : IContactService
    {
        private readonly MVCDbContext _db;
        private readonly ILogger<ContactService> _logger;

        public ContactService(MVCDbContext db, ILogger<ContactService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<ContactModel>> GetAllAsync()
        {
            return await _db.Contacts.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<ContactModel?> GetByIdAsync(int id)
        {
            return await _db.Contacts.FindAsync(id);
        }

        public async Task ImportFromCsvAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("CSV upload attempt failed: file was null or empty.");
                throw new ArgumentException("CSV file is empty or invalid.");
            }

            using var stream = new StreamReader(file.OpenReadStream());
            var contacts = new List<ContactModel>();

            string? headerLine = await stream.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                _logger.LogError("CSV file is missing header row.");
                throw new ArgumentException("CSV file is missing header row.");
            }

            int rowNumber = 1;
            while (!stream.EndOfStream)
            {
                rowNumber++;
                var line = await stream.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                {
                    _logger.LogDebug("Skipping empty line at row {RowNumber}", rowNumber);
                    continue;
                }

                var values = line.Split(',');

                if (values.Length != 5)
                {
                    _logger.LogWarning("Skipping row {RowNumber}: Expected 5 columns but got {Count}.", rowNumber, values.Length);
                    continue;
                }

                try
                {
                    var contact = new ContactModel
                    {
                        Name = values[0].Trim(),
                        DateOfBirth = DateTime.Parse(values[1].Trim(), CultureInfo.InvariantCulture),
                        Married = bool.Parse(values[2].Trim()),
                        Phone = values[3].Trim(),
                        Salary = decimal.Parse(values[4].Trim(), CultureInfo.InvariantCulture)
                    };

                    contacts.Add(contact);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to parse row {RowNumber}: {Line}", rowNumber, line);
                    continue;
                }
            }

            if (contacts.Any())
            {
                await _db.Contacts.AddRangeAsync(contacts);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Successfully imported {Count} contacts from CSV.", contacts.Count);
            }
            else
            {
                _logger.LogWarning("CSV import finished, but no valid contacts were found.");
            }
        }

        public async Task<ContactModel?> UpdateAsync(ContactModel update)
        {
            var existing = await _db.Contacts.FindAsync(update.Id);
            if (existing == null) return null;

            existing.Name = update.Name;
            existing.DateOfBirth = update.DateOfBirth.Date;
            existing.Married = update.Married;
            existing.Phone = update.Phone;
            existing.Salary = update.Salary;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Contacts.FindAsync(id);
            if (existing == null) return false;

            _db.Contacts.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

