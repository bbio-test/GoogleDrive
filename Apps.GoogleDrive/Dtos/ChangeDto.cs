namespace Apps.GoogleDrive.Dtos
{
    public class ChangeDto
    {
        public string ResourceId { get; set; }

        public IEnumerable<string> ParentFolders { get; set; }
    }
}
