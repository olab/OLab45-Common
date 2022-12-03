namespace OLabWebAPI.Importer
{
    public interface IImporter
    {
        public bool LoadAll(string archiveFileName);
        public bool SaveAll();
    }
}