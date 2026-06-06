namespace RentalHouse.Application.Interfaces;

public interface IFileStorageService
{
    // Chỉ nhận vào Stream thuần túy (C# thuần) và tên file
    Task<string> SaveFileAsync(Stream fileStream, string fileName);
    void DeleteFile(string fileUrl);
}