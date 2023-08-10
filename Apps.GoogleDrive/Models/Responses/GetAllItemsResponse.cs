using Apps.GoogleDrive.Dtos;

namespace Apps.GoogleDrive.Models.Responses;

public record GetAllItemsResponse(List<ItemsDetailsDto> Items);